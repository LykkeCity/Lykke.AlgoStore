﻿using Lykke.AlgoStore.Api.RealTimeStreaming.Stomp.Messages;
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

        private bool _listening;

        private bool _expectClientHeartbeats;
        private bool _expectServerHeartbeats;
        private TimeSpan _clientHeartbeatInterval;
        private TimeSpan _serverHeartbeatInterval;

        private string _version;

        public StompSession(WebSocket webSocket, 
            TimeSpan? connectTimeout = null, TimeSpan? maxHeartbeatTimespan = null)
        {
            _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
            _connectTimeout = connectTimeout ?? TimeSpan.FromSeconds(10);
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
                    //else if (!_authManager.IsAuthenticated())
                    //{
                    //    var message = Encoding.UTF8.GetString(result.Message.ToArray());
                    //    await _authManager.AuthenticateAsync(message);
                    //}
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

        protected async Task<(WebSocketReceiveResult ReceiveResult, IEnumerable<byte> Message)> ReceiveFullMessage(CancellationToken cancelToken)
        {
            WebSocketReceiveResult response;
            var message = new List<byte>();

            var buffer = new byte[Constants.WebSocketRecieveBufferSize];
            do
            {
                response = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancelToken);
                message.AddRange(new ArraySegment<byte>(buffer, 0, response.Count));
            } while (!response.EndOfMessage);

            return (ReceiveResult: response, Message: message);
        }

        private async Task<bool> Handshake()
        {
            var cts = new CancellationTokenSource(_connectTimeout);
            var result = await ReceiveFullMessage(cts.Token);

            if (result.ReceiveResult.MessageType == WebSocketMessageType.Close)
                return false;
            else
            {
                var msg = Message.Deserialize(Encoding.UTF8.GetString(result.Message.ToArray()));

                if (msg == null || !Message.IsClientCommandValid(msg.Command) || msg.Command != "CONNECT")
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
        }

        private async Task SendMessage(Message msg)
        {
            var msgBytes = Encoding.UTF8.GetBytes(msg.Serialize());

            await _webSocket.SendAsync(new ArraySegment<byte>(msgBytes), WebSocketMessageType.Text, true, CancellationToken.None);
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

                return new Header("heart-beat", $"0,0");
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
