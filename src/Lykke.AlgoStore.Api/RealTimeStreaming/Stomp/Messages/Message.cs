using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.Stomp.Messages
{
    public class Message
    {
        public const string COMMAND_CONNECT = "CONNECT";
        public const string COMMAND_STOMP = "STOMP";
        public const string COMMAND_DISCONNECT = "DISCONNECT";
        public const string COMMAND_SUBSCRIBE = "SUBSCRIBE";
        public const string COMMAND_UNSUBSCRIBE = "UNSUBSCRIBE";
        public const string COMMAND_ACK = "ACK";
        public const string COMMAND_NACK = "NACK";

        public const string COMMAND_CONNECTED = "CONNECTED";
        public const string COMMAND_MESSAGE = "MESSAGE";
        public const string COMMAND_RECEIPT = "RECEIPT";
        public const string COMMAND_ERROR = "ERROR";

        private Header[] _headers = new Header[0];

        public string Command { get; set; }
        public Header[] Headers
        {
            get => _headers;
            set
            {
                _headers = value;
                HeaderDictionary = _headers.ToDictionary(h => h.Key, h => h.Value);
            }
        }
        public Dictionary<string, string> HeaderDictionary { get; private set; } 
            = new Dictionary<string, string>();
        public string Body { get; set; }

        public bool HasHeader(string header)
        {
            return HeaderDictionary.ContainsKey(header);
        }

        public string Serialize()
        {
            var sb = new StringBuilder();

            sb.Append(Command);
            sb.Append("\n");
            
            foreach (var header in Headers)
                sb.Append(header.Serialize());

            sb.Append("\n");
            sb.Append(Body);
            sb.Append("\0");

            return sb.ToString();
        }

        public static Message Deserialize(string str, out string debugStr)
        {
            debugStr = "";

            if (string.IsNullOrEmpty(str))
            {
                debugStr += "input str was null or empty\n";
                return null;
            }

            string removedEol = str;

            debugStr += "input str before removing EOL: " + 
                str.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\0", "\\0") + "\n";

            while (!string.IsNullOrEmpty((removedEol = Utils.RemoveEol(removedEol))))
                str = removedEol;

            debugStr += "input str after removing EOL: " +
                str.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\0", "\\0") + "\n";

            if (!str.EndsWith("\0"))
            {
                debugStr += "input str did not end with \\0\n";
                return null;
            }

            var splits = str.Split("\r\n", StringSplitOptions.None)
                            .SelectMany(s => s.Split("\n", StringSplitOptions.None))
                            .ToArray();

            debugStr += "splits: " +
                string.Join(" ", splits.Select(s => ("[" + s.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\0", "\\0") + "]")).ToArray()) + "\n";

            if (splits.Length < 3)
            {
                debugStr += "splits were less than 3\n";
                return null;
            }

            var command = splits[0];

            debugStr += "command: " + command + "\n";
            if (string.IsNullOrEmpty(command))
            {
                debugStr += "command was null or empty\n";
                return null;
            }

            var headers = new List<Header>();

            for(var i = 1; i < splits.Length - 2; i++)
            {
                var header = Header.Deserialize(splits[i], out var headerDebug);

                debugStr += "header: \n" + headerDebug + "\n";

                if (header == null)
                    return null;

                headers.Add(header);
            }

            // Required blank line
            if (!string.IsNullOrEmpty(splits[splits.Length - 2]))
            {
                debugStr += $"required blank line was not blank: {splits[splits.Length - 2]}\n";
                return null;
            }

            var body = splits[splits.Length - 1];

            debugStr += "body: " + body.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\0", "\\0") + "\n";

            return new Message
            {
                Command = command,
                Headers = headers.ToArray(),
                Body = body.Substring(0, body.Length - 1)
            };
        }
    }
}
