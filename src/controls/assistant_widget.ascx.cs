using System;
using System.Web.UI;

namespace src.controls
{
    /// <summary>
    /// Floating chat-assistant widget. The markup is fixed; only the endpoint,
    /// title and greeting vary by the session role so the same control serves
    /// students and lecturers. The actual chat logic lives in the central
    /// endpoint at ~/assistant/chat.aspx (role-aware server-side).
    /// </summary>
    public partial class assistant_widget : UserControl
    {
        protected void Page_Load(object sender, EventArgs e) { }

        private bool IsLecturer
        {
            get { return string.Equals(Session["role"] as string, "LECTURER", StringComparison.OrdinalIgnoreCase); }
        }

        // App-resolved URL of the shared chat page method, valid from any folder depth.
        protected string EndpointUrl
        {
            get { return ResolveUrl("~/assistant/chat.aspx/Chat"); }
        }

        protected string AssistantTitle
        {
            get { return IsLecturer ? "Teaching Assistant" : "Study Assistant"; }
        }

        protected string GreetingText
        {
            get
            {
                return IsLecturer
                    ? "Hi! Ask me about your courses or students who may be at risk."
                    : "Hi! Ask me about your classes, grades, attendance or assignments.";
            }
        }

        protected string InputPlaceholder
        {
            get { return IsLecturer ? "Ask anything, e.g. my courses" : "Ask anything, e.g. classes today"; }
        }
    }
}
