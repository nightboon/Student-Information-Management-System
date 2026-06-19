using System;
using System.Web;
using System.Web.UI;

namespace src.security
{
    /// <summary>
    /// Base page for any authenticated page. Enforces a logged-in session and
    /// centralizes no-cache headers. Subclasses override EnforceRole() to gate by role.
    /// </summary>
    public abstract class SecurePage : Page
    {
        protected string CurrentRole
        {
            get { return Session["role"] as string; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetNoStore();

            if (Session["user_id"] == null)
            {
                Response.Redirect("~/login/login.aspx");
                return;
            }

            EnforceRole();
        }

        // Default: any logged-in user is allowed. Role pages override this.
        protected virtual void EnforceRole() { }
    }
}
