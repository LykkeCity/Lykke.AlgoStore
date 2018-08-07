using Common.Log;
using Lykke.AlgoStore.Api.RealTimeStreaming.Stomp.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.Stomp
{
    public sealed class StompSession
    {
        // Messages which are supported by this server
        private static readonly HashSet<string> _supportedCommands = new HashSet<string>
        {
            Message.COMMAND_SUBSCRIBE,
            Message.COMMAND_UNSUBSCRIBE,
            Message.COMMAND_ACK,
            Message.COMMAND_NACK,
            Message.COMMAND_DISCONNECT
        };

        // Contains a count of how many concurrent connections each client ID has
        private static Dictionary<string, uint> _clientConnections = new Dictionary<string, uint>();

        private readonly WebSocket _webSocket;
        private readonly ILog _log;
        private readonly TimeSpan _connectTimeout;
        private readonly uint _maxConnectionsPerClient;

        // Dictionaries and hashsets containing all the callbacks and subscriptions
        private readonly Dictionary<string, string> _subscribedQueues = new Dictionary<string, string>();
        private readonly HashSet<Func<string, string, Task<bool>>> _authenticationCallbacks
            = new HashSet<Func<string, string, Task<bool>>>();
        private readonly HashSet<Func<string, Task<bool>>> _subscribeCallbacks = new HashSet<Func<string, Task<bool>>>();
        private readonly HashSet<Func<string, Task>> _unsubscribeCallbacks = new HashSet<Func<string, Task>>();
        private readonly HashSet<Func<Task>> _disconnectCallbacks = new HashSet<Func<Task>>();

        private readonly CancellationTokenSource _taskCancellationSource = new CancellationTokenSource();

        private static readonly object _sync = new object();

        private bool _listening;

        private bool _expectClientHeartbeats;
        private bool _expectServerHeartbeats;

        // Configured intervals for heartbeats and reauthentication
        private TimeSpan _clientHeartbeatInterval;
        private TimeSpan _serverHeartbeatInterval;
        private TimeSpan _reauthenticationInterval;

        // Timestamp of the last message received by the client or sent by the server
        private DateTime _lastClientMessage = DateTime.UtcNow;
        private DateTime _lastServerMessage = DateTime.UtcNow;

        // The three long-running tasks - server heartbeats, client heartbeats and reauthentication
        // which will be stopped once the connection is closed
        private Task _serverHeartbeat;
        private Task _clientHeartbeat;
        private Task _reauthenticationTask;

        // The negotiated version for this connection
        private string _version;

        // The login credentials
        private string _login;
        private string _passcode;

        // Whether the client has successfully authenticated
        private bool _authenticated;

        // Counter for the number of sent messages
        private ulong _currentMessageId;

        public StompSession(WebSocket webSocket,
            ILog log,
            uint maxConnectionsPerClient = 3,
            TimeSpan? connectTimeout = null,
            TimeSpan? reauthenticationInterval = null)
        {
            _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
            _log = log;
            _maxConnectionsPerClient = maxConnectionsPerClient;
            _connectTimeout = connectTimeout ?? TimeSpan.FromSeconds(10);
            _reauthenticationInterval = reauthenticationInterval ?? TimeSpan.FromMinutes(1);
        }

        public void AddAuthenticationCallback(Func<string, string, Task<bool>> callback)
            => AddCallback(_authenticationCallbacks, callback);

        public void RemoveAuthenticationCallback(Func<string, string, Task<bool>> callback)
            => RemoveCallback(_authenticationCallbacks, callback);

        public void AddSubscribeCallback(Func<string, Task<bool>> callback)
            => AddCallback(_subscribeCallbacks, callback);

        public void RemoveSubscribeCallback(Func<string, Task<bool>> callback)
            => RemoveCallback(_subscribeCallbacks, callback);

        public void AddUnsubscribeCallback(Func<string, Task> callback) => AddCallback(_unsubscribeCallbacks, callback);
        public void RemoveUnsubscribeCallback(Func<string, Task> callback) => RemoveCallback(_unsubscribeCallbacks, callback);

        public void AddDisconnectCallback(Func<Task> callback) => AddCallback(_disconnectCallbacks, callback);
        public void RemoveDisconnectCallback(Func<Task> callback) => RemoveCallback(_disconnectCallbacks, callback);

        /// <summary>
        /// Sends a message to a subscribed queue
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="queueName">The name of the queue</param>
        /// <param name="message">The message to send</param>
        /// <returns>Task which completes when the message is sent</returns>
        public async Task SendToQueueAsync<T>(string queueName, T message)
        {
            var msgBody = JsonConvert.SerializeObject(message);

            var msg = new Message
            {
                Command = Message.COMMAND_MESSAGE,
                Body = msgBody,
                Headers = new Header[]
                {
                    new Header(Header.SUBSCRIPTION, _subscribedQueues[queueName]),
                    new Header(Header.DESTINATION, queueName),
                    new Header(Header.CONTENT_TYPE, "application/json"),
                    new Header(Header.CONTENT_LENGTH, msgBody.Length.ToString()),
                    new Header(Header.MESSAGE_ID, _currentMessageId.ToString())
                }
            };

            _currentMessageId++;

            await SendMessage(msg);
        }

        public async Task Listen()
        {
            if (_listening) throw new InvalidOperationException("Already listening to this WebSocket");

            _listening = true;

            try
            {
                // Wait for authentication
                if (!await Handshake())
                    return;

                while (_webSocket.State == WebSocketState.Open)
                {
                    var result = await ReceiveFullMessage(CancellationToken.None);

                    if (result.ReceiveResult.MessageType == WebSocketMessageType.Close) return;

                    await _log.WriteInfoAsync(nameof(StompSession), nameof(Listen),
                        $"Received websocket message: \n{Encoding.UTF8.GetString(result.Message.ToArray())}\n" +
                        $"{BitConverter.ToString(result.Message.ToArray())}");
                    var message = Encoding.UTF8.GetString(result.Message.ToArray());

                    // Heartbeat message
                    if (!string.IsNullOrEmpty(message) && (message == "\n" || message == "\r\n")) continue;

                    var msg = Message.Deserialize(message);

                    if (msg == null || !_supportedCommands.Contains(msg.Command))
                    {
                        await CloseWithError("invalid or unsupported message", "");
                        return;
                    }

                    // Send back a receipt if we receive a message with a receipt header
                    if(msg.HasHeader(Header.RECEIPT))
                    {
                        await SendMessage(new Message
                        {
                            Command = Message.COMMAND_RECEIPT,
                            Headers = new Header[]
                            {
                                new Header(Header.RECEIPT_ID, msg.HeaderDictionary[Header.RECEIPT])
                            },
                            Body = "",
                        });
                    }

                    switch(msg.Command)
                    {
                        case Message.COMMAND_SUBSCRIBE:
                            if (!await HandleSubscribe(msg)) return;
                            break;
                        case Message.COMMAND_UNSUBSCRIBE:
                            if (!await HandleUnsubscribe(msg)) return;
                            break;
                        case Message.COMMAND_DISCONNECT:
                            await Close();
                            return;
                        default:
                            continue;
                    }
                }
            }
            catch(Exception)
            {
                await Close();
                throw;
            }
            finally
            {
                await HandleDisconnect();
            }
        }

        /// <summary>
        /// Sends a heartbeat message based on negotiated heartbeat interval
        /// </summary>
        /// <returns>Task which completes once the websocket has been closed</returns>
        private async Task ServerHeartbeat()
        {
            while(_webSocket.State == WebSocketState.Open)
            {
                var delayTime = _serverHeartbeatInterval - (DateTime.UtcNow - _lastServerMessage);

                try
                {
                    if (delayTime > TimeSpan.Zero)
                        await Task.Delay(delayTime, _taskCancellationSource.Token);
                }
                catch(TaskCanceledException) { return; }

                if (DateTime.UtcNow - _lastServerMessage < _serverHeartbeatInterval)
                    continue;

                if(_webSocket.State == WebSocketState.Open)
                    await SendMessage(Encoding.UTF8.GetBytes("\n"));
            }
        }

        /// <summary>
        /// Verifies a client heartbeat message has been received based on negotiated heartbeat interval
        /// </summary>
        /// <returns>Task which completes once the websocket has been closed</returns>
        private async Task ClientHeartbeat()
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                var delayTime = _clientHeartbeatInterval - (DateTime.UtcNow - _lastClientMessage);

                try
                {
                    if (delayTime > TimeSpan.Zero)
                        await Task.Delay(delayTime, _taskCancellationSource.Token);
                }
                catch(TaskCanceledException) { return; }

                if (DateTime.UtcNow - _lastClientMessage < _clientHeartbeatInterval)
                    continue;

                await CloseWithError($"{Header.HEARTBEAT} expired", "");
            }
        }

        /// <summary>
        /// Periodically verifies that the credentials haven't expired
        /// </summary>
        /// <returns>Task which completes once the websocket has been closed</returns>
        private async Task Reauthenticate()
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    await Task.Delay(_reauthenticationInterval, _taskCancellationSource.Token);
                }
                catch (TaskCanceledException) { return; }

                if (!await TryAuthenticate("session expired"))
                    return;
            }
        }

        private async Task<(WebSocketReceiveResult ReceiveResult, IEnumerable<byte> Message)> ReceiveFullMessage(CancellationToken cancelToken)
        {
            WebSocketReceiveResult response;
            var message = new List<byte>();

            var buffer = new byte[Constants.WebSocketRecieveBufferSize];
            do
            {
                response = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancelToken);
                message.AddRange(new ArraySegment<byte>(buffer, 0, response.Count));
            } while (!response.EndOfMessage);

            _lastClientMessage = DateTime.UtcNow;

            return (ReceiveResult: response, Message: message);
        }

        /// <summary>
        /// Handles authorization and negotiation of settings
        /// </summary>
        /// <returns>A task which upon completion will contain the success flag - true if OK, false if error</returns>
        private async Task<bool> Handshake()
        {
            // Attempt to read connect message within time limit
            var cts = new CancellationTokenSource(_connectTimeout);
            WebSocketReceiveResult result;
            IEnumerable<byte> message;
            try
            {
                (result, message) = await ReceiveFullMessage(cts.Token);
            }
            catch (OperationCanceledException)
            {
                return false;
            }

            if (result.MessageType == WebSocketMessageType.Close)
                return false;
            else
            {
                await _log.WriteInfoAsync(nameof(StompSession), nameof(Handshake),
                    $"Received websocket message: \n{Encoding.UTF8.GetString(message.ToArray())}\n" +
                    $"{BitConverter.ToString(message.ToArray())}");
                var msg = Message.Deserialize(Encoding.UTF8.GetString(message.ToArray()));

                // Message should be either a CONNECT or a STOMP
                if (msg == null || (msg.Command != Message.COMMAND_CONNECT && msg.Command != Message.COMMAND_STOMP))
                {
                    await CloseWithError($"Expected {Message.COMMAND_CONNECT} or {Message.COMMAND_STOMP} message", "");
                    return false;
                }

                // Verify required authentication
                if(!msg.HasHeader(Header.LOGIN) || !msg.HasHeader(Header.PASSCODE))
                {
                    await CloseWithError($"{Header.LOGIN} and {Header.PASSCODE} headers are required", "");
                    return false;
                }

                // Decide on a protocol version
                _version = await NegotiateVersion(msg);
                if (string.IsNullOrEmpty(_version))
                    return false;

                // Negotiate heartbeat settings
                var heartbeatHeader = await NegotiateHeartbeat(msg);
                if (heartbeatHeader == null)
                    return false;

                _login = msg.HeaderDictionary[Header.LOGIN];
                _passcode = msg.HeaderDictionary[Header.PASSCODE];

                // Attempt authentication
                if (!await TryAuthenticate("invalid credentials"))
                    return false;

                bool overConnectionLimit;

                // Validate that we're not over the connection limit for this client ID
                lock (_sync)
                {
                    if (!_clientConnections.ContainsKey(_login))
                        _clientConnections.TryAdd(_login, 0);

                    overConnectionLimit = _clientConnections[_login] >= _maxConnectionsPerClient;

                    if (!overConnectionLimit)
                        _clientConnections[_login] += 1;
                }

                if(overConnectionLimit)
                {
                    await CloseWithError("Maximum connection limit reached", "");
                    return false;
                }

                _authenticated = true;

                // Return a success message to the client
                var response = new Message
                {
                    Command = Message.COMMAND_CONNECTED,
                    Headers = new Header[]
                    {
                        heartbeatHeader,
                        new Header(Header.VERSION, _version)
                    }
                };

                // Start the reauthentication task
                _reauthenticationTask = Reauthenticate();
                await SendMessage(response);

                return true;
            }
        }

        /// <summary>
        /// Sends an ERROR message and closes the connection
        /// </summary>
        /// <param name="reason">Short error message</param>
        /// <param name="description">Error description</param>
        /// <param name="additionalHeaders">Additional headers to attach to the message</param>
        /// <returns>Task which completes once the message has been sent and the connection has been closed</returns>
        private async Task CloseWithError(string reason, string description, Header[] additionalHeaders = null)
        {
            var message = new Message
            {
                Command = Message.COMMAND_ERROR,
                Body = description,
                Headers = new Header[]
                {
                    new Header(Header.CONTENT_TYPE, "text/plain"),
                    new Header(Header.CONTENT_LENGTH, description.Length.ToString()),
                    new Header(Header.MESSAGE, reason)
                }
            };

            if (additionalHeaders != null)
                message.Headers = message.Headers.Concat(additionalHeaders).ToArray();

            await SendMessage(message);
            await Close($"See STOMP {Message.COMMAND_ERROR} frame");
        }

        /// <summary>
        /// Closes the connection
        /// </summary>
        /// <param name="message">Closure reason</param>
        /// <returns>Task which completes once the connection has been closed</returns>
        private async Task Close(string message = "Normal closure")
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                message, CancellationToken.None);

            _taskCancellationSource.Cancel();

            if (_expectClientHeartbeats)
                await _clientHeartbeat;
            if (_expectServerHeartbeats)
                await _serverHeartbeat;
        }

        /// <summary>
        /// Sends a message to the client
        /// </summary>
        /// <param name="msg">The message to send</param>
        /// <returns>Task which completes once the message has been sent</returns>
        private async Task SendMessage(Message msg)
        {
            var msgBytes = Encoding.UTF8.GetBytes(msg.Serialize());

            await SendMessage(msgBytes);
        }

        /// <summary>
        /// Sends a byte array to the client
        /// </summary>
        /// <param name="bytes">The bytes to send</param>
        /// <returns>Task which completes once the byte array has been sent</returns>
        private async Task SendMessage(byte[] bytes)
        {
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            _lastServerMessage = DateTime.UtcNow;
        }

        /// <summary>
        /// Negotiates heartbeat settings with the client
        /// </summary>
        /// <param name="msg">The message containing the heartbeat information</param>
        /// <returns>A Task which once completed will contain the response heartbeat header</returns>
        private async Task<Header> NegotiateHeartbeat(Message msg)
        {
            uint clientHeartbeat;
            uint serverHeartbeat;

            // Check if the message contains a heartbeat header
            if(msg.HasHeader(Header.HEARTBEAT))
            {
                var heartBeatValue = msg.HeaderDictionary[Header.HEARTBEAT];
                var splits = heartBeatValue.Split(',');

                // Validate that the header contains two numbers
                if (splits.Length != 2 ||
                    !uint.TryParse(splits[0], out clientHeartbeat) ||
                    !uint.TryParse(splits[1], out serverHeartbeat))
                {
                    await CloseWithError($"invalid {Header.HEARTBEAT} header", "");
                    return null;
                }

                // Check if we want client and/or server heartbeats
                _expectClientHeartbeats = clientHeartbeat > 0;
                _expectServerHeartbeats = serverHeartbeat > 0;

                // Allow for client network delay here
                _clientHeartbeatInterval = TimeSpan.FromMilliseconds(clientHeartbeat + 1000);
                _serverHeartbeatInterval = TimeSpan.FromMilliseconds(serverHeartbeat);

                // Start each task based on the request
                if (_expectClientHeartbeats)
                    _clientHeartbeat = ClientHeartbeat();

                if (_expectServerHeartbeats)
                    _serverHeartbeat = ServerHeartbeat();

                return new Header(Header.HEARTBEAT,
                    $"{serverHeartbeat},{clientHeartbeat}");
            }
            else
            {
                // We don't have a heartbeat header - we won't have heartbeats
                _expectServerHeartbeats = false;
                _expectClientHeartbeats = false;

                return new Header(Header.HEARTBEAT, "0,0");
            }
        }

        /// <summary>
        /// Negotiates the version to use for communication
        /// </summary>
        /// <param name="msg">The message containing the accepted versions</param>
        /// <returns>A task which once completed will contain the negotiated version, null if no version was acceptable</returns>
        private async Task<string> NegotiateVersion(Message msg)
        {
            // The accept version header is required
            if(!msg.HasHeader(Header.ACCEPT_VERSION))
            {
                await CloseWithError($"{Header.ACCEPT_VERSION} header required", "");
                return "";
            }

            // Get all client supported versions
            var msgHeader = msg.HeaderDictionary[Header.ACCEPT_VERSION].Split(',');

            // Check if we have any version in common
            if (msgHeader.Any(s => s == StompVersion.VERSION_12))
                return StompVersion.VERSION_12;
            else if (msgHeader.Any(s => s == StompVersion.VERSION_11))
                return StompVersion.VERSION_12;

            // If not, send an error containing the versions supported by the server
            await CloseWithError("unsupported version", "", 
                new Header[] { new Header(Header.VERSION, $"{StompVersion.VERSION_11},{StompVersion.VERSION_12}") });
            return "";
        }

        /// <summary>
        /// Handles a SUBSCRIBE message sent from the client
        /// </summary>
        /// <param name="msg">The message to process</param>
        /// <returns>A task which once completed will contain a flag indicating the successs of the subscription request</returns>
        private async Task<bool> HandleSubscribe(Message msg)
        {
            // Subscription ID and destination are required headers
            if (!msg.HasHeader(Header.SUBSCRIPTION_ID) || !msg.HasHeader(Header.DESTINATION))
            {
                await CloseWithError(
                    $"{Header.SUBSCRIPTION_ID} and {Header.DESTINATION} headers are required", "");
                return false;
            }

            // Subscription ID - unique ID identifying the subscription, Destination - queue identifier
            var subscriptionId = msg.HeaderDictionary[Header.SUBSCRIPTION_ID];
            var queue = msg.HeaderDictionary[Header.DESTINATION];

            // If we are already subscribed to this queue - return error
            if (_subscribedQueues.ContainsKey(queue))
            {
                await CloseWithError("already subscribed to queue", "");
                return false;
            }

            // Check if any of the handlers can accept the queue
            var callbackSuccess = false;

            foreach (var callback in _subscribeCallbacks)
            {
                callbackSuccess |= await callback(queue);
            }

            if (!callbackSuccess)
            {
                await CloseWithError("invalid queue", "");
                return false;
            }

            _subscribedQueues.Add(queue, subscriptionId);

            return true;
        }

        /// <summary>
        /// Handles an UNSUBSCRIBE message from the client
        /// </summary>
        /// <param name="msg">The message to process</param>
        /// <returns>A task which once completed will contain a flag indicating the successs of the unsubscription request</returns>
        private async Task<bool> HandleUnsubscribe(Message msg)
        {
            // Subscription ID - required header
            if (!msg.HasHeader(Header.SUBSCRIPTION_ID))
            {
                await CloseWithError($"{Header.SUBSCRIPTION_ID} header is required", "");
                return false;
            }

            // Check if we have information about such a subscription
            var subscriptionId = msg.HeaderDictionary[Header.SUBSCRIPTION_ID];
            var kvp = _subscribedQueues.FirstOrDefault(k => k.Value == subscriptionId);

            if (kvp.Equals(default(KeyValuePair<string, string>)))
            {
                await CloseWithError("unknown subscription id", "");
                return false;
            }

            _subscribedQueues.Remove(kvp.Key);
            await Task.WhenAll(_unsubscribeCallbacks.Select(c => c(kvp.Key)).ToArray());
            return true;
        }

        /// <summary>
        /// Handles a DISCONNECT message sent by the client
        /// </summary>
        /// <returns>Task which will complete once the disconnect has been handled</returns>
        private async Task HandleDisconnect()
        {
            // Unsubscribe from every queue before disconnecting
            foreach (var subscription in _subscribedQueues)
            {
                await Task.WhenAll(_unsubscribeCallbacks.Select(c => c(subscription.Key)).ToArray());
            }

            // Decrement the user connection count
            lock(_sync)
            {
                if(_authenticated && _clientConnections.ContainsKey(_login))
                {
                    _clientConnections[_login] -= 1;

                    if (_clientConnections[_login] == 0)
                        _clientConnections.Remove(_login);
                }
            }

            // Run all of the disconnection callbacks
            await Task.WhenAll(_disconnectCallbacks.Select(c => c()).ToArray());
        }

        /// <summary>
        /// Attempts an authentication given the client ID and token
        /// </summary>
        /// <param name="errorMessage">The error message to send if the authentication fails</param>
        /// <returns>A task which once completed will contain a flag indicating the success of the authentication</returns>
        private async Task<bool> TryAuthenticate(string errorMessage)
        {
            var callbackSuccess = false;

            foreach (var callback in _authenticationCallbacks)
            {
                callbackSuccess |= await callback(_login, _passcode);
            }

            if (!callbackSuccess)
            {
                await CloseWithError(errorMessage, "");
                return false;
            }

            return true;
        }

        private void AddCallback<T>(HashSet<T> callbackSet, T callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callbackSet.Add(callback);
        }

        private void RemoveCallback<T>(HashSet<T> callbackSet, T callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callbackSet.Remove(callback);
        }
    }
}
