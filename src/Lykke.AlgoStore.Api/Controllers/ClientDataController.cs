using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.AlgoStore.Controllers
{
    [Authorize]
    [Route("api/ClientData")]
    public class ClientDataController : Controller
    {
    }
}
