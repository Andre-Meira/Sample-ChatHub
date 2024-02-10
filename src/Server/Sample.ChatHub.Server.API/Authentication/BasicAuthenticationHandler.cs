using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sample.ChatHub.Server.API;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly List<User> _usersOptions;
    public const string Schema = "Basic";

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, 
        IConfiguration configuration)
    : base(options, logger, encoder)
    {
        _usersOptions = configuration.GetSection(UserSettings.Key).Get<List<User>>()!;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? authentication =  Request.Headers.Authorization;        

        BasicAuthorization user = new BasicAuthorization(authentication);
        
        var userValid = _usersOptions.FirstOrDefault(e => e.Id == user.Id && user.Name == e.Name) ;

        if(userValid is null) AuthenticateResult.Fail("Usuario não é valido.");

        Claim claimName = new Claim(ClaimTypes.Name, user.Name);
        Claim claimID = new Claim(ClaimTypes.Sid, user.Id);

        ClaimsIdentity identity = new ClaimsIdentity(new[] {claimName, claimID}, Schema);                

        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);             

        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
    }
}


public record BasicAuthorization
{
    public string Name { get; } = null!;
    public string Id { get; } = null!;

    public BasicAuthorization(string? authorizationHeader)
    {
        if (string.IsNullOrEmpty(authorizationHeader)) 
            throw new ArgumentException("Sem autenticacao..");                

        if (!IsBasicAuthorization(authorizationHeader))
            throw new ArgumentException("Autenticacao não é valdia.");

        var credentials = DecodeBasicCredentials(authorizationHeader);
        var parts = credentials.Split(':');

        if (parts.Length != 2)
        {
            throw new ArgumentException("Credenciais inválidas no cabeçalho de autorização Basic");                
        }

        Name = parts[0];
        Id =  parts[1];
    }

    private static bool IsBasicAuthorization(string authorizationHeader)
    {
        return authorizationHeader?.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static string DecodeBasicCredentials(string authorizationHeader)
    {
        var base64Credentials = authorizationHeader.Substring(6);
        var credentialsBytes = Convert.FromBase64String(base64Credentials);
        return Encoding.UTF8.GetString(credentialsBytes);
    }
}