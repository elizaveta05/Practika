using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BD.Models;
using Server.Model.Model_users;
using BCrypt.Net;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly PracticeDatingAppContext _context;
        public AuthorizationController(PracticeDatingAppContext context)
        {
            _context = context;
        }

        // Авторизация пользователя
        [HttpPost("authorization")]
        public async Task<IActionResult> Authorization([FromBody] UserDataRegister user)
        {
            // Ищем пользователя по логину
            var existingUser = await _context.Userdataregisters.FirstOrDefaultAsync(u => u.Login == user.Login);
            if (existingUser == null)
            {
                return Unauthorized(new { message = "Неправильный логин!" });
            }

            // Проверяем пароль
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Неправильный пароль!" });
            }

            // Успешная авторизация, возвращаем id пользователя
            return Ok(new { userId = existingUser.UdrId });
        }
    }
}
