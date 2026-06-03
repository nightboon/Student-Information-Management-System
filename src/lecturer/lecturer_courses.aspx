<%@ Page Language="C#" MasterPageFile="~/shared/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_courses.aspx.cs" Inherits="src.lecturer.lecturer_courses" Title="My Courses - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <%-- Header --%>
    <div class="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Lecturer Portal</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">My Courses</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">
                Courses you&#8217;re teaching this academic year. Click a course to manage students, attendance, grades, and materials.
            </p>
        </div>
        <div class="flex items-center gap-3">
            <div class="rounded-xl border border-slate-200 bg-white px-3 py-2">
                <div class="text-slate-500" style="font-size:11px;font-weight:500">Pinned</div>
                <div class="text-slate-900" style="font-size:16px;font-weight:700">
                    <span id="pinned-count">0</span><span class="text-slate-400"> / <%= CourseCount %></span>
                </div>
            </div>
        </div>
    </div>

    <%-- Filters --%>
    <div class="mt-6 flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
        <div class="flex flex-wrap gap-2" id="semester-filters">
            <button type="button" data-action="filter-semester" data-semester="all"
                class="rounded-full px-3.5 py-1.5 bg-slate-900 text-white transition-all"
                style="font-size:12.5px;font-weight:600">All semesters</button>
            <asp:Repeater ID="semesterRepeater" runat="server">
                <ItemTemplate>
                    <button type="button" data-action="filter-semester" data-semester='<%# Server.HtmlEncode(Container.DataItem.ToString()) %>'
                        class="rounded-full px-3.5 py-1.5 border border-slate-200 bg-white text-slate-600 hover:border-slate-300 hover:text-slate-900 transition-all"
                        style="font-size:12.5px;font-weight:600"><%# Server.HtmlEncode(Container.DataItem.ToString()) %></button>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div class="relative w-full lg:w-72">
            <i data-lucide="search" class="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400"></i>
            <input id="course-search" type="search" placeholder="Search by name or code&#8230;"
                class="h-10 w-full rounded-xl border border-slate-200 bg-white pl-10 pr-3 text-slate-900 placeholder:text-slate-400 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10"
                style="font-size:13px" />
        </div>
    </div>

    <%-- Course grid --%>
    <div class="mt-6 grid gap-4 sm:grid-cols-2 xl:grid-cols-3" id="course-grid">
        <asp:Repeater ID="coursesRepeater" runat="server">
            <ItemTemplate>
                <article data-course-code='<%# Server.HtmlEncode(Eval("CourseCode").ToString()) %>'
                    data-semester='<%# Server.HtmlEncode(Eval("SemesterName").ToString()) %>'
                    data-search='<%# Server.HtmlEncode(SearchKey(Eval("CourseCode").ToString(), Eval("CourseName").ToString())) %>'
                    class="group relative flex flex-col overflow-hidden rounded-2xl border border-slate-200 bg-white p-5 pt-6 hover:border-slate-300 hover:shadow-sm transition-all">
                    <span class="absolute inset-x-0 top-0 h-1" style='background-color:<%# AccentColor(Eval("Color") as string) %>'></span>
                    <div class="flex items-start justify-between">
                        <div class="flex h-10 w-10 items-center justify-center rounded-xl" style='background-color:<%# AccentColor(Eval("Color") as string) %>15;color:<%# AccentColor(Eval("Color") as string) %>'>
                            <i data-lucide="book-open" class="h-4 w-4"></i>
                        </div>
                        <button type="button" data-action="toggle-pin" aria-label="Toggle pin"
                            class="rounded-lg p-2 transition-all text-slate-400 hover:bg-slate-100 hover:text-slate-700">
                            <i data-lucide="pin" data-pinned-icon class="h-4 w-4"></i>
                        </button>
                    </div>
                    <div class="mt-4 flex items-center gap-2">
                        <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600"><%# Server.HtmlEncode(Eval("CourseCode").ToString()) %></span>
                        <span class='inline-flex items-center gap-1 rounded-md px-1.5 py-0.5 <%# StatusBadgeClass(Eval("Status").ToString()) %>' style="font-size:10.5px;font-weight:600">
                            <i data-lucide='<%# StatusIcon(Eval("Status").ToString()) %>' class="h-3 w-3"></i><%# Server.HtmlEncode(Eval("Status").ToString()) %>
                        </span>
                    </div>
                    <h3 class="mt-2 text-slate-900" style="font-size:15.5px;font-weight:600;line-height:1.3"><%# Server.HtmlEncode(Eval("CourseName").ToString()) %></h3>
                    <p class="mt-1.5 text-slate-500 line-clamp-2" style="font-size:12.5px;line-height:1.5"><%# Server.HtmlEncode(Eval("Description").ToString()) %></p>
                    <div class="mt-4 flex items-center justify-between text-slate-500" style="font-size:12px">
                        <span><%# Eval("EnrolledCount") %> students enrolled</span>
                        <span><%# Server.HtmlEncode(Eval("CreditHours").ToString()) %> credits</span>
                    </div>
                    <div class="mt-4 border-t border-slate-100 pt-3 flex items-center justify-between">
                        <span class="text-slate-500" style="font-size:11.5px;font-weight:500"><%# Server.HtmlEncode(Eval("SemesterName").ToString()) %></span>
                        <a href="#"
                            class="inline-flex items-center gap-1 text-[#e0162b] hover:text-[#a01020] transition-colors"
                            style="font-size:12.5px;font-weight:600">
                            Open course <i data-lucide="arrow-right" class="h-3.5 w-3.5"></i>
                        </a>
                    </div>
                </article>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <%-- Empty state (hidden by default; shown by JS when search yields no results) --%>
    <div id="no-results" class="hidden mt-10 rounded-2xl border border-dashed border-slate-300 bg-white p-12 text-center">
        <p class="text-slate-700" style="font-size:15px;font-weight:600">No courses match your filters</p>
        <p class="mt-1 text-slate-500" style="font-size:13px">Try a different semester or search term.</p>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/lecturer/courses/courses.js") %>"></script>
</asp:Content>
