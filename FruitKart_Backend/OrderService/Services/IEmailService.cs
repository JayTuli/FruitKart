namespace OrderService.Services
{
    public interface IEmailService
    {
        System.Threading.Tasks.Task SendOrderConfirmationAsync(
            string toEmail,
            string toName,
            int orderHeaderId,
            decimal orderTotal,
            int totalItems,
            DateTime orderDate
        );
    }
}