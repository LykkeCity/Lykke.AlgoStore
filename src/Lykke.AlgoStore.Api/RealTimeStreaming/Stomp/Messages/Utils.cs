using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.Stomp.Messages
{
    public static class Utils
    {
        public static string EscapeString(string str)
        {
            return str.Replace("\\", @"\\")
                      .Replace("\r", @"\r")
                      .Replace("\n", @"\n")
                      .Replace(":", @"\c");
        }

        public static string UnescapeString(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);

            for(var i = 0; i < str.Length; i++)
            {
                if (str[i] == '\\')
                {
                    if (i == str.Length - 1) return null;

                    switch (str[i+1])
                    {
                        case 'r':
                            sb.Append("\r");
                            break;
                        case 'n':
                            sb.Append("\n");
                            break;
                        case 'c':
                            sb.Append(":");
                            break;
                        case '\\':
                            sb.Append("\\");
                            break;
                        default:
                            return null;
                    }

                    i++;
                }
                else
                    sb.Append(str[i]);
            }

            return sb.ToString();
        }

        public static string RemoveEol(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            if (str.EndsWith("\r\n", StringComparison.Ordinal))
                return str.Substring(0, str.Length - 2);
            else if (str.EndsWith("\n", StringComparison.Ordinal))
                return str.Substring(0, str.Length - 1);
            else
                return null;
        }
    }
}
