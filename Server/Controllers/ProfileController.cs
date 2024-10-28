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

        // Вывод информации о пользователе
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

    }
}
