<%@ Page Language="C#" MasterPageFile="~/shared/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_announcement.aspx.cs" Inherits="student_information_management_system.lecturer_announcements" Title="Announcements - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div><p class="text-slate-500" style="font-size:13px;font-weight:500">Lecturer Module</p><h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700">Course Announcements</h1><p class="mt-1 text-slate-500" style="font-size:14px">Compose targeted notices for enrolled students and pin important updates.</p></div>
    </div>
    <asp:Panel ID="statusBanner" runat="server" Visible="false" CssClass="mt-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800" style="font-size:13px;font-weight:600"><asp:Literal ID="statusMessage" runat="server" /></asp:Panel>

    <section class="mt-6 grid gap-6 lg:grid-cols-[360px_1fr]">
        <aside class="rounded-lg border border-slate-200 bg-white">
            <div class="border-b border-slate-100 px-6 py-4"><h2 class="text-slate-900" style="font-size:16px;font-weight:700">Compose</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">Target one course or all assigned courses.</p></div>
            <div class="space-y-4 px-6 py-5">
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">TARGET COURSE</span><asp:DropDownList ID="courseSelect" runat="server" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3" style="font-size:13px" /></label>
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">TITLE</span><asp:TextBox ID="titleInput" runat="server" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" placeholder="Announcement title" style="font-size:13px" /></label>
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">MESSAGE</span><asp:TextBox ID="messageInput" runat="server" TextMode="MultiLine" CssClass="mt-1.5 min-h-[140px] w-full rounded-md border border-slate-200 px-3 py-2" placeholder="Write the announcement content" style="font-size:13px" /></label>
                <label class="flex items-center gap-2 text-slate-700" style="font-size:13px;font-weight:600"><asp:CheckBox ID="pinnedInput" runat="server" CssClass="h-4 w-4 rounded border-slate-300 text-[#e0162b]" /> Pin this announcement</label>
                <asp:LinkButton ID="postButton" runat="server" OnClick="PostAnnouncement_Click" CssClass="inline-flex h-10 w-full items-center justify-center rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:700">Post to students</asp:LinkButton>
            </div>
        </aside>

        <div class="rounded-lg border border-slate-200 bg-white">
            <div class="flex flex-col gap-3 border-b border-slate-100 px-6 py-4 md:flex-row md:items-center md:justify-between">
                <div><h2 class="text-slate-900" style="font-size:16px;font-weight:700">Recent Announcements</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">Posted messages created by this lecturer.</p></div>
                <div class="relative"><i data-lucide="search" class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"></i><input data-filter-input data-filter-target="[data-announcement]" type="search" placeholder="Search announcements" class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 md:w-64" style="font-size:12.5px" /></div>
            </div>
            <asp:Panel ID="emptyPanel" runat="server" Visible="false" CssClass="px-6 py-10 text-center text-slate-500" style="font-size:13px">No announcements posted yet.</asp:Panel>
            <ul class="divide-y divide-slate-100">
                <asp:Repeater ID="announcementsRepeater" runat="server">
                    <ItemTemplate>
                        <li data-announcement data-filter-text='<%# Html(Eval("Title")) %> <%# Html(Eval("TargetCourses")) %>' class="px-6 py-5">
                            <div class="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
                                <div>
                                    <div class="flex flex-wrap items-center gap-2">
                                        <span class='<%# Convert.ToBoolean(Eval("IsPinned")) ? "rounded-md bg-[#e0162b]/10 px-2 py-1 text-[#a01020]" : "hidden" %>' style="font-size:11.5px;font-weight:700">PINNED</span>
                                        <span class="rounded-md bg-slate-100 px-2 py-1 text-slate-600" style="font-size:11.5px;font-weight:700"><%# Html(Eval("TargetCourses")) %></span>
                                        <span class="text-slate-400" style="font-size:12px"><%# RelativeTime(Eval("CreatedAt")) %></span>
                                    </div>
                                    <h3 class="mt-3 text-slate-900" style="font-size:16px;font-weight:700"><%# Html(Eval("Title")) %></h3>
                                    <p class="mt-1 text-slate-500" style="font-size:13px;line-height:1.6"><%# Html(Eval("Content")) %></p>
                                </div>
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
    </section>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/lecturer/lecturer-portal.js") %>"></script>
</asp:Content>
