namespace Lykke.AlgoStore.Core.Domain.Errors
{
    public enum AlgoStoreErrorCodes
    {
        None = 0,

        Unhandled = 1,

        //Data
        RuntimeSettingsExists = 500,
        UnableToDeleteData = 501,

        //Validation - 1000


    }
}
