<%@ Page Language="C#" MasterPageFile="~/admin/AdminLayout.master" AutoEventWireup="true" CodeBehind="report_generator.aspx.cs" Inherits="src.admin.report_generator" Title="Report Generator - INTI Admin Portal" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">

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
                <li><button type="button" data-report data-active="true" class="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left hover:bg-slate-50 data-[active=true]:bg-[#e0162b]/10"><span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-4 w-4"></i></span><div class="min-w-0 flex-1"><div class="flex items-center justify-between"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Student Academic Report</div><i data-lucide="chevron-right" class="h-4 w-4 text-slate-400"></i></div><div class="text-slate-500 truncate" style="font-size:12px">Per-student academic summary with grades and CGPA</div></div></button></li>
                <li><button type="button" data-report class="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left hover:bg-slate-50 data-[active=true]:bg-[#e0162b]/10"><span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-4 w-4"></i></span><div class="min-w-0 flex-1"><div class="flex items-center justify-between"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Programme Performance Report</div><i data-lucide="chevron-right" class="h-4 w-4 text-slate-400"></i></div><div class="text-slate-500 truncate" style="font-size:12px">Programme-level pass rate, GPA, enrolment</div></div></button></li>
                <li><button type="button" data-report class="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left hover:bg-slate-50 data-[active=true]:bg-[#e0162b]/10"><span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-4 w-4"></i></span><div class="min-w-0 flex-1"><div class="flex items-center justify-between"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Course Performance Report</div><i data-lucide="chevron-right" class="h-4 w-4 text-slate-400"></i></div><div class="text-slate-500 truncate" style="font-size:12px">Course-level outcomes and grade distribution</div></div></button></li>
                <li><button type="button" data-report class="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left hover:bg-slate-50 data-[active=true]:bg-[#e0162b]/10"><span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-4 w-4"></i></span><div class="min-w-0 flex-1"><div class="flex items-center justify-between"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Attendance Summary Report</div><i data-lucide="chevron-right" class="h-4 w-4 text-slate-400"></i></div><div class="text-slate-500 truncate" style="font-size:12px">Attendance percentages by student / course</div></div></button></li>
                <li><button type="button" data-report class="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left hover:bg-slate-50 data-[active=true]:bg-[#e0162b]/10"><span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-4 w-4"></i></span><div class="min-w-0 flex-1"><div class="flex items-center justify-between"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">At-Risk Student Report</div><i data-lucide="chevron-right" class="h-4 w-4 text-slate-400"></i></div><div class="text-slate-500 truncate" style="font-size:12px">Students requiring academic intervention</div></div></button></li>
                <li><button type="button" data-report class="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left hover:bg-slate-50 data-[active=true]:bg-[#e0162b]/10"><span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-4 w-4"></i></span><div class="min-w-0 flex-1"><div class="flex items-center justify-between"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Top-Performing Student Report</div><i data-lucide="chevron-right" class="h-4 w-4 text-slate-400"></i></div><div class="text-slate-500 truncate" style="font-size:12px">Dean's List / scholarship candidates</div></div></button></li>
                <li><button type="button" data-report class="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left hover:bg-slate-50 data-[active=true]:bg-[#e0162b]/10"><span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 text-slate-700"><i data-lucide="file-text" class="h-4 w-4"></i></span><div class="min-w-0 flex-1"><div class="flex items-center justify-between"><div class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Enrolment Summary Report</div><i data-lucide="chevron-right" class="h-4 w-4 text-slate-400"></i></div><div class="text-slate-500 truncate" style="font-size:12px">Semester-by-semester enrolment overview</div></div></button></li>
            </ul>
        </div>

        <%-- Config + Preview --%>
        <div class="space-y-6" style="flex:2 1 480px;min-width:0">
            <div class="rounded-lg border border-slate-200 bg-white">
                <div class="flex items-start justify-between gap-3 border-b border-slate-100 px-6 py-4">
                    <div>
                        <div class="flex items-center gap-2">
                            <h2 id="report-title" class="text-slate-900" style="font-size:18px;font-weight:700;letter-spacing:-0.01em"><%= ReportTitle %></h2>
                            <span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-sky-50 text-sky-700 border-sky-100" style="font-size:11.5px;font-weight:600">PDF &middot; Excel</span>
                        </div>
                        <p class="mt-1 text-slate-500" style="font-size:13px">Per-student academic summary with grades and CGPA</p>
                    </div>

                    <div class="flex items-center gap-2">
                        <button 
                            type="button" 
                            id="btnGeneratePdf"
                            class="inline-flex items-center gap-1.5 rounded-md border border-slate-200 bg-white px-3 h-10 text-slate-700 hover:bg-slate-50 transition-colors" 
                            style="font-size:13px;font-weight:600">
                            <i data-lucide="file-text" class="h-4 w-4"></i> Generate PDF
                        </button>

                        <button 
                            type="button" 
                            id="btnGenerateExcel"
                            class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]" 
                            style="font-size:13px;font-weight:600">
                            <i data-lucide="file-spreadsheet" class="h-4 w-4"></i> Generate Excel
                        </button>
                    </div>
                </div>

                <div class="px-6 py-6">
                    <h3 class="text-slate-900" style="font-size:14px;font-weight:700">Filters</h3>
                    <p class="mt-0.5 text-slate-500" style="font-size:12.5px">Refine the report scope before generating.</p>

                    <div class="mt-5" style="display:grid;grid-template-columns:repeat(auto-fit,minmax(200px,1fr));gap:16px">

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

                        <label class="block">
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
                    <table id="tblReportPreview" class="min-w-full">
                        <thead class="border-b border-slate-100 text-slate-500">
                            <tr><%= PreviewHeadersHtml %></tr>
                        </thead>

                        <tbody>
                            <%= PreviewRowsHtml %>
                        </tbody>
                    </table>

                    <div class="mt-3 text-slate-400" style="font-size:11.5px">
                        <%= PreviewCountText %>
                    </div>

                    <% if (!HasPreviewRows) { %>
                        <p class="mt-3 rounded-lg border border-dashed border-slate-200 p-4 text-slate-500" style="font-size:13px">
                            No records found for the selected filters.
                        </p>
                    <% } %>
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
        window.adminReportTitle = "<%= ReportTitle %>";
        var items = document.querySelectorAll("[data-report]");
        var keys = ["student", "programme", "course", "attendance", "risk", "top", "enrolment"];
        var selected = "<%= SelectedReportKey %>";
        items.forEach(function (b) {
          var index = Array.prototype.indexOf.call(items, b);
          b.setAttribute("data-active", keys[index] === selected ? "true" : "false");
          b.addEventListener("click", function () {
            var url = new URL(window.location.href);
            url.searchParams.set("report", keys[index]);
            window.location.href = url.toString();
          });
        });
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

                if (!document.getElementById("tblReportPreview")) {
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
                doc.text(window.adminReportTitle, 14, 15);

                doc.setFont("helvetica", "normal");
                doc.setFontSize(9);

                var semester = getSelectedTextByIdEnding("ddlSemester");
                var programme = getSelectedTextByIdEnding("ddlProgramme");
                var status = getSelectedTextByIdEnding("ddlStatus");
                var dateFrom = getValueByIdEnding("txtDateFrom") || "Any";
                var dateTo = getValueByIdEnding("txtDateTo") || "Any";

                var generatedAt = new Date().toLocaleString();
                var dateFrom = getValueByIdEnding("txtDateFrom") || "Any";
                var dateTo = getValueByIdEnding("txtDateTo") || "Any";

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


                var table = document.getElementById("tblReportPreview");
                var bodyRows = table.querySelectorAll("tbody tr");

                if (bodyRows.length === 0) {
                    doc.autoTable({
                        head: [["ID", "Name", "Programme", "Semester", "CGPA", "Status"]],
                        body: [
                            [
                                {
                                    content: "No records found for the selected filters.",
                                    colSpan: 6,
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
                        html: "#tblReportPreview",
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
                    title: window.adminReportTitle,
                    subject: window.adminReportTitle,
                    author: "INTI Admin Portal",
                    creator: "INTI Admin Portal"
                });

                var pdfBlob = doc.output("blob");
                var pdfUrl = URL.createObjectURL(pdfBlob);

                var previewWindow = window.open("", "_StudentAcademicReport");

                if (!previewWindow) {
                    alert("Please allow pop-ups to preview the PDF.");
                    return;
                }

                previewWindow.document.open();
                previewWindow.document.write(
                    '<!DOCTYPE html>' +
                    '<html>' +
                    '<head>' +
                    '<title>' + window.adminReportTitle + '</title>' +
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
                    '<div class="title">' + window.adminReportTitle + '</div>' +
                    '<a class="download" href="' + pdfUrl + '" download="Student_Academic_Report.pdf">Download PDF</a>' +
                    '</div>' +
                    '<iframe src="' + pdfUrl + '#toolbar=0&navpanes=0&scrollbar=1"></iframe>' +
                    '</body>' +
                    '</html>'
                );
                previewWindow.document.close();

                try {
                    previewWindow.history.replaceState(
                        null,
                        window.adminReportTitle,
                        "pdfdownloadstudentacademicreport"
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

                var table = document.getElementById("tblReportPreview");

                if (!table) {
                    alert("Preview table not found.");
                    return;
                }

                var semester = getSelectedTextByIdEnding("ddlSemester");
                var programme = getSelectedTextByIdEnding("ddlProgramme");
                var status = getSelectedTextByIdEnding("ddlStatus");
                var dateFrom = getValueByIdEnding("txtDateFrom") || "Any";
                var dateTo = getValueByIdEnding("txtDateTo") || "Any";

                var bodyRows = table.querySelectorAll("tbody tr");

                var excelData = [];

                excelData.push([window.adminReportTitle]);
                excelData.push(["Semester", semester]);
                excelData.push(["Programme", programme]);
                excelData.push(["Status", status]);
                excelData.push(["Date From", dateFrom]);
                excelData.push(["Date To", dateTo]);
                excelData.push(["Generated At", new Date().toLocaleString()]);
                excelData.push([]);
                excelData.push(["ID", "Name", "Programme", "Semester", "CGPA", "Status"]);

                if (bodyRows.length === 0) {
                    excelData.push(["No records found for", "", "", "", "", ""]);
                    excelData.push(["the selected filters.", "", "", "", "", ""]);
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

                worksheet["!cols"] = [
                    { wch: 20 },
                    { wch: 28 },
                    { wch: 15 },
                    { wch: 18 },
                    { wch: 10 },
                    { wch: 18 }
                ];

                var workbook = XLSX.utils.book_new();

                XLSX.utils.book_append_sheet(workbook, worksheet, "Academic Report");

                XLSX.writeFile(workbook, "Student_Academic_Report.xlsx");
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
    </script>
</asp:Content>
