using System;
using System.Diagnostics;
using System.Linq;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.Stomp.Messages
{
    [DebuggerDisplay("{Key} : {Value}")]
    public class Header
    {
        public const string LOGIN = "login";
        public const string PASSCODE = "passcode";
        public const string ACCEPT_VERSION = "accept-version";
        public const string VERSION = "version";
        public const string HEARTBEAT = "heart-beat";

        public const string MESSAGE = "message";

        public const string CONTENT_TYPE = "content-type";
        public const string CONTENT_LENGTH = "content-length";

        public const string SUBSCRIPTION_ID = "id";
        public const string DESTINATION = "destination";

        public const string RECEIPT = "receipt";
        public const string RECEIPT_ID = "receipt-id";

        public const string SUBSCRIPTION = "subscription";
        public const string MESSAGE_ID = "message-id";

        public string Key { get; set; }
        public string Value { get; set; }

        public Header()
        {

        }

        public Header(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Serialize()
        {
            return $"{Utils.EscapeString(Key)}:{Utils.EscapeString(Value)}\n";
        }

        public static Header Deserialize(string row, out string debugStr)
        {
            debugStr = "";

            if (string.IsNullOrEmpty(row))
            {
                debugStr += "    row was null or empty\n";
                return null;
            }

            var splits = row.Split(':', StringSplitOptions.None);

            debugStr += "    splits: " +
                string.Join(" ", splits.Select(s => ("[" + s.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\0", "\\0") + "]")).ToArray()) + "\n";

            if (splits.Length != 2)
            {
                debugStr += "    splits were not 2\n";
                return null;
            }

            var header = new Header
            {
                Key = Utils.UnescapeString(splits[0]),
                Value = Utils.UnescapeString(splits[1])
            };

            debugStr += "    escaped header key: " + header.Key.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\0", "\\0")
                + "; escaped header value" + header.Value.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\0", "\\0") + "\n";

            if (string.IsNullOrEmpty(header.Key) || string.IsNullOrEmpty(header.Value))
            {
                debugStr += "    header key or value was null or empty\n";
                return null;
            }

            return header;
        }
    }
}
