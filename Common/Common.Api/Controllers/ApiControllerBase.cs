using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SevenStar.ApiService.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]/[action]")]
public class ApiControllerBase : ControllerBase
{
}