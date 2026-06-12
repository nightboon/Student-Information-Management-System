<%@ Page Language="C#" MasterPageFile="~/shared/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_announcement.aspx.cs" Inherits="student_information_management_system.lecturer_announcements" Title="Announcements - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="max-w-none">
        <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
            <div>
                <p class="text-slate-500" style="font-size:13px;font-weight:600">Course communication</p>
                <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:800;letter-spacing:-0.015em">Announcements</h1>
                <p class="mt-1 text-slate-500" style="font-size:14px">Announcements you have sent to your courses, plus pinned notices.</p>
            </div>
            <div class="flex flex-wrap items-center gap-2">
                <% if (IsCourseScoped) { %>
                <a href="<%= ResolveUrl("~/lecturer/lecturer_course_dashboard.aspx") %>?offering=<%= SelectedOfferingId %>" class="inline-flex h-10 items-center gap-2 rounded-md border border-slate-200 bg-white px-4 text-slate-600 hover:border-slate-300 hover:text-slate-900 transition-colors" style="font-size:13px;font-weight:700">
                    <i data-lucide="arrow-left" class="h-4 w-4"></i>
                    Course
                </a>
                <% } %>
                <asp:LinkButton ID="showComposeButton" runat="server" OnClick="ShowComposeButton_Click" CssClass="inline-flex h-10 items-center gap-2 rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020] transition-colors" style="font-size:13px;font-weight:800">
                    <i data-lucide="plus" class="h-4 w-4"></i>
                    Post announcement
                </asp:LinkButton>
            </div>
        </div>

        <asp:Panel ID="statusBanner" runat="server" Visible="false" CssClass="mt-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800" style="font-size:13px;font-weight:700">
            <asp:Literal ID="statusMessage" runat="server" />
        </asp:Panel>

        <asp:Panel ID="composePanel" runat="server" CssClass="mt-5 rounded-lg border border-slate-200 bg-white" data-compose-panel="true">
            <div class="flex items-center justify-between border-b border-slate-100 px-6 py-4">
                <div>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:800">Post announcement</h2>
                    <p class="mt-0.5 text-slate-500" style="font-size:12.5px">Attach PDF, DOCX, images, spreadsheets, or ZIP files.</p>
                </div>
                <asp:Button ID="closeComposeButton" runat="server" OnClick="CloseComposeButton_Click" Text="x" CssClass="inline-flex h-9 w-9 items-center justify-center rounded-md border-0 bg-transparent text-slate-400 hover:bg-slate-100 hover:text-slate-700 cursor-pointer" ToolTip="Close" style="font-size:22px;font-weight:600;line-height:1" UseSubmitBehavior="true" CausesValidation="false" />
            </div>
            <div class="grid gap-4 p-6 lg:grid-cols-[260px_1fr]">
                <label class="block">
                    <span class="text-slate-500" style="font-size:12px;font-weight:800">TARGET COURSE</span>
                    <asp:DropDownList ID="courseSelect" runat="server" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-800 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                </label>
                <label class="block">
                    <span class="text-slate-500" style="font-size:12px;font-weight:800">TITLE</span>
                    <asp:TextBox ID="titleInput" runat="server" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3 text-slate-800 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" placeholder="Announcement title" style="font-size:13px" />
                </label>
                <label class="block lg:col-span-2">
                    <span class="text-slate-500" style="font-size:12px;font-weight:800">MESSAGE</span>
                    <asp:TextBox ID="messageInput" runat="server" TextMode="MultiLine" CssClass="mt-1.5 min-h-[120px] w-full rounded-md border border-slate-200 px-3 py-2 text-slate-800 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" placeholder="Write the announcement content" style="font-size:13px;line-height:1.55" />
                </label>
                <label class="block">
                    <span class="text-slate-500" style="font-size:12px;font-weight:800">ATTACHMENT</span>
                    <asp:FileUpload ID="attachmentInput" runat="server" CssClass="mt-1.5 block w-full rounded-md border border-dashed border-slate-300 bg-slate-50 px-3 py-2 text-slate-600 file:mr-3 file:rounded-md file:border-0 file:bg-white file:px-3 file:py-1.5 file:text-slate-700" style="font-size:12.5px" />
                </label>
                <div class="flex flex-col justify-end gap-3 sm:flex-row sm:items-end sm:justify-between">
                    <label class="inline-flex items-center gap-2 text-slate-700" style="font-size:13px;font-weight:700">
                        <asp:CheckBox ID="pinnedInput" runat="server" CssClass="h-4 w-4 rounded border-slate-300 text-[#e0162b]" />
                        Pin this announcement
                    </label>
                    <asp:Button ID="postButton" runat="server" OnClick="PostAnnouncement_Click" Text="Post to students" CssClass="inline-flex h-10 items-center justify-center rounded-md border-0 bg-[#e0162b] px-5 text-white hover:bg-[#a01020] transition-colors cursor-pointer" style="font-size:13px;font-weight:800" UseSubmitBehavior="true" />
                </div>
            </div>
        </asp:Panel>

        <section class="mt-6 grid gap-4 xl:grid-cols-[360px_1fr]">
            <aside class="rounded-lg border border-slate-200 bg-white">
                <div class="border-b border-slate-100 p-3">
                    <label class="mb-3 block">
                        <span class="text-slate-500" style="font-size:11px;font-weight:900">COURSE</span>
                        <asp:DropDownList ID="courseFilterSelect" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CourseFilterSelect_SelectedIndexChanged" CssClass="mt-1 h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-800 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                    </label>
                    <div class="relative">
                        <i data-lucide="search" class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"></i>
                        <input data-filter-input data-announcement-search data-filter-target="[data-announcement-item]" type="search" placeholder="Search notifications..." class="h-10 w-full rounded-md border border-slate-200 bg-white pl-10 pr-3 text-slate-800 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                    </div>
                    <div class="mt-3 grid grid-cols-3 gap-1 rounded-md bg-slate-100 p-1" data-announcement-tabs>
                        <asp:LinkButton ID="allTabButton" runat="server" OnCommand="TabButton_Command" CommandArgument="all" data-tab-filter="all" CssClass="rounded px-2 py-1.5 text-center" style="font-size:12px;font-weight:800">All</asp:LinkButton>
                        <asp:LinkButton ID="pinnedTabButton" runat="server" OnCommand="TabButton_Command" CommandArgument="pinned" data-tab-filter="pinned" CssClass="rounded px-2 py-1.5 text-center" style="font-size:12px;font-weight:800">Pinned</asp:LinkButton>
                        <asp:LinkButton ID="filesTabButton" runat="server" OnCommand="TabButton_Command" CommandArgument="files" data-tab-filter="files" CssClass="rounded px-2 py-1.5 text-center" style="font-size:12px;font-weight:800">Files</asp:LinkButton>
                    </div>
                </div>

                <asp:Panel ID="emptyPanel" runat="server" Visible="false" CssClass="px-6 py-10 text-center text-slate-500" style="font-size:13px">No announcements posted yet.</asp:Panel>
                <asp:Repeater ID="announcementsRepeater" runat="server" OnItemCommand="AnnouncementsRepeater_ItemCommand">
                    <HeaderTemplate><ul class="max-h-[620px] overflow-y-auto divide-y divide-slate-100"></HeaderTemplate>
                    <ItemTemplate>
                        <li data-announcement-item data-pinned='<%# Convert.ToBoolean(Eval("IsPinned")) ? "true" : "false" %>' data-file='<%# Convert.ToBoolean(Eval("HasAttachment")) ? "true" : "false" %>' data-filter-text='<%# Html(Eval("Title")) %> <%# Html(Eval("TargetCourses")) %> <%# Html(Eval("Content")) %>' class='<%# SelectedClass(Eval("AnnouncementId")) %>'>
                            <asp:LinkButton runat="server" CommandName="SelectAnnouncement" CommandArgument='<%# Eval("AnnouncementId") %>' CssClass="block w-full px-5 py-4 text-left hover:bg-slate-50 transition-colors">
                                <div class="flex items-start justify-between gap-3">
                                    <div class="min-w-0">
                                        <div class="flex flex-wrap items-center gap-1.5">
                                            <span class='<%# Convert.ToBoolean(Eval("IsPinned")) ? "rounded bg-rose-100 px-1.5 py-0.5 text-[#a01020]" : "hidden" %>' style="font-size:10px;font-weight:900">PINNED</span>
                                            <span class="rounded bg-sky-100 px-1.5 py-0.5 text-sky-700" style="font-size:10px;font-weight:900">ANNOUNCEMENT</span>
                                        </div>
                                        <p class="mt-2 truncate text-slate-800" style="font-size:13px;font-weight:800"><%# Html(Eval("Title")) %></p>
                                        <p class="mt-0.5 truncate text-slate-500" style="font-size:12px"><%# Html(Eval("TargetCourses")) %></p>
                                    </div>
                                    <span class="shrink-0 text-slate-400" style="font-size:11px"><%# RelativeTime(Eval("CreatedAt")) %></span>
                                </div>
                            </asp:LinkButton>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
            </aside>

            <article class="min-h-[620px] rounded-lg border border-slate-200 bg-white">
                <asp:Panel ID="detailPanel" runat="server" CssClass="flex min-h-[620px] flex-col">
                    <div class="flex items-center justify-end gap-1 border-b border-slate-100 px-6 py-4">
                        <asp:LinkButton ID="pinButton" runat="server" OnClick="PinButton_Click" CssClass="inline-flex h-9 w-9 items-center justify-center rounded-md text-amber-500 hover:bg-amber-50" ToolTip="Pin or unpin">
                            <i data-lucide="pin" class="h-4 w-4"></i>
                        </asp:LinkButton>
                        <asp:LinkButton ID="deleteButton" runat="server" OnClick="DeleteButton_Click" CssClass="inline-flex h-9 w-9 items-center justify-center rounded-md text-slate-400 hover:bg-rose-50 hover:text-[#e0162b]" ToolTip="Delete announcement" OnClientClick="return confirm('Delete this announcement?');">
                            <i data-lucide="trash-2" class="h-4 w-4"></i>
                        </asp:LinkButton>
                    </div>
                    <div class="flex-1 px-6 py-7">
                        <div class="flex flex-wrap items-center gap-2 text-slate-400" style="font-size:12px;font-weight:700">
                            <span class="rounded bg-rose-100 px-2 py-1 text-[#a01020]"><%= DetailPinnedLabel %></span>
                            <span><%= DetailTargetCourses %></span>
                        </div>
                        <h2 class="mt-4 text-slate-900" style="font-size:24px;font-weight:900;letter-spacing:-0.015em;line-height:1.2"><%= DetailTitle %></h2>
                        <p class="mt-3 inline-flex items-center gap-1.5 text-slate-400" style="font-size:13px">
                            <i data-lucide="clock-3" class="h-4 w-4"></i>
                            <%= DetailCreatedAt %>
                        </p>

                        <div class="mt-5 grid gap-3 md:grid-cols-3">
                            <div class="rounded-md border border-slate-200 bg-slate-50 px-4 py-3">
                                <p class="text-slate-400" style="font-size:11px;font-weight:900">COURSE</p>
                                <p class="mt-1 text-slate-900" style="font-size:13px;font-weight:900"><%= DetailTargetCourses %></p>
                            </div>
                            <div class="rounded-md border border-slate-200 bg-slate-50 px-4 py-3">
                                <p class="text-slate-400" style="font-size:11px;font-weight:900">POSTED</p>
                                <p class="mt-1 text-slate-900" style="font-size:13px;font-weight:900"><%= DetailDateOnly %></p>
                            </div>
                            <div class="rounded-md border border-slate-200 bg-slate-50 px-4 py-3">
                                <p class="text-slate-400" style="font-size:11px;font-weight:900">ATTACHMENT</p>
                                <p class="mt-1 text-slate-900" style="font-size:13px;font-weight:900"><%= DetailAttachmentLabel %></p>
                            </div>
                        </div>

                        <p class="mt-6 whitespace-pre-line text-slate-600" style="font-size:14px;line-height:1.8"><%= DetailContent %></p>

                        <% if (DetailHasAttachment) { %>
                        <a href="<%= DetailAttachmentUrl %>" target="_blank" class="mt-7 inline-flex h-10 items-center gap-2 rounded-md border border-slate-200 bg-white px-4 text-slate-700 hover:border-slate-300 hover:text-slate-900 transition-colors" style="font-size:13px;font-weight:800">
                            <i data-lucide="paperclip" class="h-4 w-4"></i>
                            Open attachment
                        </a>
                        <% } %>
                    </div>
                </asp:Panel>
            </article>
        </section>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/lecturer/lecturer-portal.js") %>"></script>
</asp:Content>
