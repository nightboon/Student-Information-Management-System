// Student attendance filters, course navigation, and filtered PDF export.
(function () {
    "use strict";

    var data = window.attendanceData || { courses: [] };
    var byOffering = {};
    data.courses.forEach(function (course) { byOffering[String(course.offeringId)] = course; });

    var selectedCardClass = "text-left rounded-lg border bg-white p-5 border-[#e0162b] ring-1 ring-[#e0162b]/10 cursor-pointer";
    var cardClass = "text-left rounded-lg border bg-white p-5 border-slate-200 cursor-pointer";

    function el(id) { return document.getElementById(id); }
    function text(id, value) { if (el(id)) { el(id).textContent = value; } }
    function show(id, visible) { if (el(id)) { el(id).style.display = visible ? "" : "none"; } }
    function value(id, fallback) { return el(id) ? el(id).value : fallback; }
    function cards() { return Array.prototype.slice.call(document.querySelectorAll("#course-grid [data-offering]")); }
    function esc(value) {
        var node = document.createElement("div");
        node.textContent = value == null ? "" : String(value);
        return node.innerHTML;
    }

    function filters() {
        return {
            semester: value("semester-filter", "all"),
            course: value("attendance-course-filter", "all"),
            status: value("attendance-status-filter", "all"),
            dateFrom: value("attendance-date-from", ""),
            dateTo: value("attendance-date-to", "")
        };
    }

    function matchesSemester(course, semester) {
        return semester === "all" || String(course.semesterId) === semester;
    }

    function matchesSession(session, current) {
        return (current.status === "all" || session.status === current.status) &&
            (!current.dateFrom || session.dateIso >= current.dateFrom) &&
            (!current.dateTo || session.dateIso <= current.dateTo);
    }

    function getCourses(current) {
        return data.courses.filter(function (course) {
            return matchesSemester(course, current.semester) &&
                (current.course === "all" || String(course.offeringId) === current.course);
        });
    }

    function getRows(courses, current) {
        var rows = [];
        courses.forEach(function (course) {
            course.sessions.forEach(function (session) {
                if (matchesSession(session, current)) { rows.push({ course: course, session: session }); }
            });
        });
        return rows.sort(function (left, right) {
            if (left.session.dateIso !== right.session.dateIso) {
                return left.session.dateIso < right.session.dateIso ? 1 : -1;
            }
            return left.session.time < right.session.time ? 1 : -1;
        });
    }

    function totals(rows) {
        var result = { present: 0, late: 0, absent: 0 };
        rows.forEach(function (row) {
            if (row.session.status === "PRESENT") { result.present += 1; }
            if (row.session.status === "LATE") { result.late += 1; }
            if (row.session.status === "ABSENT") { result.absent += 1; }
        });
        result.total = result.present + result.late + result.absent;
        return result;
    }

    function rate(present, total) {
        if (!total) { return "N/A"; }
        var percentage = Math.round((present / total) * 1000) / 10;
        return (percentage % 1 === 0 ? String(percentage) : percentage.toFixed(1)) + "%";
    }

    function statusBadge(status) {
        if (status === "PRESENT") return { cls: "bg-emerald-50 text-emerald-700 border-emerald-100", icon: "check-circle-2" };
        if (status === "LATE") return { cls: "bg-sky-50 text-sky-700 border-sky-100", icon: "clock" };
        if (status === "ABSENT") return { cls: "bg-[#e0162b]/10 text-[#a01020] border-[#e0162b]/20", icon: "x-circle" };
        return { cls: "bg-slate-50 text-slate-700 border-slate-200", icon: "circle" };
    }

    function rowHtml(row) {
        var session = row.session;
        var course = row.course;
        var badge = statusBadge(session.status);
        return '<tr class="hover:bg-slate-50/60">' +
            '<td class="px-6 py-3.5"><div class="text-slate-900" style="font-size:13px;font-weight:600">' + esc(session.date) + '</div><div class="text-slate-500" style="font-size:11px">' + esc(session.day) + '</div></td>' +
            '<td class="px-4 py-3.5 text-slate-700" style="font-size:12.5px">' + esc(session.time) + '</td>' +
            '<td class="px-4 py-3.5"><div class="text-slate-900" style="font-size:12px;font-weight:700">' + esc(course.code) + '</div><div class="truncate text-slate-500" style="font-size:10.5px">' + esc(course.name) + '</div></td>' +
            '<td class="px-4 py-3.5"><span class="inline-flex rounded border border-slate-200 bg-slate-50 px-1.5 py-0.5 text-slate-700" style="font-size:10.5px;font-weight:700">' + esc(session.type) + '</span></td>' +
            '<td class="px-4 py-3.5 text-slate-700" style="font-size:12.5px">' + esc(session.venue) + '</td>' +
            '<td class="px-6 py-3.5"><span class="inline-flex items-center gap-1 rounded border px-1.5 py-0.5 ' + badge.cls + '" style="font-size:10.5px;font-weight:700"><i data-lucide="' + badge.icon + '" class="h-3 w-3"></i> ' + esc(session.status) + '</span></td></tr>';
    }

    function rebuildCourses(semester) {
        var select = el("attendance-course-filter");
        if (!select) return;
        var previous = select.value || "all";
        select.innerHTML = '<option value="all">All courses</option>';
        data.courses.filter(function (course) { return matchesSemester(course, semester); })
            .forEach(function (course) {
                var option = document.createElement("option");
                option.value = String(course.offeringId);
                option.textContent = course.code + " - " + course.name;
                select.appendChild(option);
            });
        select.value = Array.prototype.some.call(select.options, function (option) {
            return option.value === previous;
        }) ? previous : "all";
    }

    function updateCards(current) {
        var count = 0;
        cards().forEach(function (card) {
            var course = byOffering[card.getAttribute("data-offering")];
            var visible = course && matchesSemester(course, current.semester);
            if (visible) count += 1;
            card.style.display = visible ? "" : "none";
            card.className = visible && current.course === String(course.offeringId) ? selectedCardClass : cardClass;
        });
        show("empty-courses", count === 0);
    }

    function updateExport(current) {
        var link = el("export-attendance");
        if (!link) return;
        var base = link.getAttribute("data-base-url") || link.href.split("?")[0];
        link.setAttribute("data-base-url", base);
        var params = [];
        if (current.semester !== "all") params.push("semesterId=" + encodeURIComponent(current.semester));
        if (current.course !== "all") params.push("offeringId=" + encodeURIComponent(current.course));
        if (current.status !== "all") params.push("status=" + encodeURIComponent(current.status));
        if (current.dateFrom) params.push("dateFrom=" + encodeURIComponent(current.dateFrom));
        if (current.dateTo) params.push("dateTo=" + encodeURIComponent(current.dateTo));
        link.href = base + (params.length ? "?" + params.join("&") : "");
    }

    function render() {
        var current = filters();
        var courses = getCourses(current);
        var rows = getRows(courses, current);
        var summary = totals(rows);
        var one = courses.length === 1 && current.course !== "all" ? courses[0] : null;
        var advanced = current.course !== "all" || current.status !== "all" || current.dateFrom || current.dateTo;

        updateCards(current);
        updateExport(current);
        text("hero-label", advanced ? "FILTERED ATTENDANCE" : "OVERALL ATTENDANCE");
        text("hero-rate", rate(summary.present + summary.late, summary.total));
        text("hero-present", summary.present);
        text("hero-late", summary.late);
        text("hero-absent", summary.absent);
        text("hero-subtext", rows.length ? (summary.present + summary.late) + " present / " + summary.total + " recorded across " + courses.length + " " + (courses.length === 1 ? "course" : "courses") : "No attendance records match the selected filters");

        if (el("detail-accent")) el("detail-accent").style.backgroundColor = one ? one.color : "#e0162b";
        text("detail-code", one ? one.code : "FILTERED RECORDS");
        text("detail-name", one ? one.name : "All courses");
        text("detail-lecturer", one ? one.lecturer : courses.length + " " + (courses.length === 1 ? "course" : "courses") + " in the selected scope");
        text("detail-present", summary.present);
        text("detail-late", summary.late);
        text("detail-absent", summary.absent);
        text("detail-filter-label", rows.length + " matching " + (rows.length === 1 ? "session" : "sessions"));
        text("attendance-filter-count", rows.length + " " + (rows.length === 1 ? "record" : "records"));
        text("sessions-footer", "Showing " + rows.length + " filtered " + (rows.length === 1 ? "session" : "sessions") + " from " + courses.length + " " + (courses.length === 1 ? "course" : "courses"));
        if (el("sessions-body")) el("sessions-body").innerHTML = rows.map(rowHtml).join("");
        show("detail-panel", courses.length > 0);
        show("empty-detail", courses.length === 0);
        show("empty-sessions", courses.length > 0 && rows.length === 0);

        if (el("attendance-date-from")) el("attendance-date-from").max = current.dateTo || "";
        if (el("attendance-date-to")) el("attendance-date-to").min = current.dateFrom || "";
        if (window.lucide && typeof window.lucide.createIcons === "function") window.lucide.createIcons();
    }

    function clearFilters() {
        ["attendance-date-from", "attendance-date-to"].forEach(function (id) { if (el(id)) el(id).value = ""; });
        ["attendance-course-filter", "attendance-status-filter"].forEach(function (id) { if (el(id)) el(id).value = "all"; });
        render();
    }

    function init() {
        var semester = el("semester-filter");
        rebuildCourses(semester ? semester.value : "all");
        cards().forEach(function (card) {
            card.addEventListener("click", function () {
                if (el("attendance-course-filter")) el("attendance-course-filter").value = card.getAttribute("data-offering");
                render();
                if (el("detail-panel")) el("detail-panel").scrollIntoView({ behavior: "smooth", block: "start" });
            });
        });
        if (semester) semester.addEventListener("change", function () { rebuildCourses(semester.value); render(); });
        ["attendance-course-filter", "attendance-status-filter", "attendance-date-from", "attendance-date-to"].forEach(function (id) {
            if (el(id)) el(id).addEventListener("change", render);
        });
        if (el("attendance-clear-filters")) el("attendance-clear-filters").addEventListener("click", clearFilters);
        render();
    }

    if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", init);
    else init();
})();
