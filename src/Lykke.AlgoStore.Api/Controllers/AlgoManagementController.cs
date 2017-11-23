using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.AlgoStore.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AlgoManagementController : Controller
    {
    }
}
