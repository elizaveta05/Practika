using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BD.Models;
using Server.Model.Model_users;
using BCrypt.Net;
using System.Threading.Tasks;
using makets.Model.Model_users;
using System;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly PracticeDatingAppContext _context;

        public RegistrationController(PracticeDatingAppContext context)
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
        public async Task<IActionResult> CreateNewDataUser([FromBody] DataUser newUser)
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

            
            // Создаем новую запись в таблице
            var maxUserId = await _context.Datausers.MaxAsync(u => (int?)u.UserId) ?? 0;
            newUser.UserId = maxUserId + 1;

            var newdataUser = new Datauser
            {
                UserId = newUser.UserId,
                LastName = newUser.LastName,
                FirstName = newUser.FirstName,
                Patronymic = newUser.Patronymic,
                DateOfBirth = newUser.DateOfBirth,
                GenderId = newUser.GenderId,
                LocationId = newUser.LocationId,
                UdrId = newUser.UdrId
            };
            _context.Datausers.Add(newdataUser);
            await _context.SaveChangesAsync();

            return Ok(new { userId = newUser.UserId });
        }


        [HttpPost("saveUserTags")]
        public async Task<IActionResult> SaveUserTags([FromBody] SaveUserTagsRequest request)
        {
            if (request.TagIds == null || !request.TagIds.Any())
            {
                return BadRequest("Теги не указаны.");
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

        [HttpPost("saveUserGoal")]
        public async Task<IActionResult> SaveUserGoal([FromBody] UserGoals request)
        {
            if (request.GoalId == 0)
            {
                return BadRequest("Цель не указана.");
            }

            // Проверяем, существует ли пользователь
            var userExists = await _context.Datausers.AnyAsync(u => u.UserId == request.UserId);
            if (!userExists)
            {
                return NotFound("Пользователь с таким id не существует");
            }

            // Удаляем старую цель пользователя
            var existingUserGoal = await _context.Usergoals.FirstOrDefaultAsync(ug => ug.UserId == request.UserId);
            if (existingUserGoal != null)
            {
                _context.Usergoals.Remove(existingUserGoal);
            }

            // Получаем максимальный UgId и создаем новую цель
            var maxUgId = await _context.Usergoals.MaxAsync(u => (int?)u.UgId) ?? 0;
            var newUgId = maxUgId + 1;

            var userGoal = new Usergoal
            {
                UgId = newUgId,
                UserId = request.UserId,
                GoalId = request.GoalId
            };

            _context.Usergoals.Add(userGoal);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Цель пользователя сохранена." });
        }

        [HttpPost("saveUserDescription")]
        public async Task<IActionResult> SaveUserDescription([FromBody] UserDescription request)
        {
            if (request.Description == null)
            {
                return BadRequest("Описания нет.");
            }

            // Проверяем, существует ли пользователь
            var userExists = await _context.Datausers.AnyAsync(u => u.UserId == request.UserId);
            if (!userExists)
            {
                return NotFound("Пользователь с таким id не существует");
            }

            // Получаем максимальный UgId и создаем новую цель
            var maxUgId = await _context.Userdescriptions.MaxAsync(u => (int?)u.UdId) ?? 0;
            var newUgId = maxUgId + 1;

            var userDescription = new Userdescription
            {
                UdId = newUgId,
                UserId = request.UserId,
                Description = request.Description
            };

            _context.Userdescriptions.Add(userDescription);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Описание пользователя сохранено." });
        }


    }
}
