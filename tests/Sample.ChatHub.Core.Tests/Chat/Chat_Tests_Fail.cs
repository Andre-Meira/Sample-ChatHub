using Moq;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Core.Chat.Events;
using System.Text;

namespace Sample.ChatHub.Core.Tests.Chat;

public class Chat_Tests_Fail
{
    #region Mocks       
    private readonly IChatProcessStream _chatProcess;    
    private readonly IChatEventsRepositore _chatEventsRepositore;    
    private readonly IList<IChatEventStream>  chatEventStreams = new List<IChatEventStream>();
    private Guid IdChat = Guid.NewGuid();    
   
    public Chat_Tests_Fail()
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
    public async Task Criar_Chat_Que_Ja_Existe_Retona_Erro()
    {
        Guid id = Guid.NewGuid();

        ChatCreated chatCreated = new ChatCreated(IdChat, "Chat Teste", id);
        await _chatProcess.Include(chatCreated);
        ChatCreated chatCreated2 = new ChatCreated(IdChat, "Chat Teste2", id);       

        await Assert.ThrowsAsync<ArgumentException>(() =>
        {
            return _chatProcess.Include(chatCreated);
        });   
    }

    [Fact]
    public async Task Envia_Uma_Messa_Sem_Esta_No_Chat_Retona_Erro()
    {
        Guid id = Guid.NewGuid();
        Guid IdUser = Guid.NewGuid();   

        ChatCreated chatCreated = new ChatCreated(IdChat, "Chat Teste", id);
        await _chatProcess.Include(chatCreated);

        byte[] message = Encoding.UTF8.GetBytes("Primeira message do chat");
        SendMessageChat sendMessageChat = new SendMessageChat(chatCreated.IdCorrelation, IdUser, message);

        await Assert.ThrowsAsync<ArgumentException>(() =>
        {
            return _chatProcess.Include(sendMessageChat);
        });
    }

    [Fact]
    public async Task Entrar_no_Chat_Que_Ja_Participa()
    {
        Guid idOwner = Guid.NewGuid();        

        ChatCreated chatCreated = new ChatCreated(IdChat, "Chat Teste", idOwner);
        await _chatProcess.Include(chatCreated);

        UserJoinedChat userJoinedChat = new UserJoinedChat(chatCreated.IdCorrelation, idOwner);
        
        await Assert.ThrowsAsync<ArgumentException>(() =>
        {
            return _chatProcess.Include(userJoinedChat);
        });
    }


    [Fact]
    public async Task Sair_do_Chat_Que_Nao_Participa()
    {
        Guid idOwner = Guid.NewGuid();
        Guid idUser = Guid.NewGuid();

        ChatCreated chatCreated = new ChatCreated(IdChat, "Chat Teste", idOwner);
        await _chatProcess.Include(chatCreated);

        UserLeftChat userLeftChat = new UserLeftChat(chatCreated.IdCorrelation, idUser);
         
        await Assert.ThrowsAsync<ArgumentException>(() =>
        {
            return _chatProcess.Include(userLeftChat); 
        });
    }
}
