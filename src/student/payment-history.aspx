<%@ Page Language="C#" MasterPageFile="~/student/StudentLayout.master" AutoEventWireup="true" CodeBehind="payment-history.aspx.cs" Inherits="src.student.payment_history" Title="Payment History - INTI Student Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <%-- BODY PORTED FROM payment-history-page.tsx --%>

    <%-- Page header --%>
    <div>
        <p class="text-slate-500" style="font-size:13px;font-weight:500">Payment</p>
        <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">
            Payment history
        </h1>
        <p class="mt-1 text-slate-500" style="font-size:14px">
            All transactions on your student account.
        </p>
    </div>

    <%-- Summary stat cards --%>
    <section class="mt-6 grid grid-cols-2 gap-4 lg:grid-cols-4">

        <%-- Total paid --%>
        <div class="rounded-2xl border border-slate-200 bg-white p-5">
            <p class="text-slate-500" style="font-size:12.5px;font-weight:500">Total paid</p>
            <p class="mt-1 text-slate-900" style="font-size:22px;font-weight:700;letter-spacing:-0.01em">RM <asp:Literal ID="litTotalPaid" runat="server" /></p>
            <p class="mt-0.5 text-slate-400" style="font-size:11.5px">Across all semesters</p>
        </div>

        <%-- This year (2026) --%>
        <div class="rounded-2xl border border-slate-200 bg-white p-5">
            <p class="text-slate-500" style="font-size:12.5px;font-weight:500">This year (2026)</p>
            <p class="mt-1 text-slate-900" style="font-size:22px;font-weight:700;letter-spacing:-0.01em">RM <asp:Literal ID="litThisYear" runat="server" /></p>
            <p class="mt-0.5 text-slate-400" style="font-size:11.5px">Paid in 2026</p>
        </div>

        <%-- Refunded --%>
        <div class="rounded-2xl border border-slate-200 bg-white p-5">
            <p class="text-slate-500" style="font-size:12.5px;font-weight:500">Refunded</p>
            <p class="mt-1 text-slate-900" style="font-size:22px;font-weight:700;letter-spacing:-0.01em">RM <asp:Literal ID="litRefunded" runat="server" /></p>
            <p class="mt-0.5 text-slate-400" style="font-size:11.5px">Lifetime</p>
        </div>

        <%-- Receipts --%>
        <div class="rounded-2xl border border-slate-200 bg-white p-5">
            <p class="text-slate-500" style="font-size:12.5px;font-weight:500">Receipts</p>
            <p class="mt-1 text-slate-900" style="font-size:22px;font-weight:700;letter-spacing:-0.01em"><asp:Literal ID="litReceipts" runat="server" /></p>
            <p class="mt-0.5 text-slate-400" style="font-size:11.5px">Total transactions</p>
        </div>

    </section>

    <%-- Transactions table --%>
    <section class="mt-8 rounded-2xl border border-slate-200 bg-white">

        <%-- Table header / filter bar --%>
        <header class="flex flex-col gap-4 p-6 lg:flex-row lg:items-end lg:justify-between">
            <div>
                <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Transactions</h2>
                <p id="pm-count" class="text-slate-500 mt-0.5" style="font-size:13px"><asp:Literal ID="litShown" runat="server" /> shown</p>
            </div>
            <div class="flex flex-wrap items-center gap-2">

                <%-- Method filter --%>
                <div class="relative">
                    <span class="pointer-events-none absolute left-2.5 top-1/2 -translate-y-1/2 text-slate-400">
                        <i data-lucide="credit-card" class="h-3.5 w-3.5"></i>
                    </span>
                    <select id="pm-filter-method"
                            class="h-9 appearance-none rounded-xl border border-slate-200 bg-white pl-7 pr-8 text-slate-700 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10"
                            style="font-size:12.5px;font-weight:500">
                        <option value="all">All methods</option>
                        <option value="Credit / Debit Card">Credit / Debit Card</option>
                        <option value="FPX Online Banking">FPX Online Banking</option>
                        <option value="E-Wallet">E-Wallet</option>
                    </select>
                    <i data-lucide="chevron-down" class="pointer-events-none absolute right-2 top-1/2 -translate-y-1/2 h-3.5 w-3.5 text-slate-400"></i>
                </div>

                <%-- Year filter --%>
                <div class="relative">
                    <span class="pointer-events-none absolute left-2.5 top-1/2 -translate-y-1/2 text-slate-400">
                        <i data-lucide="calendar" class="h-3.5 w-3.5"></i>
                    </span>
                    <select id="pm-filter-year"
                            class="h-9 appearance-none rounded-xl border border-slate-200 bg-white pl-7 pr-8 text-slate-700 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10"
                            style="font-size:12.5px;font-weight:500">
                        <option value="all">All years</option>
                    </select>
                    <i data-lucide="chevron-down" class="pointer-events-none absolute right-2 top-1/2 -translate-y-1/2 h-3.5 w-3.5 text-slate-400"></i>
                </div>

            </div>
        </header>

        <%-- Table --%>
        <div class="overflow-x-auto">
            <table class="w-full">
                <thead>
                    <tr class="text-slate-500" style="font-size:11px;font-weight:600;letter-spacing:0.04em">
                        <th class="text-left px-6 py-3">INVOICE</th>
                        <th class="text-left px-3 py-3">DESCRIPTION</th>
                        <th class="text-left px-3 py-3">DATE</th>
                        <th class="text-left px-3 py-3">METHOD</th>
                        <th class="text-right px-3 py-3">AMOUNT</th>
                        <th class="text-center px-3 py-3">STATUS</th>
                        <th class="text-right px-6 py-3">ACTION</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-slate-100">
                    <asp:Repeater ID="rptPayments" runat="server">
                        <ItemTemplate>
                            <tr class="js-pm-row hover:bg-slate-50/60"
                                data-method="<%# Attr(Eval("Method")) %>"
                                data-year="<%# Attr(Eval("PaidDate", "{0:yyyy}")) %>">
                                <td class="px-6 py-3.5 text-slate-900" style="font-size:12.5px;font-weight:600"><%# Eval("InvoiceNo") %></td>
                                <td class="px-3 py-3.5 text-slate-700" style="font-size:12.5px">
                                    <div><%# Eval("Description") %></div>
                                    <div class="text-slate-400" style="font-size:11px"><%# Eval("TermLabel") %></div>
                                </td>
                                <td class="px-3 py-3.5 text-slate-500" style="font-size:12.5px"><%# Eval("PaidDate", "{0:dd MMM yyyy}") %></td>
                                <td class="px-3 py-3.5 text-slate-700" style="font-size:12.5px"><%# Eval("Method") %></td>
                                <td class="px-3 py-3.5 text-right text-slate-900" style="font-size:12.5px;font-weight:600">RM <%# Eval("Amount", "{0:N2}") %></td>
                                <td class="px-3 py-3.5 text-center">
                                    <span class="inline-flex rounded-md px-2 py-0.5 bg-emerald-50 text-emerald-700" style="font-size:11px;font-weight:600"><%# FormatStatus(Eval("Status")) %></span>
                                </td>
                                <td class="px-6 py-3.5 text-right">
                                    <button type="button"
                                            class="js-invoice-pdf inline-flex items-center gap-1 rounded-lg border border-slate-200 bg-white px-2.5 py-1.5 text-slate-700 hover:bg-slate-50 transition-colors"
                                            style="font-size:11.5px;font-weight:600"
                                            data-invoice="<%# Attr(Eval("InvoiceNo")) %>"
                                            data-desc="<%# Attr(Eval("Description")) %>"
                                            data-term="<%# Attr(Eval("TermLabel")) %>"
                                            data-date="<%# Attr(Eval("PaidDate", "{0:dd MMM yyyy}")) %>"
                                            data-method="<%# Attr(Eval("Method")) %>"
                                            data-amount="<%# Attr(Eval("Amount", "{0:F2}")) %>"
                                            data-status="<%# Attr(Eval("Status")) %>">
                                        <i data-lucide="download" class="h-3.5 w-3.5"></i> PDF
                                    </button>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>

    </section>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">

    <%-- Student identity for the "billed to" section of generated invoices --%>
    <script>
        window.SIMS_STUDENT = {
            name: "<%= System.Web.HttpUtility.JavaScriptStringEncode(StudentName) %>",
            id: "<%= System.Web.HttpUtility.JavaScriptStringEncode(StudentNo) %>",
            programme: "<%= System.Web.HttpUtility.JavaScriptStringEncode(ProgrammeName) %>"
        };
    </script>

    <script src="https://unpkg.com/jspdf@2.5.1/dist/jspdf.umd.min.js"></script>
    <script src="<%= ResolveUrl("~/js/student/payment-history.js") %>?v=3"></script>

</asp:Content>
