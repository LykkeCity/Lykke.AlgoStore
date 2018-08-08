using System;
using System.Diagnostics;

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

        public static Header Deserialize(string row)
        {
            if (string.IsNullOrEmpty(row))
                return null;

            var splits = row.Split(':', StringSplitOptions.None);

            if (splits.Length != 2)
                return null;

            var header = new Header
            {
                Key = Utils.UnescapeString(splits[0]),
                Value = Utils.UnescapeString(splits[1])
            };

            if (string.IsNullOrEmpty(header.Key) || string.IsNullOrEmpty(header.Value))
                return null;

            return header;
        }
    }
}
