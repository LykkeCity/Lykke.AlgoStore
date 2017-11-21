using System;

namespace Lykke.AlgoStore.Infrastructure
{
    internal class ExceptionManager
    {
        private static readonly ExceptionManager _instance = new ExceptionManager();
        public static ExceptionManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public object CreateErrorResponse(Exception ex)
        {
            return ex;
        }
    }
}
