<%@ Page Language="C#" MasterPageFile="~/admin/AdminLayout.master" AutoEventWireup="true" CodeBehind="course_passfail.aspx.cs" Inherits="src.admin.course_passfail" Title="Course Pass/Fail - INTI Admin Portal" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <a href="<%= ResolveUrl("~/admin/academic_performance.aspx") %>" class="inline-flex items-center gap-1.5 text-slate-600 hover:text-[#a01020] transition-colors" style="font-size:13px;font-weight:600">
        <i data-lucide="arrow-left" class="h-4 w-4"></i> Back to Academic Performance
    </a>

    <div id="course-pf-root" class="mt-4"></div>

</asp:Content>
<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/admin/shared/icons.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/toast.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/table.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/shared/ui.js") %>"></script>
    <script src="<%= ResolveUrl("~/js/admin/course-passfail/passfail.js") %>"></script>
    <script>
      (function () {
        var courses = <%= PassFailJson %>;
        var status = window.PASSFAIL.status;
        var gradeTone = window.PASSFAIL.gradeTone;
        window.PASSFAIL = {
          list: courses,
          get: function (code) {
            for (var i = 0; i < courses.length; i++) if (courses[i].code === code) return courses[i];
            return { code: code, title: "Unknown course", prog: "-", sem: 0, lecturer: "-",
                     enrolled: 0, passed: 0, failed: 0, passRate: 0, avgMarks: 0, students: [] };
          },
          status: status,
          gradeTone: gradeTone
        };
      })();
    </script>
    <script>
      (function () {
        function qp(k) { var m = location.search.match(new RegExp("[?&]" + k + "=([^&]*)")); return m ? decodeURIComponent(m[1]) : ""; }
        function esc(s) { return String(s == null ? "" : s).replace(/[&<>"']/g, function (c) { return { "&":"&amp;","<":"&lt;",">":"&gt;",'"':"&quot;","'":"&#39;" }[c]; }); }
        function badgeClass(tone) {
          var map = {
            success: "bg-emerald-50 text-emerald-700 border-emerald-100",
            pending: "bg-amber-50 text-amber-700 border-amber-100",
            danger:  "bg-[#e0162b]/10 text-[#a01020] border-[#e0162b]/20",
            neutral: "bg-slate-100 text-slate-600 border-slate-200",
            info:    "bg-sky-50 text-sky-700 border-sky-100"
          };
          return "inline-flex items-center gap-1 rounded-full border px-2 py-0.5 " + (map[tone] || map.neutral);
        }
        var statusFn = window.PASSFAIL.status;
        var gradeTone = window.PASSFAIL.gradeTone;

        function donut(passed, failed) {
          var total = passed + failed;
          if (total === 0) return '<div class="text-slate-400" style="font-size:13px">No results recorded.</div>';
          var passPct = (passed / total) * 100, failPct = 100 - passPct;
          var size = 220, cx = 110, cy = 110, r = 90, inner = 56;
          function arc(s, e) {
            var sA = (s / 100) * Math.PI * 2 - Math.PI / 2;
            var eA = (e / 100) * Math.PI * 2 - Math.PI / 2;
            var large = (e - s) > 50 ? 1 : 0;
            var x1 = cx + r * Math.cos(sA), y1 = cy + r * Math.sin(sA);
            var x2 = cx + r * Math.cos(eA), y2 = cy + r * Math.sin(eA);
            var xi1 = cx + inner * Math.cos(eA), yi1 = cy + inner * Math.sin(eA);
            var xi2 = cx + inner * Math.cos(sA), yi2 = cy + inner * Math.sin(sA);
            return "M " + x1 + " " + y1 + " A " + r + " " + r + " 0 " + large + " 1 " + x2 + " " + y2 +
                   " L " + xi1 + " " + yi1 + " A " + inner + " " + inner + " 0 " + large + " 0 " + xi2 + " " + yi2 + " Z";
          }
          var onlyOne = passed === 0 || failed === 0;
          var svg = onlyOne
            ? '<circle cx="' + cx + '" cy="' + cy + '" r="' + r + '" fill="' + (passed > 0 ? "#10b981" : "#e0162b") + '"/><circle cx="' + cx + '" cy="' + cy + '" r="' + inner + '" fill="#fff"/>'
            : '<path d="' + arc(0, passPct) + '" fill="#10b981"/><path d="' + arc(passPct, 100) + '" fill="#e0162b"/>';
          return '<div class="flex flex-col items-center gap-6 lg:flex-row lg:items-center lg:justify-center">' +
                   '<div class="relative"><svg viewBox="0 0 ' + size + ' ' + size + '" width="' + size + '" height="' + size + '">' + svg +
                     '<text x="' + cx + '" y="' + (cy - 4) + '" text-anchor="middle" font-size="22" font-weight="700" fill="#0f172a">' + passPct.toFixed(1) + '%</text>' +
                     '<text x="' + cx + '" y="' + (cy + 14) + '" text-anchor="middle" font-size="10" fill="#64748b">Pass Rate</text>' +
                   '</svg></div>' +
                   '<div class="space-y-3" style="min-width:200px">' +
                     '<div class="flex items-center justify-between gap-4"><div class="inline-flex items-center gap-2 text-slate-700" style="font-size:13px;font-weight:600"><span class="h-2.5 w-2.5 rounded-sm bg-emerald-500"></span> Passed</div><div class="text-right tabular-nums"><div class="text-slate-900" style="font-size:14px;font-weight:700">' + passed + '</div><div class="text-slate-400" style="font-size:11.5px">' + passPct.toFixed(1) + '%</div></div></div>' +
                     '<div class="flex items-center justify-between gap-4"><div class="inline-flex items-center gap-2 text-slate-700" style="font-size:13px;font-weight:600"><span class="h-2.5 w-2.5 rounded-sm bg-[#e0162b]"></span> Failed</div><div class="text-right tabular-nums"><div class="text-slate-900" style="font-size:14px;font-weight:700">' + failed + '</div><div class="text-slate-400" style="font-size:11.5px">' + failPct.toFixed(1) + '%</div></div></div>' +
                     '<div class="border-t border-slate-100 pt-3 flex items-center justify-between text-slate-600" style="font-size:12.5px"><span>Total students</span><span class="tabular-nums font-semibold text-slate-900">' + total + '</span></div>' +
                   '</div>' +
                 '</div>';
        }

        function gradeDist(students) {
          var total = students.length || 1;
          var groups = [
            { label: "A / A+", color: "bg-emerald-500", match: function (g) { return g.charAt(0) === "A"; } },
            { label: "B / B+", color: "bg-sky-500",    match: function (g) { return g.charAt(0) === "B"; } },
            { label: "C / C+", color: "bg-amber-500",  match: function (g) { return g.charAt(0) === "C"; } },
            { label: "D",      color: "bg-orange-500", match: function (g) { return g === "D"; } },
            { label: "F",      color: "bg-[#e0162b]",  match: function (g) { return g === "F"; } }
          ];
          var html = '<div class="space-y-3">';
          groups.forEach(function (b) {
            var count = students.filter(function (s) { return b.match(s.grade); }).length;
            var pct = Math.round((count / total) * 1000) / 10;
            html += '<div><div class="flex items-center justify-between" style="font-size:12.5px"><span class="text-slate-700 font-medium">' + esc(b.label) + '</span><span class="text-slate-500 tabular-nums">' + count + ' &middot; ' + pct + '%</span></div>' +
                    '<div class="mt-1 h-2 w-full overflow-hidden rounded-full bg-slate-100"><div class="h-full rounded-full ' + b.color + '" style="width:' + pct + '%"></div></div></div>';
          });
          return html + '</div>';
        }

        function build(c) {
          var overall = statusFn(c.passRate);
          function stat(label, value, tone, help) {
            var col = tone === "danger" ? "text-[#a01020]" : tone === "success" ? "text-emerald-600" : tone === "pending" ? "text-amber-600" : "text-slate-900";
            return '<div class="rounded-2xl border border-slate-200 bg-white p-4 hover:border-slate-300 hover:shadow-sm transition-all">' +
                     '<div class="text-slate-500" style="font-size:11.5px;font-weight:500">' + esc(label) + '</div>' +
                     '<div class="mt-1 ' + col + '" style="font-size:22px;font-weight:700;letter-spacing:-0.01em">' + esc(value) + '</div>' +
                     (help ? '<div class="text-slate-400" style="font-size:11.5px">' + esc(help) + '</div>' : '') +
                   '</div>';
          }

          var html = '';
          html += '<section class="rounded-lg border border-slate-200 bg-white">' +
                    '<div class="flex flex-col gap-5 px-6 py-6 lg:flex-row lg:items-start lg:justify-between">' +
                      '<div>' +
                        '<div class="flex flex-wrap items-center gap-2">' +
                          '<h1 class="text-slate-900" style="font-size:24px;font-weight:700;letter-spacing:-0.01em">' + esc(c.code) + '</h1>' +
                          '<span class="' + badgeClass("neutral") + '" style="font-size:11.5px;font-weight:600">' + esc(c.prog) + '</span>' +
                          '<span class="' + badgeClass(overall.tone) + '" style="font-size:11.5px;font-weight:600">' + esc(overall.label) + ' pass rate</span>' +
                        '</div>' +
                        '<p class="mt-1 text-slate-700" style="font-size:15px;font-weight:500">' + esc(c.title) + '</p>' +
                        '<div class="mt-3 flex flex-wrap items-center gap-x-5 gap-y-2 text-slate-600" style="font-size:12.5px">' +
                          '<span class="inline-flex items-center gap-1.5"><i data-lucide="users" class="h-4 w-4 text-slate-400"></i> ' + c.enrolled + ' students</span>' +
                          '<span class="inline-flex items-center gap-1.5"><i data-lucide="book-open" class="h-4 w-4 text-slate-400"></i> Sem ' + c.sem + '</span>' +
                          '<span class="inline-flex items-center gap-1.5">Lecturer: ' + esc(c.lecturer) + '</span>' +
                        '</div>' +
                      '</div>' +
                      '<div class="flex items-center gap-2">' +
                        '<button type="button" data-toast="Course CSV export is not connected yet" data-toast-type="info" class="inline-flex items-center gap-1.5 rounded-md border border-slate-200 bg-white px-3 h-10 text-slate-700 hover:bg-slate-50 transition-colors" style="font-size:13px;font-weight:600"><i data-lucide="download" class="h-4 w-4"></i> Export CSV</button>' +
                      '</div>' +
                    '</div>' +
                  '</section>';

          html += '<section class="mt-4" style="display:grid;grid-template-columns:repeat(auto-fit,minmax(200px,1fr));gap:16px">' +
                    stat("Pass Rate",      c.passRate.toFixed(1) + "%", overall.tone, "passed / total") +
                    stat("Passed",         String(c.passed),            "success", "students") +
                    stat("Failed",         String(c.failed),            c.failed > 0 ? "danger" : "neutral", "students") +
                    stat("Average Marks",  c.avgMarks.toFixed(1),       "neutral", "out of 100") +
                  '</section>';

          // Pie + Grade dist (2:1)
          html += '<section class="mt-6" style="display:flex;flex-wrap:wrap;gap:16px">' +
                    '<div class="rounded-lg border border-slate-200 bg-white" style="flex:3 1 460px;min-width:0">' +
                      '<div class="border-b border-slate-100 px-6 py-4"><h2 class="text-slate-900" style="font-size:15px;font-weight:700">Pass vs Fail</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">Distribution of student outcomes for this course.</p></div>' +
                      '<div class="px-6 py-6">' + donut(c.passed, c.failed) + '</div>' +
                    '</div>' +
                    '<div class="rounded-lg border border-slate-200 bg-white" style="flex:2 1 320px;min-width:0">' +
                      '<div class="border-b border-slate-100 px-6 py-4"><h2 class="text-slate-900" style="font-size:15px;font-weight:700">Grade Distribution</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">Letter grade breakdown.</p></div>' +
                      '<div class="px-6 py-5">' + gradeDist(c.students) + '</div>' +
                    '</div>' +
                  '</section>';

          // Student results table
          html += '<section class="mt-6 rounded-lg border border-slate-200 bg-white">' +
                    '<div class="flex flex-col gap-3 border-b border-slate-100 px-6 py-4 lg:flex-row lg:items-center lg:justify-between">' +
                      '<div><h2 class="text-slate-900" style="font-size:15px;font-weight:700">Student Results</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px"><span id="pf-count">' + c.students.length + '</span> of ' + c.students.length + ' students</p></div>' +
                      '<div class="flex flex-wrap items-center gap-2">' +
                        '<div class="relative"><svg viewBox="0 0 24 24" class="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="7"/><path d="m21 21-4.3-4.3"/></svg><input id="pf-search" placeholder="Search name or ID&hellip;" class="h-9 w-56 rounded-md border border-slate-200 bg-white pl-9 pr-3 outline-none focus:border-[#e0162b]/40 focus:ring-4 focus:ring-[#e0162b]/10" style="font-size:12.5px" /></div>' +
                        '<div id="pf-chips" class="inline-flex items-center rounded-md border border-slate-200 bg-white p-0.5">' +
                          '<button type="button" data-chip="All" data-active="true" class="rounded px-2.5 py-1 text-slate-600 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020]" style="font-size:11.5px;font-weight:600">All</button>' +
                          '<button type="button" data-chip="Pass" class="rounded px-2.5 py-1 text-slate-600 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020]" style="font-size:11.5px;font-weight:600">Pass</button>' +
                          '<button type="button" data-chip="Fail" class="rounded px-2.5 py-1 text-slate-600 hover:text-slate-900 data-[active=true]:bg-[#e0162b]/10 data-[active=true]:text-[#a01020]" style="font-size:11.5px;font-weight:600">Fail</button>' +
                        '</div>' +
                        '<button id="pf-sort" type="button" data-dir="desc" class="inline-flex items-center gap-1.5 rounded-md border border-slate-200 bg-white px-3 h-9 text-slate-700 hover:bg-slate-50" style="font-size:12.5px;font-weight:600"><i data-lucide="arrow-up-down" class="h-3.5 w-3.5"></i> <span class="pf-sort-label">Highest marks</span></button>' +
                      '</div>' +
                    '</div>' +
                    '<div class="overflow-x-auto"><table class="min-w-full">' +
                      '<thead class="border-b border-slate-100 bg-slate-50/60 text-slate-500"><tr>' +
                        '<th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">ID</th>' +
                        '<th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Name</th>' +
                        '<th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Prog</th>' +
                        '<th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Marks</th>' +
                        '<th class="px-6 py-3 text-center uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Grade</th>' +
                        '<th class="px-6 py-3 text-left uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Status</th>' +
                        '<th class="px-6 py-3 text-right uppercase" style="font-size:11px;font-weight:600;letter-spacing:0.05em">Action</th>' +
                      '</tr></thead><tbody id="pf-tbody"></tbody>' +
                    '</table></div>' +
                  '</section>';

          return html;
        }

        function renderRows(students) {
          var tbody = document.getElementById("pf-tbody");
          if (!tbody) return;
          if (students.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="py-10 text-center text-slate-400" style="font-size:13px">No students match your filters.</td></tr>';
            return;
          }
          var html = "";
          students.forEach(function (s) {
            var gtone = gradeTone(s.grade);
            var statTone = s.status === "Pass" ? "success" : "danger";
            html += '<tr class="border-b border-slate-100 hover:bg-slate-50/60">' +
                      '<td class="px-6 py-3 text-slate-500" style="font-size:12.5px">' + esc(s.id) + '</td>' +
                      '<td class="px-6 py-3" style="font-size:12.5px"><span class="text-slate-900 font-medium">' + esc(s.name) + '</span></td>' +
                      '<td class="px-6 py-3" style="font-size:12.5px"><span class="' + badgeClass("neutral") + '" style="font-size:11.5px;font-weight:600">' + esc(s.prog) + '</span></td>' +
                      '<td class="px-6 py-3 text-right tabular-nums text-slate-700" style="font-size:12.5px">' + s.marks + '</td>' +
                      '<td class="px-6 py-3 text-center" style="font-size:12.5px"><span class="' + badgeClass(gtone) + '" style="font-size:11.5px;font-weight:600">' + esc(s.grade) + '</span></td>' +
                      '<td class="px-6 py-3" style="font-size:12.5px"><span class="' + badgeClass(statTone) + '" style="font-size:11.5px;font-weight:600"><i data-lucide="' + (s.status === "Pass" ? "check-circle-2" : "x-circle") + '" class="h-3 w-3"></i> ' + esc(s.status) + '</span></td>' +
                      '<td class="px-6 py-3 text-right" style="font-size:12.5px"><a href="student_detail.aspx?id=' + encodeURIComponent(s.id) + '" class="inline-flex h-7 items-center rounded-md border border-slate-200 bg-white px-2 text-slate-600 hover:bg-slate-50" style="font-size:11.5px;font-weight:600">View student</a></td>' +
                    '</tr>';
          });
          tbody.innerHTML = html;
          if (window.renderIcons) window.renderIcons();
        }

        function wireControls(course) {
          var state = { q: "", filter: "All", dir: "desc" };
          function refresh() {
            var list = course.students.filter(function (s) {
              if (state.q && !(s.name.toLowerCase().indexOf(state.q) > -1 || s.id.toLowerCase().indexOf(state.q) > -1)) return false;
              if (state.filter !== "All" && s.status !== state.filter) return false;
              return true;
            });
            list.sort(function (a, b) { return state.dir === "asc" ? a.marks - b.marks : b.marks - a.marks; });
            document.getElementById("pf-count").textContent = String(list.length);
            renderRows(list);
          }
          var search = document.getElementById("pf-search");
          if (search) search.addEventListener("input", function () { state.q = search.value.toLowerCase(); refresh(); });
          var chips = document.getElementById("pf-chips");
          if (chips) chips.addEventListener("click", function (e) {
            var b = e.target.closest("[data-chip]"); if (!b) return;
            chips.querySelectorAll("[data-chip]").forEach(function (x) { x.setAttribute("data-active", x === b ? "true" : "false"); });
            state.filter = b.getAttribute("data-chip"); refresh();
          });
          var sortBtn = document.getElementById("pf-sort");
          if (sortBtn) sortBtn.addEventListener("click", function () {
            state.dir = state.dir === "asc" ? "desc" : "asc";
            sortBtn.querySelector(".pf-sort-label").textContent = state.dir === "asc" ? "Lowest marks" : "Highest marks";
            refresh();
          });
          refresh();
        }

        var c = window.PASSFAIL.get(qp("code") || "CSC2024");
        document.getElementById("course-pf-root").innerHTML = build(c);
        if (window.renderIcons) window.renderIcons();
        wireControls(c);
      })();
    </script>
</asp:Content>
