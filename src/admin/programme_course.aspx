<%@ Page Language="C#" MasterPageFile="~/admin/AdminLayout.master" AutoEventWireup="true" CodeBehind="programme_course.aspx.cs" Inherits="src.admin.programme_course" Title="Programme & Course - INTI Admin Portal" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div data-tabs>

    <div class="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Admin</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">Programme &amp; Course</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">Manage programmes, courses, and lecturer course assignments.</p>
        </div>
        <div class="flex flex-wrap items-center gap-2">
            <span data-tab-panel="programmes"><button type="button" data-modal-open="prog-modal" class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]" style="font-size:13px;font-weight:600"><i data-lucide="plus" class="h-4 w-4"></i> New Programme</button></span>
            <span data-tab-panel="courses"><button type="button" data-modal-open="course-modal" class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]" style="font-size:13px;font-weight:600"><i data-lucide="plus" class="h-4 w-4"></i> New Course</button></span>
            <span data-tab-panel="assign"><button type="button" data-modal-open="assign-modal" class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]" style="font-size:13px;font-weight:600"><i data-lucide="plus" class="h-4 w-4"></i> New Assignment</button></span>
            <span data-tab-panel="departments"><button type="button" data-modal-open="department-modal" class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]" style="font-size:13px;font-weight:600"><i data-lucide="plus" class="h-4 w-4"></i> New Department</button></span>
        </div>
    </div>

    <section class="mt-6 rounded-lg border border-slate-200 bg-white">
        <div class="flex flex-wrap gap-1 border-b border-slate-100 px-4 pt-3">
            <button type="button" data-tab="programmes" class="inline-flex items-center gap-2 rounded-t-lg border-b-2 border-transparent px-3 py-2 text-slate-500 hover:text-slate-900 data-[active=true]:border-[#e0162b] data-[active=true]:text-[#a01020]" style="font-size:13px;font-weight:600">Programmes</button>
            <button type="button" data-tab="courses" class="inline-flex items-center gap-2 rounded-t-lg border-b-2 border-transparent px-3 py-2 text-slate-500 hover:text-slate-900 data-[active=true]:border-[#e0162b] data-[active=true]:text-[#a01020]" style="font-size:13px;font-weight:600">Courses</button>
            <button type="button" data-tab="assign" class="inline-flex items-center gap-2 rounded-t-lg border-b-2 border-transparent px-3 py-2 text-slate-500 hover:text-slate-900 data-[active=true]:border-[#e0162b] data-[active=true]:text-[#a01020]" style="font-size:13px;font-weight:600">Course Assignment</button>
            <button type="button" data-tab="departments" class="inline-flex items-center gap-2 rounded-t-lg border-b-2 border-transparent px-3 py-2 text-slate-500 hover:text-slate-900 data-[active=true]:border-[#e0162b] data-[active=true]:text-[#a01020]" style="font-size:13px;font-weight:600">Departments</button>
        </div>

        <%-- Programmes tab --%>
        <div data-tab-panel="programmes">
            <div data-table data-page-size="20">
                <div class="flex flex-col gap-3 px-6 py-4 lg:flex-row lg:items-center lg:justify-between">
                    <div class="relative w-full lg:max-w-sm">
                        <svg viewBox="0 0 24 24" class="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="7" /><path d="m21 21-4.3-4.3" /></svg>
                        <input data-table-search placeholder="Search&hellip;" class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px" />
                    </div>
                    <select data-table-filter="status" class="h-9 rounded-md border border-slate-200 bg-white px-3 text-slate-700 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px"><%= ProgrammeStatusFilterOptionsHtml %></select>
                </div>
                <div class="overflow-x-auto">
                    <table class="min-w-full">
                        <thead class="border-y border-slate-100 bg-slate-50/60 text-slate-500"><tr>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Code</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Name</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Level</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Duration</th>
                            <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Sems</th>
                            <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Courses</th>
                            <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Students</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Status</th>
                            <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Action</th>
                        </tr></thead>
                        <tbody>
                            <%= ProgrammeRowsHtml %>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

        <%-- Departments tab --%>
        <div data-tab-panel="departments">
            <div data-table data-page-size="20">
                <div class="px-6 py-4"><div class="relative w-full lg:max-w-sm"><i data-lucide="search" class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"></i><input data-table-search placeholder="Search departments..." class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px" /></div></div>
                <div class="overflow-x-auto"><table class="min-w-full">
                    <thead class="border-y border-slate-100 bg-slate-50/60 text-slate-500"><tr>
                        <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Code</th>
                        <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Department</th>
                        <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Programmes</th>
                        <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Lecturers</th>
                        <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Status</th>
                        <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Action</th>
                    </tr></thead>
                    <tbody><%= DepartmentRowsHtml %></tbody>
                </table></div>
            </div>
        </div>

        <%-- Courses tab --%>
        <div data-tab-panel="courses">
            <div data-table data-page-size="20">
                <div class="flex flex-col gap-3 px-6 py-4 lg:flex-row lg:items-center lg:justify-between">
                    <div class="relative w-full lg:max-w-sm">
                        <svg viewBox="0 0 24 24" class="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="7" /><path d="m21 21-4.3-4.3" /></svg>
                        <input data-table-search placeholder="Search&hellip;" class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px" />
                    </div>
                    <select data-table-filter="status" class="h-9 rounded-md border border-slate-200 bg-white px-3 text-slate-700 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px"><%= CourseStatusFilterOptionsHtml %></select>
                </div>
                <div class="overflow-x-auto">
                    <table class="min-w-full">
                        <thead class="border-y border-slate-100 bg-slate-50/60 text-slate-500"><tr>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Code</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Title</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Programme</th>
                            <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Credit</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Prerequisites</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Lecturer</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Status</th>
                            <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Action</th>
                        </tr></thead>
                        <tbody>
                            <%= CourseRowsHtml %>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

        <%-- Course Assignment tab --%>
        <div data-tab-panel="assign">
            <div data-table data-page-size="20">
                <div class="flex flex-col gap-3 px-6 py-4 lg:flex-row lg:items-center lg:justify-between">
                    <div class="relative w-full lg:max-w-sm">
                        <svg viewBox="0 0 24 24" class="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="7" /><path d="m21 21-4.3-4.3" /></svg>
                        <input data-table-search placeholder="Search&hellip;" class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px" />
                    </div>
                </div>
                <div class="overflow-x-auto">
                    <table class="min-w-full">
                        <thead class="border-y border-slate-100 bg-slate-50/60 text-slate-500"><tr>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Code</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Semester</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Programme</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Title</th>
                            <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Credit</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Lecturer</th>
                            <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Action</th>
                        </tr></thead>
                        <tbody>
                            <%= AssignmentRowsHtml %>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </section>

</div>

<div id="department-modal" data-modal class="fixed inset-0 z-[60] items-center justify-center p-4" style="display:none">
    <div data-modal-backdrop class="absolute inset-0 bg-slate-900/40 backdrop-blur-sm"></div>
    <div class="relative w-full max-w-xl overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-xl">
        <div class="flex items-center justify-between border-b border-slate-100 px-6 py-4"><h2 class="text-slate-900" style="font-size:17px;font-weight:700">Department Details</h2><button type="button" data-modal-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100"><i data-lucide="x" class="h-4 w-4"></i></button></div>
        <div class="grid gap-4 px-6 py-5">
            <label><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Department Code</span><input data-field="id" class="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></label>
            <label><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Department Name</span><input data-field="name" class="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></label>
            <label><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Status</span><select data-field="status" class="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><%= ProgrammeStatusOptionsHtml %></select></label>
        </div>
        <div class="flex justify-end gap-2 border-t border-slate-100 bg-slate-50/40 px-6 py-4"><button type="button" data-modal-close class="h-10 px-4 text-slate-600" style="font-size:13px;font-weight:600">Cancel</button><button type="button" data-admin-save-department class="h-10 rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:600">Save</button></div>
    </div>
</div>

<%-- Programme modal --%>
<div id="prog-modal" data-modal class="fixed inset-0 z-[60] items-center justify-center p-4" style="display:none">
    <div data-modal-backdrop class="absolute inset-0 bg-slate-900/40 backdrop-blur-sm"></div>
    <div class="relative w-full max-w-2xl max-h-[90vh] overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-xl flex flex-col">
        <div class="flex items-start justify-between gap-4 border-b border-slate-100 px-6 py-4"><div><h2 class="text-slate-900" style="font-size:17px;font-weight:700;letter-spacing:-0.01em">Programme Details</h2></div><button type="button" data-modal-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-700"><i data-lucide="x" class="h-4 w-4"></i></button></div>
        <div class="flex-1 overflow-y-auto px-6 py-5">
            <div style="display:grid;grid-template-columns:repeat(auto-fit,minmax(240px,1fr));gap:16px">
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Code</span><div class="mt-1.5"><input data-field="code" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Department</span><div class="mt-1.5"><select data-field="department" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><%= DepartmentOptionsHtml %></select></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Name</span><div class="mt-1.5"><input data-field="name" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Department</span><div class="mt-1.5"><select data-field="department" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><%= DepartmentOptionsHtml %></select></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Level</span><div class="mt-1.5"><select data-field="level" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><%= EducationLevelOptionsHtml %></select></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Duration</span><div class="mt-1.5"><input data-field="duration" value="3 yrs" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Total Semesters</span><div class="mt-1.5"><input data-field="semesters" type="number" value="6" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Status</span><div class="mt-1.5"><select data-field="status" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><%= ProgrammeStatusOptionsHtml %></select></div></label>
            </div>
            <div class="mt-5"><label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Description</span><div class="mt-1.5"><textarea placeholder="Programme overview, accreditation, career outcomes&hellip;" class="w-full min-h-[88px] rounded-md border border-slate-200 bg-white px-3 py-2 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px;line-height:1.5"></textarea></div></label></div>
        </div>
        <div class="flex items-center justify-end gap-2 border-t border-slate-100 px-6 py-4 bg-slate-50/40"><button type="button" data-modal-close class="inline-flex items-center rounded-md px-4 h-10 text-slate-600 hover:bg-slate-100" style="font-size:13px;font-weight:600">Cancel</button><button type="button" data-modal-close data-admin-save-programme data-toast="Programme saved" class="inline-flex items-center rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:600">Save</button></div>
    </div>
</div>

<%-- Course modal --%>
<div id="course-modal" data-modal class="fixed inset-0 z-[60] items-center justify-center p-4" style="display:none">
    <div data-modal-backdrop class="absolute inset-0 bg-slate-900/40 backdrop-blur-sm"></div>
    <div class="relative w-full max-w-3xl max-h-[90vh] overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-xl flex flex-col">
        <div class="flex items-start justify-between gap-4 border-b border-slate-100 px-6 py-4"><div><h2 class="text-slate-900" style="font-size:17px;font-weight:700;letter-spacing:-0.01em">Course Details</h2></div><button type="button" data-modal-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-700"><i data-lucide="x" class="h-4 w-4"></i></button></div>
        <div class="flex-1 overflow-y-auto px-6 py-5">
            <div style="display:grid;grid-template-columns:repeat(auto-fit,minmax(240px,1fr));gap:16px">
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Course Code</span><div class="mt-1.5"><input data-field="code" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Title</span><div class="mt-1.5"><input data-field="name" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Programme</span><div class="mt-1.5"><select data-field="programme" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><%= ProgrammeOptionsHtml %></select></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Credit Value</span><div class="mt-1.5"><input data-field="creditHours" type="number" value="3" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Lecturer</span><div class="mt-1.5"><select data-field="lecturer" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><%= LecturerOptionsHtml %></select></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Status</span><div class="mt-1.5"><select data-field="status" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><%= CourseStatusOptionsHtml %></select></div></label>
            </div>
            <div class="mt-5">
                <span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Prerequisites</span>
                <div class="relative mt-1.5" data-dropdown>
                    <button type="button" data-dropdown-toggle class="flex h-9 w-full items-center justify-between rounded-md border border-slate-200 bg-white px-3 text-left text-slate-400 hover:border-slate-300" style="font-size:12.5px"><span>Select prerequisites&hellip;</span><i data-lucide="chevron-down" class="h-4 w-4 text-slate-400"></i></button>
                    <div data-dropdown-menu style="display:none" class="absolute left-0 right-0 z-20 mt-1 rounded-md border border-slate-200 bg-white shadow-lg">
                        <div class="border-b border-slate-100 px-3 py-2 text-slate-500" style="font-size:11.5px">Students must pass these courses before enrolling.</div>
                        <ul class="max-h-56 overflow-y-auto py-1" id="prereq-list">
                            <%= PrerequisiteItemsHtml %>
                            <li id="prereq-empty" style="display:none" class="px-3 py-3 text-slate-400" style="font-size:12.5px">No courses available for this programme.</li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
        <div class="flex items-center justify-end gap-2 border-t border-slate-100 px-6 py-4 bg-slate-50/40"><button type="button" data-modal-close class="inline-flex items-center rounded-md px-4 h-10 text-slate-600 hover:bg-slate-100" style="font-size:13px;font-weight:600">Cancel</button><button type="button" data-modal-close data-admin-save-course data-toast="Course saved" class="inline-flex items-center rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:600">Save</button></div>
    </div>
</div>

<%-- Assignment modal --%>
<div id="assign-modal" data-modal class="fixed inset-0 z-[60] items-center justify-center p-4" style="display:none">
    <div data-modal-backdrop class="absolute inset-0 bg-slate-900/40 backdrop-blur-sm"></div>
    <div class="relative w-full max-w-2xl max-h-[90vh] overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-xl flex flex-col">
        <div class="flex items-start justify-between gap-4 border-b border-slate-100 px-6 py-4"><div><h2 class="text-slate-900" style="font-size:17px;font-weight:700;letter-spacing:-0.01em">Assignment Details</h2></div><button type="button" data-modal-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-700"><i data-lucide="x" class="h-4 w-4"></i></button></div>
        <div class="flex-1 overflow-y-auto px-6 py-5">
            <input type="hidden" data-field="offerId" />
            <div style="display:grid;grid-template-columns:repeat(auto-fit,minmax(240px,1fr));gap:16px">
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Programme</span><div class="mt-1.5"><select data-field="programme" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><%= ProgrammeOptionsHtml %></select></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Intake Semester</span><div class="mt-1.5"><select data-field="semester" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><%= SemesterOptionsHtml %></select></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Course</span><div class="mt-1.5"><select data-field="course" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><%= CourseOptionsHtml %></select></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Credit</span><div class="mt-1.5"><input data-field="credit" type="number" value="3" readonly class="h-10 w-full rounded-md border border-slate-200 bg-slate-50 px-3 outline-none text-slate-500" style="font-size:13px" /></div></label>
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Lecturer</span><div class="mt-1.5"><select data-field="lecturer" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><%= LecturerOptionsHtml %></select></div></label>
            </div>
        </div>
        <div class="flex items-center justify-end gap-2 border-t border-slate-100 px-6 py-4 bg-slate-50/40"><button type="button" data-modal-close class="inline-flex items-center rounded-md px-4 h-10 text-slate-600 hover:bg-slate-100" style="font-size:13px;font-weight:600">Cancel</button><button type="button" data-modal-close data-admin-save-assignment data-toast="Assignment saved" class="inline-flex items-center rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:600">Save</button></div>
    </div>
</div>

<%-- Programme view drawer --%>
<div id="view-prog" data-drawer class="fixed inset-0 z-[60]" style="display:none">
    <div data-drawer-backdrop class="absolute inset-0 bg-slate-900/40"></div>
    <div data-drawer-panel class="absolute right-0 top-0 h-full w-full max-w-xl bg-white shadow-2xl border-l border-slate-200 flex flex-col">
        <div class="flex items-start justify-between gap-4 border-b border-slate-100 px-6 py-4"><div><h2 class="text-slate-900" style="font-size:18px;font-weight:700;letter-spacing:-0.01em">Bachelor of Computer Science</h2><p class="mt-0.5 text-slate-500" style="font-size:13px">BCS</p></div><button type="button" data-drawer-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-700"><i data-lucide="x" class="h-4 w-4"></i></button></div>
        <div class="flex-1 overflow-y-auto px-6 py-5">
            <div class="grid gap-3">
                <div class="rounded-xl border border-slate-200 p-4"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">Level</div><div class="mt-1 text-slate-900" style="font-size:14px">Undergraduate</div></div>
                <div class="rounded-xl border border-slate-200 p-4"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">Duration</div><div class="mt-1 text-slate-900" style="font-size:14px">3 yrs</div></div>
                <div class="rounded-xl border border-slate-200 p-4"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">Total Semesters</div><div class="mt-1 text-slate-900" style="font-size:14px">6</div></div>
                <div class="rounded-xl border border-slate-200 p-4"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">Students Enrolled</div><div class="mt-1 text-slate-900" style="font-size:14px">612</div></div>
                <div class="rounded-xl border border-slate-200 p-4"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">Status</div><div class="mt-1 text-slate-900" style="font-size:14px">Active</div></div>
            </div>
        </div>
        <div class="flex items-center justify-end gap-2 border-t border-slate-100 px-6 py-4 bg-slate-50/40"><button type="button" data-modal-open="prog-modal" data-drawer-close class="inline-flex items-center rounded-md border border-slate-200 bg-white px-3 h-10 text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:600">Edit</button><button type="button" data-drawer-close class="inline-flex items-center rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:600">Close</button></div>
    </div>
</div>

</asp:Content>
<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/admin/shared/icons.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/toast.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/table.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/ui.js") %>"></script>
    <script>
        (function () {
            var tabKey = "admin.programme_course.activeTab";
            function currentTab() {
                var active = document.querySelector("[data-tab][data-active='true']");
                return active ? active.getAttribute("data-tab") : (sessionStorage.getItem(tabKey) || "programmes");
            }
            function restoreTab() {
                var tab = (location.hash || "").replace("#", "") || sessionStorage.getItem(tabKey);
                var button = tab && document.querySelector('[data-tab="' + tab + '"]');
                if (button) button.click();
            }
            function post(method, payload) {
                return fetch("programme_course.aspx/" + method, {
                    method: "POST",
                    headers: { "Content-Type": "application/json; charset=utf-8" },
                    credentials: "same-origin",
                    body: JSON.stringify(payload || {})
                }).then(function (r) {
                    return r.json().then(function (json) {
                        var data = json && json.d ? json.d : json;
                        if (!r.ok || (data && data.ok === false)) {
                            throw new Error((data && data.message) || "Request failed");
                        }
                        return data;
                    });
                });
            }
            function field(modal, name) {
                var el = modal.querySelector('[data-field="' + name + '"]');
                return el ? el.value.trim() : "";
            }
            // Returns true when every listed field has a value; otherwise flags the
            // first empty one and returns false so the caller can stop before posting.
            function requireFields(modal, specs) {
                for (var i = 0; i < specs.length; i++) {
                    var el = modal.querySelector('[data-field="' + specs[i].name + '"]');
                    var value = el ? String(el.value).trim() : "";
                    if (!value) {
                        if (window.toast) window.toast.error(specs[i].label + " is required");
                        if (el && el.focus) el.focus();
                        return false;
                    }
                }
                return true;
            }
            function setField(modal, name, value) {
                var el = modal.querySelector('[data-field="' + name + '"]');
                if (!el) return;
                if (el.tagName === "SELECT" && value) {
                    var exists = Array.prototype.some.call(el.options, function (option) { return option.value === value; });
                    if (!exists) el.add(new Option(value, value));
                }
                el.value = value || "";
            }
            function done(message) {
                if (window.toast) window.toast.success(message);
                sessionStorage.setItem(tabKey, currentTab());
                setTimeout(function () { location.reload(); }, 450);
            }
            function fail(error, fallback) {
                if (window.toast) window.toast.error(error && error.message ? error.message : fallback);
            }
            function filterPrerequisiteItems(modal, currentCode, checkedCodes) {
                var programme = (modal.querySelector('[data-field="programme"]') || {}).value || "";
                var list = document.getElementById("prereq-list");
                var emptyMsg = document.getElementById("prereq-empty");
                if (!list) return;
                var items = list.querySelectorAll("[data-prereq-item]");
                var visible = 0;
                Array.prototype.forEach.call(items, function (li) {
                    var itemCode = li.getAttribute("data-code");
                    var itemProgramme = li.getAttribute("data-programme");
                    var hide = (itemCode === currentCode) || (programme && itemProgramme && itemProgramme !== programme);
                    li.style.display = hide ? "none" : "";
                    if (!hide) visible++;
                    var cb = li.querySelector("[data-prereq-check]");
                    if (cb && checkedCodes !== undefined) {
                        cb.checked = checkedCodes.indexOf(itemCode) !== -1;
                    }
                });
                if (emptyMsg) emptyMsg.style.display = (visible === 0) ? "" : "none";
            }
            function collectPrerequisites(modal) {
                var checks = modal.querySelectorAll("[data-prereq-check]:checked");
                var codes = [];
                Array.prototype.forEach.call(checks, function (cb) { codes.push(cb.value); });
                return codes.join(",");
            }
            function filterCourseOptions(modal, keepValue) {
                var programmeSel = modal.querySelector('[data-field="programme"]');
                var courseSel = modal.querySelector('[data-field="course"]');
                if (!programmeSel || !courseSel) return;
                var programme = programmeSel.value;
                var previous = keepValue || courseSel.value;
                Array.prototype.forEach.call(courseSel.options, function (option) {
                    var optProgramme = option.getAttribute("data-programme");
                    option.hidden = !!(programme && optProgramme && optProgramme !== programme);
                });
                var stillValid = Array.prototype.some.call(courseSel.options, function (option) {
                    return option.value === previous && !option.hidden;
                });
                courseSel.value = stillValid ? previous : "";
                applyCourseCredit(modal);
            }
            function applyCourseCredit(modal) {
                var courseSel = modal.querySelector('[data-field="course"]');
                var creditField = modal.querySelector('[data-field="credit"]');
                if (!courseSel || !creditField) return;
                var selected = courseSel.options[courseSel.selectedIndex];
                creditField.value = (selected && selected.getAttribute("data-credit")) || "";
            }
            document.addEventListener("click", function (e) {
                var editDepartment = e.target.closest("[data-admin-edit-department]");
                if (editDepartment) {
                    var departmentRow = editDepartment.closest("tr");
                    var departmentModal = document.getElementById("department-modal");
                    setField(departmentModal, "id", departmentRow && departmentRow.dataset.id);
                    setField(departmentModal, "name", departmentRow && departmentRow.dataset.name);
                    setField(departmentModal, "status", departmentRow && departmentRow.dataset.status);
                    return;
                }

                var editProgramme = e.target.closest("[data-admin-edit-programme]");
                if (editProgramme) {
                    var programmeRow = editProgramme.closest("tr");
                    var programmeModal = document.getElementById("prog-modal");
                    setField(programmeModal, "code", programmeRow && programmeRow.dataset.code);
                    setField(programmeModal, "department", programmeRow && programmeRow.dataset.department);
                    setField(programmeModal, "name", programmeRow && programmeRow.dataset.name);
                    setField(programmeModal, "department", programmeRow && programmeRow.dataset.department);
                    setField(programmeModal, "level", programmeRow && programmeRow.dataset.level);
                    setField(programmeModal, "duration", programmeRow && programmeRow.dataset.duration);
                    setField(programmeModal, "semesters", programmeRow && programmeRow.dataset.semesters);
                    setField(programmeModal, "status", programmeRow && programmeRow.dataset.status);
                    return;
                }

                var editCourse = e.target.closest("[data-admin-edit-course]");
                if (editCourse) {
                    var courseRow = editCourse.closest("tr");
                    var courseModal = document.getElementById("course-modal");
                    setField(courseModal, "code", courseRow && courseRow.dataset.code);
                    setField(courseModal, "name", courseRow && courseRow.dataset.name);
                    setField(courseModal, "programme", courseRow && courseRow.dataset.programme);
                    setField(courseModal, "creditHours", courseRow && courseRow.dataset.creditHours);
                    setField(courseModal, "status", courseRow && courseRow.dataset.status);
                    var prereqs = (courseRow && courseRow.dataset.prerequisites) ? courseRow.dataset.prerequisites.split(",").map(function(s){return s.trim();}).filter(Boolean) : [];
                    filterPrerequisiteItems(courseModal, courseRow && courseRow.dataset.code, prereqs);
                    return;
                }

                var editAssignment = e.target.closest("[data-admin-edit-assignment]");
                if (editAssignment) {
                    var row = editAssignment.closest("tr");
                    var am = document.getElementById("assign-modal");
                    setField(am, "offerId", row && row.dataset.offerId);
                    setField(am, "programme", row && row.dataset.programme);
                    setField(am, "semester", row && row.dataset.sessionId);
                    setField(am, "lecturer", row && row.dataset.lecturer);
                    filterCourseOptions(am, row && row.dataset.courseCode);
                    setField(am, "course", row && row.dataset.courseCode);
                    applyCourseCredit(am);
                    return;
                }

                var newProgramme = e.target.closest("[data-modal-open='prog-modal']");
                if (newProgramme) {
                    var freshProgramme = document.getElementById("prog-modal");
                    setField(freshProgramme, "code", "");
                    setField(freshProgramme, "department", "");
                    setField(freshProgramme, "name", "");
                    setField(freshProgramme, "department", "");
                    setField(freshProgramme, "level", "Undergraduate");
                    setField(freshProgramme, "duration", "3 yrs");
                    setField(freshProgramme, "semesters", "6");
                    setField(freshProgramme, "status", "Active");
                }

                var newDepartment = e.target.closest("[data-modal-open='department-modal']");
                if (newDepartment) {
                    var freshDepartment = document.getElementById("department-modal");
                    setField(freshDepartment, "id", "");
                    setField(freshDepartment, "name", "");
                    setField(freshDepartment, "status", "Active");
                }

                var newCourse = e.target.closest("[data-modal-open='course-modal']");
                if (newCourse) {
                    var freshCourse = document.getElementById("course-modal");
                    setField(freshCourse, "code", "");
                    setField(freshCourse, "name", "");
                    setField(freshCourse, "creditHours", "3");
                    setField(freshCourse, "status", "Active");
                    filterPrerequisiteItems(freshCourse, "", []);
                }

                var newAssignment = e.target.closest("[data-modal-open='assign-modal']");
                if (newAssignment) {
                    var freshAssignment = document.getElementById("assign-modal");
                    setField(freshAssignment, "offerId", "");
                    setField(freshAssignment, "programme", "");
                    setField(freshAssignment, "course", "");
                    setField(freshAssignment, "credit", "");
                    setField(freshAssignment, "lecturer", "");
                    filterCourseOptions(freshAssignment, "");
                }

                var programme = e.target.closest("[data-admin-save-programme]");
                if (programme) {
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    var pm = document.getElementById("prog-modal");
                    if (!requireFields(pm, [
                        { name: "code", label: "Programme code" },
                        { name: "name", label: "Programme name" },
                        { name: "department", label: "Department" }
                    ])) return;
                    post("SaveProgramme", {
                        request: {
                            code: field(pm, "code"),
                            department: field(pm, "department"),
                            name: field(pm, "name"),
                            department: field(pm, "department"),
                            level: field(pm, "level"),
                            duration: field(pm, "duration"),
                            semesters: parseInt(field(pm, "semesters"), 10) || 1,
                            status: field(pm, "status")
                        }
                    }).then(function () { done("Programme saved"); })
                        .catch(function (error) { fail(error, "Could not save programme"); });
                    return;
                }

                var department = e.target.closest("[data-admin-save-department]");
                if (department) {
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    var dm = document.getElementById("department-modal");
                    if (!requireFields(dm, [
                        { name: "id", label: "Department code" },
                        { name: "name", label: "Department name" }
                    ])) return;
                    post("SaveDepartment", { request: { id: field(dm, "id"), name: field(dm, "name"), status: field(dm, "status") } })
                        .then(function () { done("Department saved"); })
                        .catch(function (error) { fail(error, "Could not save department"); });
                    return;
                }

          var course = e.target.closest("[data-admin-save-course]");
          if (course) {
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    var cm = document.getElementById("course-modal");
                    if (!requireFields(cm, [
                        { name: "code", label: "Course code" },
                        { name: "name", label: "Course title" }
                    ])) return;
                    post("SaveCourse", {
                        request: {
                            code: field(cm, "code"),
                            name: field(cm, "name"),
                            programme: field(cm, "programme"),
                            creditHours: parseInt(field(cm, "creditHours"), 10) || 1,
                            prerequisites: collectPrerequisites(cm),
                            status: field(cm, "status")
                        }
                    }).then(function () { done("Course saved"); })
              .catch(function (error) { fail(error, "Could not save course"); });
            return;
          }

          var assignment = e.target.closest("[data-admin-save-assignment]");
          if (assignment) {
            e.preventDefault();
            e.stopImmediatePropagation();
            var am = document.getElementById("assign-modal");
            if (!requireFields(am, [
                { name: "course", label: "Course" },
                { name: "lecturer", label: "Lecturer" },
                { name: "semester", label: "Intake semester" }
            ])) return;
            post("SaveCourseAssignment", {
              request: {
                offerId: parseInt(field(am, "offerId"), 10) || 0,
                courseCode: field(am, "course"),
                lecturer: field(am, "lecturer"),
                sessionId: field(am, "semester"),
                status: "Active"
              }
            }).then(function () { done("Assignment saved"); })
              .catch(function (error) { fail(error, "Could not save assignment"); });
            return;
          }

          var deleteProgramme = e.target.closest("[data-admin-delete-programme]");
          if (deleteProgramme) {
            e.preventDefault();
            e.stopImmediatePropagation();
            if (!confirm("Delete this programme? Existing linked records will be marked inactive instead.")) return;
            post("DeleteProgramme", { code: deleteProgramme.getAttribute("data-code") })
              .then(function () { done("Programme deleted"); })
              .catch(function (error) { fail(error, "Could not delete programme"); });
            return;
          }

          var deleteDepartment = e.target.closest("[data-admin-delete-department]");
          if (deleteDepartment) {
            e.preventDefault();
            e.stopImmediatePropagation();
            if (!confirm("Delete this department? Linked records will be marked inactive instead.")) return;
            post("DeleteDepartment", { id: deleteDepartment.getAttribute("data-id") })
              .then(function () { done("Department deleted"); })
              .catch(function (error) { fail(error, "Could not delete department"); });
            return;
          }

          var deleteCourse = e.target.closest("[data-admin-delete-course]");
          if (deleteCourse) {
            e.preventDefault();
            e.stopImmediatePropagation();
            if (!confirm("Delete this course? Existing linked records will be marked inactive instead.")) return;
            post("DeleteCourse", { code: deleteCourse.getAttribute("data-code") })
              .then(function () { done("Course deleted"); })
              .catch(function (error) { fail(error, "Could not delete course"); });
            return;
          }

          var deleteAssignment = e.target.closest("[data-admin-delete-assignment]");
          if (deleteAssignment) {
            e.preventDefault();
            e.stopImmediatePropagation();
            if (!confirm("Delete this assignment? Existing linked records will be marked inactive instead.")) return;
            post("DeleteCourseAssignment", { offerId: parseInt(deleteAssignment.getAttribute("data-offer-id"), 10) })
              .then(function () { done("Assignment deleted"); })
              .catch(function (error) { fail(error, "Could not delete assignment"); });
          }
        }, true);
        document.addEventListener("click", function (e) {
          var tab = e.target.closest("[data-tab]");
          if (tab) sessionStorage.setItem(tabKey, tab.getAttribute("data-tab"));
        });
        document.addEventListener("change", function (e) {
          var am = document.getElementById("assign-modal");
          if (am && am.contains(e.target)) {
            if (e.target.matches('[data-field="programme"]')) filterCourseOptions(am);
            else if (e.target.matches('[data-field="course"]')) applyCourseCredit(am);
          }
          var cm = document.getElementById("course-modal");
          if (cm && cm.contains(e.target) && e.target.matches('[data-field="programme"]')) {
            var currentCode = (cm.querySelector('[data-field="code"]') || {}).value || "";
            filterPrerequisiteItems(cm, currentCode);
          }
        });
        setTimeout(restoreTab, 0);
      })();
    </script>
</asp:Content>


