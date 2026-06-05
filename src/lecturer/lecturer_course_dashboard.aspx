<%@ Page Language="C#" MasterPageFile="~/shared/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_course_dashboard.aspx.cs" Inherits="src.lecturer.lecturer_course_dashboard" Title="Course Dashboard - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <div id="course-dashboard-root"
        style="--course-accent:<%= AccentColor %>;--course-accent-soft:<%= AccentColor %>15;--course-accent-dark:color-mix(in srgb,var(--course-accent) 72%,#000)">

    <%-- Back --%>
    <a href="<%= ResolveUrl("~/lecturer/lecturer_courses.aspx") %>" class="inline-flex items-center gap-1.5 text-slate-500 hover:text-slate-900 transition-colors" style="font-size:13px;font-weight:500">
        <i data-lucide="arrow-left" class="h-3.5 w-3.5"></i> Back to My Courses
    </a>

    <%-- Course header (DB-driven) --%>
    <section class="relative mt-4 overflow-hidden rounded-3xl p-7 lg:p-9 text-white" style="background:linear-gradient(135deg,var(--course-accent) 0%,#1e293b 100%)">
        <div class="pointer-events-none absolute -top-16 -right-10 h-64 w-64 rounded-full bg-white/10 blur-3xl"></div>
        <div class="relative max-w-2xl">
            <div class="flex items-center gap-2">
                <span class="rounded-md bg-white/15 px-2 py-0.5 backdrop-blur" style="font-size:11px;font-weight:600;letter-spacing:0.04em"><%: CourseCode %></span>
                <% if (!string.IsNullOrEmpty(LevelLabel)) { %>
                <span class="text-white/70" style="font-size:12px"><%: LevelLabel %></span>
                <% } %>
            </div>
            <h1 class="mt-3 text-white" style="font-size:30px;font-weight:700;letter-spacing:-0.015em;line-height:1.15">
                <%: CourseName %>
            </h1>
            <% if (!string.IsNullOrEmpty(Description)) { %>
            <p class="mt-2 text-white/80" style="font-size:14.5px;line-height:1.6"><%: Description %></p>
            <% } %>
            <div class="mt-4 flex flex-wrap gap-x-5 gap-y-1 text-white/80" style="font-size:13px">
                <span class="inline-flex items-center gap-1.5"><i data-lucide="calendar-days" class="h-4 w-4"></i> <%: SemesterName %></span>
                <span class="inline-flex items-center gap-1.5"><i data-lucide="award" class="h-4 w-4"></i> <%: CreditHours %> credits</span>
                <span class="inline-flex items-center gap-1.5"><i data-lucide="users" class="h-4 w-4"></i> <%: EnrolledLabel %></span>
            </div>
        </div>
    </section>

    <%-- Overview metrics --%>
    <section class="mt-6 grid grid-cols-2 gap-4 xl:grid-cols-4">

        <%-- Students --%>
        <div class="rounded-2xl border border-slate-200 bg-white p-5">
            <div class="flex items-center gap-2 text-slate-500">
                <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-slate-100 text-slate-600">
                    <i data-lucide="users" class="h-4 w-4"></i>
                </span>
                <p style="font-size:12.5px;font-weight:500">Students</p>
            </div>
            <p class="mt-2 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= EnrolledCount %></p>
            <p class="mt-1 text-slate-400" style="font-size:12px">enrolled</p>
        </div>

        <%-- Class average --%>
        <div class="rounded-2xl border border-slate-200 bg-white p-5">
            <div class="flex items-center gap-2 text-slate-500">
                <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-slate-100 text-slate-600">
                    <i data-lucide="trending-up" class="h-4 w-4"></i>
                </span>
                <p style="font-size:12.5px;font-weight:500">Class average</p>
            </div>
            <p class="mt-2 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= AverageGradeDisplay %></p>
            <p class="mt-1 text-slate-400" style="font-size:12px">published grades</p>
        </div>

        <%-- Attendance --%>
        <div class="rounded-2xl border border-slate-200 bg-white p-5">
            <div class="flex items-center gap-2 text-slate-500">
                <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-slate-100 text-slate-600">
                    <i data-lucide="clipboard-check" class="h-4 w-4"></i>
                </span>
                <p style="font-size:12.5px;font-weight:500">Attendance</p>
            </div>
            <p class="mt-2 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= AttendanceDisplay %></p>
            <p class="mt-1 text-slate-400" style="font-size:12px">present rate</p>
        </div>

        <%-- Pending grading --%>
        <div class="rounded-2xl border border-slate-200 bg-white p-5">
            <div class="flex items-center gap-2 text-slate-500">
                <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-slate-100 text-slate-600">
                    <i data-lucide="pen-line" class="h-4 w-4"></i>
                </span>
                <p style="font-size:12.5px;font-weight:500">Pending grading</p>
            </div>
            <p class="mt-2 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= PendingGrading %></p>
            <p class="mt-1 text-slate-400" style="font-size:12px">to mark</p>
        </div>

    </section>

    <%-- Manage this course (clickable cards -> sub-pages) --%>
    <section class="mt-8">
        <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Manage this course</h2>
        <p class="mt-0.5 text-slate-500" style="font-size:13px">Jump into a section to manage it.</p>

        <div class="mt-4 grid gap-4 sm:grid-cols-2 xl:grid-cols-4">

            <%-- Announcements --%>
            <a href="<%= AnnouncementsUrl %>" class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
                <div class="flex items-start justify-between">
                    <span class="flex h-11 w-11 items-center justify-center rounded-xl" style="background-color:var(--course-accent-soft);color:var(--course-accent)">
                        <i data-lucide="megaphone" class="h-5 w-5"></i>
                    </span>
                    <i data-lucide="arrow-up-right" class="h-4 w-4 text-slate-300 group-hover:text-slate-500 transition-colors"></i>
                </div>
                <p class="mt-4 text-slate-900" style="font-size:15px;font-weight:600">Announcements</p>
                <p class="mt-0.5 text-slate-500" style="font-size:12.5px"><%= AnnouncementLabel %></p>
            </a>

            <%-- People --%>
            <a href="<%= PeopleUrl %>" class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
                <div class="flex items-start justify-between">
                    <span class="flex h-11 w-11 items-center justify-center rounded-xl" style="background-color:var(--course-accent-soft);color:var(--course-accent)">
                        <i data-lucide="users" class="h-5 w-5"></i>
                    </span>
                    <i data-lucide="arrow-up-right" class="h-4 w-4 text-slate-300 group-hover:text-slate-500 transition-colors"></i>
                </div>
                <p class="mt-4 text-slate-900" style="font-size:15px;font-weight:600">People</p>
                <p class="mt-0.5 text-slate-500" style="font-size:12.5px"><%= EnrolledLabel %></p>
            </a>

            <%-- Grades --%>
            <a href="<%= GradesUrl %>" class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
                <div class="flex items-start justify-between">
                    <span class="flex h-11 w-11 items-center justify-center rounded-xl" style="background-color:var(--course-accent-soft);color:var(--course-accent)">
                        <i data-lucide="graduation-cap" class="h-5 w-5"></i>
                    </span>
                    <i data-lucide="arrow-up-right" class="h-4 w-4 text-slate-300 group-hover:text-slate-500 transition-colors"></i>
                </div>
                <p class="mt-4 text-slate-900" style="font-size:15px;font-weight:600">Grades</p>
                <p class="mt-0.5 text-slate-500" style="font-size:12.5px"><%= AssessmentLabel %></p>
            </a>

            <%-- Materials --%>
            <a href="<%= MaterialsUrl %>" class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
                <div class="flex items-start justify-between">
                    <span class="flex h-11 w-11 items-center justify-center rounded-xl" style="background-color:var(--course-accent-soft);color:var(--course-accent)">
                        <i data-lucide="folder" class="h-5 w-5"></i>
                    </span>
                    <i data-lucide="arrow-up-right" class="h-4 w-4 text-slate-300 group-hover:text-slate-500 transition-colors"></i>
                </div>
                <p class="mt-4 text-slate-900" style="font-size:15px;font-weight:600">Materials</p>
                <p class="mt-0.5 text-slate-500" style="font-size:12.5px"><%= MaterialLabel %></p>
            </a>

        </div>
    </section>

    <%-- Latest announcements --%>
    <section class="mt-6 rounded-2xl border border-slate-200 bg-white p-6">
        <header class="flex items-center justify-between mb-4">
            <div class="flex items-center gap-2">
                <span class="flex h-8 w-8 items-center justify-center rounded-lg" style="background-color:var(--course-accent-soft);color:var(--course-accent)">
                    <i data-lucide="megaphone" class="h-4 w-4"></i>
                </span>
                <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Latest announcements</h2>
            </div>
            <a href="<%= AnnouncementsUrl %>" class="inline-flex items-center gap-1 text-[#e0162b] hover:text-[#a01020] transition-colors" style="font-size:13px;font-weight:600">
                See all <i data-lucide="arrow-up-right" class="h-3.5 w-3.5"></i>
            </a>
        </header>
        <asp:Repeater ID="announcementsRepeater" runat="server">
            <HeaderTemplate><ul class="space-y-4"></HeaderTemplate>
            <ItemTemplate>
                <li class="border-b border-slate-100 pb-4 last:border-b-0 last:pb-0">
                    <div class="flex items-center gap-2 flex-wrap">
                        <span class="text-slate-900" style="font-size:13.5px;font-weight:600"><%# Server.HtmlEncode(Eval("Title").ToString()) %></span>
                        <%# (bool)Eval("IsPinned") ? "<span class=\"inline-flex items-center gap-1 rounded-md px-1.5 py-0.5\" style=\"background-color:var(--course-accent-soft);color:var(--course-accent-dark);font-size:10.5px;font-weight:600\">Pinned</span>" : "" %>
                        <span class="text-slate-400" style="font-size:11.5px">&middot; <%# Server.HtmlEncode(Eval("AuthorName").ToString()) %> &middot; <%# ((System.DateTime)Eval("CreatedAt")).ToString("d MMM · HH:mm") %></span>
                    </div>
                    <p class="mt-1 text-slate-500 line-clamp-2" style="font-size:12.5px;line-height:1.55"><%# Server.HtmlEncode(Eval("Content").ToString()) %></p>
                </li>
            </ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
        </asp:Repeater>
        <% if (AnnouncementCount == 0) { %>
            <p class="py-8 text-center text-slate-400" style="font-size:13px">No announcements for this course yet.</p>
        <% } %>
    </section>

    </div>

</asp:Content>
