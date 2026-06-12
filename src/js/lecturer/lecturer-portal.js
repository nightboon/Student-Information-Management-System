(function () {
    function initFilters() {
        document.querySelectorAll("[data-filter-input]").forEach(function (input) {
            var selector = input.getAttribute("data-filter-target");
            input.addEventListener("input", function () {
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

    function init() {
        initFilters();
        initToasts();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
