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

        public static Message Deserialize(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            string removedEol = str;

            while (!string.IsNullOrEmpty((removedEol = Utils.RemoveEol(removedEol))))
                str = removedEol;

            if (!str.EndsWith("\0")) return null;

            var splits = str.Split("\r\n", StringSplitOptions.None)
                            .SelectMany(s => s.Split("\n", StringSplitOptions.None))
                            .ToArray();

            if (splits.Length < 3) return null;

            var command = splits[0];

            if (string.IsNullOrEmpty(command)) return null;

            var headers = new List<Header>();

            for(var i = 1; i < splits.Length - 2; i++)
            {
                var header = Header.Deserialize(splits[i]);

                if (header == null)
                    return null;

                headers.Add(header);
            }

            // Required blank line
            if (!string.IsNullOrEmpty(splits[splits.Length - 2]))
                return null;

            var body = splits[splits.Length - 1];

            return new Message
            {
                Command = command,
                Headers = headers.ToArray(),
                Body = body.Substring(0, body.Length - 1)
            };
        }
    }
}
