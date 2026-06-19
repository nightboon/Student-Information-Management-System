<%@ Page Language="C#" MasterPageFile="~/admin/AdminLayout.master" AutoEventWireup="true" CodeBehind="student_detail.aspx.cs" Inherits="src.admin.student_detail" Title="Student Detail - INTI Admin Portal" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <a href="<%= ResolveUrl("~/admin/academic_performance.aspx") %>" class="inline-flex items-center gap-1.5 text-slate-600 hover:text-[#a01020] transition-colors" style="font-size:13px;font-weight:600">
        <i data-lucide="arrow-left" class="h-4 w-4"></i> Back to Academic Performance
    </a>

    <div class="mt-4">

        <%-- Header card --%>
        <section class="rounded-lg border border-slate-200 bg-white">
            <div class="flex flex-col gap-5 px-6 py-6 lg:flex-row lg:items-start lg:justify-between">
                <div class="flex items-start gap-4">
                    <div class="flex h-16 w-16 shrink-0 items-center justify-center rounded-2xl bg-[#e0162b]/10 text-[#a01020]" style="font-size:22px;font-weight:700;letter-spacing:-0.01em">LH</div>
                    <div>
                        <div class="flex flex-wrap items-center gap-2">
                            <h1 class="text-slate-900" style="font-size:24px;font-weight:700;letter-spacing:-0.01em">Lee Hui Min</h1>
                            <span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Dean's List</span>
                        </div>
                        <div class="mt-1 text-slate-500" style="font-size:13px">Student ID: <span class="text-slate-700 font-medium">S12101</span></div>
                        <div class="mt-3 flex flex-wrap items-center gap-x-5 gap-y-2 text-slate-600" style="font-size:12.5px">
                            <span class="inline-flex items-center gap-1.5"><i data-lucide="graduation-cap" class="h-4 w-4 text-slate-400"></i> Bachelor of Computer Science</span>
                            <span class="inline-flex items-center gap-1.5"><i data-lucide="calendar-clock" class="h-4 w-4 text-slate-400"></i> Intake Sep 2023</span>
                            <span class="inline-flex items-center gap-1.5"><i data-lucide="mail" class="h-4 w-4 text-slate-400"></i> lee.huimin@student.newinti.edu.my</span>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <%-- Stat strip --%>
        <section class="mt-4" style="display:grid;grid-template-columns:repeat(auto-fit,minmax(200px,1fr));gap:16px">
            <div class="rounded-2xl border border-slate-200 bg-white p-4 hover:border-slate-300 hover:shadow-sm transition-all">
                <div class="flex items-center gap-1.5 text-slate-500" style="font-size:11.5px;font-weight:500"><i data-lucide="activity" class="h-3.5 w-3.5 text-slate-400"></i> Overall CGPA</div>
                <div class="mt-1 text-emerald-600" style="font-size:24px;font-weight:700;letter-spacing:-0.01em">3.95</div>
                <div class="text-slate-400" style="font-size:11.5px">Semester 5</div>
            </div>
            <div class="rounded-2xl border border-slate-200 bg-white p-4 hover:border-slate-300 hover:shadow-sm transition-all">
                <div class="flex items-center gap-1.5 text-slate-500" style="font-size:11.5px;font-weight:500"><i data-lucide="activity" class="h-3.5 w-3.5 text-slate-400"></i> Current Sem GPA</div>
                <div class="mt-1 text-emerald-600" style="font-size:24px;font-weight:700;letter-spacing:-0.01em">3.98</div>
                <div class="text-slate-400" style="font-size:11.5px">Sep 2025</div>
            </div>
            <div class="rounded-2xl border border-slate-200 bg-white p-4 hover:border-slate-300 hover:shadow-sm transition-all">
                <div class="flex items-center gap-1.5 text-slate-500" style="font-size:11.5px;font-weight:500"><i data-lucide="activity" class="h-3.5 w-3.5 text-slate-400"></i> Courses Completed</div>
                <div class="mt-1 text-slate-900" style="font-size:24px;font-weight:700;letter-spacing:-0.01em">20</div>
                <div class="text-slate-400" style="font-size:11.5px">58 credit hrs</div>
            </div>
            <div class="rounded-2xl border border-slate-200 bg-white p-4 hover:border-slate-300 hover:shadow-sm transition-all">
                <div class="flex items-center gap-1.5 text-slate-500" style="font-size:11.5px;font-weight:500"><i data-lucide="activity" class="h-3.5 w-3.5 text-slate-400"></i> Attendance</div>
                <div class="mt-1 text-emerald-600" style="font-size:24px;font-weight:700;letter-spacing:-0.01em">97%</div>
                <div class="text-slate-400" style="font-size:11.5px">all semesters</div>
            </div>
        </section>

        <%-- CGPA trend --%>
        <section class="mt-6 rounded-lg border border-slate-200 bg-white">
            <div class="border-b border-slate-100 px-6 py-4"><h2 class="text-slate-900" style="font-size:15px;font-weight:700">CGPA Trend</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">Cumulative GPA vs semester GPA across all completed semesters.</p></div>
            <div class="px-6 py-6">
                <div class="w-full overflow-x-auto">
                    <svg viewBox="0 0 720 220" class="w-full min-w-[640px]">
                        <line x1="36" x2="696" y1="184" y2="184" stroke="#f1f5f9" stroke-width="1" /><text x="28" y="188" text-anchor="end" font-size="10" fill="#94a3b8">0.0</text>
                        <line x1="36" x2="696" y1="143" y2="143" stroke="#f1f5f9" stroke-width="1" /><text x="28" y="147" text-anchor="end" font-size="10" fill="#94a3b8">1.0</text>
                        <line x1="36" x2="696" y1="102" y2="102" stroke="#f1f5f9" stroke-width="1" /><text x="28" y="106" text-anchor="end" font-size="10" fill="#94a3b8">2.0</text>
                        <line x1="36" x2="696" y1="61" y2="61" stroke="#f1f5f9" stroke-width="1" /><text x="28" y="65" text-anchor="end" font-size="10" fill="#94a3b8">3.0</text>
                        <line x1="36" x2="696" y1="20" y2="20" stroke="#f1f5f9" stroke-width="1" /><text x="28" y="24" text-anchor="end" font-size="10" fill="#94a3b8">4.0</text>
                        <text x="36" y="202" text-anchor="middle" font-size="11" fill="#64748b" font-weight="600">Sem 1</text>
                        <text x="201" y="202" text-anchor="middle" font-size="11" fill="#64748b" font-weight="600">Sem 2</text>
                        <text x="366" y="202" text-anchor="middle" font-size="11" fill="#64748b" font-weight="600">Sem 3</text>
                        <text x="531" y="202" text-anchor="middle" font-size="11" fill="#64748b" font-weight="600">Sem 4</text>
                        <text x="696" y="202" text-anchor="middle" font-size="11" fill="#64748b" font-weight="600">Sem 5</text>
                        <path d="M 36 23.28 L 201 22.05 L 366 24.1 L 531 20 L 696 20.82" fill="none" stroke="#cbd5e1" stroke-width="2" stroke-dasharray="4 4" />
                        <path d="M 36 23.28 L 201 22.87 L 366 23.28 L 531 22.46 L 696 22.05" fill="none" stroke="#e0162b" stroke-width="2.5" />
                        <circle cx="36" cy="23.28" r="3.5" fill="#fff" stroke="#94a3b8" stroke-width="2" /><circle cx="36" cy="23.28" r="5" fill="#fff" stroke="#e0162b" stroke-width="2.5" />
                        <circle cx="201" cy="22.05" r="3.5" fill="#fff" stroke="#94a3b8" stroke-width="2" /><circle cx="201" cy="22.87" r="5" fill="#fff" stroke="#e0162b" stroke-width="2.5" />
                        <circle cx="366" cy="24.1" r="3.5" fill="#fff" stroke="#94a3b8" stroke-width="2" /><circle cx="366" cy="23.28" r="5" fill="#fff" stroke="#e0162b" stroke-width="2.5" />
                        <circle cx="531" cy="20" r="3.5" fill="#fff" stroke="#94a3b8" stroke-width="2" /><circle cx="531" cy="22.46" r="5" fill="#fff" stroke="#e0162b" stroke-width="2.5" />
                        <circle cx="696" cy="20.82" r="3.5" fill="#fff" stroke="#94a3b8" stroke-width="2" /><circle cx="696" cy="22.05" r="5" fill="#fff" stroke="#e0162b" stroke-width="2.5" />
                    </svg>
                    <div class="mt-2 flex items-center gap-4 px-2 text-slate-500" style="font-size:12px">
                        <span class="inline-flex items-center gap-1.5"><span class="h-0.5 w-5 bg-[#e0162b]"></span> CGPA</span>
                        <span class="inline-flex items-center gap-1.5"><span class="h-0.5 w-5 border-t-2 border-dashed border-slate-300"></span> Semester GPA</span>
                    </div>
                </div>
            </div>
        </section>

        <%-- Award alert --%>
        <div class="mt-6 flex items-start gap-3 rounded-lg border border-emerald-100 bg-emerald-50 px-5 py-4">
            <i data-lucide="trophy" class="h-5 w-5 shrink-0 text-emerald-600"></i>
            <div>
                <div class="flex items-center gap-2"><div class="text-slate-900" style="font-size:13.5px;font-weight:700">Top Performer</div>
                    <span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Awarded</span></div>
                <div class="mt-0.5 text-slate-600" style="font-size:12.5px">Dean's List × 3 · President's Award 2025</div>
            </div>
        </div>

        <%-- Attendance by semester --%>
        <section class="mt-6 rounded-lg border border-slate-200 bg-white">
            <div class="flex items-center justify-between border-b border-slate-100 px-6 py-4">
                <div><h2 class="text-slate-900" style="font-size:15px;font-weight:700">Attendance by Semester</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">Per-course attendance rate across each semester.</p></div>
                <div class="inline-flex items-center gap-1.5 text-slate-500" style="font-size:12.5px"><i data-lucide="calendar-check" class="h-4 w-4 text-slate-400"></i> Overall 97%</div>
            </div>
            <ul class="divide-y divide-slate-100">

                <li>
                    <div class="flex w-full items-center justify-between gap-3 px-6 py-3">
                        <div class="flex items-center gap-3">
                            <i data-lucide="chevron-down" class="h-4 w-4 text-slate-500"></i>
                            <div><div class="text-slate-900" style="font-size:13.5px;font-weight:700">Semester 1</div>
                            <div class="text-slate-500" style="font-size:12px">Sep 2023 &middot; 4 courses</div></div>
                        </div>
                        <div class="flex items-center gap-3">
                            <div class="h-1.5 w-28 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:98.3%"></div></div>
                            <span class="tabular-nums text-slate-700" style="font-size:13px;font-weight:600">98.3%</span>
                            <span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span>
                        </div>
                    </div>
                    <div>
                        <div class="overflow-x-auto"><table class="min-w-full">
                            <thead class="text-slate-500"><tr>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Code</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Course</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Attendance</th>
                                <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Rate</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Status</th>
                            </tr></thead><tbody>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC1010</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Intro to Programming</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:100%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">100.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">MAT1011</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Calculus I</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:100%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">100.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">ENG1001</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Academic English</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:96%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">96.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">ITN1001</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Computer Fundamentals</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:97%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">97.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                            </tbody></table></div>
                    </div>
                </li>

                <li>
                    <div class="flex w-full items-center justify-between gap-3 px-6 py-3">
                        <div class="flex items-center gap-3">
                            <i data-lucide="chevron-down" class="h-4 w-4 text-slate-500"></i>
                            <div><div class="text-slate-900" style="font-size:13.5px;font-weight:700">Semester 2</div>
                            <div class="text-slate-500" style="font-size:12px">Feb 2024 &middot; 4 courses</div></div>
                        </div>
                        <div class="flex items-center gap-3">
                            <div class="h-1.5 w-28 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:97%"></div></div>
                            <span class="tabular-nums text-slate-700" style="font-size:13px;font-weight:600">97.0%</span>
                            <span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span>
                        </div>
                    </div>
                    <div>
                        <div class="overflow-x-auto"><table class="min-w-full">
                            <thead class="text-slate-500"><tr>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Code</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Course</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Attendance</th>
                                <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Rate</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Status</th>
                            </tr></thead><tbody>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC1020</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Data Structures</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:100%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">100.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC1030</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Discrete Math</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:98%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">98.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">ITN1020</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Web Development</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:90%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">90.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">MAT1020</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Linear Algebra</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:100%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">100.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                            </tbody></table></div>
                    </div>
                </li>

                <li>
                    <div class="flex w-full items-center justify-between gap-3 px-6 py-3">
                        <div class="flex items-center gap-3">
                            <i data-lucide="chevron-down" class="h-4 w-4 text-slate-500"></i>
                            <div><div class="text-slate-900" style="font-size:13.5px;font-weight:700">Semester 3</div>
                            <div class="text-slate-500" style="font-size:12px">Sep 2024 &middot; 4 courses</div></div>
                        </div>
                        <div class="flex items-center gap-3">
                            <div class="h-1.5 w-28 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:94.8%"></div></div>
                            <span class="tabular-nums text-slate-700" style="font-size:13px;font-weight:600">94.8%</span>
                            <span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span>
                        </div>
                    </div>
                    <div>
                        <div class="overflow-x-auto"><table class="min-w-full">
                            <thead class="text-slate-500"><tr>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Code</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Course</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Attendance</th>
                                <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Rate</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Status</th>
                            </tr></thead><tbody>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC2010</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Algorithms</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:94%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">94.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC2020</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Database Systems</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:91%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">91.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">ITN2010</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Networking I</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:100%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">100.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">BUS2001</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Business Communication</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:94%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">94.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                            </tbody></table></div>
                    </div>
                </li>

                <li>
                    <div class="flex w-full items-center justify-between gap-3 px-6 py-3">
                        <div class="flex items-center gap-3">
                            <i data-lucide="chevron-down" class="h-4 w-4 text-slate-500"></i>
                            <div><div class="text-slate-900" style="font-size:13.5px;font-weight:700">Semester 4</div>
                            <div class="text-slate-500" style="font-size:12px">Feb 2025 &middot; 4 courses</div></div>
                        </div>
                        <div class="flex items-center gap-3">
                            <div class="h-1.5 w-28 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:97.3%"></div></div>
                            <span class="tabular-nums text-slate-700" style="font-size:13px;font-weight:600">97.3%</span>
                            <span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span>
                        </div>
                    </div>
                    <div>
                        <div class="overflow-x-auto"><table class="min-w-full">
                            <thead class="text-slate-500"><tr>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Code</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Course</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Attendance</th>
                                <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Rate</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Status</th>
                            </tr></thead><tbody>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC2024</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Mobile App Dev</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:95%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">95.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC2030</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Machine Learning</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:100%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">100.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">ITN2030</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Cloud Computing</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:94%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">94.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC2040</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Operating Systems</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:100%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">100.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                            </tbody></table></div>
                    </div>
                </li>

                <li>
                    <div class="flex w-full items-center justify-between gap-3 px-6 py-3">
                        <div class="flex items-center gap-3">
                            <i data-lucide="chevron-down" class="h-4 w-4 text-slate-500"></i>
                            <div><div class="text-slate-900" style="font-size:13.5px;font-weight:700">Semester 5</div>
                            <div class="text-slate-500" style="font-size:12px">Sep 2025 &middot; 4 courses</div></div>
                        </div>
                        <div class="flex items-center gap-3">
                            <div class="h-1.5 w-28 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:96.3%"></div></div>
                            <span class="tabular-nums text-slate-700" style="font-size:13px;font-weight:600">96.3%</span>
                            <span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span>
                        </div>
                    </div>
                    <div>
                        <div class="overflow-x-auto"><table class="min-w-full">
                            <thead class="text-slate-500"><tr>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Code</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Course</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Attendance</th>
                                <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Rate</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Status</th>
                            </tr></thead><tbody>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC3010</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Software Engineering</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:100%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">100.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC3020</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">AI Foundations</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:98%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">98.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC3030</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Cybersecurity</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:95%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">95.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC3040</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Capstone I</span></td><td class="px-6 py-3" style="font-size:12.5px"><div class="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full bg-emerald-500" style="width:92%"></div></div></td><td class="px-6 py-3 text-right tabular-nums" style="font-size:12.5px">92.0%</td><td class="px-6 py-3" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">Healthy</span></td></tr>
                            </tbody></table></div>
                    </div>
                </li>

            </ul>
        </section>

        <%-- Semester results --%>
        <section class="mt-6 rounded-lg border border-slate-200 bg-white">
            <div class="border-b border-slate-100 px-6 py-4"><h2 class="text-slate-900" style="font-size:15px;font-weight:700">Semester Results</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">Expand each semester to view course grades.</p></div>
            <ul class="divide-y divide-slate-100">

                <li>
                    <div class="flex w-full items-center justify-between gap-3 px-6 py-4">
                        <div class="flex items-center gap-3">
                            <i data-lucide="chevron-down" class="h-4 w-4 text-slate-500"></i>
                            <div><div class="text-slate-900" style="font-size:14px;font-weight:700">Semester 1</div>
                            <div class="text-slate-500" style="font-size:12px">Sep 2023 &middot; 4 courses</div></div>
                        </div>
                        <div class="flex items-center gap-6">
                            <div class="text-right"><div class="text-slate-400 uppercase" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">Sem GPA</div><div class="text-emerald-600" style="font-size:16px;font-weight:700">3.92</div></div>
                            <div class="text-right"><div class="text-slate-400 uppercase" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">CGPA</div><div class="text-emerald-600" style="font-size:16px;font-weight:700">3.92</div></div>
                        </div>
                    </div>
                    <div>
                        <div class="border-t border-slate-100 bg-slate-50/40"><div class="overflow-x-auto"><table class="min-w-full">
                            <thead class="text-slate-500"><tr>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Code</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Course</th>
                                <th class="px-6 py-3 text-center uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Credits</th>
                                <th class="px-6 py-3 text-center uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Grade</th>
                                <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">GPA Points</th>
                            </tr></thead><tbody>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC1010</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Intro to Programming</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">MAT1011</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Calculus I</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">ENG1001</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Academic English</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">2</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A-</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">3.70</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">ITN1001</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Computer Fundamentals</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                            </tbody></table></div></div>
                    </div>
                </li>

                <li>
                    <div class="flex w-full items-center justify-between gap-3 px-6 py-4">
                        <div class="flex items-center gap-3">
                            <i data-lucide="chevron-down" class="h-4 w-4 text-slate-500"></i>
                            <div><div class="text-slate-900" style="font-size:14px;font-weight:700">Semester 2</div>
                            <div class="text-slate-500" style="font-size:12px">Feb 2024 &middot; 4 courses</div></div>
                        </div>
                        <div class="flex items-center gap-6">
                            <div class="text-right"><div class="text-slate-400 uppercase" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">Sem GPA</div><div class="text-emerald-600" style="font-size:16px;font-weight:700">3.95</div></div>
                            <div class="text-right"><div class="text-slate-400 uppercase" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">CGPA</div><div class="text-emerald-600" style="font-size:16px;font-weight:700">3.93</div></div>
                        </div>
                    </div>
                    <div>
                        <div class="border-t border-slate-100 bg-slate-50/40"><div class="overflow-x-auto"><table class="min-w-full">
                            <thead class="text-slate-500"><tr>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Code</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Course</th>
                                <th class="px-6 py-3 text-center uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Credits</th>
                                <th class="px-6 py-3 text-center uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Grade</th>
                                <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">GPA Points</th>
                            </tr></thead><tbody>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC1020</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Data Structures</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC1030</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Discrete Math</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">ITN1020</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Web Development</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A-</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">3.70</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">MAT1020</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Linear Algebra</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                            </tbody></table></div></div>
                    </div>
                </li>

                <li>
                    <div class="flex w-full items-center justify-between gap-3 px-6 py-4">
                        <div class="flex items-center gap-3">
                            <i data-lucide="chevron-down" class="h-4 w-4 text-slate-500"></i>
                            <div><div class="text-slate-900" style="font-size:14px;font-weight:700">Semester 3</div>
                            <div class="text-slate-500" style="font-size:12px">Sep 2024 &middot; 4 courses</div></div>
                        </div>
                        <div class="flex items-center gap-6">
                            <div class="text-right"><div class="text-slate-400 uppercase" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">Sem GPA</div><div class="text-emerald-600" style="font-size:16px;font-weight:700">3.90</div></div>
                            <div class="text-right"><div class="text-slate-400 uppercase" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">CGPA</div><div class="text-emerald-600" style="font-size:16px;font-weight:700">3.92</div></div>
                        </div>
                    </div>
                    <div>
                        <div class="border-t border-slate-100 bg-slate-50/40"><div class="overflow-x-auto"><table class="min-w-full">
                            <thead class="text-slate-500"><tr>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Code</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Course</th>
                                <th class="px-6 py-3 text-center uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Credits</th>
                                <th class="px-6 py-3 text-center uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Grade</th>
                                <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">GPA Points</th>
                            </tr></thead><tbody>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC2010</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Algorithms</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC2020</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Database Systems</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A-</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">3.70</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">ITN2010</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Networking I</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">BUS2001</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Business Communication</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">2</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                            </tbody></table></div></div>
                    </div>
                </li>

                <li>
                    <div class="flex w-full items-center justify-between gap-3 px-6 py-4">
                        <div class="flex items-center gap-3">
                            <i data-lucide="chevron-down" class="h-4 w-4 text-slate-500"></i>
                            <div><div class="text-slate-900" style="font-size:14px;font-weight:700">Semester 4</div>
                            <div class="text-slate-500" style="font-size:12px">Feb 2025 &middot; 4 courses</div></div>
                        </div>
                        <div class="flex items-center gap-6">
                            <div class="text-right"><div class="text-slate-400 uppercase" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">Sem GPA</div><div class="text-emerald-600" style="font-size:16px;font-weight:700">4.00</div></div>
                            <div class="text-right"><div class="text-slate-400 uppercase" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">CGPA</div><div class="text-emerald-600" style="font-size:16px;font-weight:700">3.94</div></div>
                        </div>
                    </div>
                    <div>
                        <div class="border-t border-slate-100 bg-slate-50/40"><div class="overflow-x-auto"><table class="min-w-full">
                            <thead class="text-slate-500"><tr>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Code</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Course</th>
                                <th class="px-6 py-3 text-center uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Credits</th>
                                <th class="px-6 py-3 text-center uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Grade</th>
                                <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">GPA Points</th>
                            </tr></thead><tbody>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC2024</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Mobile App Dev</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC2030</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Machine Learning</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">ITN2030</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Cloud Computing</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC2040</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Operating Systems</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                            </tbody></table></div></div>
                    </div>
                </li>

                <li>
                    <div class="flex w-full items-center justify-between gap-3 px-6 py-4">
                        <div class="flex items-center gap-3">
                            <i data-lucide="chevron-down" class="h-4 w-4 text-slate-500"></i>
                            <div><div class="text-slate-900" style="font-size:14px;font-weight:700">Semester 5</div>
                            <div class="text-slate-500" style="font-size:12px">Sep 2025 &middot; 4 courses</div></div>
                        </div>
                        <div class="flex items-center gap-6">
                            <div class="text-right"><div class="text-slate-400 uppercase" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">Sem GPA</div><div class="text-emerald-600" style="font-size:16px;font-weight:700">3.98</div></div>
                            <div class="text-right"><div class="text-slate-400 uppercase" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">CGPA</div><div class="text-emerald-600" style="font-size:16px;font-weight:700">3.95</div></div>
                        </div>
                    </div>
                    <div>
                        <div class="border-t border-slate-100 bg-slate-50/40"><div class="overflow-x-auto"><table class="min-w-full">
                            <thead class="text-slate-500"><tr>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Code</th>
                                <th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Course</th>
                                <th class="px-6 py-3 text-center uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Credits</th>
                                <th class="px-6 py-3 text-center uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Grade</th>
                                <th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">GPA Points</th>
                            </tr></thead><tbody>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC3010</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Software Engineering</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC3020</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">AI Foundations</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC3030</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Cybersecurity</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A-</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">3.70</td></tr>
                                <tr class="border-t border-slate-100"><td class="px-6 py-3 text-slate-500 font-medium" style="font-size:12.5px">CSC3040</td><td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">Capstone I</span></td><td class="px-6 py-3 text-center text-slate-700" style="font-size:12.5px">3</td><td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100" style="font-size:11.5px;font-weight:600">A</span></td><td class="px-6 py-3 text-right text-slate-700" style="font-size:12.5px">4.00</td></tr>
                            </tbody></table></div></div>
                    </div>
                </li>

            </ul>
        </section>

    </div>

</asp:Content>
<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
</asp:Content>
