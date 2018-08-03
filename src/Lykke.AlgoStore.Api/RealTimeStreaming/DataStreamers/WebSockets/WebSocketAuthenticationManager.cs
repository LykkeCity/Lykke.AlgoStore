using Common.Log;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Handlers;
using Lykke.Common.Log;
using Lykke.Service.Session;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets
{
    public class WebSocketAuthenticationManager
    {
        private readonly IClientSessionsClient _clientSessionsClient;
        private bool _isAuthenticated = false;
        private IWebSocketHandler _webSocketHandler;
        private ILog Log;

        public WebSocketAuthenticationManager(IClientSessionsClient clientSessionsClient, ILogFactory log)
        {
            _clientSessionsClient = clientSessionsClient;
            Log = log.CreateLog(Constants.LogComponent);
        }

        public void StartSession(IWebSocketHandler webSocketHandler)
        {
            _webSocketHandler = webSocketHandler;
            _isAuthenticated = false;
        }

        public bool IsAuthenticated()
        {
            return _isAuthenticated;
        }

        public async Task<bool> AuthenticateAsync(string clientId, string token)
        {
            if (!IsAuthenticated())
            {
                var session = await _clientSessionsClient.GetAsync(token);
                if (session != null && session.ClientId == clientId)
                {
                    _isAuthenticated = true;
                    Log.Info(nameof(WebSocketAuthenticationManager), 
                        $"Successful websocket authentication for clientId {clientId}." +
                        $" {_webSocketHandler.GetType().Name}", "AuthenticateOK");
                    return true;
                }

                return false;
            }

            return true;
        }
    }
}
