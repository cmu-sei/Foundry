using System.Threading.Tasks;

namespace AppMailClient
{
    public interface IAppMailClient
    {
        Task<MailMessageStatus> Send(MailMessage message);
        Task<MailMessageStatus> Status(string referenceId);
    }
}
