using BD.Models;
using makets.Model.Model_users;
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

        // Вывод информации о пользователе в профиль
        [HttpPost("getProfileUser")]
        public async Task<IActionResult> GetProfileUser([FromBody] int userId)
        {
            if (userId == 0)
            {
                return BadRequest(new { message = "Ошибка! Неверный ID пользователя." });
            }

            // Проверяем, существует ли пользователь
            var userExists = await _context.Datausers.AnyAsync(u => u.UserId == userId);
            if (!userExists)
            {
                return NotFound("Пользователь с таким id не существует");
            }

            var user = await _context.Datausers
                                     .Include(u => u.Gender)
                                     .Include(u => u.Location)
                                     .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound(new { message = "Данные о пользователе не найдены!" });
            }

            return Ok(new UserProfileData
            {
                FullName = user.LastName.Trim() + " " + user.FirstName.Trim() +
                              (string.IsNullOrEmpty(user.Patronymic) ? "" : " " + user.Patronymic.Trim()),
                Age = DateTime.Now.Year - user.DateOfBirth.Year,
                City = user.Location.LocationName,
                Gender = user.Gender.GenderName
            });
        }

        // Вывод информации о пользователе из таблицы DataUser
        [HttpPost("getDataUser")]
        public async Task<IActionResult> GetDataUser([FromBody] int userId)
        {
            if (userId == 0)
            {
                return BadRequest(new { message = "Ошибка! Неверный ID пользователя." });
            }

            // Проверяем, существует ли пользователь
            var userExists = await _context.Datausers.AnyAsync(u => u.UserId == userId);
            if (!userExists)
            {
                return NotFound("Пользователь с таким id не существует");
            }

            var user = await _context.Datausers
                                     .Include(u => u.Gender)
                                     .Include(u => u.Location)
                                     .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound(new { message = "Данные о пользователе не найдены!" });
            }

            return Ok(new DataUser
            {
                LastName = user.LastName,
                FirstName = user.FirstName,
                Patronymic = user.Patronymic ?? null,
                DateOfBirth = user.DateOfBirth,
                GenderId = user.GenderId,
                LocationId = user.LocationId,
                UdrId = user.UdrId
            });
        }

        //Обновление записи в таблице DataUser
        [HttpPost("updateUserData")]
        public async Task<IActionResult> UpdateUserData([FromBody] DataUser newUser)
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

            // Поиск существующего пользователя
            var existingUser = await _context.Datausers.FindAsync(newUser.UserId);
            if (existingUser == null)
            {
                return NotFound("Пользователь не найден.");
            }

            // Обновление остальных полей пользователя
            existingUser.LastName = newUser.LastName;
            existingUser.FirstName = newUser.FirstName;
            existingUser.Patronymic = newUser.Patronymic ?? null;
            existingUser.DateOfBirth = newUser.DateOfBirth;
            existingUser.GenderId = newUser.GenderId;
            existingUser.LocationId = newUser.LocationId;
            existingUser.UdrId = newUser.UdrId;

            _context.Datausers.Update(existingUser);
            await _context.SaveChangesAsync();

            return Ok(new { userId = existingUser.UserId });
        }

        [HttpPost("getUserTags")]
        public async Task<IActionResult> GetUserTags([FromBody] int userId)
        {
            // Проверяем, указан ли UserId
            if (userId <= 0)
            {
                return BadRequest("Некорректный идентификатор пользователя.");
            }

            // Проверяем, существует ли пользователь
            var userExists = await _context.Datausers.AnyAsync(u => u.UserId == userId);
            if (!userExists)
            {
                return NotFound("Пользователь с таким id не существует.");
            }

            // Получаем теги пользователя
            var userTags = await _context.Userinterests
                .Where(ut => ut.UserId == userId)
                .Select(ut => ut.TagId) 
                .ToListAsync();

            return Ok(userTags);
        }

    

    }
}
