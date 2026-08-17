<%@ Page Language="C#" MasterPageFile="~/lecturer/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_materials.aspx.cs" Inherits="student_information_management_system.lecturer_materials" Title="Course Materials - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="courseModulesPanel" runat="server" Visible="false">
        <div id="course-materials-root" style="--course-accent:<%= CourseAccentColor %>;--course-accent-soft:<%= CourseAccentColor %>15">
            <a href="<%= CourseDashboardUrl %>" class="inline-flex items-center gap-1.5 text-slate-500 hover:text-slate-900 transition-colors" style="font-size:13px;font-weight:500">
                <i data-lucide="arrow-left" class="h-3.5 w-3.5"></i> Back to course dashboard
            </a>

            <nav class="mt-5 border-b border-slate-200">
                <div class="flex gap-1">
                    <button type="button" data-course-material-tab="modules" data-active="<%= ShowAssignmentsTab ? "false" : "true" %>" class="relative inline-flex items-center gap-2 px-4 py-3 <%= ShowAssignmentsTab ? "text-slate-500 hover:text-slate-900" : "text-slate-900" %> transition-colors" style="font-size:13.5px;font-weight:600">
                        <i data-lucide="book-open" class="h-4 w-4"></i>Modules
                        <span data-course-material-indicator class="absolute inset-x-2 -bottom-px <%= ShowAssignmentsTab ? "hidden " : "" %>h-0.5 rounded-full" style="background-color:var(--course-accent)"></span>
                    </button>
                    <button type="button" data-course-material-tab="assignments" data-active="<%= ShowAssignmentsTab ? "true" : "false" %>" class="relative inline-flex items-center gap-2 px-4 py-3 <%= ShowAssignmentsTab ? "text-slate-900" : "text-slate-500 hover:text-slate-900" %> transition-colors" style="font-size:13.5px;font-weight:600">
                        <i data-lucide="clipboard-list" class="h-4 w-4"></i>Assignments
                        <span data-course-material-indicator class="absolute inset-x-2 -bottom-px <%= ShowAssignmentsTab ? "" : "hidden " %>h-0.5 rounded-full" style="background-color:var(--course-accent)"></span>
                    </button>
                </div>
            </nav>

            <asp:Panel ID="courseModuleStatusPanel" runat="server" Visible="false" CssClass="mt-4 rounded-md border px-4 py-3" style="font-size:13px;font-weight:600">
                <asp:Literal ID="courseModuleStatusMessage" runat="server" />
            </asp:Panel>

            <section data-course-material-pane="modules" class="mt-6 <%= ShowAssignmentsTab ? "hidden " : "" %>overflow-hidden rounded-2xl border border-slate-200 bg-white">
                <header class="p-6 pb-4">
                    <h1 class="text-slate-900" style="font-size:16px;font-weight:600">Course Modules</h1>
                    <p class="mt-0.5 text-slate-500" style="font-size:13px"><%= CourseModuleCount %> weekly modules</p>
                </header>
                <ul class="divide-y divide-slate-100">
                    <asp:Repeater ID="courseModulesRepeater" runat="server" OnItemCommand="CourseModulesRepeater_ItemCommand">
                        <ItemTemplate>
                            <li>
                                <div class="flex items-center pr-4 transition-colors hover:bg-slate-50/60">
                                    <button type="button" data-course-module-toggle class="flex min-w-0 flex-1 items-center gap-4 px-6 py-4 pr-2 text-left">
                                        <span class="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg" style="background-color:var(--course-accent-soft);color:var(--course-accent);font-size:12px;font-weight:700"><%# Eval("Week") %></span>
                                        <div class="min-w-0 flex-1">
                                            <p class="truncate text-slate-900" style="font-size:14px;font-weight:600"><%# Server.HtmlEncode(Eval("Title").ToString()) %></p>
                                            <p class="mt-0.5 truncate text-slate-500" style="font-size:12.5px"><%# Server.HtmlEncode(Eval("Description").ToString()) %></p>
                                        </div>
                                        <span class="text-slate-400" style="font-size:11.5px;font-weight:600"><%# ((System.Collections.ICollection)Eval("Items")).Count %> items</span>
                                        <i data-lucide="chevron-right" class="h-4 w-4 text-slate-300 transition-transform" data-module-chevron></i>
                                    </button>
                                    <button type="button" data-module-edit-toggle class="inline-flex h-8 shrink-0 items-center gap-1.5 rounded-md border border-slate-200 bg-white px-3 text-slate-600 hover:border-slate-300 hover:text-slate-900" style="font-size:12px;font-weight:600">
                                        <i data-lucide="pencil" class="h-3.5 w-3.5"></i>Edit
                                    </button>
                                </div>
                                <div data-module-edit-form class="hidden border-t border-slate-100 bg-slate-50/70 px-6 py-4">
                                    <div class="grid gap-3 lg:grid-cols-[minmax(220px,1fr)_minmax(320px,2fr)_auto] lg:items-end">
                                        <label class="block">
                                            <span class="text-slate-500" style="font-size:11.5px;font-weight:700">WEEK TITLE</span>
                                            <asp:TextBox ID="moduleTitleInput" runat="server" Text='<%# Eval("Title") %>' MaxLength="100" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-800" style="font-size:13px" />
                                        </label>
                                        <label class="block">
                                            <span class="text-slate-500" style="font-size:11.5px;font-weight:700">DESCRIPTION</span>
                                            <asp:TextBox ID="moduleDescriptionInput" runat="server" Text='<%# Eval("Description") %>' MaxLength="255" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-800" style="font-size:13px" />
                                        </label>
                                        <div class="flex gap-2">
                                            <asp:LinkButton runat="server" CommandName="SaveModule" CommandArgument='<%# Eval("ModuleId") %>' CausesValidation="false" CssClass="inline-flex h-10 items-center justify-center rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020]" style="font-size:12.5px;font-weight:700">Save changes</asp:LinkButton>
                                            <button type="button" data-module-edit-cancel class="inline-flex h-10 items-center justify-center rounded-md border border-slate-200 bg-white px-4 text-slate-600 hover:bg-slate-50" style="font-size:12.5px;font-weight:600">Cancel</button>
                                        </div>
                                    </div>
                                </div>
                                <ul data-course-module-items class="hidden bg-slate-50/50 px-6 pb-4">
                                    <asp:Repeater runat="server" DataSource='<%# Eval("Items") %>'>
                                        <ItemTemplate>
                                            <li data-material-url='<%# MaterialPreviewUrl(Eval("MaterialId")) %>' class="ml-12 cursor-pointer rounded-xl transition-colors hover:bg-white">
                                                <div class="flex items-center gap-3 px-3 py-2.5">
                                                    <span class="flex h-8 w-8 items-center justify-center rounded-lg bg-slate-100 text-slate-600"><i data-lucide='<%# CourseFileIcon(Eval("FileType").ToString()) %>' class="h-4 w-4"></i></span>
                                                    <div class="min-w-0 flex-1">
                                                        <p class="truncate text-slate-900" style="font-size:13px;font-weight:500"><%# Server.HtmlEncode(Eval("Title").ToString()) %></p>
                                                        <p class="text-slate-400" style="font-size:11px"><%# CourseFileSize(Eval("FileSizeBytes")) %></p>
                                                    </div>
                                                    <span class="rounded-lg p-1.5 text-slate-400"><i data-lucide="eye" class="h-4 w-4"></i></span>
                                                </div>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <asp:Panel ID="courseModulesEmptyPanel" runat="server" Visible="false" CssClass="border-t border-slate-100 px-6 py-10 text-center text-slate-500" style="font-size:13px">No weekly lecture notes yet.</asp:Panel>
            </section>

            <section data-course-material-pane="assignments" class="mt-6 <%= ShowAssignmentsTab ? "" : "hidden " %>overflow-hidden rounded-2xl border border-slate-200 bg-white">
                <header class="p-6 pb-4">
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Assignments</h2>
                    <p class="mt-0.5 text-slate-500" style="font-size:13px"><%= CourseAssignmentCount %> published assessments</p>
                </header>
                <ul class="divide-y divide-slate-100">
                    <asp:Repeater ID="courseAssignmentsRepeater" runat="server">
                        <ItemTemplate>
                            <li class="flex items-center gap-2 px-4 transition-colors hover:bg-slate-50/60 sm:px-6">
                                <a href='<%# MaterialPreviewUrl(Eval("MaterialId")) %>' class="flex min-w-0 flex-1 items-center gap-4 py-4">
                                    <span class="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl" style="background-color:var(--course-accent-soft);color:var(--course-accent)"><i data-lucide='<%# MaterialIcon(Eval("MaterialType")) %>' class="h-5 w-5"></i></span>
                                    <div class="min-w-0 flex-1">
                                        <div class="flex items-center gap-2">
                                            <p class="truncate text-slate-900" style="font-size:14px;font-weight:600"><%# Html(Eval("Title")) %></p>
                                            <span class="rounded bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:700"><%# Html(Eval("MaterialType")) %></span>
                                        </div>
                                        <p class="mt-1 flex flex-wrap items-center gap-x-3 gap-y-1 text-slate-500" style="font-size:12px">
                                            <span>Due <%# DueDateLabel(Eval("DueDate")) %></span>
                                            <span class="font-semibold text-slate-600">Course weight: <span data-material-weight-display='<%# Eval("MaterialId") %>'><%# CourseWeightLabel(Eval("Weight")) %></span></span>
                                        </p>
                                    </div>
                                    <i data-lucide="eye" class="h-4 w-4 text-slate-400"></i>
                                </a>
                                <button type="button" data-edit-material-weight='<%# Eval("MaterialId") %>'
                                    data-material-title='<%# Html(Eval("Title")) %>' data-current-weight='<%# WeightInputValue(Eval("Weight")) %>'
                                    class="inline-flex h-9 w-9 shrink-0 items-center justify-center rounded-md text-slate-500 hover:bg-white hover:text-[#a01020]"
                                    title="Edit course weight" aria-label="Edit course weight">
                                    <i data-lucide="pencil" class="h-4 w-4"></i>
                                </button>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <asp:Panel ID="courseAssignmentsEmptyPanel" runat="server" Visible="false" CssClass="border-t border-slate-100 px-6 py-10 text-center text-slate-500" style="font-size:13px">No assessments yet.</asp:Panel>
            </section>
        </div>
    </asp:Panel>

    <asp:Panel ID="materialsManagerPanel" runat="server">
    <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Lecturer Module</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700">Upload Materials</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">Organise assignments, lecture notes, quizzes, and tests for your students.</p>
        </div>
    </div>

    <asp:Panel ID="statusBanner" runat="server" Visible="false" CssClass="mt-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800" style="font-size:13px;font-weight:600">
        <asp:Literal ID="statusMessage" runat="server" />
    </asp:Panel>
    <div data-material-client-status class="mt-4 hidden rounded-md border px-4 py-3" style="font-size:13px;font-weight:600"></div>

    <section class="mt-6 grid items-stretch gap-6 lg:grid-cols-[1fr_340px]">
        <div data-published-resources-card class="flex min-h-0 min-w-0 flex-col overflow-hidden rounded-lg border border-slate-200 bg-white">
            <div class="flex flex-col gap-3 border-b border-slate-100 px-6 py-4 xl:flex-row xl:items-center xl:justify-between">
                <div>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:700">Published Resources</h2>
                    <p class="mt-0.5 text-slate-500" style="font-size:12.5px">Visible material records by assigned course.</p>
                </div>
                <div class="flex flex-col gap-2 sm:flex-row">
                    <asp:DropDownList ID="yearFilterSelect" runat="server" data-material-year-filter="true" CssClass="h-9 rounded-md border border-slate-200 bg-white px-3 text-slate-700 sm:w-36" style="font-size:12.5px" />
                    <asp:DropDownList ID="semesterFilterSelect" runat="server" data-material-semester-filter="true" CssClass="h-9 rounded-md border border-slate-200 bg-white px-3 text-slate-700 sm:w-36" style="font-size:12.5px" />
                    <asp:DropDownList ID="courseFilterSelect" runat="server" data-material-course-filter="true" CssClass="h-9 rounded-md border border-slate-200 bg-white px-3 text-slate-700 sm:w-44" style="font-size:12.5px" />
                    <div class="relative">
                        <i data-lucide="search" class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"></i>
                        <input data-filter-input data-filter-target="[data-material]" type="search" placeholder="Search materials" class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 xl:w-64" style="font-size:12.5px" />
                    </div>
                </div>
            </div>

            <div class="material-tabs flex flex-wrap gap-2 border-b border-slate-100 px-6 py-3" data-material-tabs>
                <button type="button" data-material-tab="all" data-active="true" class="material-tab inline-flex h-9 items-center gap-1.5 rounded-lg border px-3 transition-all duration-200" style="font-size:12px;font-weight:700">All <span class="material-tab-count rounded-md px-1.5"><%= CountByType(null) %></span></button>
                <button type="button" data-material-tab="Assignment" data-active="false" class="material-tab inline-flex h-9 items-center gap-1.5 rounded-lg border px-3 transition-all duration-200" style="font-size:12px;font-weight:700"><i data-lucide="clipboard-check" class="h-3.5 w-3.5"></i>Assignment <span class="material-tab-count rounded-md px-1.5"><%= CountByType("Assignment") %></span></button>
                <button type="button" data-material-tab="Lecture Notes" data-active="false" class="material-tab inline-flex h-9 items-center gap-1.5 rounded-lg border px-3 transition-all duration-200" style="font-size:12px;font-weight:700"><i data-lucide="book-open" class="h-3.5 w-3.5"></i>Lecture Notes <span class="material-tab-count rounded-md px-1.5"><%= CountByType("Lecture Notes") %></span></button>
                <button type="button" data-material-tab="Quiz" data-active="false" class="material-tab inline-flex h-9 items-center gap-1.5 rounded-lg border px-3 transition-all duration-200" style="font-size:12px;font-weight:700"><i data-lucide="circle-help" class="h-3.5 w-3.5"></i>Quiz <span class="material-tab-count rounded-md px-1.5"><%= CountByType("Quiz") %></span></button>
                <button type="button" data-material-tab="Test" data-active="false" class="material-tab inline-flex h-9 items-center gap-1.5 rounded-lg border px-3 transition-all duration-200" style="font-size:12px;font-weight:700"><i data-lucide="clipboard-list" class="h-3.5 w-3.5"></i>Test <span class="material-tab-count rounded-md px-1.5"><%= CountByType("Test") %></span></button>
                <button type="button" data-material-tab="Viva" data-active="false" class="material-tab inline-flex h-9 items-center gap-1.5 rounded-lg border px-3 transition-all duration-200" style="font-size:12px;font-weight:700"><i data-lucide="presentation" class="h-3.5 w-3.5"></i>Viva <span class="material-tab-count rounded-md px-1.5"><%= CountByType("Viva") %></span></button>
            </div>

            <asp:Panel ID="emptyPanel" runat="server" Visible="false" CssClass="px-6 py-10 text-center text-slate-500" style="font-size:13px">No materials have been published yet.</asp:Panel>
            <ul data-material-list class="min-h-0 flex-1 overflow-y-auto divide-y divide-slate-100">
                <asp:Repeater ID="materialsRepeater" runat="server" OnItemCommand="MaterialsRepeater_ItemCommand">
                    <ItemTemplate>
                        <li data-material data-material-id='<%# Eval("MaterialId") %>' data-material-url='<%# MaterialPreviewUrl(Eval("MaterialId")) %>' data-material-category='<%# Html(Eval("MaterialType")) %>' data-material-offering='<%# Eval("OfferingId") %>' data-material-year='<%# Html(Eval("AcademicYear")) %>' data-material-semester='<%# Html(Eval("Semester")) %>' data-filter-text='<%# Html(Eval("Title")) %> <%# Html(Eval("CourseCode")) %> <%# Html(Eval("CourseName")) %> <%# Html(Eval("MaterialType")) %>' class="flex cursor-pointer flex-col gap-3 px-6 py-4 hover:bg-slate-50/70 md:flex-row md:items-center md:justify-between">
                            <div class="flex min-w-0 items-start gap-3">
                                <span class="flex h-10 w-10 shrink-0 items-center justify-center rounded-md bg-[#e0162b]/10 text-[#e0162b]"><i data-lucide='<%# MaterialIcon(Eval("MaterialType")) %>' class="h-5 w-5"></i></span>
                                <div class="min-w-0">
                                    <div class="flex flex-wrap items-center gap-2">
                                        <p class="truncate text-slate-900" style="font-size:14px;font-weight:700"><%# Html(Eval("Title")) %></p>
                                        <span class='rounded px-2 py-0.5 <%# TypeBadgeClass(Eval("MaterialType")) %>' style="font-size:11px;font-weight:800"><%# Html(Eval("MaterialType")) %></span>
                                        <%# WeekLabel(Eval("MaterialType"), Eval("Week")) %>
                                    </div>
                                    <p class="mt-0.5 flex flex-wrap gap-x-3 gap-y-0.5 text-slate-500" style="font-size:12.5px">
                                        <span><%# Html(Eval("CourseCode")) %></span>
                                        <span>Due <%# DueDateLabel(Eval("DueDate")) %></span>
                                        <span>Uploaded <%# UploadedLabel(Eval("UploadedAt")) %></span>
                                        <span><%# FileMeta(Eval("FileType"), Eval("FileSizeBytes")) %></span>
                                    </p>
                                    <p class="mt-1 text-slate-500" style="font-size:12.5px"><%# Html(Eval("Description")) %></p>
                                </div>
                            </div>
                            <div class="flex shrink-0 items-center gap-4 md:justify-end">
                                <div class="text-right">
                                    <p class="text-slate-400" style="font-size:10.5px;font-weight:800;letter-spacing:0.08em">COURSE WEIGHT</p>
                                    <p class="text-slate-900" style="font-size:14px;font-weight:800"><span data-material-weight-display='<%# Eval("MaterialId") %>'><%# WeightLabel(Eval("Weight")) %></span></p>
                                </div>
                                <button runat="server" Visible='<%# Eval("Weight") != null && Eval("Weight") != DBNull.Value %>'
                                    type="button" data-edit-material-weight='<%# Eval("MaterialId") %>'
                                    data-material-title='<%# Html(Eval("Title")) %>' data-current-weight='<%# WeightInputValue(Eval("Weight")) %>'
                                    class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-[#a01020]"
                                    title="Edit course weight" aria-label="Edit course weight">
                                    <i data-lucide="pencil" class="h-4 w-4"></i>
                                </button>
                                <button type="button" data-delete-material='<%# Eval("MaterialId") %>' class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-red-50 hover:text-[#e0162b]" title="Delete material"><i data-lucide="trash-2" class="h-4 w-4"></i></button>
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>

        <aside data-upload-details-card class="self-start rounded-lg border border-slate-200 bg-white">
            <div class="border-b border-slate-100 px-6 py-4">
                <h2 class="text-slate-900" style="font-size:16px;font-weight:700">Upload Details</h2>
                <p class="mt-0.5 text-slate-500" style="font-size:12.5px">Publish a new course material or assessment.</p>
            </div>
            <div class="space-y-4 px-6 py-5">
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">ACADEMIC YEAR <span class="text-[#e0162b]">*</span></span><asp:DropDownList ID="uploadYearSelect" runat="server" data-upload-year-select="true" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3" style="font-size:13px" /></label>
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">SEMESTER <span class="text-[#e0162b]">*</span></span><asp:DropDownList ID="uploadSemesterSelect" runat="server" data-upload-semester-select="true" disabled="disabled" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 disabled:cursor-not-allowed disabled:bg-slate-100 disabled:text-slate-400" style="font-size:13px" /></label>
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">COURSE <span class="text-[#e0162b]">*</span></span><asp:DropDownList ID="courseSelect" runat="server" data-material-course-select="true" disabled="disabled" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 disabled:cursor-not-allowed disabled:bg-slate-100 disabled:text-slate-400" style="font-size:13px" /></label>
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">TYPE</span><asp:DropDownList ID="materialTypeSelect" runat="server" data-material-type-select="true" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3" style="font-size:13px" /></label>
                <label data-assessment-mode-field class="hidden"><span class="text-slate-500" style="font-size:12px;font-weight:700">VIVA METHOD <span class="text-[#e0162b]">*</span></span><select name="assessmentMode" data-assessment-mode class="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3" style="font-size:13px"><option value="" disabled selected hidden>Choose a viva method</option><option value="LINK">Google Drive Link</option><option value="MANUAL">Live Presentation</option></select></label>
                <label data-week-field class="hidden"><span class="text-slate-500" style="font-size:12px;font-weight:700">WEEK <span class="text-[#e0162b]">*</span></span><asp:DropDownList ID="weekSelect" runat="server" data-material-week-select="true" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3" style="font-size:13px" /></label>
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">TITLE <span class="text-[#e0162b]">*</span></span><asp:TextBox ID="titleInput" runat="server" data-material-title="true" required="required" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" placeholder="Material title" style="font-size:13px" /></label>
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">DESCRIPTION <span data-description-required class="hidden text-[#e0162b]">*</span></span><asp:TextBox ID="descriptionInput" runat="server" data-material-description="true" TextMode="MultiLine" CssClass="mt-1.5 min-h-[88px] w-full rounded-md border border-slate-200 px-3 py-2" placeholder="Short student-facing description" style="font-size:13px" /></label>
                <label data-due-date-field class="block transition-opacity"><span class="text-slate-500" style="font-size:12px;font-weight:700">DUE DATE <span data-due-date-required class="hidden text-[#e0162b]">*</span></span><asp:TextBox ID="dueDateInput" runat="server" data-material-due-date="true" TextMode="Date" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3 disabled:cursor-not-allowed disabled:bg-slate-100 disabled:text-slate-400" /></label>
                <div class="grid grid-cols-2 gap-2">
                    <label data-due-time-field class="block transition-opacity"><span class="text-slate-500" style="font-size:12px;font-weight:700">DUE TIME <span data-due-time-required class="hidden text-[#e0162b]">*</span></span><asp:TextBox ID="dueTimeInput" runat="server" data-material-due-time="true" TextMode="Time" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3 disabled:cursor-not-allowed disabled:bg-slate-100 disabled:text-slate-400" /></label>
                    <label data-weight-field class="block transition-opacity"><span class="text-slate-500" style="font-size:12px;font-weight:700">WEIGHT % <span data-weight-required class="hidden text-[#e0162b]">*</span></span><asp:TextBox ID="weightInput" runat="server" data-material-weight="true" TextMode="Number" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3 disabled:cursor-not-allowed disabled:bg-slate-100 disabled:text-slate-400" /></label>
                </div>
                <label data-file-field class="block transition-opacity"><span class="text-slate-500" style="font-size:12px;font-weight:700">FILE <span data-file-required class="text-[#e0162b]">*</span></span><asp:FileUpload ID="materialFileInput" runat="server" data-material-file="true" CssClass="mt-1.5 block w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-slate-600 disabled:cursor-not-allowed disabled:bg-slate-100 disabled:text-slate-400" /></label>
                <asp:LinkButton ID="publishButton" runat="server" data-publish-material="true" OnClick="PublishMaterial_Click" CssClass="inline-flex h-10 w-full items-center justify-center gap-1.5 rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:700"><i data-lucide="upload" class="h-4 w-4"></i><span data-publish-label>Publish material</span></asp:LinkButton>
            </div>
        </aside>
    </section>
    </asp:Panel>

    <div data-weight-update-status class="fixed right-5 top-5 z-[90] hidden max-w-sm rounded-lg border px-4 py-3 shadow-xl" style="font-size:13px;font-weight:700"></div>
    <dialog data-weight-editor class="m-auto w-[min(92vw,430px)] rounded-xl border border-slate-200 bg-white p-0 shadow-2xl">
        <div class="flex items-start justify-between border-b border-slate-100 px-5 py-4">
            <div>
                <h2 class="text-slate-900" style="font-size:17px;font-weight:800">Edit course weight</h2>
                <p data-weight-editor-title class="mt-1 text-slate-500" style="font-size:12.5px"></p>
            </div>
            <button type="button" data-weight-editor-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-500 hover:bg-slate-100" aria-label="Close">
                <i data-lucide="x" class="h-4 w-4"></i>
            </button>
        </div>
        <div class="px-5 py-5">
            <label class="block">
                <span class="text-slate-600" style="font-size:12px;font-weight:800">COURSE WEIGHT %</span>
                <input data-weight-editor-input type="number" min="0.01" max="100" step="0.01"
                    class="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3 text-slate-800"
                    style="font-size:13px" />
            </label>
            <p class="mt-2 text-slate-500" style="font-size:11.5px">Changing weight does not remove the assessment, files, marks, or student submissions.</p>
            <p data-weight-editor-error class="mt-3 hidden rounded-md border border-amber-200 bg-amber-50 px-3 py-2 text-amber-800" style="font-size:12px;font-weight:600"></p>
            <div class="mt-5 flex justify-end gap-2">
                <button type="button" data-weight-editor-close class="inline-flex h-9 items-center justify-center rounded-md border border-slate-200 px-4 text-slate-700 hover:bg-slate-50" style="font-size:12.5px;font-weight:700">Cancel</button>
                <button type="button" data-save-material-weight class="inline-flex h-9 items-center justify-center rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020]" style="font-size:12.5px;font-weight:700">Save weight</button>
            </div>
        </div>
    </dialog>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeadPlaceholder" runat="server">
    <style>
        .material-tab {
            border-color: #e2e8f0;
            background: #fff;
            color: #64748b;
        }
        select[data-assessment-mode]:required:invalid {
            color: #94a3b8;
        }
        select[data-assessment-mode] option {
            color: #334155;
        }
        [data-weight-editor]::backdrop {
            background: rgba(15, 23, 42, .48);
            backdrop-filter: blur(1px);
        }
        .material-tab:hover {
            border-color: #e2e8f0;
            background: #f8fafc;
            color: #0f172a;
            box-shadow: 0 4px 12px -8px rgba(15, 23, 42, .35);
            transform: translateY(-1px);
        }
        .material-tab[data-active="true"] {
            border-color: rgba(224, 22, 43, .14);
            background: rgba(224, 22, 43, .09);
            color: #a01020;
            box-shadow: 0 5px 14px -10px rgba(224, 22, 43, .7);
        }
        .material-tab-count {
            background: #f1f5f9;
            color: #64748b;
        }
        .material-tab[data-active="true"] .material-tab-count {
            background: #fff;
            color: #a01020;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/lecturer/lecturer-portal.js") %>?v=24"></script>
    <script src="<%= ResolveUrl("~/js/lecturer/course-dashboard.js") %>?v=2"></script>
    <script>
        document.addEventListener("click", function (event) {
            var tab = event.target.closest("[data-course-material-tab]");
            if (!tab) return;
            var selected = tab.getAttribute("data-course-material-tab");
            document.querySelectorAll("[data-course-material-tab]").forEach(function (button) {
                var active = button === tab;
                button.setAttribute("data-active", active ? "true" : "false");
                button.classList.toggle("text-slate-900", active);
                button.classList.toggle("text-slate-500", !active);
                var indicator = button.querySelector("[data-course-material-indicator]");
                if (indicator) indicator.classList.toggle("hidden", !active);
            });
            document.querySelectorAll("[data-course-material-pane]").forEach(function (pane) {
                pane.classList.toggle("hidden", pane.getAttribute("data-course-material-pane") !== selected);
            });
        });
    </script>
</asp:Content>
