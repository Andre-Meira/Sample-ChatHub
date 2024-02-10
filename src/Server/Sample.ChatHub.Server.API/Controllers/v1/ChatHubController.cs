using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Domain.Contracts;
using Sample.ChatHub.Server.API;

namespace Sample.ChatHub.API.Controllers.v1;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ChatHubController : DefaultController
{
    private readonly IPublishContext _context;
    public ChatHubController(IPublishContext publish) => _context = publish;
    
    
    [HttpPost("Create")]
    [Produces("application/json")]
    public async Task<IActionResult> Create([FromBody, Required] string name)
    {
        Guid IdChat = Guid.NewGuid();

        CreateChat chat = new CreateChat(IdChat, name, UserID);        
        await _context.PublishMessage(chat).ConfigureAwait(false);

        return Ok($"Chat criado. id:{IdChat}");
    }
}
