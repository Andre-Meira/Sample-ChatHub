using Sample.ChatHub.Domain.Abstracts.EventStream;

namespace Sample.ChatHub.Core.Chat;

public record ChatHub : IAggregateStream<IChatEventStream>
{
    public ChatHub()
    {
        Users = new List<Guid>();
        Messages = new List<Message>(); 
    }

    public Guid ChatId { get; private set; }
    public string Name { get; set; } = null!;
    public Guid UserCreated { get; set; }

    public List<Guid> Users { get; private set; }
    public List<Message> Messages { get; private set; }

    public void Apply(IChatEventStream @event) => @event.Process(this);
    

    public void AddUser(Guid user)
    {
        bool userExist = Users.Contains(user);

        if (userExist)        
            throw new ArgumentException("Usuario já faz parte do chat.");        
            
        Users.Add(user);
    }

    public void RemoveUser(Guid userId)
    {
        bool isRemoveSucess = Users.Remove(userId);

        if (isRemoveSucess == false)
            throw new ArgumentException("Usuario não se encontra no chat.");
    }

    public void Create(Guid id)
    {
        if (ChatId != Guid.Empty)
            throw new ArgumentException("Já existe um id para esse chat.");

        ChatId = id;
    }
        
    public void SendMessage(Message message)
    {
        bool UserExist = Users.Contains(message.UserId);

        if (UserExist == false)
        {
            throw new ArgumentException("Usuario não faz parte do chat então não pode mandar mensagem");
        }

        Messages.Add(message);
    }

   
}
