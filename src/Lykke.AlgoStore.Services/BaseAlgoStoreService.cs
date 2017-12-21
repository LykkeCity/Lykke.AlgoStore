using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Domain.Errors;

namespace Lykke.AlgoStore.Services
{
    public class BaseAlgoStoreService
    {
        public BaseAlgoStoreService(ILog log, string componentName)
        {
            Log = log;
            ComponentName = componentName;
        }

        protected ILog Log { get; }
        public string ComponentName { get; }

        protected async Task LogTimedInfoAsync(string methodName, string clientId, Func<Task> action)
        {
            if (action == null)
                return;

            var begin = DateTime.UtcNow;
            var hasError = false;

            try
            {
                await action();
            }
            catch (Exception ex)
            {
                hasError = true;
                throw HandleException(ex);
            }
            finally
            {
                var lenght = DateTime.UtcNow - begin;
                Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName, $"Client {clientId} execute {methodName} takes {lenght.TotalMilliseconds}ms with HasError={hasError}").Wait();
            }
        }
        protected async Task<T> LogTimedInfoAsync<T>(string methodName, string clientId, Func<Task<T>> action)
        {
            if (action == null)
                return default(T);

            var begin = DateTime.UtcNow;
            var hasError = false;

            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                hasError = true;
                throw HandleException(ex);
            }
            finally
            {
                var lenght = DateTime.UtcNow - begin;
                Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, ComponentName, $"Client {clientId} execute {methodName} takes {lenght.TotalMilliseconds}ms with HasError={hasError}").Wait();
            }
        }

        protected AlgoStoreException HandleException(Exception ex)
        {
            var exception = ex as AlgoStoreException;

            if (exception == null)
                exception = new AlgoStoreException(AlgoStoreErrorCodes.Unhandled, ex);

            var validationException = exception as AlgoStoreAggregateException;
            if (validationException != null)
                Log.WriteErrorAsync(AlgoStoreConstants.ProcessName, ComponentName, validationException.ToBaseException()).Wait();
            else
                Log.WriteErrorAsync(AlgoStoreConstants.ProcessName, ComponentName, exception).Wait();

            return exception;
        }
    }
}
