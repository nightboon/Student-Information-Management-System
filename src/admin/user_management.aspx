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
                    <button type="button" data-toast="Use Reports to generate downloadable files" data-toast-type="info" class="flex w-full items-center gap-2 rounded-md px-3 py-2 text-left text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:500"><i data-lucide="file-text" class="h-4 w-4 text-[#e0162b]"></i> Export as PDF</button>
                    <button type="button" data-toast="Use Reports to generate downloadable files" data-toast-type="info" class="flex w-full items-center gap-2 rounded-md px-3 py-2 text-left text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:500"><i data-lucide="file-spreadsheet" class="h-4 w-4 text-emerald-600"></i> Export as Excel</button>
                </div>
            </div>
            <button type="button" data-modal-open="create-user" class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]" style="font-size:13px;font-weight:600">
                <i data-lucide="user-plus" class="h-4 w-4"></i> Create User
            </button>
        </div>
    </div>

    <%-- Summary cards --%>
    <section class="mt-6" style="display:grid;grid-template-columns:repeat(auto-fit,minmax(220px,1fr));gap:16px">
        <div class="rounded-2xl border border-slate-200 bg-white p-5"><div class="text-slate-500" style="font-size:12.5px;font-weight:500">Total Users</div><div class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= Number(TotalUsers) %></div><div class="mt-1 text-slate-400" style="font-size:12px">all accounts</div></div>
        <div class="rounded-2xl border border-slate-200 bg-white p-5"><div class="text-slate-500" style="font-size:12.5px;font-weight:500">Students</div><div class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= Number(StudentUsers) %></div><div class="mt-1 text-slate-400" style="font-size:12px">enrolled</div></div>
        <div class="rounded-2xl border border-slate-200 bg-white p-5"><div class="text-slate-500" style="font-size:12.5px;font-weight:500">Lecturers</div><div class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= Number(LecturerUsers) %></div><div class="mt-1 text-slate-400" style="font-size:12px">teaching staff</div></div>
        <div class="rounded-2xl border border-slate-200 bg-white p-5"><div class="text-slate-500" style="font-size:12.5px;font-weight:500">Active Accounts</div><div class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em"><%= Number(ActiveUsers) %></div><div class="mt-1 text-slate-400" style="font-size:12px">last 30 days</div></div>
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
                        <%= UserRowsHtml %>
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
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Full Name</span><div class="mt-1.5"><input data-field="fullName" placeholder="e.g. Lim Wei Jian" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 placeholder:text-slate-400 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Email</span><div class="mt-1.5"><input data-field="email" type="email" placeholder="name@inti.edu.my" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 placeholder:text-slate-400 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Phone</span><div class="mt-1.5"><input data-field="phone" placeholder="+60 12-345 6789" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 placeholder:text-slate-400 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Role</span><div class="mt-1.5"><select data-field="role" disabled class="h-10 w-full rounded-md border border-slate-200 bg-slate-50 px-3 text-slate-700 outline-none" style="font-size:13px"><option>Student</option><option>Lecturer</option></select></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Programme / Department</span><div class="mt-1.5"><select data-field="programmeOrDepartment" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><option>BCS</option><option>BIT</option><option>BBA</option><option>BME</option><option>BAC</option><option>BDS</option></select></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Status</span><div class="mt-1.5"><select data-field="status" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><option>Active</option><option>Pending</option><option>Inactive</option></select></div></label>
                </div>
                <div class="mt-5"><label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Initial Notes (optional)</span><div class="mt-1.5"><textarea placeholder="Onboarding notes, advisor assignment, etc." class="w-full min-h-[88px] rounded-md border border-slate-200 bg-white px-3 py-2 text-slate-900 placeholder:text-slate-400 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px;line-height:1.5"></textarea></div></label></div>
            </div>
            <div class="flex items-center justify-end gap-2 border-t border-slate-100 px-6 py-4 bg-slate-50/40">
                <button type="button" data-modal-close class="inline-flex items-center rounded-md px-4 h-10 text-slate-600 hover:bg-slate-100" style="font-size:13px;font-weight:600">Cancel</button>
                <button type="button" data-modal-close data-admin-create-user data-toast="User created" data-toast-desc="New account added." class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]" style="font-size:13px;font-weight:600">Create User</button>
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
                <input type="hidden" data-field="userId" />
                <div style="display:grid;grid-template-columns:repeat(auto-fit,minmax(240px,1fr));gap:16px">
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Full Name</span><div class="mt-1.5"><input data-field="fullName" value="<%= FirstUserName %>" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Email</span><div class="mt-1.5"><input data-field="email" type="email" value="<%= FirstUserEmail %>" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Phone</span><div class="mt-1.5"><input data-field="phone" value="<%= FirstUserPhone %>" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Role</span><div class="mt-1.5"><select data-field="role" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><option>Student</option><option>Lecturer</option></select></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Programme / Department</span><div class="mt-1.5"><select data-field="programmeOrDepartment" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><option>BCS</option><option>BIT</option><option>BBA</option><option>BME</option><option>BAC</option><option>BDS</option></select></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Status</span><div class="mt-1.5"><select data-field="status" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><option>Active</option><option>Pending</option><option>Inactive</option></select></div></label>
                </div>
            </div>
            <div class="flex items-center justify-end gap-2 border-t border-slate-100 px-6 py-4 bg-slate-50/40">
                <button type="button" data-modal-close class="inline-flex items-center rounded-md px-4 h-10 text-slate-600 hover:bg-slate-100" style="font-size:13px;font-weight:600">Cancel</button>
                <button type="button" data-modal-close data-admin-update-user data-toast="User updated" class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]" style="font-size:13px;font-weight:600">Save Changes</button>
            </div>
        </div>
    </div>

    <%-- View User drawer --%>
    <div id="view-user" data-drawer class="fixed inset-0 z-[60]" style="display:none">
        <div data-drawer-backdrop class="absolute inset-0 bg-slate-900/40"></div>
        <div data-drawer-panel class="absolute right-0 top-0 h-full w-full max-w-xl bg-white shadow-2xl border-l border-slate-200 flex flex-col">
            <div class="flex items-start justify-between gap-4 border-b border-slate-100 px-6 py-4">
                <div><h2 data-view-field="fullName" class="text-slate-900" style="font-size:18px;font-weight:700;letter-spacing:-0.01em"></h2><p data-view-field="profileId" class="mt-0.5 text-slate-500" style="font-size:13px"></p></div>
                <button type="button" data-drawer-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-700"><i data-lucide="x" class="h-4 w-4"></i></button>
            </div>
            <div class="flex-1 overflow-y-auto px-6 py-5">
                <div class="space-y-5">
                    <div class="flex items-center gap-3">
                        <div data-view-field="initials" class="flex h-12 w-12 items-center justify-center rounded-xl bg-[#e0162b]/10 text-[#a01020]" style="font-size:16px;font-weight:700"></div>
                        <div><div data-view-field="fullName" class="text-slate-900" style="font-size:16px;font-weight:700"></div><div class="text-slate-500" style="font-size:13px"><span data-view-field="role"></span> &middot; <span data-view-field="programme"></span></div></div>
                    </div>
                    <div class="grid gap-3">
                        <div class="flex items-center gap-3 rounded-xl border border-slate-200 px-4 py-3"><i data-lucide="id-card" class="h-4 w-4 text-slate-400"></i><div class="flex-1"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">User ID</div><div data-view-field="profileId" class="text-slate-900" style="font-size:13.5px"></div></div></div>
                        <div class="flex items-center gap-3 rounded-xl border border-slate-200 px-4 py-3"><i data-lucide="mail" class="h-4 w-4 text-slate-400"></i><div class="flex-1"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">Email</div><div data-view-field="email" class="text-slate-900" style="font-size:13.5px"></div></div></div>
                        <div class="flex items-center gap-3 rounded-xl border border-slate-200 px-4 py-3"><i data-lucide="phone" class="h-4 w-4 text-slate-400"></i><div class="flex-1"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">Phone</div><div data-view-field="phone" class="text-slate-900" style="font-size:13.5px"></div></div></div>
                        <div class="flex items-center gap-3 rounded-xl border border-slate-200 px-4 py-3"><i data-lucide="briefcase" class="h-4 w-4 text-slate-400"></i><div class="flex-1"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">Programme</div><div data-view-field="programme" class="text-slate-900" style="font-size:13.5px"></div></div></div>
                    </div>
                    <div class="rounded-xl border border-slate-200 p-4"><div class="text-slate-500" style="font-size:11.5px;font-weight:600">Status</div><div class="mt-1"><span data-view-field="status" class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-700 border-slate-200" style="font-size:11.5px;font-weight:600"></span></div></div>
                </div>
            </div>
            <div class="flex items-center justify-end gap-2 border-t border-slate-100 px-6 py-4 bg-slate-50/40">
                <a data-view-profile-link href="#" class="inline-flex items-center gap-1.5 rounded-md border border-slate-200 bg-white px-3 h-10 text-slate-700 hover:bg-slate-50 transition-colors" style="font-size:13px;font-weight:600">View Student Profile</a>
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
    <script>
      (function () {
        function post(method, payload) {
          return fetch("user_management.aspx/" + method, {
            method: "POST",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            credentials: "same-origin",
            body: JSON.stringify(payload || {})
          }).then(function (r) {
            if (!r.ok) throw new Error("Request failed");
            return r.json();
          });
        }
        function field(modal, name) {
          var el = modal.querySelector('[data-field="' + name + '"]');
          return el ? el.value.trim() : "";
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
        function reloadSuccess(message) {
          if (window.toast) window.toast.success(message);
          setTimeout(function () { location.reload(); }, 450);
        }

        document.addEventListener("click", function (e) {
          var view = e.target.closest("[data-admin-view-user]");
          if (view) {
            var viewRow = view.closest("tr");
            var drawer = document.getElementById("view-user");
            var values = {
              fullName: viewRow && viewRow.dataset.fullName,
              profileId: viewRow && viewRow.dataset.profileId,
              email: viewRow && viewRow.dataset.email,
              phone: viewRow && viewRow.dataset.phone,
              role: viewRow && viewRow.dataset.role,
              programme: viewRow && viewRow.dataset.programme,
              status: viewRow && viewRow.dataset.status
            };
            Object.keys(values).forEach(function (name) {
              drawer.querySelectorAll('[data-view-field="' + name + '"]').forEach(function (el) {
                el.textContent = values[name] || "-";
              });
            });
            var words = (values.fullName || "").trim().split(/\s+/);
            var initials = words.length ? words[0].charAt(0) + (words.length > 1 ? words[words.length - 1].charAt(0) : "") : "U";
            drawer.querySelector('[data-view-field="initials"]').textContent = initials.toUpperCase();
            var profileLink = drawer.querySelector("[data-view-profile-link]");
            if (values.role === "Student") {
              profileLink.style.display = "";
              profileLink.href = "student_detail.aspx?id=" + encodeURIComponent(values.profileId || "");
            } else {
              profileLink.style.display = "none";
              profileLink.href = "#";
            }
            return;
          }

          var edit = e.target.closest("[data-admin-edit-user]");
          if (edit) {
            var row = edit.closest("tr");
            var editModal = document.getElementById("edit-user");
            setField(editModal, "userId", row && row.dataset.userId);
            setField(editModal, "fullName", row && row.dataset.fullName);
            setField(editModal, "email", row && row.dataset.email);
            setField(editModal, "phone", row && row.dataset.phone);
            setField(editModal, "role", row && row.dataset.role);
            setField(editModal, "programmeOrDepartment", row && row.dataset.programme);
            setField(editModal, "status", row && row.dataset.status);
            return;
          }

          var create = e.target.closest("[data-admin-create-user]");
          if (create) {
            e.preventDefault();
            e.stopImmediatePropagation();
            var modal = document.getElementById("create-user");
            var request = {
              fullName: field(modal, "fullName"),
              email: field(modal, "email"),
              phone: field(modal, "phone"),
              role: field(modal, "role"),
              programmeOrDepartment: field(modal, "programmeOrDepartment"),
              status: field(modal, "status")
            };
            post("CreateUser", { request: request })
              .then(function () { reloadSuccess("User created"); })
              .catch(function () { if (window.toast) window.toast.error("Could not create user"); });
            return;
          }

          var update = e.target.closest("[data-admin-update-user]");
          if (update) {
            e.preventDefault();
            e.stopImmediatePropagation();
            var updateModal = document.getElementById("edit-user");
            var updateRequest = {
              userId: parseInt(field(updateModal, "userId"), 10),
              fullName: field(updateModal, "fullName"),
              email: field(updateModal, "email"),
              phone: field(updateModal, "phone"),
              role: field(updateModal, "role"),
              programmeOrDepartment: field(updateModal, "programmeOrDepartment"),
              status: field(updateModal, "status")
            };
            post("UpdateUser", { request: updateRequest })
              .then(function () { reloadSuccess("User updated"); })
              .catch(function () { if (window.toast) window.toast.error("Could not update user"); });
            return;
          }

          var status = e.target.closest("[data-admin-status]");
          if (status) {
            e.preventDefault();
            e.stopImmediatePropagation();
            var next = status.getAttribute("data-next-status");
            var name = status.getAttribute("data-user-name") || "this user";
            if (!confirm((next === "ACTIVE" ? "Activate " : "Deactivate ") + name + "?")) return;
            post("SetUserStatus", { userId: parseInt(status.getAttribute("data-user-id"), 10), status: next })
              .then(function () { reloadSuccess("User updated"); })
              .catch(function () { if (window.toast) window.toast.error("Could not update user"); });
          }
        }, true);
      })();
    </script>
</asp:Content>




