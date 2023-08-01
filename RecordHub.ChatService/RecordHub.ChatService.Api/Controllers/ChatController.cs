using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RecordHub.ChatService.Api.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IDictionary<string, UserConnection> _connections;
        public ChatController(IDictionary<string, UserConnection> connections)
        {
            _connections = connections;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetChats()
        {

            return Ok(_connections);
        }
    }
}
