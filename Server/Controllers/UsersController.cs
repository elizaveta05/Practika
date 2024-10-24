using BD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PracticeDatingAppContext _context;

        public UsersController(PracticeDatingAppContext context)
        {
            _context = context;
        }



    }
}
