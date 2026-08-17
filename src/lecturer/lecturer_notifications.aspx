<%@ Page Language="C#" MasterPageFile="~/lecturer/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_notifications.aspx.cs" Inherits="src.lecturer.lecturer_notifications" Title="Notifications - INTI Lecturer Portal" %>
<%@ Import Namespace="src.lecturer" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Inbox</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">Notifications</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">
                Updates from your assigned courses. <span id="unread-count" class="font-semibold text-[#a01020]"><%= UnreadCount %></span> unread.
            </p>
        </div>
        <button id="mark-all-read" type="button" class="inline-flex h-10 items-center gap-2 rounded-md border border-slate-200 bg-white px-3 text-slate-700 transition-colors hover:bg-slate-50" style="font-size:13px;font-weight:600">
            <i data-lucide="check-check" class="h-4 w-4"></i> Mark all as read
        </button>
    </div>

    <section class="mt-6 grid gap-4 lg:h-[820px] lg:grid-cols-[minmax(320px,400px)_1fr]">
        <div class="flex min-h-[620px] flex-col overflow-hidden rounded-lg border border-slate-200 bg-white lg:min-h-0">
            <div class="border-b border-slate-100 p-3">
                <div class="mb-2 grid grid-cols-2 gap-2">
                    <asp:DropDownList ID="yearFilter" runat="server" data-notification-year-filter="true" CssClass="h-9 min-w-0 rounded-md border border-slate-200 bg-white px-2 text-slate-700" style="font-size:12px" />
                    <asp:DropDownList ID="semesterFilter" runat="server" data-notification-semester-filter="true" CssClass="h-9 min-w-0 rounded-md border border-slate-200 bg-white px-2 text-slate-700" style="font-size:12px" />
                </div>
                <asp:DropDownList ID="courseFilter" runat="server" data-notification-course-filter="true" CssClass="mb-2 h-9 w-full rounded-md border border-slate-200 bg-white px-2 text-slate-700" style="font-size:12px" />
                <div class="relative">
                    <i data-lucide="search" class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"></i>
                    <input id="notif-search" placeholder="Search notifications&hellip;" class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 text-slate-900 placeholder:text-slate-400 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px" />
                </div>
            </div>

            <ul id="notif-list" data-notification-endpoint="<%= ResolveUrl("~/lecturer/lecturer_notifications.aspx") %>" class="min-h-0 flex-1 divide-y divide-slate-100 overflow-y-auto">
                <asp:Repeater ID="notificationsRepeater" runat="server">
                    <ItemTemplate>
                        <li>
                            <button type="button" class="notif-item flex w-full items-start gap-3 px-4 py-3.5 text-left transition-colors hover:bg-slate-50"
                                data-id="<%# Eval("NotificationId") %>"
                                data-type="<%# Eval("NotificationType") %>"
                                data-offering="<%# Eval("OfferingId") %>"
                                data-year="<%# Server.HtmlEncode((string)Eval("AcademicYear")) %>"
                                data-semester="<%# Server.HtmlEncode((string)Eval("Semester")) %>"
                                data-read="<%# ReadFlag(Eval("IsRead")) %>"
                                data-category="<%# Server.HtmlEncode((string)Eval("Category")) %>"
                                data-course="<%# Server.HtmlEncode((string)Eval("CourseLabel")) %>"
                                data-title="<%# Server.HtmlEncode((string)Eval("Title")) %>"
                                data-author="<%# Server.HtmlEncode((string)Eval("AuthorName")) %>"
                                data-time="<%# ListTime((DateTime)Eval("CreatedAt")) %>"
                                data-fulltime="<%# FullTime((DateTime)Eval("CreatedAt")) %>"
                                data-pinned="<%# PinnedFlag(Eval("IsPinned")) %>">
                                <span class="notif-dot mt-1.5 h-2 w-2 shrink-0 rounded-full"></span>
                                <div class="min-w-0 flex-1">
                                    <div class="flex items-center gap-2">
                                        <span class="notif-badge rounded border px-1.5 py-0.5" style="font-size:9.5px;font-weight:700;letter-spacing:0.04em"><%# Server.HtmlEncode((string)Eval("Category")) %></span>
                                        <i data-lucide="pin" class="notif-pin h-3 w-3 text-amber-500"></i>
                                        <span class="ml-auto truncate text-slate-400" style="font-size:10.5px"><%# ListTime((DateTime)Eval("CreatedAt")) %></span>
                                    </div>
                                    <p class="mt-1 line-clamp-2 text-slate-900" style="font-size:13px;font-weight:600;line-height:1.4"><%# Server.HtmlEncode((string)Eval("Title")) %></p>
                                    <p class="mt-0.5 truncate text-slate-500" style="font-size:11.5px"><%# Server.HtmlEncode((string)Eval("CourseLabel")) %></p>
                                </div>
                                <span class="notif-content" hidden style="white-space:pre-line"><%# Server.HtmlEncode((string)Eval("Content")) %></span>
                            </button>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="emptyPanel" runat="server" Visible="false">
                    <li class="px-4 py-12 text-center text-slate-400" style="font-size:13px">No notifications yet.</li>
                </asp:PlaceHolder>
            </ul>
        </div>

        <div class="min-h-[620px] overflow-hidden rounded-lg border border-slate-200 bg-white lg:min-h-0">
            <article class="flex h-full flex-col">
                <div id="detail-empty" class="flex flex-1 items-center justify-center px-7 py-16 text-slate-400" style="display:none;font-size:13px">Select a notification to read it.</div>
                <div id="detail-card" class="flex-1 overflow-y-auto px-7 py-6">
                    <div class="flex items-center gap-2">
                        <span id="detail-badge" class="rounded border px-1.5 py-0.5" style="font-size:10.5px;font-weight:700;letter-spacing:0.04em"></span>
                        <span class="text-slate-400" style="font-size:11.5px">&middot;</span>
                        <span id="detail-course" class="text-slate-500" style="font-size:12px"></span>
                    </div>
                    <h2 id="detail-title" class="mt-3 text-slate-900" style="font-size:22px;font-weight:700;letter-spacing:-0.01em;line-height:1.25"></h2>
                    <div class="mt-2 flex flex-wrap items-center gap-x-4 gap-y-1 text-slate-500" style="font-size:12px">
                        <span class="inline-flex items-center gap-1.5"><i data-lucide="clock" class="h-3.5 w-3.5 text-slate-400"></i><span id="detail-time"></span></span>
                        <span class="inline-flex items-center gap-1.5"><i data-lucide="user" class="h-3.5 w-3.5 text-slate-400"></i><span id="detail-author"></span></span>
                    </div>
                    <div id="detail-content" class="mt-5 text-slate-700" style="font-size:14px;line-height:1.7;white-space:pre-line"></div>
                </div>
                <footer class="flex items-center justify-end gap-2 border-t border-slate-100 bg-slate-50/40 px-5 py-3">
                    <button id="mark-unread" type="button" class="inline-flex h-9 items-center gap-1.5 rounded-md border border-slate-200 bg-white px-3 text-slate-700 hover:bg-slate-50" style="font-size:12.5px;font-weight:600">Mark unread</button>
                </footer>
            </article>
        </div>
    </section>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/shared/notifications.js") %>?v=11"></script>
</asp:Content>
