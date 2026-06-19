// Lecturer My Courses page: semester filter (all / a specific semester), search,
// pin, empty-state. Cards are rendered server-side; each <article> carries:
//   data-semester="2026-S2"    – the offering's semester (matched against the tab)
//   data-search="..."          – lowercased code/name for search
//   data-course-code="CODE"     – stable key for pinning
(function () {
    "use strict";

    var PIN_KEY = "lecturer.courses.pinned";
    var state = { semester: "all", query: "" };

    function cards() {
        return Array.prototype.slice.call(document.querySelectorAll("#course-grid [data-course-code]"));
    }

    function loadPins() {
        try { return JSON.parse(localStorage.getItem(PIN_KEY)) || []; }
        catch (e) { return []; }
    }

    function savePins(pins) {
        // Storage can throw in private mode or when the quota is exceeded; in that
        // case the pin is simply not persisted across reloads.
        try { localStorage.setItem(PIN_KEY, JSON.stringify(pins)); }
        catch (e) { /* storage unavailable */ }
    }

    function isPinned(code) {
        return loadPins().indexOf(code) !== -1;
    }

    function applyFilters() {
        var q = state.query.trim().toLowerCase();
        var visible = 0;

        cards().forEach(function (card) {
            var matchesSemester = state.semester === "all" || card.getAttribute("data-semester") === state.semester;
            var matchesQuery = q === "" || (card.getAttribute("data-search") || "").indexOf(q) !== -1;
            var show = matchesSemester && matchesQuery;
            card.style.display = show ? "" : "none";
            if (show) { visible++; }
        });

        var noResults = document.getElementById("no-results");
        if (noResults) { noResults.classList.toggle("hidden", visible !== 0); }
    }

    function updatePinnedCount() {
        var el = document.getElementById("pinned-count");
        if (el) { el.textContent = String(loadPins().length); }
    }

    function paintPin(card) {
        var code = card.getAttribute("data-course-code");
        var btn = card.querySelector('[data-action="toggle-pin"]');
        if (!btn) { return; }
        // Inline style rather than a Tailwind arbitrary class: the Play CDN may not
        // generate styles for a class only ever added dynamically by JS.
        if (isPinned(code)) {
            btn.style.color = "#e0162b";
        } else {
            btn.style.removeProperty("color");
        }
    }

    function togglePin(code) {
        var pins = loadPins();
        var i = pins.indexOf(code);
        if (i === -1) { pins.push(code); } else { pins.splice(i, 1); }
        savePins(pins);
    }

    function activateSemesterButton(clicked) {
        var buttons = document.querySelectorAll('[data-action="filter-semester"]');
        Array.prototype.forEach.call(buttons, function (b) {
            var active = b === clicked;
            b.classList.toggle("bg-slate-900", active);
            b.classList.toggle("text-white", active);
            b.classList.toggle("border", !active);
            b.classList.toggle("border-slate-200", !active);
            b.classList.toggle("bg-white", !active);
            b.classList.toggle("text-slate-600", !active);
        });
    }

    function init() {
        // Semester buttons
        Array.prototype.forEach.call(
            document.querySelectorAll('[data-action="filter-semester"]'),
            function (btn) {
                btn.addEventListener("click", function () {
                    state.semester = btn.getAttribute("data-semester") || "all";
                    activateSemesterButton(btn);
                    applyFilters();
                });
            }
        );

        // Search
        var search = document.getElementById("course-search");
        if (search) {
            search.addEventListener("input", function () {
                state.query = search.value || "";
                applyFilters();
            });
        }

        // Pin buttons
        cards().forEach(function (card) {
            paintPin(card);
            var btn = card.querySelector('[data-action="toggle-pin"]');
            if (btn) {
                btn.addEventListener("click", function () {
                    togglePin(card.getAttribute("data-course-code"));
                    paintPin(card);
                    updatePinnedCount();
                });
            }
        });

        // Establish the default active filter button visually (all semesters).
        var allBtn = document.querySelector('[data-action="filter-semester"][data-semester="all"]');
        if (allBtn) { activateSemesterButton(allBtn); }

        updatePinnedCount();
        applyFilters();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
