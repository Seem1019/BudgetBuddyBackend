using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GmailServiceHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string[] Scopes = { GmailService.Scope.GmailReadonly };
    private readonly string ApplicationName = "BudgetBuddy";

    public GmailServiceHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<GmailService> GetGmailServiceAsync()
    {
        var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded || authenticateResult.None)
        {
            throw new UnauthorizedAccessException("No authentication token found.");
        }

        var token = authenticateResult.Properties.GetTokenValue("access_token");

        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException("Access token is missing.");
        }

        var credential = GoogleCredential.FromAccessToken(token);
        var service = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        return service;
    }

    public async Task<IList<Google.Apis.Gmail.v1.Data.Message>> GetBankTransactionsAsync(GmailService service, string bankEmail)
    {
        var request = service.Users.Messages.List("me");
        request.LabelIds = "INBOX";
        request.Q = $"from:{bankEmail}";

        var response = await request.ExecuteAsync();
        return response.Messages;
    }

    public async Task<Google.Apis.Gmail.v1.Data.Message> GetMessageAsync(GmailService service, string messageId)
    {
        return await service.Users.Messages.Get("me", messageId).ExecuteAsync();
    }
}
