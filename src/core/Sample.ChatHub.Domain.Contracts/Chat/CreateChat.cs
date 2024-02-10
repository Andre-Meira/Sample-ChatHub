namespace Sample.ChatHub.Domain.Contracts;

[Contract("create-chat")]
public class CreateChat
{
    public CreateChat(string name, Guid idUser)
    {
        Name = name;
        IdUser = idUser;
    }

    public string Name { get; init; }
    public Guid IdUser { get; init; }

}
