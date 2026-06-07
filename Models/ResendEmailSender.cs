// =============================================================
// File: Services/ResendEmailSender.cs
// Path: DunesOfArabia/Services/ResendEmailSender.cs
// =============================================================

using DunesOfArabia.Models;  // FIX: ApplicationUser lives in Models, not Data
using Microsoft.AspNetCore.Identity;
using Resend;

namespace DunesOfArabia.Services;

public class ResendEmailSender : IEmailSender<ApplicationUser>
{
    private readonly IResend _resend;
    private readonly IConfiguration _config;
    private readonly ILogger<ResendEmailSender> _logger;

    public ResendEmailSender(
        IResend resend,
        IConfiguration config,
        ILogger<ResendEmailSender> logger)
    {
        _resend = resend;
        _config = config;
        _logger = logger;
    }

    // ── Called by Identity on registration ───────────────────
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        SendAsync(
            to: email,
            subject: "Confirm your Dunes of Arabia account",
            html: $@"
<div style='font-family:Inter,Segoe UI,sans-serif;max-width:560px;margin:0 auto;background:#fff;border-radius:12px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,.08)'>
  <div style='background:#7B2D10;padding:32px;text-align:center'>
    <h1 style='color:#fff;font-family:Georgia,serif;font-size:26px;letter-spacing:3px;margin:0'>SAUDI HERITAGE</h1>
    <p style='color:#E8D9C5;margin:8px 0 0;font-size:13px'>Dunes of Arabia · Travel Platform</p>
  </div>
  <div style='padding:40px 36px'>
    <h2 style='color:#1C0F07;font-family:Georgia,serif;font-size:20px;margin:0 0 14px'>Welcome, {user.FullName ?? email}! 🎉</h2>
    <p style='color:#5A3E30;line-height:1.7;margin:0 0 24px'>
      Thank you for joining us. Please confirm your email address to activate your account
      and start exploring Saudi Arabia's most breathtaking destinations.
    </p>
    <div style='text-align:center;margin:32px 0'>
      <a href='{confirmationLink}'
         style='display:inline-block;background:#7B2D10;color:#fff;padding:14px 36px;border-radius:8px;text-decoration:none;font-weight:700;font-size:15px;letter-spacing:.5px'>
        Confirm My Email
      </a>
    </div>
    <p style='color:#8B6650;font-size:12px;line-height:1.6'>
      If the button doesn't work, copy and paste this link:<br/>
      <a href='{confirmationLink}' style='color:#7B2D10;word-break:break-all'>{confirmationLink}</a>
    </p>
    <hr style='border:none;border-top:1px solid #F0E6D8;margin:28px 0'/>
    <p style='color:#BBA898;font-size:11px;margin:0'>If you didn't create this account, ignore this email.</p>
  </div>
</div>");

    // ── Called by Identity on forgot password ─────────────────
    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        SendAsync(
            to: email,
            subject: "Reset your Dunes of Arabia password",
            html: $@"
<div style='font-family:Inter,Segoe UI,sans-serif;max-width:560px;margin:0 auto;background:#fff;border-radius:12px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,.08)'>
  <div style='background:#7B2D10;padding:32px;text-align:center'>
    <h1 style='color:#fff;font-family:Georgia,serif;font-size:26px;letter-spacing:3px;margin:0'>SAUDI HERITAGE</h1>
    <p style='color:#E8D9C5;margin:8px 0 0;font-size:13px'>Dunes of Arabia · Travel Platform</p>
  </div>
  <div style='padding:40px 36px'>
    <h2 style='color:#1C0F07;font-family:Georgia,serif;font-size:20px;margin:0 0 14px'>Password Reset Request 🔑</h2>
    <p style='color:#5A3E30;line-height:1.7;margin:0 0 24px'>
      We received a request to reset the password for <strong>{email}</strong>.
      Click the button below — this link expires in <strong>24 hours</strong>.
    </p>
    <div style='text-align:center;margin:32px 0'>
      <a href='{resetLink}'
         style='display:inline-block;background:#7B2D10;color:#fff;padding:14px 36px;border-radius:8px;text-decoration:none;font-weight:700;font-size:15px;letter-spacing:.5px'>
        Reset My Password
      </a>
    </div>
    <p style='color:#8B6650;font-size:12px;line-height:1.6'>
      If the button doesn't work, copy and paste this link:<br/>
      <a href='{resetLink}' style='color:#7B2D10;word-break:break-all'>{resetLink}</a>
    </p>
    <hr style='border:none;border-top:1px solid #F0E6D8;margin:28px 0'/>
    <p style='color:#BBA898;font-size:11px;margin:0'>If you didn't request this, ignore this email. Your password won't change.</p>
  </div>
</div>");

    // ── Called by Identity for OTP-style reset code ───────────
    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        SendAsync(
            to: email,
            subject: "Your Dunes of Arabia password reset code",
            html: $@"
<div style='font-family:Inter,Segoe UI,sans-serif;max-width:560px;margin:0 auto;background:#fff;border-radius:12px;overflow:hidden;'>
  <div style='background:#7B2D10;padding:32px;text-align:center'>
    <h1 style='color:#fff;font-family:Georgia,serif;font-size:26px;letter-spacing:3px;margin:0'>SAUDI HERITAGE</h1>
  </div>
  <div style='padding:40px 36px;text-align:center'>
    <h2 style='color:#1C0F07;font-family:Georgia,serif;font-size:20px;margin:0 0 14px'>Your Reset Code</h2>
    <p style='color:#5A3E30;line-height:1.7;margin:0 0 24px'>Enter this code on the reset page:</p>
    <div style='background:#FBF7F2;border:2px solid #E5D8C8;border-radius:12px;padding:24px;margin:0 0 20px;display:inline-block;min-width:220px'>
      <span style='font-size:34px;font-weight:700;color:#7B2D10;letter-spacing:8px;font-family:Courier New,monospace'>{resetCode}</span>
    </div>
    <p style='color:#8B6650;font-size:12px'>Expires in 15 minutes. Never share this with anyone.</p>
  </div>
</div>");

    // ── Core send ─────────────────────────────────────────────
    private async Task SendAsync(string to, string subject, string html)
    {
        var from = _config["Resend:FromEmail"]
            ?? throw new InvalidOperationException(
                "Resend:FromEmail is not configured. Add it to appsettings.json or user-secrets.");
        var fromName = _config["Resend:FromName"] ?? "Dunes of Arabia";

        var message = new EmailMessage
        {
            From = $"{fromName} <{from}>",
            Subject = subject,
            HtmlBody = html,
        };
        message.To.Add(to);

        try
        {
            var response = await _resend.EmailSendAsync(message);
            _logger.LogInformation(
                "Resend: email sent to {Email} (id={Id})", to, response.Content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Resend: failed to send email to {Email} — subject: {Subject}", to, subject);
            throw new InvalidOperationException(
                $"Email send failed: {ex.Message}. Check your Resend API key and sender address.", ex);
        }
    }
}