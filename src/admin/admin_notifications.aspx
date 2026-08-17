<%@ Page Language="C#" MasterPageFile="~/admin/AdminLayout.master" AutoEventWireup="true" CodeBehind="admin_notifications.aspx.cs" Inherits="src.admin.admin_notifications" Title="Notifications - INTI Admin Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Admin</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">Notifications</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">Create, review, and update role-targeted portal notifications.</p>
        </div>
        <button type="button"
                data-admin-new-notification
                data-modal-open="notification-modal"
                class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.9)]"
                style="font-size:13px;font-weight:600">
            <i data-lucide="plus" class="h-4 w-4"></i>
            New Notification
        </button>
    </div>

    <section class="mt-6 rounded-lg border border-slate-200 bg-white">
        <div data-table data-page-size="20">
            <div class="flex flex-col gap-3 px-6 py-4 lg:flex-row lg:items-center lg:justify-between">
                <div class="relative w-full lg:max-w-sm">
                    <i data-lucide="search" class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"></i>
                    <input data-table-search placeholder="Search notifications..." class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px" />
                </div>
                <select data-table-filter="audience" class="h-9 rounded-md border border-slate-200 bg-white px-3 text-slate-700 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px">
                    <option value="">All audiences</option>
                    <option value="STUDENT">Students</option>
                    <option value="LECTURER">Lecturers</option>
                    <option value="BOTH">Students &amp; Lecturers</option>
                </select>
            </div>
            <div class="overflow-x-auto">
                <table class="min-w-full">
                    <thead class="border-y border-slate-100 bg-slate-50/60 text-slate-500">
                        <tr>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Title</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Audience</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Message</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Created By</th>
                            <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Created</th>
                            <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        <%= NotificationRowsHtml %>
                    </tbody>
                </table>
            </div>
        </div>
    </section>

    <div id="notification-modal" data-modal class="fixed inset-0 z-[60] items-center justify-center p-4" style="display:none">
        <div data-modal-backdrop class="absolute inset-0 bg-slate-900/40 backdrop-blur-sm"></div>
        <div class="relative flex max-h-[90vh] w-full max-w-2xl flex-col overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-xl">
            <div class="flex items-start justify-between gap-4 border-b border-slate-100 px-6 py-4">
                <div>
                    <h2 id="notification-modal-title" class="text-slate-900" style="font-size:17px;font-weight:700;letter-spacing:-0.01em">Notification Details</h2>
                    <p class="mt-0.5 text-slate-500" style="font-size:12.5px">Send a portal notification to the selected audience.</p>
                </div>
                <button type="button" data-modal-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-700">
                    <i data-lucide="x" class="h-4 w-4"></i>
                </button>
            </div>
            <div class="flex-1 overflow-y-auto px-6 py-5">
                <input type="hidden" data-field="notificationId" />
                <div class="grid gap-4">
                    <label class="block">
                        <span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Title</span>
                        <input data-field="title" maxlength="150" class="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                    </label>
                    <label class="block">
                        <span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Target Audience</span>
                        <select data-field="targetRole" class="mt-1.5 h-10 w-full rounded-md border border-slate-200 bg-white px-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px">
                            <option value="STUDENT">Students only</option>
                            <option value="LECTURER">Lecturers only</option>
                            <option value="BOTH">Students &amp; Lecturers</option>
                        </select>
                    </label>
                    <label class="block">
                        <span class="block text-slate-500 uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.06em">Message</span>
                        <textarea data-field="message" class="mt-1.5 min-h-[180px] w-full rounded-md border border-slate-200 bg-white px-3 py-2 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px;line-height:1.55"></textarea>
                    </label>
                </div>
            </div>
            <div class="flex items-center justify-end gap-2 border-t border-slate-100 bg-slate-50/40 px-6 py-4">
                <button type="button" data-modal-close class="inline-flex h-10 items-center rounded-md px-4 text-slate-600 hover:bg-slate-100" style="font-size:13px;font-weight:600">Cancel</button>
                <button type="button" data-admin-save-notification class="inline-flex h-10 items-center rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:600">Save Notification</button>
            </div>
        </div>
    </div>

    <div id="notification-drawer" data-drawer class="fixed inset-0 z-[60]" style="display:none">
        <div data-drawer-backdrop class="absolute inset-0 bg-slate-900/40"></div>
        <div data-drawer-panel class="absolute right-0 top-0 flex h-full w-full max-w-xl flex-col border-l border-slate-200 bg-white shadow-2xl">
            <div class="flex items-start justify-between gap-4 border-b border-slate-100 px-6 py-4">
                <div class="min-w-0">
                    <span id="view-audience" class="inline-flex items-center rounded-full border px-2 py-0.5" style="font-size:11.5px;font-weight:600"></span>
                    <h2 id="view-title" class="mt-3 text-slate-900" style="font-size:20px;font-weight:700;letter-spacing:-0.01em;line-height:1.25"></h2>
                </div>
                <button type="button" data-drawer-close class="inline-flex h-8 w-8 items-center justify-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-700">
                    <i data-lucide="x" class="h-4 w-4"></i>
                </button>
            </div>
            <div class="flex-1 overflow-y-auto px-6 py-5">
                <div class="flex flex-wrap items-center gap-x-4 gap-y-1 text-slate-500" style="font-size:12.5px">
                    <span class="inline-flex items-center gap-1.5">
                        <i data-lucide="user" class="h-3.5 w-3.5 text-slate-400"></i>
                        <span id="view-author"></span>
                    </span>
                    <span class="inline-flex items-center gap-1.5">
                        <i data-lucide="clock" class="h-3.5 w-3.5 text-slate-400"></i>
                        <span id="view-created"></span>
                    </span>
                </div>
                <div id="view-message" class="mt-5 whitespace-pre-line text-slate-700" style="font-size:14px;line-height:1.7"></div>
            </div>
            <div class="flex items-center justify-end gap-2 border-t border-slate-100 bg-slate-50/40 px-6 py-4">
                <button type="button" data-drawer-close class="inline-flex h-10 items-center rounded-md px-4 text-slate-600 hover:bg-slate-100" style="font-size:13px;font-weight:600">Close</button>
                <button type="button" data-admin-edit-from-view class="inline-flex h-10 items-center gap-1.5 rounded-md border border-slate-200 bg-white px-4 text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:600">
                    <i data-lucide="pencil" class="h-4 w-4"></i>
                    Edit
                </button>
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
            var selectedRow = null;

            function post(method, payload) {
                return fetch("admin_notifications.aspx/" + method, {
                    method: "POST",
                    headers: { "Content-Type": "application/json; charset=utf-8" },
                    credentials: "same-origin",
                    body: JSON.stringify(payload || {})
                }).then(function (r) {
                    return r.json().then(function (json) {
                        var data = json && json.d ? json.d : json;
                        if (!r.ok || (data && data.ok === false)) throw new Error((data && data.message) || "Request failed");
                        return data;
                    });
                });
            }

            function field(modal, name) {
                var el = modal.querySelector('[data-field="' + name + '"]');
                return el ? el.value.trim() : "";
            }

            function setField(modal, name, value) {
                var el = modal.querySelector('[data-field="' + name + '"]');
                if (el) el.value = value || "";
            }

            function setText(id, value) {
                var el = document.getElementById(id);
                if (el) el.textContent = value || "";
            }

            function applyAudienceBadge(el, audience) {
                if (!el) return;
                var classes = "inline-flex items-center rounded-full border px-2 py-0.5 ";
                if (audience === "STUDENT") classes += "bg-emerald-50 text-emerald-700 border-emerald-100";
                else if (audience === "LECTURER") classes += "bg-sky-50 text-sky-700 border-sky-100";
                else classes += "bg-slate-100 text-slate-600 border-slate-200";
                el.className = classes;
            }

            function openModal(id) {
                var modal = document.getElementById(id);
                if (!modal) return;
                modal.style.display = "flex";
                document.body.style.overflow = "hidden";
                if (window.renderIcons) window.renderIcons();
            }

            function closeDrawer(id) {
                var drawer = document.getElementById(id);
                if (!drawer) return;
                var panel = drawer.querySelector("[data-drawer-panel]");
                if (panel) panel.style.transform = "translateX(100%)";
                drawer.style.display = "none";
                document.body.style.overflow = "";
            }

            function fillModal(row) {
                var modal = document.getElementById("notification-modal");
                if (!modal) return;
                var isEdit = !!row;
                setText("notification-modal-title", isEdit ? "Edit Notification" : "New Notification");
                setField(modal, "notificationId", row ? row.dataset.id : "");
                setField(modal, "title", row ? row.dataset.title : "");
                setField(modal, "targetRole", row ? row.dataset.audience : "STUDENT");
                setField(modal, "message", row ? row.dataset.message : "");
            }

            function fillDrawer(row) {
                if (!row) return;
                selectedRow = row;
                setText("view-title", row.dataset.title);
                setText("view-author", row.dataset.author);
                setText("view-created", row.dataset.created);
                setText("view-message", row.dataset.message);
                var badge = document.getElementById("view-audience");
                if (badge) {
                    badge.textContent = row.dataset.audienceLabel;
                    applyAudienceBadge(badge, row.dataset.audience);
                }
            }

            function done(message) {
                if (window.toast) window.toast.success(message);
                setTimeout(function () { location.reload(); }, 450);
            }

            document.addEventListener("click", function (e) {
                var newBtn = e.target.closest("[data-admin-new-notification]");
                if (newBtn) {
                    selectedRow = null;
                    fillModal(null);
                    return;
                }

                var viewBtn = e.target.closest("[data-admin-view-notification]");
                if (viewBtn) {
                    fillDrawer(viewBtn.closest("tr"));
                    return;
                }

                var editBtn = e.target.closest("[data-admin-edit-notification]");
                if (editBtn) {
                    selectedRow = editBtn.closest("tr");
                    fillModal(selectedRow);
                    return;
                }

                var editFromView = e.target.closest("[data-admin-edit-from-view]");
                if (editFromView) {
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    fillModal(selectedRow);
                    closeDrawer("notification-drawer");
                    openModal("notification-modal");
                    return;
                }

                var saveBtn = e.target.closest("[data-admin-save-notification]");
                if (saveBtn) {
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    var modal = document.getElementById("notification-modal");
                    post("SaveNotification", {
                        request: {
                            notificationId: parseInt(field(modal, "notificationId"), 10) || 0,
                            title: field(modal, "title"),
                            message: field(modal, "message"),
                            targetRole: field(modal, "targetRole")
                        }
                    }).then(function () { done("Notification saved"); })
                        .catch(function (error) { if (window.toast) window.toast.error(error && error.message ? error.message : "Could not save notification"); });
                }
            }, true);
        })();
    </script>
</asp:Content>
