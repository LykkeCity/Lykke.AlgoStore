using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.Stomp.Messages
{
    [DebuggerDisplay("{Key} : {Value}")]
    public class Header
    {
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
