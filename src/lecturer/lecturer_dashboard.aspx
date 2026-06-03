<%@ Page Language="C#" MasterPageFile="~/shared/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_dashboard.aspx.cs" Inherits="student_information_management_system.lecturer_dashboard" Title="Dashboard - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <%-- Welcome banner --%>
    <section class="relative overflow-hidden rounded-3xl bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 p-7 lg:p-9 text-white">
        <div class="pointer-events-none absolute -top-20 -right-10 h-72 w-72 rounded-full bg-[#e0162b]/30 blur-3xl"></div>
        <div class="pointer-events-none absolute -bottom-24 left-1/3 h-72 w-72 rounded-full bg-amber-300/15 blur-3xl"></div>
        <div class="relative flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
            <div class="max-w-xl">
                <div class="inline-flex items-center gap-2 rounded-full border border-white/20 bg-white/10 px-3 py-1 backdrop-blur" style="font-size:12px;font-weight:500">
                    <i data-lucide="sparkles" class="h-3.5 w-3.5 text-amber-200"></i>
                    <%= CurrentDateLabel %> &middot; Week <%= SemesterWeek %> of <%= SemesterName %>
                </div>
                <h1 class="mt-4 text-white" style="font-size:32px;font-weight:700;letter-spacing:-0.015em;line-height:1.15">
                    <%= Greeting %>, <%= LecturerName %> &#128075;
                </h1>
                <p class="mt-2 text-white/75" style="font-size:15px;line-height:1.6">
                    You have <span class="text-white font-semibold"><%= ClassesTodayLabel %></span> today and <span class="text-white font-semibold"><%= SubmissionsToReviewLabel %></span> awaiting your review. Keep your students on track.
                </p>
            </div>
            <div class="flex flex-wrap gap-3">
                <button class="inline-flex items-center gap-2 rounded-xl bg-white/10 px-4 h-11 text-white ring-1 ring-white/25 backdrop-blur hover:bg-white/15 transition-colors" style="font-size:14px;font-weight:500">
                    <i data-lucide="calendar-days" class="h-4 w-4"></i>
                    Today's schedule
                </button>
            </div>
        </div>
    </section>

    <%-- Stat cards --%>
    <section class="mt-6 grid grid-cols-2 gap-4 xl:grid-cols-4">

        <%-- Active Courses --%>
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div>
                    <p class="text-slate-500" style="font-size:12.5px;font-weight:500">Active Courses</p>
                    <p class="mt-1.5 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">6</p>
                </div>
                <span class="inline-flex items-center gap-1 rounded-full px-2 py-0.5 bg-emerald-50 text-emerald-700" style="font-size:11px;font-weight:600">
                    <i data-lucide="trending-up" class="h-3 w-3"></i>
                    +1 new
                </span>
            </div>
            <p class="mt-3 text-slate-400" style="font-size:12px">this trimester</p>
        </div>

        <%-- Attendance --%>
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div>
                    <p class="text-slate-500" style="font-size:12.5px;font-weight:500">Attendance</p>
                    <p class="mt-1.5 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">94%</p>
                </div>
                <span class="inline-flex items-center gap-1 rounded-full px-2 py-0.5 bg-emerald-50 text-emerald-700" style="font-size:11px;font-weight:600">
                    <i data-lucide="trending-up" class="h-3 w-3"></i>
                    +2.4%
                </span>
            </div>
            <p class="mt-3 text-slate-400" style="font-size:12px">this term</p>
        </div>

        <%-- Students Taught --%>
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div>
                    <p class="text-slate-500" style="font-size:12.5px;font-weight:500">Students Taught</p>
                    <p class="mt-1.5 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">205</p>
                </div>
                <span class="inline-flex items-center gap-1 rounded-full px-2 py-0.5 bg-slate-100 text-slate-600" style="font-size:11px;font-weight:600">
                    across 6 courses
                </span>
            </div>
            <p class="mt-3 text-slate-400" style="font-size:12px">all enrolled</p>
        </div>

        <%-- Pending Grading --%>
        <div class="group rounded-2xl border border-slate-200 bg-white p-5 hover:border-slate-300 hover:shadow-sm transition-all">
            <div class="flex items-start justify-between">
                <div>
                    <p class="text-slate-500" style="font-size:12.5px;font-weight:500">Pending Grading</p>
                    <p class="mt-1.5 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">3</p>
                </div>
                <span class="inline-flex items-center gap-1 rounded-full px-2 py-0.5 bg-slate-100 text-slate-600" style="font-size:11px;font-weight:600">
                    Due soon
                </span>
            </div>
            <p class="mt-3 text-slate-400" style="font-size:12px">this week</p>
        </div>

    </section>

    <%-- Main grid: Today's Schedule + To Grade --%>
    <section class="mt-6 grid gap-6 lg:grid-cols-3">

        <%-- Today's Schedule --%>
        <div class="lg:col-span-2 rounded-2xl border border-slate-200 bg-white">
            <header class="flex items-center justify-between p-6 pb-4">
                <div>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Today's Schedule</h2>
                    <p class="text-slate-500 mt-0.5" style="font-size:13px">4 classes &middot; 6h 30m total</p>
                </div>
                <a href="#" class="inline-flex items-center gap-1 text-[#e0162b] hover:text-[#a01020] transition-colors" style="font-size:13px;font-weight:600">
                    Full week <i data-lucide="arrow-up-right" class="h-3.5 w-3.5"></i>
                </a>
            </header>
            <ul class="divide-y divide-slate-100">

                <%-- Software Engineering — status: now --%>
                <li class="flex items-center gap-4 px-6 py-4 hover:bg-slate-50/60 transition-colors">
                    <div class="w-1.5 h-12 rounded-full" style="background-color:#e0162b"></div>
                    <div class="min-w-0 flex-1">
                        <div class="flex items-center gap-2">
                            <span class="text-slate-900 truncate" style="font-size:14px;font-weight:600">Software Engineering</span>
                            <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600">CSC2104</span>
                            <span class="inline-flex items-center gap-1 rounded-md bg-[#e0162b]/10 px-1.5 py-0.5 text-[#a01020]" style="font-size:10.5px;font-weight:600">
                                <span class="h-1.5 w-1.5 rounded-full bg-[#e0162b] animate-pulse"></span>
                                LIVE
                            </span>
                        </div>
                        <div class="mt-1 flex flex-wrap items-center gap-x-4 gap-y-1 text-slate-500" style="font-size:12.5px">
                            <span class="inline-flex items-center gap-1"><i data-lucide="clock" class="h-3.5 w-3.5"></i>09:00 &ndash; 10:30</span>
                            <span class="inline-flex items-center gap-1"><i data-lucide="map-pin" class="h-3.5 w-3.5"></i>Block C &middot; Lab 3</span>
                        </div>
                    </div>
                    <i data-lucide="chevron-right" class="h-4 w-4 text-slate-300"></i>
                </li>

                <%-- Data Structures — status: next --%>
                <li class="flex items-center gap-4 px-6 py-4 hover:bg-slate-50/60 transition-colors">
                    <div class="w-1.5 h-12 rounded-full" style="background-color:#3b82f6"></div>
                    <div class="min-w-0 flex-1">
                        <div class="flex items-center gap-2">
                            <span class="text-slate-900 truncate" style="font-size:14px;font-weight:600">Data Structures</span>
                            <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600">CSC2103</span>
                        </div>
                        <div class="mt-1 flex flex-wrap items-center gap-x-4 gap-y-1 text-slate-500" style="font-size:12.5px">
                            <span class="inline-flex items-center gap-1"><i data-lucide="clock" class="h-3.5 w-3.5"></i>11:00 &ndash; 12:30</span>
                            <span class="inline-flex items-center gap-1"><i data-lucide="map-pin" class="h-3.5 w-3.5"></i>Block A &middot; Hall 2</span>
                        </div>
                    </div>
                    <i data-lucide="chevron-right" class="h-4 w-4 text-slate-300"></i>
                </li>

                <%-- Discrete Mathematics — status: later --%>
                <li class="flex items-center gap-4 px-6 py-4 hover:bg-slate-50/60 transition-colors">
                    <div class="w-1.5 h-12 rounded-full" style="background-color:#f59e0b"></div>
                    <div class="min-w-0 flex-1">
                        <div class="flex items-center gap-2">
                            <span class="text-slate-900 truncate" style="font-size:14px;font-weight:600">Discrete Mathematics</span>
                            <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600">MTH2101</span>
                        </div>
                        <div class="mt-1 flex flex-wrap items-center gap-x-4 gap-y-1 text-slate-500" style="font-size:12.5px">
                            <span class="inline-flex items-center gap-1"><i data-lucide="clock" class="h-3.5 w-3.5"></i>14:00 &ndash; 15:30</span>
                            <span class="inline-flex items-center gap-1"><i data-lucide="map-pin" class="h-3.5 w-3.5"></i>Block B &middot; 2.04</span>
                        </div>
                    </div>
                    <i data-lucide="chevron-right" class="h-4 w-4 text-slate-300"></i>
                </li>

                <%-- Career Workshop — status: later --%>
                <li class="flex items-center gap-4 px-6 py-4 hover:bg-slate-50/60 transition-colors">
                    <div class="w-1.5 h-12 rounded-full" style="background-color:#10b981"></div>
                    <div class="min-w-0 flex-1">
                        <div class="flex items-center gap-2">
                            <span class="text-slate-900 truncate" style="font-size:14px;font-weight:600">Career Workshop</span>
                            <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600">CAR101</span>
                        </div>
                        <div class="mt-1 flex flex-wrap items-center gap-x-4 gap-y-1 text-slate-500" style="font-size:12.5px">
                            <span class="inline-flex items-center gap-1"><i data-lucide="clock" class="h-3.5 w-3.5"></i>16:00 &ndash; 17:00</span>
                            <span class="inline-flex items-center gap-1"><i data-lucide="map-pin" class="h-3.5 w-3.5"></i>Auditorium</span>
                        </div>
                    </div>
                    <i data-lucide="chevron-right" class="h-4 w-4 text-slate-300"></i>
                </li>

            </ul>
        </div>

        <%-- To Grade --%>
        <div class="rounded-2xl border border-slate-200 bg-white">
            <header class="flex items-center justify-between p-6 pb-4">
                <div>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">To Grade</h2>
                    <p class="text-slate-500 mt-0.5" style="font-size:13px">3 due this week</p>
                </div>
            </header>
            <ul class="space-y-2 px-3 pb-4">

                <%-- ER Diagram Design — urgent --%>
                <li class="flex items-start gap-3 rounded-xl px-3 py-3 hover:bg-slate-50 transition-colors">
                    <span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-[#e0162b]/10 text-[#e0162b]">
                        <i data-lucide="alert-circle" class="h-4 w-4"></i>
                    </span>
                    <div class="min-w-0 flex-1">
                        <p class="text-slate-900 truncate" style="font-size:13.5px;font-weight:600">ER Diagram Design</p>
                        <p class="text-slate-500 mt-0.5" style="font-size:12px">
                            CSC2104 &middot; <span class="text-[#e0162b] font-semibold">Tomorrow &middot; 11:59 PM</span>
                        </p>
                    </div>
                </li>

                <%-- Linked List Implementation — soon --%>
                <li class="flex items-start gap-3 rounded-xl px-3 py-3 hover:bg-slate-50 transition-colors">
                    <span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-amber-50 text-amber-600">
                        <i data-lucide="check-circle-2" class="h-4 w-4"></i>
                    </span>
                    <div class="min-w-0 flex-1">
                        <p class="text-slate-900 truncate" style="font-size:13.5px;font-weight:600">Linked List Implementation</p>
                        <p class="text-slate-500 mt-0.5" style="font-size:12px">
                            CSC2103 &middot; In 3 days
                        </p>
                    </div>
                </li>

                <%-- Logic Proofs Worksheet — ok --%>
                <li class="flex items-start gap-3 rounded-xl px-3 py-3 hover:bg-slate-50 transition-colors">
                    <span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-emerald-50 text-emerald-600">
                        <i data-lucide="check-circle-2" class="h-4 w-4"></i>
                    </span>
                    <div class="min-w-0 flex-1">
                        <p class="text-slate-900 truncate" style="font-size:13.5px;font-weight:600">Logic Proofs Worksheet</p>
                        <p class="text-slate-500 mt-0.5" style="font-size:12px">
                            MTH2101 &middot; In 5 days
                        </p>
                    </div>
                </li>

            </ul>
            <div class="border-t border-slate-100 p-3">
                <button class="w-full rounded-xl py-2.5 text-slate-700 hover:bg-slate-50 transition-colors" style="font-size:13px;font-weight:600">
                    View all submissions
                </button>
            </div>
        </div>

    </section>

    <%-- My Courses + Announcements --%>
    <section class="mt-6 grid gap-6 lg:grid-cols-3">

        <%-- My Courses --%>
        <div class="lg:col-span-2 rounded-2xl border border-slate-200 bg-white p-6">
            <header class="flex items-center justify-between mb-5">
                <div>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">My Courses</h2>
                    <p class="text-slate-500 mt-0.5" style="font-size:13px">Trimester 1 &middot; 2025/26</p>
                </div>
                <a href="/lecturer/academic/courses.aspx" class="inline-flex items-center gap-1 text-[#e0162b] hover:text-[#a01020] transition-colors" style="font-size:13px;font-weight:600">
                    See all <i data-lucide="arrow-up-right" class="h-3.5 w-3.5"></i>
                </a>
            </header>
            <ul class="grid gap-3 sm:grid-cols-2">

                <%-- CSC2104 Software Engineering --%>
                <li class="group rounded-xl border border-slate-200 p-4 hover:border-slate-300 hover:shadow-sm transition-all cursor-pointer">
                    <div class="flex items-center justify-between">
                        <div class="flex h-9 w-9 items-center justify-center rounded-lg" style="background-color:#e0162b15;color:#e0162b">
                            <i data-lucide="book-open" class="h-4 w-4"></i>
                        </div>
                        <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600">CSC2104</span>
                    </div>
                    <p class="mt-3 text-slate-900 line-clamp-1" style="font-size:14px;font-weight:600">Software Engineering</p>
                    <p class="mt-0.5 text-slate-500" style="font-size:12px">60 students enrolled</p>
                </li>

                <%-- CSC2103 Data Structures & Algorithms --%>
                <li class="group rounded-xl border border-slate-200 p-4 hover:border-slate-300 hover:shadow-sm transition-all cursor-pointer">
                    <div class="flex items-center justify-between">
                        <div class="flex h-9 w-9 items-center justify-center rounded-lg" style="background-color:#3b82f615;color:#3b82f6">
                            <i data-lucide="book-open" class="h-4 w-4"></i>
                        </div>
                        <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600">CSC2103</span>
                    </div>
                    <p class="mt-3 text-slate-900 line-clamp-1" style="font-size:14px;font-weight:600">Data Structures &amp; Algorithms</p>
                    <p class="mt-0.5 text-slate-500" style="font-size:12px">60 students enrolled</p>
                </li>

                <%-- MTH2101 Discrete Mathematics --%>
                <li class="group rounded-xl border border-slate-200 p-4 hover:border-slate-300 hover:shadow-sm transition-all cursor-pointer">
                    <div class="flex items-center justify-between">
                        <div class="flex h-9 w-9 items-center justify-center rounded-lg" style="background-color:#f59e0b15;color:#f59e0b">
                            <i data-lucide="book-open" class="h-4 w-4"></i>
                        </div>
                        <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600">MTH2101</span>
                    </div>
                    <p class="mt-3 text-slate-900 line-clamp-1" style="font-size:14px;font-weight:600">Discrete Mathematics</p>
                    <p class="mt-0.5 text-slate-500" style="font-size:12px">Dr. Rajesh K.</p>
                </li>

                <%-- ENG2001 Professional Communication --%>
                <li class="group rounded-xl border border-slate-200 p-4 hover:border-slate-300 hover:shadow-sm transition-all cursor-pointer">
                    <div class="flex items-center justify-between">
                        <div class="flex h-9 w-9 items-center justify-center rounded-lg" style="background-color:#10b98115;color:#10b981">
                            <i data-lucide="book-open" class="h-4 w-4"></i>
                        </div>
                        <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600">ENG2001</span>
                    </div>
                    <p class="mt-3 text-slate-900 line-clamp-1" style="font-size:14px;font-weight:600">Professional Communication</p>
                    <p class="mt-0.5 text-slate-500" style="font-size:12px">60 students enrolled</p>
                </li>

            </ul>
        </div>

        <%-- Announcements --%>
        <div class="rounded-2xl border border-slate-200 bg-white p-6">
            <header class="flex items-center justify-between mb-4">
                <div class="flex items-center gap-2">
                    <span class="flex h-8 w-8 items-center justify-center rounded-lg bg-[#e0162b]/10 text-[#e0162b]">
                        <i data-lucide="megaphone" class="h-4 w-4"></i>
                    </span>
                    <h2 class="text-slate-900" style="font-size:16px;font-weight:600">Announcements</h2>
                </div>
            </header>
            <ul class="space-y-4">

                <%-- Academic --%>
                <li class="border-b border-slate-100 pb-4">
                    <div class="flex items-center gap-2">
                        <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600">Academic</span>
                        <span class="text-slate-400" style="font-size:11.5px">2h ago</span>
                    </div>
                    <p class="mt-2 text-slate-900" style="font-size:13.5px;font-weight:600;line-height:1.45">Mid-semester break: 10&ndash;14 June 2026</p>
                    <p class="mt-1 text-slate-500" style="font-size:12.5px;line-height:1.55">Classes resume Monday, 17 June. Library remains open.</p>
                </li>

                <%-- Event --%>
                <li class="border-b border-slate-100 pb-4">
                    <div class="flex items-center gap-2">
                        <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600">Event</span>
                        <span class="text-slate-400" style="font-size:11.5px">Yesterday</span>
                    </div>
                    <p class="mt-2 text-slate-900" style="font-size:13.5px;font-weight:600;line-height:1.45">INTI Career Fair 2026 &mdash; Register now</p>
                    <p class="mt-1 text-slate-500" style="font-size:12.5px;line-height:1.55">Over 80 employers. Earn 2 co-curricular points.</p>
                </li>

                <%-- System --%>
                <li class="pb-0">
                    <div class="flex items-center gap-2">
                        <span class="rounded-md bg-slate-100 px-1.5 py-0.5 text-slate-600" style="font-size:10.5px;font-weight:600">System</span>
                        <span class="text-slate-400" style="font-size:11.5px">2 days ago</span>
                    </div>
                    <p class="mt-2 text-slate-900" style="font-size:13.5px;font-weight:600;line-height:1.45">Portal maintenance Sunday 1&ndash;3 AM</p>
                    <p class="mt-1 text-slate-500" style="font-size:12.5px;line-height:1.55">Brief downtime expected. Save your work in advance.</p>
                </li>

            </ul>
        </div>

    </section>

</asp:Content>
