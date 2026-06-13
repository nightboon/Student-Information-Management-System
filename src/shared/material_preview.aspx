<%@ Page Language="C#" MasterPageFile="~/shared/DashboardLayout.master" AutoEventWireup="true" CodeBehind="material_preview.aspx.cs" Inherits="src.shared.material_preview" Title="Material Preview" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <a href="<%= BackUrl %>" class="inline-flex items-center gap-1.5 text-slate-500 hover:text-slate-900" style="font-size:13px;font-weight:600"><i data-lucide="arrow-left" class="h-4 w-4"></i>Back</a>
            <p class="mt-4 text-slate-500" style="font-size:13px;font-weight:600"><%= Html(Material == null ? "" : Material.CourseCode) %></p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700"><%= Html(Material == null ? "Material unavailable" : Material.Title) %></h1>
            <p class="mt-1 text-slate-500" style="font-size:14px"><%= Html(Material == null ? "" : Material.Description) %></p>
        </div>
        <asp:HyperLink ID="downloadLink" runat="server" CssClass="inline-flex h-10 items-center justify-center gap-1.5 rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:700">
            <i data-lucide="download" class="h-4 w-4"></i>Download
        </asp:HyperLink>
    </div>

    <asp:Panel ID="missingPanel" runat="server" Visible="false" CssClass="mt-6 rounded-lg border border-amber-200 bg-amber-50 px-5 py-4 text-amber-800" style="font-size:13px;font-weight:600">
        This material is unavailable or you do not have access to it.
    </asp:Panel>

    <asp:Panel ID="previewPanel" runat="server" Visible="false" CssClass="mt-6 overflow-hidden rounded-lg border border-slate-200 bg-white">
        <div class="flex items-center justify-between border-b border-slate-100 px-5 py-3">
            <div class="flex items-center gap-2 text-slate-600" style="font-size:12.5px;font-weight:700">
                <i data-lucide="file" class="h-4 w-4"></i><asp:Literal ID="fileTypeLabel" runat="server" />
            </div>
        </div>
        <div class="min-h-[70vh] bg-slate-100 p-4">
            <iframe ID="previewFrame" runat="server" class="h-[70vh] w-full rounded-md border border-slate-200 bg-white"></iframe>
            <asp:Panel ID="limitedPreviewPanel" runat="server" Visible="false" CssClass="flex min-h-[55vh] items-center justify-center rounded-md border border-slate-200 bg-white px-6 py-10 text-center text-slate-600" style="font-size:13px">
                <div>
                    <div class="mx-auto flex h-12 w-12 items-center justify-center rounded-lg bg-slate-100 text-slate-500"><i data-lucide="file-warning" class="h-6 w-6"></i></div>
                    <p class="mt-3 text-slate-900" style="font-size:15px;font-weight:700">Preview is not available for this file type.</p>
                </div>
            </asp:Panel>
        </div>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/shared/core/icons-init.js") %>"></script>
</asp:Content>
