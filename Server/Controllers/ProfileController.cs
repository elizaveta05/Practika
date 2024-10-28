using BD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Model.Model_users;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        
        private readonly PracticeDatingAppContext _context;

        public ProfileController(PracticeDatingAppContext context)
        {
            _context = context;
        }

        // Вывод информации о пользователе
        [HttpPost("getProfileUser")]
        public async Task<IActionResult> getProfileUser([FromBody] int userId)
        {
            //Проверяем id
            if (userId.Equals(null))
            {
                return Unauthorized(new { message = "Ошибка!" });
            }

            // Ищем пользователя по логину
            var user = await _context.Datausers.FirstOrDefaultAsync(u => u.UserId == userId);

            //Проверяем находится ли пользователь
            if (user == null)
            {
                return Unauthorized(new { message = "Данные о пользователе не найдены!" });
            }


            return Ok();
        }
        
    }
}
