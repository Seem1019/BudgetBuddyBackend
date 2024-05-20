using Microsoft.AspNetCore.Mvc;

namespace BudgetBuddyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GmailIntegrationController : ControllerBase
    {
        private readonly GmailServiceHelper _gmailServiceHelper;

        public GmailIntegrationController(GmailServiceHelper gmailServiceHelper)
        {
            _gmailServiceHelper = gmailServiceHelper;
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions()
        {
            var service = await _gmailServiceHelper.GetGmailServiceAsync();
            var bankEmail = "alertasynotificaciones@notificacionesbancolombia.com"; // Reemplaza con la dirección de correo de tu banco
            var messages = await _gmailServiceHelper.GetBankTransactionsAsync(service, bankEmail);

            var transactions = messages.Select(async messageItem =>
            {
                var message = await _gmailServiceHelper.GetMessageAsync(service, messageItem.Id);
                return ExtractTransactionFromEmail(message);
            }).ToList();

            return Ok(await Task.WhenAll(transactions));
        }

        private Transaction ExtractTransactionFromEmail(Google.Apis.Gmail.v1.Data.Message message)
        {
            // Lógica para extraer información de transacción del contenido del correo electrónico
            var snippet = message.Snippet;
            // Implementa la lógica para parsear el contenido del correo y extraer la información de la transacción
            return new Transaction
            {
                // Asigna los valores extraídos del correo a las propiedades de la transacción
                Date = DateTime.Now, // Ejemplo
                Amount = 0, // Ejemplo
                Description = snippet // Ejemplo
            };
        }
    }

    public class Transaction
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
