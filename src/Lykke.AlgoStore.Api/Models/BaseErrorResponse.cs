namespace Lykke.AlgoStore.Api.Models
{
    public class BaseErrorResponse
    {
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorMessage { get; set; }
    }
}
