<%@ Page Language="C#" MasterPageFile="~/shared/DashboardLayout.master"  AutoEventWireup="true" CodeBehind="course_detail.aspx.cs" Inherits="src.student.course_detail" %>


<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <div id="course-detail-root"
        style="--course-accent:<%= AccentColor(Header.Color) %>;--course-accent-soft:<%= AccentColor(Header.Color) %>15;--course-accent-dark:color-mix(in srgb,var(--course-accent) 72%,#000)">

    <%-- Breadcrumb / back --%>
    <a href="<%= ResolveUrl("~/student/courses.aspx") %>" class="inline-flex items-center gap-1.5 text-slate-500 hover:text-slate-900 transition-colors" style="font-size:13px;font-weight:500">
        <i data-lucide="arrow-left" class="h-3.5 w-3.5"></i> Back to My Courses
    </a>

    <%-- Course header (DB-driven) --%>
    <section class="relative mt-4 overflow-hidden rounded-3xl p-7 lg:p-9 text-white" style="background:linear-gradient(135deg,var(--course-accent) 0%,#1e293b 100%)">
        <div class="pointer-events-none absolute -top-16 -right-10 h-64 w-64 rounded-full bg-white/10 blur-3xl"></div>
        <div class="relative flex flex-col gap-5 lg:flex-row lg:items-end lg:justify-between">
            <div class="max-w-2xl">
                <div class="flex items-center gap-2">
                    <span class="rounded-md bg-white/15 px-2 py-0.5 backdrop-blur" style="font-size:11px;font-weight:600;letter-spacing:0.04em"><%= Server.HtmlEncode(Header.CourseCode) %></span>
                    <span class="text-white/70" style="font-size:12px"><%= Server.HtmlEncode(Header.LevelLabel) %></span>
                </div>
                <h1 class="mt-3 text-white" style="font-size:30px;font-weight:700;letter-spacing:-0.015em;line-height:1.15">
                    <%= Server.HtmlEncode(Header.CourseName) %>
                </h1>
                <p class="mt-2 text-white/80 max-w-xl" style="font-size:14.5px;line-height:1.6">
                    <%= Server.HtmlEncode(Header.Description) %>
                </p>
                <div class="mt-4 flex flex-wrap gap-x-5 gap-y-1 text-white/80" style="font-size:13px">
                    <span>&#128100; <%= Server.HtmlEncode(Header.LecturerName) %></span>
                    <span>&#127891; <%= Header.CreditHours %> credits</span>
                    <span>&#128218; <%= Header.ModuleCount %> modules</span>
                </div>
            </div>
            <div class="shrink-0">
                <button type="button" data-action="toggle-pin" data-code="<%= Server.HtmlEncode(Header.CourseCode) %>"
                    class="inline-flex items-center gap-2 rounded-2xl border border-white/25 bg-white/10 px-5 py-2.5 text-white backdrop-blur hover:bg-white/20 transition-colors"
                    style="font-size:13px;font-weight:600" aria-label="Toggle pin">
                    <i data-lucide="pin" class="h-4 w-4"></i>
                    Pin course
                </button>
            </div>
        </div>
    </section>

    <%-- Tab navigation --%>
    <nav class="mt-6 border-b border-slate-200">
        <ul class="flex gap-1 overflow-x-auto">
            <li>
                <button type="button" data-action="switch-tab" data-tab="modules" data-active="true"
                    class="relative inline-flex items-center gap-2 px-4 py-3 transition-colors data-[active=false]:text-slate-500 data-[active=false]:hover:text-slate-900"
                    style="font-size:13.5px;font-weight:600">
                    <i data-lucide="book-open" class="h-4 w-4"></i>
                    Modules
                    <span class="tab-indicator absolute inset-x-2 -bottom-px h-0.5 rounded-full"
                        style="background-color:var(--course-accent)"></span>
                </button>
            </li>
            <li>
                <button type="button" data-action="switch-tab" data-tab="announcements" data-active="false"
                    class="relative inline-flex items-center gap-2 px-4 py-3 transition-colors data-[active=false]:text-slate-500 data-[active=false]:hover:text-slate-900"
                    style="font-size:13.5px;font-weight:600">
                    <i data-lucide="megaphone" class="h-4 w-4"></i>
                    Announcements
                    <span class="tab-indicator absolute inset-x-2 -bottom-px h-0.5 rounded-full hidden"
                        style="background-color:var(--course-accent)"></span>
                </button>
            </li>
            <li>
                <button type="button" data-action="switch-tab" data-tab="materials" data-active="false"
                    class="relative inline-flex items-center gap-2 px-4 py-3 transition-colors data-[active=false]:text-slate-500 data-[active=false]:hover:text-slate-900"
                    style="font-size:13.5px;font-weight:600">
                    <i data-lucide="folder-open" class="h-4 w-4"></i>
                    Materials
                    <span class="tab-indicator absolute inset-x-2 -bottom-px h-0.5 rounded-full hidden"
                        style="background-color:var(--course-accent)"></span>
                </button>
            </li>
            <li>
                <button type="button" data-action="switch-tab" data-tab="assignments" data-active="false"
                    class="relative inline-flex items-center gap-2 px-4 py-3 transition-colors data-[active=false]:text-slate-500 data-[active=false]:hover:text-slate-900"
                    style="font-size:13.5px;font-weight:600">
                    <i data-lucide="clipboard-list" class="h-4 w-4"></i>
                    Assignments
                    <span class="tab-indicator absolute inset-x-2 -bottom-px h-0.5 rounded-full hidden"
                        style="background-color:var(--course-accent)"></span>
                </button>
            </li>
            <li>
                <button type="button" data-action="switch-tab" data-tab="grades" data-active="false"
                    class="relative inline-flex items-center gap-2 px-4 py-3 transition-colors data-[active=false]:text-slate-500 data-[active=false]:hover:text-slate-900"
                    style="font-size:13.5px;font-weight:600">
                    <i data-lucide="graduation-cap" class="h-4 w-4"></i>
                    Grades
                    <span class="tab-indicator absolute inset-x-2 -bottom-px h-0.5 rounded-full hidden"
                        style="background-color:var(--course-accent)"></span>
                </button>
            </li>
        </ul>
    </nav>

    <%-- Tab content --%>
    <section class="mt-6">

        <%-- ==================== MODULES PANE ==================== --%>
        <div data-pane="modules">
            <div class="grid gap-6 lg:grid-cols-3">

                <%-- Module accordion --%>
                <div class="lg:col-span-2 rounded-2xl border border-slate-200 bg-white">
                    <header class="flex items-center justify-between p-6 pb-4">
                        <div>
                            <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Course Modules</h2>
                            <p class="text-slate-500 mt-0.5" style="font-size:13px"><%= Header.ModuleCount %> weekly modules</p>
                        </div>
                    </header>
                    <ul class="divide-y divide-slate-100" id="module-accordion">
                        <asp:Repeater ID="modulesRepeater" runat="server">
                            <ItemTemplate>
                                <li>
                                    <button type="button" data-action="toggle-module" data-week='<%# Eval("Week") %>'
                                        class="flex w-full items-center gap-4 px-6 py-4 hover:bg-slate-50/60 transition-colors text-left">
                                        <span class="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg" style="background-color:var(--course-accent-soft);color:var(--course-accent);font-size:12px;font-weight:700"><%# Eval("Week") %></span>
                                        <div class="min-w-0 flex-1">
                                            <p class="text-slate-900 truncate" style="font-size:14px;font-weight:600"><%# Server.HtmlEncode(Eval("Title").ToString()) %></p>
                                            <p class="text-slate-500 mt-0.5 truncate" style="font-size:12.5px"><%# Server.HtmlEncode(Eval("Description").ToString()) %></p>
                                        </div>
                                        <span class="text-slate-400" style="font-size:11.5px;font-weight:600"><%# ((System.Collections.ICollection)Eval("Items")).Count %> items</span>
                                        <i data-lucide="chevron-right" class="h-4 w-4 text-slate-300 transition-transform module-chevron"></i>
                                    </button>
                                    <ul class="bg-slate-50/50 px-6 pb-4 module-items hidden" data-week='<%# Eval("Week") %>'>
                                        <asp:Repeater runat="server" DataSource='<%# Eval("Items") %>'>
                                            <ItemTemplate>
                                                <li class="ml-12 flex items-center gap-3 rounded-xl px-3 py-2.5 hover:bg-white transition-colors">
                                                    <span class="flex h-8 w-8 items-center justify-center rounded-lg bg-slate-100 text-slate-600">
                                                        <i data-lucide='<%# FileIcon(Eval("FileType").ToString()) %>' class="h-4 w-4"></i>
                                                    </span>
                                                    <div class="min-w-0 flex-1">
                                                        <p class="text-slate-900 truncate" style="font-size:13px;font-weight:500"><%# Server.HtmlEncode(Eval("Title").ToString()) %></p>
                                                        <p class="text-slate-400" style="font-size:11px"><%# FileSize(Eval("FileSizeBytes")) %></p>
                                                    </div>
                                                    <button type="button" class="rounded-lg p-1.5 text-slate-400 hover:bg-slate-100 hover:text-slate-700 transition-colors" aria-label="Download">
                                                        <i data-lucide="download" class="h-4 w-4"></i>
                                                    </button>
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>

                <%-- Course details sidebar --%>
                <aside class="rounded-2xl border border-slate-200 bg-white p-6">
                    <h3 class="text-slate-900" style="font-size:15px;font-weight:600">Course Details</h3>
                    <dl class="mt-4 space-y-3">
                        <div class="flex justify-between gap-4">
                            <dt class="text-slate-500" style="font-size:12.5px">Mode</dt>
                            <dd class="text-slate-900 text-right" style="font-size:12.5px;font-weight:500"><%= Server.HtmlEncode(Header.Mode) %></dd>
                        </div>
                        <div class="flex justify-between gap-4">
                            <dt class="text-slate-500" style="font-size:12.5px">Contact hours</dt>
                            <dd class="text-slate-900 text-right" style="font-size:12.5px;font-weight:500"><%= Server.HtmlEncode(Header.ContactHours) %></dd>
                        </div>
                        <div class="flex justify-between gap-4">
                            <dt class="text-slate-500" style="font-size:12.5px">Prerequisites</dt>
                            <dd class="text-slate-900 text-right" style="font-size:12.5px;font-weight:500"><%= Server.HtmlEncode(Header.Prerequisites) %></dd>
                        </div>
                        <div class="flex justify-between gap-4">
                            <dt class="text-slate-500" style="font-size:12.5px">Textbook</dt>
                            <dd class="text-slate-900 text-right" style="font-size:12.5px;font-weight:500"><%= Server.HtmlEncode(Header.Textbook) %></dd>
                        </div>
                        <div class="flex justify-between gap-4">
                            <dt class="text-slate-500" style="font-size:12.5px">Office hours</dt>
                            <dd class="text-slate-900 text-right" style="font-size:12.5px;font-weight:500"><%= Server.HtmlEncode(Header.OfficeHours) %></dd>
                        </div>
                    </dl>
                    <div class="mt-5 rounded-xl bg-slate-50 p-4">
                        <p class="text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">LEARNING OUTCOMES</p>
                        <ul class="mt-2 space-y-2 text-slate-700" style="font-size:12.5px;line-height:1.55">
                            <asp:Repeater ID="outcomesRepeater" runat="server">
                                <ItemTemplate>
                                    <li>&bull; <%# Server.HtmlEncode(Eval("Text").ToString()) %></li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>
                </aside>

            </div>
        </div>

        <%-- ==================== MATERIALS PANE ==================== --%>
        <div data-pane="materials" class="hidden">
            <div class="rounded-2xl border border-slate-200 bg-white">
                <header class="flex items-center justify-between p-6 pb-4">
                    <div>
                        <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Course Materials</h2>
                        <p class="text-slate-500 mt-0.5" style="font-size:13px"><%= StudentMaterialCount %> uploaded resources</p>
                    </div>
                </header>
                <asp:Panel ID="studentMaterialsEmptyPanel" runat="server" Visible="false" CssClass="px-6 pb-6">
                    <div class="rounded-xl border border-dashed border-slate-200 bg-slate-50 px-5 py-6 text-center text-slate-500" style="font-size:13px">
                        No materials have been uploaded for this course yet.
                    </div>
                </asp:Panel>
                <div class="divide-y divide-slate-100">
                    <asp:Repeater ID="studentMaterialsRepeater" runat="server">
                        <ItemTemplate>
                            <a href='<%# MaterialPreviewUrl(Eval("MaterialId")) %>' class="group flex flex-col gap-3 px-6 py-5 transition-colors hover:bg-slate-50/70 sm:flex-row sm:items-center">
                                <span class="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl" style="background-color:var(--course-accent-soft);color:var(--course-accent)">
                                    <i data-lucide='<%# MaterialIcon(Eval("MaterialType")) %>' class="h-5 w-5"></i>
                                </span>
                                <div class="min-w-0 flex-1">
                                    <div class="flex flex-wrap items-center gap-2">
                                        <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:700"><%# Server.HtmlEncode(Eval("CourseCode").ToString()) %></span>
                                        <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600"><%# Server.HtmlEncode(Eval("MaterialType").ToString()) %></span>
                                    </div>
                                    <h3 class="mt-1 text-slate-900 group-hover:text-slate-950" style="font-size:14px;font-weight:650"><%# Server.HtmlEncode(Eval("Title").ToString()) %></h3>
                                    <p class="mt-1 text-slate-500" style="font-size:12.5px"><%# Server.HtmlEncode(Eval("Description").ToString()) %></p>
                                    <p class="mt-2 flex flex-wrap gap-x-4 gap-y-1 text-slate-500" style="font-size:11.5px">
                                        <span><%# MaterialDueLabel(Eval("DueDate")) %></span>
                                        <span><%# MaterialWeightLabel(Eval("Weight")) %></span>
                                        <span><%# Server.HtmlEncode(Eval("FileType").ToString().ToUpperInvariant()) %><%# string.IsNullOrWhiteSpace(FileSize(Eval("FileSizeBytes"))) ? "" : " · " + FileSize(Eval("FileSizeBytes")) %></span>
                                        <span>Uploaded <%# ((System.DateTime)Eval("UploadedAt")).ToString("d MMM yyyy") %></span>
                                    </p>
                                </div>
                                <i data-lucide="chevron-right" class="h-4 w-4 shrink-0 text-slate-300 transition-transform group-hover:translate-x-0.5 group-hover:text-slate-500"></i>
                            </a>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>

        <%-- ==================== ANNOUNCEMENTS PANE ==================== --%>
        <div data-pane="announcements" class="hidden">
            <div class="rounded-2xl border border-slate-200 bg-white">
                <header class="flex items-center justify-between p-6 pb-4">
                    <div>
                        <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Announcements</h2>
                        <p class="text-slate-500 mt-0.5" style="font-size:13px">Messages from your lecturer</p>
                    </div>
                </header>
                <ul class="divide-y divide-slate-100">
                    <asp:Repeater ID="announcementsRepeater" runat="server">
                        <ItemTemplate>
                            <li>
                                <div class="flex w-full items-start gap-3 px-6 py-5">
                                    <div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-full text-white" style="background-color:var(--course-accent);font-size:12px;font-weight:600"><%# Server.HtmlEncode(Initials(Eval("AuthorName").ToString())) %></div>
                                    <div class="min-w-0 flex-1">
                                        <div class="flex items-center gap-2 flex-wrap">
                                            <span class="text-slate-900" style="font-size:13.5px;font-weight:600"><%# Server.HtmlEncode(Eval("AuthorName").ToString()) %></span>
                                            <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600"><%# Server.HtmlEncode(Eval("AuthorRole").ToString()) %></span>
                                            <span class="text-slate-400" style="font-size:11.5px">&middot; <%# ((System.DateTime)Eval("CreatedAt")).ToString("d MMM · HH:mm") %></span>
                                            <%# (bool)Eval("IsPinned") ? "<span class=\"inline-flex items-center gap-1 rounded-md px-1.5 py-0.5\" style=\"background-color:var(--course-accent-soft);color:var(--course-accent-dark);font-size:10.5px;font-weight:600\">Pinned</span>" : "" %>
                                        </div>
                                        <p class="mt-1.5 text-slate-900" style="font-size:14px;font-weight:600"><%# Server.HtmlEncode(Eval("Title").ToString()) %></p>
                                        <p class="mt-1 text-slate-600" style="font-size:13px;line-height:1.6"><%# Server.HtmlEncode(Eval("Content").ToString()) %></p>
                                        <%# (bool)Eval("HasAttachment") ? "<p class=\"mt-2 inline-flex items-center gap-1 text-slate-500\" style=\"font-size:11.5px\">1 attachment</p>" : "" %>
                                    </div>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </div>

        <%-- ==================== ASSIGNMENTS PANE ==================== --%>
        <div data-pane="assignments" class="hidden">
            <asp:Panel ID="assignmentStatusPanel" runat="server" Visible="false" CssClass="mb-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800" style="font-size:13px;font-weight:600">
                <asp:Literal ID="assignmentStatusMessage" runat="server" />
            </asp:Panel>
            <div class="grid gap-3">
                <asp:Repeater ID="assignmentsRepeater" runat="server" OnItemCommand="assignmentsRepeater_ItemCommand">
                    <ItemTemplate>
                        <div class="group flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-5 text-left sm:flex-row sm:items-center">
                            <span class="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl" style="background-color:var(--course-accent-soft);color:var(--course-accent)">
                                <i data-lucide="clipboard-list" class="h-5 w-5"></i>
                            </span>
                            <div class="min-w-0 flex-1">
                                <div class="flex items-center gap-2 flex-wrap">
                                    <h3 class="text-slate-900" style="font-size:14.5px;font-weight:600"><%# Server.HtmlEncode(Eval("Title").ToString()) %></h3>
                                    <%# Eval("Weight") != null ? "<span class=\"rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600\" style=\"font-size:10.5px;font-weight:600\">" + System.Convert.ToDecimal(Eval("Weight")).ToString("0.#") + "%</span>" : "" %>
                                    <%# !string.IsNullOrEmpty(System.Convert.ToString(Eval("AssignmentType")) + System.Convert.ToString(Eval("GroupSize"))) ? "<span class=\"rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600\" style=\"font-size:10.5px;font-weight:600\">" + Server.HtmlEncode(System.Convert.ToString(Eval("AssignmentType")) + (string.IsNullOrEmpty(System.Convert.ToString(Eval("GroupSize"))) ? "" : " (" + System.Convert.ToString(Eval("GroupSize")) + ")")) + "</span>" : "" %>
                                </div>
                                <p class="mt-1 text-slate-500" style="font-size:12.5px"><%# Server.HtmlEncode(Eval("Description").ToString()) %></p>
                                <p class="mt-1 inline-flex items-center gap-1 text-slate-500" style="font-size:12px">
                                    <span><%# Server.HtmlEncode(DueLabel(Eval("SubmissionStatus").ToString(), (System.DateTime)Eval("DueDate"))) %></span>
                                </p>
                                <%# (bool)Eval("HasSubmission") ? "<a class=\"mt-2 inline-flex items-center gap-1 text-[#e0162b] hover:text-[#a01020]\" style=\"font-size:12px;font-weight:600\" href=\"" + ResolveUrl(Eval("SubmissionFileUrl").ToString()) + "\" target=\"_blank\" rel=\"noopener\"><i data-lucide=\"paperclip\" class=\"h-3.5 w-3.5\"></i>Submitted file</a>" : "" %>
                                <%# !string.IsNullOrWhiteSpace(System.Convert.ToString(Eval("Feedback"))) ? "<p class=\"mt-2 rounded-lg bg-slate-50 px-3 py-2 text-slate-600\" style=\"font-size:12px\"><span class=\"font-semibold text-slate-700\">Feedback:</span> " + Server.HtmlEncode(Eval("Feedback").ToString()) + "</p>" : "" %>
                            </div>
                            <div class="flex flex-col gap-2 sm:w-72">
                                <%# Eval("Marks") != null ? "<span class=\"rounded-lg bg-emerald-50 px-3 py-1.5 text-emerald-700\" style=\"font-size:12.5px;font-weight:700\">" + System.Convert.ToDecimal(Eval("Marks")).ToString("0.#") + " / 100</span>" : "" %>
                                <asp:FileUpload ID="submissionInput" runat="server" CssClass="block w-full rounded-md border border-dashed border-slate-300 bg-slate-50 px-3 py-2 text-slate-600 file:mr-3 file:rounded-md file:border-0 file:bg-white file:px-3 file:py-1.5 file:text-slate-700" style="font-size:12px" />
                                <asp:LinkButton ID="submitAssignmentButton" runat="server" CommandName="SubmitAssignment" CommandArgument='<%# Eval("AssignmentId") %>'
                                    CssClass="inline-flex h-9 items-center justify-center gap-1.5 rounded-md bg-[#e0162b] px-3 text-white hover:bg-[#a01020]" style="font-size:12.5px;font-weight:600">
                                    <i data-lucide="upload" class="h-4 w-4"></i>Submit
                                </asp:LinkButton>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <%-- ==================== GRADES PANE ==================== --%>
        <div data-pane="grades" class="hidden">
            <div class="grid gap-6 lg:grid-cols-3">

                <%-- Overall summary card --%>
                <div class="rounded-2xl border border-slate-200 bg-white p-6">
                    <h3 class="text-slate-900" style="font-size:15px;font-weight:600">Overall Grade</h3>
                    <p class="text-slate-500 mt-0.5" style="font-size:12.5px">Based on <%= Book.Items.FindAll(i => i.IsGraded).Count %> graded items</p>
                    <div class="relative mx-auto mt-3 h-44 w-44 flex items-center justify-center">
                        <svg viewBox="0 0 120 120" class="w-full h-full -rotate-90">
                            <circle cx="60" cy="60" r="48" fill="none" stroke="#f1f5f9" stroke-width="12" />
                            <circle cx="60" cy="60" r="48" fill="none" style="stroke:var(--course-accent)" stroke-width="12"
                                stroke-dasharray="301.6" stroke-dashoffset='<%= DonutOffset(Book.OverallAverage) %>' stroke-linecap="round" />
                        </svg>
                        <div class="absolute inset-0 flex flex-col items-center justify-center">
                            <div class="text-slate-900" style="font-size:32px;font-weight:700;letter-spacing:-0.01em"><%= Book.OverallAverage.HasValue ? Book.OverallAverage.Value + "%" : "—" %></div>
                            <div class="text-slate-500" style="font-size:12px">Average</div>
                        </div>
                    </div>
                    <div class="mt-4 grid grid-cols-2 gap-3">
                        <div class="rounded-xl bg-slate-50 p-3">
                            <div class="text-slate-500" style="font-size:11.5px;font-weight:500">Earned (weighted)</div>
                            <div class="text-slate-900 mt-1" style="font-size:18px;font-weight:700"><%= Book.EarnedWeighted %><span class="text-slate-400" style="font-size:12px"> /100</span></div>
                        </div>
                        <div class="rounded-xl bg-slate-50 p-3">
                            <div class="text-slate-500" style="font-size:11.5px;font-weight:500">Completed</div>
                            <div class="text-slate-900 mt-1" style="font-size:18px;font-weight:700"><%= Book.CompletedPercent %><span class="text-slate-400" style="font-size:12px">%</span></div>
                        </div>
                    </div>
                </div>

                <%-- Score breakdown bar chart (static SVG approximation) --%>
                <div class="lg:col-span-2 rounded-2xl border border-slate-200 bg-white p-6">
                    <header class="mb-4 flex items-center justify-between">
                        <div>
                            <h3 class="text-slate-900" style="font-size:15px;font-weight:600">Score Breakdown</h3>
                            <p class="text-slate-500 mt-0.5" style="font-size:12.5px">Per-assessment scores out of 100</p>
                        </div>
                        <div class="flex items-center gap-3 text-slate-500" style="font-size:11.5px">
                            <span class="inline-flex items-center gap-1.5">
                                <span class="h-2.5 w-2.5 rounded-sm" style="background-color:var(--course-accent)"></span> Score
                            </span>
                        </div>
                    </header>
                    <%-- Static bar chart --%>
                    <div class="h-72 flex items-end gap-3 px-2 pb-8 relative">
                        <div class="absolute inset-x-2 bottom-8 top-0 flex flex-col justify-between pointer-events-none">
                            <div class="border-b border-slate-100 flex items-center text-slate-400" style="font-size:10px">100</div>
                            <div class="border-b border-slate-100 flex items-center text-slate-400" style="font-size:10px">75</div>
                            <div class="border-b border-slate-100 flex items-center text-slate-400" style="font-size:10px">50</div>
                            <div class="border-b border-slate-100 flex items-center text-slate-400" style="font-size:10px">25</div>
                            <div class="border-b border-slate-100 flex items-center text-slate-400" style="font-size:10px">0</div>
                        </div>
                        <asp:Repeater ID="barsRepeater" runat="server">
                            <ItemTemplate>
                                <div class="flex-1 flex flex-col items-center gap-1 z-10" style="min-width:0">
                                    <div class="w-full rounded-t-md" style='height:<%# (bool)Eval("IsGraded") ? System.Math.Round(System.Convert.ToDecimal(Eval("Marks")) / System.Convert.ToInt32(Eval("MaxMarks")) * 72, 0) : 0 %>%;background-color:var(--course-accent)'></div>
                                    <span class="text-slate-500 text-center" title='<%# Server.HtmlEncode(Eval("Name").ToString()) %>' style="font-size:10px;max-width:100%;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;display:block"><%# Server.HtmlEncode(Eval("Name").ToString()) %></span>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <%-- All assessments table --%>
                <div class="lg:col-span-3 rounded-2xl border border-slate-200 bg-white">
                    <header class="flex items-center justify-between p-6 pb-4">
                        <div>
                            <h3 class="text-slate-900" style="font-size:15px;font-weight:600">All Assessments</h3>
                            <p class="text-slate-500 mt-0.5" style="font-size:12.5px">Detailed marks and weighting</p>
                        </div>
                    </header>
                    <div class="overflow-x-auto">
                        <table class="w-full">
                            <thead>
                                <tr class="text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">
                                    <th class="text-left px-6 py-3">ASSESSMENT</th>
                                    <th class="text-left px-3 py-3">TYPE</th>
                                    <th class="text-right px-3 py-3">WEIGHT</th>
                                    <th class="text-right px-3 py-3">SCORE</th>
                                    <th class="text-right px-6 py-3">CONTRIB.</th>
                                </tr>
                            </thead>
                            <tbody class="divide-y divide-slate-100">
                                <asp:Repeater ID="assessmentsRepeater" runat="server">
                                    <ItemTemplate>
                                        <tr class="hover:bg-slate-50/60">
                                            <td class="px-6 py-3.5 text-slate-900" style="font-size:13px;font-weight:500"><%# Server.HtmlEncode(Eval("Name").ToString()) %></td>
                                            <td class="px-3 py-3.5 text-slate-500" style="font-size:12.5px"><%# Server.HtmlEncode(Eval("Type").ToString()) %></td>
                                            <td class="px-3 py-3.5 text-right text-slate-600" style="font-size:12.5px"><%# System.Convert.ToDecimal(Eval("Weight")).ToString("0.#") %>%</td>
                                            <td class="px-3 py-3.5 text-right" style="font-size:13px;font-weight:600">
                                                <%# (bool)Eval("IsGraded") ? "<span class=\"text-emerald-700\">" + System.Convert.ToDecimal(Eval("Marks")).ToString("0.#") + " / " + Eval("MaxMarks") + "</span>" : "<span class=\"text-slate-400\">Pending</span>" %>
                                            </td>
                                            <td class="px-6 py-3.5 text-right text-slate-700" style="font-size:12.5px;font-weight:600"><%# Eval("Contribution") != null ? System.Convert.ToDecimal(Eval("Contribution")).ToString("0.#") + " pts" : "—" %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                            <tfoot>
                                <tr class="bg-slate-50">
                                    <td class="px-6 py-3.5 text-slate-900" colspan="2" style="font-size:13px;font-weight:700">Total</td>
                                    <td class="px-3 py-3.5 text-right text-slate-900" style="font-size:13px;font-weight:700">100%</td>
                                    <td class="px-3 py-3.5 text-right text-slate-500" style="font-size:12.5px">&mdash;</td>
                                    <td class="px-6 py-3.5 text-right text-slate-900" style="font-size:13px;font-weight:700"><%= Book.EarnedWeighted %> pts</td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                </div>

            </div>
        </div>

    </section>

    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/student/course-detail/course-detail.js") %>"></script>
</asp:Content>
