using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using makets.Models;
using BD.Models;
using Newtonsoft.Json;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly PracticeDatingAppContext _context;
        private static readonly Dictionary<int, WebSocket> ConnectedUsers = new();

        public ChatController(PracticeDatingAppContext context)
        {
            _context = context;
        }

        // Метод для работы с WebSocket
        [HttpGet("connect/{userId}")]
        public async Task<IActionResult> Connect(int userId)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                if (!ConnectedUsers.ContainsKey(userId))
                {
                    ConnectedUsers.Add(userId, webSocket);
                }

                await ReceiveMessages(webSocket, userId);
                return new EmptyResult();
            }
            else
            {
                return BadRequest("WebSocket connection expected.");
            }
        }

        private async Task ReceiveMessages(WebSocket webSocket, int userId)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    ConnectedUsers.Remove(userId);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the client", CancellationToken.None);
                }
            }
        }

        // Метод для отправки сообщения всем подключенным пользователям
        [HttpPost("send")]
        public async Task<ActionResult<makets.Models.Message>> SendMessage([FromBody] makets.Models.Message newMessage)
        {
            var sender = await _context.Datausers.FindAsync(newMessage.UserSendingId);
            var chat = await _context.Chats.FindAsync(newMessage.ChatId);

            if (sender == null || chat == null || string.IsNullOrWhiteSpace(newMessage.MessageText))
            {
                return BadRequest("Ошибка при отправке сообщения.");
            }

            var message = new BD.Models.Message
            {
                MessageId = await _context.Messages.MaxAsync(u => (int?)u.MessageId) + 1 ?? 1,
                ChatId = newMessage.ChatId,
                UserSendingId = newMessage.UserSendingId,
                MessageText = newMessage.MessageText,
                TimeCreated = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var messageJson = JsonConvert.SerializeObject(new
            {
                message.MessageId,
                message.ChatId,
                message.UserSendingId,
                message.MessageText,
                TimeCreated = message.TimeCreated.ToString("O")
            });

            var messageBuffer = Encoding.UTF8.GetBytes(messageJson);

            foreach (var (userId, socket) in ConnectedUsers)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }

            return Ok(message);
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


    }
}
