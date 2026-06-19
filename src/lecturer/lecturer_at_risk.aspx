<%@ Page Language="C#" MasterPageFile="~/lecturer/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_at_risk.aspx.cs" Inherits="student_information_management_system.lecturer_at_risk" Title="At-risk Students - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div><p class="text-slate-500" style="font-size:13px;font-weight:500">Lecturer Module</p><h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700">At-risk Students</h1><p class="mt-1 text-slate-500" style="font-size:14px">Identify students with weak attendance, low marks, or missing submissions.</p></div>
        <button type="button" data-toast="Intervention list ready for report" class="inline-flex h-10 items-center justify-center gap-1.5 rounded-md border border-slate-200 bg-white px-4 text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:600"><i data-lucide="download" class="h-4 w-4"></i>Export list</button>
    </div>

    <section class="mt-6 grid gap-4 md:grid-cols-4">
        <div class="rounded-lg border border-slate-200 bg-white p-5"><p class="text-slate-500" style="font-size:12.5px">Flagged students</p><p class="mt-1 text-[#e0162b]" style="font-size:28px;font-weight:700"><%= FlaggedCount %></p></div>
        <div class="rounded-lg border border-slate-200 bg-white p-5"><p class="text-slate-500" style="font-size:12.5px">Attendance risk</p><p class="mt-1 text-amber-700" style="font-size:28px;font-weight:700"><%= AttendanceRiskCount %></p></div>
        <div class="rounded-lg border border-slate-200 bg-white p-5"><p class="text-slate-500" style="font-size:12.5px">Academic risk</p><p class="mt-1 text-[#e0162b]" style="font-size:28px;font-weight:700"><%= AcademicRiskCount %></p></div>
        <div class="rounded-lg border border-slate-200 bg-white p-5"><p class="text-slate-500" style="font-size:12.5px">High priority</p><p class="mt-1 text-slate-900" style="font-size:28px;font-weight:700"><%= HighRiskCount %></p></div>
    </section>

    <section class="mt-6 rounded-lg border border-slate-200 bg-white">
        <div class="flex flex-col gap-3 border-b border-slate-100 px-6 py-4 md:flex-row md:items-center md:justify-between">
            <div><h2 class="text-slate-900" style="font-size:16px;font-weight:700">Risk Monitor</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">Calculated from attendance, assessment marks, and missing submissions.</p></div>
            <div class="relative"><i data-lucide="search" class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"></i><input data-filter-input data-filter-target="[data-risk-row]" type="search" placeholder="Search student or risk" class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 md:w-72" style="font-size:12.5px" /></div>
        </div>
        <asp:Panel ID="emptyPanel" runat="server" Visible="false" CssClass="px-6 py-10 text-center text-slate-500" style="font-size:13px">No at-risk students found from the current records.</asp:Panel>
        <div class="overflow-x-auto">
            <table class="min-w-full">
                <thead class="bg-slate-50 text-left text-slate-500" style="font-size:11.5px;font-weight:700;letter-spacing:0.04em"><tr><th class="px-6 py-3">Student</th><th class="px-6 py-3">Course</th><th class="px-6 py-3">Attendance</th><th class="px-6 py-3">Current Mark</th><th class="px-6 py-3">Risk Reason</th><th class="px-6 py-3">Priority</th></tr></thead>
                <tbody class="divide-y divide-slate-100" style="font-size:13px">
                    <asp:Repeater ID="riskRepeater" runat="server">
                        <ItemTemplate>
                            <tr data-risk-row data-filter-text='<%# Html(Eval("StudentName")) %> <%# Html(Eval("CourseCode")) %> <%# Html(Eval("RiskReason")) %>'>
                                <td class="px-6 py-4"><div class="font-semibold text-slate-900"><%# Html(Eval("StudentName")) %></div><div class="text-slate-500"><%# Html(Eval("StudentNo")) %></div></td>
                                <td class="px-6 py-4 text-slate-700"><%# Html(Eval("CourseCode")) %><div class="text-slate-500"><%# Html(Eval("CourseName")) %></div></td>
                                <td class="px-6 py-4"><span class='rounded-full px-2 py-1 <%# RiskBadgeClass(Eval("AttendancePercent")) %>' style="font-size:12px;font-weight:700"><%# Percent(Eval("AttendancePercent")) %></span></td>
                                <td class="px-6 py-4"><span class='rounded-full px-2 py-1 <%# RiskBadgeClass(Eval("CurrentMark")) %>' style="font-size:12px;font-weight:700"><%# Percent(Eval("CurrentMark")) %></span></td>
                                <td class="px-6 py-4 text-slate-700"><%# Html(Eval("RiskReason")) %></td>
                                <td class="px-6 py-4"><button type="button" data-toast="Follow-up noted" class='h-8 rounded-md px-3 <%# LevelClass(Eval("RiskLevel")) %>' style="font-size:12.5px;font-weight:600"><%# Html(Eval("RiskLevel")) %></button></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </section>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/lecturer/lecturer-portal.js") %>"></script>
</asp:Content>
