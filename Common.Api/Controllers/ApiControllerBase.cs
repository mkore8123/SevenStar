using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SevenStar.ApiService.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ApiControllerBase : ControllerBase
{
}