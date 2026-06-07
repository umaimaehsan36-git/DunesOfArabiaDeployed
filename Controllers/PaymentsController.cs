using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace DunesOfArabia.Controllers;

[ApiController]
[Route("api/[controller]")]          // → /api/payments
[IgnoreAntiforgeryToken]             // HttpClient POSTs from Blazor don't carry an AV token
public class PaymentsController : ControllerBase
{
    // POST /api/payments/create-intent
    [HttpPost("create-intent")]
    [AllowAnonymous]                 // Auth is enforced at the Blazor page level, not here
    public async Task<IActionResult> CreateIntent([FromBody] CreateIntentRequest request)
    {
        if (request.AmountSar <= 0)
            return BadRequest(new { error = "Amount must be greater than zero." });

        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount      = request.AmountSar,           // already in halalas (SAR × 100)
                Currency    = request.Currency ?? "sar",
                Description = request.Description,
                ReceiptEmail = request.CustomerEmail,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };

            var service = new PaymentIntentService();
            var intent  = await service.CreateAsync(options);

            return Ok(new { clientSecret = intent.ClientSecret });
        }
        catch (StripeException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    public record CreateIntentRequest(
        long   AmountSar,
        string? Currency,
        string? Description,
        string? CustomerEmail);
}
