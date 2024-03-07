using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Domain.Contracts;
using Sample.ChatHub.Domain.Contracts.Chat;
using Sample.ChatHub.Server.API;

namespace Sample.ChatHub.API.Controllers.v1;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ChatHubController : DefaultController
{
    private readonly IPublishContext _context;
    private readonly IUserService _userService;

    public ChatHubController(IPublishContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpPost("Create")]
    [Produces("application/json")]
    public async Task<IActionResult> Create([FromBody, Required] string name)
    {
        Guid IdChat = Guid.NewGuid();
        CreateChat chat = new CreateChat(IdChat, name, UserID);        

        await _context.PublishMessage(chat).ConfigureAwait(false);
        await _userService.IncludeUserChat(UserID, IdChat);

        return Ok($"Chat criado, id:{IdChat}");
    }

    [HttpPost("Join/{IdChat}")]
    [Produces("application/json")]
    public async Task<IActionResult> JoinChat(Guid IdChat)
    {
        await _context.PublishMessage(new UserJoinChat(IdChat, UserID)).ConfigureAwait(false);
        await _userService.IncludeUserChat(UserID, IdChat);

        return Ok("Sucessco.");
    }
}
