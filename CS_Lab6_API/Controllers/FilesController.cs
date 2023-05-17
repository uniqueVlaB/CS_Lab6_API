using Microsoft.AspNetCore.Mvc;

namespace CS_Lab6_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        [HttpGet, Route("Users/{callerName}")]
        public IActionResult Users([FromRoute]string callerName)
        {
            var dirs = Directory.GetDirectories("Users").Select(d => d.Remove(0, 6)).ToList();
            if (!dirs.Contains(callerName))
            {
                return BadRequest("Wrong caller name");
            }
            else if (dirs.Count < 1) {
                return BadRequest("Cannot find users");
            }
            List<List<string>> usersFiles = new List<List<string>>();

            foreach (var dir in dirs)
            {
                if (dir != callerName && CS_Lab6_API.Users.connected.Contains(dir))
                {
                    List<string> dirFiles = new List<string> { dir };
                    dirFiles.AddRange(Directory.GetFiles($"Users/{dir}/Public").Select(f => f.Remove(0, 14 + dir.Length)));
                    usersFiles.Add(dirFiles);
                }
            }
            return Ok(usersFiles);
        }
        [HttpGet, Route("Public/{callerName}")]
        public IActionResult Public([FromRoute] string callerName)
        {
            if (!Directory.Exists("Users/"+callerName)) return BadRequest("Cannot find user");

            var files = Directory.GetFiles($"Users/{callerName}/Public").Select(f => f.Remove(0, 14 + callerName.Length)).ToArray();
            
            if (files.Length < 1)return BadRequest("Cannot find user files");
            
            return Ok(files);
        }
        [HttpGet, Route("Private/{callerName}")]
        public IActionResult Private([FromRoute] string callerName)
        {
            if (!Directory.Exists("Users/" + callerName)) return BadRequest("Cannot find user");

            var files = Directory.GetFiles($"Users/{callerName}/Private").Select(f => f.Remove(0, 15 + callerName.Length)).ToArray();

            if (files.Length < 1) return BadRequest("Cannot find user files");

            return Ok(files);
        }
        [HttpPut, Route("Add/{callerName}/{privateFlag}/{fileName}")]
        public IActionResult Add([FromRoute] string callerName, bool privateFlag, [FromBody]string content, [FromRoute]string fileName)
        {
            if (!Directory.Exists("Users/" + callerName)) return BadRequest("Cannot find user");
            
                if (privateFlag)
                {
                System.IO.File.Create($"Users/{callerName}/Private/{fileName}.txt").Close();
                System.IO.File.WriteAllText($"Users/{callerName}/Private/{fileName}.txt", content);
            }
                else {
                  System.IO.File.Create($"Users/{callerName}/Public/{fileName}.txt").Close();
                  System.IO.File.WriteAllText($"Users/{callerName}/Public/{fileName}.txt", content);
            }
            
            return Ok();
        }
        [HttpPost, Route("ChangeStatus/{callerName}/{fileName}")]
        public IActionResult ChangeStatus([FromRoute] string callerName, string fileName)
        {
            if (!Directory.Exists("Users/" + callerName)) return BadRequest("Cannot find user");

            if (System.IO.File.Exists($"Users/{callerName}/Private/{fileName}.txt"))
            {
                System.IO.File.Move($"Users/{callerName}/Private/{fileName}.txt", $"Users/{callerName}/Public/{fileName}.txt");
            }
            else
            {
                if (!System.IO.File.Exists($"Users/{callerName}/Public/{fileName}.txt")) return BadRequest("Cannot find file");
                System.IO.File.Move($"Users/{callerName}/Public/{fileName}.txt", $"Users/{callerName}/Private/{fileName}.txt");
            }
            return Ok();
           
        }
        [HttpDelete, Route("Delete/{callerName}/{fileName}")]
        public IActionResult Delete([FromRoute] string callerName, string fileName)
        {

            if (!Directory.Exists("Users/" + callerName)) return BadRequest("Cannot find user");

            if (System.IO.File.Exists($"Users/{callerName}/Private/{fileName}.txt"))
            {
                System.IO.File.Delete($"Users/{callerName}/Private/{fileName}.txt");
            }
            else
            {
                if (!System.IO.File.Exists($"Users/{callerName}/Public/{fileName}.txt")) return BadRequest("Cannot find file");
                System.IO.File.Delete($"Users/{callerName}/Public/{fileName}.txt");
            }
            return Ok();

        }
    }
}
