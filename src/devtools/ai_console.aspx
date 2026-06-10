<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ai_console.aspx.cs" Inherits="src.devtools.ai_console" Title="AI Console" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="robots" content="noindex" />
    <title>AI Console</title>
    <style>
        * { box-sizing: border-box; }
        body { font-family: system-ui, -apple-system, "Segoe UI", sans-serif; font-size: 14px;
               color: #1e293b; background: #f8fafc; margin: 0; padding: 24px; }
        .card { background: #fff; border: 1px solid #e2e8f0; border-radius: 8px; padding: 20px;
                max-width: 1100px; margin: 0 auto 16px; }
        h1 { font-size: 18px; margin: 0; }
        h2 { font-size: 15px; margin: 0 0 12px; }
        label { display: inline-block; width: 80px; color: #64748b; }
        .row { margin-bottom: 10px; }
        input[type=text], input[type=password] { width: 420px; max-width: 100%; padding: 6px 8px;
                border: 1px solid #cbd5e1; border-radius: 6px; font: inherit; }
        .btn { padding: 6px 14px; border: 1px solid #cbd5e1; border-radius: 6px; background: #fff;
               font: inherit; cursor: pointer; }
        .btn:hover { background: #f1f5f9; }
        .btn-primary { background: #1e293b; border-color: #1e293b; color: #fff; }
        .btn-primary:hover { background: #334155; }
        .msg { color: #16a34a; margin-left: 10px; }
        .err { color: #dc2626; margin-left: 10px; }
        .topbar { display: flex; justify-content: space-between; align-items: center;
                  max-width: 1100px; margin: 0 auto 16px; }
        table { width: 100%; border-collapse: collapse; font-size: 13px; }
        th, td { border-bottom: 1px solid #e2e8f0; padding: 6px 8px; text-align: left;
                 vertical-align: top; }
        th { color: #64748b; font-weight: 600; background: #f8fafc; }
        td.clip { max-width: 320px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
        tr.summary { cursor: pointer; }
        tr.summary:hover { background: #f8fafc; }
        tr.detail { display: none; }
        tr.detail.open { display: table-row; }
        tr.detail td { background: #f8fafc; white-space: pre-wrap; word-break: break-word;
                       color: #334155; }
        .gate { max-width: 320px; margin: 120px auto 0; }
        .muted { color: #94a3b8; }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <%-- Password gate --%>
        <asp:Panel ID="gatePanel" runat="server" CssClass="card gate" DefaultButton="enterBtn">
            <h1>AI Console</h1>
            <div class="row" style="margin-top: 14px">
                <asp:TextBox ID="passwordBox" runat="server" TextMode="Password" placeholder="Password" Width="100%" />
            </div>
            <asp:Button ID="enterBtn" runat="server" Text="Enter" CssClass="btn btn-primary" OnClick="EnterBtn_Click" />
            <asp:Label ID="gateError" runat="server" CssClass="err" />
        </asp:Panel>

        <%-- Console --%>
        <asp:Panel ID="consolePanel" runat="server" Visible="false">
            <div class="topbar">
                <h1>AI Console</h1>
                <asp:Button ID="logoutBtn" runat="server" Text="Logout" CssClass="btn" OnClick="LogoutBtn_Click" />
            </div>

            <div class="card">
                <h2>Settings</h2>
                <div class="row"><label>Base URL</label><asp:TextBox ID="baseUrlBox" runat="server" /></div>
                <div class="row"><label>API Key</label><asp:TextBox ID="apiKeyBox" runat="server" /></div>
                <div class="row"><label>Model</label><asp:TextBox ID="modelBox" runat="server" /></div>
                <asp:Button ID="saveBtn" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="SaveBtn_Click" />
                <asp:Button ID="resetBtn" runat="server" Text="Reset to default" CssClass="btn" OnClick="ResetBtn_Click"
                    OnClientClick="return confirm('Clear Base URL, API Key and Model?');" />
                <asp:Label ID="saveMsg" runat="server" CssClass="msg" />
            </div>

            <div class="card">
                <h2 style="display:inline-block">Chat Logs <span class="muted">(latest 100 conversations, click a row to expand)</span></h2>
                <asp:Button ID="refreshBtn" runat="server" Text="Refresh" CssClass="btn" style="float:right" OnClick="RefreshBtn_Click" />
                <asp:Repeater ID="logsRepeater" runat="server">
                    <HeaderTemplate>
                        <table>
                            <tr><th style="width:130px">Last active</th><th style="width:140px">Student</th><th>First question</th><th style="width:55px">Turns</th><th style="width:200px">Tools</th><th style="width:80px">Total ms</th></tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="summary" onclick="this.nextElementSibling.classList.toggle('open')">
                            <td><%# ((DateTime)Eval("UpdatedAt")).ToString("MM-dd HH:mm:ss") %></td>
                            <td class="clip"><%# Server.HtmlEncode((string)Eval("StudentLabel")) %></td>
                            <td class="clip"><%# Server.HtmlEncode((string)Eval("FirstQuestion")) %></td>
                            <td><%# Eval("Turns") %></td>
                            <td class="clip"><%# Server.HtmlEncode((string)Eval("ToolsUsed")) %></td>
                            <td><%# Eval("DurationMs") %></td>
                        </tr>
                        <tr class="detail">
                            <td colspan="6"><%# Server.HtmlEncode((string)Eval("Transcript")) %></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate></table></FooterTemplate>
                </asp:Repeater>
                <asp:Label ID="emptyLabel" runat="server" CssClass="muted" Visible="false" Text="No logs yet." />
            </div>
        </asp:Panel>

    </form>
</body>
</html>
