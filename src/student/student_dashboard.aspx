<%@ Page Language="C#" MasterPageFile="~/shared/DashboardLayout.master" AutoEventWireup="true" CodeBehind="student_dashboard.aspx.cs" Inherits="src.student.student_dashboard" Title="Dashboard - INTI Student Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <%-- Welcome banner --%>
    <section class="relative overflow-hidden rounded-3xl bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 p-7 lg:p-9 text-white">
        <div class="pointer-events-none absolute -top-20 -right-10 h-72 w-72 rounded-full bg-[#e0162b]/30 blur-3xl"></div>
        <div class="pointer-events-none absolute -bottom-24 left-1/3 h-72 w-72 rounded-full bg-amber-300/15 blur-3xl"></div>
        <div class="relative flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
            <div class="max-w-xl">
                <div class="inline-flex items-center gap-2 rounded-full border border-white/20 bg-white/10 px-3 py-1 backdrop-blur" style="font-size:12px;font-weight:500">
                    <i data-lucide="sparkles" class="h-3.5 w-3.5 text-amber-200"></i>
                    <%= CurrentDateLabel %> &middot; Week <%= SemesterWeek %> of semester <%= SemesterNumber %>
                </div>
                <h1 class="mt-4 text-white" style="font-size:32px;font-weight:700;letter-spacing:-0.015em;line-height:1.15">
                    <%= Greeting %>, <%=GetUserName%> <span role="img" aria-label="waving hand">&#128075;</span>
                </h1>
                <p class="mt-2 text-white/75" style="font-size:15px;line-height:1.6">
                    You have <span class="text-white font-semibold"><%= TodayClassCount %> <%= TodayClassCount == 1 ? "class" : "classes" %></span> today and <span class="text-white font-semibold"><%= AssignmentDueCount %> <%= AssignmentDueCount == 1 ? "assignment" : "assignments" %></span> due this week. Keep going &mdash; you're on track.
                </p>
            </div>
            <div class="flex flex-wrap gap-3">
                <button type="button" class="inline-flex items-center gap-2 rounded-xl bg-white/10 px-4 h-11 text-white ring-1 ring-white/25 backdrop-blur hover:bg-white/15 transition-colors" style="font-size:14px;font-weight:500" onclick="window.location.href='/student/timetable.aspx'">
                    <i data-lucide="calendar-days" class="h-4 w-4"></i>
                    View timetable
                </button>
            </div>
        </div>
    </section>

    <%-- Stat cards --%>
    <section class="mt-6 grid grid-cols-2 gap-4 xl:grid-cols-4">

        <%-- Current GPA --%>
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div>
                    <p class="text-slate-500" style="font-size:12.5px;font-weight:500">Current GPA</p>
                    <p class="mt-1.5 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= CgpaDisplay %></p>
                </div>
            </div>
            <p class="mt-3 text-slate-400" style="font-size:12px">cumulative</p>
        </div>

        <%-- Attendance --%>
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div>
                    <p class="text-slate-500" style="font-size:12.5px;font-weight:500">Attendance</p>
                    <p class="mt-1.5 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= AttendanceDisplay %></p>
                </div>
            </div>
            <p class="mt-3 text-slate-400" style="font-size:12px">this term</p>
        </div>

        <%-- Credits Earned --%>
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div>
                    <p class="text-slate-500" style="font-size:12.5px;font-weight:500">Credits Earned</p>
                    <p class="mt-1.5 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= CreditsEarnedValue %></p>
                </div>
            </div>
            <p class="mt-3 text-slate-400" style="font-size:12px">earned to date</p>
        </div>

        <%-- Pending Tasks --%>
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div>
                    <p class="text-slate-500" style="font-size:12.5px;font-weight:500">Pending Tasks</p>
                    <p class="mt-1.5 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= PendingTaskCount %></p>
                </div>
                <span class="inline-flex items-center gap-1 rounded-full px-2 py-0.5 bg-slate-100 text-slate-600" style="font-size:11px;font-weight:600">
                    Due soon
                </span>
            </div>
            <p class="mt-3 text-slate-400" style="font-size:12px">next 7 days</p>
        </div>

    </section>

    <%-- Main grid: Today's Schedule + Upcoming Assignments --%>
    <section class="mt-6 grid gap-6 lg:grid-cols-3">

        <%-- Today's Schedule --%>
        <div class="lg:col-span-2 rounded-2xl border border-slate-200 bg-white">
            <header class="flex items-center justify-between p-6 pb-4">
                <div>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Today's Schedule</h2>
                    <p class="text-slate-500 mt-0.5" style="font-size:13px"><%= TodayScheduleSubtitle %></p>
                </div>
                <a href="#" class="inline-flex items-center gap-1 text-[#e0162b] hover:text-[#a01020] transition-colors" style="font-size:13px;font-weight:600">
                    Full week <i data-lucide="arrow-up-right" class="h-3.5 w-3.5"></i>
                </a>
            </header>
            <asp:Repeater ID="scheduleRepeater" runat="server">
                <HeaderTemplate><ul class="divide-y divide-slate-100"></HeaderTemplate>
                <ItemTemplate>
                    <li class="flex items-center gap-4 px-6 py-4 hover:bg-slate-50/60 transition-colors">
                        <div class="w-1.5 h-12 rounded-full" style="background-color:<%# ClassColor(Eval("Color") as string) %>"></div>
                        <div class="min-w-0 flex-1">
                            <div class="flex items-center gap-2">
                                <span class="text-slate-900 truncate" style="font-size:14px;font-weight:600"><%# Server.HtmlEncode(Eval("CourseName").ToString()) %></span>
                                <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600"><%# Server.HtmlEncode(Eval("CourseCode").ToString()) %></span>
                                <asp:Literal runat="server" Visible='<%# IsLiveNow((TimeSpan)Eval("StartTime"), (TimeSpan)Eval("EndTime")) %>'
                                    Text='<span class="inline-flex items-center gap-1 rounded-md bg-[#e0162b]/10 px-1.5 py-0.5 text-[#a01020]" style="font-size:10.5px;font-weight:600"><span class="h-1.5 w-1.5 rounded-full bg-[#e0162b] animate-pulse"></span>LIVE</span>' />
                            </div>
                            <div class="mt-1 flex flex-wrap items-center gap-x-4 gap-y-1 text-slate-500" style="font-size:12.5px">
                                <span class="inline-flex items-center gap-1"><i data-lucide="clock" class="h-3.5 w-3.5"></i><%# FormatTimeRange((TimeSpan)Eval("StartTime"), (TimeSpan)Eval("EndTime")) %></span>
                                <span class="inline-flex items-center gap-1"><i data-lucide="map-pin" class="h-3.5 w-3.5"></i><%# Server.HtmlEncode(Eval("Venue").ToString()) %></span>
                            </div>
                        </div>
                        <i data-lucide="chevron-right" class="h-4 w-4 text-slate-300"></i>
                    </li>
                </ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
            <% if (TodayClassCount == 0) { %>
                <p class="px-6 py-8 text-center text-slate-400" style="font-size:13px">No classes scheduled today.</p>
            <% } %>
        </div>

        <%-- Upcoming Assignments --%>
        <div class="rounded-2xl border border-slate-200 bg-white">
            <header class="flex items-center justify-between p-6 pb-4">
                <div>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Upcoming Assignments</h2>
                    <p class="text-slate-500 mt-0.5" style="font-size:13px"><%= AssignmentDueCount %> due this week</p>
                </div>
            </header>
            <asp:Repeater ID="assignmentsRepeater" runat="server">
                <HeaderTemplate><ul class="space-y-2 px-3 pb-4"></HeaderTemplate>
                <ItemTemplate>
                    <li class="flex items-start gap-3 rounded-xl px-3 py-3 hover:bg-slate-50 transition-colors">
                        <span class='mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg <%# DueBadgeClass((DateTime)Eval("DueDate")) %>'>
                            <i data-lucide='<%# DueIcon((DateTime)Eval("DueDate")) %>' class="h-4 w-4"></i>
                        </span>
                        <div class="min-w-0 flex-1">
                            <p class="text-slate-900 truncate" style="font-size:13.5px;font-weight:600"><%# Server.HtmlEncode(Eval("Title").ToString()) %></p>
                            <p class="mt-0.5" style="font-size:12px">
                                <span class="text-slate-500"><%# Server.HtmlEncode(Eval("CourseCode").ToString()) %></span>
                                <span class="text-slate-400"> &middot; </span>
                                <span class='<%# DueTextClass((DateTime)Eval("DueDate")) %>'><%# FormatRelativeDue((DateTime)Eval("DueDate")) %></span>
                            </p>
                        </div>
                    </li>
                </ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
            <% if (AssignmentDueCount == 0) { %>
                <p class="px-6 py-8 text-center text-slate-400" style="font-size:13px">Nothing due this week.</p>
            <% } %>
            <div class="border-t border-slate-100 p-3">
                <button class="w-full rounded-xl py-2.5 text-slate-700 hover:bg-slate-50 transition-colors" style="font-size:13px;font-weight:600">
                    View all assignments
                </button>
            </div>
        </div>

    </section>

    <%-- My Courses + Announcements --%>
    <section class="mt-6 grid gap-6 lg:grid-cols-3">

        <%-- My Courses --%>
        <div class="lg:col-span-2 rounded-2xl border border-slate-200 bg-white p-6">
            <header class="flex items-center justify-between mb-5">
                <div>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">My Courses</h2>
                    <p class="text-slate-500 mt-0.5" style="font-size:13px">Trimester 1 &middot; 2025/26</p>
                </div>
                <a href="#" class="inline-flex items-center gap-1 text-[#e0162b] hover:text-[#a01020] transition-colors" style="font-size:13px;font-weight:600">
                    See all <i data-lucide="arrow-up-right" class="h-3.5 w-3.5"></i>
                </a>
            </header>
            <asp:Repeater ID="coursesRepeater" runat="server">
                <HeaderTemplate><ul class="grid gap-3 sm:grid-cols-2"></HeaderTemplate>
                <ItemTemplate>
                    <li class="group rounded-xl border border-slate-200 p-4 hover:border-slate-300 hover:shadow-sm transition-all cursor-pointer">
                        <div class="flex items-center justify-between">
                            <div class="flex h-9 w-9 items-center justify-center rounded-lg" style="background-color:#e0162b15;color:#e0162b">
                                <i data-lucide="book-open" class="h-4 w-4"></i>
                            </div>
                            <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600"><%# Server.HtmlEncode(Eval("CourseCode").ToString()) %></span>
                        </div>
                        <p class="mt-3 text-slate-900 line-clamp-1" style="font-size:14px;font-weight:600"><%# Server.HtmlEncode(Eval("CourseName").ToString()) %></p>
                        <p class="mt-0.5 text-slate-500" style="font-size:12px"><%# Server.HtmlEncode(Eval("LecturerName").ToString()) %></p>
                    </li>
                </ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
        </div>

        <%-- Announcements --%>
        <div class="rounded-2xl border border-slate-200 bg-white p-6">
            <header class="flex items-center justify-between mb-4">
                <div class="flex items-center gap-2">
                    <span class="flex h-8 w-8 items-center justify-center rounded-lg bg-[#e0162b]/10 text-[#e0162b]">
                        <i data-lucide="megaphone" class="h-4 w-4"></i>
                    </span>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Announcements</h2>
                </div>
            </header>
            <asp:Repeater ID="announcementsRepeater" runat="server">
                <HeaderTemplate><ul class="space-y-4"></HeaderTemplate>
                <ItemTemplate>
                    <li class="border-b border-slate-100 pb-4 last:border-b-0 last:pb-0">
                        <div class="flex items-center gap-2">
                            <span class="text-slate-400" style="font-size:11.5px"><%# Server.HtmlEncode(FormatRelativeTime((DateTime)Eval("CreatedAt"))) %></span>
                        </div>
                        <p class="mt-2 text-slate-900" style="font-size:13.5px;font-weight:600;line-height:1.45"><%# Server.HtmlEncode(Eval("Title").ToString()) %></p>
                        <p class="mt-1 text-slate-500 line-clamp-2" style="font-size:12.5px;line-height:1.55"><%# Server.HtmlEncode(Eval("Content").ToString()) %></p>
                    </li>
                </ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
            <% if (AnnouncementCount == 0) { %>
                <p class="py-8 text-center text-slate-400" style="font-size:13px">No announcements.</p>
            <% } %>
        </div>

    </section>

    <%-- Floating chat assistant --%>
    <button id="assistant-fab" type="button" aria-label="Open assistant"
            class="fixed bottom-6 right-6 z-50 flex h-14 w-14 items-center justify-center rounded-full bg-[#e0162b] text-white shadow-lg shadow-[#e0162b]/30 hover:bg-[#a01020] transition-colors">
        <i data-lucide="message-circle" class="h-6 w-6"></i>
    </button>

    <div id="assistant-panel"
         class="fixed bottom-24 right-6 z-50 hidden h-[45rem] max-h-[80vh] w-[33rem] flex-col overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-2xl">
        <header class="flex items-center justify-between bg-[#e0162b] px-4 py-3 text-white">
            <div class="flex items-center gap-2">
                <i data-lucide="sparkles" class="h-4 w-4"></i>
                <span style="font-size:14px;font-weight:600">Study Assistant</span>
            </div>
            <button id="assistant-close" type="button" aria-label="Close assistant" class="text-white/80 hover:text-white">
                <i data-lucide="x" class="h-4 w-4"></i>
            </button>
        </header>

        <div id="assistant-messages" class="flex-1 space-y-3 overflow-y-auto bg-slate-50/60 p-4" style="font-size:13px;line-height:1.55">
            <div class="text-slate-500">Hi! Ask me about your classes, grades, attendance or assignments.</div>
        </div>

        <div class="flex items-center gap-2 border-t border-slate-100 p-3">
            <input id="assistant-input" type="text" autocomplete="off"
                   class="min-w-0 flex-1 rounded-xl border border-slate-200 px-3 py-2 text-slate-800 outline-none focus:border-[#e0162b]"
                   style="font-size:13px" placeholder="Ask anything, e.g. classes today" />
            <button id="assistant-send" type="button" aria-label="Send"
                    class="flex h-9 w-9 shrink-0 items-center justify-center rounded-xl bg-[#e0162b] text-white hover:bg-[#a01020] transition-colors disabled:opacity-50">
                <i data-lucide="send" class="h-4 w-4"></i>
            </button>
        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <%-- Markdown rendering for assistant replies (marked) --%>
    <style>
        #assistant-messages .assistant-md ul,
        #assistant-messages .assistant-md ol { padding-left: 1.1rem; list-style: revert; }
        #assistant-messages .assistant-md table { display: block; overflow-x: auto; border-collapse: collapse; font-size: 12px; }
        #assistant-messages .assistant-md th,
        #assistant-messages .assistant-md td { border: 1px solid #e2e8f0; padding: 3px 6px; white-space: nowrap; }
    </style>
    <script src="https://cdn.jsdelivr.net/npm/marked@12.0.0/marked.min.js"></script>
    <script src="<%= ResolveUrl("~/js/student/assistant/assistant.js") %>"></script>
</asp:Content>
