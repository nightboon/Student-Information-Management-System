using System;
using src.services.email;

namespace src.shared
{
    /// <summary>
    /// Admin harness for sending a real test email of each branded template.
    /// All values are taken from the form; only the fields relevant to the
    /// selected template are used.
    /// </summary>
    public partial class email_test : src.security.AdminPage
    {
        protected void SendButton_Click(object sender, EventArgs e)
        {
            string to = ToBox.Text;
            EmailResult result;

            switch (TemplateDropDown.SelectedValue)
            {
                case "newaccount":
                    result = EmailService.SendNewAccount(
                        to, StudentNameBox.Text, StudentEmailBox.Text, TempPasswordBox.Text);
                    break;
                case "reset":
                    result = EmailService.SendPasswordReset(to, StudentNameBox.Text, ResetUrlBox.Text);
                    break;
                default: // "notification"
                    string pdfPath = string.IsNullOrWhiteSpace(PdfPathBox.Text)
                        ? null : PdfPathBox.Text.Trim();
                    result = EmailService.SendNotification(to, TitleBox.Text, DetailBox.Text, pdfPath);
                    break;
            }

            ResultOkPanel.Visible = result.Success;
            ResultErrorPanel.Visible = !result.Success;
            if (!result.Success)
            {
                ErrorText.Text = Server.HtmlEncode(result.Error);
            }
        }
    }
}
