﻿namespace Lykke.AlgoStore.Api.RealTimeStreaming
{
    public class Constants
    {
        public const int WebSocketRecieveBufferSize = 1024 * 4;
        public const int WebSocketKeepAliveIntervalSeconds = 5;
        public const string WebSocketErrorMessage = "Error while streaming data to socket";
        public const string InstanceIdIdentifier = "InstanceId";
        public const string LogComponent = "WebSockets";
    }
}
