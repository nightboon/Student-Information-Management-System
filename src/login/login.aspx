<%@ Page Language="C#" MasterPageFile="LoginLayout.master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="src.login.login" Title="Sign in - INTI Student Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex w-full max-w-md flex-col">
        
        <div class="mb-10">
            <div class="inline-flex items-center gap-2 rounded-full border border-indigo-100 bg-indigo-50/70 px-3 py-1 text-indigo-700" style="font-size:12px;font-weight:500">
                <i data-lucide="shield-check" class="h-3.5 w-3.5"></i>
                Secure sign-in
            </div>
            <h2 id="login-heading" class="mt-4 text-gray-900" style="font-size:28px;font-weight:700;letter-spacing:-0.005em">Welcome back</h2>
            <p id="login-subtitle" class="mt-2 text-gray-500" style="font-size:15px;line-height:1.6">Sign in to access your INTI portal.</p>
        </div>

        <div class="rounded-2xl border border-gray-200/80 bg-white p-8 shadow-[0_8px_30px_-12px_rgba(15,23,42,0.12)]">

            <!-- EMAIL CARD -->
            <div id="email-card">
                <div id="email-form">
                    <div class="flex flex-col gap-2">
                        <div id="email-wrap" class="relative rounded-xl border bg-white transition-all border-gray-200 hover:border-gray-300">
                            <label for="email" class="pointer-events-none absolute left-4 transition-all top-1/2 -translate-y-1/2 text-gray-400" style="font-size:14px;font-weight:500" id="email-label">Email address</label>
                            <input id="email" name="email" type="email" autocomplete="email" autofocus class="block w-full bg-transparent px-4 pt-6 pb-2 text-gray-900 outline-none" style="font-size:14px;font-weight:400;height:56px" />
                        </div>
                        <div class="min-h-[20px] px-1">
                            <p id="email-helper" class="text-gray-500" style="font-size:13px">Use your institutional email to continue.</p>
                            <div id="email-error" class="hidden items-center gap-1.5 text-red-600" style="font-size:13px">
                                <i data-lucide="alert-circle" class="h-3.5 w-3.5"></i>
                                <span id="email-error-text">Please enter a valid INTI email.</span>
                            </div>
                        </div>
                    </div>
                    <button id="email_submit" runat="server" type="submit" onserverclick="EmailSubmit_Click" ClientIDMode="Static" disabled="disabled" class="mt-6 group flex w-full items-center justify-center gap-2 rounded-xl px-4 transition-all h-12 bg-indigo-300 text-white cursor-not-allowed" style="font-size:15px;font-weight:600">
                        <span id="email-submit-label">Continue</span>
                        <span id="email-submit-arrow" class="inline-flex"><i data-lucide="arrow-right" class="h-4 w-4 transition-transform group-hover:translate-x-0.5"></i></span>
                        <span id="email-submit-spinner" class="hidden"><i data-lucide="loader-2" class="h-4 w-4 animate-spin"></i></span>
                    </button>
                    <p class="mt-6 text-center text-gray-400" style="font-size:12px">By continuing you agree to INTI's Acceptable Use Policy.</p>
                </div>
            </div>

            <!-- PASSWORD CARD (hidden initially) -->
            <div id="password-card" class="hidden">
                <div id="password-form">
                    <button type="button" data-action="back-to-email" class="group mb-5 flex w-full items-center justify-between rounded-xl border border-gray-200 bg-gray-50/70 px-3 py-2.5 transition-all hover:border-indigo-200 hover:bg-indigo-50/40">
                        <span class="flex items-center gap-2.5 text-gray-700">
                            <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-white ring-1 ring-gray-200">
                                <i data-lucide="mail" class="h-3.5 w-3.5 text-indigo-600"></i>
                            </span>
                            <span id="email-chip" style="font-size:13px;font-weight:500"></span>
                        </span>
                        <span class="flex items-center gap-1 text-indigo-600 group-hover:text-indigo-700" style="font-size:12px;font-weight:600">
                            <i data-lucide="arrow-left" class="h-3.5 w-3.5"></i>
                            Change
                        </span>
                    </button>

                    <div class="flex flex-col gap-2">
                        <div id="pw-wrap" class="relative rounded-xl border bg-white transition-all border-gray-200 hover:border-gray-300">
                            <label for="password" class="pointer-events-none absolute left-4 transition-all top-1/2 -translate-y-1/2 text-gray-400" style="font-size:14px;font-weight:500" id="pw-label">Password</label>
                            <input id="password" name="password" type="password" autocomplete="current-password" class="block w-full bg-transparent pl-4 pr-12 pt-6 pb-2 text-gray-900 outline-none" style="font-size:14px;font-weight:400;height:56px" />
                            <button type="button" data-action="toggle-pw" aria-label="Show password" class="absolute right-3 top-1/2 -translate-y-1/2 rounded-md p-1.5 text-gray-400 hover:bg-gray-100 hover:text-gray-700 transition-colors">
                                <i data-lucide="eye" class="h-4 w-4"></i>
                            </button>
                        </div>
                        <div class="min-h-[20px] px-1">
                            <p id="pw-helper" class="text-gray-500" style="font-size:13px">Your password is encrypted and never shared.</p>
                            <div id="pw-error" class="hidden items-center gap-1.5 text-red-600" style="font-size:13px">
                                <i data-lucide="alert-circle" class="h-3.5 w-3.5"></i>
                                <span id="pw-error-text">Password must be at least 6 characters.</span>
                            </div>
                        </div>
                    </div>

                    <div class="mt-3 flex items-center justify-between">
                        <label class="flex cursor-pointer items-center gap-2 select-none">
                            <input id="remember" name="remember" type="checkbox" checked class="h-4 w-4 cursor-pointer rounded border-gray-300 text-indigo-600" />
                            <span class="text-gray-700" style="font-size:13px;font-weight:500">Remember me</span>
                        </label>
                        <a href="#" class="text-indigo-600 hover:text-indigo-700 transition-colors" style="font-size:13px;font-weight:600">Forgot password?</a>
                    </div>

                    <button id="pw_submit" runat="server" type="submit" onserverclick="PwSubmit_Click" ClientIDMode="Static" disabled="disabled" class="mt-6 group flex w-full items-center justify-center gap-2 rounded-xl px-4 transition-all h-12 bg-indigo-300 text-white cursor-not-allowed" style="font-size:15px;font-weight:600">
                        <span id="pw-submit-label">Sign in</span>
                        <i data-lucide="arrow-right" class="h-4 w-4 transition-transform group-hover:translate-x-0.5"></i>
                    </button>

                    <p class="mt-6 text-center text-gray-400" style="font-size:12px">Protected by INTI Identity &middot; End-to-end encrypted</p>
                </div>
            </div>

        </div>

        <div class="mt-8 flex items-center justify-between px-1 text-gray-500" style="font-size:13px">
            <a href="#" class="hover:text-indigo-600 transition-colors">Need help signing in?</a>
            <a href="#" class="hover:text-indigo-600 transition-colors">Contact IT Support</a>
        </div>
    </div>

    <asp:HiddenField ID="ShowPwCard"       runat="server" ClientIDMode="Static" Value="" />
    <asp:HiddenField ID="StoredEmail"      runat="server" ClientIDMode="Static" Value="" />
    <asp:HiddenField ID="ServerEmailError" runat="server" ClientIDMode="Static" Value="" />
    <asp:HiddenField ID="ServerPwError"    runat="server" ClientIDMode="Static" Value="" />
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/login/login.js") %>"></script>
</asp:Content>
