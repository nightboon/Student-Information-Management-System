<%@ Page Language="C#" MasterPageFile="~/admin/AdminLayout.master" AutoEventWireup="true" CodeBehind="user_management.aspx.cs" Inherits="src.admin.user_management" Title="User Management - INTI Admin Portal" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <%-- Page header --%>
    <div class="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Admin</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">User Management</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">Manage student and lecturer accounts, roles, and access.</p>
        </div>
        <div class="flex flex-wrap items-center gap-2">
            <div class="relative" data-dropdown>
                <button type="button" data-dropdown-toggle class="inline-flex items-center gap-1.5 rounded-md border border-slate-200 bg-white px-3 h-10 text-slate-700 hover:bg-slate-50 transition-colors" style="font-size:13px;font-weight:600">
                    <i data-lucide="download" class="h-4 w-4"></i> Export <i data-lucide="chevron-down" class="h-3.5 w-3.5"></i>
                </button>
                <div data-dropdown-menu style="display:none" class="absolute right-0 z-20 mt-2 w-48 rounded-xl border border-slate-200 bg-white p-1 shadow-lg">
                    <button type="button" data-toast="PDF export started" data-toast-desc="users queued for download" class="flex w-full items-center gap-2 rounded-md px-3 py-2 text-left text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:500"><i data-lucide="file-text" class="h-4 w-4 text-[#e0162b]"></i> Export as PDF</button>
                    <button type="button" data-toast="Excel export started" data-toast-desc="users queued for download" class="flex w-full items-center gap-2 rounded-md px-3 py-2 text-left text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:500"><i data-lucide="file-spreadsheet" class="h-4 w-4 text-emerald-600"></i> Export as Excel</button>
                </div>
            </div>
            <button type="button" data-modal-open="create-user" class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]" style="font-size:13px;font-weight:600">
                <i data-lucide="user-plus" class="h-4 w-4"></i> Create User
            </button>
        </div>
    </div>

    <%-- Summary cards --%>
    <section class="mt-6" style="display:grid;grid-template-columns:repeat(auto-fit,minmax(220px,1fr));gap:16px">
        <div class="rounded-2xl border border-slate-200 bg-white p-5"><div class="text-slate-500" style="font-size:12.5px;font-weight:500">Total Users</div><div class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">9</div><div class="mt-1 text-slate-400" style="font-size:12px">all accounts</div></div>
        <div class="rounded-2xl border border-slate-200 bg-white p-5"><div class="text-slate-500" style="font-size:12.5px;font-weight:500">Students</div><div class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">6</div><div class="mt-1 text-slate-400" style="font-size:12px">enrolled</div></div>
        <div class="rounded-2xl border border-slate-200 bg-white p-5"><div class="text-slate-500" style="font-size:12.5px;font-weight:500">Lecturers</div><div class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">3</div><div class="mt-1 text-slate-400" style="font-size:12px">teaching staff</div></div>
        <div class="rounded-2xl border border-slate-200 bg-white p-5"><div class="text-slate-500" style="font-size:12.5px;font-weight:500">Active Accounts</div><div class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">8</div><div class="mt-1 text-slate-400" style="font-size:12px">last 30 days</div></div>
    </section>

    <%-- Table card --%>
    <section class="mt-6 rounded-lg border border-slate-200 bg-white">
        <div data-table data-page-size="6">
            <div class="flex flex-col gap-3 px-6 py-4 lg:flex-row lg:items-center lg:justify-between">
                <div class="w-full lg:max-w-sm">
                    <div class="relative">
                        <svg viewBox="0 0 24 24" class="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="7" /><path d="m21 21-4.3-4.3" /></svg>
                        <input data-table-search placeholder="Search by name, ID, or email&hellip;" class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 text-slate-900 placeholder:text-slate-400 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px" />
                    </div>
                </div>
                <div class="flex flex-wrap items-center gap-2">
                    <select data-table-filter="role" class="h-9 rounded-md border border-slate-200 bg-white px-3 text-slate-700 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px"><option value="">All roles</option><option>Student</option><option>Lecturer</option></select>
                    <select data-table-filter="status" class="h-9 rounded-md border border-slate-200 bg-white px-3 text-slate-700 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px"><option value="">All statuses</option><option>Active</option><option>Pending</option><option>Inactive</option></select>
                    <select data-table-filter="programme" class="h-9 rounded-md border border-slate-200 bg-white px-3 text-slate-700 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px"><option value="">All programmes</option><option>BCS</option><option>BIT</option><option>BBA</option><option>BME</option><option>BDS</option></select>
                    <button type="button" data-table-clear class="text-slate-500 hover:text-slate-900 px-2" style="font-size:12.5px;font-weight:600">Clear</button>
                </div>
            </div>

            <div class="overflow-x-auto">
                <table class="min-w-full">
                    <thead class="border-y border-slate-100 bg-slate-50/60 text-slate-500">
                        <tr>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">User ID</th>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Full Name</th>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Email</th>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Phone</th>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Role</th>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Status</th>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Created</th>
                            <th class="px-6 py-3 text-right text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr data-row data-search="usr-10241 lee hui min lee.huimin@student.newinti.edu.my" data-role="Student" data-status="Active" data-programme="BCS" class="border-b border-slate-100 hover:bg-slate-50/60 transition-colors">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">USR-10241</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Lee Hui Min</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">lee.huimin@student.newinti.edu.my</td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">+60 12-345 6789</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">Student</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Active</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">12 Jan 2025</td>
                            <td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-drawer-open="view-user" title="View" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="eye" class="h-3.5 w-3.5"></i></button><button type="button" data-modal-open="edit-user" title="Edit" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Reset password?" data-confirm-message="We'll email a password reset link to lee.huimin@student.newinti.edu.my." data-confirm-label="Send Reset Link" data-confirm-toast="Password reset link sent" title="Reset password" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="key-round" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Deactivate user?" data-confirm-message="Lee Hui Min will lose access immediately." data-confirm-label="Deactivate" data-confirm-danger="true" data-confirm-toast="User deactivated" title="Deactivate" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="power" class="h-3.5 w-3.5"></i></button></div></td>
                        </tr>
                        <tr data-row data-search="usr-10242 nur aisyah binti hamzah n.aisyah@inti.edu.my" data-role="Lecturer" data-status="Active" data-programme="BIT" class="border-b border-slate-100 hover:bg-slate-50/60 transition-colors">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">USR-10242</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Nur Aisyah Binti Hamzah</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">n.aisyah@inti.edu.my</td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">+60 13-998 2210</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-sky-50 text-sky-700 border-sky-100" style="font-size:11.5px;font-weight:600">Lecturer</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Active</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">08 Feb 2025</td>
                            <td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-drawer-open="view-user" title="View" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="eye" class="h-3.5 w-3.5"></i></button><button type="button" data-modal-open="edit-user" title="Edit" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Reset password?" data-confirm-message="We'll email a password reset link to n.aisyah@inti.edu.my." data-confirm-label="Send Reset Link" data-confirm-toast="Password reset link sent" title="Reset password" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="key-round" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Deactivate user?" data-confirm-message="Nur Aisyah Binti Hamzah will lose access immediately." data-confirm-label="Deactivate" data-confirm-danger="true" data-confirm-toast="User deactivated" title="Deactivate" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="power" class="h-3.5 w-3.5"></i></button></div></td>
                        </tr>
                        <tr data-row data-search="usr-10243 farah diana farah.diana@student.newinti.edu.my" data-role="Student" data-status="Active" data-programme="BBA" class="border-b border-slate-100 hover:bg-slate-50/60 transition-colors">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">USR-10243</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Farah Diana</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">farah.diana@student.newinti.edu.my</td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">+60 17-441 3322</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">Student</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Active</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">21 Mar 2025</td>
                            <td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-drawer-open="view-user" title="View" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="eye" class="h-3.5 w-3.5"></i></button><button type="button" data-modal-open="edit-user" title="Edit" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Reset password?" data-confirm-message="We'll email a password reset link to farah.diana@student.newinti.edu.my." data-confirm-label="Send Reset Link" data-confirm-toast="Password reset link sent" title="Reset password" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="key-round" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Deactivate user?" data-confirm-message="Farah Diana will lose access immediately." data-confirm-label="Deactivate" data-confirm-danger="true" data-confirm-toast="User deactivated" title="Deactivate" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="power" class="h-3.5 w-3.5"></i></button></div></td>
                        </tr>
                        <tr data-row data-search="usr-10244 dr. tan mei ling ml.tan@inti.edu.my" data-role="Lecturer" data-status="Active" data-programme="BCS" class="border-b border-slate-100 hover:bg-slate-50/60 transition-colors">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">USR-10244</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Dr. Tan Mei Ling</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">ml.tan@inti.edu.my</td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">+60 11-220 7788</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-sky-50 text-sky-700 border-sky-100" style="font-size:11.5px;font-weight:600">Lecturer</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Active</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">02 Apr 2025</td>
                            <td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-drawer-open="view-user" title="View" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="eye" class="h-3.5 w-3.5"></i></button><button type="button" data-modal-open="edit-user" title="Edit" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Reset password?" data-confirm-message="We'll email a password reset link to ml.tan@inti.edu.my." data-confirm-label="Send Reset Link" data-confirm-toast="Password reset link sent" title="Reset password" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="key-round" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Deactivate user?" data-confirm-message="Dr. Tan Mei Ling will lose access immediately." data-confirm-label="Deactivate" data-confirm-danger="true" data-confirm-toast="User deactivated" title="Deactivate" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="power" class="h-3.5 w-3.5"></i></button></div></td>
                        </tr>
                        <tr data-row data-search="usr-10245 tan mei ling tan.meiling@student.newinti.edu.my" data-role="Student" data-status="Active" data-programme="BCS" class="border-b border-slate-100 hover:bg-slate-50/60 transition-colors">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">USR-10245</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Tan Mei Ling</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">tan.meiling@student.newinti.edu.my</td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">+60 19-887 6210</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">Student</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Active</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">16 Apr 2025</td>
                            <td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-drawer-open="view-user" title="View" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="eye" class="h-3.5 w-3.5"></i></button><button type="button" data-modal-open="edit-user" title="Edit" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Reset password?" data-confirm-message="We'll email a password reset link to tan.meiling@student.newinti.edu.my." data-confirm-label="Send Reset Link" data-confirm-toast="Password reset link sent" title="Reset password" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="key-round" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Deactivate user?" data-confirm-message="Tan Mei Ling will lose access immediately." data-confirm-label="Deactivate" data-confirm-danger="true" data-confirm-toast="User deactivated" title="Deactivate" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="power" class="h-3.5 w-3.5"></i></button></div></td>
                        </tr>
                        <tr data-row data-search="usr-10246 vivek sharma vivek.sharma@student.newinti.edu.my" data-role="Student" data-status="Active" data-programme="BDS" class="border-b border-slate-100 hover:bg-slate-50/60 transition-colors">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">USR-10246</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Vivek Sharma</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">vivek.sharma@student.newinti.edu.my</td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">+60 14-557 1199</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">Student</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Active</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">30 Apr 2025</td>
                            <td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-drawer-open="view-user" title="View" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="eye" class="h-3.5 w-3.5"></i></button><button type="button" data-modal-open="edit-user" title="Edit" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Reset password?" data-confirm-message="We'll email a password reset link to vivek.sharma@student.newinti.edu.my." data-confirm-label="Send Reset Link" data-confirm-toast="Password reset link sent" title="Reset password" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="key-round" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Deactivate user?" data-confirm-message="Vivek Sharma will lose access immediately." data-confirm-label="Deactivate" data-confirm-danger="true" data-confirm-toast="User deactivated" title="Deactivate" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="power" class="h-3.5 w-3.5"></i></button></div></td>
                        </tr>
                        <tr data-row data-search="usr-10247 prof. dr. hadi yusof hadi.y@inti.edu.my" data-role="Lecturer" data-status="Active" data-programme="BCS" class="border-b border-slate-100 hover:bg-slate-50/60 transition-colors">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">USR-10247</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Prof. Dr. Hadi Yusof</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">hadi.y@inti.edu.my</td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">+60 12-100 4567</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-sky-50 text-sky-700 border-sky-100" style="font-size:11.5px;font-weight:600">Lecturer</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Active</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">11 May 2025</td>
                            <td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-drawer-open="view-user" title="View" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="eye" class="h-3.5 w-3.5"></i></button><button type="button" data-modal-open="edit-user" title="Edit" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Reset password?" data-confirm-message="We'll email a password reset link to hadi.y@inti.edu.my." data-confirm-label="Send Reset Link" data-confirm-toast="Password reset link sent" title="Reset password" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="key-round" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Deactivate user?" data-confirm-message="Prof. Dr. Hadi Yusof will lose access immediately." data-confirm-label="Deactivate" data-confirm-danger="true" data-confirm-toast="User deactivated" title="Deactivate" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="power" class="h-3.5 w-3.5"></i></button></div></td>
                        </tr>
                        <tr data-row data-search="usr-10248 ahmad faizal ahmad.faizal@student.newinti.edu.my" data-role="Student" data-status="Active" data-programme="BIT" class="border-b border-slate-100 hover:bg-slate-50/60 transition-colors">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">USR-10248</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Ahmad Faizal</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">ahmad.faizal@student.newinti.edu.my</td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">+60 16-220 4411</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">Student</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Active</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">18 May 2025</td>
                            <td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-drawer-open="view-user" title="View" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="eye" class="h-3.5 w-3.5"></i></button><button type="button" data-modal-open="edit-user" title="Edit" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Reset password?" data-confirm-message="We'll email a password reset link to ahmad.faizal@student.newinti.edu.my." data-confirm-label="Send Reset Link" data-confirm-toast="Password reset link sent" title="Reset password" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="key-round" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Deactivate user?" data-confirm-message="Ahmad Faizal will lose access immediately." data-confirm-label="Deactivate" data-confirm-danger="true" data-confirm-toast="User deactivated" title="Deactivate" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="power" class="h-3.5 w-3.5"></i></button></div></td>
                        </tr>
                        <tr data-row data-search="usr-10249 choo kah yan choo.kahyan@student.newinti.edu.my" data-role="Student" data-status="Pending" data-programme="BBA" class="border-b border-slate-100 hover:bg-slate-50/60 transition-colors">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">USR-10249</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Choo Kah Yan</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">choo.kahyan@student.newinti.edu.my</td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">+60 18-339 5522</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">Student</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-amber-50 text-amber-700 border-amber-100" style="font-size:11.5px;font-weight:600">Pending</span></td>
                            <td class="px-6 py-3 text-slate-700" style="font-size:12.5px">22 May 2025</td>
                            <td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-drawer-open="view-user" title="View" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="eye" class="h-3.5 w-3.5"></i></button><button type="button" data-modal-open="edit-user" title="Edit" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Reset password?" data-confirm-message="We'll email a password reset link to choo.kahyan@student.newinti.edu.my." data-confirm-label="Send Reset Link" data-confirm-toast="Password reset link sent" title="Reset password" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="key-round" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Activate user?" data-confirm-message="Choo Kah Yan will regain access immediately." data-confirm-label="Activate" data-confirm-toast="User activated" title="Activate" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="power" class="h-3.5 w-3.5"></i></button></div></td>
                        </tr>
                        <tr data-table-empty style="display:none"><td colspan="8" class="px-6 py-16 text-center text-slate-500" style="font-size:13px">No users match your filters.</td></tr>
                    </tbody>
                </table>
            </div>

            <div class="flex items-center justify-between px-6 py-3 text-slate-500" style="font-size:12.5px">
                <span data-table-info></span>
                <div class="flex items-center gap-1" data-table-pager></div>
            </div>
        </div>
    </section>

    <%-- Create User modal --%>
    <div id="create-user" data-modal class="fixed inset-0 z-[60] items-center justify-center p-4" style="display:none">
        <div data-modal-backdrop class="absolute inset-0 bg-slate-900/40 backdrop-blur-sm"></div>
        <div class="relative w-full max-w-2xl max-h-[90vh] overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-xl flex flex-col">
            <div class="flex items-start justify-between gap-4 border-b border-slate-100 px-6 py-4">
                <div><h2 class="text-slate-900" style="font-size:17px;font-weight:700;letter-spacing:-0.01em">Create User</h2><p class="mt-0.5 text-slate-500" style="font-size:13px">Add a new student or lecturer account.</p></div>
                <button type="button" data-modal-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-700"><i data-lucide="x" class="h-4 w-4"></i></button>
            </div>
            <div class="flex-1 overflow-y-auto px-6 py-5">
                <div style="display:grid;grid-template-columns:repeat(auto-fit,minmax(240px,1fr));gap:16px">
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Full Name</span><div class="mt-1.5"><input placeholder="e.g. Lim Wei Jian" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 placeholder:text-slate-400 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Email</span><div class="mt-1.5"><input type="email" placeholder="name@inti.edu.my" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 placeholder:text-slate-400 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Phone</span><div class="mt-1.5"><input placeholder="+60 12-345 6789" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 placeholder:text-slate-400 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Role</span><div class="mt-1.5"><select class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><option>Student</option><option>Lecturer</option></select></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Programme / Department</span><div class="mt-1.5"><select class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><option>BCS</option><option>BIT</option><option>BBA</option><option>BME</option><option>BAC</option><option>BDS</option></select></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Status</span><div class="mt-1.5"><select class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><option>Active</option><option>Pending</option><option>Inactive</option></select></div></label>
                </div>
                <div class="mt-5"><label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Initial Notes (optional)</span><div class="mt-1.5"><textarea placeholder="Onboarding notes, advisor assignment, etc." class="w-full min-h-[88px] rounded-md border border-slate-200 bg-white px-3 py-2 text-slate-900 placeholder:text-slate-400 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px;line-height:1.5"></textarea></div></label></div>
            </div>
            <div class="flex items-center justify-end gap-2 border-t border-slate-100 px-6 py-4 bg-slate-50/40">
                <button type="button" data-modal-close class="inline-flex items-center rounded-md px-4 h-10 text-slate-600 hover:bg-slate-100" style="font-size:13px;font-weight:600">Cancel</button>
                <button type="button" data-modal-close data-toast="User created" data-toast-desc="New account added." class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]" style="font-size:13px;font-weight:600">Create User</button>
            </div>
        </div>
    </div>

    <%-- Edit User modal --%>
    <div id="edit-user" data-modal class="fixed inset-0 z-[60] items-center justify-center p-4" style="display:none">
        <div data-modal-backdrop class="absolute inset-0 bg-slate-900/40 backdrop-blur-sm"></div>
        <div class="relative w-full max-w-2xl max-h-[90vh] overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-xl flex flex-col">
            <div class="flex items-start justify-between gap-4 border-b border-slate-100 px-6 py-4">
                <div><h2 class="text-slate-900" style="font-size:17px;font-weight:700;letter-spacing:-0.01em">Edit User</h2><p class="mt-0.5 text-slate-500" style="font-size:13px">Update account details.</p></div>
                <button type="button" data-modal-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-700"><i data-lucide="x" class="h-4 w-4"></i></button>
            </div>
            <div class="flex-1 overflow-y-auto px-6 py-5">
                <div style="display:grid;grid-template-columns:repeat(auto-fit,minmax(240px,1fr));gap:16px">
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Full Name</span><div class="mt-1.5"><input value="Lee Hui Min" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Email</span><div class="mt-1.5"><input type="email" value="lee.huimin@student.newinti.edu.my" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Phone</span><div class="mt-1.5"><input value="+60 12-345 6789" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Role</span><div class="mt-1.5"><select class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><option>Student</option><option>Lecturer</option></select></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Programme / Department</span><div class="mt-1.5"><select class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><option>BCS</option><option>BIT</option><option>BBA</option><option>BME</option><option>BAC</option><option>BDS</option></select></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Status</span><div class="mt-1.5"><select class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><option>Active</option><option>Pending</option><option>Inactive</option></select></div></label>
                </div>
            </div>
            <div class="flex items-center justify-end gap-2 border-t border-slate-100 px-6 py-4 bg-slate-50/40">
                <button type="button" data-modal-close class="inline-flex items-center rounded-md px-4 h-10 text-slate-600 hover:bg-slate-100" style="font-size:13px;font-weight:600">Cancel</button>
                <button type="button" data-modal-close data-toast="User updated" class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]" style="font-size:13px;font-weight:600">Save Changes</button>
            </div>
        </div>
    </div>

    <%-- View User drawer --%>
    <div id="view-user" data-drawer class="fixed inset-0 z-[60]" style="display:none">
        <div data-drawer-backdrop class="absolute inset-0 bg-slate-900/40"></div>
        <div data-drawer-panel class="absolute right-0 top-0 h-full w-full max-w-xl bg-white shadow-2xl border-l border-slate-200 flex flex-col">
            <div class="flex items-start justify-between gap-4 border-b border-slate-100 px-6 py-4">
                <div><h2 class="text-slate-900" style="font-size:18px;font-weight:700;letter-spacing:-0.01em">Lee Hui Min</h2><p class="mt-0.5 text-slate-500" style="font-size:13px">USR-10241</p></div>
                <button type="button" data-drawer-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-700"><i data-lucide="x" class="h-4 w-4"></i></button>
            </div>
            <div class="flex-1 overflow-y-auto px-6 py-5">
                <div class="space-y-5">
                    <div class="flex items-center gap-3">
                        <div class="flex h-12 w-12 items-center justify-center rounded-xl bg-[#e0162b]/10 text-[#a01020]" style="font-size:16px;font-weight:700">LH</div>
                        <div><div class="text-slate-900" style="font-size:16px;font-weight:700">Lee Hui Min</div><div class="text-slate-500" style="font-size:13px">Student &middot; BCS</div></div>
                    </div>
                    <div class="grid gap-3">
                        <div class="flex items-center gap-3 rounded-xl border border-slate-200 px-4 py-3"><i data-lucide="id-card" class="h-4 w-4 text-slate-400"></i><div class="flex-1"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">User ID</div><div class="text-slate-900" style="font-size:13.5px">USR-10241</div></div></div>
                        <div class="flex items-center gap-3 rounded-xl border border-slate-200 px-4 py-3"><i data-lucide="mail" class="h-4 w-4 text-slate-400"></i><div class="flex-1"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">Email</div><div class="text-slate-900" style="font-size:13.5px">lee.huimin@student.newinti.edu.my</div></div></div>
                        <div class="flex items-center gap-3 rounded-xl border border-slate-200 px-4 py-3"><i data-lucide="phone" class="h-4 w-4 text-slate-400"></i><div class="flex-1"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">Phone</div><div class="text-slate-900" style="font-size:13.5px">+60 12-345 6789</div></div></div>
                        <div class="flex items-center gap-3 rounded-xl border border-slate-200 px-4 py-3"><i data-lucide="briefcase" class="h-4 w-4 text-slate-400"></i><div class="flex-1"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">Programme</div><div class="text-slate-900" style="font-size:13.5px">BCS</div></div></div>
                    </div>
                    <div class="rounded-xl border border-slate-200 p-4"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">Status</div><div class="mt-1"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Active</span></div></div>
                </div>
            </div>
            <div class="flex items-center justify-end gap-2 border-t border-slate-100 px-6 py-4 bg-slate-50/40">
                <a href="<%= ResolveUrl("~/admin/student_detail.aspx?id=S12101") %>" class="inline-flex items-center gap-1.5 rounded-md border border-slate-200 bg-white px-3 h-10 text-slate-700 hover:bg-slate-50 transition-colors" style="font-size:13px;font-weight:600">View Student Profile</a>
                <button type="button" data-drawer-close class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]" style="font-size:13px;font-weight:600">Close</button>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/admin/shared/icons.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/toast.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/table.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/ui.js") %>"></script>
</asp:Content>
