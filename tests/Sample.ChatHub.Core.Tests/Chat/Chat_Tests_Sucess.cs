using Moq;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Core.Chat.Events;
using Sample.ChatHub.Domain.Contracts.Messages;
using System.Text;

namespace Sample.ChatHub.Core.Tests.Chat;

public class Chat_Tests_Sucess
{
    #region Mocks       
    private readonly IChatProcessStream _chatProcess;    
    private readonly IChatEventsRepositore _chatEventsRepositore;    
    private readonly IList<IChatEventStream>  chatEventStreams = new List<IChatEventStream>();
    private Guid IdChat = Guid.NewGuid();

    public Chat_Tests_Sucess()
    {        
        var chatRepositoreRepositore = new Mock<IChatEventsRepositore>();

        chatRepositoreRepositore.Setup(s => s.IncressEvent(It.IsAny<IChatEventStream>()))
            .Callback<IChatEventStream>(chatEventStreams.Add); 

        chatRepositoreRepositore.Setup(s => s.GetEvents(It.IsAny<Guid>()))
            .Returns((Guid id) => chatEventStreams.Where(e => e.IdCorrelation == id));

        _chatEventsRepositore = chatRepositoreRepositore.Object;
        _chatProcess = new ChatProcessStream(_chatEventsRepositore);
    }
    #endregion


    [Fact]    
    public async Task Criar_Chat_Retonar_Sucesso()
    {
        Guid id = Guid.NewGuid();

        ChatCreated chatCreated = new ChatCreated(IdChat, "Chat Teste", id);
        await _chatProcess.Include(chatCreated);

        var chat = await _chatProcess.Process(chatCreated.IdCorrelation);

        Assert.NotNull(chat.Name);   
    }

    [Fact]
    public async Task Enviar_Uma_Message_No_Chat_Retona_Sucesso()
    {
        Guid id = Guid.NewGuid();

        ChatCreated chatCreated = new ChatCreated(IdChat, "Chat Teste", id);
        await _chatProcess.Include(chatCreated);

        string message = "Primeira message do chat";
        SendMessageChat sendMessageChat = new SendMessageChat(new ContextMessage(IdChat, id, message));
        await _chatProcess.Include(sendMessageChat);

        var chat = await _chatProcess.Process(chatCreated.IdCorrelation);

        Assert.NotEmpty(chat.Messages);
    }

    [Fact]
    public async Task Entrar_no_Chat_Retona_Sucesso()
    {
        Guid idOwner = Guid.NewGuid();
        Guid idUser = Guid.NewGuid();

        ChatCreated chatCreated = new ChatCreated(IdChat, "Chat Teste", idOwner);
        await _chatProcess.Include(chatCreated);

        UserJoinedChat userJoinedChat = new UserJoinedChat(chatCreated.IdCorrelation, idUser);
        await _chatProcess.Include(userJoinedChat);

        var chat = await _chatProcess.Process(chatCreated.IdCorrelation);

        Assert.NotEmpty(chat.Users.Where(e => e == idUser));
    }


    [Fact]
    public async Task Sair_do_Chat_Retona_Sucesso()
    {
        Guid idOwner = Guid.NewGuid();        

        ChatCreated chatCreated = new ChatCreated(IdChat, "Chat Teste", idOwner);
        await _chatProcess.Include(chatCreated);

        UserLeftChat userLeftChat = new UserLeftChat(chatCreated.IdCorrelation, idOwner);
        await _chatProcess.Include(userLeftChat);

        var chat = await _chatProcess.Process(chatCreated.IdCorrelation);

        Assert.Empty(chat.Users.Where(e => e == idOwner));
    }
}
