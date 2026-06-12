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

        #pdfTimetableWrapper * {
            box-sizing: border-box;
        }

        #pdfTimetableWrapper .fc-timegrid-slot {
            height: 70px !important;
        }

        #pdfTimetableWrapper .fc {
            width: 100% !important;
        }

        #pdfTimetableWrapper .fc-view-harness {
            width: 100% !important;
        }

        #pdfTimetableWrapper .fc-scrollgrid {
            width: 100% !important;
        }
    </style>


    </asp:Content>

  <asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">

    <script src="<%= ResolveUrl("~/js/student/timetable/timetable.js") %>"></script>

    <script src="https://unpkg.com/jspdf@2.5.1/dist/jspdf.umd.min.js"></script>

    <script>
        document.addEventListener("DOMContentLoaded", function () {
            var downloadBtn = document.getElementById("btnDownloadPdf");

            if (!downloadBtn) return;

            downloadBtn.addEventListener("click", function () {
                var originalBtnText = downloadBtn.innerHTML;
                downloadBtn.innerHTML = "Generating PDF...";
                downloadBtn.disabled = true;

                try {
                    generateTimetablePdf();
                } catch (err) {
                    console.error("PDF Error:", err);
                    alert("PDF generation failed. Open Console to see the error.");
                }

                downloadBtn.innerHTML = originalBtnText;
                downloadBtn.disabled = false;
            });
        });

        function generateTimetablePdf() {
            if (!window.jspdf || !window.jspdf.jsPDF) {
                alert("jsPDF is not loaded.");
                return;
            }

            const { jsPDF } = window.jspdf;

            const doc = new jsPDF({
                orientation: "landscape",
                unit: "mm",
                format: "a3"
            });

            const pageWidth = doc.internal.pageSize.getWidth();
            const pageHeight = doc.internal.pageSize.getHeight();

            const margin = 8;
            const titleHeight = 15;
            const leftTimeWidth = 18;
            const headerHeight = 12;

            const gridX = margin + leftTimeWidth;
            const gridY = margin + titleHeight + headerHeight;
            const gridWidth = pageWidth - margin * 2 - leftTimeWidth;
            const gridHeight = pageHeight - margin * 2 - titleHeight - headerHeight;

            const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
            const startHour = 9;
            const endHour = 21;
            const totalHours = endHour - startHour;

            const colWidth = gridWidth / days.length;
            const hourHeight = gridHeight / totalHours;

            drawBaseTimetable(doc, pageWidth, pageHeight, margin, titleHeight, leftTimeWidth, headerHeight, gridX, gridY, gridWidth, gridHeight, days, startHour, endHour, colWidth, hourHeight);

            let events = getEventsFromVisibleTimetable();

            if (events.length === 0) {
                events = getEventsFromStudentData();
            }

            console.log("PDF events:", events);

            events.forEach(function (event) {
                drawEvent(doc, event, gridX, gridY, colWidth, hourHeight, startHour, endHour);
            });

            doc.save("Class_Timetable.pdf");
        }

        function drawBaseTimetable(doc, pageWidth, pageHeight, margin, titleHeight, leftTimeWidth, headerHeight, gridX, gridY, gridWidth, gridHeight, days, startHour, endHour, colWidth, hourHeight) {
            doc.setFillColor(255, 255, 255);
            doc.rect(0, 0, pageWidth, pageHeight, "F");

            doc.setTextColor(15, 23, 42);
            doc.setFont("helvetica", "bold");
            doc.setFontSize(16);
            doc.text("Class Timetable", margin, margin + 6);

            doc.setFont("helvetica", "normal");
            doc.setFontSize(9);
            doc.setTextColor(100, 116, 139);
            doc.text("Weekly schedule", margin, margin + 12);

            doc.setFillColor(248, 250, 252);
            doc.setDrawColor(226, 232, 240);
            doc.rect(gridX, margin + titleHeight, gridWidth, headerHeight, "FD");

            doc.setFont("helvetica", "bold");
            doc.setFontSize(9);
            doc.setTextColor(15, 23, 42);

            days.forEach(function (day, index) {
                const x = gridX + index * colWidth;
                doc.rect(x, margin + titleHeight, colWidth, headerHeight);
                doc.text(day, x + colWidth / 2, margin + titleHeight + 7.5, {
                    align: "center"
                });
            });

            doc.setFont("helvetica", "normal");
            doc.setFontSize(7);
            doc.setTextColor(100, 116, 139);

            for (let h = startHour; h <= endHour; h++) {
                const y = gridY + (h - startHour) * hourHeight;

                doc.setDrawColor(226, 232, 240);
                doc.line(gridX, y, gridX + gridWidth, y);

                if (h < endHour) {
                    doc.text(formatHour(h), margin + 2, y + 3);
                }
            }

            for (let i = 0; i <= days.length; i++) {
                const x = gridX + i * colWidth;
                doc.setDrawColor(226, 232, 240);
                doc.line(x, gridY, x, gridY + gridHeight);
            }

            doc.setDrawColor(226, 232, 240);
            doc.rect(gridX, gridY, gridWidth, gridHeight);
        }

        function getEventsFromStudentData() {
            let raw = window.studentTimetableData;

            if (!raw) return [];

            if (raw.events && Array.isArray(raw.events)) {
                raw = raw.events;
            }

            if (!Array.isArray(raw)) return [];

            const result = [];

            raw.forEach(function (event) {
                const props = event.extendedProps || {};

                const title = props.courseName || props.CourseName || event.courseName || event.CourseName || event.title || "Class";
                const code = props.courseCode || props.CourseCode || event.courseCode || event.CourseCode || "";
                const lecturer = props.lecturerName || props.LecturerName || event.lecturerName || event.LecturerName || "";
                const room = props.roomName || props.RoomName || props.room || event.roomName || event.RoomName || "";
                const type = props.type || props.Type || props.sessionType || props.SessionType || "Class";
                const color = event.backgroundColor || event.borderColor || props.color || event.color || "#e0162b";

                if (event.start && event.end) {
                    const start = new Date(event.start);
                    const end = new Date(event.end);

                    if (!isNaN(start.getTime()) && !isNaN(end.getTime())) {
                        result.push({
                            dayIndex: start.getDay() - 1,
                            startDecimal: start.getHours() + start.getMinutes() / 60,
                            endDecimal: end.getHours() + end.getMinutes() / 60,
                            title: title,
                            code: code,
                            lecturer: lecturer,
                            room: room,
                            type: type,
                            color: color
                        });
                    }
                }

                if (event.daysOfWeek && event.startTime && event.endTime) {
                    event.daysOfWeek.forEach(function (dayValue) {
                        result.push({
                            dayIndex: Number(dayValue) - 1,
                            startDecimal: timeStringToDecimal(event.startTime),
                            endDecimal: timeStringToDecimal(event.endTime),
                            title: title,
                            code: code,
                            lecturer: lecturer,
                            room: room,
                            type: type,
                            color: color
                        });
                    });
                }
            });

            return result.filter(function (event) {
                return event.dayIndex >= 0 && event.dayIndex <= 4;
            });
        }

        function getEventsFromVisibleTimetable() {
            const eventNodes = document.querySelectorAll("#studentTimetable .fc-event");
            const events = [];

            eventNodes.forEach(function (node) {
                const code = getText(node, ".tt-code");
                const type = getText(node, ".tt-type") || "Class";
                const title = getText(node, ".tt-title") || "Class";
                const timeText = getText(node, ".tt-time");

                const metaNodes = node.querySelectorAll(".tt-meta");
                let lecturer = "";
                let room = "";

                metaNodes.forEach(function (metaNode) {
                    const text = metaNode.innerText.trim();

                    if (text.toLowerCase().startsWith("lecturer:")) {
                        lecturer = text.replace(/lecturer:/i, "").trim();
                    }

                    if (text.toLowerCase().startsWith("room:")) {
                        room = text.replace(/room:/i, "").trim();
                    }
                });

                const timeRange = parseTimeRange(timeText);
                const dayIndex = getDayIndexFromDom(node);

                // Get exact colour from the course code badge
                const codeBadge = node.querySelector(".tt-code");
                let color = "#e0162b";

                if (codeBadge) {
                    color = window.getComputedStyle(codeBadge).backgroundColor;
                } else {
                    color = window.getComputedStyle(node).borderColor;
                }

                if (timeRange && dayIndex >= 0 && dayIndex <= 4) {
                    events.push({
                        dayIndex: dayIndex,
                        startDecimal: timeRange.start,
                        endDecimal: timeRange.end,
                        title: title,
                        code: code,
                        lecturer: lecturer,
                        room: room,
                        type: type,
                        color: color
                    });
                }
            });

            return events;
        }

        function drawEvent(doc, event, gridX, gridY, colWidth, hourHeight, startHour, endHour) {
            if (event.endDecimal <= startHour || event.startDecimal >= endHour) return;

            const safeStart = Math.max(event.startDecimal, startHour);
            const safeEnd = Math.min(event.endDecimal, endHour);

            const x = gridX + event.dayIndex * colWidth + 2;
            const y = gridY + (safeStart - startHour) * hourHeight + 2;
            const w = colWidth - 4;
            const h = (safeEnd - safeStart) * hourHeight - 4;

            const borderRgb = cssColorToRgb(event.color);
            const bgRgb = lightenRgb(borderRgb, 0.92);

            // Light pastel background like your original timetable
            doc.setFillColor(bgRgb.r, bgRgb.g, bgRgb.b);
            doc.setDrawColor(borderRgb.r, borderRgb.g, borderRgb.b);
            doc.setLineWidth(0.5);
            doc.roundedRect(x, y, w, h, 2, 2, "FD");

            // Course code badge
            doc.setFillColor(borderRgb.r, borderRgb.g, borderRgb.b);
            doc.roundedRect(x + 3, y + 3, 15, 5, 1, 1, "F");

            doc.setTextColor(255, 255, 255);
            doc.setFont("helvetica", "bold");
            doc.setFontSize(8);

            doc.text(event.code || "", x + 10.5, y + 6.5, {
                align: "center"
            });

            // Class badge
            doc.setFillColor(255, 255, 255);
            doc.setDrawColor(226, 232, 240);
            doc.roundedRect(x + 20, y + 3, 13, 5, 1, 1, "FD");

            doc.setTextColor(51, 65, 85);
            doc.setFont("helvetica", "normal");
            doc.setFontSize(7.5);
            doc.text(event.type || "Class", x + 26.5, y + 6.5, {
                align: "center"
            });

            // Course name
            doc.setTextColor(15, 23, 42);
            doc.setFont("helvetica", "bold");
            doc.setFontSize(9);

            const titleLines = doc.splitTextToSize(event.title || "Class", w - 8);
            doc.text(titleLines.slice(0, 2), x + 3, y + 13);

            // Lecturer and room
            doc.setFont("helvetica", "normal");
            doc.setFontSize(8);
            doc.setTextColor(51, 65, 85);

            let metaY = y + 21;

            if (event.lecturer) {
                doc.text("Lecturer: " + event.lecturer, x + 3, metaY);
                metaY += 4;
            }

            if (event.room) {
                doc.text("Room: " + event.room, x + 3, metaY);
            }

            // Time badge
            const timeText = decimalToTime(event.startDecimal) + " - " + decimalToTime(event.endDecimal);
            const timeBadgeWidth = 28;

            doc.setFillColor(255, 255, 255);
            doc.setDrawColor(226, 232, 240);
            doc.roundedRect(x + w - timeBadgeWidth - 3, y + h - 8, timeBadgeWidth, 5, 1, 1, "FD");

            doc.setFont("helvetica", "bold");
            doc.setFontSize(7.5);
            doc.setTextColor(15, 23, 42);
            doc.text(timeText, x + w - 3 - timeBadgeWidth / 2, y + h - 4.5, {
                align: "center"
            });
        }

        function getText(parent, selector) {
            const el = parent.querySelector(selector);
            return el ? el.innerText.trim() : "";
        }

        function getDayIndexFromDom(node) {
            let current = node;

            while (current) {
                if (current.getAttribute && current.getAttribute("data-date")) {
                    const dateText = current.getAttribute("data-date");
                    const date = new Date(dateText + "T00:00:00");
                    return date.getDay() - 1;
                }

                current = current.parentElement;
            }

            const nodeRect = node.getBoundingClientRect();
            const centerX = nodeRect.left + nodeRect.width / 2;

            const headers = document.querySelectorAll("#studentTimetable .fc-col-header-cell");

            for (let i = 0; i < headers.length; i++) {
                const headerText = headers[i].innerText.toLowerCase();
                const rect = headers[i].getBoundingClientRect();

                if (centerX >= rect.left && centerX <= rect.right) {
                    if (headerText.includes("monday")) return 0;
                    if (headerText.includes("tuesday")) return 1;
                    if (headerText.includes("wednesday")) return 2;
                    if (headerText.includes("thursday")) return 3;
                    if (headerText.includes("friday")) return 4;
                }
            }

            return -1;
        }

        function parseTimeRange(text) {
            if (!text) return null;

            const match = text.match(/(\d{1,2}:\d{2}\s*[AP]M)\s*[-–—]\s*(\d{1,2}:\d{2}\s*[AP]M)/i);

            if (!match) return null;

            return {
                start: timeStringToDecimal(match[1]),
                end: timeStringToDecimal(match[2])
            };
        }

        function timeStringToDecimal(timeText) {
            if (!timeText) return 0;

            const match = timeText.match(/(\d{1,2}):(\d{2})\s*(AM|PM)?/i);

            if (!match) return 0;

            let hour = parseInt(match[1], 10);
            const minute = parseInt(match[2], 10);
            const suffix = match[3] ? match[3].toUpperCase() : "";

            if (suffix === "PM" && hour !== 12) hour += 12;
            if (suffix === "AM" && hour === 12) hour = 0;

            return hour + minute / 60;
        }

        function decimalToTime(value) {
            let hour = Math.floor(value);
            const minute = Math.round((value - hour) * 60);

            const suffix = hour >= 12 ? "PM" : "AM";

            let displayHour = hour % 12;
            if (displayHour === 0) displayHour = 12;

            return displayHour + ":" + String(minute).padStart(2, "0") + " " + suffix;
        }

        function formatHour(hour) {
            const suffix = hour >= 12 ? "PM" : "AM";

            let displayHour = hour % 12;
            if (displayHour === 0) displayHour = 12;

            return displayHour + ":00 " + suffix;
        }

        function lightenRgb(rgb, amount) {
            return {
                r: Math.round(rgb.r + (255 - rgb.r) * amount),
                g: Math.round(rgb.g + (255 - rgb.g) * amount),
                b: Math.round(rgb.b + (255 - rgb.b) * amount)
            };
        }

        function cssColorToRgb(color) {
            if (!color) return { r: 224, g: 22, b: 43 };

            if (color.startsWith("#")) {
                let hex = color.replace("#", "");

                if (hex.length === 3) {
                    hex = hex.split("").map(function (c) {
                        return c + c;
                    }).join("");
                }

                const value = parseInt(hex, 16);

                if (isNaN(value)) return { r: 224, g: 22, b: 43 };

                return {
                    r: (value >> 16) & 255,
                    g: (value >> 8) & 255,
                    b: value & 255
                };
            }

            const rgbMatch = color.match(/rgba?\((\d+),\s*(\d+),\s*(\d+)/i);

            if (rgbMatch) {
                return {
                    r: parseInt(rgbMatch[1], 10),
                    g: parseInt(rgbMatch[2], 10),
                    b: parseInt(rgbMatch[3], 10)
                };
            }

            return { r: 224, g: 22, b: 43 };
        }
    </script>

</asp:Content>