<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="lecturer_sidebar.ascx.cs" Inherits="src.controls.lecturer_sidebar" %>

<div class="hidden w-64 shrink-0 lg:block" aria-hidden="true"></div>

<aside id="mobile-menu" data-open="false"
       class="fixed inset-y-0 left-0 z-50 flex h-screen w-64 shrink-0 flex-col border-r border-slate-200 bg-white -translate-x-full transition-transform duration-300 data-[open=true]:translate-x-0 lg:translate-x-0">
    <div class="flex h-20 items-center px-6 border-b border-slate-100">
        <img src="<%= ResolveUrl("~/img/inti_logo.png") %>" alt="INTI" class="h-25 w-auto object-contain" />
        <button data-action="close-menu" aria-label="Close menu" class="ml-auto lg:hidden inline-flex h-9 w-9 items-center justify-center rounded-md text-slate-500 hover:bg-slate-100 hover:text-slate-900 transition-colors">
            <i data-lucide="x" class="h-4 w-4"></i>
        </button>
    </div>

    <nav class="flex-1 overflow-y-auto px-3 py-5">
        <p class="px-3 pb-2 text-slate-400" style="font-size:11px;font-weight:600;letter-spacing:0.08em">LECTURER MENU</p>
        <ul class="space-y-0.5">
            <li><a href="<%= ResolveUrl("~/lecturer/lecturer_dashboard.aspx") %>" class="group flex items-center gap-3 rounded-xl px-3 py-2.5 text-slate-600 hover:bg-slate-50 hover:text-slate-900 transition-all" style="font-size:14px;font-weight:500" data-nav-link="lecturer_dashboard.aspx"><i data-lucide="layout-dashboard" class="h-4 w-4 text-slate-400"></i><span class="flex-1">Dashboard</span></a></li>
            <li><a href="<%= ResolveUrl("~/lecturer/lecturer_courses.aspx") %>" class="group flex items-center gap-3 rounded-xl px-3 py-2.5 text-slate-600 hover:bg-slate-50 hover:text-slate-900 transition-all" style="font-size:14px;font-weight:500" data-nav-link="lecturer_courses.aspx"><i data-lucide="book-open" class="h-4 w-4 text-slate-400"></i><span class="flex-1">My Courses</span></a></li>
        </ul>
    </nav>

    <div class="border-t border-slate-100 p-3">
        <a href="<%= ResolveUrl("~/shared/login.aspx") %>" data-action="logout" class="flex w-full items-center gap-3 rounded-xl px-3 py-2.5 text-slate-600 hover:bg-slate-50 hover:text-[#e0162b] transition-colors" style="font-size:14px;font-weight:500">
            <i data-lucide="log-out" class="h-4 w-4 text-slate-400"></i>
            Sign out
        </a>
    </div>
</aside>
