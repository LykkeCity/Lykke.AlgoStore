﻿namespace Lykke.AlgoStore.Core.Domain.Errors
{
    public enum AlgoStoreErrorCodes
    {
        None = 0,

        Unhandled = 1,
        InternalError = 2,

        //Data
        RuntimeSettingsExists = 500,
        UnableToDeleteData = 501,
        AlgoNotFound = 502,
        AlgoBinaryDataNotFound = 503,
        AlgoRuntimeDataNotFound = 504,
        PodNotFound = 505,
        AssetNotFound = 505,

        //Validation - 1000
        ValidationError = 1000,
        MoreThanOnePodFound = 1001,

    }
}
