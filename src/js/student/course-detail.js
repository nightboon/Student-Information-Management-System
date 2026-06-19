// Course detail page: tab switching, module accordion, and pin toggle.
// All content is rendered server-side; this script only drives interactivity.
// Hooks in the markup:
//   [data-action="switch-tab"][data-tab][data-active]  – tab buttons (with a
//        child .tab-indicator); panes are [data-pane] (inactive ones are .hidden)
//   [data-action="toggle-module"][data-week]           – accordion header; its
//        sibling .module-items.hidden expands, its .module-chevron rotates
//   [data-action="toggle-pin"][data-code]              – pin the course
(function () {
    "use strict";

    // Shared with the My Courses page so a pin set here shows there too.
    var PIN_KEY = "courses.pinned";
    var INACTIVE = "#64748b"; // slate-500 for inactive tabs

    function activeColor() {
        var root = document.getElementById("course-detail-root") || document.documentElement;
        var color = window.getComputedStyle(root).getPropertyValue("--course-accent").trim();
        return color || INACTIVE;
    }

    function loadPins() {
        try { return JSON.parse(localStorage.getItem(PIN_KEY)) || []; }
        catch (e) { return []; }
    }

    function savePins(pins) {
        // Storage can throw in private mode or when the quota is exceeded; the pin
        // simply isn't persisted across reloads in that case.
        try { localStorage.setItem(PIN_KEY, JSON.stringify(pins)); }
        catch (e) { /* storage unavailable */ }
    }

    // ---- Tabs -------------------------------------------------------------
    // Colour is set inline rather than via Tailwind classes: the markup mixes
    // base and data-[active] variant classes, so inline style is the only
    // deterministic way to override colour regardless of the Play CDN output.
    function switchTab(tab) {
        Array.prototype.forEach.call(
            document.querySelectorAll("[data-pane]"),
            function (pane) {
                pane.classList.toggle("hidden", pane.getAttribute("data-pane") !== tab);
            }
        );
        Array.prototype.forEach.call(
            document.querySelectorAll('[data-action="switch-tab"]'),
            function (btn) {
                var active = btn.getAttribute("data-tab") === tab;
                btn.setAttribute("data-active", active ? "true" : "false");
                btn.style.color = active ? activeColor() : INACTIVE;
                var indicator = btn.querySelector(".tab-indicator");
                if (indicator) { indicator.classList.toggle("hidden", !active); }
            }
        );
    }

    // ---- Module accordion -------------------------------------------------
    function toggleModule(btn) {
        var li = btn.closest("li");
        if (!li) { return; }
        var items = li.querySelector(".module-items");
        var chevron = btn.querySelector(".module-chevron");
        if (!items) { return; }
        var open = items.classList.toggle("hidden") === false;
        if (chevron) { chevron.style.transform = open ? "rotate(90deg)" : ""; }
    }

    // ---- Pin --------------------------------------------------------------
    function paintPin(btn) {
        var code = btn.getAttribute("data-code");
        btn.style.color = loadPins().indexOf(code) !== -1 ? activeColor() : "";
    }

    function togglePin(btn) {
        var code = btn.getAttribute("data-code");
        var pins = loadPins();
        var i = pins.indexOf(code);
        if (i === -1) { pins.push(code); } else { pins.splice(i, 1); }
        savePins(pins);
        paintPin(btn);
    }

    function init() {
        // Tabs
        Array.prototype.forEach.call(
            document.querySelectorAll('[data-action="switch-tab"]'),
            function (btn) {
                btn.addEventListener("click", function () {
                    switchTab(btn.getAttribute("data-tab"));
                });
            }
        );
        // Normalise to the default active tab (the one marked data-active="true",
        // falling back to "modules") so tab styling is consistent on load.
        var current = document.querySelector('[data-action="switch-tab"][data-active="true"]');
        switchTab(current ? current.getAttribute("data-tab") : "modules");

        // Module accordion
        Array.prototype.forEach.call(
            document.querySelectorAll('[data-action="toggle-module"]'),
            function (btn) {
                btn.addEventListener("click", function () { toggleModule(btn); });
            }
        );

        // Pin
        Array.prototype.forEach.call(
            document.querySelectorAll('[data-action="toggle-pin"]'),
            function (btn) {
                paintPin(btn);
                btn.addEventListener("click", function () { togglePin(btn); });
            }
        );
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
