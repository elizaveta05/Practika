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
            var existingUser = await _context.Userdataregisters
                .FirstOrDefaultAsync(u => u.Login == user.Login);

            if (existingUser != null)
            {
                return BadRequest("Пользователь с таким логином уже существует.");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            var maxUserId = await _context.Userdataregisters.MaxAsync(u => (int?)u.UdrId) ?? 0;
            var newUserId = maxUserId + 1;

            var newUser = new Userdataregister
            {
                UdrId = newUserId,
                Login = user.Login,
                Password = hashedPassword
            };

            _context.Userdataregisters.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { userId = newUserId });
        }

        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _context.Locations
                .Select(c => new { c.LocationId, c.LocationName })
                .ToListAsync();

            return Ok(cities);
        }

        [HttpPost("newDataUser")]
        public async Task<IActionResult> CreateNewDataUser([FromBody] Datauser newUser)
        {
            if (newUser == null)
            {
                return BadRequest("Некорректные данные.");
            }

            // Проверка обязательных данных
            if (newUser.GenderId <= 0)
            {
                return BadRequest("Пол пользователя обязателен.");
            }

            if (newUser.LocationId <= 0)
            {
                return BadRequest("Местоположение пользователя обязательно.");
            }

            if (newUser.UdrId <= 0)
            {
                return BadRequest("ID пользователя обязателен.");
            }

            // Проверка наличия внешних ключей в базе данных
            var genderExists = await _context.Genders.AnyAsync(g => g.GenderId == newUser.GenderId);
            if (!genderExists)
            {
                return BadRequest("Некорректный идентификатор пола.");
            }

            var locationExists = await _context.Locations.AnyAsync(l => l.LocationId == newUser.LocationId);
            if (!locationExists)
            {
                return BadRequest("Некорректный идентификатор местоположения.");
            }

            var udrExists = await _context.Userdataregisters.AnyAsync(u => u.UdrId == newUser.UdrId);
            if (!udrExists)
            {
                return BadRequest("Некорректный идентификатор регистрации пользователя.");
            }

            // Получаем максимальный UserId и создаем новый
            var maxUserId = await _context.Datausers.MaxAsync(u => (int?)u.UserId) ?? 0;
            newUser.UserId = maxUserId + 1; // Увеличиваем на 1 для нового ID

            var existingUser = await _context.Datausers.FirstOrDefaultAsync(u => u.UserId == newUser.UserId);
            if (existingUser != null)
            {
                return BadRequest("Пользователь с таким идентификатором уже существует.");
            }

            _context.Datausers.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Пользователь успешно создан." });
        }

    }
}
