using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace src.services.email
{
    /// <summary>Outcome of a send attempt. Never throws to callers.</summary>
    public class EmailResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
    }

    /// <summary>
    /// Reusable email facade for the whole app. Three send functions build one
    /// shared branded shell (EmailLayout), embed the INTI logo inline via CID,
    /// and send through the SMTP settings in smtp.config / mailSettings. Every
    /// caller-supplied scalar is HTML-encoded; detail text is treated as plain
    /// text (encoded), never raw HTML.
    /// </summary>
    public static class EmailService
    {
        // ---- Public send functions -----------------------------------------

        /// <summary>Welcome email sent to the student's PERSONAL inbox; body shows
        /// their student (login) email + temporary password.</summary>
        public static EmailResult SendNewAccount(string toPersonalEmail, string studentName,
            string studentEmail, string tempPassword)
        {
            string n = WebUtility.HtmlEncode(studentName);
            string e = WebUtility.HtmlEncode(studentEmail);
            string p = WebUtility.HtmlEncode(tempPassword);

            string inner =
                "<h1 style=\"margin:0 0 16px;font-size:22px;color:#1f2933;\">Welcome, " + n + "</h1>"
              + "<p style=\"margin:0 0 16px;font-size:15px;line-height:1.6;color:#3e4c59;\">"
              + "An account has been created for you on " + EmailLayout.BrandName
              + ". Use the credentials below to sign in for the first time.</p>"
              + "<table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" "
              + "style=\"background:#f5f7fa;border:1px solid #e2e6ea;border-radius:6px;margin:8px 0 20px;\">"
              + "<tr><td style=\"padding:18px 20px;\">"
              + "<p style=\"margin:0 0 8px;font-size:12px;text-transform:uppercase;letter-spacing:.6px;color:#7b8794;\">Student email</p>"
              + "<p style=\"margin:0 0 16px;font-size:15px;color:#1f2933;font-weight:bold;\">" + e + "</p>"
              + "<p style=\"margin:0 0 8px;font-size:12px;text-transform:uppercase;letter-spacing:.6px;color:#7b8794;\">Temporary password</p>"
              + "<p style=\"margin:0;font-size:18px;color:#1f2933;font-weight:bold;"
              + "font-family:'Courier New',monospace;letter-spacing:1px;\">" + p + "</p>"
              + "</td></tr></table>"
              + "<p style=\"margin:0 0 4px;font-size:13px;line-height:1.6;color:#7b8794;\">"
              + "For your security, please change this temporary password right after your first sign-in.</p>";

            var msg = new EmailMessage
            {
                Subject = "Welcome to " + EmailLayout.BrandName + " — your account is ready",
                Html = EmailLayout.Render("Your " + EmailLayout.BrandName + " account is ready", inner),
                Text = "Welcome, " + studentName + "\n\n"
                     + "An account has been created for you on " + EmailLayout.BrandName + ".\n\n"
                     + "Student email: " + studentEmail + "\n"
                     + "Temporary password: " + tempPassword + "\n\n"
                     + "Please change this temporary password right after your first sign-in."
            };
            return Send(toPersonalEmail, msg);
        }

        /// <summary>Password-reset email sent to the student email. The button links
        /// to the caller-supplied resetUrl (reset page is out of scope).</summary>
        public static EmailResult SendPasswordReset(string toStudentEmail, string studentName,
            string resetUrl)
        {
            string n = WebUtility.HtmlEncode(studentName);

            string inner =
                "<h1 style=\"margin:0 0 14px;font-size:20px;color:#1f2933;\">Reset your password</h1>"
              + "<p style=\"margin:0 0 18px;font-size:15px;line-height:1.6;color:#3e4c59;\">Hi " + n
              + ", we received a request to reset your " + EmailLayout.BrandName
              + " password. Click below to choose a new one.</p>"
              + EmailLayout.Button("Reset password", resetUrl)
              + "<p style=\"margin:18px 0 0;font-size:13px;line-height:1.6;color:#7b8794;\">"
              + "If you didn't request this, you can safely ignore this email — your password won't change.</p>";

            var msg = new EmailMessage
            {
                Subject = "Reset your " + EmailLayout.BrandName + " password",
                Html = EmailLayout.Render("Reset your " + EmailLayout.BrandName + " password", inner),
                Text = "Hi " + studentName + ",\n\n"
                     + "We received a request to reset your " + EmailLayout.BrandName + " password.\n\n"
                     + "Reset it here: " + resetUrl + "\n\n"
                     + "If you didn't request this, ignore this email."
            };
            return Send(toStudentEmail, msg);
        }

        /// <summary>Notification email sent to the student email. detail is plain text
        /// (HTML-encoded). pdfPath is optional; when set the file is attached.</summary>
        public static EmailResult SendNotification(string toStudentEmail, string title,
            string detail, string pdfPath = null)
        {
            string t = WebUtility.HtmlEncode(title);
            string d = WebUtility.HtmlEncode(detail);

            string inner =
                "<h1 style=\"margin:0 0 14px;font-size:20px;color:#1f2933;\">" + t + "</h1>"
              + "<p style=\"margin:0;font-size:15px;line-height:1.6;color:#3e4c59;white-space:pre-line;\">"
              + d + "</p>";

            var msg = new EmailMessage
            {
                Subject = title,
                Html = EmailLayout.Render(title, inner),
                Text = title + "\n\n" + detail,
                PdfPath = pdfPath
            };
            return Send(toStudentEmail, msg);
        }

        // ---- Private transport ---------------------------------------------

        private static EmailResult Send(string toEmail, EmailMessage msg)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                return Fail("Recipient address is required.");
            if (string.IsNullOrWhiteSpace(msg.Subject))
                return Fail("Subject is required.");

            // A provided-but-missing PDF path is an error (null is fine = no attachment).
            if (msg.PdfPath != null && !File.Exists(msg.PdfPath))
                return Fail("Attachment file not found: " + msg.PdfPath);

            try
            {
                // From is left unset: SmtpClient populates it from mailSettings/smtp "from".
                using (var message = new MailMessage())
                using (var client = new SmtpClient())
                {
                    message.To.Add(toEmail.Trim());
                    message.Subject = msg.Subject;

                    // Plain-text fallback view.
                    var textView = AlternateView.CreateAlternateViewFromString(
                        msg.Text ?? "", null, MediaTypeNames.Text.Plain);

                    // HTML view + inline logo (CID "logo").
                    var htmlView = AlternateView.CreateAlternateViewFromString(
                        msg.Html ?? "", null, MediaTypeNames.Text.Html);

                    string logoPath = HttpContext.Current.Server.MapPath("~/img/inti_logo.png");
                    var logo = new LinkedResource(logoPath, "image/png")
                    {
                        ContentId = EmailLayout.LogoContentId,
                        TransferEncoding = TransferEncoding.Base64
                    };
                    htmlView.LinkedResources.Add(logo);

                    message.AlternateViews.Add(textView);
                    message.AlternateViews.Add(htmlView);

                    if (msg.PdfPath != null)
                        message.Attachments.Add(new Attachment(msg.PdfPath));

                    client.Send(message);
                }
                return new EmailResult { Success = true };
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        private static EmailResult Fail(string error)
        {
            return new EmailResult { Success = false, Error = error };
        }
    }
}
