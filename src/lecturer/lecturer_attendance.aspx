<%@ Page Language="C#" MasterPageFile="~/lecturer/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_attendance.aspx.cs" Inherits="src.lecturer.lecturer_attendance" Title="Attendance - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <%-- Header --%>
    <div class="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Attendance &amp; schedule</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">Attendance</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">
                Your teaching schedule for <span class="text-slate-900 font-semibold"><%: SemesterDisplay %></span>. Click a class to take attendance.
            </p>
        </div>
        <div class="flex items-center gap-2">
            <span class="rounded-full bg-slate-100 px-3 py-1 text-slate-700" style="font-size:12px;font-weight:600"><%: SemesterDisplay %></span>
            <button type="button" class="inline-flex items-center gap-2 rounded-xl bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_20px_-8px_rgba(224,22,43,0.55)]"
                style="font-size:13px;font-weight:600">
                <i data-lucide="download" class="h-4 w-4"></i> Download PDF
            </button>
        </div>
    </div>

    <%-- Summary header --%>
    <section class="mt-6 rounded-lg border border-slate-200 bg-white">
        <div class="grid gap-0 sm:grid-cols-2 lg:grid-cols-4">

            <%-- Department --%>
            <div class="p-6 border-slate-100 border-b sm:border-b lg:border-b-0 lg:border-r">
                <div class="flex items-center gap-2 text-slate-500">
                    <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-slate-100 text-slate-600">
                        <i data-lucide="building-2" class="h-4 w-4"></i>
                    </span>
                    <p style="font-size:11px;font-weight:600;letter-spacing:0.06em">DEPARTMENT</p>
                </div>
                <p class="mt-2 text-slate-900" style="font-size:15px;font-weight:600;letter-spacing:-0.005em"><%: DepartmentDisplay %></p>
            </div>

            <%-- Courses teaching --%>
            <div class="p-6 border-slate-100 border-b sm:border-b lg:border-b-0 lg:border-r">
                <div class="flex items-center gap-2 text-slate-500">
                    <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-slate-100 text-slate-600">
                        <i data-lucide="book-open" class="h-4 w-4"></i>
                    </span>
                    <p style="font-size:11px;font-weight:600;letter-spacing:0.06em">COURSES TEACHING</p>
                </div>
                <p class="mt-2 text-slate-900" style="font-size:15px;font-weight:600;letter-spacing:-0.005em"><%: CoursesTeachingCount %></p>
            </div>

            <%-- Total students --%>
            <div class="p-6 border-slate-100 border-b sm:border-b lg:border-b-0 lg:border-r">
                <div class="flex items-center gap-2 text-slate-500">
                    <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-slate-100 text-slate-600">
                        <i data-lucide="users" class="h-4 w-4"></i>
                    </span>
                    <p style="font-size:11px;font-weight:600;letter-spacing:0.06em">TOTAL STUDENTS</p>
                </div>
                <p class="mt-2 text-slate-900" style="font-size:15px;font-weight:600;letter-spacing:-0.005em"><%: TotalStudentsDisplay %></p>
            </div>

            <%-- Weekly hours --%>
            <div class="p-6 border-slate-100">
                <div class="flex items-center gap-2 text-slate-500">
                    <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-slate-100 text-slate-600">
                        <i data-lucide="calendar" class="h-4 w-4"></i>
                    </span>
                    <p style="font-size:11px;font-weight:600;letter-spacing:0.06em">WEEKLY HOURS</p>
                </div>
                <p class="mt-2 text-slate-900" style="font-size:15px;font-weight:600;letter-spacing:-0.005em"><%: WeeklyHoursDisplay %></p>
            </div>
        </div>

        <%-- Courses teaching this semester --%>
        <div class="border-t border-slate-100 p-6">
            <p class="text-slate-500" style="font-size:11px;font-weight:600;letter-spacing:0.06em">COURSES TEACHING THIS SEMESTER</p>
            <asp:Repeater ID="coursesRepeater" runat="server">
                <HeaderTemplate>
                    <ul class="mt-3 grid gap-3 md:grid-cols-2 lg:grid-cols-3">
                </HeaderTemplate>
                <ItemTemplate>
                    <li class="flex items-start gap-3 rounded-xl border border-slate-200 p-3">
                        <span class="mt-0.5 flex h-9 w-9 shrink-0 items-center justify-center rounded-lg" style='<%# CourseIconStyle(Eval("Color")) %>'>
                            <i data-lucide="book-open" class="h-4 w-4"></i>
                        </span>
                        <div class="min-w-0 flex-1">
                            <div class="flex items-center gap-2 flex-wrap">
                                <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600"><%#: Eval("CourseCode") %></span>
                                <span class="rounded-md bg-slate-50 px-1.5 py-0.5 text-slate-500 border border-slate-200" style="font-size:10.5px;font-weight:600"><%#: Eval("CreditHours") %> cr</span>
                            </div>
                            <p class="mt-1 text-slate-900 truncate" style="font-size:13.5px;font-weight:600"><%#: Eval("CourseName") %></p>
                            <p class="text-slate-500 truncate" style="font-size:12px"><%#: EnrolledLabel((int)Eval("EnrolledCount")) %></p>
                        </div>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
            <% if (CoursesThisSemesterCount == 0) { %>
                <p class="mt-3 rounded-lg border border-dashed border-slate-200 p-4 text-slate-500" style="font-size:13px">
                    No courses assigned for the current semester.
                </p>
            <% } %>
        </div>
    </section>

    <%-- Weekly schedule (FullCalendar) --%>
    <section class="mt-6 rounded-lg border border-slate-200 bg-white p-4 lg:p-6">
        <header class="flex flex-col gap-1 sm:flex-row sm:items-center sm:justify-between mb-4">
            <div>
                <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Weekly schedule</h2>
                <p class="text-slate-500" style="font-size:12px">Click a class to record attendance &middot; Mon &ndash; Fri</p>
            </div>
        </header>

        <div class="overflow-x-auto">
            <div class="min-w-[920px] rounded-md border border-slate-200 bg-white p-3">
                <div id="lecturerTimetable"></div>
            </div>
        </div>
    </section>

    <%-- FullCalendar standard bundle: includes timeGrid view --%>
    <script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.20/index.global.min.js"></script>
    <script>
        window.lecturerTimetableData = <%= TimetablePayloadJson %>;
    </script>

    <style>
        #lecturerTimetable .fc {
            font-family: inherit;
            color: #0f172a;
        }

        #lecturerTimetable .fc-theme-standard td,
        #lecturerTimetable .fc-theme-standard th,
        #lecturerTimetable .fc-theme-standard .fc-scrollgrid {
            border-color: #e2e8f0;
        }

        #lecturerTimetable .fc-col-header-cell {
            background: #f8fafc;
            padding: 10px 0;
        }

        #lecturerTimetable .lt-dayname {
            color: #1e293b;
            font-size: 13px;
            font-weight: 700;
        }

        #lecturerTimetable .lt-daycount {
            margin-top: 2px;
            color: #94a3b8;
            font-size: 11px;
            font-weight: 600;
        }

        #lecturerTimetable .fc-timegrid-slot-label-cushion {
            color: #94a3b8;
            font-size: 11px;
            font-weight: 600;
        }

        #lecturerTimetable .fc-timegrid-slot {
            height: 80px;
        }

        #lecturerTimetable .fc-event {
            border-radius: 8px;
            padding: 0;
            box-shadow: none;
            overflow: hidden;
            cursor: pointer;
        }

        #lecturerTimetable .tt-event {
            height: 100%;
            padding: 10px;
            background: rgba(255, 255, 255, 0.68);
        }

        #lecturerTimetable .tt-badges {
            display: flex;
            flex-wrap: wrap;
            gap: 4px;
            align-items: center;
        }

        #lecturerTimetable .tt-code {
            border-radius: 4px;
            padding: 2px 6px;
            color: white;
            font-size: 9.5px;
            font-weight: 700;
            letter-spacing: 0.02em;
        }

        #lecturerTimetable .tt-type {
            border: 1px solid #e2e8f0;
            background: rgba(255, 255, 255, 0.8);
            color: #475569;
            border-radius: 4px;
            padding: 2px 6px;
            font-size: 9.5px;
            font-weight: 600;
        }

        #lecturerTimetable .tt-title {
            margin-top: 6px;
            color: #0f172a;
            font-size: 12px;
            line-height: 1.2;
            font-weight: 700;
            white-space: normal;
        }

        #lecturerTimetable .tt-meta {
            margin-top: 4px;
            color: #475569;
            font-size: 10.5px;
            line-height: 1.2;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        #lecturerTimetable .tt-time {
            position: absolute;
            right: 8px;
            bottom: 8px;
            border: 1px solid #e2e8f0;
            background: rgba(255, 255, 255, 0.85);
            color: #334155;
            border-radius: 4px;
            padding: 2px 6px;
            font-size: 9.5px;
            font-weight: 700;
        }
    </style>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/lecturer/attendance.js") %>"></script>
</asp:Content>
