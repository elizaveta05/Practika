﻿using BD.Models;
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

        [HttpPost("getUserGoal")]
        public async Task<IActionResult> GetUserGoal([FromBody] int userId)
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

            // Получаем единственную цель пользователя
            var userGoalId = await _context.Usergoals
                .Where(ug => ug.UserId == userId)
                .Select(ug => (int?)ug.GoalId)
                .FirstOrDefaultAsync();

            return Ok(userGoalId);
        }

        [HttpPost("getUserDescription")]
        public async Task<IActionResult> GetUserDescription([FromBody] int userId)
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

            // Получаем описание пользователя
            var userDescription = await _context.Userdescriptions
                .Where(ud => ud.UserId == userId)
                .Select(ud => new { ud.UdId, ud.Description })
                .FirstOrDefaultAsync(); // Используем FirstOrDefault для получения одного элемента

            if (userDescription == null)
            {
                return NotFound("Описание пользователя не найдено.");
            }

            return Ok(userDescription);
        }

        [HttpPost("updateUserDescription")]
        public async Task<IActionResult> UpdateUserDescription([FromBody] UserDescription userDescription)
        {
            // Проверяем, указаны ли UserId и описание
            if (userDescription.UserId <= 0 || string.IsNullOrWhiteSpace(userDescription.Description))
            {
                return BadRequest("Некорректные данные.");
            }

            // Проверяем, существует ли пользователь
            var user = await _context.Datausers.FindAsync(userDescription.UserId);
            if (user == null)
            {
                return NotFound("Пользователь с таким id не существует.");
            }

            // Получаем и обновляем описание пользователя
            var existingDescription = await _context.Userdescriptions
                .FirstOrDefaultAsync(ud => ud.UserId == userDescription.UserId);

            if (existingDescription != null)
            {
                existingDescription.Description = userDescription.Description; // Обновляем описание
            }
            else
            {
                // Получаем максимальный UgId и создаем новую цель
                var maxUgId = await _context.Userdescriptions.MaxAsync(u => (int?)u.UdId) ?? 0;
                var newUgId = maxUgId + 1;

                // Если описание не существует, добавляем новое
                existingDescription = new Userdescription
                {
                    UdId = newUgId,
                    UserId = userDescription.UserId,
                    Description = userDescription.Description
                };
                _context.Userdescriptions.Add(existingDescription);
            }

            // Сохраняем изменения
            await _context.SaveChangesAsync();

            return Ok(new { message = "Описание пользователя обновлено." });
        }

        [HttpPost("postUserPhotoProfile")]
        public async Task<IActionResult> PostUserPhotoProfile([FromBody] UserPhotoProfile userPhotoProfile)
        {
            var user = await _context.Datausers.FindAsync(userPhotoProfile.UserId);
            if (user == null)
            {
                return NotFound("Пользователь с таким id не существует.");
            }

            if (userPhotoProfile.Photo == null)
            {
                return BadRequest("Отсутствует изображение.");
            }

            var existingPhotoProfile = await _context.Userphotopprofiles
                .FirstOrDefaultAsync(ud => ud.UserId == userPhotoProfile.UserId);

            if (existingPhotoProfile != null)
            {
                existingPhotoProfile.Photo = userPhotoProfile.Photo; // Обновляем изображение
            }
            else
            {
                var maxUgId = await _context.Userphotopprofiles.MaxAsync(u => (int?)u.UppId) ?? 0;
                var newUgId = maxUgId + 1;

                var data = new Userphotopprofile
                {
                    UppId = newUgId,
                    UserId = userPhotoProfile.UserId,
                    Photo = userPhotoProfile.Photo
                };
                _context.Userphotopprofiles.Add(data);
            }

            await _context.SaveChangesAsync();

            // Возвращаем обновленное фото профиля
            return Ok(userPhotoProfile.Photo);
        }

        [HttpGet("getUserPhotoProfile/{userId}")]
        public async Task<IActionResult> GetUserPhotoProfile(int userId)
        {
            var user = await _context.Datausers.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Пользователь с таким ID не существует.");
            }

            var photoProfile = await _context.Userphotopprofiles
                .FirstOrDefaultAsync(up => up.UserId == userId);

            if (photoProfile == null)
            {
                return NotFound("Фотография профиля не найдена."); // Если нет фото, возвращаем 404
            }

            // Предполагаем, что поле Photo содержит изображение в формате byte[]
            var base64Image = Convert.ToBase64String(photoProfile.Photo);
            return Ok(base64Image); // Возвращаем строку в формате Base64
        }

    }
}
