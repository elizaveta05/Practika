using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BD.Models;
using Server.Model.Model_users;
using BCrypt.Net;
using System.Threading.Tasks;
using makets.Model.Model_users;

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

        //Регистрация пользователя в таблицу UserDataRegister
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

        //Возвращения списка городов
        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _context.Locations
                .Select(c => new { c.LocationId, c.LocationName })
                .ToListAsync();

            return Ok(cities);
        }

        //Создание записи в таблице DataUser
        [HttpPost("newDataUser")]
        public async Task<IActionResult> CreateNewDataUser([FromBody] Datauser newUser)
        {
            // Проверка наличия внешних ключей
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
            newUser.UserId = maxUserId + 1;

            // Проверка на наличие пользователя с таким ID
            var existingUser = await _context.Datausers.FirstOrDefaultAsync(u => u.UserId == newUser.UserId);
            if (existingUser != null)
            {
                return BadRequest("Пользователь с таким идентификатором уже существует.");
            }

            // Добавляем новую запись в контекст
            _context.Datausers.Add(newUser);

            // Сохранение изменений в базе данных
            await _context.SaveChangesAsync();

            return Ok(new { userId = newUser.UserId });
        }

        public class SaveUserTagsRequest
        {
            public List<int> TagIds { get; set; }
            public int UserId { get; set; }
        }

        [HttpPost("saveUserTags")]
        public async Task<IActionResult> SaveUserTags([FromBody] SaveUserTagsRequest request)
        {
            if (request.TagIds == null || !request.TagIds.Any())
            {
                return BadRequest("No tags provided.");
            }

            // Проверяем, существует ли пользователь
            var userExists = await _context.Datausers.AnyAsync(u => u.UserId == request.UserId);
            if (!userExists)
            {
                return NotFound("Пользователь с таким id не существует");
            }

            // Удаляем старые теги пользователя
            var existingUserTags = await _context.Userinterests.Where(ut => ut.UserId == request.UserId).ToListAsync();
            _context.Userinterests.RemoveRange(existingUserTags);

            // Получаем максимальный UiId и создаем новый
            var maxUiId = await _context.Userinterests.MaxAsync(u => (int?)u.UiId) ?? 0;

            // Добавляем новые теги
            foreach (var tagId in request.TagIds)
            {
                var newUiId = ++maxUiId;
                var userTag = new Userinterest
                {
                    UiId = newUiId,
                    UserId = request.UserId,
                    TagId = tagId
                };
                _context.Userinterests.Add(userTag);
            }

            // Сохраняем изменения
            await _context.SaveChangesAsync();

            return Ok(new { message = "Пользовательские теги сохранены." });
        }



    }
}
