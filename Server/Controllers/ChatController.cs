using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using makets.Models;
using BD.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly PracticeDatingAppContext _context;

        public ChatController(PracticeDatingAppContext context)
        {
            _context = context;
        }

        // Метод для получения всех чатов пользователя с дополнительной информацией
        [HttpGet("getUserChats/{userId}")]
        public async Task<IActionResult> GetUserChats(int userId)
        {
            // Извлечение всех чатов, в которых участвует пользователь
            var chats = await _context.Chats
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .Select(c => new
                {
                    ChatId = c.ChatId,
                    OtherUserId = c.User1Id == userId ? c.User2Id : c.User1Id,
                    ChatCreated = c.TimeCreated
                })
                .ToListAsync();

            var chatDetails = new List<object>();

            // Цикл для получения дополнительной информации для каждого чата
            foreach (var chat in chats)
            {
                // Получение информации о другом пользователе в чате
                var otherUser = await _context.Datausers
                    .Where(u => u.UserId == chat.OtherUserId)
                    .Select(u => new
                    {
                        UserId = u.UserId,
                        Name = $"{u.LastName.Trim()} {u.FirstName.Trim()}"
                    })
                    .FirstOrDefaultAsync();

                // Получение фотографии профиля другого пользователя
                var profilePhoto = await _context.Userphotopprofiles
                    .Where(p => p.UserId == chat.OtherUserId)
                    .Select(p => p.Photo)
                    .FirstOrDefaultAsync();

                // Получение последнего сообщения в чате
                var lastMessage = await _context.Messages
                    .Where(m => m.ChatId == chat.ChatId)
                    .OrderByDescending(m => m.TimeCreated)
                    .Select(m => new
                    {
                        MessageText = m.MessageText.Length > 10 ? m.MessageText.Substring(0, 10) + "..." : m.MessageText,
                        m.TimeCreated
                    })
                    .FirstOrDefaultAsync();

                // Форматирование последнего сообщения
                var lastMessageFormatted = lastMessage != null
                    ? $"{lastMessage.MessageText} ({lastMessage.TimeCreated:dd/MM/yyyy HH:mm})"
                    : "Нет сообщений"; // Здесь указываем, что сообщений нет

                // Добавление собранных данных о чате в список
                chatDetails.Add(new
                {
                    ChatId = chat.ChatId,
                    OtherUser = new
                    {
                        otherUser.UserId,
                        otherUser.Name,
                        ProfilePhoto = profilePhoto
                    },
                    LastMessage = lastMessageFormatted
                });
            }

            // Возврат информации о чатах пользователя
            return Ok(chatDetails);
        }

        // Метод для получения сообщений 
        [HttpGet("messages/{chatId}")]
        public async Task<ActionResult<IEnumerable<makets.Models.Message>>> GetMessages(int chatId, [FromQuery] DateTime? lastTimestamp)
        {
            // Проверка существования чата
            var chatExists = await _context.Chats.AnyAsync(c => c.ChatId == chatId);
            if (!chatExists)
            {
                return NotFound("Чат не найден.");
            }

            // Запрос на получение сообщений из чата
            var query = _context.Messages.Where(m => m.ChatId == chatId);

            // Фильтрация сообщений по времени, если указан lastTimestamp
            if (lastTimestamp.HasValue)
            {
                query = query.Where(m => m.TimeCreated > lastTimestamp.Value);
            }

            // Получение и возврат списка сообщений
            var messages = await query
                .OrderBy(m => m.TimeCreated)
                .Select(m => new makets.Models.Message
                {
                    MessageId = m.MessageId,
                    UserSendingId = m.UserSendingId,
                    MessageText = m.MessageText,
                    TimeCreated = m.TimeCreated
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<ActionResult<makets.Models.Message>> SendMessage([FromBody] makets.Models.Message newMessage)
        {
            // Проверка существования отправителя и чата
            var sender = await _context.Datausers.FindAsync(newMessage.UserSendingId);
            var chat = await _context.Chats
                .Where(c => c.ChatId == newMessage.ChatId)
                .FirstOrDefaultAsync();

            // Проверка наличия отправителя
            if (sender == null)
            {
                return BadRequest("Отправитель не найден.");
            }

            // Проверка наличия чата
            if (chat == null)
            {
                return BadRequest("Чат не найден.");
            }

            // Проверка, не пытается ли пользователь отправить сообщение самому себе
            if (newMessage.UserSendingId == (chat.User1Id == newMessage.UserSendingId ? chat.User2Id : chat.User1Id))
            {
                return BadRequest("Вы не можете отправить сообщение самому себе.");
            }

            // Проверка, что сообщение не пустое
            if (string.IsNullOrWhiteSpace(newMessage.MessageText))
            {
                return BadRequest("Текст сообщения пустое.");
            }

            // Получаем максимальный id
            var maxMessageId = await _context.Messages.MaxAsync(u => (int?)u.MessageId) ?? 0;
            var newMessageId = maxMessageId + 1;

            var message = new BD.Models.Message
            {
                MessageId = newMessageId,
                ChatId = newMessage.ChatId,
                UserSendingId = newMessage.UserSendingId,
                MessageText = newMessage.MessageText,
                TimeCreated = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            // Сохранение нового сообщения в базе данных
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Возврат успешно созданного сообщения
            return Ok(message);
        }
    }

}
