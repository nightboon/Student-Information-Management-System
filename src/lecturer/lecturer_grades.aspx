<%@ Page Language="C#" MasterPageFile="~/shared/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_grades.aspx.cs" Inherits="student_information_management_system.lecturer_grades" Title="Marks and Grades - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Lecturer Module</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700">Marks &amp; Grades</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">Enter assessment marks, save drafts, and publish final course grades.</p>
        </div>
        <div class="flex flex-col gap-2 sm:flex-row">
            <asp:DropDownList ID="assessmentSelect" runat="server" AutoPostBack="true" OnSelectedIndexChanged="AssessmentChanged"
                CssClass="h-10 rounded-md border border-slate-200 bg-white px-3 text-slate-700" style="font-size:13px;font-weight:600" />
            <asp:LinkButton ID="saveButton" runat="server" OnClick="SaveDraft_Click"
                CssClass="inline-flex h-10 items-center justify-center gap-1.5 rounded-md border border-slate-200 bg-white px-4 text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:600">
                <i data-lucide="save" class="h-4 w-4"></i>Save draft
            </asp:LinkButton>
            <asp:LinkButton ID="publishButton" runat="server" OnClick="Publish_Click"
                CssClass="inline-flex h-10 items-center justify-center gap-1.5 rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:600">
                <i data-lucide="send" class="h-4 w-4"></i>Publish
            </asp:LinkButton>
        </div>
    </div>

    <asp:Panel ID="statusBanner" runat="server" Visible="false" CssClass="mt-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800" style="font-size:13px;font-weight:600">
        <asp:Literal ID="statusMessage" runat="server" />
    </asp:Panel>

    <section class="mt-6 grid gap-4 md:grid-cols-4">
        <div class="rounded-lg border border-slate-200 bg-white p-5"><p class="text-slate-500" style="font-size:12.5px">Students</p><p class="mt-1 text-slate-900" style="font-size:28px;font-weight:700"><%= StudentCount %></p></div>
        <div class="rounded-lg border border-slate-200 bg-white p-5"><p class="text-slate-500" style="font-size:12.5px">Marked</p><p class="mt-1 text-emerald-700" style="font-size:28px;font-weight:700"><%= MarksDisplay %></p></div>
        <div class="rounded-lg border border-slate-200 bg-white p-5"><p class="text-slate-500" style="font-size:12.5px">Pending</p><p class="mt-1 text-amber-700" style="font-size:28px;font-weight:700"><%= PendingCount %></p></div>
        <div class="rounded-lg border border-slate-200 bg-white p-5"><p class="text-slate-500" style="font-size:12.5px">Average</p><p class="mt-1 text-slate-900" style="font-size:28px;font-weight:700"><%= AverageDisplay %></p></div>
    </section>

    <section class="mt-6 rounded-lg border border-slate-200 bg-white">
        <div class="flex flex-col gap-3 border-b border-slate-100 px-6 py-4 md:flex-row md:items-center md:justify-between">
            <div><h2 class="text-slate-900" style="font-size:16px;font-weight:700">Assessment Mark Entry</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">Rows are loaded from students enrolled in this course offering.</p></div>
            <div class="relative"><i data-lucide="search" class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"></i><input data-filter-input data-filter-target="[data-grade-row]" type="search" placeholder="Search student" class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 md:w-64" style="font-size:12.5px" /></div>
        </div>
        <asp:Panel ID="emptyPanel" runat="server" Visible="false" CssClass="px-6 py-10 text-center text-slate-500" style="font-size:13px">No students found for this assessment.</asp:Panel>
        <div class="overflow-x-auto">
            <table class="min-w-full">
                <thead class="bg-slate-50 text-left text-slate-500" style="font-size:11.5px;font-weight:700;letter-spacing:0.04em"><tr><th class="px-6 py-3">Student</th><th class="px-6 py-3">Email</th><th class="px-6 py-3">Marks / 100</th><th class="px-6 py-3">Grade</th><th class="px-6 py-3">Course Progress</th><th class="px-6 py-3">Status</th></tr></thead>
                <tbody class="divide-y divide-slate-100" style="font-size:13px">
                    <asp:Repeater ID="gradeRepeater" runat="server">
                        <ItemTemplate>
                            <tr data-grade-row data-filter-text='<%# Html(Eval("StudentName")) %> <%# Html(Eval("StudentNo")) %>'>
                                <td class="px-6 py-4"><asp:HiddenField ID="studentId" runat="server" Value='<%# Eval("StudentId") %>' /><div class="font-semibold text-slate-900"><%# Html(Eval("StudentName")) %></div><div class="text-slate-500"><%# Html(Eval("StudentNo")) %></div></td>
                                <td class="px-6 py-4 text-slate-600"><%# Html(Eval("StudentEmail")) %></td>
                                <td class="px-6 py-4"><asp:TextBox ID="marksInput" runat="server" Text='<%# MarksValue(Eval("Marks")) %>' TextMode="Number" CssClass="h-9 w-24 rounded-md border border-slate-200 px-3" /></td>
                                <td class="px-6 py-4"><span class='rounded-full px-2 py-1 <%# GradeBadgeClass(Eval("LetterGrade")) %>' style="font-size:12px;font-weight:700"><%# Html(Eval("LetterGrade")) %></span></td>
                                <td class="px-6 py-4 text-slate-700"><%# Eval("CurrentAverage") %>%</td>
                                <td class="px-6 py-4"><span class='<%# Convert.ToBoolean(Eval("HasMarks")) ? "text-emerald-700" : "text-amber-700" %> font-semibold'><%# Convert.ToBoolean(Eval("HasMarks")) ? "Ready" : "Draft" %></span></td>
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
