// Attendance page: clickable course cards + semester filter.
// The server renders every course card (all kept semesters) and serializes the
// full per-course data to window.attendanceData. This script filters cards by
// semester, recomputes the hero summary from the visible courses, and repaints
// the detail panel (header, KPI strip, sessions table, footer) on card click.
(function () {
    "use strict";

    var data = window.attendanceData || { courses: [], defaultOfferingId: 0 };

    // Index courses by offeringId. Object keys are strings, and the card
    // data-offering attribute is a string, so lookups stay consistent.
    var byOffering = {};
    data.courses.forEach(function (c) { byOffering[String(c.offeringId)] = c; });

    var selectedOfferingId = String(data.defaultOfferingId || "");

    // Mirror of attendance.aspx.cs CourseCardClass (both selection states).
    var SELECTED_CARD =
        "text-left rounded-lg border bg-white p-5 border-slate-200 cursor-pointer";
    var UNSELECTED_CARD =
        "text-left rounded-lg border bg-white p-5 border-slate-200 cursor-pointer";

    // Mirror of attendance.aspx.cs StatusBadgeClass / StatusIcon.
    function badge(status) {
        switch (status) {
            case "PRESENT":
                return { cls: "inline-flex items-center gap-1 rounded border bg-emerald-50 text-emerald-700 border-emerald-100 px-1.5 py-0.5", icon: "check-circle-2" };
            case "LATE":
                return { cls: "inline-flex items-center gap-1 rounded border bg-sky-50 text-sky-700 border-sky-100 px-1.5 py-0.5", icon: "clock" };
            case "ABSENT":
                return { cls: "inline-flex items-center gap-1 rounded border bg-[#e0162b]/10 text-[#a01020] border-[#e0162b]/20 px-1.5 py-0.5", icon: "x-circle" };
            default:
                return { cls: "inline-flex items-center gap-1 rounded border bg-slate-50 text-slate-700 border-slate-200 px-1.5 py-0.5", icon: "circle" };
        }
    }

    function esc(s) {
        var d = document.createElement("div");
        d.textContent = s == null ? "" : String(s);
        return d.innerHTML;
    }

    function cards() {
        return Array.prototype.slice.call(
            document.querySelectorAll("#course-grid [data-offering]"));
    }

    function setText(id, txt) {
        var el = document.getElementById(id);
        if (el) { el.textContent = txt; }
    }

    function show(id, on) {
        var el = document.getElementById(id);
        if (el) { el.style.display = on ? "" : "none"; }
    }

    // Mirror of attendance.aspx.cs FormatRate: one optional decimal, "N/A" when empty.
    function formatRate(present, total) {
        if (total === 0) { return "N/A"; }
        var r = Math.round((present / total) * 1000) / 10;
        var str = (r % 1 === 0) ? String(r) : r.toFixed(1);
        return str + "%";
    }

    function rowHtml(s) {
        var b = badge(s.status);
        return '<tr class="hover:bg-slate-50/60">' +
            '<td class="px-6 py-3.5">' +
            '<div class="text-slate-900" style="font-size:13px;font-weight:600">' + esc(s.date) + '</div>' +
            '<div class="text-slate-500" style="font-size:11px">' + esc(s.day) + '</div>' +
            '</td>' +
            '<td class="px-4 py-3.5 text-slate-700" style="font-size:12.5px">' + esc(s.time) + '</td>' +
            '<td class="px-4 py-3.5">' +
            '<span class="inline-flex items-center rounded border border-slate-200 bg-slate-50 px-1.5 py-0.5 text-slate-700" style="font-size:10.5px;font-weight:700;letter-spacing:0.04em">' + esc(s.type) + '</span>' +
            '</td>' +
            '<td class="px-4 py-3.5 text-slate-700" style="font-size:12.5px">' + esc(s.venue) + '</td>' +
            '<td class="px-6 py-3.5">' +
            '<span class="' + b.cls + '" style="font-size:10.5px;font-weight:700;letter-spacing:0.04em">' +
            '<i data-lucide="' + b.icon + '" class="h-3 w-3"></i> ' + esc(s.status) +
            '</span>' +
            '</td>' +
            '</tr>';
    }

    function selectCourse(offeringId) {
        offeringId = String(offeringId);
        selectedOfferingId = offeringId;

        cards().forEach(function (card) {
            var on = card.getAttribute("data-offering") === offeringId;
            card.className = on ? SELECTED_CARD : UNSELECTED_CARD;
        });

        var c = byOffering[offeringId];
        if (!c) { return; }

        var accent = document.getElementById("detail-accent");
        if (accent) { accent.style.backgroundColor = c.color; }
        setText("detail-code", c.code);
        setText("detail-name", c.name);
        setText("detail-lecturer", c.lecturer);
        setText("detail-present", String(c.present));
        setText("detail-late", String(c.late));
        setText("detail-absent", String(c.absent));

        var body = document.getElementById("sessions-body");
        if (body) { body.innerHTML = c.sessions.map(rowHtml).join(""); }
        show("empty-sessions", c.sessions.length === 0);
        setText("sessions-footer",
            "Showing " + c.sessions.length + " of " + c.sessions.length + " recorded sessions");

        if (window.lucide && typeof window.lucide.createIcons === "function") {
            window.lucide.createIcons();
        }
    }

    function applyFilter(value) {
        var visible = [];
        cards().forEach(function (card) {
            var sid = card.getAttribute("data-semester-id");
            var on = value === "all" || sid === value;
            card.style.display = on ? "" : "none";
            if (on) { visible.push(card); }
        });

        // Hero recompute from the visible courses.
        var present = 0, late = 0, absent = 0, total = 0;
        visible.forEach(function (card) {
            var c = byOffering[card.getAttribute("data-offering")];
            if (!c) { return; }
            present += c.present; late += c.late; absent += c.absent; total += c.total;
        });
        setText("hero-rate", formatRate(present, total));
        setText("hero-present", String(present));
        setText("hero-late", String(late));
        setText("hero-absent", String(absent));
        setText("hero-subtext", total === 0
            ? "No attendance records yet"
            : present + " present / " + total + " recorded across " +
            visible.length + " " + (visible.length === 1 ? "course" : "courses"));

        show("empty-courses", visible.length === 0);

        // Keep the current selection if it is still visible, else pick the first.
        var keep = visible.filter(function (card) {
            return card.getAttribute("data-offering") === selectedOfferingId;
        })[0];
        var target = keep || visible[0];

        if (target) {
            show("detail-panel", true);
            show("empty-detail", false);
            selectCourse(target.getAttribute("data-offering"));
        } else {
            show("detail-panel", false);
            show("empty-detail", true);
        }
    }

    function init() {
        cards().forEach(function (card) {
            card.addEventListener("click", function () {
                selectCourse(card.getAttribute("data-offering"));
            });
        });

        var sel = document.getElementById("semester-filter");
        if (sel) {
            sel.addEventListener("change", function () { applyFilter(sel.value); });
            applyFilter(sel.value);
        } else {
            applyFilter("all");
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
