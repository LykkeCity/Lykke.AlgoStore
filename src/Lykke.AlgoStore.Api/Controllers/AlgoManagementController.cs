using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.AlgoStore.Controllers
{
    [Authorize]
    [Route("api/AlgoManagement")]
    public class AlgoManagementController : Controller
    {
    }
}
