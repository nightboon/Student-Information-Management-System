<%@ Page Language="C#" MasterPageFile="~/shared/DashboardLayout.master" AutoEventWireup="true" CodeBehind="timetable.aspx.cs" Inherits="src.student.timetable" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <%-- Title --%>
    <div class="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Academic schedule</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">Class timetable</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">
                Your weekly schedule for <span class="text-slate-900 font-semibold"><%: SemesterDisplay %></span>.
            </p>
        </div>
        <div class="flex items-center gap-2">
            <span class="rounded-full bg-slate-100 px-3 py-1 text-slate-700" style="font-size:12px;font-weight:600"><%: SemesterDisplay %></span>
                <button type="button" id="btnDownloadPdf" class="inline-flex items-center gap-2 rounded-xl bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_20px_-8px_rgba(224,22,43,0.55)]"
                    style="font-size:13px;font-weight:600">
                    <i data-lucide="download" class="h-4 w-4"></i> Download PDF
                </button>
        </div>
    </div>

    <%-- Summary header --%>
    <section class="mt-6 rounded-lg border border-slate-200 bg-white">
        <div class="grid gap-0 sm:grid-cols-2 lg:grid-cols-4">

            <%-- Major --%>
            <div class="p-6 border-slate-100 border-b sm:border-b lg:border-b-0 lg:border-r">
                <div class="flex items-center gap-2 text-slate-500">
                    <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-slate-100 text-slate-600">
                        <i data-lucide="graduation-cap" class="h-4 w-4"></i>
                    </span>
                    <p style="font-size:11px;font-weight:600;letter-spacing:0.06em">MAJOR</p>
                </div>
                <p class="mt-2 text-slate-900" style="font-size:15px;font-weight:600;letter-spacing:-0.005em"><%: ProgrammeName %></p>
            </div>

            <%-- Courses taken --%>
            <div class="p-6 border-slate-100 border-b sm:border-b lg:border-b-0 lg:border-r">
                <div class="flex items-center gap-2 text-slate-500">
                    <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-slate-100 text-slate-600">
                        <i data-lucide="book-open" class="h-4 w-4"></i>
                    </span>
                    <p style="font-size:11px;font-weight:600;letter-spacing:0.06em">COURSES TAKEN</p>
                </div>
                <p class="mt-2 text-slate-900" style="font-size:15px;font-weight:600;letter-spacing:-0.005em"><%: CourseCountDisplay %></p>
            </div>

            <%-- Total credit hours --%>
            <div class="p-6 border-slate-100 border-b sm:border-b lg:border-b-0 lg:border-r">
                <div class="flex items-center gap-2 text-slate-500">
                    <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-slate-100 text-slate-600">
                        <i data-lucide="hash" class="h-4 w-4"></i>
                    </span>
                    <p style="font-size:11px;font-weight:600;letter-spacing:0.06em">TOTAL CREDIT HOURS</p>
                </div>
                <p class="mt-2 text-slate-900" style="font-size:15px;font-weight:600;letter-spacing:-0.005em"><%: TotalCreditHoursDisplay %></p>
            </div>

            <%-- Weekly contact --%>
            <div class="p-6 border-slate-100">
                <div class="flex items-center gap-2 text-slate-500">
                    <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-slate-100 text-slate-600">
                        <i data-lucide="calendar" class="h-4 w-4"></i>
                    </span>
                    <p style="font-size:11px;font-weight:600;letter-spacing:0.06em">WEEKLY CONTACT</p>
                </div>
                <p class="mt-2 text-slate-900" style="font-size:15px;font-weight:600;letter-spacing:-0.005em"><%: WeeklyContactDisplay %></p>
            </div>
        </div>

        <%-- Courses this semester --%>
        <div class="border-t border-slate-100 p-6">
            <p class="text-slate-500" style="font-size:11px;font-weight:600;letter-spacing:0.06em">COURSES THIS SEMESTER</p>
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
                            <p class="text-slate-500 truncate" style="font-size:12px"><%#: Eval("LecturerName") %></p>
                        </div>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
            <asp:PlaceHolder ID="emptyCoursesPanel" runat="server" Visible="false">
                <p class="mt-3 rounded-lg border border-dashed border-slate-200 p-4 text-slate-500" style="font-size:13px">
                    No enrolled courses found for the current semester.
                </p>
            </asp:PlaceHolder>
        </div>
    </section>

    <%-- Timetable calendar using FullCalendar --%>
    <section class="mt-6 rounded-lg border border-slate-200 bg-white p-4 lg:p-6">
        <header class="flex flex-col gap-1 sm:flex-row sm:items-center sm:justify-between mb-4">
            <div>
                <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Weekly schedule</h2>
                <p class="text-slate-500" style="font-size:12px">Generated with FullCalendar &middot; <%: ScheduleRangeDisplay %></p>
            </div>
        </header>

        <div class="overflow-x-auto">
            <div class="w-full rounded-md border border-slate-200 bg-white p-3">
                <div id="studentTimetable"></div>
            </div>
        </div>
    </section>

    <%-- FullCalendar standard bundle: includes timeGrid view --%>
    <script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.20/index.global.min.js"></script>
    <script>
        window.studentTimetableData = <%= TimetablePayloadJson %>;
    </script>

    <style>
        #studentTimetable .fc {
            font-family: inherit;
            color: #0f172a;
        }

        #studentTimetable .fc-theme-standard td,
        #studentTimetable .fc-theme-standard th,
        #studentTimetable .fc-theme-standard .fc-scrollgrid {
            border-color: #e2e8f0;
        }

        #studentTimetable .fc-col-header-cell {
            background: #f8fafc;
            padding: 10px 0;
        }

        #studentTimetable .fc-col-header-cell-cushion {
            color: #1e293b;
            font-size: 13px;
            font-weight: 700;
            text-decoration: none;
        }

        #studentTimetable .fc-timegrid-slot-label-cushion {
            color: #94a3b8;
            font-size: 11px;
            font-weight: 600;
        }

        #studentTimetable .fc-timegrid-slot {
            height: 80px;
        }

        #studentTimetable .fc-event {
            border-radius: 8px;
            padding: 0;
            box-shadow: none;
            overflow: hidden;
        }

        #studentTimetable .tt-event {
            height: 100%;
            padding: 10px;
            background: rgba(255, 255, 255, 0.68);
        }

        #studentTimetable .tt-badges {
            display: flex;
            flex-wrap: wrap;
            gap: 4px;
            align-items: center;
        }

        #studentTimetable .tt-code {
            border-radius: 4px;
            padding: 2px 6px;
            color: white;
            font-size: 9.5px;
            font-weight: 700;
            letter-spacing: 0.02em;
        }

        #studentTimetable .tt-type {
            border: 1px solid #e2e8f0;
            background: rgba(255, 255, 255, 0.8);
            color: #475569;
            border-radius: 4px;
            padding: 2px 6px;
            font-size: 9.5px;
            font-weight: 600;
        }

        #studentTimetable .tt-title {
            margin-top: 6px;
            color: #0f172a;
            font-size: 12px;
            line-height: 1.2;
            font-weight: 700;
            white-space: normal;
        }

        #studentTimetable .tt-meta {
            margin-top: 4px;
            color: #475569;
            font-size: 10.5px;
            line-height: 1.2;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        #studentTimetable .tt-time {
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
        <script src="<%= ResolveUrl("~/js/student/timetable/timetable.js") %>"></script>

        <script src="https://cdnjs.cloudflare.com/ajax/libs/html2pdf.js/0.10.1/html2pdf.bundle.min.js"></script>
    
        <script>
            document.addEventListener("DOMContentLoaded", function () {
                var downloadBtn = document.getElementById('btnDownloadPdf');
            
                if (downloadBtn) {
                    downloadBtn.addEventListener('click', function () {
                        // Targets your crisp calendar box container layout
                        var element = document.querySelector('.w-full.rounded-md.border'); 
                    
                        if (!element) {
                            console.error("Calendar container element not found.");
                            return;
                        }

                        // Configuration optimized for a high-quality A3 Landscape document sheet
                        var opt = {
                            margin:       10, // 10mm margins for edge spacing
                            filename:     'My_Timetable.pdf',
                            image:        { type: 'jpeg', quality: 0.98 },
                            html2canvas:  { scale: 2, useCORS: true, logging: false },
                            jsPDF:        { unit: 'mm', format: 'a3', orientation: 'landscape' },
                            pagebreak:    { mode: ['avoid-all'] }
                        };

                        // Execute generation and download cleanly
                        html2pdf().set(opt).from(element).save();
                    });
                }
            });
        </script>
    </asp:Content>
