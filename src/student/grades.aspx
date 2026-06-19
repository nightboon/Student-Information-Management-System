<%@ Page Language="C#" MasterPageFile="~/student/StudentLayout.master" AutoEventWireup="true" CodeBehind="grades.aspx.cs" Inherits="src.student.grade" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <%-- Header --%>
    <div class="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Academic record</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700;letter-spacing:-0.01em">Grades</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">
                Your performance across courses and semesters.
            </p>
        </div>
        <div class="flex items-center gap-2">
            <span class="inline-flex items-center gap-2 rounded-full bg-slate-100 px-3 py-1 text-slate-700" style="font-size:12px;font-weight:600">
                <i data-lucide="calendar" class="h-3.5 w-3.5"></i>
                <%: FilterLabel %>
            </span>
            <button type="button" class="inline-flex items-center gap-2 rounded-md border border-slate-200 bg-white px-3 h-10 text-slate-700 hover:bg-slate-50 transition-colors" style="font-size:13px;font-weight:600">
                <i data-lucide="download" class="h-4 w-4"></i> Transcript
            </button>
        </div>
    </div>

    <%-- Headline KPIs --%>
    <section class="mt-6 grid gap-4 lg:grid-cols-4">
        <%-- CGPA hero --%>
        <div class="lg:col-span-2 rounded-lg border border-slate-200 bg-gradient-to-br from-[#e0162b] to-[#a01020] p-6 text-white relative overflow-hidden">
            <div class="pointer-events-none absolute -top-10 -right-10 h-48 w-48 rounded-full bg-white/10 blur-3xl"></div>
            <div class="relative flex items-start justify-between">
                <div>
                    <p class="text-white/80" style="font-size:11.5px;font-weight:600;letter-spacing:0.08em">CGPA &middot; FILTERED</p>
                    <p class="mt-2 text-white" style="font-size:56px;font-weight:800;letter-spacing:-0.02em;line-height:1"><%: CgpaDisplay %></p>
                    <p class="mt-2 text-white/80" style="font-size:13px"><%: CgpaSubtext %></p>
                </div>
                <span class="inline-flex items-center gap-1 rounded border bg-white/15 backdrop-blur px-2.5 py-1 text-white border-white/25" style="font-size:11.5px;font-weight:700">
                    <i data-lucide="trophy" class="h-3.5 w-3.5"></i> <%: StandingDisplay %>
                </span>
            </div>
            <div class="mt-5 grid grid-cols-2 gap-4 border-t border-white/15 pt-4">
                <div>
                    <p class="text-white/70" style="font-size:11px;font-weight:600;letter-spacing:0.06em">CREDITS EARNED</p>
                    <p class="mt-1 text-white" style="font-size:20px;font-weight:700"><%: CreditsEarnedDisplay %><span class="text-white/60" style="font-size:13px"> / <%: CreditsAttemptedDisplay %></span></p>
                </div>
                <div>
                    <p class="text-white/70" style="font-size:11px;font-weight:600;letter-spacing:0.06em">CURRENT GPA</p>
                    <p class="mt-1 text-white" style="font-size:20px;font-weight:700"><%: CurrentGpaDisplay %><span class="text-white/60" style="font-size:13px"> &middot; <%: CurrentGpaContext %></span></p>
                </div>
            </div>
        </div>

        <%-- Stat: Best semester --%>
        <div class="rounded-lg border border-slate-200 bg-white p-5">
            <div class="flex items-center gap-2 text-slate-500">
                <span class="flex h-7 w-7 items-center justify-center rounded-md bg-slate-100 text-slate-600">
                    <i data-lucide="trending-up" class="h-4 w-4"></i>
                </span>
                <p style="font-size:11px;font-weight:600;letter-spacing:0.06em">BEST SEMESTER</p>
            </div>
            <p class="mt-2 text-slate-900" style="font-size:22px;font-weight:700;letter-spacing:-0.01em"><%: BestSemesterDisplay %></p>
            <p class="mt-0.5 text-slate-400" style="font-size:11.5px"><%: BestSemesterSubtext %></p>
        </div>

        <%-- Stat: Courses graded --%>
        <div class="rounded-lg border border-slate-200 bg-white p-5">
            <div class="flex items-center gap-2 text-slate-500">
                <span class="flex h-7 w-7 items-center justify-center rounded-md bg-slate-100 text-slate-600">
                    <i data-lucide="book-open" class="h-4 w-4"></i>
                </span>
                <p style="font-size:11px;font-weight:600;letter-spacing:0.06em">COURSES GRADED</p>
            </div>
            <p class="mt-2 text-slate-900" style="font-size:22px;font-weight:700;letter-spacing:-0.01em"><%: CoursesGradedDisplay %></p>
            <p class="mt-0.5 text-slate-400" style="font-size:11.5px">Within filter</p>
        </div>
    </section>

    <%-- Charts --%>
    <section class="mt-6 grid gap-4 lg:grid-cols-5">
        <%-- GPA per semester --%>
        <div class="lg:col-span-3 rounded-lg border border-slate-200 bg-white p-6">
            <header class="flex items-center justify-between">
                <div>
                    <h2 class="text-slate-900" style="font-size:15px;font-weight:600">GPA per semester</h2>
                    <p class="text-slate-500 mt-0.5" style="font-size:12.5px">Per-semester GPA across your studies</p>
                </div>
                <span class="inline-flex items-center gap-1.5 text-slate-500" style="font-size:11.5px">
                    <span class="h-2.5 w-2.5 rounded-sm bg-[#e0162b]"></span> GPA (0-4)
                </span>
            </header>
            <ul class="mt-5 space-y-3">
                <asp:Repeater ID="gpaRepeater" runat="server">
                    <ItemTemplate>
                        <li class="grid grid-cols-12 items-center gap-3">
                            <span class="col-span-2 text-slate-700" style="font-size:12.5px;font-weight:600"><%#: SemesterShortLabel(Container.DataItem) %></span>
                            <div class="col-span-8 h-3 rounded-full bg-slate-100 overflow-hidden">
                                <div class="h-full rounded-full bg-[#e0162b]" style="<%# GpaBarStyle(Container.DataItem) %>"></div>
                            </div>
                            <span class="col-span-2 text-right text-slate-900" style="font-size:13px;font-weight:700"><%#: FormatGpa(Eval("Gpa")) %></span>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
            <asp:PlaceHolder ID="emptyGpaPanel" runat="server" Visible="false">
                <div class="mt-5 rounded-md border border-dashed border-slate-200 bg-slate-50 px-4 py-6 text-center text-slate-500" style="font-size:13px">
                    No published semester GPA is available yet.
                </div>
            </asp:PlaceHolder>
        </div>

        <%-- Grade distribution --%>
        <div class="lg:col-span-2 rounded-lg border border-slate-200 bg-white p-6">
            <header class="flex items-center justify-between">
                <div>
                    <h2 class="text-slate-900" style="font-size:15px;font-weight:600">Grade distribution</h2>
                    <p class="text-slate-500 mt-0.5" style="font-size:12.5px">Letter grades within filter</p>
                </div>
            </header>
            <ul class="mt-5 space-y-3">
                <asp:Repeater ID="distributionRepeater" runat="server">
                    <ItemTemplate>
                        <li class="grid grid-cols-12 items-center gap-3">
                            <span class="col-span-3 inline-flex items-center justify-center rounded border px-2 py-0.5" style='<%# GradeBadgeStyle(Eval("Grade")) %>'><%#: Eval("Grade") %></span>
                            <div class="col-span-7 h-3 rounded-full bg-slate-100 overflow-hidden">
                                <div class="h-full rounded-full" style="<%# DistributionBarStyle(Container.DataItem) %>"></div>
                            </div>
                            <span class="col-span-2 text-right text-slate-900" style="font-size:12.5px;font-weight:700"><%#: Eval("Count") %></span>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
            <asp:PlaceHolder ID="emptyDistributionPanel" runat="server" Visible="false">
                <div class="mt-5 rounded-md border border-dashed border-slate-200 bg-slate-50 px-4 py-6 text-center text-slate-500" style="font-size:13px">
                    No published letter grades are available yet.
                </div>
            </asp:PlaceHolder>
        </div>
    </section>

    <%-- Semester sections --%>
    <section class="mt-6 space-y-6">
        <asp:Repeater ID="semesterRepeater" runat="server" OnItemDataBound="semesterRepeater_ItemDataBound">
            <ItemTemplate>
                <div class="rounded-lg border border-slate-200 bg-white">
                    <header class="flex flex-col gap-3 border-b border-slate-100 p-5 lg:flex-row lg:items-center lg:justify-between">
                        <div>
                            <div class="flex items-center gap-2">
                                <h2 class="text-slate-900" style="font-size:16px;font-weight:600"><%#: SemesterTitle(Container.DataItem) %></h2>
                                <asp:PlaceHolder runat="server" Visible='<%# Eval("IsCurrent") %>'>
                                    <span class="rounded bg-[#e0162b]/10 text-[#a01020] px-1.5 py-0.5" style="font-size:10.5px;font-weight:700">CURRENT</span>
                                </asp:PlaceHolder>
                            </div>
                            <p class="text-slate-500 mt-0.5" style="font-size:12.5px"><%#: SemesterSubtitle(Container.DataItem) %></p>
                        </div>
                        <div class="flex items-center gap-5">
                            <div>
                                <p class="text-slate-400" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">GPA</p>
                                <p class="text-slate-900" style="font-size:22px;font-weight:700;letter-spacing:-0.01em"><%#: FormatGpa(Eval("Gpa")) %></p>
                            </div>
                            <div>
                                <p class="text-slate-400" style="font-size:10.5px;font-weight:600;letter-spacing:0.06em">EARNED</p>
                                <p class="text-slate-900" style="font-size:22px;font-weight:700;letter-spacing:-0.01em"><%#: Eval("EarnedCredits") %><span class="text-slate-400" style="font-size:12px"> cr</span></p>
                            </div>
                        </div>
                    </header>
                    <div class="overflow-x-auto">
                        <table class="w-full">
                            <thead>
                                <tr class="text-slate-500" style="font-size:11px;font-weight:600;letter-spacing:0.04em">
                                    <th class="text-left px-5 py-3">COURSE</th>
                                    <th class="text-center px-2 py-3">CREDITS</th>
                                    <th class="text-center px-2 py-3">CURRENT</th>
                                    <th class="text-center px-2 py-3">FINAL EXAM</th>
                                    <th class="text-center px-2 py-3">GRADE</th>
                                    <th class="text-right px-5 py-3">GP</th>
                                </tr>
                            </thead>
                            <tbody class="divide-y divide-slate-100">
                                <asp:Repeater ID="coursesRepeater" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="px-5 py-3.5">
                                                <div class="flex items-center gap-3">
                                                    <span class="h-9 w-1 rounded-sm shrink-0" style='<%# CourseColorStyle(Eval("Color")) %>'></span>
                                                    <div class="min-w-0">
                                                        <div class="flex items-center gap-2">
                                                            <span class="rounded bg-slate-100 text-slate-600 px-1.5 py-0.5" style="font-size:10.5px;font-weight:700"><%#: Eval("CourseCode") %></span>
                                                            <span class="text-slate-900 truncate" style="font-size:13.5px;font-weight:600"><%#: Eval("CourseName") %></span>
                                                        </div>
                                                        <p class="text-slate-500 truncate mt-0.5" style="font-size:11.5px"><%#: LecturerDisplay(Eval("LecturerName")) %></p>
                                                    </div>
                                                </div>
                                            </td>
                                            <td class="px-2 py-3.5 text-center text-slate-700" style="font-size:12.5px;font-weight:600"><%#: Eval("CreditHours") %></td>
                                            <td class="px-2 py-3.5 text-center">
                                                <div>
                                                    <span class="text-slate-900" style="font-size:13.5px;font-weight:700"><%#: CurrentScoreDisplay(Container.DataItem) %></span>
                                                    <p class="text-slate-400" style="font-size:10.5px"><%#: CurrentScoreSubtext(Container.DataItem) %></p>
                                                </div>
                                            </td>
                                            <td class="px-2 py-3.5 text-center">
                                                <span class="<%# FinalExamCss(Container.DataItem) %>" style="font-size:10.5px;font-weight:600"><%#: FinalExamDisplay(Container.DataItem) %></span>
                                            </td>
                                            <td class="px-2 py-3.5 text-center">
                                                <span class="inline-flex items-center justify-center rounded border px-2 py-0.5" style='<%# GradeBadgeStyle(Eval("LetterGrade")) %>'><%#: LetterGradeDisplay(Container.DataItem) %></span>
                                            </td>
                                            <td class="px-5 py-3.5 text-right text-slate-900" style="font-size:13.5px;font-weight:700"><%#: GradePointDisplay(Container.DataItem) %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <asp:PlaceHolder ID="emptyGradesPanel" runat="server" Visible="false">
            <div class="rounded-lg border border-dashed border-slate-200 bg-white px-6 py-10 text-center">
                <p class="text-slate-900" style="font-size:15px;font-weight:600">No grade records yet</p>
                <p class="mt-1 text-slate-500" style="font-size:13px">Grades will appear after your enrolments and assessment records are published.</p>
            </div>
        </asp:PlaceHolder>
    </section>

</asp:Content>
