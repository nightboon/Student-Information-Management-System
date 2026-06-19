<%@ Page Language="C#" MasterPageFile="~/site.master" AutoEventWireup="true" CodeBehind="email_test.aspx.cs" Inherits="src.shared.email_test" Title="Email Test - INTI Student Portal" %>

<asp:Content ContentPlaceHolderID="BodyPlaceholder" runat="server">

    <div class="mx-auto max-w-3xl px-6 py-10">
        <p class="text-slate-500" style="font-size:13px;font-weight:500">Admin</p>
        <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">Send test email</h1>
        <p class="mt-1 text-slate-500" style="font-size:14px">Send a real email of each branded template to verify SMTP and rendering. Templates use sample data.</p>

        <div class="mt-6">
        <section class="rounded-lg border border-slate-200 bg-white">
            <div class="border-b border-slate-100 px-6 py-4">
                <h2 class="text-slate-900" style="font-size:15px;font-weight:700">Message</h2>
            </div>
            <div class="px-6 py-6" style="display:grid;grid-template-columns:1fr;gap:16px;max-width:640px">

                <asp:Panel ID="ResultOkPanel" runat="server" Visible="false"
                    CssClass="flex items-start gap-2 rounded-md border border-emerald-100 bg-emerald-50 px-3 py-2 text-emerald-700" style="font-size:12.5px">
                    <i data-lucide="check-circle" class="h-4 w-4 mt-0.5"></i>
                    <span>Email sent successfully.</span>
                </asp:Panel>

                <asp:Panel ID="ResultErrorPanel" runat="server" Visible="false"
                    CssClass="flex items-start gap-2 rounded-md border border-[#e0162b]/20 bg-[#e0162b]/5 px-3 py-2 text-[#a01020]" style="font-size:12.5px">
                    <i data-lucide="alert-circle" class="h-4 w-4 mt-0.5"></i>
                    <asp:Literal ID="ErrorText" runat="server" />
                </asp:Panel>

                <label class="block">
                    <span class="text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">TO</span>
                    <asp:TextBox ID="ToBox" runat="server" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                </label>

                <label class="block">
                    <span class="text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">TEMPLATE</span>
                    <asp:DropDownList ID="TemplateDropDown" runat="server" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900" style="font-size:13px">
                        <asp:ListItem Value="newaccount" Text="New account / welcome" />
                        <asp:ListItem Value="reset" Text="Password reset" />
                        <asp:ListItem Value="notification" Text="Notification" />
                    </asp:DropDownList>
                    <span class="text-slate-400" style="font-size:11.5px">Only the fields used by the selected template are sent.</span>
                </label>

                <div style="border-top:1px solid #f1f5f9;padding-top:4px"></div>
                <p class="text-slate-500" style="font-size:11.5px;font-weight:700;letter-spacing:0.04em">NEW ACCOUNT &amp; RESET</p>

                <label class="block">
                    <span class="text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">STUDENT NAME</span>
                    <asp:TextBox ID="StudentNameBox" runat="server" Text="Aisyah Rahman" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                </label>

                <label class="block">
                    <span class="text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">STUDENT EMAIL (shown in welcome body)</span>
                    <asp:TextBox ID="StudentEmailBox" runat="server" Text="p2300456@student.inti.edu.my" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                </label>

                <label class="block">
                    <span class="text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">TEMPORARY PASSWORD</span>
                    <asp:TextBox ID="TempPasswordBox" runat="server" Text="Xy7$kPm2" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                </label>

                <label class="block">
                    <span class="text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">RESET URL</span>
                    <asp:TextBox ID="ResetUrlBox" runat="server" Text="https://portal.inti.edu.my/reset?token=demo" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                </label>

                <div style="border-top:1px solid #f1f5f9;padding-top:4px"></div>
                <p class="text-slate-500" style="font-size:11.5px;font-weight:700;letter-spacing:0.04em">NOTIFICATION</p>

                <label class="block">
                    <span class="text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">TITLE</span>
                    <asp:TextBox ID="TitleBox" runat="server" Text="Semester 2 Results Released" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                </label>

                <label class="block">
                    <span class="text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">DETAIL</span>
                    <asp:TextBox ID="DetailBox" runat="server" TextMode="MultiLine" Rows="4" Text="Your results for May 2026 are now available.&#10;Sign in to the portal to view your full transcript." CssClass="mt-1.5 w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                </label>

                <label class="block">
                    <span class="text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">PDF PATH (optional)</span>
                    <asp:TextBox ID="PdfPathBox" runat="server" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" placeholder="e.g. C:\inetpub\reports\sample.pdf" />
                </label>

                <div class="flex items-center justify-end pt-1">
                    <asp:Button ID="SendButton" runat="server" Text="Send email" OnClick="SendButton_Click"
                        CssClass="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors" style="font-size:13px;font-weight:600;border:none" />
                </div>
            </div>
        </section>
        </div>
    </div>

</asp:Content>
