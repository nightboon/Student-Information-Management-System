<%@ Page Language="C#" MasterPageFile="~/student/StudentLayout.master" AutoEventWireup="true" CodeBehind="attendance.aspx.cs" Inherits="src.student.attendance" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <%-- Header --%>
    <div class="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Academic record</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">Attendance</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">Your personal attendance log, visible only to you.</p>
        </div>
        <div class="flex flex-wrap items-center gap-2">
            <div class="inline-flex items-center gap-2 rounded-full bg-slate-100 px-3 py-1 text-slate-700" style="font-size:12px;font-weight:600">
                <i data-lucide="calendar-days" class="h-3.5 w-3.5"></i>
                <select id="semester-filter" class="bg-transparent text-slate-700 focus:outline-none cursor-pointer" style="font-size:12px;font-weight:600">
                    <%= SemesterOptionsHtml %>
                </select>
            </div>
            <button type="button" class="inline-flex items-center gap-2 rounded-md border border-slate-200 bg-white px-3 h-10 text-slate-700 hover:bg-slate-50 transition-colors" style="font-size:13px;font-weight:600">
                <i data-lucide="download" class="h-4 w-4"></i> Export report
            </button>
        </div>
    </div>

    <%-- Privacy banner --%>
    <div class="mt-4 flex items-start gap-2.5 rounded-md border border-sky-100 bg-sky-50/70 px-4 py-2.5 text-sky-800">
        <i data-lucide="shield-check" class="h-4 w-4 mt-0.5 shrink-0"></i>
        <p style="font-size:12.5px;line-height:1.5">
            These records are private to you. Only you and the registrar can view them. Lecturers see classroom rolls, not personal histories.
        </p>
    </div>

    <%-- Hero summary --%>
    <section class="mt-6">
        <div class="rounded-lg border border-slate-200 bg-gradient-to-br from-[#e0162b] to-[#a01020] p-6 text-white relative overflow-hidden">
            <div class="pointer-events-none absolute -top-10 -right-10 h-48 w-48 rounded-full bg-white/10 blur-3xl"></div>
            <div class="relative flex items-start justify-between">
                <div>
                    <p class="text-white/80" style="font-size:11.5px;font-weight:600;letter-spacing:0.08em">OVERALL ATTENDANCE</p>
                    <p class="mt-2" style="font-size:56px;font-weight:800;letter-spacing:-0.02em;line-height:1"><span id="hero-rate"><%: OverallRateDisplay %></span></p>
                    <p id="hero-subtext" class="mt-2 text-white/80" style="font-size:13px"><%: OverallSubtext %></p>
                </div>
                <span class="inline-flex items-center gap-1 rounded border bg-white/15 backdrop-blur px-2.5 py-1 text-white border-white/25" style="font-size:11.5px;font-weight:700">
                    <i data-lucide="trending-up" class="h-3.5 w-3.5"></i> Term in progress
                </span>
            </div>
            <div class="mt-5 grid grid-cols-3 gap-2 border-t border-white/15 pt-4">
                <div>
                    <p class="text-white/70" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">PRESENT</p>
                    <p class="mt-1 text-white" style="font-size:18px;font-weight:700"><span id="hero-present"><%: PresentCountDisplay %></span></p>
                </div>
                <div>
                    <p class="text-white/70" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">LATE</p>
                    <p class="mt-1 text-white" style="font-size:18px;font-weight:700"><span id="hero-late"><%: LateCountDisplay %></span></p>
                </div>
                <div>
                    <p class="text-white/70" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">ABSENT</p>
                    <p class="mt-1 text-white" style="font-size:18px;font-weight:700"><span id="hero-absent"><%: AbsentCountDisplay %></span></p>
                </div>
            </div>
        </div>
    </section>

    <%-- Per-course cards --%>
    <section class="mt-6">
        <div class="flex items-center justify-between mb-3">
            <h2 class="text-slate-900" style="font-size:15px;font-weight:600">By course</h2>
            <span class="text-slate-500" style="font-size:12px">Recorded sessions so far this term</span>
        </div>
        <div id="course-grid" class="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
            <asp:Repeater ID="courseRepeater" runat="server">
                <ItemTemplate>
                    <div class='<%# CourseCardClass(Container.DataItem) %>' data-offering='<%# Eval("OfferingId") %>' data-semester-id='<%# Eval("SemesterId") %>'>
                        <div class="flex items-center gap-2.5">
                            <span class="h-9 w-1.5 rounded-full" style='<%# CourseColorStyle(Eval("Color")) %>'></span>
                            <div class="min-w-0 flex-1">
                                <p class="text-slate-500" style="font-size:11px;font-weight:700;letter-spacing:0.06em"><%#: Eval("CourseCode") %></p>
                                <p class="text-slate-900 truncate" style="font-size:13.5px;font-weight:600"><%#: Eval("CourseName") %></p>
                            </div>
                        </div>
                        <div class="mt-4 flex items-baseline gap-1.5">
                            <span style="font-size:28px;font-weight:700;color:#e0162b;letter-spacing:-0.01em"><%#: CourseRateDisplay(Container.DataItem) %></span>
                            <span class="text-slate-400" style="font-size:11.5px"><%#: CourseRatioDisplay(Container.DataItem) %></span>
                        </div>
                        <div class="mt-2 h-1.5 rounded-full bg-slate-100 overflow-hidden">
                            <div class="h-full rounded-full" style='<%# CourseBarStyle(Container.DataItem) %>'></div>
                        </div>
                        <div class="mt-3 flex items-center gap-1.5 flex-wrap">
                            <span class="inline-flex items-center gap-1 rounded bg-slate-50 border border-slate-200 px-1.5 py-0.5 text-slate-700" style="font-size:10.5px;font-weight:700">
                                <span class="h-1.5 w-1.5 rounded-full" style="background-color:#10b981"></span> <%#: Eval("PresentCount") %> P
                            </span>
                            <span class="inline-flex items-center gap-1 rounded bg-slate-50 border border-slate-200 px-1.5 py-0.5 text-slate-700" style="font-size:10.5px;font-weight:700">
                                <span class="h-1.5 w-1.5 rounded-full" style="background-color:#0ea5e9"></span> <%#: Eval("LateCount") %> L
                            </span>
                            <span class="inline-flex items-center gap-1 rounded bg-slate-50 border border-slate-200 px-1.5 py-0.5 text-slate-700" style="font-size:10.5px;font-weight:700">
                                <span class="h-1.5 w-1.5 rounded-full" style="background-color:#e0162b"></span> <%#: Eval("AbsentCount") %> A
                            </span>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div id="empty-courses" class="mt-3 rounded-lg border border-dashed border-slate-200 bg-white px-6 py-8 text-center" style="display:none">
            <p class="text-slate-900" style="font-size:15px;font-weight:600">No courses for this semester</p>
            <p class="mt-1 text-slate-500" style="font-size:13px">Attendance appears after you are enrolled in courses for the selected semester.</p>
        </div>
    </section>

    <%-- Detail sessions --%>
    <section id="detail-panel" class="mt-6 rounded-lg border border-slate-200 bg-white">
            <header class="flex flex-col gap-3 border-b border-slate-100 px-6 py-5 lg:flex-row lg:items-center lg:justify-between">
                <div class="flex items-center gap-3 min-w-0">
                    <span id="detail-accent" class="h-10 w-1.5 rounded-full" style="<%= DetailCourseColorStyle %>"></span>
                    <div class="min-w-0">
                        <p id="detail-code" class="text-slate-500" style="font-size:11.5px;font-weight:700;letter-spacing:0.06em"><%: DetailCourseCode %></p>
                        <h3 id="detail-name" class="text-slate-900 truncate" style="font-size:18px;font-weight:700;letter-spacing:-0.01em"><%: DetailCourseName %></h3>
                        <p id="detail-lecturer" class="text-slate-500 truncate" style="font-size:12.5px"><%: DetailLecturerName %></p>
                    </div>
                </div>
                <span class="inline-flex items-center gap-2 rounded-full bg-slate-100 px-3 py-1 text-slate-700" style="font-size:12px;font-weight:600">
                    <i data-lucide="filter" class="h-3.5 w-3.5"></i>
                    All sessions
                </span>
            </header>

            <%-- KPI strip --%>
            <div class="grid gap-px bg-slate-100 sm:grid-cols-3">
                <div class="bg-white px-6 py-4">
                    <div class="flex items-center gap-1.5 text-slate-500">
                        <span class="h-2 w-2 rounded-full" style="background-color:#10b981"></span>
                        <p style="font-size:10.5px;font-weight:700;letter-spacing:0.06em">PRESENT</p>
                    </div>
                    <p id="detail-present" class="mt-1 text-slate-900" style="font-size:22px;font-weight:700"><%: DetailPresentDisplay %></p>
                </div>
                <div class="bg-white px-6 py-4">
                    <div class="flex items-center gap-1.5 text-slate-500">
                        <span class="h-2 w-2 rounded-full" style="background-color:#0ea5e9"></span>
                        <p style="font-size:10.5px;font-weight:700;letter-spacing:0.06em">LATE</p>
                    </div>
                    <p id="detail-late" class="mt-1 text-slate-900" style="font-size:22px;font-weight:700"><%: DetailLateDisplay %></p>
                </div>
                <div class="bg-white px-6 py-4">
                    <div class="flex items-center gap-1.5 text-slate-500">
                        <span class="h-2 w-2 rounded-full" style="background-color:#e0162b"></span>
                        <p style="font-size:10.5px;font-weight:700;letter-spacing:0.06em">ABSENT</p>
                    </div>
                    <p id="detail-absent" class="mt-1 text-slate-900" style="font-size:22px;font-weight:700"><%: DetailAbsentDisplay %></p>
                </div>
            </div>

            <%-- Sessions table --%>
            <div class="overflow-x-auto">
                <table class="w-full text-left table-fixed">
                    <colgroup>
                        <col style="width:22%" />
                        <col style="width:20%" />
                        <col style="width:18%" />
                        <col style="width:20%" />
                        <col style="width:20%" />
                    </colgroup>
                    <thead class="bg-slate-50/60 text-slate-500" style="font-size:10.5px;font-weight:700;letter-spacing:0.06em">
                        <tr>
                            <th class="px-6 py-3">DATE</th>
                            <th class="px-4 py-3">TIME</th>
                            <th class="px-4 py-3">TYPE</th>
                            <th class="px-4 py-3">ROOM</th>
                            <th class="px-6 py-3">STATUS</th>
                        </tr>
                    </thead>
                    <tbody id="sessions-body" class="divide-y divide-slate-100">
                        <asp:Repeater ID="sessionsRepeater" runat="server">
                            <ItemTemplate>
                                <tr class="hover:bg-slate-50/60">
                                    <td class="px-6 py-3.5">
                                        <div class="text-slate-900" style="font-size:13px;font-weight:600"><%#: SessionDateDisplay(Container.DataItem) %></div>
                                        <div class="text-slate-500" style="font-size:11px"><%#: SessionDayDisplay(Container.DataItem) %></div>
                                    </td>
                                    <td class="px-4 py-3.5 text-slate-700" style="font-size:12.5px"><%#: SessionTimeDisplay(Container.DataItem) %></td>
                                    <td class="px-4 py-3.5">
                                        <span class="inline-flex items-center rounded border border-slate-200 bg-slate-50 px-1.5 py-0.5 text-slate-700" style="font-size:10.5px;font-weight:700;letter-spacing:0.04em"><%#: SessionTypeDisplay(Container.DataItem) %></span>
                                    </td>
                                    <td class="px-4 py-3.5 text-slate-700" style="font-size:12.5px"><%#: SessionVenueDisplay(Container.DataItem) %></td>
                                    <td class="px-6 py-3.5">
                                        <span class='<%# StatusBadgeClass(Eval("Status")) %>' style="font-size:10.5px;font-weight:700;letter-spacing:0.04em">
                                            <i data-lucide='<%# StatusIcon(Eval("Status")) %>' class="h-3 w-3"></i> <%#: StatusDisplay(Eval("Status")) %>
                                        </span>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>

            <div id="empty-sessions" class="border-t border-slate-100 px-6 py-8 text-center" style="display:none">
                <p class="text-slate-900" style="font-size:15px;font-weight:600">No sessions recorded for this course</p>
                <p class="mt-1 text-slate-500" style="font-size:13px">Attendance rows will appear after records are entered.</p>
            </div>

            <footer class="flex items-center justify-between border-t border-slate-100 px-6 py-3">
                <p id="sessions-footer" class="text-slate-500" style="font-size:12px"><%: SessionsFooterDisplay %></p>
            </footer>
        </section>

    <section id="empty-detail" class="mt-6 rounded-lg border border-dashed border-slate-200 bg-white px-6 py-10 text-center" style="display:none">
        <p class="text-slate-900" style="font-size:15px;font-weight:600">No attendance details yet</p>
        <p class="mt-1 text-slate-500" style="font-size:13px">Course attendance details will appear after enrolments with attendance are available.</p>
    </section>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script type="text/javascript">
        window.attendanceData = <%= AttendancePayloadJson %>;
    </script>
    <script src='<%= ResolveUrl("~/js/student/attendance.js") %>'></script>
</asp:Content>
