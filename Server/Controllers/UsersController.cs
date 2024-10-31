using BD.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PracticeDatingAppContext _context;

        public UsersController(PracticeDatingAppContext context)
        {
            _context = context;
        }

        // Метод для получения логина пользователя по его ID
        [HttpGet("getUsername/{userId}")]
        public async Task<IActionResult> GetUsername(int userId)
        {
            var user = await _context.Datausers.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            return Ok(user.FirstName);
        }
    }
}
