namespace Lykke.AlgoStore.Core.Domain.Errors
{
    public enum AlgoStoreErrorCodes
    {
        None = 0,

        Unhandled = 1,
        InternalError = 2,

        //Data
        RuntimeSettingsExists = 500,
        AlgoNotFound = 501,
        AlgoBinaryDataNotFound = 502,
        AlgoRuntimeDataNotFound = 503,
        PodNotFound = 504,
        AssetNotFound = 505,
        AlgoInstanceDataNotFound = 506,
        MoreThanOnePodFound = 507,
        AlgoNotPublic = 508,
        WalletNotFound = 509,
        WalletIsAlreadyUsed = 510,
        InitialWalletBalanceNotCalculated = 511,
        StatisticsSumaryNotFound = 512,
        //Operation
        UnableToDeleteData = 601,

        //Validation - 1000
        ValidationError = 1000,

    }
}
