﻿namespace Lykke.AlgoStore.Core.Constants
{
    public static class AlgoStoreConstants
    {
        public const string ProcessName = "AlgoStore";
        public const string LogTableName = "AlgoStoreApiLog";

        public const string ApiVersion = "v1";
        public const string AppName = "Lykke Algo API v0.1";

        public const string DefaultDisplayMessage = "There was a problem, please contact administrator";

        public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        public const string CustomDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public const string AdminRoleName = "Admin";
        public const string UserRoleName = "User";

        public const int RunningAlgoInstancesCountLimit = 3;
    }
}
