<%@ Page Language="C#" MasterPageFile="~/shared/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_dashboard.aspx.cs" Inherits="student_information_management_system.lecturer_dashboard" Title="Dashboard - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <%-- Welcome banner --%>
    <section class="relative overflow-hidden rounded-3xl bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 p-7 lg:p-9 text-white">
        <div class="pointer-events-none absolute -top-20 -right-10 h-72 w-72 rounded-full bg-[#e0162b]/30 blur-3xl"></div>
        <div class="pointer-events-none absolute -bottom-24 left-1/3 h-72 w-72 rounded-full bg-amber-300/15 blur-3xl"></div>
        <div class="relative flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
            <div class="max-w-xl">
                <div class="inline-flex items-center gap-2 rounded-full border border-white/20 bg-white/10 px-3 py-1 backdrop-blur" style="font-size:12px;font-weight:500">
                    <i data-lucide="sparkles" class="h-3.5 w-3.5 text-amber-200"></i>
                    <%= CurrentDateLabel %> &middot; Week <%= SemesterWeek %> of <%= SemesterName %>
                </div>
                <h1 class="mt-4 text-white" style="font-size:32px;font-weight:700;letter-spacing:-0.015em;line-height:1.15">
                    <%= Greeting %>, <%= LecturerName %> &#128075;
                </h1>
                <p class="mt-2 text-white/75" style="font-size:15px;line-height:1.6">
                    You have <span class="text-white font-semibold"><%= ClassesTodayLabel %></span> today and <span class="text-white font-semibold"><%= SubmissionsToReviewLabel %></span> awaiting your review. Keep your students on track.
                </p>
            </div>
            <div class="flex flex-wrap gap-3">
                <a href='<%= ResolveUrl("~/lecturer/lecturer_attendance.aspx") %>' class="inline-flex items-center gap-2 rounded-xl bg-white/10 px-4 h-11 text-white ring-1 ring-white/25 backdrop-blur hover:bg-white/15 transition-colors" style="font-size:14px;font-weight:500">
                    <i data-lucide="calendar-days" class="h-4 w-4"></i>
                    Today's schedule
                </a>
            </div>
        </div>
    </section>

    <%-- Stat cards --%>
    <section class="mt-6 grid grid-cols-2 gap-4 xl:grid-cols-4">

        <%-- Active Courses --%>
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div>
                    <p class="text-slate-500" style="font-size:12.5px;font-weight:500">Active Courses</p>
                    <p class="mt-1.5 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= ActiveCoursesCount %></p>
                </div>
            </div>
            <p class="mt-3 text-slate-400" style="font-size:12px">this trimester</p>
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

        <%-- Students Taught --%>
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div>
                    <p class="text-slate-500" style="font-size:12.5px;font-weight:500">Students Taught</p>
                    <p class="mt-1.5 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= StudentsTaughtCount %></p>
                </div>
            </div>
            <p class="mt-3 text-slate-400" style="font-size:12px">this semester</p>
        </div>

        <%-- Pending Grading --%>
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div>
                    <p class="text-slate-500" style="font-size:12.5px;font-weight:500">Pending Grading</p>
                    <p class="mt-1.5 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= PendingGradingCount %></p>
                </div>
                <span class="inline-flex items-center gap-1 rounded-full px-2 py-0.5 bg-slate-100 text-slate-600" style="font-size:11px;font-weight:600">
                    Due soon
                </span>
            </div>
            <p class="mt-3 text-slate-400" style="font-size:12px">this week</p>
        </div>

    </section>

    <%-- Main grid: Today's Schedule + To Grade --%>
    <section class="mt-6 grid gap-6 lg:grid-cols-3">

        <%-- Today's Schedule --%>
        <div class="lg:col-span-2 rounded-2xl border border-slate-200 bg-white">
            <header class="flex items-center justify-between p-6 pb-4">
                <div>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Today's Schedule</h2>
                    <p class="text-slate-500 mt-0.5" style="font-size:13px"><%= TodayScheduleSubtitle %></p>
                </div>
                <a href='<%= ResolveUrl("~/lecturer/lecturer_attendance.aspx") %>' class="inline-flex items-center gap-1 text-[#e0162b] hover:text-[#a01020] transition-colors" style="font-size:13px;font-weight:600">
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

        <%-- To Grade --%>
        <div class="rounded-2xl border border-slate-200 bg-white">
            <header class="flex items-center justify-between p-6 pb-4">
                <div>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">To Grade</h2>
                    <p class="text-slate-500 mt-0.5" style="font-size:13px"><%= ToGradeCount %> awaiting grading</p>
                </div>
            </header>
            <asp:Repeater ID="gradeRepeater" runat="server">
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
                                <span class="text-slate-400"> &middot; </span>
                                <span class="text-slate-500"><%# Eval("PendingCount") %> to mark</span>
                            </p>
                        </div>
                    </li>
                </ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
            <% if (ToGradeCount == 0) { %>
                <p class="px-6 py-8 text-center text-slate-400" style="font-size:13px">Nothing awaiting grading.</p>
            <% } %>
            <div class="border-t border-slate-100 p-3">
                <a href='<%= ResolveUrl("~/lecturer/lecturer_grades.aspx") %>' class="block w-full rounded-xl py-2.5 text-center text-slate-700 hover:bg-slate-50 transition-colors" style="font-size:13px;font-weight:600">
                    View all submissions
                </a>
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
                    <p class="text-slate-500 mt-0.5" style="font-size:13px"><%= MyCoursesSubtitle %></p>
                </div>
                <a href='<%= ResolveUrl("~/lecturer/lecturer_courses.aspx") %>' class="inline-flex items-center gap-1 text-[#e0162b] hover:text-[#a01020] transition-colors" style="font-size:13px;font-weight:600">
                    See all <i data-lucide="arrow-up-right" class="h-3.5 w-3.5"></i>
                </a>
            </header>
            <asp:Repeater ID="coursesRepeater" runat="server">
                <HeaderTemplate><ul class="grid gap-3 sm:grid-cols-2"></HeaderTemplate>
                <ItemTemplate>
                    <li class="group rounded-xl border border-slate-200 p-4 hover:border-slate-300 hover:shadow-sm transition-all cursor-pointer">
                        <div class="flex items-center justify-between">
                            <div class="flex h-9 w-9 items-center justify-center rounded-lg" style="background-color:<%# AccentColor(Eval("Color") as string) %>15;color:<%# AccentColor(Eval("Color") as string) %>">
                                <i data-lucide="book-open" class="h-4 w-4"></i>
                            </div>
                            <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600"><%# Server.HtmlEncode(Eval("CourseCode").ToString()) %></span>
                        </div>
                        <p class="mt-3 text-slate-900 line-clamp-1" style="font-size:14px;font-weight:600"><%# Server.HtmlEncode(Eval("CourseName").ToString()) %></p>
                        <p class="mt-0.5 text-slate-500" style="font-size:12px"><%# EnrolledLabel((int)Eval("EnrolledCount")) %></p>
                    </li>
                </ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
            <% if (MyCoursesCount == 0) { %>
                <p class="py-8 text-center text-slate-400" style="font-size:13px">No courses this semester.</p>
            <% } %>
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
                <a href='<%= ResolveUrl("~/lecturer/lecturer_announcement.aspx") %>' class="inline-flex items-center gap-1 text-[#e0162b] hover:text-[#a01020] transition-colors" style="font-size:13px;font-weight:600">
                    See all <i data-lucide="arrow-up-right" class="h-3.5 w-3.5"></i>
                </a>
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

</asp:Content>
