(function () {
    var announcementTabFilter = "all";
    var materialTabFilter = "all";

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

                if (selector === "[data-material]") {
                    applyMaterialFilters();
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

    function materialMatchesTab(item) {
        return materialTabFilter === "all" ||
            (item.getAttribute("data-material-category") || "").toLowerCase() === materialTabFilter.toLowerCase();
    }

    function materialMatchesSearch(item, query) {
        var text = (item.getAttribute("data-filter-text") || item.textContent || "").toLowerCase();
        return text.indexOf(query) >= 0;
    }

    function applyMaterialFilters() {
        var input = document.querySelector("[data-filter-target='[data-material]']");
        var courseFilter = document.querySelector("[data-material-course-filter]");
        var query = input ? input.value.trim().toLowerCase() : "";
        var course = courseFilter ? courseFilter.value : "all";
        document.querySelectorAll("[data-material]").forEach(function (item) {
            var courseMatch = course === "all" || item.getAttribute("data-material-course") === course;
            item.style.display = materialMatchesTab(item) && materialMatchesSearch(item, query) && courseMatch ? "" : "none";
        });
    }

    function initMaterialTabs() {
        var tabRoot = document.querySelector("[data-material-tabs]");
        if (!tabRoot) return;

        function setActiveTab(filter) {
            materialTabFilter = filter;
            tabRoot.querySelectorAll("[data-material-tab]").forEach(function (button) {
                var active = button.getAttribute("data-material-tab") === filter;
                var count = button.querySelector("span");
                button.classList.toggle("bg-slate-900", active);
                button.classList.toggle("text-white", active);
                button.classList.toggle("bg-slate-100", !active);
                button.classList.toggle("text-slate-600", !active);
                button.classList.toggle("hover:bg-slate-200", !active);
                if (count) {
                    count.classList.toggle("bg-white/20", active);
                    count.classList.toggle("text-white", active);
                    count.classList.toggle("bg-white", !active);
                    count.classList.toggle("text-slate-600", !active);
                }
            });
            applyMaterialFilters();
        }

        tabRoot.addEventListener("click", function (event) {
            var button = event.target.closest("[data-material-tab]");
            if (!button) return;
            event.preventDefault();
            setActiveTab(button.getAttribute("data-material-tab"));
        });

        var courseFilter = document.querySelector("[data-material-course-filter]");
        if (courseFilter) {
            courseFilter.addEventListener("change", applyMaterialFilters);
        }

        setActiveTab("all");
    }

    function initMaterialRows() {
        document.addEventListener("click", function (event) {
            var row = event.target.closest("[data-material-url]");
            if (!row || event.target.closest("a, button, input, select, textarea")) return;
            var url = row.getAttribute("data-material-url");
            if (url) window.location.href = url;
        });

        document.querySelectorAll("[data-material-url]").forEach(function (row) {
            row.setAttribute("tabindex", "0");
            row.setAttribute("role", "link");
            row.addEventListener("keydown", function (event) {
                if (event.key !== "Enter" && event.key !== " ") return;
                if (event.target.closest("a, button, input, select, textarea")) return;
                event.preventDefault();
                var url = row.getAttribute("data-material-url");
                if (url) window.location.href = url;
            });
        });
    }

    function modalById(id) {
        return document.querySelector("[data-review-modal='" + id + "']");
    }

    function resizeAnnotationCanvas(modal) {
        var shell = modal.querySelector("[data-preview-shell]");
        var frame = modal.querySelector("[data-submission-preview]");
        var canvas = modal.querySelector("[data-annotation-canvas]");
        if (!shell || !frame || !canvas) return;

        var shellBox = shell.getBoundingClientRect();
        var frameBox = frame.getBoundingClientRect();
        var left = frameBox.left - shellBox.left + shell.scrollLeft;
        var top = frameBox.top - shellBox.top + shell.scrollTop;
        var width = frameBox.width;
        var height = frameBox.height;
        var ratio = window.devicePixelRatio || 1;

        canvas.style.left = left + "px";
        canvas.style.top = top + "px";
        canvas.style.width = width + "px";
        canvas.style.height = height + "px";
        canvas.width = Math.max(1, Math.floor(width * ratio));
        canvas.height = Math.max(1, Math.floor(height * ratio));

        var ctx = canvas.getContext("2d");
        ctx.setTransform(ratio, 0, 0, ratio, 0, 0);
    }

    function setAnnotateTool(modal, tool) {
        modal.setAttribute("data-active-annotation-tool", tool);
        modal.querySelectorAll("[data-annotate-tool]").forEach(function (button) {
            var active = button.getAttribute("data-annotate-tool") === tool;
            button.classList.toggle("bg-white/15", active);
        });
        var canvas = modal.querySelector("[data-annotation-canvas]");
        if (canvas) canvas.style.pointerEvents = tool === "pointer" ? "none" : "auto";
    }

    function getCanvasPoint(canvas, event) {
        var box = canvas.getBoundingClientRect();
        return {
            x: event.clientX - box.left,
            y: event.clientY - box.top
        };
    }

    function drawBookmark(ctx, point, text) {
        ctx.save();
        ctx.fillStyle = "#e0162b";
        ctx.strokeStyle = "#a01020";
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.arc(point.x, point.y, 7, 0, Math.PI * 2);
        ctx.fill();
        ctx.beginPath();
        ctx.moveTo(point.x, point.y + 7);
        ctx.lineTo(point.x - 5, point.y + 20);
        ctx.lineTo(point.x + 5, point.y + 20);
        ctx.closePath();
        ctx.fill();
        if (text) {
            ctx.fillStyle = "rgba(255,255,255,0.95)";
            ctx.strokeStyle = "#cbd5e1";
            ctx.fillRect(point.x + 12, point.y - 14, 190, 38);
            ctx.strokeRect(point.x + 12, point.y - 14, 190, 38);
            ctx.fillStyle = "#0f172a";
            ctx.font = "13px Arial";
            ctx.fillText(text.substring(0, 32), point.x + 20, point.y + 9);
        }
        ctx.restore();
    }

    function initAnnotationCanvas(modal) {
        var canvas = modal.querySelector("[data-annotation-canvas]");
        if (!canvas || canvas.getAttribute("data-ready") === "true") return;

        canvas.setAttribute("data-ready", "true");
        var drawing = false;
        var start = null;
        var last = null;

        canvas.addEventListener("mousedown", function (event) {
            var tool = modal.getAttribute("data-active-annotation-tool") || "pointer";
            if (tool === "pointer") return;
            var ctx = canvas.getContext("2d");
            var point = getCanvasPoint(canvas, event);

            if (tool === "text") {
                var text = window.prompt("Text to place on the submission");
                if (!text) return;
                ctx.save();
                ctx.fillStyle = "#0f172a";
                ctx.font = "16px Arial";
                ctx.fillText(text, point.x, point.y);
                ctx.restore();
                return;
            }

            if (tool === "bookmark") {
                var comment = window.prompt("Comment for this bookmark");
                drawBookmark(ctx, point, comment || "");
                return;
            }

            drawing = true;
            start = point;
            last = point;
            event.preventDefault();
        });

        canvas.addEventListener("mousemove", function (event) {
            if (!drawing) return;
            var tool = modal.getAttribute("data-active-annotation-tool") || "draw";
            var ctx = canvas.getContext("2d");
            var point = getCanvasPoint(canvas, event);

            ctx.save();
            if (tool === "highlight") {
                ctx.globalAlpha = 0.35;
                ctx.strokeStyle = "#facc15";
                ctx.lineWidth = 18;
                ctx.lineCap = "round";
            } else if (tool === "draw") {
                ctx.strokeStyle = "#e0162b";
                ctx.lineWidth = 3;
                ctx.lineCap = "round";
            } else {
                ctx.restore();
                return;
            }
            ctx.beginPath();
            ctx.moveTo(last.x, last.y);
            ctx.lineTo(point.x, point.y);
            ctx.stroke();
            ctx.restore();
            last = point;
            event.preventDefault();
        });

        window.addEventListener("mouseup", function (event) {
            if (!drawing) return;
            var tool = modal.getAttribute("data-active-annotation-tool") || "draw";
            var ctx = canvas.getContext("2d");
            var point = getCanvasPoint(canvas, event);
            if (tool === "strike" && start) {
                ctx.save();
                ctx.strokeStyle = "#e0162b";
                ctx.lineWidth = 3;
                ctx.beginPath();
                ctx.moveTo(start.x, start.y);
                ctx.lineTo(point.x, point.y);
                ctx.stroke();
                ctx.restore();
            }
            drawing = false;
            start = null;
            last = null;
        });
    }

    function initReviewModals() {
        document.addEventListener("click", function (event) {
            var openButton = event.target.closest("[data-review-open]");
            if (openButton) {
                event.preventDefault();
                var modal = modalById(openButton.getAttribute("data-review-open"));
                if (!modal) return;
                modal.classList.remove("hidden");
                document.body.classList.add("overflow-hidden");
                resizeAnnotationCanvas(modal);
                initAnnotationCanvas(modal);
                setAnnotateTool(modal, "pointer");
                if (window.lucide) window.lucide.createIcons();
                return;
            }

            var closeButton = event.target.closest("[data-review-close]");
            if (closeButton) {
                event.preventDefault();
                var closeModal = closeButton.closest("[data-review-modal]");
                if (!closeModal) return;
                closeModal.classList.add("hidden");
                document.body.classList.remove("overflow-hidden");
                return;
            }

            var toolButton = event.target.closest("[data-annotate-tool]");
            if (toolButton) {
                event.preventDefault();
                setAnnotateTool(toolButton.closest("[data-review-modal]"), toolButton.getAttribute("data-annotate-tool"));
                return;
            }

            var clearButton = event.target.closest("[data-annotate-clear]");
            if (clearButton) {
                event.preventDefault();
                var clearCanvas = clearButton.closest("[data-review-modal]").querySelector("[data-annotation-canvas]");
                clearCanvas.getContext("2d").clearRect(0, 0, clearCanvas.width, clearCanvas.height);
                return;
            }

            var downloadButton = event.target.closest("[data-annotate-download]");
            if (downloadButton) {
                event.preventDefault();
                window.print();
            }
        });

        window.addEventListener("resize", function () {
            document.querySelectorAll("[data-review-modal]:not(.hidden)").forEach(resizeAnnotationCanvas);
        });
    }

    function init() {
        initFilters();
        initToasts();
        initComposeToggle();
        initAnnouncementTabs();
        initMaterialTabs();
        initMaterialRows();
        initReviewModals();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
