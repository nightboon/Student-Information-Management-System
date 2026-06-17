using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using src.services;

namespace src.devtools
{
    /// <summary>
    /// Developer-only console for the AI assistant: edit the OpenAI-compatible
    /// endpoint settings (stored in the gitignored ai.config) and inspect chat
    /// logs. Guarded by a fixed password, kept as a SHA-256 hash only; access is
    /// remembered in the session. Not linked from any sidebar.
    /// </summary>
    public partial class ai_console : System.Web.UI.Page
    {
        // SHA-256 of the console password.
        private const string PasswordHash =
            "1f3ce40415a2081fa3eee75fc39fff8e56c22270d1a978a7249b592dcebd20b4";

        private const string SessionFlag = "ai_console_ok";
        private const int LogPageSize = 100;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            bool unlocked = Session[SessionFlag] != null;
            gatePanel.Visible = !unlocked;
            consolePanel.Visible = unlocked;

            if (unlocked && !IsPostBack)
            {
                LoadSettings();
                BindLogs();
            }
        }

        protected void EnterBtn_Click(object sender, EventArgs e)
        {
            if (Sha256Hex(passwordBox.Text ?? "") == PasswordHash)
            {
                Session[SessionFlag] = true;
                Response.Redirect(Request.RawUrl);
                return;
            }
            gateError.Text = "Wrong password.";
        }

        protected void LogoutBtn_Click(object sender, EventArgs e)
        {
            Session.Remove(SessionFlag);
            Response.Redirect(Request.RawUrl);
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            AiConsoleService.SaveSettings(baseUrlBox.Text, apiKeyBox.Text, modelBox.Text);
            saveMsg.Text = "Saved.";
            BindLogs();
        }

        protected void ResetBtn_Click(object sender, EventArgs e)
        {
            AiConsoleService.ResetSettings();
            LoadSettings();
            saveMsg.Text = "Settings cleared.";
            BindLogs();
        }

        protected void RefreshBtn_Click(object sender, EventArgs e)
        {
            BindLogs();
        }

        private void LoadSettings()
        {
            baseUrlBox.Text = AiConsoleService.GetSetting("Ai:BaseUrl");
            apiKeyBox.Text = AiConsoleService.GetSetting("Ai:ApiKey");
            modelBox.Text = AiConsoleService.GetSetting("Ai:Model");
        }

        private void BindLogs()
        {
            var logs = AiConsoleService.GetRecentLogs(LogPageSize);
            logsRepeater.DataSource = logs;
            logsRepeater.DataBind();
            emptyLabel.Visible = logs.Count == 0;
        }

        private static string Sha256Hex(string value)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
