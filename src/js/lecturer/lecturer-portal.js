(function () {
    var announcementTabFilter = "all";

    function announcementMatchesTab(item) {
        return announcementTabFilter === "all" ||
            (announcementTabFilter === "pinned" && item.getAttribute("data-pinned") === "true") ||
            (announcementTabFilter === "files" && item.getAttribute("data-file") === "true");
    }

    function announcementMatchesSearch(item, query) {
        var text = (item.getAttribute("data-filter-text") || item.textContent || "").toLowerCase();
        return text.indexOf(query) >= 0;
    }

    function applyAnnouncementFilters() {
        var input = document.querySelector("[data-announcement-search]");
        var query = input ? input.value.trim().toLowerCase() : "";

        document.querySelectorAll("[data-announcement-item]").forEach(function (item) {
            item.style.display = announcementMatchesTab(item) && announcementMatchesSearch(item, query) ? "" : "none";
        });
    }

    function initFilters() {
        document.querySelectorAll("[data-filter-input]").forEach(function (input) {
            var selector = input.getAttribute("data-filter-target");
            input.addEventListener("input", function () {
                if (input.hasAttribute("data-announcement-search")) {
                    applyAnnouncementFilters();
                    return;
                }

                var query = input.value.trim().toLowerCase();
                document.querySelectorAll(selector).forEach(function (item) {
                    var text = (item.getAttribute("data-filter-text") || item.textContent || "").toLowerCase();
                    item.style.display = text.indexOf(query) >= 0 ? "" : "none";
                });
            });
        });
    }

    function showToast(message) {
        var toast = document.getElementById("lecturer-toast");
        if (!toast) {
            toast = document.createElement("div");
            toast.id = "lecturer-toast";
            toast.className = "fixed bottom-5 right-5 z-[80] rounded-lg border border-slate-200 bg-white px-4 py-3 text-slate-900 shadow-lg";
            toast.style.fontSize = "13px";
            toast.style.fontWeight = "700";
            document.body.appendChild(toast);
        }
        toast.textContent = message;
        toast.style.opacity = "1";
        toast.style.transform = "translateY(0)";
        window.clearTimeout(showToast.timer);
        showToast.timer = window.setTimeout(function () {
            toast.style.opacity = "0";
            toast.style.transform = "translateY(8px)";
        }, 2200);
    }

    function initToasts() {
        document.addEventListener("click", function (event) {
            var trigger = event.target.closest("[data-toast]");
            if (!trigger) return;
            event.preventDefault();
            showToast(trigger.getAttribute("data-toast"));
        });
    }

    function initComposeToggle() {
        document.addEventListener("click", function (event) {
            var trigger = event.target.closest("[data-action='toggle-compose']");
            if (!trigger) return;

            var panel = document.querySelector("[data-compose-panel='true']");
            if (!panel) return;

            event.preventDefault();
            panel.classList.toggle("hidden");
            if (!panel.classList.contains("hidden")) {
                var input = panel.querySelector("input[type='text'], textarea, select");
                if (input) input.focus();
            }
        });
    }

    function initAnnouncementTabs() {
        var tabRoot = document.querySelector("[data-announcement-tabs]");
        if (!tabRoot) return;

        function setActiveTab(filter) {
            announcementTabFilter = filter;
            tabRoot.querySelectorAll("[data-tab-filter]").forEach(function (button) {
                var active = button.getAttribute("data-tab-filter") === filter;
                button.classList.toggle("bg-white", active);
                button.classList.toggle("text-slate-900", active);
                button.classList.toggle("shadow-sm", active);
                button.classList.toggle("text-slate-500", !active);
            });
            applyAnnouncementFilters();
        }

        tabRoot.addEventListener("click", function (event) {
            var button = event.target.closest("[data-tab-filter]");
            if (!button) return;
            event.preventDefault();
            setActiveTab(button.getAttribute("data-tab-filter"));
        });

        setActiveTab("all");
    }

    function init() {
        initFilters();
        initToasts();
        initComposeToggle();
        initAnnouncementTabs();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
