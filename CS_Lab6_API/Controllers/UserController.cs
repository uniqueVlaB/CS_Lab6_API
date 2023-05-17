using Microsoft.AspNetCore.Mvc;

namespace CS_Lab6_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpPut, Route("Connect/{callerName}")]
        public IActionResult Connect([FromRoute]string callerName)
        {
            if (!Directory.Exists($"Users/{callerName}")){
                Directory.CreateDirectory($"Users/{callerName}");
                Directory.CreateDirectory($"Users/{callerName}/Public");
                Directory.CreateDirectory($"Users/{callerName}/Private");
            }
            if (!Users.connected.Contains(callerName))
            {
                Users.connected.Add(callerName);
                return Ok();
            }
            return BadRequest("User already connected");
        }
        [HttpPost, Route("Disconnect/{callerName}")]
        public IActionResult Disconnect([FromRoute] string callerName)
        {
            if (Users.connected.Contains(callerName))
            {
                Users.connected.Remove(callerName);
                return Ok();
            }
            return BadRequest("User wasn`t connected");
        }
    }
}
