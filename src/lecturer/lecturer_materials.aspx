<%@ Page Language="C#" MasterPageFile="~/shared/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_materials.aspx.cs" Inherits="student_information_management_system.lecturer_materials" Title="Course Materials - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Lecturer Module</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700">Course Materials</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">Organise assignments, lecture notes, quizzes, and tests for your students.</p>
        </div>
    </div>

    <asp:Panel ID="statusBanner" runat="server" Visible="false" CssClass="mt-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800" style="font-size:13px;font-weight:600">
        <asp:Literal ID="statusMessage" runat="server" />
    </asp:Panel>

    <section class="mt-6 grid gap-6 lg:grid-cols-[1fr_340px]">
        <div class="rounded-lg border border-slate-200 bg-white">
            <div class="flex flex-col gap-3 border-b border-slate-100 px-6 py-4 xl:flex-row xl:items-center xl:justify-between">
                <div>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:700">Published Resources</h2>
                    <p class="mt-0.5 text-slate-500" style="font-size:12.5px">Visible material records by assigned course.</p>
                </div>
                <div class="flex flex-col gap-2 sm:flex-row">
                    <asp:DropDownList ID="courseFilterSelect" runat="server" data-material-course-filter="true" CssClass="h-9 rounded-md border border-slate-200 bg-white px-3 text-slate-700 sm:w-44" style="font-size:12.5px" />
                    <div class="relative">
                        <i data-lucide="search" class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"></i>
                        <input data-filter-input data-filter-target="[data-material]" type="search" placeholder="Search materials" class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 xl:w-64" style="font-size:12.5px" />
                    </div>
                </div>
            </div>

            <div class="flex flex-wrap gap-2 border-b border-slate-100 px-6 py-3" data-material-tabs>
                <button type="button" data-material-tab="all" class="inline-flex h-8 items-center gap-1.5 rounded-md bg-slate-900 px-3 text-white" style="font-size:12px;font-weight:700">All <span class="rounded bg-white/20 px-1.5"><%= CountByType(null) %></span></button>
                <button type="button" data-material-tab="Assignment" class="inline-flex h-8 items-center gap-1.5 rounded-md bg-slate-100 px-3 text-slate-600 hover:bg-slate-200" style="font-size:12px;font-weight:700"><i data-lucide="clipboard-check" class="h-3.5 w-3.5"></i>Assignment <span class="rounded bg-white px-1.5"><%= CountByType("Assignment") %></span></button>
                <button type="button" data-material-tab="Lecture Notes" class="inline-flex h-8 items-center gap-1.5 rounded-md bg-slate-100 px-3 text-slate-600 hover:bg-slate-200" style="font-size:12px;font-weight:700"><i data-lucide="book-open" class="h-3.5 w-3.5"></i>Lecture Notes <span class="rounded bg-white px-1.5"><%= CountByType("Lecture Notes") %></span></button>
                <button type="button" data-material-tab="Quiz" class="inline-flex h-8 items-center gap-1.5 rounded-md bg-slate-100 px-3 text-slate-600 hover:bg-slate-200" style="font-size:12px;font-weight:700"><i data-lucide="circle-help" class="h-3.5 w-3.5"></i>Quiz <span class="rounded bg-white px-1.5"><%= CountByType("Quiz") %></span></button>
                <button type="button" data-material-tab="Test" class="inline-flex h-8 items-center gap-1.5 rounded-md bg-slate-100 px-3 text-slate-600 hover:bg-slate-200" style="font-size:12px;font-weight:700"><i data-lucide="clipboard-list" class="h-3.5 w-3.5"></i>Test <span class="rounded bg-white px-1.5"><%= CountByType("Test") %></span></button>
            </div>

            <asp:Panel ID="emptyPanel" runat="server" Visible="false" CssClass="px-6 py-10 text-center text-slate-500" style="font-size:13px">No materials have been published yet.</asp:Panel>
            <ul class="divide-y divide-slate-100">
                <asp:Repeater ID="materialsRepeater" runat="server" OnItemCommand="MaterialsRepeater_ItemCommand">
                    <ItemTemplate>
                        <li data-material data-material-url='<%# MaterialPreviewUrl(Eval("MaterialId")) %>' data-material-category='<%# Html(Eval("MaterialType")) %>' data-material-course='<%# Html(Eval("CourseCode")) %>' data-filter-text='<%# Html(Eval("Title")) %> <%# Html(Eval("CourseCode")) %> <%# Html(Eval("CourseName")) %> <%# Html(Eval("MaterialType")) %>' class="flex cursor-pointer flex-col gap-3 px-6 py-4 hover:bg-slate-50/70 md:flex-row md:items-center md:justify-between">
                            <div class="flex min-w-0 items-start gap-3">
                                <span class="flex h-10 w-10 shrink-0 items-center justify-center rounded-md bg-[#e0162b]/10 text-[#e0162b]"><i data-lucide='<%# MaterialIcon(Eval("MaterialType")) %>' class="h-5 w-5"></i></span>
                                <div class="min-w-0">
                                    <div class="flex flex-wrap items-center gap-2">
                                        <p class="truncate text-slate-900" style="font-size:14px;font-weight:700"><%# Html(Eval("Title")) %></p>
                                        <span class='rounded px-2 py-0.5 <%# TypeBadgeClass(Eval("MaterialType")) %>' style="font-size:11px;font-weight:800"><%# Html(Eval("MaterialType")) %></span>
                                    </div>
                                    <p class="mt-0.5 text-slate-500" style="font-size:12.5px"><%# Html(Eval("CourseCode")) %> - Due <%# DueDateLabel(Eval("DueDate")) %> - Uploaded <%# UploadedLabel(Eval("UploadedAt")) %> - <%# FileMeta(Eval("FileType"), Eval("FileSizeBytes")) %></p>
                                    <p class="mt-1 text-slate-500" style="font-size:12.5px"><%# Html(Eval("Description")) %></p>
                                </div>
                            </div>
                            <div class="flex shrink-0 items-center gap-4 md:justify-end">
                                <div class="text-right">
                                    <p class="text-slate-400" style="font-size:10.5px;font-weight:800;letter-spacing:0.08em">COURSE WEIGHT</p>
                                    <p class="text-slate-900" style="font-size:14px;font-weight:800"><%# WeightLabel(Eval("Weight")) %></p>
                                </div>
                                <asp:LinkButton runat="server" CommandName="DeleteMaterial" CommandArgument='<%# Eval("MaterialId") %>' OnClientClick="return confirm('Delete this material?');" CssClass="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-red-50 hover:text-[#e0162b]" ToolTip="Delete material"><i data-lucide="trash-2" class="h-4 w-4"></i></asp:LinkButton>
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>

        <aside class="rounded-lg border border-slate-200 bg-white">
            <div class="border-b border-slate-100 px-6 py-4">
                <h2 class="text-slate-900" style="font-size:16px;font-weight:700">Upload Details</h2>
                <p class="mt-0.5 text-slate-500" style="font-size:12.5px">Publish a new student-facing file.</p>
            </div>
            <div class="space-y-4 px-6 py-5">
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">COURSE</span><asp:DropDownList ID="courseSelect" runat="server" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3" style="font-size:13px" /></label>
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">TYPE</span><asp:DropDownList ID="materialTypeSelect" runat="server" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3" style="font-size:13px" /></label>
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">TITLE</span><asp:TextBox ID="titleInput" runat="server" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" placeholder="Material title" style="font-size:13px" /></label>
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">DESCRIPTION</span><asp:TextBox ID="descriptionInput" runat="server" TextMode="MultiLine" CssClass="mt-1.5 min-h-[88px] w-full rounded-md border border-slate-200 px-3 py-2" placeholder="Short student-facing description" style="font-size:13px" /></label>
                <div class="grid grid-cols-2 gap-2">
                    <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">DUE DATE</span><asp:TextBox ID="dueDateInput" runat="server" TextMode="Date" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" /></label>
                    <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">WEIGHT %</span><asp:TextBox ID="weightInput" runat="server" TextMode="Number" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" /></label>
                </div>
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">FILE</span><asp:FileUpload ID="materialFileInput" runat="server" CssClass="mt-1.5 block w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-slate-600" /></label>
                <asp:LinkButton ID="publishButton" runat="server" OnClick="PublishMaterial_Click" CssClass="inline-flex h-10 w-full items-center justify-center gap-1.5 rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:700"><i data-lucide="upload" class="h-4 w-4"></i>Publish material</asp:LinkButton>
            </div>
        </aside>
    </section>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/lecturer/lecturer-portal.js") %>"></script>
</asp:Content>
