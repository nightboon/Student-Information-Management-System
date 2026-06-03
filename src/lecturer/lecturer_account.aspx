<%@ Page Language="C#" MasterPageFile="~/shared/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_account.aspx.cs" Inherits="src.lecturer.lecturer_account" Title="Account - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <%-- Header --%>
    <div>
        <p class="text-slate-500" style="font-size:13px;font-weight:500">Settings</p>
        <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">Account</h1>
        <p class="mt-1 text-slate-500" style="font-size:14px">Manage your profile, password, and preferences.</p>
    </div>

    <div class="mt-6 space-y-5">

        <%-- Profile card --%>
        <section class="rounded-lg border border-slate-200 bg-white">
            <div class="border-b border-slate-100 px-6 py-4">
                <h2 class="text-slate-900" style="font-size:15px;font-weight:700">Profile</h2>
            </div>
            <div class="px-6 py-6">
                <div class="flex items-center gap-5">
                    <div class="relative">
                        <% if (!string.IsNullOrEmpty(IconUrl)) { %>
                            <img src="<%= IconUrl %>" alt="<%= Server.HtmlEncode(FullName) %>" class="h-20 w-20 rounded-full object-cover" />
                        <% } else { %>
                            <div class="flex h-20 w-20 items-center justify-center rounded-full bg-gradient-to-br from-[#e0162b] to-[#a01020] text-white" style="font-size:26px;font-weight:700"><%= Initials %></div>
                        <% } %>
                        <span class="absolute -bottom-1 -right-1 inline-flex h-6 w-6 items-center justify-center rounded-full border-2 border-white bg-emerald-500">
                            <i data-lucide="check" class="h-3 w-3 text-white"></i>
                        </span>
                    </div>
                    <div class="min-w-0">
                        <p class="text-slate-900 truncate" style="font-size:18px;font-weight:700;letter-spacing:-0.01em"><%= FullName %></p>
                        <p class="text-slate-500" style="font-size:13px"><%= Department %></p>
                        <div class="mt-2 inline-flex items-center gap-1.5 rounded border border-emerald-100 bg-emerald-50 px-2 py-0.5 text-emerald-700" style="font-size:11px;font-weight:700;letter-spacing:0.04em">
                            <span class="h-1.5 w-1.5 rounded-full bg-emerald-500"></span> <%= RoleBadge %>
                        </div>
                    </div>
                </div>

                <div id="profile-form" class="mt-6" style="display:grid;grid-template-columns:1fr 1fr;gap:16px">

                    <%-- Read-only: Full name --%>
                    <div>
                        <span class="inline-flex items-center gap-1.5 text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">
                            <i data-lucide="user" class="h-3.5 w-3.5 text-slate-400"></i> FULL NAME
                        </span>
                        <div class="mt-1.5 flex h-10 w-full items-center rounded-md border border-slate-200 bg-slate-50/70 px-3">
                            <span class="text-slate-700 truncate" style="font-size:13px"><%= FullName %></span>
                        </div>
                    </div>

                    <%-- Read-only: Lecturer ID --%>
                    <div>
                        <span class="inline-flex items-center gap-1.5 text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">
                            <i data-lucide="id-card" class="h-3.5 w-3.5 text-slate-400"></i> LECTURER ID
                        </span>
                        <div class="mt-1.5 flex h-10 w-full items-center rounded-md border border-slate-200 bg-slate-50/70 px-3">
                            <span class="text-slate-700 truncate" style="font-size:13px"><%= LecturerIdLabel %></span>
                        </div>
                    </div>

                    <%-- Read-only: Email --%>
                    <div>
                        <span class="inline-flex items-center gap-1.5 text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">
                            <i data-lucide="mail" class="h-3.5 w-3.5 text-slate-400"></i> EMAIL
                        </span>
                        <div class="mt-1.5 flex h-10 w-full items-center rounded-md border border-slate-200 bg-slate-50/70 px-3">
                            <span class="text-slate-700 truncate" style="font-size:13px"><%= Email %></span>
                        </div>
                    </div>

                    <%-- Read-only: Department --%>
                    <div>
                        <span class="inline-flex items-center gap-1.5 text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">
                            <i data-lucide="building-2" class="h-3.5 w-3.5 text-slate-400"></i> DEPARTMENT
                        </span>
                        <div class="mt-1.5 flex h-10 w-full items-center rounded-md border border-slate-200 bg-slate-50/70 px-3">
                            <span class="text-slate-700 truncate" style="font-size:13px"><%= Department %></span>
                        </div>
                    </div>

                    <%-- Phone (editable) --%>
                    <label class="block">
                        <span class="inline-flex items-center gap-1.5 text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">
                            <i data-lucide="phone" class="h-3.5 w-3.5 text-slate-400"></i> PHONE
                        </span>
                        <div class="mt-1.5">
                            <input id="phone" type="text" value="<%= Server.HtmlEncode(Phone) %>" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                        </div>
                    </label>

                    <%-- Address (editable) --%>
                    <label class="block">
                        <span class="inline-flex items-center gap-1.5 text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">
                            <i data-lucide="map-pin" class="h-3.5 w-3.5 text-slate-400"></i> MAILING ADDRESS
                        </span>
                        <div class="mt-1.5">
                            <input id="address" type="text" value="<%= Server.HtmlEncode(MailingAddress) %>" class="h-10 w-full rounded-md border border-slate-200 bg-white px-3 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                        </div>
                    </label>

                    <div class="flex items-center justify-end gap-2 pt-2" style="grid-column:1 / -1">
                        <span id="profile-saved" class="hidden items-center gap-1 text-emerald-600" style="font-size:12.5px;font-weight:600">
                            <i data-lucide="check" class="h-4 w-4"></i> Profile saved
                        </span>
                        <button type="button" class="inline-flex items-center gap-1.5 rounded-md border border-slate-200 bg-white px-3 h-10 text-slate-700 hover:bg-slate-50 transition-colors" style="font-size:13px;font-weight:600">Cancel</button>
                        <button type="button" id="profile-save-btn" class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.6)]" style="font-size:13px;font-weight:600">Save changes</button>
                    </div>
                </div>
            </div>
        </section>

        <%-- Change password --%>
        <section class="rounded-lg border border-slate-200 bg-white">
            <div class="border-b border-slate-100 px-6 py-4">
                <h2 class="text-slate-900 inline-flex items-center gap-2" style="font-size:15px;font-weight:700">
                    <i data-lucide="lock" class="h-4 w-4 text-slate-500"></i> Change password
                </h2>
                <p class="text-slate-500" style="font-size:12.5px">Use at least 8 characters with a mix of letters, numbers, and symbols.</p>
            </div>

            <div id="password-form" class="px-6 py-6" style="display:grid;grid-template-columns:1fr 1fr;gap:16px">

                <%-- Current password (wide) --%>
                <label class="block" style="grid-column:1 / -1">
                    <span class="inline-flex items-center gap-1.5 text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">
                        <i data-lucide="lock" class="h-3.5 w-3.5 text-slate-400"></i> CURRENT PASSWORD
                    </span>
                    <div class="mt-1.5 relative">
                        <input id="pw-current" type="password" class="h-10 w-full rounded-md border border-slate-200 bg-white pl-3 pr-10 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                        <button type="button" data-toggle-pw="pw-current" class="absolute right-2 top-1/2 -translate-y-1/2 inline-flex h-7 w-7 items-center justify-center rounded text-slate-400 hover:text-slate-700 hover:bg-slate-100 transition-colors" aria-label="Show password">
                            <i data-lucide="eye" class="h-4 w-4"></i>
                        </button>
                    </div>
                </label>

                <%-- New password --%>
                <label>
                    <span class="inline-flex items-center gap-1.5 text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">
                        <i data-lucide="lock" class="h-3.5 w-3.5 text-slate-400"></i> NEW PASSWORD
                    </span>
                    <div class="mt-1.5 relative">
                        <input id="pw-new" type="password" class="h-10 w-full rounded-md border border-slate-200 bg-white pl-3 pr-10 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                        <button type="button" data-toggle-pw="pw-new" class="absolute right-2 top-1/2 -translate-y-1/2 inline-flex h-7 w-7 items-center justify-center rounded text-slate-400 hover:text-slate-700 hover:bg-slate-100 transition-colors" aria-label="Show password">
                            <i data-lucide="eye" class="h-4 w-4"></i>
                        </button>
                    </div>
                </label>

                <%-- Re-enter new password --%>
                <label>
                    <span class="inline-flex items-center gap-1.5 text-slate-500" style="font-size:11.5px;font-weight:600;letter-spacing:0.04em">
                        <i data-lucide="lock" class="h-3.5 w-3.5 text-slate-400"></i> RE-ENTER NEW PASSWORD
                    </span>
                    <div class="mt-1.5 relative">
                        <input id="pw-confirm" type="password" class="h-10 w-full rounded-md border border-slate-200 bg-white pl-3 pr-10 text-slate-900 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:13px" />
                        <button type="button" data-toggle-pw="pw-confirm" class="absolute right-2 top-1/2 -translate-y-1/2 inline-flex h-7 w-7 items-center justify-center rounded text-slate-400 hover:text-slate-700 hover:bg-slate-100 transition-colors" aria-label="Show password">
                            <i data-lucide="eye" class="h-4 w-4"></i>
                        </button>
                    </div>
                </label>

                <%-- Message slot --%>
                <div id="pw-msg" class="hidden items-start gap-2 rounded-md border px-3 py-2" style="font-size:12.5px;grid-column:1 / -1">
                    <i data-lucide="alert-circle" class="h-4 w-4 mt-0.5"></i>
                    <span id="pw-msg-text"></span>
                </div>

                <div class="flex items-center justify-end gap-2 pt-1" style="grid-column:1 / -1">
                    <button type="button" id="pw-submit-btn" class="inline-flex items-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.6)]" style="font-size:13px;font-weight:600">Update password</button>
                </div>
            </div>
        </section>

        <%-- Preferences --%>
        <section class="rounded-lg border border-slate-200 bg-white">
            <div class="border-b border-slate-100 px-6 py-4">
                <h2 class="text-slate-900" style="font-size:15px;font-weight:700">Preferences</h2>
                <p class="text-slate-500" style="font-size:12.5px">Customize how you receive updates.</p>
            </div>
            <div class="divide-y divide-slate-100">

                <%-- Email notifications toggle --%>
                <div class="flex items-start gap-4 px-6 py-4">
                    <span class="mt-0.5 flex h-9 w-9 items-center justify-center rounded-md bg-slate-100 text-slate-600">
                        <i data-lucide="bell" class="h-4 w-4"></i>
                    </span>
                    <div class="flex-1">
                        <p class="text-slate-900" style="font-size:13.5px;font-weight:600">Email notifications</p>
                        <p class="text-slate-500" style="font-size:12px">Submission alerts, grade reminders, and announcements.</p>
                    </div>
                    <button type="button" id="email-notif-toggle" data-on="true" class="relative inline-flex h-6 w-10 shrink-0 items-center rounded-full transition-colors bg-[#e0162b]" aria-pressed="true">
                        <span id="email-notif-knob" class="inline-block h-5 w-5 rounded-full bg-white shadow transition-transform translate-x-[18px]"></span>
                    </button>
                </div>

                <%-- Language (static) --%>
                <div class="flex items-start gap-4 px-6 py-4">
                    <span class="mt-0.5 flex h-9 w-9 items-center justify-center rounded-md bg-slate-100 text-slate-600">
                        <i data-lucide="globe" class="h-4 w-4"></i>
                    </span>
                    <div class="flex-1">
                        <p class="text-slate-900" style="font-size:13.5px;font-weight:600">Display language</p>
                        <p class="text-slate-500" style="font-size:12px">Used across the portal interface.</p>
                    </div>
                    <span class="inline-flex items-center gap-1.5 rounded-md border border-slate-200 bg-slate-50 px-3 h-9 text-slate-700" style="font-size:12.5px;font-weight:600">English</span>
                </div>
            </div>
        </section>

        <%-- Active sessions --%>
        <section class="rounded-lg border border-slate-200 bg-white">
            <div class="border-b border-slate-100 px-6 py-4 flex items-center justify-between">
                <div>
                    <h2 class="text-slate-900" style="font-size:15px;font-weight:700">Active sessions</h2>
                    <p class="text-slate-500" style="font-size:12.5px">Devices currently signed in to your account.</p>
                </div>
                <button class="inline-flex items-center gap-1.5 rounded-md border border-slate-200 bg-white px-3 h-9 text-slate-700 hover:bg-slate-50 transition-colors" style="font-size:12.5px;font-weight:600">
                    Sign out of all other sessions
                </button>
            </div>
            <ul class="divide-y divide-slate-100">
                <li class="flex items-center justify-between gap-3 px-6 py-4">
                    <div class="min-w-0">
                        <p class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Chrome &middot; Windows 11</p>
                        <p class="text-slate-500" style="font-size:12px">Penang, MY</p>
                    </div>
                    <span class="inline-flex items-center gap-1 rounded border border-emerald-100 bg-emerald-50 px-1.5 py-0.5 text-emerald-700" style="font-size:10px;font-weight:700;letter-spacing:0.04em">
                        <span class="h-1.5 w-1.5 rounded-full bg-emerald-500"></span> THIS DEVICE
                    </span>
                </li>
                <li class="flex items-center justify-between gap-3 px-6 py-4">
                    <div class="min-w-0">
                        <p class="text-slate-900 truncate" style="font-size:13px;font-weight:600">Safari &middot; iPhone 15</p>
                        <p class="text-slate-500" style="font-size:12px">Penang, MY</p>
                    </div>
                    <button class="text-[#e0162b] hover:text-[#a01020]" style="font-size:12px;font-weight:600">Sign out</button>
                </li>
            </ul>
        </section>

        <%-- Sign out --%>
        <section class="rounded-lg border border-[#e0162b]/20 bg-[#e0162b]/5 px-6 py-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
            <div>
                <h3 class="text-[#a01020] inline-flex items-center gap-1.5" style="font-size:14px;font-weight:700">
                    <i data-lucide="log-out" class="h-4 w-4"></i> Sign out
                </h3>
                <p class="text-[#a01020]/80 mt-0.5" style="font-size:12.5px">End your session on this device.</p>
            </div>
            <a href="<%= ResolveUrl("~/shared/login.aspx") %>" data-action="logout" class="inline-flex items-center justify-center gap-1.5 rounded-md bg-[#e0162b] px-4 h-10 text-white hover:bg-[#a01020] transition-colors shadow-[0_8px_18px_-10px_rgba(224,22,43,0.6)]" style="font-size:13px;font-weight:600">
                <i data-lucide="log-out" class="h-4 w-4"></i> Sign out
            </a>
        </section>

    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/shared/account/account.js") %>"></script>
</asp:Content>
