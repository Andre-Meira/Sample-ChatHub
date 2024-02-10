using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Domain.Contracts;

namespace Sample.ChatHub.API.Controllers.v1;

[Route("api/[controller]")]
[ApiController]
public class ChatHubController : ControllerBase
{
    private readonly IPublishContext _context;
    public ChatHubController(IPublishContext publish) => _context = publish;
    
    
    [HttpPost("Create")]
    [Produces("application/json")]
    public async Task<IActionResult> Create([FromBody, Required] string name)
    {
        CreateChat chat = new CreateChat(name, Guid.NewGuid());        
        await _context.PublishMessage(chat).ConfigureAwait(false);

        return Ok("Chat criado");
    }
}
