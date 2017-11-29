using System;
using System.Threading.Tasks;
using Common.Log;

namespace Lykke.AlgoStore.Tests.Infrastructure
{
    public class LogMock : ILog
    {
        public Task WriteErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null)
        {
            string error = exception == null ? string.Empty : exception.Message;
            Console.WriteLine(string.Format("component:{0}; process:{1}; context:{2}; exception:{3}", component, process, context, error));
            return Task.CompletedTask;
        }

        public Task WriteErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return WriteErrorAsync(string.Empty, process, context, exception, dateTime);
        }

        public Task WriteFatalErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return WriteErrorAsync(component, process, context, exception, dateTime);
        }

        public Task WriteFatalErrorAsync(string process, string context, Exception exception, DateTime? dateTime = null)
        {
            return WriteErrorAsync(string.Empty, process, context, exception, dateTime);
        }

        public Task WriteInfoAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            Console.WriteLine(string.Format("component:{0}; process:{1}; context:{2}; info:{3}", component, process, context, info));
            return Task.CompletedTask;
        }

        public Task WriteInfoAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteInfoAsync(string.Empty, process, context, info, dateTime);
        }

        public Task WriteMonitorAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteInfoAsync(component, process, context, info, dateTime);
        }

        public Task WriteMonitorAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteInfoAsync(string.Empty, process, context, info, dateTime);
        }

        public Task WriteWarningAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteInfoAsync(component, process, context, info, dateTime);
        }

        public Task WriteWarningAsync(string process, string context, string info, DateTime? dateTime = null)
        {
            return WriteInfoAsync(string.Empty, process, context, info, dateTime);
        }
    }
}
