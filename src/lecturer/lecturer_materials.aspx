<%@ Page Language="C#" MasterPageFile="~/shared/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_materials.aspx.cs" Inherits="student_information_management_system.lecturer_materials" Title="Course Materials - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div><p class="text-slate-500" style="font-size:13px;font-weight:500">Lecturer Module</p><h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700">Course Materials</h1><p class="mt-1 text-slate-500" style="font-size:14px">Organise lecture slides, lab sheets, readings, and student-facing resources.</p></div>
    </div>
    <asp:Panel ID="statusBanner" runat="server" Visible="false" CssClass="mt-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800" style="font-size:13px;font-weight:600"><asp:Literal ID="statusMessage" runat="server" /></asp:Panel>

    <section class="mt-6 grid gap-6 lg:grid-cols-[1fr_340px]">
        <div class="rounded-lg border border-slate-200 bg-white">
            <div class="flex flex-col gap-3 border-b border-slate-100 px-6 py-4 md:flex-row md:items-center md:justify-between">
                <div><h2 class="text-slate-900" style="font-size:16px;font-weight:700">Published Resources</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">Visible material records by assigned course.</p></div>
                <div class="relative"><i data-lucide="search" class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"></i><input data-filter-input data-filter-target="[data-material]" type="search" placeholder="Search materials" class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 md:w-64" style="font-size:12.5px" /></div>
            </div>
            <asp:Panel ID="emptyPanel" runat="server" Visible="false" CssClass="px-6 py-10 text-center text-slate-500" style="font-size:13px">No materials have been published yet.</asp:Panel>
            <ul class="divide-y divide-slate-100">
                <asp:Repeater ID="materialsRepeater" runat="server">
                    <ItemTemplate>
                        <li data-material data-filter-text='<%# Html(Eval("Title")) %> <%# Html(Eval("CourseCode")) %>' class="flex flex-col gap-3 px-6 py-4 md:flex-row md:items-center md:justify-between">
                            <div class="flex min-w-0 items-start gap-3">
                                <span class="flex h-10 w-10 shrink-0 items-center justify-center rounded-md bg-[#e0162b]/10 text-[#e0162b]"><i data-lucide='<%# FileIcon(Eval("FileType")) %>' class="h-5 w-5"></i></span>
                                <div class="min-w-0">
                                    <p class="truncate text-slate-900" style="font-size:14px;font-weight:700"><%# Html(Eval("Title")) %></p>
                                    <p class="mt-0.5 text-slate-500" style="font-size:12.5px"><%# Html(Eval("CourseCode")) %> - Week <%# Eval("Week") ?? "-" %> - Uploaded <%# UploadedLabel(Eval("UploadedAt")) %> - <%# FileMeta(Eval("FileType"), Eval("FileSizeBytes")) %></p>
                                    <p class="mt-1 text-slate-500" style="font-size:12.5px"><%# Html(Eval("Description")) %></p>
                                </div>
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>

        <aside class="rounded-lg border border-slate-200 bg-white">
            <div class="border-b border-slate-100 px-6 py-4"><h2 class="text-slate-900" style="font-size:16px;font-weight:700">Upload Details</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">Publish a new material record.</p></div>
            <div class="space-y-4 px-6 py-5">
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">COURSE</span><asp:DropDownList ID="courseSelect" runat="server" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3" style="font-size:13px" /></label>
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">TITLE</span><asp:TextBox ID="titleInput" runat="server" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" placeholder="Material title" style="font-size:13px" /></label>
                <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">DESCRIPTION</span><asp:TextBox ID="descriptionInput" runat="server" TextMode="MultiLine" CssClass="mt-1.5 min-h-[88px] w-full rounded-md border border-slate-200 px-3 py-2" placeholder="Short student-facing description" style="font-size:13px" /></label>
                <div class="grid grid-cols-3 gap-2">
                    <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">WEEK</span><asp:TextBox ID="weekInput" runat="server" TextMode="Number" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" /></label>
                    <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">TYPE</span><asp:TextBox ID="fileTypeInput" runat="server" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" placeholder="pdf" /></label>
                    <label class="block"><span class="text-slate-500" style="font-size:12px;font-weight:700">BYTES</span><asp:TextBox ID="sizeInput" runat="server" TextMode="Number" CssClass="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" /></label>
                </div>
                <asp:LinkButton ID="publishButton" runat="server" OnClick="PublishMaterial_Click" CssClass="inline-flex h-10 w-full items-center justify-center rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:700">Publish material</asp:LinkButton>
            </div>
        </aside>
    </section>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/lecturer/lecturer-portal.js") %>"></script>
</asp:Content>
