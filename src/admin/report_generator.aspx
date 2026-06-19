<%@ Page Language="C#" MasterPageFile="~/admin/AdminLayout.master" AutoEventWireup="true" CodeBehind="report_generator.aspx.cs" Inherits="src.admin.report_generator" Title="Report Generator - INTI Admin Portal" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnReportType" runat="server" Value="student" />

    <div>
        <p class="text-slate-500" style="font-size:13px;font-weight:500">Admin</p>
        <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">Report Generator</h1>
        <p class="mt-1 text-slate-500" style="font-size:14px">Select a report, apply filters, preview, and export to PDF or Excel.</p>
    </div>

    <section class="mt-6" style="display:flex;flex-wrap:wrap;gap:16px;align-items:flex-start">
        <%-- Report types --%>
        <div class="rounded-lg border border-slate-200 bg-white" style="flex:1 1 320px;max-width:400px;min-width:0">
            <div class="border-b border-slate-100 px-6 py-4"><h2 class="text-slate-900" style="font-size:15px;font-weight:700">Report Types</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">Choose a report to generate</p></div>
            <ul class="p-2" id="report-list">
                <li><button type="button" data-report data-report-key="student" data-report-title="Student Academic Report" data-report-desc="Per-student academic summary with grades and CGPA" data-active="true" class="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left hover:bg-slate-50 data-[active=true]:bg-[#e0162b]/10"><span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-4 w-4"></i></span><div class="min-w-0 flex-1"><div class="flex items-center justify-between"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Student Academic Report</div><i data-lucide="chevron-right" class="h-4 w-4 text-slate-400"></i></div><div class="text-slate-500 truncate" style="font-size:12px">Per-student academic summary with grades and CGPA</div></div></button></li>
                <li><button type="button" data-report data-report-key="programme" data-report-title="Programme Performance Report" data-report-desc="Programme-level pass rate, GPA, enrolment" class="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left hover:bg-slate-50 data-[active=true]:bg-[#e0162b]/10"><span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-4 w-4"></i></span><div class="min-w-0 flex-1"><div class="flex items-center justify-between"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Programme Performance Report</div><i data-lucide="chevron-right" class="h-4 w-4 text-slate-400"></i></div><div class="text-slate-500 truncate" style="font-size:12px">Programme-level pass rate, GPA, enrolment</div></div></button></li>
                <li><button type="button" data-report data-report-key="course" data-report-title="Course Performance Report" data-report-desc="Course-level outcomes and grade distribution" class="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left hover:bg-slate-50 data-[active=true]:bg-[#e0162b]/10"><span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-4 w-4"></i></span><div class="min-w-0 flex-1"><div class="flex items-center justify-between"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Course Performance Report</div><i data-lucide="chevron-right" class="h-4 w-4 text-slate-400"></i></div><div class="text-slate-500 truncate" style="font-size:12px">Course-level outcomes and grade distribution</div></div></button></li>
                <li><button type="button" data-report data-report-key="attendance" data-report-title="Attendance Summary Report" data-report-desc="Attendance percentage by course" class="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left hover:bg-slate-50 data-[active=true]:bg-[#e0162b]/10"><span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-4 w-4"></i></span><div class="min-w-0 flex-1"><div class="flex items-center justify-between"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Attendance Summary Report</div><i data-lucide="chevron-right" class="h-4 w-4 text-slate-400"></i></div><div class="text-slate-500 truncate" style="font-size:12px">Attendance percentage by course</div></div></button></li>
                <li><button type="button" data-report data-report-key="atrisk" data-report-title="At-Risk Student Report" data-report-desc="Students requiring academic intervention" class="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left hover:bg-slate-50 data-[active=true]:bg-[#e0162b]/10"><span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-4 w-4"></i></span><div class="min-w-0 flex-1"><div class="flex items-center justify-between"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">At-Risk Student Report</div><i data-lucide="chevron-right" class="h-4 w-4 text-slate-400"></i></div><div class="text-slate-500 truncate" style="font-size:12px">Students requiring academic intervention</div></div></button></li>
                <li><button type="button" data-report data-report-key="top" data-report-title="Top-Performing Student Report" data-report-desc="Dean's List / scholarship candidates" class="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left hover:bg-slate-50 data-[active=true]:bg-[#e0162b]/10"><span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-4 w-4"></i></span><div class="min-w-0 flex-1"><div class="flex items-center justify-between"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Top-Performing Student Report</div><i data-lucide="chevron-right" class="h-4 w-4 text-slate-400"></i></div><div class="text-slate-500 truncate" style="font-size:12px">Dean's List / scholarship candidates</div></div></button></li>
                <li><button type="button" data-report class="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left hover:bg-slate-50 data-[active=true]:bg-[#e0162b]/10"><span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-4 w-4"></i></span><div class="min-w-0 flex-1"><div class="flex items-center justify-between"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Enrolment Summary Report</div><i data-lucide="chevron-right" class="h-4 w-4 text-slate-400"></i></div><div class="text-slate-500 truncate" style="font-size:12px">Semester-by-semester enrolment overview</div></div></button></li>
            </ul>
        </div>

        <%-- Config + Preview --%>
        <div class="space-y-6" style="flex:2 1 480px;min-width:0">
            <div class="rounded-lg border border-slate-200 bg-white">
                <div class="flex items-start justify-between gap-3 border-b border-slate-100 px-6 py-4">
                    <div>
                        <div class="flex items-center gap-2">
                            <h2 id="reportTitle" class="text-slate-900" style="font-size:18px;font-weight:700;letter-spacing:-0.01em">Student Academic Report</h2>
                            <span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-sky-50 text-sky-700 border-sky-100" style="font-size:11.5px;font-weight:600">PDF &middot; Excel</span>
                        </div>
                        <p id="reportDescription" class="mt-1 text-slate-500" style="font-size:13px">Per-student academic summary with grades and CGPA</p>
                    </div>

                    <div class="flex items-center gap-2">
                        <button 
                            type="button" 
                            id="btnGeneratePdf"
                            data-toast="Student Academic Report generated"
                            data-toast-desc="PDF preview opened"
                            class="inline-flex items-center gap-1.5 rounded-md border border-slate-200 bg-white px-3 h-10 text-slate-700 hover:bg-slate-50 transition-colors" 
                            style="font-size:13px;font-weight:600">
                            <i data-lucide="file-text" class="h-4 w-4"></i> Generate PDF
                        </button>

                        <button 
                            type="button" 
                            id="btnGenerateExcel"
                            data-toast="Student Academic Report generated"
                            data-toast-desc="Excel file downloaded"
                            class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]" 
                            style="font-size:13px;font-weight:600">
                            <i data-lucide="file-spreadsheet" class="h-4 w-4"></i> Generate Excel
                        </button>
                    </div>
                </div>

                <div class="px-6 py-6">
                    <h3 class="text-slate-900" style="font-size:14px;font-weight:700">Filters</h3>
                    <p class="mt-0.5 text-slate-500" style="font-size:12.5px">Refine the report scope before generating.</p>

                    <div class="mt-5" style="display:grid;grid-template-columns:repeat(auto-fit,minmax(240px,1fr));gap:16px">

                        <label class="block">
                            <span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Semester</span>
                            <div class="mt-1.5">
                                <asp:DropDownList 
                                    ID="ddlSemester" 
                                    runat="server"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="Filter_Changed"
                                    CssClass="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10"
                                    style="font-size:13px">
                                </asp:DropDownList>
                            </div>
                        </label>

                        <label class="block">
                            <span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Programme</span>
                            <div class="mt-1.5">
                                <asp:DropDownList 
                                    ID="ddlProgramme" 
                                    runat="server"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="Filter_Changed"
                                    CssClass="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10"
                                    style="font-size:13px">
                                </asp:DropDownList>
                            </div>
                        </label>

                    </div>

                    <div class="mt-4" style="display:grid;grid-template-columns:repeat(auto-fit,minmax(180px,1fr));gap:16px">

                        <label class="block">
                            <span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Date From</span>
                            <div class="mt-1.5">
                                <asp:TextBox 
                                    ID="txtDateFrom"
                                    runat="server"
                                    TextMode="Date"
                                    AutoPostBack="true"
                                    OnTextChanged="Filter_Changed"
                                    CssClass="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10"
                                    style="font-size:13px">
                                </asp:TextBox>
                            </div>
                        </label>

                        <label class="block">
                            <span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Date To</span>
                            <div class="mt-1.5">
                                <asp:TextBox 
                                    ID="txtDateTo"
                                    runat="server"
                                    TextMode="Date"
                                    AutoPostBack="true"
                                    OnTextChanged="Filter_Changed"
                                    CssClass="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10"
                                    style="font-size:13px">
                                </asp:TextBox>
                            </div>
                        </label>

                        <label id="statusFilterField" class="block">
                            <span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Status</span>
                            <div class="mt-1.5">
                                <asp:DropDownList 
                                    ID="ddlStatus" 
                                    runat="server"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="Filter_Changed"
                                    CssClass="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10"
                                    style="font-size:13px">
                                </asp:DropDownList>
                            </div>
                        </label>

                    </div>
                </div>
            </div>

            <div class="rounded-lg border border-slate-200 bg-white">
                <div class="flex items-start justify-between gap-3 border-b border-slate-100 px-6 py-4">
                    <div>
                        <h2 class="text-slate-900" style="font-size:15px;font-weight:700">Preview</h2>
                        <p class="mt-0.5 text-slate-500" style="font-size:12.5px">Sample of records that will appear in the report</p>
                    </div>
                </div>

                <div class="overflow-x-auto px-6 py-4">
                    <div data-report-preview="student">
                        <table id="tblReportPreview" class="min-w-full">
                            <thead class="border-b border-slate-100 text-slate-500">
                                <tr>
                                    <th class="py-2 text-left uppercase" style="font-size:11px;font-weight:600">ID</th>
                                    <th class="py-2 text-left uppercase" style="font-size:11px;font-weight:600">Name</th>
                                    <th class="py-2 text-left uppercase" style="font-size:11px;font-weight:600">Programme</th>
                                    <th class="py-2 text-left uppercase" style="font-size:11px;font-weight:600">Semester</th>
                                    <th class="py-2 text-right uppercase" style="font-size:11px;font-weight:600">CGPA</th>
                                    <th class="py-2 text-left uppercase" style="font-size:11px;font-weight:600">Status</th>
                                </tr>
                            </thead>

                            <tbody>
                                <asp:Repeater ID="rptPreview" runat="server">
                                    <ItemTemplate>
                                        <tr class="border-b border-slate-100" style="font-size:12.5px">
                                            <td class="py-2 text-slate-500"><%# Eval("StudentNo") %></td>
                                            <td class="py-2 text-slate-900"><%# Eval("StudentName") %></td>
                                            <td class="py-2"><%# Eval("Programme") %></td>
                                            <td class="py-2"><%# Eval("SemesterName") %></td>
                                            <td class="py-2 text-right"><%# Eval("CGPADisplay") %></td>
                                            <td class="py-2"><%# Eval("Status") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                        <div class="mt-3 text-slate-400" style="font-size:11.5px">
                            <asp:Literal ID="litPreviewCount" runat="server"></asp:Literal>
                        </div>

                        <asp:PlaceHolder ID="emptyPreviewPanel" runat="server" Visible="false">
                            <p class="mt-3 rounded-lg border border-dashed border-slate-200 p-4 text-slate-500" style="font-size:13px">
                                No records found for the selected filters.
                            </p>
                        </asp:PlaceHolder>
                    </div>

                    <div data-report-preview="programme" style="display:none">
                        <table id="tblProgrammePreview" class="min-w-full">
                            <thead class="border-b border-slate-100 text-slate-500">
                                <tr>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Code</th>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Programme Name</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Students</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Avg GPA</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Avg CGPA</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Pass Rate</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Fail Rate</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Completion</th>
                                    <th class="py-2 text-left uppercase" style="font-size:11px;font-weight:600">Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptProgrammePreview" runat="server">
                                    <ItemTemplate>
                                        <tr class="border-b border-slate-100" style="font-size:12.5px">
                                            <td class="py-2 pr-4 text-slate-900 font-medium"><%# Eval("ProgrammeCode") %></td>
                                            <td class="py-2 pr-4 text-slate-900"><%# Eval("ProgrammeName") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("Students") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("AvgGpaDisplay") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("AvgCgpaDisplay") %></td>
                                            <td class="py-2 pr-4 text-right text-emerald-600 font-semibold"><%# Eval("PassRateDisplay") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("FailRateDisplay") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("CompletionRateDisplay") %></td>
                                            <td class="py-2"><span class='<%# Eval("StatusCssClass") %>' style="font-size:11.5px;font-weight:600"><%# Eval("Status") %></span></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                        <div class="mt-3 text-slate-400" style="font-size:11.5px">
                            <asp:Literal ID="litProgrammePreviewCount" runat="server"></asp:Literal>
                        </div>

                        <asp:PlaceHolder ID="emptyProgrammePreviewPanel" runat="server" Visible="false">
                            <p class="mt-3 rounded-lg border border-dashed border-slate-200 p-4 text-slate-500" style="font-size:13px">
                                No programme performance records found for the selected filters.
                            </p>
                        </asp:PlaceHolder>
                    </div>

                    <div data-report-preview="course" style="display:none">
                        <table id="tblCoursePreview" class="min-w-full">
                            <thead class="border-b border-slate-100 text-slate-500">
                                <tr>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Code</th>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Course Name</th>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Programme</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Enrolled</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Graded</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Avg GPA</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Pass Rate</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">A</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">B</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">C</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">D</th>
                                    <th class="py-2 text-right uppercase" style="font-size:11px;font-weight:600">F</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptCoursePreview" runat="server">
                                    <ItemTemplate>
                                        <tr class="border-b border-slate-100" style="font-size:12.5px">
                                            <td class="py-2 pr-4 text-slate-900 font-medium"><%# Eval("CourseCode") %></td>
                                            <td class="py-2 pr-4 text-slate-900"><%# Eval("CourseName") %></td>
                                            <td class="py-2 pr-4 text-slate-700"><%# Eval("Programme") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("Enrolled") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("Graded") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("AvgGpaDisplay") %></td>
                                            <td class="py-2 pr-4 text-right text-emerald-600 font-semibold"><%# Eval("PassRateDisplay") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("GradeA") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("GradeB") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("GradeC") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("GradeD") %></td>
                                            <td class="py-2 text-right text-slate-700"><%# Eval("GradeF") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                        <div class="mt-3 text-slate-400" style="font-size:11.5px">
                            <asp:Literal ID="litCoursePreviewCount" runat="server"></asp:Literal>
                        </div>

                        <asp:PlaceHolder ID="emptyCoursePreviewPanel" runat="server" Visible="false">
                            <p class="mt-3 rounded-lg border border-dashed border-slate-200 p-4 text-slate-500" style="font-size:13px">
                                No course performance records found for the selected filters.
                            </p>
                        </asp:PlaceHolder>
                    </div>

                    <div data-report-preview="attendance" style="display:none">
                        <table id="tblAttendancePreview" class="min-w-full">
                            <thead class="border-b border-slate-100 text-slate-500">
                                <tr>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Programme</th>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Course</th>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Semester</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Students</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Sessions</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Present</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Late</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Absent</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Recorded</th>
                                    <th class="py-2 text-right uppercase" style="font-size:11px;font-weight:600">Attendance</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptAttendancePreview" runat="server">
                                    <ItemTemplate>
                                        <tr class="border-b border-slate-100" style="font-size:12.5px">
                                            <td class="py-2 pr-4 text-slate-700"><%# Eval("Programme") %></td>
                                            <td class="py-2 pr-4 text-slate-700"><span class="font-medium"><%# Eval("CourseCode") %></span> - <%# Eval("CourseName") %></td>
                                            <td class="py-2 pr-4 text-slate-700"><%# Eval("SemesterName") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("EnrolledStudents") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("ClassSessions") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("PresentCount") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("LateCount") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("AbsentCount") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("RecordedSessions") %></td>
                                            <td class="py-2 text-right text-slate-900 font-semibold"><%# Eval("AttendancePercentageDisplay") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                        <div class="mt-3 text-slate-400" style="font-size:11.5px">
                            <asp:Literal ID="litAttendancePreviewCount" runat="server"></asp:Literal>
                        </div>

                        <asp:PlaceHolder ID="emptyAttendancePreviewPanel" runat="server" Visible="false">
                            <p class="mt-3 rounded-lg border border-dashed border-slate-200 p-4 text-slate-500" style="font-size:13px">
                                No attendance records found for the selected filters.
                            </p>
                        </asp:PlaceHolder>
                    </div>

                    <div data-report-preview="atrisk" style="display:none">
                        <table id="tblAtRiskPreview" class="min-w-full">
                            <thead class="border-b border-slate-100 text-slate-500">
                                <tr>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Student ID</th>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Student Name</th>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Programme</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Semester</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">CGPA</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Attendance</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Failed</th>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Risk</th>
                                    <th class="py-2 text-left uppercase" style="font-size:11px;font-weight:600">Reason</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptAtRiskPreview" runat="server">
                                    <ItemTemplate>
                                        <tr class="border-b border-slate-100" style="font-size:12.5px">
                                            <td class="py-2 pr-4 text-slate-500"><%# Eval("StudentNo") %></td>
                                            <td class="py-2 pr-4 text-slate-900"><%# Eval("StudentName") %></td>
                                            <td class="py-2 pr-4 text-slate-700"><%# Eval("Programme") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("SemesterNo") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("CgpaDisplay") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("AttendancePercentageDisplay") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("FailedCourses") %></td>
                                            <td class="py-2 pr-4 text-[#a01020] font-semibold"><%# Eval("RiskLevel") %></td>
                                            <td class="py-2 text-slate-700"><%# Eval("RiskReason") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                        <div class="mt-3 text-slate-400" style="font-size:11.5px">
                            <asp:Literal ID="litAtRiskPreviewCount" runat="server"></asp:Literal>
                        </div>

                        <asp:PlaceHolder ID="emptyAtRiskPreviewPanel" runat="server" Visible="false">
                            <p class="mt-3 rounded-lg border border-dashed border-slate-200 p-4 text-slate-500" style="font-size:13px">
                                No at-risk students found for the selected filters.
                            </p>
                        </asp:PlaceHolder>
                    </div>

                    <div data-report-preview="top" style="display:none">
                        <table id="tblTopPerformingPreview" class="min-w-full">
                            <thead class="border-b border-slate-100 text-slate-500">
                                <tr>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Student ID</th>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Student Name</th>
                                    <th class="py-2 pr-4 text-left uppercase" style="font-size:11px;font-weight:600">Programme</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Semester</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">Courses Graded</th>
                                    <th class="py-2 pr-4 text-right uppercase" style="font-size:11px;font-weight:600">CGPA</th>
                                    <th class="py-2 text-left uppercase" style="font-size:11px;font-weight:600">Eligibility</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptTopPerformingPreview" runat="server">
                                    <ItemTemplate>
                                        <tr class="border-b border-slate-100" style="font-size:12.5px">
                                            <td class="py-2 pr-4 text-slate-500"><%# Eval("StudentNo") %></td>
                                            <td class="py-2 pr-4 text-slate-900"><%# Eval("StudentName") %></td>
                                            <td class="py-2 pr-4 text-slate-700"><%# Eval("Programme") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("SemesterNo") %></td>
                                            <td class="py-2 pr-4 text-right text-slate-700"><%# Eval("CoursesGraded") %></td>
                                            <td class="py-2 pr-4 text-right text-emerald-600 font-semibold"><%# Eval("CgpaDisplay") %></td>
                                            <td class="py-2 text-slate-700"><%# Eval("Eligibility") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                        <div class="mt-3 text-slate-400" style="font-size:11.5px">
                            <asp:Literal ID="litTopPerformingPreviewCount" runat="server"></asp:Literal>
                        </div>

                        <asp:PlaceHolder ID="emptyTopPerformingPreviewPanel" runat="server" Visible="false">
                            <p class="mt-3 rounded-lg border border-dashed border-slate-200 p-4 text-slate-500" style="font-size:13px">
                                No Dean's List or scholarship candidates found for the selected filters.
                            </p>
                        </asp:PlaceHolder>
                    </div>
                </div>
            </div>
        </div>
    </section>

</asp:Content>
<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/admin/shared/icons.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/toast.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/table.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/ui.js") %>"></script>
    <script>
      (function () {
        var config = {
          student: {
            title: "Student Academic Report",
            description: "Per-student academic summary with grades and CGPA",
            tableId: "tblReportPreview",
            fileBase: "Student_Academic_Report",
            sheetName: "Academic Report",
            usesStatus: true
          },
          programme: {
            title: "Programme Performance Report",
            description: "Programme-level pass rate, GPA, enrolment",
            tableId: "tblProgrammePreview",
            fileBase: "Programme_Performance_Report",
            sheetName: "Programme Report",
            usesStatus: true
          },
          course: {
            title: "Course Performance Report",
            description: "Course-level outcomes and grade distribution",
            tableId: "tblCoursePreview",
            fileBase: "Course_Performance_Report",
            sheetName: "Course Report",
            usesStatus: false
          },
          attendance: {
            title: "Attendance Summary Report",
            description: "Attendance percentage by course",
            tableId: "tblAttendancePreview",
            fileBase: "Attendance_Summary_Report",
            sheetName: "Attendance Report",
            usesStatus: false
          },
          atrisk: {
            title: "At-Risk Student Report",
            description: "Students requiring academic intervention",
            tableId: "tblAtRiskPreview",
            fileBase: "At_Risk_Student_Report",
            sheetName: "At-Risk Students",
            usesStatus: false
          },
          top: {
            title: "Top-Performing Student Report",
            description: "Dean's List / scholarship candidates",
            tableId: "tblTopPerformingPreview",
            fileBase: "Top_Performing_Student_Report",
            sheetName: "Top Performers",
            usesStatus: false
          }
        };
        var activeKey = "student";

        function getActiveConfig() {
          return config[activeKey] || config.student;
        }

        function visibleTable() {
          return document.getElementById(getActiveConfig().tableId);
        }

        function tableHeaders(table) {
          return Array.prototype.slice.call(table.querySelectorAll("thead th")).map(function (th) {
            return th.innerText.trim();
          });
        }

        function bodyRows(table) {
          return Array.prototype.slice.call(table.querySelectorAll("tbody tr")).filter(function (row) {
            return row.offsetParent !== null || row.style.display !== "none";
          });
        }

        function setReport(key) {
          activeKey = config[key] ? key : "student";
          var current = getActiveConfig();
          var hidden = document.querySelector("[id$='hdnReportType']");
          var title = document.getElementById("reportTitle");
          var desc = document.getElementById("reportDescription");
          var pdfBtn = document.getElementById("btnGeneratePdf");
          var excelBtn = document.getElementById("btnGenerateExcel");
          var statusFilter = document.getElementById("statusFilterField");
          if (hidden) hidden.value = activeKey;
          if (title) title.textContent = current.title;
          if (desc) desc.textContent = current.description;
          if (pdfBtn) pdfBtn.setAttribute("data-toast", current.title + " generated");
          if (excelBtn) excelBtn.setAttribute("data-toast", current.title + " generated");
          if (statusFilter) statusFilter.style.display = current.usesStatus === false ? "none" : "";
          document.querySelectorAll("[data-report-preview]").forEach(function (panel) {
            panel.style.display = panel.getAttribute("data-report-preview") === activeKey ? "" : "none";
          });
        }

        var items = document.querySelectorAll("[data-report]");
        items.forEach(function (b) {
          b.addEventListener("click", function () {
            items.forEach(function (x) { x.setAttribute("data-active", "false"); });
            b.setAttribute("data-active", "true");
            setReport(b.getAttribute("data-report-key") || "student");
          });
        });

        window.REPORT_GENERATOR = {
          getActiveConfig: getActiveConfig,
          getVisibleTable: visibleTable,
          getTableHeaders: tableHeaders,
          getBodyRows: bodyRows
        };
        var hidden = document.querySelector("[id$='hdnReportType']");
        setReport(hidden && hidden.value ? hidden.value : "student");
      })();
    </script>
    <%-- PDF --%>
    <script src="https://unpkg.com/jspdf@2.5.1/dist/jspdf.umd.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/jspdf-autotable@3.8.4/dist/jspdf.plugin.autotable.min.js"></script>
    <%-- Excel --%>
    <script src="https://cdn.jsdelivr.net/npm/xlsx@0.18.5/dist/xlsx.full.min.js"></script>

    <%-- Download Function --%>
    <%-- PDF --%>
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            var btn = document.getElementById("btnGeneratePdf");

            if (!btn) return;

            btn.addEventListener("click", function () {
                if (!window.jspdf || !window.jspdf.jsPDF) {
                    alert("jsPDF is not loaded.");
                    return;
                }

                var report = window.REPORT_GENERATOR && window.REPORT_GENERATOR.getActiveConfig
                    ? window.REPORT_GENERATOR.getActiveConfig()
                    : { title: "Student Academic Report", tableId: "tblReportPreview", fileBase: "Student_Academic_Report" };
                var table = window.REPORT_GENERATOR && window.REPORT_GENERATOR.getVisibleTable
                    ? window.REPORT_GENERATOR.getVisibleTable()
                    : document.getElementById("tblReportPreview");

                if (!table) {
                    alert("Preview table not found.");
                    return;
                }

                const { jsPDF } = window.jspdf;

                const doc = new jsPDF({
                    orientation: "landscape",
                    unit: "mm",
                    format: "a4"
                });

                doc.setFont("helvetica", "bold");
                doc.setFontSize(16);
                doc.text(report.title, 14, 15);

                doc.setFont("helvetica", "normal");
                doc.setFontSize(9);

                var semester = getSelectedTextByIdEnding("ddlSemester");
                var programme = getSelectedTextByIdEnding("ddlProgramme");
                var status = report.usesStatus === false ? "Not applicable" : getSelectedTextByIdEnding("ddlStatus");
                var dateFrom = getValueByIdEnding("txtDateFrom") || "Any";
                var dateTo = getValueByIdEnding("txtDateTo") || "Any";

                var generatedAt = new Date().toLocaleString();
                // Row 1
                doc.setFont("helvetica", "normal");
                doc.setFontSize(9);
                doc.setTextColor(0, 0, 0);

                doc.text("Semester: " + semester, 14, 23);
                doc.text("Programme: " + programme, 90, 23);
                doc.text("Status: " + status, 190, 23);

                // Row 2
                doc.text("Date From: " + dateFrom, 14, 30);
                doc.text("Date To: " + dateTo, 90, 30);

                // Row 3
                doc.setFontSize(8);
                doc.setTextColor(100, 116, 139);
                doc.text("Generated at: " + generatedAt, 14, 37);


                var bodyRows = window.REPORT_GENERATOR && window.REPORT_GENERATOR.getBodyRows
                    ? window.REPORT_GENERATOR.getBodyRows(table)
                    : table.querySelectorAll("tbody tr");
                var headers = window.REPORT_GENERATOR && window.REPORT_GENERATOR.getTableHeaders
                    ? window.REPORT_GENERATOR.getTableHeaders(table)
                    : ["ID", "Name", "Programme", "Semester", "CGPA", "Status"];

                if (bodyRows.length === 0) {
                    doc.autoTable({
                        head: [headers],
                        body: [
                            [
                                {
                                    content: "No records found for the selected filters.",
                                    colSpan: headers.length,
                                    styles: {
                                        halign: "left",
                                        textColor: [71, 85, 105],
                                        fontStyle: "normal"
                                    }
                                }
                            ]
                        ],
                        startY: 43,
                        theme: "grid",
                        styles: {
                            fontSize: 8,
                            cellPadding: 3
                        },
                        headStyles: {
                            fillColor: [224, 22, 43],
                            textColor: [255, 255, 255],
                            fontStyle: "bold"
                        }
                    });
                } else {
                    doc.autoTable({
                        html: "#" + table.id,
                        startY: 43,
                        theme: "grid",
                        styles: {
                            fontSize: 8,
                            cellPadding: 2
                        },
                        headStyles: {
                            fillColor: [224, 22, 43],
                            textColor: [255, 255, 255],
                            fontStyle: "bold"
                        },
                        alternateRowStyles: {
                            fillColor: [248, 250, 252]
                        }
                    });
                }

                doc.setProperties({
                    title: report.title,
                    subject: report.title,
                    author: "INTI Admin Portal",
                    creator: "INTI Admin Portal"
                });

                var pdfBlob = doc.output("blob");
                var pdfUrl = URL.createObjectURL(pdfBlob);

                var previewWindow = window.open("", "_" + report.fileBase);

                if (!previewWindow) {
                    alert("Please allow pop-ups to preview the PDF.");
                    return;
                }

                previewWindow.document.open();
                previewWindow.document.write(
                    '<!DOCTYPE html>' +
                    '<html>' +
                    '<head>' +
                    '<title>' + escapeHtml(report.title) + '</title>' +
                    '<style>' +
                    'html, body { margin:0; height:100%; font-family:"Inter","Segoe UI",Arial,helvetica; background:#f8fafc; }' +
                    '.topbar { height:54px; display:flex; align-items:center; justify-content:space-between; padding:0 18px; border-bottom:1px solid #e5e7eb; background:#f8fafc; box-shadow:0 1px 4px rgba(15,23,42,0.06); }' +
                    '.title { font-size:15px; font-weight:750; color:#0f172a; letter-spacing:-0.01em; border-radius:999px; padding:7px 12px; background:#ffffff; }' +
                    '.download { background:#e0162b; color:white; text-decoration:none; padding:9px 16px; border-radius:999px; font-size:12px; font-weight:700; box-shadow:0 8px 18px -10px rgba(224,22,43,0.9); transition:all .15s ease; }' +
                    'iframe { width:100%; height:calc(100% - 55px); border:none; }' +
                    '</style>' +
                    '</head>' +
                    '<body>' +
                    '<div class="topbar">' +
                    '<div class="title">' + escapeHtml(report.title) + '</div>' +
                    '<a class="download" href="' + pdfUrl + '" download="' + report.fileBase + '.pdf">Download PDF</a>' +
                    '</div>' +
                    '<iframe src="' + pdfUrl + '#toolbar=0&navpanes=0&scrollbar=1"></iframe>' +
                    '</body>' +
                    '</html>'
                );
                previewWindow.document.close();

                try {
                    previewWindow.history.replaceState(
                        null,
                        report.title,
                        "pdfdownload" + report.fileBase.toLowerCase().replace(/_/g, "")
                    );
                } catch (e) {
                    console.warn("Could not change preview URL:", e);
                }

            });
        });


 
    <%-- Excel --%>

        document.addEventListener("DOMContentLoaded", function () {
            var btnExcel = document.getElementById("btnGenerateExcel");

            if (!btnExcel) return;

            btnExcel.addEventListener("click", function () {
                if (typeof XLSX === "undefined") {
                    alert("Excel library is not loaded.");
                    return;
                }

                var report = window.REPORT_GENERATOR && window.REPORT_GENERATOR.getActiveConfig
                    ? window.REPORT_GENERATOR.getActiveConfig()
                    : { title: "Student Academic Report", tableId: "tblReportPreview", fileBase: "Student_Academic_Report", sheetName: "Academic Report" };
                var table = window.REPORT_GENERATOR && window.REPORT_GENERATOR.getVisibleTable
                    ? window.REPORT_GENERATOR.getVisibleTable()
                    : document.getElementById("tblReportPreview");

                if (!table) {
                    alert("Preview table not found.");
                    return;
                }

                var semester = getSelectedTextByIdEnding("ddlSemester");
                var programme = getSelectedTextByIdEnding("ddlProgramme");
                var status = report.usesStatus === false ? "Not applicable" : getSelectedTextByIdEnding("ddlStatus");
                var dateFrom = getValueByIdEnding("txtDateFrom") || "Any";
                var dateTo = getValueByIdEnding("txtDateTo") || "Any";

                var bodyRows = window.REPORT_GENERATOR && window.REPORT_GENERATOR.getBodyRows
                    ? window.REPORT_GENERATOR.getBodyRows(table)
                    : table.querySelectorAll("tbody tr");
                var headers = window.REPORT_GENERATOR && window.REPORT_GENERATOR.getTableHeaders
                    ? window.REPORT_GENERATOR.getTableHeaders(table)
                    : ["ID", "Name", "Programme", "Semester", "CGPA", "Status"];

                var excelData = [];

                excelData.push([report.title]);
                excelData.push(["Semester", semester]);
                excelData.push(["Programme", programme]);
                excelData.push(["Status", status]);
                excelData.push(["Date From", dateFrom]);
                excelData.push(["Date To", dateTo]);
                excelData.push(["Generated At", new Date().toLocaleString()]);
                excelData.push([]);
                excelData.push(headers);

                if (bodyRows.length === 0) {
                    excelData.push(headers.map(function (_, i) { return i === 0 ? "No records found for the selected filters." : ""; }));
                } else {
                    bodyRows.forEach(function (row) {
                        var cells = row.querySelectorAll("td");
                        var rowData = [];

                        cells.forEach(function (cell) {
                            rowData.push(cell.innerText.trim());
                        });

                        excelData.push(rowData);
                    });
                }

                var worksheet = XLSX.utils.aoa_to_sheet(excelData);


                // Make Excel columns wider based on the longest text in each column
                var colCount = headers.length;
                var colWidths = [];

                for (var col = 0; col < colCount; col++) {
                    var maxLength = headers[col] ? headers[col].length : 12;

                    excelData.forEach(function (row) {
                        var cellValue = row[col] ? row[col].toString() : "";
                        if (cellValue.length > maxLength) {
                            maxLength = cellValue.length;
                        }
                    });

                    colWidths.push({
                        wch: Math.max(18, Math.min(50, maxLength + 3))
                    });
                }

                worksheet["!cols"] = colWidths;

                worksheet["!rows"] = excelData.map(function (_, index) {
                    return {
                        hpt: index === 0 ? 26 : 22
                    };
                });

                var workbook = XLSX.utils.book_new();

                XLSX.utils.book_append_sheet(workbook, worksheet, report.sheetName || "Report");

                XLSX.writeFile(workbook, report.fileBase + ".xlsx");
                var workbook = XLSX.utils.book_new();

            });
        });

        function getSelectedTextByIdEnding(idEnding) {
            var el = document.querySelector("[id$='" + idEnding + "']");

            if (!el) return "All";

            if (el.selectedIndex >= 0) {
                return el.options[el.selectedIndex].text;
            }

            return el.value || "All";
        }

        function getValueByIdEnding(idEnding) {
            var el = document.querySelector("[id$='" + idEnding + "']");
            return el ? el.value : "";
        }

        function escapeHtml(value) {
            return String(value == null ? "" : value).replace(/[&<>"']/g, function (c) {
                return { "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c];
            });
        }
    </script>
</asp:Content>
