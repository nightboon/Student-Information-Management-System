<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="lecturer_topbar.ascx.cs" Inherits="src.controls.lecturer_topbar" %>

<header class="sticky top-0 z-30 flex h-16 items-center gap-3 border-b border-slate-200 bg-white/80 px-6 backdrop-blur-md">

    <button type="button"
            data-action="toggle-menu"
            aria-label="Menu"
            aria-expanded="false"
            class="rounded-lg p-2 text-slate-600 hover:bg-slate-100 lg:hidden">
        <i data-lucide="menu" class="h-5 w-5"></i>
    </button>

    <div class="ml-auto flex items-center gap-2">

        <button type="button"
                class="inline-flex h-10 w-10 items-center justify-center rounded-xl text-slate-500 transition-colors hover:bg-slate-100 hover:text-slate-900"
                aria-label="Help">
            <i data-lucide="help-circle" class="h-5 w-5"></i>
        </button>

        <%-- Notifications: styled to match the student topbar but not yet wired
             (no lecturer unread-count source), so it is a plain non-navigating
             button with no count badge for now. --%>
        <button type="button"
                class="relative inline-flex h-10 w-10 items-center justify-center rounded-xl border border-slate-200 bg-white text-slate-600 transition-colors hover:bg-slate-50 hover:text-slate-900"
                aria-label="Notifications">
            <i data-lucide="bell" class="h-4 w-4"></i>
        </button>

        <a href="<%= ResolveUrl("~/lecturer/lecturer_account.aspx") %>"
           class="flex items-center gap-2.5 rounded-full border border-slate-200 bg-white py-1 pl-1 pr-3 transition-colors hover:border-[#e0162b]/30 hover:bg-[#e0162b]/[0.03]"
           aria-label="Open account">

            <% if (!string.IsNullOrEmpty(IconUrl)) { %>
                <img src="<%= IconUrl %>"
                     alt="Profile picture"
                     class="h-8 w-8 rounded-full object-cover" />
            <% } else { %>
                <span class="flex h-8 w-8 items-center justify-center rounded-full bg-gradient-to-br from-[#e0162b] to-[#a01020] text-white" style="font-size:12px;font-weight:700"><%= Initials %></span>
            <% } %>

            <div class="text-left leading-tight">
                <div class="text-slate-900" style="font-size:13px;font-weight:600">
                    <%= FullName %>
                </div>

                <div class="text-slate-500" style="font-size:11px">
                    <%= Subtitle %>
                </div>
            </div>
        </a>

    </div>
</header>
