using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Charting;
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
        private readonly WebSocket _webSocket;
        private readonly TimeSpan _connectTimeout;

        private readonly Dictionary<string, string> _subscribedQueues = new Dictionary<string, string>();
        private readonly HashSet<Func<string, string, Task<bool>>> _authenticationCallbacks
            = new HashSet<Func<string, string, Task<bool>>>();
        private readonly HashSet<Func<string, Task<bool>>> _subscriptionCallbacks = new HashSet<Func<string, Task<bool>>>();

        private bool _listening;

        private bool _expectClientHeartbeats;
        private bool _expectServerHeartbeats;

        private TimeSpan _clientHeartbeatInterval;
        private TimeSpan _serverHeartbeatInterval;

        private DateTime _lastClientMessage = DateTime.UtcNow;
        private DateTime _lastServerMessage = DateTime.UtcNow;

        private Task _serverHeartbeat;
        private Task _clientHeartbeat;

        private string _version;

        private string _subscribed;

        public StompSession(WebSocket webSocket, 
            TimeSpan? connectTimeout = null, TimeSpan? maxHeartbeatTimespan = null)
        {
            _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
            _connectTimeout = connectTimeout ?? TimeSpan.FromSeconds(10);
        }

        public void AddAuthenticationCallback(Func<string, string, Task<bool>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _authenticationCallbacks.Add(callback);
        }

        public void RemoveAuthenticationCallback(Func<string, string, Task<bool>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _authenticationCallbacks.Remove(callback);
        }

        public void AddSubscriptionCallback(Func<string, Task<bool>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _subscriptionCallbacks.Add(callback);
        }

        public void RemoveSubscriptionCallback(Func<string, Task<bool>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _subscriptionCallbacks.Remove(callback);
        }

        public async Task Listen()
        {
            if (_listening) throw new InvalidOperationException("Already listening to this WebSocket");

            if (!await Handshake())
                return;

            try
            {
                while (_webSocket.State == WebSocketState.Open)
                {
                    var result = await ReceiveFullMessage(CancellationToken.None);

                    if (result.ReceiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        //await OnDisconnected();
                    }
                    else
                    {
                        var message = Encoding.UTF8.GetString(result.Message.ToArray());

                        // Heartbeat message
                        if (!string.IsNullOrEmpty(message) && (message == "\n" || message == "\r\n")) continue;

                        var msg = Message.Deserialize(message);

                        if (msg.Command == "SUBSCRIBE")
                        {
                            if (!msg.HasHeader("id") || !msg.HasHeader("destination"))
                            {
                                await CloseWithError("id and destination headers are required", "");
                                return;
                            }

                            var subscriptionId = msg.HeaderDictionary["id"];
                            var queue = msg.HeaderDictionary["destination"];

                            if (queue == "dummy")
                            {
                                _subscribed = subscriptionId;
                                BeginSending();
                                continue;
                            }

                            var callbackSuccess = false;

                            foreach (var callback in _subscriptionCallbacks)
                            {
                                callbackSuccess |= await callback(queue);
                            }

                            if (!callbackSuccess)
                            {
                                await CloseWithError("invalid queue", "");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //await OnDisconnected(ex);
            }
            finally
            {
                //DataListener.TokenSource.Cancel();
            }
        }

        private async void BeginSending()
        {
            var msgId = 1;

            while(_webSocket.State == WebSocketState.Open)
            {
                string msg;
                var instanceId = Guid.NewGuid();

                if (msgId % 3 == 0)
                    msg = JsonConvert.SerializeObject(new CandleChartingUpdate
                    {
                        DateTime = DateTime.UtcNow,
                    });
                else if (msgId % 3 == 1)
                    msg = JsonConvert.SerializeObject(new FunctionChartingUpdate
                    {
                        CalculatedOn = DateTime.UtcNow,
                        FunctionName = "asdf",
                        InnerFunctions = new List<FunctionChartingUpdate>(),
                        InstanceId = instanceId.ToString(),
                        Value = 69
                    });
                else
                    msg = JsonConvert.SerializeObject(new TradeChartingUpdate
                    {
                        AssetId = "BTC",
                        Amount = 1,
                        AssetPairId = "BTCUSD",
                        DateOfTrade = DateTime.UtcNow,
                        InstanceId = instanceId.ToString(),
                        Id = Guid.NewGuid().ToString(),
                        OrderId = Guid.NewGuid().ToString(),
                        Fee = 0.123,
                        IsBuy = false,
                        Price = 1234,
                        WalletId = Guid.NewGuid().ToString()
                    });

                var dummyMsg = new Message
                {
                    Command = "MESSAGE",
                    Body = msg,
                    Headers = new Header[]
                    {
                        new Header("subscription", _subscribed),
                        new Header("destination", "dummy"),
                        new Header("content-type", "application/json"),
                        new Header("content-length", msg.Length.ToString()),
                        new Header("message-id", msgId.ToString())
                    }
                };

                try
                {
                    await SendMessage(dummyMsg);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }

                msgId++;

                await Task.Delay(1000);
            }
        }

        private async Task ServerHeartbeat()
        {
            while(_webSocket.State == WebSocketState.Open)
            {
                var delayTime = _serverHeartbeatInterval - (DateTime.UtcNow - _lastServerMessage);

                if (delayTime > TimeSpan.Zero)
                    await Task.Delay(delayTime);

                if (DateTime.UtcNow - _lastServerMessage < _serverHeartbeatInterval)
                    continue;

                if(_webSocket.State == WebSocketState.Open)
                    await SendMessage(Encoding.UTF8.GetBytes("\n"));
            }
        }

        private async Task ClientHeartbeat()
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                var delayTime = _clientHeartbeatInterval - (DateTime.UtcNow - _lastClientMessage);

                if (delayTime > TimeSpan.Zero)
                    await Task.Delay(delayTime);

                if (DateTime.UtcNow - _lastClientMessage < _clientHeartbeatInterval)
                    continue;

                await CloseWithError("heart-beat expired", "");
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

        private async Task<bool> Handshake()
        {
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
                var msg = Message.Deserialize(Encoding.UTF8.GetString(message.ToArray()));

                if (msg == null || !Message.IsClientCommandValid(msg.Command) || (msg.Command != "CONNECT" && msg.Command != "STOMP"))
                {
                    await CloseWithError("Expected CONNECT message", "");
                    return false;
                }

                if(!msg.HasHeader("login") || !msg.HasHeader("passcode"))
                {
                    await CloseWithError("login and passcode headers are required", "");
                    return false;
                }

                _version = await NegotiateVersion(msg);
                if (string.IsNullOrEmpty(_version))
                    return false;

                var heartbeatHeader = await NegotiateHeartbeat(msg);
                if (heartbeatHeader == null)
                    return false;

                var callbackSuccess = false;

                foreach(var callback in _authenticationCallbacks)
                {
                    callbackSuccess |= await callback(msg.HeaderDictionary["login"], msg.HeaderDictionary["passcode"]);
                }

                if (!callbackSuccess)
                {
                    await CloseWithError("invalid credentials", "");
                    return false;
                }

                var response = new Message
                {
                    Command = "CONNECTED",
                    Headers = new Header[]
                    {
                        heartbeatHeader,
                        new Header("version", _version)
                    }
                };

                await SendMessage(response);

                return true;
            }
        }

        private async Task CloseWithError(string reason, string description, Header[] additionalHeaders = null)
        {
            var message = new Message
            {
                Command = "ERROR",
                Body = description,
                Headers = new Header[]
                {
                    new Header("content-type", "text/plain"),
                    new Header("content-length", description.Length.ToString()),
                    new Header("message", reason)
                }
            };

            if (additionalHeaders != null)
                message.Headers = message.Headers.Concat(additionalHeaders).ToArray();

            await SendMessage(message);
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "See STOMP ERROR frame", CancellationToken.None);
            if (_expectClientHeartbeats)
                await _clientHeartbeat;
            if (_expectServerHeartbeats)
                await _serverHeartbeat;
        }

        private async Task SendMessage(Message msg)
        {
            var msgBytes = Encoding.UTF8.GetBytes(msg.Serialize());

            await SendMessage(msgBytes);
        }

        private async Task SendMessage(byte[] bytes)
        {
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            _lastServerMessage = DateTime.UtcNow;
        }

        private async Task<Header> NegotiateHeartbeat(Message msg)
        {
            uint clientHeartbeat;
            uint serverHeartbeat;

            if(msg.HasHeader("heart-beat"))
            {
                var heartBeatValue = msg.HeaderDictionary["heart-beat"];
                var splits = heartBeatValue.Split(',');

                if (splits.Length != 2 ||
                    !uint.TryParse(splits[0], out clientHeartbeat) ||
                    !uint.TryParse(splits[1], out serverHeartbeat))
                {
                    await CloseWithError("invalid heart-beat header", "");
                    return null;
                }

                _expectClientHeartbeats = clientHeartbeat > 0;
                _expectServerHeartbeats = serverHeartbeat > 0;

                // Allow for client network delay here
                _clientHeartbeatInterval = TimeSpan.FromMilliseconds(clientHeartbeat + 1000);
                _serverHeartbeatInterval = TimeSpan.FromMilliseconds(serverHeartbeat);

                if (_expectClientHeartbeats)
                    _clientHeartbeat = ClientHeartbeat();

                if (_expectServerHeartbeats)
                    _serverHeartbeat = ServerHeartbeat();

                return new Header("heart-beat",
                    $"{serverHeartbeat},{clientHeartbeat}");
            }
            else
            {
                _expectServerHeartbeats = false;
                _expectClientHeartbeats = false;

                return new Header("heart-beat", "0,0");
            }
        }

        private async Task<string> NegotiateVersion(Message msg)
        {
            if(!msg.HasHeader("accept-version"))
            {
                await CloseWithError("accept-version header required", "");
                return "";
            }

            var msgHeader = msg.HeaderDictionary["accept-version"].Split(',');

            if (msgHeader.Any(s => s == "1.2"))
                return "1.2";
            else if (msgHeader.Any(s => s == "1.1"))
                return "1.1";

            await CloseWithError("unsupported version", "", new Header[] { new Header("version", "1.1,1.2") });
            return "";
        }
    }
}
