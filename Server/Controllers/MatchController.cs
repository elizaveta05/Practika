using BD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly PracticeDatingAppContext _context;

        public MatchController(PracticeDatingAppContext context)
        {
            _context = context;
        }
        /*
            На клиенте метод будет вызываться при каждом лайке другого пользователя в отборе каждое определенное время

            Сначала мы создаем метч, потом проверяем есть ли метч у другого пользователя и если да, то мы создаем чат
        */

        // Метод для создания чата при взаимном мэтче
        [HttpPost("createMatch")]
        public async Task<IActionResult> CreateMatch([FromBody] Match matchRequest)
        {
            // Проверка существования матча в одном порядке
            var existingMatch = await _context.Matches
                .FirstOrDefaultAsync(m => m.User1Id == matchRequest.User1Id && m.User2Id == matchRequest.User2Id);

            if (existingMatch == null)
            {
                // Если метча не существует, создаем новый
                var newMatch = new Match
                {
                    User1Id = matchRequest.User1Id,
                    User2Id = matchRequest.User2Id,
                    Timestamp = DateOnly.FromDateTime(DateTime.UtcNow)
                };

                _context.Matches.Add(newMatch);
                await _context.SaveChangesAsync();
            }

            // Проверка обратного матча для создания чата
            var reciprocalMatch = await _context.Matches
                .FirstOrDefaultAsync(m => m.User1Id == matchRequest.User2Id && m.User2Id == matchRequest.User1Id);

            if (reciprocalMatch != null)
            {
                // Если обратный матч существует, создаем чат
                var chat = new Chat
                {
                    User1Id = matchRequest.User1Id,
                    User2Id = matchRequest.User2Id,
                    TimeCreated = DateTime.UtcNow
                };

                _context.Chats.Add(chat);
                await _context.SaveChangesAsync();

                return Ok("Чат создан успешно.");
            }

            return Ok("Пользователь отобран.");
        }

    }
}
