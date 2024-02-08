using Moq;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Core.Chat.Events;

namespace Sample.ChatHub.Core.Tests.Chat;

public class Create_Chat_Tests_Sucess
{
    #region Mocks
    private Guid id = Guid.NewGuid();

    private readonly IChatProcessStream _chatProcess;    
    private readonly IChatEventsRepositore _chatEventsRepositore;    
    private readonly IList<IChatEventStream>  chatEventStreams = new List<IChatEventStream>();

    public Create_Chat_Tests_Sucess()
    {        
        var chatRepositoreRepositore = new Mock<IChatEventsRepositore>();                

        _chatEventsRepositore = chatRepositoreRepositore.Object;
        _chatProcess = new ChatProcessStream(_chatEventsRepositore);
    }
    #endregion


    [Fact(DisplayName = "Criação do chat")]    
    public async Task Criar_Chat_Com_SucessoAsync()
    {
        ChatCreated chatCreated = new ChatCreated("Chat Teste", id);
        await _chatProcess.Include(chatCreated);        
    }

}
