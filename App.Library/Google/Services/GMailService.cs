using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;

namespace App.Library.Google.Services;

public class GMailService(IConfiguration config) : IGMailService
{
    private IConfigurationSection _settings = config.GetSection("Google");

    private static string[] Scopes = { GmailService.Scope.GmailReadonly }; 
    private static string ApplicationName = "Jobster07";
    private static string FilePath = "C:\\Users\\kbdav\\AppData\\Roaming\\Microsoft\\UserSecrets\\43046854-fa06-4890-bc9f-b1f7ffceff0c";
    private static string FileName = "d-client_secret_308518579938-vrpjek9f49ueuh7i2eu8qcs0cekufv8b.apps.googleusercontent.json";
    private static string FullPathWithName = $"{FilePath}\\{FileName}";

    public async Task<List<string>> GetMail()
    {
        var report = new List<string>();

        UserCredential credential;

        using (var stream = new FileStream(FullPathWithName, FileMode.Open, FileAccess.Read))
        {
            string credPath = "token.json";

            var clientSecrets = GoogleClientSecrets.FromStream(stream).Secrets;

            var authResult = GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets,new[] { GmailService.Scope.GmailReadonly },
                "user",CancellationToken.None,new FileDataStore("token.json", true)).Result;

            var RefreshToken = authResult.Token.RefreshToken;
            report.Add("Credential file saved to: " + credPath);

            credential = new UserCredential(new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets { ClientId = clientSecrets.ClientId, ClientSecret = clientSecrets.ClientSecret },
            }),
            "user",
            authResult.Token);
           
        }

        var service = new GmailService(new BaseClientService.Initializer() { HttpClientInitializer = credential, ApplicationName = ApplicationName });
        
        UsersResource.MessagesResource.ListRequest request = service.Users.Messages.List("me");

        request.LabelIds = "Label_JobSearch2024";

        IList<Message> messages = request.Execute().Messages;

        report.Add("Messages:");


        if (messages != null && messages.Count > 0)
        {
            foreach (var messageItem in messages)
            {
                var message = service.Users.Messages.Get("me", messageItem.Id).Execute();
                Console.WriteLine($"- {message.Snippet}");

                report.Add($"- {message.Snippet}");
            }
        }
        else
        {
            report.Add("No messages found.");
        }

        return report;
    }

}
