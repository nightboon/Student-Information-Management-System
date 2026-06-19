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
                    <button type="button" data-toast="Use Reports to generate downloadable files" data-toast-type="info" class="flex w-full items-center gap-2 rounded-md px-3 py-2 text-left text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:500"><i data-lucide="file-text" class="h-4 w-4 text-[#e0162b]"></i> Export as PDF</button>
                    <button type="button" data-toast="Use Reports to generate downloadable files" data-toast-type="info" class="flex w-full items-center gap-2 rounded-md px-3 py-2 text-left text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:500"><i data-lucide="file-spreadsheet" class="h-4 w-4 text-emerald-600"></i> Export as Excel</button>
                </div>
            </div>
        </div>
    </div>

    <%-- KPI cards --%>
    <section class="mt-6" style="display:grid;grid-template-columns:repeat(auto-fit,minmax(200px,1fr));gap:16px">
        <div class="rounded-2xl border border-slate-200 bg-white p-4 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-center justify-between"><div class="text-slate-500" style="font-size:11.5px;font-weight:500">Pending</div><span class="inline-flex h-7 w-7 items-center justify-center rounded-lg bg-amber-50 text-amber-600"><i data-lucide="clock" class="h-4 w-4"></i></span></div><div class="mt-1 text-amber-600" style="font-size:24px;font-weight:700;letter-spacing:-0.01em"><%= Number(PendingCount) %></div>
        </div>
        <div class="rounded-2xl border border-slate-200 bg-white p-4 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-center justify-between"><div class="text-slate-500" style="font-size:11.5px;font-weight:500">Approved</div><span class="inline-flex h-7 w-7 items-center justify-center rounded-lg bg-emerald-50 text-emerald-600"><i data-lucide="check-circle-2" class="h-4 w-4"></i></span></div><div class="mt-1 text-emerald-600" style="font-size:24px;font-weight:700;letter-spacing:-0.01em"><%= Number(ApprovedCount) %></div>
        </div>
        <div class="rounded-2xl border border-slate-200 bg-white p-4 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-center justify-between"><div class="text-slate-500" style="font-size:11.5px;font-weight:500">Rejected</div><span class="inline-flex h-7 w-7 items-center justify-center rounded-lg bg-[#e0162b]/10 text-[#a01020]"><i data-lucide="x-circle" class="h-4 w-4"></i></span></div><div class="mt-1 text-[#a01020]" style="font-size:24px;font-weight:700;letter-spacing:-0.01em"><%= Number(RejectedCount) %></div>
        </div>
        <div class="rounded-2xl border border-slate-200 bg-white p-4 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-center justify-between"><div class="text-slate-500" style="font-size:11.5px;font-weight:500">Total Requests</div><span class="inline-flex h-7 w-7 items-center justify-center rounded-lg bg-slate-100 text-slate-500"><i data-lucide="file-edit" class="h-4 w-4"></i></span></div><div class="mt-1 text-slate-900" style="font-size:24px;font-weight:700;letter-spacing:-0.01em"><%= Number(TotalCount) %></div>
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
                        <%= RequestRowsHtml %>
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
    <script>
      (function () {
        function post(method, payload) {
          return fetch("add_drop_requests.aspx/" + method, {
            method: "POST",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            credentials: "same-origin",
            body: JSON.stringify(payload || {})
          }).then(function (r) {
            if (!r.ok) throw new Error("Request failed");
            return r.json();
          });
        }
        document.addEventListener("click", function (e) {
          var action = e.target.closest("[data-request-action]");
          if (!action) return;
          e.preventDefault();
          e.stopPropagation();
          var next = action.getAttribute("data-next-status");
          var label = next === "ENROLLED" ? "approve" : "reject";
          if (!confirm("Confirm " + label + " request?")) return;
          post("SetRequestStatus", {
            enrollmentId: parseInt(action.getAttribute("data-request-id"), 10),
            status: next
          }).then(function () {
            if (window.toast) window.toast.success(next === "ENROLLED" ? "Request approved" : "Request rejected");
            setTimeout(function () { location.reload(); }, 450);
          }).catch(function () {
            if (window.toast) window.toast.error("Could not update request");
          });
        }, true);
      })();
    </script>
</asp:Content>

