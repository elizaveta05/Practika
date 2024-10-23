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
    public class AuthoController : ControllerBase
    {
        private readonly PracticeDatingAppContext _context;

        public AuthoController(PracticeDatingAppContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDataRegister user)
        {
            // Проверка, существует ли уже пользователь с таким логином
            var existingUser = await _context.Userdataregisters
                .FirstOrDefaultAsync(u => u.Login == user.Login);

            if (existingUser != null)
            {
                return BadRequest("Пользователь с таким логином уже существует.");
            }

            // Хеширование пароля
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Получение максимального UserId и установка нового Id
            var maxUserId = await _context.Userdataregisters.MaxAsync(u => (int?)u.UdrId) ?? 0;
            var newUserId = maxUserId + 1;

            // Создание нового пользователя, используя класс Userdataregister
            var newUser = new Userdataregister
            {
                UdrId = newUserId, // Установка нового UserId
                Login = user.Login,
                Password = hashedPassword
            };

            _context.Userdataregisters.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("Пользователь успешно зарегистрирован.");
        }
    }
}
