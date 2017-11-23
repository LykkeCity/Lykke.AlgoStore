namespace Lykke.AlgoStore.Api.Infrastructure.Context
{
    public interface IRequestContext
    {
        string GetClientId();
        string GetPartnerId();
    }
}
