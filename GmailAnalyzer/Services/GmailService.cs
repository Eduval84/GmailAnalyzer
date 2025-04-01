using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GmailAnalyzer.Services
{
    public class GmailService
    {
        private readonly string _email;
        private readonly string _password;
        private const string ImapServer = "imap.gmail.com";
        private const int ImapPort = 993;

        public GmailService(string email, string password)
        {
            _email = email;
            _password = password;
        }

        public async Task<List<MimeMessage>> GetUnreadEmails()
        {
            using var client = new ImapClient();
            await client.ConnectAsync(ImapServer, ImapPort, true);
            
            // Para cuentas de Workspace, usa la contraseña normal
            // Para cuentas personales, necesitarás una contraseña de aplicación
            await client.AuthenticateAsync(_email, _password);

            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly);

            var unread = await inbox.SearchAsync(SearchQuery.NotSeen);
            var messages = new List<MimeMessage>();

            foreach (var uid in unread)
            {
                messages.Add(await inbox.GetMessageAsync(uid));
            }

            await client.DisconnectAsync(true);
            return messages;
        }
    }
}
