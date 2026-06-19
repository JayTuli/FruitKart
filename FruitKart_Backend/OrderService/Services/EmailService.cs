using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

using SystemTask = System.Threading.Tasks.Task;

namespace OrderService.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //  Return SystemTask instead of Task to avoid ambiguity
        public async SystemTask SendOrderConfirmationAsync(
            string toEmail,
            string toName,
            int orderHeaderId,
            decimal orderTotal,
            int totalItems,
            DateTime orderDate)
        {
            try
            {
                Configuration.Default.ApiKey["api-key"] =
                    _config["Brevo:ApiKey"]
                    ?? throw new InvalidOperationException("Brevo:ApiKey not configured");

                var apiInstance = new TransactionalEmailsApi();

                var sendSmtpEmail = new SendSmtpEmail(
                    sender: new SendSmtpEmailSender(
                        name: _config["Brevo:FromName"] ?? "FruitKart",
                        email: _config["Brevo:FromEmail"] ?? "noreply@fruitkart.com"
                    ),
                    to: new List<SendSmtpEmailTo>
                    {
                        new SendSmtpEmailTo(email: toEmail, name: toName)
                    },
                    subject: $"Order Confirmed! #{orderHeaderId} - FruitKart",
                    htmlContent: BuildEmailHtml(
                        toName, orderHeaderId, orderTotal, totalItems, orderDate)
                );

                await apiInstance.SendTransacEmailAsync(sendSmtpEmail);

                _logger.LogInformation(
                    "Order confirmation email sent to {Email} for Order #{OrderId}",
                    toEmail, orderHeaderId);
            }
            catch (Exception ex)
            {
                //  Never crash the order if email fails — just log it
                _logger.LogError(ex,
                    "Failed to send order confirmation email to {Email}", toEmail);
            }
        }

        private static string BuildEmailHtml(
            string toName,
            int orderHeaderId,
            decimal orderTotal,
            int totalItems,
            DateTime orderDate)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8' />
  <meta name='viewport' content='width=device-width, initial-scale=1.0' />
</head>
<body style='margin:0; padding:0; background-color:#f4f4f4; font-family: Arial, sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0'>
    <tr>
      <td align='center' style='padding: 40px 0;'>
        <table width='600' cellpadding='0' cellspacing='0'
               style='background:#ffffff; border-radius:12px;
                      overflow:hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>

          <!-- Header -->
          <tr>
            <td style='background-color:#2e7d32; padding:30px; text-align:center;'>
              <h1 style='color:#ffffff; margin:0; font-size:28px;'>FruitKart</h1>
              <p style='color:#a5d6a7; margin:6px 0 0; font-size:14px;'>
                Fresh fruits delivered to you
              </p>
            </td>
          </tr>

          <!-- Success Banner -->
          <tr>
            <td style='background-color:#e8f5e9; padding:24px; text-align:center;'>
              <h2 style='color:#2e7d32; margin:0; font-size:24px;'>Order Confirmed!</h2>
              <p style='color:#555; margin:8px 0 0; font-size:15px;'>
                Hi <strong>{toName}</strong>, thank you for shopping with FruitKart!
              </p>
            </td>
          </tr>

          <!-- Order Details -->
          <tr>
            <td style='padding:30px;'>
              <h3 style='color:#333; margin:0 0 16px; font-size:16px;
                          border-bottom:2px solid #e8f5e9; padding-bottom:10px;'>
                Order Summary
              </h3>
              <table width='100%' cellpadding='0' cellspacing='0'>
                <tr>
                  <td style='padding:10px 0; color:#666; font-size:14px;
                              border-bottom:1px solid #f0f0f0;'>Order ID</td>
                  <td style='padding:10px 0; text-align:right; font-weight:bold;
                              font-size:14px; border-bottom:1px solid #f0f0f0;'>
                    #{orderHeaderId}
                  </td>
                </tr>
                <tr>
                  <td style='padding:10px 0; color:#666; font-size:14px;
                              border-bottom:1px solid #f0f0f0;'>Order Date</td>
                  <td style='padding:10px 0; text-align:right; font-weight:bold;
                              font-size:14px; border-bottom:1px solid #f0f0f0;'>
                    {orderDate:dd MMM yyyy, hh:mm tt}
                  </td>
                </tr>
                <tr>
                  <td style='padding:10px 0; color:#666; font-size:14px;
                              border-bottom:1px solid #f0f0f0;'>Total Items</td>
                  <td style='padding:10px 0; text-align:right; font-weight:bold;
                              font-size:14px; border-bottom:1px solid #f0f0f0;'>
                    {totalItems} item(s)
                  </td>
                </tr>
                <tr>
                  <td style='padding:14px 0; color:#333; font-size:16px; font-weight:bold;'>
                    Order Total
                  </td>
                  <td style='padding:14px 0; text-align:right;
                              font-size:22px; font-weight:bold; color:#2e7d32;'>
                    Rs.{orderTotal:F2}
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Ready Time Banner -->
          <tr>
            <td style='padding:0 30px 30px;'>
              <div style='background:#e8f5e9; border-left:4px solid #2e7d32;
                           padding:16px; border-radius:6px;'>
                <p style='margin:0; color:#2e7d32; font-size:14px;'>
                  <strong>Ready in 15-20 minutes</strong>
                  after order confirmation.
                  Please carry your Order ID <strong>#{orderHeaderId}</strong>
                  when picking up.
                </p>
              </div>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style='background:#f9f9f9; padding:20px; text-align:center;
                        border-top:1px solid #eee;'>
              <p style='color:#999; font-size:12px; margin:0;'>
                This is an automated email from FruitKart. Please do not reply.
              </p>
              <p style='color:#999; font-size:12px; margin:6px 0 0;'>
                Best in your town | Contact Us: +91 (89456-89456)
              </p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        }
    }
}