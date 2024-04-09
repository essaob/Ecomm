namespace Ecomm.Repository.IRepository
{
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
    }
}
