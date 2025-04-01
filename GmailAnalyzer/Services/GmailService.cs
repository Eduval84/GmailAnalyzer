using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace GmailAnalyzer.Services
{
    public class GmailService
    {
        private readonly GmailService _gmailService;

        public GmailService(string credentialsPath)
        {
            string[] Scopes = { GmailService.Scope.GmailReadOnly };
            UserCredential credential;

            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token.json", true)).Result;
            }

            _gmailService = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Gmail Analyzer"
            });
        }

        public async Task<List<Message>> GetUnreadEmails()
        {
            var request = _gmailService.Users.Messages.List("me");
            request.Q = "is:unread";
            var response = await request.ExecuteAsync();
            return response.Messages?.ToList() ?? new List<Message>();
        }
    }
}
