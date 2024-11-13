using Microsoft.Extensions.Configuration;

namespace App.Library.Google.Services.Tests
{
    public class GMailServiceTests
    {
        [Fact()]
        public void MainTest()
        {
            // Create a configuration builder
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Google:ClientSecretPath", "C:\\Users\\kbdav\\AppData\\Roaming\\Microsoft\\UserSecrets\\43046854-fa06-4890-bc9f-b1f7ffceff0c\\client_secret_308518579938-n821v7h7l9hl7ov9hr43p7k9iipa49ua.apps.googleusercontent.com.json" },
                { "Google:TokenPath", "token.json" },
                { "Google:ApplicationName", "Jobster07" }
            });

            // Build the configuration
            var config = configurationBuilder.Build();

            // Pass the configuration to the GMailService
            var mailService = new GMailService(config);

            var result = mailService.GetMail();

            // Assert.Fail to indicate the test needs implementation
            // Assert.Fail("This test needs an implementation");
        }
    }
}