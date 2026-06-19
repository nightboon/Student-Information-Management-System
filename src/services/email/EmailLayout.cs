using System.Net;

namespace src.services.email
{
    /// <summary>
    /// The single branded, email-safe shell for every outgoing email: table-based
    /// layout, all-inline CSS, 600px card, white header with the inline-CID INTI
    /// logo and a red accent rule, muted footer. This is the ONLY place
    /// header/footer/button markup lives. The logo image itself is attached by
    /// EmailService as a LinkedResource with ContentId "logo".
    /// </summary>
    public static class EmailLayout
    {
        public const string BrandName = "INTI Student Portal";
        public const string PrimaryColor = "#e0162b";
        public const string LogoContentId = "logo";

        private const string FooterNote = "© 2026 INTI Student Portal";

        /// <param name="preheader">Short inbox preview text (hidden in the body).</param>
        /// <param name="innerHtml">Body HTML produced by a builder.</param>
        public static string Render(string preheader, string innerHtml)
        {
            return
"<!DOCTYPE html><html><head><meta charset=\"utf-8\">"
+ "<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"></head>"
+ "<body style=\"margin:0;padding:24px;background:#eef1f4;"
+ "font-family:-apple-system,Segoe UI,Roboto,Helvetica,Arial,sans-serif;\">"
+ "<span style=\"display:none;max-height:0;overflow:hidden;opacity:0;color:#eef1f4;\">"
+ WebUtility.HtmlEncode(preheader) + "</span>"
+ "<table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\" "
+ "style=\"margin:0 auto;background:#ffffff;border-radius:10px;overflow:hidden;"
+ "border:1px solid #e2e6ea;\">"
+ "<tr><td style=\"padding:24px 32px 18px;background:#ffffff;"
+ "border-bottom:3px solid " + PrimaryColor + ";\">"
+ "<img src=\"cid:" + LogoContentId + "\" alt=\"" + WebUtility.HtmlEncode(BrandName)
+ "\" height=\"34\" style=\"display:block;border:0;\" /></td></tr>"
+ "<tr><td style=\"padding:32px;\">" + innerHtml + "</td></tr>"
+ "<tr><td style=\"background:#f5f7fa;padding:18px 32px;border-top:1px solid #e2e6ea;\">"
+ "<p style=\"margin:0;font-size:12px;line-height:1.6;color:#9aa5b1;\">"
+ WebUtility.HtmlEncode(FooterNote)
+ " · This is an automated message — please do not reply.</p>"
+ "</td></tr></table></body></html>";
        }

        /// <summary>Email-safe primary button (table cell so Outlook renders the fill).</summary>
        public static string Button(string text, string url)
        {
            return "<table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\"><tr>"
                 + "<td style=\"background:" + PrimaryColor + ";border-radius:6px;\">"
                 + "<a href=\"" + WebUtility.HtmlEncode(url) + "\" style=\"display:inline-block;"
                 + "padding:13px 28px;color:#ffffff;font-size:15px;font-weight:bold;"
                 + "text-decoration:none;\">" + WebUtility.HtmlEncode(text) + "</a></td></tr></table>";
        }
    }
}
