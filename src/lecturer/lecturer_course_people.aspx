<%@ Page Language="C#" MasterPageFile="~/lecturer/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_course_people.aspx.cs" Inherits="src.lecturer.lecturer_course_people" Title="Course People - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <a href="<%= BackUrl %>" class="inline-flex items-center gap-1.5 text-slate-500 hover:text-slate-900 transition-colors" style="font-size:13px;font-weight:500">
        <i data-lucide="arrow-left" class="h-3.5 w-3.5"></i> Back to course dashboard
    </a>

    <div class="mt-4 flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500"><%= CourseCode %></p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700">People</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">Students enrolled in <%= CourseName %>.</p>
        </div>
        <div class="rounded-md border border-slate-200 bg-white px-4 py-2 text-slate-700" style="font-size:13px;font-weight:600"><%= EnrolledLabel %></div>
    </div>

    <section class="mt-6 rounded-lg border border-slate-200 bg-white">
        <div class="flex flex-col gap-3 border-b border-slate-100 px-6 py-4 md:flex-row md:items-center md:justify-between">
            <div>
                <h2 class="text-slate-900" style="font-size:16px;font-weight:700">Enrolled students</h2>
                <p class="mt-0.5 text-slate-500" style="font-size:12.5px">Loaded from ENROLMENTS for this course offering.</p>
            </div>
            <div class="relative">
                <i data-lucide="search" class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"></i>
                <input data-filter-input data-filter-target="[data-student-row]" type="search" placeholder="Search student" class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 md:w-72" style="font-size:12.5px" />
            </div>
        </div>

        <asp:Panel ID="emptyPanel" runat="server" Visible="false" CssClass="px-6 py-10 text-center text-slate-500" style="font-size:13px">
            No enrolled students found for this course.
        </asp:Panel>

        <div class="overflow-x-auto">
            <table class="min-w-full">
                <thead class="bg-slate-50 text-left text-slate-500" style="font-size:11.5px;font-weight:700;letter-spacing:0.04em">
                    <tr>
                        <th class="px-6 py-3">Student ID</th>
                        <th class="px-6 py-3">Name</th>
                        <th class="px-6 py-3">Email</th>
                        <th class="px-6 py-3">Programme</th>
                        <th class="px-6 py-3">Phone</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-slate-100" style="font-size:13px">
                    <asp:Repeater ID="studentsRepeater" runat="server">
                        <ItemTemplate>
                            <tr data-student-row data-filter-text='<%# Html(Eval("StudentNo")) %> <%# Html(Eval("FullName")) %> <%# Html(Eval("Email")) %> <%# Html(Eval("ProgrammeCode")) %> <%# Html(Eval("ProgrammeName")) %> <%# Html(Eval("Phone")) %>'>
                                <td class="px-6 py-4 text-slate-700"><%# Html(Eval("StudentNo")) %></td>
                                <td class="px-6 py-4 font-semibold text-slate-900"><%# Html(Eval("FullName")) %></td>
                                <td class="px-6 py-4 text-slate-700"><%# Html(Eval("Email")) %></td>
                                <td class="px-6 py-4 text-slate-700">
                                    <span title='<%# Html(Eval("ProgrammeName")) %>'><%# Html(Eval("ProgrammeCode")) %></span>
                                </td>
                                <td class="px-6 py-4 text-slate-700"><%# Html(Eval("Phone")) %></td>
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
