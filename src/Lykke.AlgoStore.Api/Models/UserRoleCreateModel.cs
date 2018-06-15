namespace Lykke.AlgoStore.Api.Models
{
    public class UserRoleCreateModel
    {
        public string Name { get; set; }
        public bool CanBeModified { get; set; }
        public bool CanBeDeleted { get; set; }
    }
}
