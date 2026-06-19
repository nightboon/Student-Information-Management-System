<%@ Page Language="C#" MasterPageFile="~/admin/AdminLayout.master" AutoEventWireup="true" CodeBehind="admin_dashboard.aspx.cs" Inherits="src.admin.admin_dashboard" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <%-- Welcome banner --%>
    <section class="relative overflow-hidden rounded-3xl bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 p-7 lg:p-9 text-white">
        <div class="pointer-events-none absolute -top-20 -right-10 h-72 w-72 rounded-full bg-[#e0162b]/30 blur-3xl"></div>
        <div class="pointer-events-none absolute -bottom-24 left-1/3 h-72 w-72 rounded-full bg-amber-300/15 blur-3xl"></div>
        <div class="relative">
            <div class="inline-flex items-center gap-2 rounded-full border border-white/20 bg-white/10 px-3 py-1 backdrop-blur" style="font-size:12px;font-weight:500">
                <i data-lucide="sparkles" class="h-3.5 w-3.5 text-amber-200"></i>
                <%= DateTime.Now.ToString("dddd, dd MMMM yyyy") %>
            </div>
            <h1 class="mt-4 text-white" style="font-size:32px;font-weight:700;letter-spacing:-0.015em;line-height:1.15">
                Good Morning, <%= Server.HtmlEncode(Session["username"] as string ?? "Administrator") %> <span role="img" aria-label="wave">&#128075;</span>
            </h1>
            <p class="mt-2 max-w-2xl text-white/75" style="font-size:15px;line-height:1.6">
                You have <span class="text-white font-semibold"><%= Number(Dashboard.PendingRequests) %> add/drop requests</span> and
                <span class="text-white font-semibold"><%= Number(Dashboard.AtRiskStudents) %> at-risk students</span> requiring attention.
            </p>
            <div class="mt-6 flex flex-wrap items-center gap-3">
                <a href="<%= ResolveUrl("~/admin/add_drop_requests.aspx") %>" class="inline-flex items-center gap-2 rounded-xl bg-white/10 px-4 h-11 text-white ring-1 ring-white/25 backdrop-blur hover:bg-white/15 transition-colors" style="font-size:14px;font-weight:500">
                    <i data-lucide="clipboard-check" class="h-4 w-4"></i>
                    Review pending actions
                </a>
                <a href="<%= ResolveUrl("~/admin/academic_performance.aspx") %>" class="inline-flex items-center gap-2 rounded-xl px-4 h-11 text-white/80 hover:text-white transition-colors" style="font-size:14px;font-weight:500">
                    View dashboard insights
                    <i data-lucide="arrow-up-right" class="h-4 w-4"></i>
                </a>
            </div>
        </div>
    </section>

    <%-- Summary cards --%>
    <section class="mt-6" style="display:grid;grid-template-columns:repeat(auto-fit,minmax(220px,1fr));gap:16px">
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div class="flex h-9 w-9 items-center justify-center rounded-xl bg-slate-100 text-slate-700"><i data-lucide="graduation-cap" class="h-5 w-5"></i></div>
            </div>
            <div class="mt-4 text-slate-500" style="font-size:12.5px;font-weight:500">Total Students</div>
            <div class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= Number(Dashboard.TotalStudents) %></div>
            <div class="mt-1 text-slate-400" style="font-size:12px">active records</div>
        </div>
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div class="flex h-9 w-9 items-center justify-center rounded-xl bg-slate-100 text-slate-700"><i data-lucide="users" class="h-5 w-5"></i></div>
            </div>
            <div class="mt-4 text-slate-500" style="font-size:12.5px;font-weight:500">Total Lecturers</div>
            <div class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= Number(Dashboard.TotalLecturers) %></div>
            <div class="mt-1 text-slate-400" style="font-size:12px">teaching staff</div>
        </div>
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div class="flex h-9 w-9 items-center justify-center rounded-xl bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-5 w-5"></i></div>
                <span class="rounded-full bg-amber-50 px-2 py-0.5 text-amber-700 border border-amber-100" style="font-size:10.5px;font-weight:600">New</span>
            </div>
            <div class="mt-4 text-slate-500" style="font-size:12.5px;font-weight:500">Pending Requests</div>
            <div class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= Number(Dashboard.PendingRequests) %></div>
            <div class="mt-1 text-slate-400" style="font-size:12px">awaiting review</div>
        </div>
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div class="flex h-9 w-9 items-center justify-center rounded-xl bg-[#e0162b]/10 text-[#e0162b]"><i data-lucide="alert-triangle" class="h-5 w-5"></i></div>
            </div>
            <div class="mt-4 text-slate-500" style="font-size:12.5px;font-weight:500">At-Risk Students</div>
            <div class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= Number(Dashboard.AtRiskStudents) %></div>
            <div class="mt-1 text-slate-400" style="font-size:12px">academic support needed</div>
        </div>
    </section>

    <%-- Main grid: Enrolment Overview + Pending Actions --%>
    <section class="mt-6" style="display:flex;flex-wrap:wrap;gap:24px">
        <div class="rounded-2xl border border-slate-200 bg-white" style="flex:2 1 460px;min-width:0">
            <header class="flex items-center justify-between p-6 pb-4">
                <div>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Enrolment Overview</h2>
                    <p class="text-slate-500 mt-0.5" style="font-size:13px">Students by programme</p>
                </div>
                <a href="<%= ResolveUrl("~/admin/academic_performance.aspx") %>" class="inline-flex items-center gap-1 text-[#e0162b] hover:text-[#a01020] transition-colors" style="font-size:13px;font-weight:600">
                    View details <i data-lucide="arrow-up-right" class="h-3.5 w-3.5"></i>
                </a>
            </header>
            <div class="px-6 pb-6 space-y-3">
                <%= ProgrammeEnrolmentHtml %>
            </div>
        </div>

        <div class="rounded-2xl border border-slate-200 bg-white" style="flex:1 1 300px;min-width:0">
            <header class="flex items-center justify-between p-6 pb-4">
                <div>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Pending Actions</h2>
                    <p class="text-slate-500 mt-0.5" style="font-size:13px">Items needing your review</p>
                </div>
            </header>
            <ul class="px-3 pb-3">
                <li class="flex items-start gap-3 rounded-xl px-3 py-3 hover:bg-slate-50 transition-colors cursor-pointer">
                    <span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-[#e0162b]/10 text-[#e0162b]"><i data-lucide="file-text" class="h-4 w-4"></i></span>
                    <div class="min-w-0 flex-1"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">12 new admission applications</div><div class="text-slate-500 truncate" style="font-size:12px">Awaiting review &middot; BCS, BIT, BBA</div></div>
                </li>
                <li class="flex items-start gap-3 rounded-xl px-3 py-3 hover:bg-slate-50 transition-colors cursor-pointer">
                    <span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-amber-50 text-amber-700"><i data-lucide="inbox" class="h-4 w-4"></i></span>
                    <div class="min-w-0 flex-1"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">9 add/drop requests pending</div><div class="text-slate-500 truncate" style="font-size:12px">Submitted in the last 24 hours</div></div>
                </li>
                <li class="flex items-start gap-3 rounded-xl px-3 py-3 hover:bg-slate-50 transition-colors cursor-pointer">
                    <span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-[#e0162b]/10 text-[#e0162b]"><i data-lucide="alert-triangle" class="h-4 w-4"></i></span>
                    <div class="min-w-0 flex-1"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">5 critical at-risk students</div><div class="text-slate-500 truncate" style="font-size:12px">CGPA below 2.00 &mdash; intervention required</div></div>
                </li>
                <li class="flex items-start gap-3 rounded-xl px-3 py-3 hover:bg-slate-50 transition-colors cursor-pointer">
                    <span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-amber-50 text-amber-700"><i data-lucide="calendar" class="h-4 w-4"></i></span>
                    <div class="min-w-0 flex-1"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Course registration closes Fri</div><div class="text-slate-500 truncate" style="font-size:12px">Reminder due to all students</div></div>
                </li>
                <li class="flex items-start gap-3 rounded-xl px-3 py-3 hover:bg-slate-50 transition-colors cursor-pointer">
                    <span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-600"><i data-lucide="calendar" class="h-4 w-4"></i></span>
                    <div class="min-w-0 flex-1"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Final exam period in 2 weeks</div><div class="text-slate-500 truncate" style="font-size:12px">Confirm room allocations</div></div>
                </li>
            </ul>
        </div>
    </section>

    <%-- Secondary grid: Academic Performance + Admin Notices --%>
    <section class="mt-6" style="display:flex;flex-wrap:wrap;gap:24px">
        <div class="rounded-2xl border border-slate-200 bg-white p-6" style="flex:2 1 460px;min-width:0">
            <header class="flex items-center justify-between">
                <div>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Academic Performance Overview</h2>
                    <p class="text-slate-500 mt-0.5" style="font-size:13px">High-level snapshot &middot; current semester</p>
                </div>
                <span class="inline-flex items-center gap-1 rounded-full border border-emerald-100 bg-emerald-50 px-2.5 py-0.5 text-emerald-700" style="font-size:11.5px;font-weight:600">
                    <i data-lucide="trending-up" class="h-3 w-3"></i> +2.4% MoM
                </span>
            </header>
            <ul class="mt-5" style="display:grid;grid-template-columns:repeat(auto-fit,minmax(160px,1fr));gap:12px">
                <li class="group rounded-xl border border-slate-200 p-4 hover:border-slate-300 hover:shadow-sm transition-all cursor-pointer">
                    <div class="text-slate-500" style="font-size:11.5px;font-weight:500">Total Attendance Rate</div>
                    <div class="mt-1 text-slate-900" style="font-size:22px;font-weight:700;letter-spacing:-0.01em"><%= Percent(Dashboard.AverageAttendance) %></div>
                    <div class="text-slate-400" style="font-size:11.5px">across all courses</div>
                </li>
                <li class="group rounded-xl border border-slate-200 p-4 hover:border-slate-300 hover:shadow-sm transition-all cursor-pointer">
                    <div class="text-slate-500" style="font-size:11.5px;font-weight:500">Average CGPA</div>
                    <div class="mt-1 text-slate-900" style="font-size:22px;font-weight:700;letter-spacing:-0.01em"><%= DecimalNumber(Dashboard.AverageCgpa) %></div>
                    <div class="text-slate-400" style="font-size:11.5px">all active students</div>
                </li>
                <li class="group rounded-xl border border-slate-200 p-4 hover:border-slate-300 hover:shadow-sm transition-all cursor-pointer">
                    <div class="text-slate-500" style="font-size:11.5px;font-weight:500">Pass Rate</div>
                    <div class="mt-1 text-slate-900" style="font-size:22px;font-weight:700;letter-spacing:-0.01em"><%= Percent(Dashboard.PassRate) %></div>
                    <div class="text-slate-400" style="font-size:11.5px">current semester</div>
                </li>
                <li class="group rounded-xl border border-slate-200 p-4 hover:border-slate-300 hover:shadow-sm transition-all cursor-pointer">
                    <div class="text-slate-500" style="font-size:11.5px;font-weight:500">Fail Rate</div>
                    <div class="mt-1 text-slate-900" style="font-size:22px;font-weight:700;letter-spacing:-0.01em"><%= Percent(Dashboard.FailRate) %></div>
                    <div class="text-slate-400" style="font-size:11.5px">current semester</div>
                </li>
            </ul>
        </div>

        <div class="rounded-2xl border border-slate-200 bg-white p-6" style="flex:1 1 300px;min-width:0">
            <header class="flex items-center justify-between">
                <div class="flex items-center gap-2">
                    <i data-lucide="megaphone" class="h-4 w-4 text-[#e0162b]"></i>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Admin Notices</h2>
                </div>
                <a href="<%= ResolveUrl("~/admin/academic_calendar.aspx") %>" class="text-[#e0162b] hover:text-[#a01020]" style="font-size:12.5px;font-weight:600">All</a>
            </header>
            <ul class="mt-4 space-y-4">
                <li class="border-b border-slate-100 pb-4 last:border-b-0 last:pb-0">
                    <div class="flex items-center justify-between gap-2"><div class="text-slate-900" style="font-size:13px;font-weight:600">Final Exam Timetable Published</div><div class="text-slate-400 shrink-0" style="font-size:11.5px">May 28, 2026</div></div>
                    <div class="mt-1 text-slate-500" style="font-size:12.5px;line-height:1.55">Examination period: 12&ndash;25 June 2026. Lecturers must confirm invigilation slots by 5 June.</div>
                </li>
                <li class="border-b border-slate-100 pb-4 last:border-b-0 last:pb-0">
                    <div class="flex items-center justify-between gap-2"><div class="text-slate-900" style="font-size:13px;font-weight:600">Admission Period Closing</div><div class="text-slate-400 shrink-0" style="font-size:11.5px">May 26, 2026</div></div>
                    <div class="mt-1 text-slate-500" style="font-size:12.5px;line-height:1.55">Sep 2026 intake applications close 14 June. Current pending: 38 applications.</div>
                </li>
                <li class="border-b border-slate-100 pb-4 last:border-b-0 last:pb-0">
                    <div class="flex items-center justify-between gap-2"><div class="text-slate-900" style="font-size:13px;font-weight:600">Add/Drop Deadline Reminder</div><div class="text-slate-400 shrink-0" style="font-size:11.5px">May 24, 2026</div></div>
                    <div class="mt-1 text-slate-500" style="font-size:12.5px;line-height:1.55">Final add/drop deadline is 30 May, 11:59 PM. Late requests need HOP approval.</div>
                </li>
                <li class="border-b border-slate-100 pb-4 last:border-b-0 last:pb-0">
                    <div class="flex items-center justify-between gap-2"><div class="text-slate-900" style="font-size:13px;font-weight:600">Result Release Schedule</div><div class="text-slate-400 shrink-0" style="font-size:11.5px">May 22, 2026</div></div>
                    <div class="mt-1 text-slate-500" style="font-size:12.5px;line-height:1.55">Semester 1 results will be released to students on 10 June at 9:00 AM.</div>
                </li>
            </ul>
        </div>
    </section>

</asp:Content>
<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/admin/shared/icons.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/toast.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/table.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/ui.js") %>"></script>
</asp:Content>
