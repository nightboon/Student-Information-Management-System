<%@ Page Language="C#" MasterPageFile="~/admin/AdminLayout.master" AutoEventWireup="true" CodeBehind="add_drop_requests.aspx.cs" Inherits="src.admin.add_drop_requests" Title="Add/Drop Requests - INTI Admin Portal" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <div class="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Admin</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">Add/Drop Requests</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">Review and resolve course add and drop requests submitted by students.</p>
        </div>
        <div class="flex flex-wrap items-center gap-2">
            <div class="relative" data-dropdown>
                <button type="button" data-dropdown-toggle class="inline-flex items-center gap-1.5 rounded-md border border-slate-200 bg-white px-3 h-10 text-slate-700 hover:bg-slate-50 transition-colors" style="font-size:13px;font-weight:600">
                    <i data-lucide="download" class="h-4 w-4"></i> Export <i data-lucide="chevron-down" class="h-3.5 w-3.5"></i>
                </button>
                <div data-dropdown-menu style="display:none" class="absolute right-0 z-20 mt-2 w-48 rounded-xl border border-slate-200 bg-white p-1 shadow-lg">
                    <button type="button" data-toast="PDF export started" data-toast-desc="requests queued for download" class="flex w-full items-center gap-2 rounded-md px-3 py-2 text-left text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:500"><i data-lucide="file-text" class="h-4 w-4 text-[#e0162b]"></i> Export as PDF</button>
                    <button type="button" data-toast="Excel export started" data-toast-desc="requests queued for download" class="flex w-full items-center gap-2 rounded-md px-3 py-2 text-left text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:500"><i data-lucide="file-spreadsheet" class="h-4 w-4 text-emerald-600"></i> Export as Excel</button>
                </div>
            </div>
        </div>
    </div>

    <%-- KPI cards --%>
    <section class="mt-6" style="display:grid;grid-template-columns:repeat(auto-fit,minmax(200px,1fr));gap:16px">
        <div class="rounded-2xl border border-slate-200 bg-white p-4 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-center justify-between"><div class="text-slate-500" style="font-size:11.5px;font-weight:500">Pending</div><span class="inline-flex h-7 w-7 items-center justify-center rounded-lg bg-amber-50 text-amber-600"><i data-lucide="clock" class="h-4 w-4"></i></span></div>
            <div class="mt-1 text-amber-600" style="font-size:24px;font-weight:700;letter-spacing:-0.01em">4</div>
        </div>
        <div class="rounded-2xl border border-slate-200 bg-white p-4 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-center justify-between"><div class="text-slate-500" style="font-size:11.5px;font-weight:500">Approved</div><span class="inline-flex h-7 w-7 items-center justify-center rounded-lg bg-emerald-50 text-emerald-600"><i data-lucide="check-circle-2" class="h-4 w-4"></i></span></div>
            <div class="mt-1 text-emerald-600" style="font-size:24px;font-weight:700;letter-spacing:-0.01em">2</div>
        </div>
        <div class="rounded-2xl border border-slate-200 bg-white p-4 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-center justify-between"><div class="text-slate-500" style="font-size:11.5px;font-weight:500">Rejected</div><span class="inline-flex h-7 w-7 items-center justify-center rounded-lg bg-[#e0162b]/10 text-[#a01020]"><i data-lucide="x-circle" class="h-4 w-4"></i></span></div>
            <div class="mt-1 text-[#a01020]" style="font-size:24px;font-weight:700;letter-spacing:-0.01em">1</div>
        </div>
        <div class="rounded-2xl border border-slate-200 bg-white p-4 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-center justify-between"><div class="text-slate-500" style="font-size:11.5px;font-weight:500">Total Requests</div><span class="inline-flex h-7 w-7 items-center justify-center rounded-lg bg-slate-100 text-slate-500"><i data-lucide="file-edit" class="h-4 w-4"></i></span></div>
            <div class="mt-1 text-slate-900" style="font-size:24px;font-weight:700;letter-spacing:-0.01em">7</div>
        </div>
    </section>

    <section class="mt-6 rounded-lg border border-slate-200 bg-white">
        <div data-table data-page-size="20">
            <div class="flex flex-col gap-3 px-6 py-4 lg:flex-row lg:items-center lg:justify-between">
                <div class="relative w-full lg:max-w-sm">
                    <svg viewBox="0 0 24 24" class="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="7" /><path d="m21 21-4.3-4.3" /></svg>
                    <input data-table-search placeholder="Search student, ID, course or request&hellip;" class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 text-slate-900 placeholder:text-slate-400 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px" />
                </div>
                <div class="flex flex-wrap items-center gap-2">
                    <select data-table-filter="programme" class="h-9 rounded-md border border-slate-200 bg-white px-3 text-slate-700 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px"><option value="">All programmes</option><option>BCS</option><option>BIT</option><option>BBA</option><option>BME</option><option>BDS</option></select>
                    <div data-table-chip="type" class="inline-flex items-center rounded-md border border-slate-200 bg-white p-0.5">
                        <button type="button" data-chip="All" data-active="true" class="rounded px-2.5 py-1 text-slate-600 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020]" style="font-size:11.5px;font-weight:600">All</button>
                        <button type="button" data-chip="Add" class="rounded px-2.5 py-1 text-slate-600 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020]" style="font-size:11.5px;font-weight:600">Add</button>
                        <button type="button" data-chip="Drop" class="rounded px-2.5 py-1 text-slate-600 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020]" style="font-size:11.5px;font-weight:600">Drop</button>
                    </div>
                    <div data-table-chip="status" class="inline-flex items-center rounded-md border border-slate-200 bg-white p-0.5">
                        <button type="button" data-chip="All" data-active="true" class="rounded px-2.5 py-1 text-slate-600 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020]" style="font-size:11.5px;font-weight:600">All</button>
                        <button type="button" data-chip="Pending" class="rounded px-2.5 py-1 text-slate-600 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020]" style="font-size:11.5px;font-weight:600">Pending</button>
                        <button type="button" data-chip="Approved" class="rounded px-2.5 py-1 text-slate-600 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020]" style="font-size:11.5px;font-weight:600">Approved</button>
                        <button type="button" data-chip="Rejected" class="rounded px-2.5 py-1 text-slate-600 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020]" style="font-size:11.5px;font-weight:600">Rejected</button>
                    </div>
                </div>
            </div>

            <div class="overflow-x-auto">
                <table class="min-w-full">
                    <thead class="border-y border-slate-100 bg-slate-50/60 text-slate-500">
                        <tr>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Request ID</th>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Student</th>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Programme</th>
                            <th class="px-6 py-3 text-center text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Sem</th>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Course</th>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Type</th>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Reason</th>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Submitted</th>
                            <th class="px-6 py-3 text-left text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Status</th>
                            <th class="px-6 py-3 text-right text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr data-row data-search="req-3321 lim wei jian s12039 csc2024" data-programme="BCS" data-type="Add" data-status="Pending" class="border-b border-slate-100 hover:bg-slate-50/60">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px"><span class="font-medium">REQ-3321</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><div class="text-slate-900 font-medium">Lim Wei Jian</div><div class="text-slate-500" style="font-size:11.5px">S12039</div></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">BCS</span></td>
                            <td class="px-6 py-3 text-slate-700 text-center" style="font-size:12.5px">4</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><div class="text-slate-900 font-medium">CSC2024</div><div class="text-slate-500" style="font-size:11.5px">Mobile App Dev</div></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Add</span></td>
                            <td class="px-6 py-3 max-w-xs" style="font-size:12.5px"><div class="truncate text-slate-600" title="Need for specialization track">Need for specialization track</div></td>
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">27 May 2026</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-amber-50 text-amber-700 border-amber-100" style="font-size:11.5px;font-weight:600">Pending</span></td>
                            <td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-modal-open="approve-req" class="inline-flex h-7 items-center rounded-md bg-emerald-50 border border-emerald-200 px-2 text-emerald-700 hover:bg-emerald-100" style="font-size:11.5px;font-weight:600">Approve</button><button type="button" data-modal-open="reject-req" class="inline-flex h-7 items-center rounded-md bg-[#e0162b]/10 border border-[#e0162b]/20 px-2 text-[#a01020] hover:bg-[#e0162b]/15" style="font-size:11.5px;font-weight:600">Reject</button></div></td>
                        </tr>
                        <tr data-row data-search="req-3322 nur aisyah s12040 itn3010" data-programme="BIT" data-type="Drop" data-status="Pending" class="border-b border-slate-100 hover:bg-slate-50/60">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px"><span class="font-medium">REQ-3322</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><div class="text-slate-900 font-medium">Nur Aisyah</div><div class="text-slate-500" style="font-size:11.5px">S12040</div></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">BIT</span></td>
                            <td class="px-6 py-3 text-slate-700 text-center" style="font-size:12.5px">3</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><div class="text-slate-900 font-medium">ITN3010</div><div class="text-slate-500" style="font-size:11.5px">Cloud Networking</div></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-[#e0162b]/10 text-[#a01020] border-[#e0162b]/20" style="font-size:11.5px;font-weight:600">Drop</span></td>
                            <td class="px-6 py-3 max-w-xs" style="font-size:12.5px"><div class="truncate text-slate-600" title="Workload too heavy this semester">Workload too heavy this semester</div></td>
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">27 May 2026</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-amber-50 text-amber-700 border-amber-100" style="font-size:11.5px;font-weight:600">Pending</span></td>
                            <td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-modal-open="approve-req" class="inline-flex h-7 items-center rounded-md bg-emerald-50 border border-emerald-200 px-2 text-emerald-700 hover:bg-emerald-100" style="font-size:11.5px;font-weight:600">Approve</button><button type="button" data-modal-open="reject-req" class="inline-flex h-7 items-center rounded-md bg-[#e0162b]/10 border border-[#e0162b]/20 px-2 text-[#a01020] hover:bg-[#e0162b]/15" style="font-size:11.5px;font-weight:600">Reject</button></div></td>
                        </tr>
                        <tr data-row data-search="req-3323 tan mei ling s12042 csc2030" data-programme="BCS" data-type="Add" data-status="Approved" class="border-b border-slate-100 hover:bg-slate-50/60">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px"><span class="font-medium">REQ-3323</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><div class="text-slate-900 font-medium">Tan Mei Ling</div><div class="text-slate-500" style="font-size:11.5px">S12042</div></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">BCS</span></td>
                            <td class="px-6 py-3 text-slate-700 text-center" style="font-size:12.5px">2</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><div class="text-slate-900 font-medium">CSC2030</div><div class="text-slate-500" style="font-size:11.5px">Machine Learning</div></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Add</span></td>
                            <td class="px-6 py-3 max-w-xs" style="font-size:12.5px"><div class="truncate text-slate-600" title="Prerequisite completed">Prerequisite completed</div></td>
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">26 May 2026</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Approved</span></td>
                            <td class="px-6 py-3 text-right text-slate-400" style="font-size:12px">Resolved</td>
                        </tr>
                        <tr data-row data-search="req-3324 raj kumar s12041 bba3201" data-programme="BBA" data-type="Drop" data-status="Pending" class="border-b border-slate-100 hover:bg-slate-50/60">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px"><span class="font-medium">REQ-3324</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><div class="text-slate-900 font-medium">Raj Kumar</div><div class="text-slate-500" style="font-size:11.5px">S12041</div></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">BBA</span></td>
                            <td class="px-6 py-3 text-slate-700 text-center" style="font-size:12.5px">5</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><div class="text-slate-900 font-medium">BBA3201</div><div class="text-slate-500" style="font-size:11.5px">Strategic Marketing</div></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-[#e0162b]/10 text-[#a01020] border-[#e0162b]/20" style="font-size:11.5px;font-weight:600">Drop</span></td>
                            <td class="px-6 py-3 max-w-xs" style="font-size:12.5px"><div class="truncate text-slate-600" title="Scheduling clash with elective">Scheduling clash with elective</div></td>
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">26 May 2026</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-amber-50 text-amber-700 border-amber-100" style="font-size:11.5px;font-weight:600">Pending</span></td>
                            <td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-modal-open="approve-req" class="inline-flex h-7 items-center rounded-md bg-emerald-50 border border-emerald-200 px-2 text-emerald-700 hover:bg-emerald-100" style="font-size:11.5px;font-weight:600">Approve</button><button type="button" data-modal-open="reject-req" class="inline-flex h-7 items-center rounded-md bg-[#e0162b]/10 border border-[#e0162b]/20 px-2 text-[#a01020] hover:bg-[#e0162b]/15" style="font-size:11.5px;font-weight:600">Reject</button></div></td>
                        </tr>
                        <tr data-row data-search="req-3325 daniel ong s12043 bme3010" data-programme="BME" data-type="Add" data-status="Rejected" class="border-b border-slate-100 hover:bg-slate-50/60">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px"><span class="font-medium">REQ-3325</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><div class="text-slate-900 font-medium">Daniel Ong</div><div class="text-slate-500" style="font-size:11.5px">S12043</div></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">BME</span></td>
                            <td class="px-6 py-3 text-slate-700 text-center" style="font-size:12.5px">6</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><div class="text-slate-900 font-medium">BME3010</div><div class="text-slate-500" style="font-size:11.5px">Robotics Capstone</div></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Add</span></td>
                            <td class="px-6 py-3 max-w-xs" style="font-size:12.5px"><div class="truncate text-slate-600" title="Prerequisite waived by HOP">Prerequisite waived by HOP</div></td>
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">25 May 2026</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-[#e0162b]/10 text-[#a01020] border-[#e0162b]/20" style="font-size:11.5px;font-weight:600">Rejected</span></td>
                            <td class="px-6 py-3 text-right text-slate-400" style="font-size:12px">Resolved</td>
                        </tr>
                        <tr data-row data-search="req-3326 farah diana s12119 bba3060" data-programme="BBA" data-type="Add" data-status="Approved" class="border-b border-slate-100 hover:bg-slate-50/60">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px"><span class="font-medium">REQ-3326</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><div class="text-slate-900 font-medium">Farah Diana</div><div class="text-slate-500" style="font-size:11.5px">S12119</div></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">BBA</span></td>
                            <td class="px-6 py-3 text-slate-700 text-center" style="font-size:12.5px">6</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><div class="text-slate-900 font-medium">BBA3060</div><div class="text-slate-500" style="font-size:11.5px">International Business</div></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Add</span></td>
                            <td class="px-6 py-3 max-w-xs" style="font-size:12.5px"><div class="truncate text-slate-600" title="Dean's List elective">Dean's List elective</div></td>
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">25 May 2026</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Approved</span></td>
                            <td class="px-6 py-3 text-right text-slate-400" style="font-size:12px">Resolved</td>
                        </tr>
                        <tr data-row data-search="req-3327 ahmad faizal s12048 itn3020" data-programme="BIT" data-type="Drop" data-status="Pending" class="border-b border-slate-100 hover:bg-slate-50/60">
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px"><span class="font-medium">REQ-3327</span></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><div class="text-slate-900 font-medium">Ahmad Faizal</div><div class="text-slate-500" style="font-size:11.5px">S12048</div></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">BIT</span></td>
                            <td class="px-6 py-3 text-slate-700 text-center" style="font-size:12.5px">3</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><div class="text-slate-900 font-medium">ITN3020</div><div class="text-slate-500" style="font-size:11.5px">Security Fundamentals</div></td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-[#e0162b]/10 text-[#a01020] border-[#e0162b]/20" style="font-size:11.5px;font-weight:600">Drop</span></td>
                            <td class="px-6 py-3 max-w-xs" style="font-size:12.5px"><div class="truncate text-slate-600" title="Repeat exemption applied">Repeat exemption applied</div></td>
                            <td class="px-6 py-3 text-slate-500" style="font-size:12.5px">24 May 2026</td>
                            <td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-amber-50 text-amber-700 border-amber-100" style="font-size:11.5px;font-weight:600">Pending</span></td>
                            <td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-modal-open="approve-req" class="inline-flex h-7 items-center rounded-md bg-emerald-50 border border-emerald-200 px-2 text-emerald-700 hover:bg-emerald-100" style="font-size:11.5px;font-weight:600">Approve</button><button type="button" data-modal-open="reject-req" class="inline-flex h-7 items-center rounded-md bg-[#e0162b]/10 border border-[#e0162b]/20 px-2 text-[#a01020] hover:bg-[#e0162b]/15" style="font-size:11.5px;font-weight:600">Reject</button></div></td>
                        </tr>
                        <tr data-table-empty style="display:none"><td colspan="10" class="py-10 text-center text-slate-400" style="font-size:13px">No requests match your filters.</td></tr>
                    </tbody>
                </table>
            </div>
        </div>
    </section>

    <%-- Approve modal --%>
    <div id="approve-req" data-modal class="fixed inset-0 z-[60] items-center justify-center p-4" style="display:none">
        <div data-modal-backdrop class="absolute inset-0 bg-slate-900/40 backdrop-blur-sm"></div>
        <div class="relative w-full max-w-md max-h-[90vh] overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-xl flex flex-col">
            <div class="flex items-start justify-between gap-4 border-b border-slate-100 px-6 py-4">
                <div><h2 class="text-slate-900" style="font-size:17px;font-weight:700;letter-spacing:-0.01em">Approve Request</h2><p class="mt-0.5 text-slate-500" style="font-size:13px">This decision will be logged with your admin identity.</p></div>
                <button type="button" data-modal-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-700"><i data-lucide="x" class="h-4 w-4"></i></button>
            </div>
            <div class="flex-1 overflow-y-auto px-6 py-5">
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Reason / Comment</span><div class="mt-1.5"><textarea placeholder="Provide a reason that will be logged with this decision." class="w-full min-h-[88px] rounded-md border border-slate-200 bg-white px-3 py-2 text-slate-900 placeholder:text-slate-400 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px;line-height:1.5"></textarea></div></label>
            </div>
            <div class="flex items-center justify-end gap-2 border-t border-slate-100 px-6 py-4 bg-slate-50/40">
                <button type="button" data-modal-close class="inline-flex items-center rounded-md px-4 h-10 text-slate-600 hover:bg-slate-100" style="font-size:13px;font-weight:600">Cancel</button>
                <button type="button" data-modal-close data-toast="Request approved" class="inline-flex items-center rounded-md px-4 h-10 text-white bg-emerald-600 hover:bg-emerald-700" style="font-size:13px;font-weight:600">Approve</button>
            </div>
        </div>
    </div>

    <%-- Reject modal --%>
    <div id="reject-req" data-modal class="fixed inset-0 z-[60] items-center justify-center p-4" style="display:none">
        <div data-modal-backdrop class="absolute inset-0 bg-slate-900/40 backdrop-blur-sm"></div>
        <div class="relative w-full max-w-md max-h-[90vh] overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-xl flex flex-col">
            <div class="flex items-start justify-between gap-4 border-b border-slate-100 px-6 py-4">
                <div><h2 class="text-slate-900" style="font-size:17px;font-weight:700;letter-spacing:-0.01em">Reject Request</h2><p class="mt-0.5 text-slate-500" style="font-size:13px">This decision will be logged with your admin identity.</p></div>
                <button type="button" data-modal-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-700"><i data-lucide="x" class="h-4 w-4"></i></button>
            </div>
            <div class="flex-1 overflow-y-auto px-6 py-5">
                <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Reason / Comment</span><div class="mt-1.5"><textarea placeholder="Provide a reason that will be logged with this decision." class="w-full min-h-[88px] rounded-md border border-slate-200 bg-white px-3 py-2 text-slate-900 placeholder:text-slate-400 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px;line-height:1.5"></textarea></div></label>
            </div>
            <div class="flex items-center justify-end gap-2 border-t border-slate-100 px-6 py-4 bg-slate-50/40">
                <button type="button" data-modal-close class="inline-flex items-center rounded-md px-4 h-10 text-slate-600 hover:bg-slate-100" style="font-size:13px;font-weight:600">Cancel</button>
                <button type="button" data-modal-close data-toast="Request rejected" class="inline-flex items-center rounded-md px-4 h-10 text-white bg-[#e0162b] hover:bg-[#a01020]" style="font-size:13px;font-weight:600">Reject</button>
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
