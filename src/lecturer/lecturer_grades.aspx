<%@ Page Language="C#" MasterPageFile="~/lecturer/LecturerLayout.master" AutoEventWireup="true" CodeBehind="lecturer_grades.aspx.cs" Inherits="student_information_management_system.lecturer_grades" Title="Marks and Grades - INTI Lecturer Portal" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <style>
        @media print {
            body * {
                visibility: hidden !important;
            }

            [data-review-modal]:not(.hidden),
            [data-review-modal]:not(.hidden) * {
                visibility: visible !important;
            }

            [data-review-modal]:not(.hidden) {
                background: #fff !important;
                inset: 0 !important;
                padding: 0 !important;
                position: fixed !important;
                z-index: 9999 !important;
            }

            [data-review-close],
            [data-annotate-tool],
            [data-annotate-clear],
            [data-annotate-download],
            aside {
                display: none !important;
            }
        }
    </style>
    <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Lecturer Module</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700">Marks &amp; Grades</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">Enter assessment marks, save drafts, and publish final course grades.</p>
        </div>
        <div class="flex flex-col gap-2 sm:flex-row">
            <asp:DropDownList ID="assessmentSelect" runat="server" AutoPostBack="true" OnSelectedIndexChanged="AssessmentChanged"
                CssClass="h-10 rounded-md border border-slate-200 bg-white px-3 text-slate-700" style="font-size:13px;font-weight:600" />
            <asp:LinkButton ID="saveButton" runat="server" OnClick="SaveDraft_Click"
                CssClass="inline-flex h-10 items-center justify-center gap-1.5 rounded-md border border-slate-200 bg-white px-4 text-slate-700 hover:bg-slate-50" style="font-size:13px;font-weight:600">
                <i data-lucide="save" class="h-4 w-4"></i>Save draft
            </asp:LinkButton>
            <asp:LinkButton ID="publishButton" runat="server" OnClick="Publish_Click"
                CssClass="inline-flex h-10 items-center justify-center gap-1.5 rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:600">
                <i data-lucide="send" class="h-4 w-4"></i>Publish
            </asp:LinkButton>
        </div>
    </div>

    <asp:Panel ID="statusBanner" runat="server" Visible="false" CssClass="mt-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800" style="font-size:13px;font-weight:600">
        <asp:Literal ID="statusMessage" runat="server" />
    </asp:Panel>

    <section class="mt-6 grid gap-4 md:grid-cols-4">
        <div class="rounded-lg border border-slate-200 bg-white p-5"><p class="text-slate-500" style="font-size:12.5px">Students</p><p class="mt-1 text-slate-900" style="font-size:28px;font-weight:700"><%= StudentCount %></p></div>
        <div class="rounded-lg border border-slate-200 bg-white p-5"><p class="text-slate-500" style="font-size:12.5px">Marked</p><p class="mt-1 text-emerald-700" style="font-size:28px;font-weight:700"><%= MarksDisplay %></p></div>
        <div class="rounded-lg border border-slate-200 bg-white p-5"><p class="text-slate-500" style="font-size:12.5px">Pending</p><p class="mt-1 text-amber-700" style="font-size:28px;font-weight:700"><%= PendingCount %></p></div>
        <div class="rounded-lg border border-slate-200 bg-white p-5"><p class="text-slate-500" style="font-size:12.5px">Average</p><p class="mt-1 text-slate-900" style="font-size:28px;font-weight:700"><%= AverageDisplay %></p></div>
    </section>

    <section class="mt-6 rounded-lg border border-slate-200 bg-white">
        <div class="flex flex-col gap-3 border-b border-slate-100 px-6 py-4 md:flex-row md:items-center md:justify-between">
            <div><h2 class="text-slate-900" style="font-size:16px;font-weight:700">Assessment Mark Entry</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">Review submitted attachments and enter marks for students.</p></div>
            <div class="relative"><i data-lucide="search" class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"></i><input data-filter-input data-filter-target="[data-grade-row]" type="search" placeholder="Search student" class="h-9 w-full rounded-md border border-slate-200 bg-white pl-9 pr-3 md:w-64" style="font-size:12.5px" /></div>
        </div>
        <asp:Panel ID="emptyPanel" runat="server" Visible="false" CssClass="px-6 py-10 text-center text-slate-500" style="font-size:13px">No students found for this assessment.</asp:Panel>
        <div class="overflow-x-auto">
            <table class="min-w-full table-fixed">
                <colgroup>
                    <col style="width:14%" />
                    <col style="width:16%" />
                    <col style="width:30%" />
                    <col style="width:17%" />
                    <col style="width:10%" />
                    <col style="width:12%" />
                </colgroup>
                <thead class="bg-slate-50 text-left text-slate-500" style="font-size:11.5px;font-weight:700;letter-spacing:0.04em"><tr><th class="px-6 py-3">Student</th><th class="px-6 py-3">Email</th><th class="px-6 py-3">Submission</th><th class="px-6 py-3">Marks / 100</th><th class="px-6 py-3">Grade</th><th class="px-6 py-3">Status</th></tr></thead>
                <tbody class="divide-y divide-slate-100" style="font-size:13px">
                    <asp:Repeater ID="gradeRepeater" runat="server" OnItemDataBound="gradeRepeater_ItemDataBound" OnItemCommand="gradeRepeater_ItemCommand">
                        <ItemTemplate>
                            <tr data-grade-row data-filter-text='<%# Html(Eval("StudentName")) %> <%# Html(Eval("StudentNo")) %> <%# Html(Eval("SubmissionStatus")) %>' class="hover:bg-slate-50/60">
                                <td class="px-6 py-4"><asp:HiddenField ID="studentId" runat="server" Value='<%# Eval("StudentId") %>' /><asp:HiddenField ID="submissionId" runat="server" Value='<%# Eval("SubmissionId") %>' /><div class="font-semibold text-slate-900"><%# Html(Eval("StudentName")) %></div><div class="text-slate-500"><%# Html(Eval("StudentNo")) %></div></td>
                                <td class="break-words px-6 py-4 text-slate-600"><%# Html(Eval("StudentEmail")) %></td>
                                <td class="px-6 py-4 align-top">
                                    <button type="button" data-review-open='<%# ReviewModalId(Eval("SubmissionId")) %>' class='<%# Convert.ToBoolean(Eval("HasSubmission")) ? "inline-flex h-9 items-center justify-center gap-1.5 rounded-md border border-slate-200 bg-white px-3 text-slate-700 hover:bg-slate-50" : "hidden" %>' style="font-size:12.5px;font-weight:600">
                                        <i data-lucide="file-search" class="h-4 w-4"></i>Review submission
                                    </button>
                                    <span class='<%# Convert.ToBoolean(Eval("HasSubmission")) ? "hidden" : "text-slate-400" %>' style="font-size:12.5px">No attachment</span>
                                    <div class="mt-1 text-slate-400" style="font-size:11.5px"><%# Html(Eval("SubmissionStatus")) %><%# SubmittedAtDisplay(Eval("SubmittedAt")) %></div>
                                    <div data-review-modal='<%# ReviewModalId(Eval("SubmissionId")) %>' class="fixed inset-0 z-[70] hidden bg-slate-950/55 px-4 py-5">
                                        <div class="mx-auto flex h-full max-w-[1500px] flex-col overflow-hidden rounded-lg bg-white shadow-2xl">
                                            <div class="flex items-center justify-between border-b border-slate-200 px-5 py-3">
                                                <div>
                                                    <h3 class="text-slate-900" style="font-size:20px;font-weight:700">Submission Details</h3>
                                                    <p class="mt-1 text-slate-600" style="font-size:13px"><%# Html(Eval("StudentName")) %> submitted <%# SubmittedAtText(Eval("SubmittedAt")) %></p>
                                                </div>
                                                <div class="flex items-center gap-4">
                                                    <div class="text-slate-700" style="font-size:13px;font-weight:700">Grade: <%# MarksValue(Eval("Marks")) == "" ? "-" : MarksValue(Eval("Marks")) %> / 100</div>
                                                    <button type="button" data-review-close class="inline-flex h-9 w-9 items-center justify-center rounded-md text-slate-500 hover:bg-slate-100" aria-label="Close"><i data-lucide="x" class="h-5 w-5"></i></button>
                                                </div>
                                            </div>
                                            <div class="grid min-h-0 flex-1 grid-cols-1 lg:grid-cols-[minmax(0,1fr)_300px]">
                                                <div class="flex min-h-0 flex-col bg-slate-100">
                                                    <div class="flex h-11 items-center gap-1 border-b border-slate-300 bg-slate-700 px-4 text-white">
                                                        <button type="button" data-annotate-download class="inline-flex h-8 w-8 items-center justify-center rounded-md hover:bg-white/10" title="Download annotated PDF"><i data-lucide="download" class="h-4 w-4"></i></button>
                                                        <span class="mx-2 h-5 w-px bg-white/25"></span>
                                                        <button type="button" data-annotate-tool="pointer" class="inline-flex h-8 w-8 items-center justify-center rounded-md bg-white/15 hover:bg-white/20" title="Select"><i data-lucide="mouse-pointer-2" class="h-4 w-4"></i></button>
                                                        <button type="button" data-annotate-tool="highlight" class="inline-flex h-8 w-8 items-center justify-center rounded-md hover:bg-white/10" title="Highlight"><i data-lucide="highlighter" class="h-4 w-4"></i></button>
                                                        <button type="button" data-annotate-tool="bookmark" class="inline-flex h-8 w-8 items-center justify-center rounded-md hover:bg-white/10" title="Bookmark comment"><i data-lucide="map-pin" class="h-4 w-4"></i></button>
                                                        <button type="button" data-annotate-tool="text" class="inline-flex h-8 w-8 items-center justify-center rounded-md hover:bg-white/10" title="Text"><i data-lucide="type" class="h-4 w-4"></i></button>
                                                        <button type="button" data-annotate-tool="strike" class="inline-flex h-8 w-8 items-center justify-center rounded-md hover:bg-white/10" title="Strikeout"><i data-lucide="strikethrough" class="h-4 w-4"></i></button>
                                                        <button type="button" data-annotate-tool="draw" class="inline-flex h-8 w-8 items-center justify-center rounded-md hover:bg-white/10" title="Free draw"><i data-lucide="pencil" class="h-4 w-4"></i></button>
                                                        <button type="button" data-annotate-clear class="ml-auto inline-flex h-8 items-center justify-center gap-1.5 rounded-md px-3 hover:bg-white/10" style="font-size:12px;font-weight:700"><i data-lucide="eraser" class="h-4 w-4"></i>Clear</button>
                                                    </div>
                                                    <div data-preview-shell class="relative min-h-0 flex-1 overflow-auto p-5">
                                                        <iframe data-submission-preview src='<%# SubmissionPreviewUrl(Eval("SubmissionFileUrl")) %>' class="mx-auto block h-full min-h-[620px] w-full max-w-5xl rounded border border-slate-300 bg-white"></iframe>
                                                        <canvas data-annotation-canvas class="pointer-events-auto absolute left-5 top-5 h-[calc(100%-2.5rem)] w-[calc(100%-2.5rem)]"></canvas>
                                                    </div>
                                                </div>
                                                <aside class="border-l border-slate-200 bg-white p-5">
                                                    <h4 class="text-slate-900" style="font-size:14px;font-weight:800">Add Feedback</h4>
                                                    <asp:TextBox ID="reviewFeedbackInput" runat="server" Text='<%# Eval("Feedback") %>' TextMode="MultiLine" Rows="6" CssClass="mt-3 w-full rounded-md border border-slate-200 px-3 py-2 text-slate-700" />
                                                    <div class="mt-4">
                                                        <label class="mb-2 block text-slate-700" style="font-size:12.5px;font-weight:700">Attach annotated file</label>
                                                        <asp:FileUpload ID="annotatedFileInput" runat="server" CssClass="block w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-slate-600" />
                                                        <%# AnnotatedReviewLink(Eval("AnnotatedFileUrl")) %>
                                                    </div>
                                                    <asp:LinkButton runat="server" CommandName="SaveReview" CommandArgument='<%# Eval("SubmissionId") %>' CssClass="mt-5 inline-flex h-10 items-center justify-center gap-1.5 rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:700">
                                                        <i data-lucide="save" class="h-4 w-4"></i>Save feedback
                                                    </asp:LinkButton>
                                                </aside>
                                            </div>
                                        </div>
                                    </div>
                                </td>
                                <td class="px-6 py-4"><asp:TextBox ID="marksInput" runat="server" Text='<%# MarksValue(Eval("Marks")) %>' TextMode="Number" CssClass="h-9 w-24 rounded-md border border-slate-200 px-3" /></td>
                                <td class="px-6 py-4"><span class='rounded-full px-2 py-1 <%# GradeBadgeClass(Eval("LetterGrade")) %>' style="font-size:12px;font-weight:700"><%# Html(Eval("LetterGrade")) %></span></td>
                                <td class="px-6 py-4"><span class='<%# Convert.ToBoolean(Eval("HasMarks")) ? "text-emerald-700" : "text-amber-700" %> font-semibold'><%# Convert.ToBoolean(Eval("HasMarks")) ? "Ready" : "Draft" %></span></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </section>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/lecturer/lecturer-portal.js") %>"></script>
</asp:Content>
