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
        private Timer _timer;
        private ILog Log;
        private readonly int UNAUTHORIZED_TIME_ALLOWANCE_SECONDS = 10;
        public readonly string UNAUTHORIZED_MESSAGE = "Unauthorized";

        public WebSocketAuthenticationManager(IClientSessionsClient clientSessionsClient, ILogFactory log)
        {
            _clientSessionsClient = clientSessionsClient;
            Log = log.CreateLog(Constants.LogComponent);
        }

        public void StartSession(IWebSocketHandler webSocketHandler)
        {
            _webSocketHandler = webSocketHandler;
            _isAuthenticated = false;
            _timer = new Timer(UNAUTHORIZED_TIME_ALLOWANCE_SECONDS * 1000)
            {
                Enabled = true,
                AutoReset = false
            };
            _timer.Elapsed += (sender, args) =>
            {
                if (!IsAuthenticated())
                {
                    //_webSocketHandler.OnDisconnected(new WebSocketException(UNAUTHORIZED_MESSAGE));
                }
                CancelTimer(); 
            };
        }

        public bool IsAuthenticated()
        {
            return _isAuthenticated;
        }

        public async Task AuthenticateAsync(string authString)
        {
            if (!IsAuthenticated())
            {
                if (new Regex(@"^(Token:[a-zA-Z\d-]+)(_)(ClientId:.*)").IsMatch(authString))
                {
                    var tokenParsed = authString.Split("_")[0].Replace("Token:", "");
                    var clientIdParsed = authString.Split("_")[1].Replace("ClientId:", "");

                    var session = await _clientSessionsClient.GetAsync(tokenParsed);
                    if (session != null && session.ClientId == clientIdParsed)
                    {
                        _isAuthenticated = true;
                        Log.Info(nameof(WebSocketAuthenticationManager), $"Successful websocket authentication for cleintId {clientIdParsed}. {_webSocketHandler.GetType().Name}", "AuthenticateOK");
                        CancelTimer();
                    }
                    else
                    {
                        await _webSocketHandler.OnDisconnected(new WebSocketException(UNAUTHORIZED_MESSAGE));
                    }
                }
            }
        }

        private void CancelTimer()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
