<%@ Page Language="C#" MasterPageFile="~/admin/AdminLayout.master" AutoEventWireup="true" CodeBehind="academic_calendar.aspx.cs" Inherits="src.admin.academic_calendar" Title="Academic Calendar - INTI Admin Portal" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <div class="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Admin</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">Academic Calendar</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">Plan and manage semesters, examination periods, and key academic dates.</p>
        </div>
        <div class="flex flex-wrap items-center gap-2">
            <div class="relative" data-dropdown>
                <button type="button" data-dropdown-toggle class="inline-flex items-center gap-1.5 rounded-md border border-slate-200 bg-white px-3 h-10 text-slate-700 hover:bg-slate-50 transition-colors" style="font-size:13px;font-weight:600"><i data-lucide="download" class="h-4 w-4"></i> Export <i data-lucide="chevron-down" class="h-3.5 w-3.5"></i></button>
                <div data-dropdown-menu style="display:none" class="absolute right-0 z-20 mt-2 w-48 rounded-xl border border-slate-200 bg-white p-1 shadow-lg">
                    <button type="button" data-toast="PDF export started" data-toast-desc="events queued for download" class="flex w-full items-center gap-2 rounded-md px-3 py-2 text-left text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:500"><i data-lucide="file-text" class="h-4 w-4 text-[#e0162b]"></i> Export as PDF</button>
                    <button type="button" data-toast="Excel export started" data-toast-desc="events queued for download" class="flex w-full items-center gap-2 rounded-md px-3 py-2 text-left text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:500"><i data-lucide="file-spreadsheet" class="h-4 w-4 text-emerald-600"></i> Export as Excel</button>
                </div>
            </div>
            <button type="button" data-modal-open="event-modal" class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]" style="font-size:13px;font-weight:600"><i data-lucide="plus" class="h-4 w-4"></i> Add Event</button>
        </div>
    </div>

    <div data-table data-page-size="20">
        <section class="mt-6" style="display:flex;flex-wrap:wrap;gap:24px">
            <%-- Calendar --%>
            <div class="rounded-lg border border-slate-200 bg-white" style="flex:2 1 460px;min-width:0">
                <div class="flex items-center justify-between border-b border-slate-100 px-6 py-4">
                    <div class="flex items-center gap-3">
                        <button type="button" class="inline-flex h-8 w-8 items-center justify-center rounded-md hover:bg-slate-100"><i data-lucide="chevron-left" class="h-4 w-4"></i></button>
                        <div class="text-slate-900" style="font-size:15px;font-weight:700">May 2026</div>
                        <button type="button" class="inline-flex h-8 w-8 items-center justify-center rounded-md hover:bg-slate-100"><i data-lucide="chevron-right" class="h-4 w-4"></i></button>
                        <button type="button" class="ml-2 inline-flex h-8 items-center rounded-md border border-slate-200 px-2.5 text-slate-600 hover:bg-slate-50" style="font-size:12px;font-weight:600">Today</button>
                    </div>
                    <select class="h-9 rounded-md border border-slate-200 bg-white px-3 text-slate-700 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px"><option value="">All semesters</option><option>Sem 1 25/26</option><option>Sem 2 25/26</option><option>Sem 1 26/27</option></select>
                </div>
                <div class="px-6 py-4 text-slate-500" style="display:grid;grid-template-columns:repeat(7,minmax(0,1fr));gap:4px;font-size:11px;font-weight:600">
                    <div class="px-2">Mon</div><div class="px-2">Tue</div><div class="px-2">Wed</div><div class="px-2">Thu</div><div class="px-2">Fri</div><div class="px-2">Sat</div><div class="px-2">Sun</div>
                </div>
                <div class="px-6 pb-6" style="display:grid;grid-template-columns:repeat(7,minmax(0,1fr));gap:4px">
                    <div class="min-h-[88px] rounded-lg border border-transparent bg-slate-50/30 p-1.5"></div>
                    <div class="min-h-[88px] rounded-lg border border-transparent bg-slate-50/30 p-1.5"></div>
                    <div class="min-h-[88px] rounded-lg border border-transparent bg-slate-50/30 p-1.5"></div>
                    <div class="min-h-[88px] rounded-lg border border-transparent bg-slate-50/30 p-1.5"></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">1</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">2</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">3</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">4</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">5</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">6</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">7</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">8</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">9</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">10</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">11</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">12</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">13</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">14</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">15</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">16</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">17</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">18</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">19</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">20</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">21</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">22</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">23</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">24</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">25</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">26</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">27</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">28</div></div>
                    <div class="min-h-[88px] rounded-lg border border-[#e0162b]/40 ring-2 ring-[#e0162b]/30 p-1.5"><div class="text-[#a01020]" style="font-size:12px;font-weight:700">29</div></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">30</div><button type="button" data-modal-open="event-modal" title="Add/Drop Deadline" class="mt-1 block w-full truncate rounded px-1.5 py-0.5 text-left bg-[#e0162b]/10 text-[#a01020]" style="font-size:10px;font-weight:600">Add/Drop Deadline</button></div>
                    <div class="min-h-[88px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px;font-weight:500">31</div></div>
                </div>
            </div>

            <%-- Filters --%>
            <div class="rounded-lg border border-slate-200 bg-white" style="flex:1 1 280px;min-width:0">
                <div class="flex items-start justify-between gap-3 border-b border-slate-100 px-6 py-4"><div><h2 class="text-slate-900" style="font-size:15px;font-weight:700">Filters</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">Narrow events shown</p></div></div>
                <div class="px-6 py-5 space-y-4">
                    <div>
                        <div class="text-slate-500 mb-1.5 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Semester</div>
                        <select data-table-filter="sem" class="h-9 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-700 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px"><option value="">All semesters</option><option>Sem 1 25/26</option><option>Sem 2 25/26</option><option>Sem 1 26/27</option></select>
                    </div>
                    <div>
                        <div class="text-slate-500 mb-1.5 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Event Type</div>
                        <select data-table-filter="type" class="h-9 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-700 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px"><option value="">All types</option><option>Semester start</option><option>Semester end</option><option>Examination period</option><option>Registration deadline</option><option>Add/drop deadline</option><option>Holiday</option><option>Academic break</option><option>Result release</option></select>
                    </div>
                    <button type="button" data-table-clear class="text-slate-500 hover:text-slate-900" style="font-size:12.5px;font-weight:600">Clear filters</button>
                </div>
            </div>
        </section>

        <%-- Events table --%>
        <section class="mt-6 rounded-lg border border-slate-200 bg-white">
            <div class="flex items-start justify-between gap-3 border-b border-slate-100 px-6 py-4"><div><h2 class="text-slate-900" style="font-size:15px;font-weight:700">Calendar Events</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">6 events</p></div></div>
            <div class="overflow-x-auto">
                <table class="min-w-full">
                    <thead class="border-y border-slate-100 bg-slate-50/60 text-slate-500"><tr>
                        <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Event</th>
                        <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Type</th>
                        <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Start</th>
                        <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">End</th>
                        <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Semester</th>
                        <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Status</th>
                        <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Action</th>
                    </tr></thead>
                    <tbody>
                        <tr data-row data-search="semester 1 start" data-sem="Sem 1 25/26" data-type="Semester start" class="border-b border-slate-100 hover:bg-slate-50/60"><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Semester 1 Start</span></td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">Semester start</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">01 Sep 2025</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">01 Sep 2025</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">Sem 1 25/26</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">Completed</span></td><td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-modal-open="event-modal" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Delete event?" data-confirm-message="&quot;Semester 1 Start&quot; will be permanently removed." data-confirm-label="Delete" data-confirm-danger="true" data-confirm-toast="Event deleted" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-[#e0162b] hover:bg-[#e0162b]/5"><i data-lucide="trash-2" class="h-3.5 w-3.5"></i></button></div></td></tr>
                        <tr data-row data-search="add/drop deadline" data-sem="Sem 1 25/26" data-type="Add/drop deadline" class="border-b border-slate-100 hover:bg-slate-50/60"><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Add/Drop Deadline</span></td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">Add/drop deadline</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">30 May 2026</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">30 May 2026</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">Sem 1 25/26</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-amber-50 text-amber-700 border-amber-100" style="font-size:11.5px;font-weight:600">Upcoming</span></td><td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-modal-open="event-modal" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Delete event?" data-confirm-message="&quot;Add/Drop Deadline&quot; will be permanently removed." data-confirm-label="Delete" data-confirm-danger="true" data-confirm-toast="Event deleted" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-[#e0162b] hover:bg-[#e0162b]/5"><i data-lucide="trash-2" class="h-3.5 w-3.5"></i></button></div></td></tr>
                        <tr data-row data-search="final examination period" data-sem="Sem 1 25/26" data-type="Examination period" class="border-b border-slate-100 hover:bg-slate-50/60"><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Final Examination Period</span></td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">Examination period</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">12 Jun 2026</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">25 Jun 2026</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">Sem 1 25/26</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-amber-50 text-amber-700 border-amber-100" style="font-size:11.5px;font-weight:600">Upcoming</span></td><td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-modal-open="event-modal" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Delete event?" data-confirm-message="&quot;Final Examination Period&quot; will be permanently removed." data-confirm-label="Delete" data-confirm-danger="true" data-confirm-toast="Event deleted" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-[#e0162b] hover:bg-[#e0162b]/5"><i data-lucide="trash-2" class="h-3.5 w-3.5"></i></button></div></td></tr>
                        <tr data-row data-search="mid-term break" data-sem="Sem 1 25/26" data-type="Academic break" class="border-b border-slate-100 hover:bg-slate-50/60"><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Mid-Term Break</span></td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">Academic break</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">21 Apr 2026</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">27 Apr 2026</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">Sem 1 25/26</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200" style="font-size:11.5px;font-weight:600">Completed</span></td><td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-modal-open="event-modal" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Delete event?" data-confirm-message="&quot;Mid-Term Break&quot; will be permanently removed." data-confirm-label="Delete" data-confirm-danger="true" data-confirm-toast="Event deleted" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-[#e0162b] hover:bg-[#e0162b]/5"><i data-lucide="trash-2" class="h-3.5 w-3.5"></i></button></div></td></tr>
                        <tr data-row data-search="result release" data-sem="Sem 1 25/26" data-type="Result release" class="border-b border-slate-100 hover:bg-slate-50/60"><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Result Release</span></td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">Result release</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">10 Jul 2026</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">10 Jul 2026</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">Sem 1 25/26</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-sky-50 text-sky-700 border-sky-100" style="font-size:11.5px;font-weight:600">Scheduled</span></td><td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-modal-open="event-modal" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Delete event?" data-confirm-message="&quot;Result Release&quot; will be permanently removed." data-confirm-label="Delete" data-confirm-danger="true" data-confirm-toast="Event deleted" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-[#e0162b] hover:bg-[#e0162b]/5"><i data-lucide="trash-2" class="h-3.5 w-3.5"></i></button></div></td></tr>
                        <tr data-row data-search="sep 2026 intake registration" data-sem="Sem 1 26/27" data-type="Registration deadline" class="border-b border-slate-100 hover:bg-slate-50/60"><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Sep 2026 Intake Registration</span></td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">Registration deadline</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">14 Jun 2026</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">14 Jun 2026</td><td class="px-6 py-3 text-slate-700" style="font-size:12.5px">Sem 1 26/27</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-amber-50 text-amber-700 border-amber-100" style="font-size:11.5px;font-weight:600">Upcoming</span></td><td class="px-6 py-3 text-right" style="font-size:12.5px"><div class="flex items-center justify-end gap-1"><button type="button" data-modal-open="event-modal" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50"><i data-lucide="pencil" class="h-3.5 w-3.5"></i></button><button type="button" data-confirm data-confirm-title="Delete event?" data-confirm-message="&quot;Sep 2026 Intake Registration&quot; will be permanently removed." data-confirm-label="Delete" data-confirm-danger="true" data-confirm-toast="Event deleted" class="inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-[#e0162b] hover:bg-[#e0162b]/5"><i data-lucide="trash-2" class="h-3.5 w-3.5"></i></button></div></td></tr>
                        <tr data-table-empty style="display:none"><td colspan="7" class="px-6 py-12 text-center text-slate-400" style="font-size:13px">No events match your filters.</td></tr>
                    </tbody>
                </table>
            </div>
        </section>
    </div>

    <%-- Event modal --%>
    <div id="event-modal" data-modal class="fixed inset-0 z-[60] items-center justify-center p-4" style="display:none">
        <div data-modal-backdrop class="absolute inset-0 bg-slate-900/40 backdrop-blur-sm"></div>
        <div class="relative w-full max-w-2xl max-h-[90vh] overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-xl flex flex-col">
            <div class="flex items-start justify-between gap-4 border-b border-slate-100 px-6 py-4"><div><h2 class="text-slate-900" style="font-size:17px;font-weight:700;letter-spacing:-0.01em">Event Details</h2></div><button type="button" data-modal-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-700"><i data-lucide="x" class="h-4 w-4"></i></button></div>
            <div class="flex-1 overflow-y-auto px-6 py-5">
                <div style="display:grid;grid-template-columns:repeat(auto-fit,minmax(240px,1fr));gap:16px">
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Event Title</span><div class="mt-1.5"><input class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Event Type</span><div class="mt-1.5"><select class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><option>Semester start</option><option>Semester end</option><option>Examination period</option><option>Registration deadline</option><option>Add/drop deadline</option><option>Holiday</option><option>Academic break</option><option>Result release</option></select></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Start Date</span><div class="mt-1.5"><input type="date" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">End Date</span><div class="mt-1.5"><input type="date" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" /></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Semester</span><div class="mt-1.5"><select class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><option>Sem 1 25/26</option><option>Sem 2 25/26</option><option>Sem 1 26/27</option></select></div></label>
                    <label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Status</span><div class="mt-1.5"><select class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px"><option>Upcoming</option><option>Scheduled</option><option>Completed</option></select></div></label>
                </div>
                <div class="mt-5"><label class="block"><span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Notes</span><div class="mt-1.5"><textarea placeholder="Optional description or instructions&hellip;" class="w-full min-h-[88px] rounded-md border border-slate-200 bg-white px-3 py-2 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px;line-height:1.5"></textarea></div></label></div>
            </div>
            <div class="flex items-center justify-end gap-2 border-t border-slate-100 px-6 py-4 bg-slate-50/40"><button type="button" data-modal-close class="inline-flex items-center rounded-md px-4 h-10 text-slate-600 hover:bg-slate-100" style="font-size:13px;font-weight:600">Cancel</button><button type="button" data-modal-close data-toast="Event saved" class="inline-flex items-center rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:600">Save Event</button></div>
        </div>
    </div>

</asp:Content>
<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/admin/shared/icons.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/toast.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/table.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/ui.js") %>"></script>
</asp:Content>
