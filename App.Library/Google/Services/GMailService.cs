using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Newtonsoft.Json;
using System.Text;

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

            var authResult = GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets, new[] { GmailService.Scope.GmailReadonly },
                "user", CancellationToken.None, new FileDataStore("token.json", true)).Result;

            var RefreshToken = authResult.Token.RefreshToken;

            credential = new UserCredential(new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets { ClientId = clientSecrets.ClientId, ClientSecret = clientSecrets.ClientSecret },
            }),
            "user",
            authResult.Token);

        }

        var service = new GmailService(new BaseClientService.Initializer() { HttpClientInitializer = credential, ApplicationName = ApplicationName });

        // UsersResource.MessagesResource.ListRequest request = service.Users.Messages.List("me");

        var allMessages = new List<Message>(); 
        string pageToken = null;
        
        do
        {
            var request = service.Users.Messages.List("me");
            request.PageToken = pageToken; request.MaxResults = 100;
            request.LabelIds = "Label_3128620828543033678";

            var response = request.Execute();

            if (response.Messages != null) 
            { 
                allMessages.AddRange(response.Messages); 
            }
            
            pageToken = response.NextPageToken; 

        } 

        while (!string.IsNullOrEmpty(pageToken));

        if (allMessages != null && allMessages.Count > 0)
        {
            foreach (var messageItem in allMessages)
            {
                var message = service.Users.Messages.Get("me", messageItem.Id).Execute();

                // Request the raw format of the message
                var rawMessage = service.Users.Messages.Get("me", messageItem.Id);

                rawMessage.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Raw;
                
                var rawMessageResult = rawMessage.Execute();

                var rawMessageData = rawMessageResult.Raw;

                // Replace URL-safe characters with standard base64 characters
                rawMessageData = rawMessageData.Replace('-', '+').Replace('_', '/');

                // Pad the string with '=' characters to make its length a multiple of 4
                switch (rawMessageData.Length % 4)
                {
                    case 2: rawMessageData += "=="; break;
                    case 3: rawMessageData += "="; break;
                }

                var decodedBytes = Convert.FromBase64String(rawMessageData);

                var email = MimeMessage.Load(new MemoryStream(Convert.FromBase64String(rawMessageData)));

                var emailStore = new { email.MessageId, email.Subject, email.To, email.From, email.Date, email.TextBody, email.HtmlBody, email.Headers, email.Attachments };

                //var emailData = JsonConvert.SerializeObject(email);

                report.Add(emailStore.Subject);
            }
        }
        else
        {
            report.Add("No messages found.");
        }

        return report;
    }

}
