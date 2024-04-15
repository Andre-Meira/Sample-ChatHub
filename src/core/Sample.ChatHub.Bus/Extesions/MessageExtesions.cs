using Newtonsoft.Json;
using System.Text;

namespace Sample.ChatHub.Bus.Extesions;


public static class MessageExtesions
{
    public static string GetExchangeContract<IMessage>()
    {
        Type messageType = typeof(IMessage);
        MessageAttribute? infoAttribute = (MessageAttribute?)Attribute.GetCustomAttribute(messageType, typeof(MessageAttribute));

        if (infoAttribute == null)
            throw new ArgumentException("A mensagem não possui o atributo ContractAttribute");

        return infoAttribute.Exchange;
    }

    public static string GetExchangeTypeContract<IMessage>()
    {
        Type messageType = typeof(IMessage);
        MessageAttribute? infoAttribute = (MessageAttribute?)Attribute.GetCustomAttribute(messageType, typeof(MessageAttribute));

        if (infoAttribute == null)
            throw new ArgumentException("A mensagem não possui o atributo ContractAttribute");

        return infoAttribute.ExchangeType;
    }

    public static byte[] SerializationMessage<IMessage>(this IMessage message) where IMessage: class
    {
        string json = JsonConvert.SerializeObject(message, Formatting.Indented);
        return Encoding.UTF8.GetBytes(json);
    }

    public static IMessage DeserializationMessage<IMessage>(this byte[] message) where IMessage: class
    {
        string json = Encoding.UTF8.GetString(message);

        IMessage? messageObject = JsonConvert.DeserializeObject<IMessage>(json);

        if (messageObject is null) throw new ArgumentException($"Não foi possivel transforma message em {nameof(message)}");        

        return messageObject;
    }
}

