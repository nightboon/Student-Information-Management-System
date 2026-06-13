<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="admin_sidebar.ascx.cs" Inherits="src.controls.admin_sidebar" %>

<div class="hidden w-64 shrink-0 lg:block" aria-hidden="true"></div>

<aside id="mobile-menu"
       data-open="false"
       class="fixed inset-y-0 left-0 z-50 flex h-screen w-64 shrink-0 flex-col border-r border-slate-200 bg-white -translate-x-full transition-transform duration-300 data-[open=true]:translate-x-0 lg:translate-x-0">
    <div class="flex h-20 items-center px-6 border-b border-slate-100">
        <img src="<%= ResolveUrl("~/img/inti_logo.png") %>" alt="INTI" class="h-25 w-auto object-contain" />
        <button data-action="close-menu" aria-label="Close menu" class="ml-auto lg:hidden inline-flex h-9 w-9 items-center justify-center rounded-md text-slate-500 hover:bg-slate-100 hover:text-slate-900 transition-colors">
            <i data-lucide="x" class="h-4 w-4"></i>
        </button>
    </div>

    <nav class="flex-1 overflow-y-auto px-3 py-5">
        <p class="px-3 pb-2 text-slate-400" style="font-size:11px;font-weight:600;letter-spacing:0.08em">MAIN MENU</p>
        <ul class="space-y-0.5">
            <li>
                <a href="<%= ResolveUrl("~/admin/admin_dashboard.aspx") %>" data-nav-link="admin_dashboard.aspx" class="group flex items-center gap-3 rounded-xl px-3 py-2.5 transition-all text-slate-600 hover:bg-slate-50 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020] data-[active=true]:font-semibold" style="font-size:14px;font-weight:500">
                    <i data-lucide="layout-dashboard" class="h-4 w-4 text-slate-400 group-hover:text-slate-700"></i>
                    <span class="flex-1">Dashboard</span>
                </a>
            </li>
            <li>
                <a href="<%= ResolveUrl("~/admin/user_management.aspx") %>" data-nav-link="user_management.aspx" class="group flex items-center gap-3 rounded-xl px-3 py-2.5 transition-all text-slate-600 hover:bg-slate-50 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020] data-[active=true]:font-semibold" style="font-size:14px;font-weight:500">
                    <i data-lucide="users" class="h-4 w-4 text-slate-400 group-hover:text-slate-700"></i>
                    <span class="flex-1">User Management</span>
                </a>
            </li>
            <li>
                <a href="<%= ResolveUrl("~/admin/programme_course.aspx") %>" data-nav-link="programme_course.aspx" class="group flex items-center gap-3 rounded-xl px-3 py-2.5 transition-all text-slate-600 hover:bg-slate-50 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020] data-[active=true]:font-semibold" style="font-size:14px;font-weight:500">
                    <i data-lucide="book-open" class="h-4 w-4 text-slate-400 group-hover:text-slate-700"></i>
                    <span class="flex-1">Programmes &amp; Courses</span>
                </a>
            </li>
            <li>
                <a href="<%= ResolveUrl("~/admin/academic_calendar.aspx") %>" data-nav-link="academic_calendar.aspx" class="group flex items-center gap-3 rounded-xl px-3 py-2.5 transition-all text-slate-600 hover:bg-slate-50 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020] data-[active=true]:font-semibold" style="font-size:14px;font-weight:500">
                    <i data-lucide="calendar-days" class="h-4 w-4 text-slate-400 group-hover:text-slate-700"></i>
                    <span class="flex-1">Academic Calendar</span>
                </a>
            </li>
            <li>
                <a href="<%= ResolveUrl("~/admin/academic_performance.aspx") %>" data-nav-link="academic_performance.aspx" class="group flex items-center gap-3 rounded-xl px-3 py-2.5 transition-all text-slate-600 hover:bg-slate-50 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020] data-[active=true]:font-semibold" style="font-size:14px;font-weight:500">
                    <i data-lucide="trending-up" class="h-4 w-4 text-slate-400 group-hover:text-slate-700"></i>
                    <span class="flex-1">Academic Performance</span>
                </a>
            </li>
            <li>
                <a href="<%= ResolveUrl("~/admin/add_drop_requests.aspx") %>" data-nav-link="add_drop_requests.aspx" class="group flex items-center gap-3 rounded-xl px-3 py-2.5 transition-all text-slate-600 hover:bg-slate-50 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020] data-[active=true]:font-semibold" style="font-size:14px;font-weight:500">
                    <i data-lucide="inbox" class="h-4 w-4 text-slate-400 group-hover:text-slate-700"></i>
                    <span class="flex-1">Add/Drop Requests</span>
                </a>
            </li>
            <li>
                <a href="<%= ResolveUrl("~/admin/report_generator.aspx") %>" data-nav-link="report_generator.aspx" class="group flex items-center gap-3 rounded-xl px-3 py-2.5 transition-all text-slate-600 hover:bg-slate-50 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020] data-[active=true]:font-semibold" style="font-size:14px;font-weight:500">
                    <i data-lucide="file-text" class="h-4 w-4 text-slate-400 group-hover:text-slate-700"></i>
                    <span class="flex-1">Reports</span>
                </a>
            </li>
        </ul>

        <p class="mt-7 px-3 pb-2 text-slate-400" style="font-size:11px;font-weight:600;letter-spacing:0.08em">SUPPORT</p>
        <ul class="space-y-0.5">
            <li>
                <a href="#" class="group flex items-center gap-3 rounded-xl px-3 py-2.5 text-slate-600 hover:bg-slate-50 hover:text-slate-900 transition-all" style="font-size:14px;font-weight:500">
                    <i data-lucide="life-buoy" class="h-4 w-4 text-slate-400 group-hover:text-slate-700"></i>
                    Help &amp; Support
                </a>
            </li>
        </ul>
    </nav>

    <div class="border-t border-slate-100 p-3">
        <a href="<%= ResolveUrl("~/shared/login.aspx") %>" data-action="logout" class="flex w-full items-center gap-3 rounded-xl px-3 py-2.5 text-slate-600 hover:bg-slate-50 hover:text-[#e0162b] transition-colors" style="font-size:14px;font-weight:500">
            <i data-lucide="log-out" class="h-4 w-4 text-slate-400"></i>
            Sign out
        </a>
    </div>
</aside>
