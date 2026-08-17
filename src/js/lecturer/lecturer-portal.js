(function () {
    function syncMaterialCardHeights() {
        var resources = document.querySelector("[data-published-resources-card]");
        var uploadDetails = document.querySelector("[data-upload-details-card]");
        if (!resources || !uploadDetails) return;

        if (window.matchMedia("(min-width: 1024px)").matches) {
            resources.style.height = uploadDetails.offsetHeight + "px";
        } else {
            resources.style.height = "";
        }
    }

    window.addEventListener("resize", syncMaterialCardHeights);
    document.addEventListener("DOMContentLoaded", function () {
        syncMaterialCardHeights();
        var uploadDetails = document.querySelector("[data-upload-details-card]");
        if (uploadDetails && window.ResizeObserver) {
            new ResizeObserver(syncMaterialCardHeights).observe(uploadDetails);
        }
    });

    var activeMaterialType = "all";
    var MATERIAL_VIEW_KEY = "lecturer.materials.view";
    var weightBackdropPointerDown = null;

    function normalized(value) {
        return String(value || "")
            .replace(/\u00a0/g, " ")
            .trim()
            .toLowerCase();
    }

    function normalizedTerm(value) {
        return normalized(value).replace(/^semester\s+/, "");
    }

    function academicYearLabel(value) {
        var year = Number(value);
        return String(year) === String(value) ? String(year - 1) + "/" + value : value;
    }

    function semesterLabel(value) {
        return /^\d+$/.test(String(value)) ? "Semester " + value : value;
    }

    function saveMaterialView() {
        var search = document.querySelector("[data-filter-target='[data-material]']");
        var courseSelect = document.querySelector("[data-material-course-filter]");
        var yearSelect = document.querySelector("[data-material-year-filter]");
        var semesterSelect = document.querySelector("[data-material-semester-filter]");
        try {
            sessionStorage.setItem(MATERIAL_VIEW_KEY, JSON.stringify({
                type: activeMaterialType,
                course: courseSelect ? courseSelect.value : "all",
                year: yearSelect ? yearSelect.value : "all",
                semester: semesterSelect ? semesterSelect.value : "all",
                search: search ? search.value : ""
            }));
        } catch (error) {
            // The filters still work normally when browser storage is unavailable.
        }
    }

    function restoreMaterialView() {
        try {
            var saved = JSON.parse(sessionStorage.getItem(MATERIAL_VIEW_KEY) || "{}");
            var search = document.querySelector("[data-filter-target='[data-material]']");
            var courseSelect = document.querySelector("[data-material-course-filter]");
            var yearSelect = document.querySelector("[data-material-year-filter]");
            var semesterSelect = document.querySelector("[data-material-semester-filter]");
            var savedTab = document.querySelector(
                '[data-material-tab="' + String(saved.type || "all").replace(/"/g, '\\"') + '"]'
            );

            if (courseSelect && saved.course &&
                Array.prototype.some.call(courseSelect.options, function (option) {
                    return option.value === saved.course;
                })) {
                courseSelect.value = saved.course;
            }
            if (search && typeof saved.search === "string") search.value = saved.search;
            if (yearSelect && saved.year && Array.prototype.some.call(yearSelect.options, function (option) { return option.value === saved.year; })) yearSelect.value = saved.year;
            if (semesterSelect && saved.semester && Array.prototype.some.call(semesterSelect.options, function (option) { return option.value === saved.semester; })) semesterSelect.value = saved.semester;
            if (savedTab) {
                activeMaterialType = savedTab.getAttribute("data-material-tab") || "all";
                document.querySelectorAll("[data-material-tab]").forEach(function (tab) {
                    tab.setAttribute("data-active", tab === savedTab ? "true" : "false");
                });
            }
        } catch (error) {
            activeMaterialType = "all";
        }
    }

    function updateMaterialCounts() {
        var courseSelect = document.querySelector("[data-material-course-filter]");
        var course = courseSelect ? courseSelect.value : "all";
        var yearSelect = document.querySelector("[data-material-year-filter]");
        var semesterSelect = document.querySelector("[data-material-semester-filter]");
        var year = yearSelect ? yearSelect.value : "all";
        var semester = semesterSelect ? semesterSelect.value : "all";
        var counts = { all: 0 };

        document.querySelectorAll("[data-material]").forEach(function (row) {
            var rowCourse = row.getAttribute("data-material-offering") || "";
            var rowYear = row.getAttribute("data-material-year") || "";
            var rowSemester = row.getAttribute("data-material-semester") || "";
            if (normalized(course) !== "all" && normalized(rowCourse) !== normalized(course)) return;
            if (normalized(year) !== "all" && normalized(rowYear) !== normalized(year)) return;
            if (normalized(semester) !== "all" && normalized(rowSemester) !== normalized(semester)) return;
            var type = row.getAttribute("data-material-category") || "";
            counts.all += 1;
            counts[type] = (counts[type] || 0) + 1;
        });

        document.querySelectorAll("[data-material-tab]").forEach(function (tab) {
            var type = tab.getAttribute("data-material-tab") || "all";
            var count = tab.querySelector(".material-tab-count");
            if (count) count.textContent = String(counts[type] || 0);
        });
    }

    function applyMaterialFilters() {
        var search = document.querySelector("[data-filter-target='[data-material]']");
        var courseSelect = document.querySelector("[data-material-course-filter]");
        var yearSelect = document.querySelector("[data-material-year-filter]");
        var semesterSelect = document.querySelector("[data-material-semester-filter]");
        var query = search ? search.value.trim().toLowerCase() : "";
        var course = courseSelect ? courseSelect.value : "all";
        var year = yearSelect ? yearSelect.value : "all";
        var semester = semesterSelect ? semesterSelect.value : "all";

        document.querySelectorAll("[data-material]").forEach(function (row) {
            var type = normalized(row.getAttribute("data-material-category"));
            var rowCourse = row.getAttribute("data-material-offering") || "";
            var rowYear = row.getAttribute("data-material-year") || "";
            var rowSemester = row.getAttribute("data-material-semester") || "";
            var text = (row.getAttribute("data-filter-text") || row.textContent || "").toLowerCase();
            var typeMatch = normalized(activeMaterialType) === "all" || type === normalized(activeMaterialType);
            var courseMatch = normalized(course) === "all" || normalized(rowCourse) === normalized(course);
            var yearMatch = normalized(year) === "all" || normalized(rowYear) === normalized(year);
            var semesterMatch = normalized(semester) === "all" || normalizedTerm(rowSemester) === normalizedTerm(semester);
            var searchMatch = !query || text.indexOf(query) >= 0;
            row.style.display = typeMatch && courseMatch && yearMatch && semesterMatch && searchMatch ? "" : "none";
        });
        updateMaterialCounts();
    }

    function selectMaterialTab(button) {
        activeMaterialType = button.getAttribute("data-material-tab") || "all";
        document.querySelectorAll("[data-material-tab]").forEach(function (tab) {
            tab.setAttribute("data-active", tab === button ? "true" : "false");
        });
        syncPublishTypeToTab();
        saveMaterialView();
        applyMaterialFilters();
    }

    function syncPublishTypeToTab() {
        var typeSelect = document.querySelector("[data-material-type-select]");
        if (!typeSelect) return;
        var desiredType = activeMaterialType === "all" ? "Assignment" : activeMaterialType;
        var exists = Array.prototype.some.call(typeSelect.options, function (option) {
            return option.value === desiredType;
        });
        if (exists) typeSelect.value = desiredType;
        updateMaterialForm();
    }

    function initUploadCourseFilters() {
        var yearSelect = document.querySelector("[data-upload-year-select]");
        var semesterSelect = document.querySelector("[data-upload-semester-select]");
        var courseSelect = document.querySelector("[data-material-course-select]");
        if (!yearSelect || !semesterSelect || !courseSelect) return;

        var courses = Array.prototype.slice.call(courseSelect.options, 1).map(function (option) {
            return {
                value: option.value,
                label: option.textContent,
                year: option.getAttribute("data-year") || "",
                semester: option.getAttribute("data-semester") || ""
            };
        });

        function resetSelect(select, label, disabled) {
            select.innerHTML = "";
            var option = document.createElement("option");
            option.value = "";
            option.textContent = label;
            select.appendChild(option);
            select.disabled = disabled;
        }

        function loadSemesters() {
            var year = yearSelect.value;
            resetSelect(semesterSelect, year ? "Choose semester" : "Choose academic year first", !year);
            resetSelect(courseSelect, "Choose semester first", true);
            if (!year) return;

            var semesters = [];
            courses.forEach(function (course) {
                if (course.year === year && semesters.indexOf(course.semester) === -1) semesters.push(course.semester);
            });
            semesters.forEach(function (semester) {
                var option = document.createElement("option");
                option.value = semester;
                option.textContent = semesterLabel(semester);
                semesterSelect.appendChild(option);
            });
        }

        function loadCourses() {
            var year = yearSelect.value;
            var semester = semesterSelect.value;
            resetSelect(courseSelect, semester ? "Choose course" : "Choose semester first", !semester);
            if (!year || !semester) return;

            courses.forEach(function (course) {
                if (course.year !== year || normalizedTerm(course.semester) !== normalizedTerm(semester)) return;
                var option = document.createElement("option");
                option.value = course.value;
                option.textContent = course.label;
                option.setAttribute("data-year", course.year);
                option.setAttribute("data-semester", course.semester);
                courseSelect.appendChild(option);
            });
        }

        yearSelect.addEventListener("change", loadSemesters);
        semesterSelect.addEventListener("change", loadCourses);
        loadSemesters();
    }

    function initPublishedCourseFilters() {
        var courseSelect = document.querySelector("[data-material-course-filter]");
        var yearSelect = document.querySelector("[data-material-year-filter]");
        var semesterSelect = document.querySelector("[data-material-semester-filter]");
        if (!courseSelect || !yearSelect || !semesterSelect) return;

        var courses = Array.prototype.slice.call(courseSelect.options, 1).map(function (option) {
            return {
                value: option.value,
                label: option.textContent,
                year: option.getAttribute("data-year") || "",
                semester: option.getAttribute("data-semester") || ""
            };
        });

        function addOption(value, label, year, semester) {
            var option = document.createElement("option");
            option.value = value;
            option.textContent = label;
            if (year) option.setAttribute("data-year", year);
            if (semester) option.setAttribute("data-semester", semester);
            courseSelect.appendChild(option);
        }

        function rebuildCourses() {
            var previous = courseSelect.value || "all";
            var year = yearSelect.value;
            var semester = semesterSelect.value;
            var hasPrevious = previous === "all";

            courseSelect.innerHTML = "";
            addOption("all", "All courses");

            courses.forEach(function (course) {
                var yearMatch = normalized(year) === "all" || normalized(course.year) === normalized(year);
                var semesterMatch = normalized(semester) === "all" || normalizedTerm(course.semester) === normalizedTerm(semester);
                if (!yearMatch || !semesterMatch) return;
                addOption(course.value, course.label, course.year, course.semester);
                if (course.value === previous) hasPrevious = true;
            });

            courseSelect.value = hasPrevious ? previous : "all";
        }

        [yearSelect, semesterSelect].forEach(function (filter) {
            filter.addEventListener("change", function () {
                rebuildCourses();
                saveMaterialView();
                applyMaterialFilters();
            });
        });

        rebuildCourses();
    }

    function updateMaterialForm() {
        var typeSelect = document.querySelector("[data-material-type-select]");
        if (!typeSelect) return;

        var type = typeSelect.value;
        var lectureNotes = type === "Lecture Notes";
        var quiz = type === "Quiz";
        var viva = type === "Viva";
        var requiresAssessmentDetails = type === "Assignment" || type === "Quiz" || type === "Test" || viva;
        var modeField = document.querySelector("[data-assessment-mode-field]");
        var modeSelect = document.querySelector("[data-assessment-mode]");
        var dueInput = document.querySelector("[data-due-date-field] input");
        var dueTimeInput = document.querySelector("[data-due-time-field] input");
        var weightInput = document.querySelector("[data-weight-field] input");
        var fileInput = document.querySelector("[data-file-field] input[type='file']");
        var description = document.querySelector("[data-material-description]");
        var requiredMark = document.querySelector("[data-description-required]");
        var fileRequiredMark = document.querySelector("[data-file-required]");
        var dueDateRequiredMark = document.querySelector("[data-due-date-required]");
        var dueTimeRequiredMark = document.querySelector("[data-due-time-required]");
        var weightRequiredMark = document.querySelector("[data-weight-required]");
        var weekField = document.querySelector("[data-week-field]");
        var weekSelect = document.querySelector("[data-material-week-select]");

        if (modeField) modeField.classList.toggle("hidden", !viva);
        if (modeSelect) {
            modeSelect.disabled = !viva;
            modeSelect.required = viva;
            if (modeSelect.getAttribute("data-for-type") !== type) {
                modeSelect.value = viva ? "" : type === "Assignment" ? "FILE" : "MANUAL";
                modeSelect.setAttribute("data-for-type", type);
            }
        }

        [dueInput, dueTimeInput, weightInput].forEach(function (input) {
            if (!input) return;
            input.disabled = lectureNotes;
            input.required = requiresAssessmentDetails;
            if (lectureNotes) input.value = "";
            if (!lectureNotes && input === dueTimeInput && !input.value) input.value = "23:59";
            var field = input.closest("label");
            if (field) field.classList.toggle("opacity-45", lectureNotes);
        });

        if (fileInput) {
            fileInput.disabled = quiz;
            if (quiz) fileInput.value = "";
            var fileField = fileInput.closest("label");
            if (fileField) fileField.classList.toggle("opacity-60", quiz);
        }

        if (description) {
            description.required = quiz;
            description.placeholder = quiz
                ? "Google Form quiz link"
                : "Short student-facing description";
        }
        if (requiredMark) requiredMark.classList.toggle("hidden", !quiz);
        if (fileRequiredMark) fileRequiredMark.classList.toggle("hidden", quiz);
        if (dueDateRequiredMark) dueDateRequiredMark.classList.toggle("hidden", !requiresAssessmentDetails);
        if (dueTimeRequiredMark) dueTimeRequiredMark.classList.toggle("hidden", !requiresAssessmentDetails);
        if (weightRequiredMark) weightRequiredMark.classList.toggle("hidden", !requiresAssessmentDetails);
        if (weekField) weekField.classList.toggle("hidden", !lectureNotes);
        if (weekSelect) {
            weekSelect.disabled = !lectureNotes;
            weekSelect.required = lectureNotes;
        }
    }

    async function deleteMaterial(button) {
        var materialId = Number(button.getAttribute("data-delete-material"));
        if (!materialId || !window.confirm("Delete this material?")) return;

        button.disabled = true;
        button.classList.add("opacity-50");
        try {
            var response = await fetch(window.location.pathname + "/DeleteMaterial", {
                method: "POST",
                credentials: "same-origin",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify({ materialId: materialId })
            });
            if (!response.ok) throw new Error("The material server could not be reached.");
            var body = await response.json();
            var result = body && Object.prototype.hasOwnProperty.call(body, "d") ? body.d : body;
            if (!result || !result.success) {
                throw new Error(result && result.message ? result.message : "Material could not be deleted.");
            }

            var row = button.closest("[data-material]");
            if (row) row.remove();
            applyMaterialFilters();
        } catch (error) {
            window.alert(error.message || "Material could not be deleted.");
            button.disabled = false;
            button.classList.remove("opacity-50");
        }
    }

    function showMaterialStatus(message, success) {
        var status = document.querySelector("[data-material-client-status]");
        if (!status) return;
        status.textContent = message || "";
        status.classList.toggle("hidden", !message);
        status.classList.toggle("border-emerald-200", !!success);
        status.classList.toggle("bg-emerald-50", !!success);
        status.classList.toggle("text-emerald-800", !!success);
        status.classList.toggle("border-amber-200", !success);
        status.classList.toggle("bg-amber-50", !success);
        status.classList.toggle("text-amber-800", !success);
        if (message) {
            status.scrollIntoView({ behavior: "smooth", block: "start" });
        }
    }

    function showRequiredError(message, field) {
        showMaterialStatus(message, false);
        if (field) {
            field.focus();
            field.classList.add("border-[#e0162b]", "ring-1", "ring-[#e0162b]/30");
            var clearError = function () {
                field.classList.remove("border-[#e0162b]", "ring-1", "ring-[#e0162b]/30");
                field.removeEventListener("input", clearError);
                field.removeEventListener("change", clearError);
            };
            field.addEventListener("input", clearError);
            field.addEventListener("change", clearError);
        }
        return false;
    }

    function uploadField(selector) {
        var uploadCard = document.querySelector("[data-upload-details-card]");
        return uploadCard ? uploadCard.querySelector(selector) : document.querySelector(selector);
    }

    function validateMaterialPublish() {
        var uploadYear = uploadField("[data-upload-year-select]");
        var uploadSemester = uploadField("[data-upload-semester-select]");
        var course = uploadField("[data-material-course-select]");
        var typeSelect = uploadField("[data-material-type-select]");
        var title = uploadField("input[data-material-title]");
        var description = uploadField("[data-material-description]");
        var dueDate = uploadField("[data-material-due-date]");
        var dueTime = uploadField("[data-material-due-time]");
        var weight = uploadField("[data-material-weight]");
        var week = uploadField("[data-material-week-select]");
        var file = uploadField("[data-material-file]");
        var submissionMode = uploadField("[data-assessment-mode]");
        var type = typeSelect ? typeSelect.value : "";
        var requiresAssessment = type === "Assignment" || type === "Quiz" || type === "Test" || type === "Viva";
        var requiresPositiveWeight = type === "Assignment" || type === "Test" || type === "Viva";
        var lectureNotes = type === "Lecture Notes";
        var quiz = type === "Quiz";
        var viva = type === "Viva";

        if (uploadYear && !uploadYear.value) {
            return showRequiredError("Choose an academic year first.", uploadYear);
        }
        if (uploadSemester && !uploadSemester.value) {
            return showRequiredError("Choose a semester first.", uploadSemester);
        }
        if (course && !course.value) {
            return showRequiredError("Choose a course before publishing.", course);
        }

        if (title && !title.value.trim()) {
            return showRequiredError("Title is required.", title);
        }
        if (title) title.setCustomValidity("");
        if (lectureNotes && week && !week.value) {
            return showRequiredError("Week is required for lecture notes.", week);
        }
        if (week) week.setCustomValidity("");
        if (viva && submissionMode && !submissionMode.value) {
            return showRequiredError(
                "Choose whether this Viva uses a Google Drive video link or lecturer-entered marks.",
                submissionMode
            );
        }
        if (requiresAssessment && dueDate && !dueDate.value) {
            return showRequiredError("Due date is required for assessments.", dueDate);
        }
        if (dueDate) dueDate.setCustomValidity("");
        if (requiresAssessment && dueTime && !dueTime.value) {
            return showRequiredError("Due time is required for assessments.", dueTime);
        }
        if (dueTime) dueTime.setCustomValidity("");
        if (requiresAssessment && weight && !weight.value) {
            return showRequiredError("Course weight is required for assessments.", weight);
        }
        if (requiresPositiveWeight && weight && Number(weight.value) <= 0) {
            return showRequiredError("Course weight for assessments must be greater than 0%.", weight);
        }
        if (weight) weight.setCustomValidity("");
        if (quiz && description && !description.value.trim()) {
            return showRequiredError("Google Forms quiz link is required.", description);
        }
        if (quiz && description) {
            try {
                var quizUrl = new URL(description.value.trim());
                var host = quizUrl.hostname.toLowerCase().replace(/\.$/, "");
                var googleForm = host === "forms.gle" ||
                    (host === "docs.google.com" && quizUrl.pathname.toLowerCase().indexOf("/forms/") === 0);
                if (!googleForm) {
                    return showRequiredError(
                        "Quiz links must use Google Forms. Please paste a forms.gle or docs.google.com/forms sharing link.",
                        description
                    );
                }
            } catch (error) {
                return showRequiredError(
                    "Quiz links must use Google Forms. Please paste a valid Google Forms sharing link.",
                    description
                );
            }
        }
        if (description) description.setCustomValidity("");
        if (!quiz && file && (!file.files || !file.files.length)) {
            return showRequiredError("Please choose a file to upload.", file);
        }
        if (file) file.setCustomValidity("");
        return true;
    }

    async function insertPublishedMaterial(materialId) {
        var response = await fetch(window.location.pathname, {
            credentials: "same-origin",
            cache: "no-store"
        });
        if (!response.ok) return;
        var html = await response.text();
        var documentCopy = new DOMParser().parseFromString(html, "text/html");
        var freshRow = documentCopy.querySelector('[data-material-id="' + materialId + '"]');
        var list = document.querySelector("[data-material-list]");
        if (!freshRow || !list) return null;
        var imported = document.importNode(freshRow, true);
        imported.setAttribute("tabindex", "0");
        imported.setAttribute("role", "link");
        list.insertBefore(imported, list.firstChild);
        if (window.lucide) window.lucide.createIcons();
        return imported;
    }

    function revealPublishedMaterial(row) {
        if (!row) return;
        var type = row.getAttribute("data-material-category") || "all";
        var course = row.getAttribute("data-material-offering") || "all";
        var year = row.getAttribute("data-material-year") || "all";
        var semester = row.getAttribute("data-material-semester") || "all";
        var search = document.querySelector("[data-filter-target='[data-material]']");
        var courseSelect = document.querySelector("[data-material-course-filter]");
        var yearSelect = document.querySelector("[data-material-year-filter]");
        var semesterSelect = document.querySelector("[data-material-semester-filter]");

        activeMaterialType = type;
        document.querySelectorAll("[data-material-tab]").forEach(function (tab) {
            tab.setAttribute("data-active",
                tab.getAttribute("data-material-tab") === type ? "true" : "false");
        });
        if (search) search.value = "";
        if (courseSelect) courseSelect.value = course;
        if (yearSelect) yearSelect.value = year;
        if (semesterSelect) semesterSelect.value = semester;
        applyMaterialFilters();
        row.scrollIntoView({ behavior: "smooth", block: "center" });
    }

    async function publishMaterial(button) {
        if (!validateMaterialPublish()) return;

        var course = uploadField("[data-material-course-select]");
        var type = uploadField("[data-material-type-select]");
        var title = uploadField("input[data-material-title]");
        var description = uploadField("[data-material-description]");
        var dueDate = uploadField("[data-material-due-date]");
        var dueTime = uploadField("[data-material-due-time]");
        var weight = uploadField("[data-material-weight]");
        var week = uploadField("[data-material-week-select]");
        var file = uploadField("[data-material-file]");
        var submissionMode = uploadField("[data-assessment-mode]");
        var label = button.querySelector("[data-publish-label]");
        var formData = new FormData();
        formData.append("offeringId", course ? course.value : "");
        formData.append("materialType", type ? type.value : "");
        formData.append("submissionMode", submissionMode ? submissionMode.value : "");
        formData.append("title", title ? title.value : "");
        formData.append("description", description ? description.value : "");
        formData.append("dueDate", dueDate ? dueDate.value : "");
        formData.append("dueTime", dueTime ? dueTime.value : "");
        formData.append("weight", weight ? weight.value : "");
        formData.append("week", week ? week.value : "");
        if (file && file.files && file.files.length) formData.append("file", file.files[0]);

        button.classList.add("pointer-events-none", "opacity-60");
        button.setAttribute("aria-disabled", "true");
        if (label) label.textContent = "Publishing...";
        showMaterialStatus("", false);

        try {
            var endpoint = new URL("material_publish.ashx", window.location.href);
            var response = await fetch(endpoint.href, {
                method: "POST",
                credentials: "same-origin",
                body: formData
            });
            if (!response.ok) throw new Error("The material server could not be reached.");
            var result = await response.json();
            if (!result || !result.success) {
                throw new Error(result && result.message ? result.message : "Material could not be published.");
            }

            var publishedRow = await insertPublishedMaterial(result.materialId);
            if (title) title.value = "";
            if (description) description.value = "";
            if (dueDate) dueDate.value = "";
            if (dueTime) dueTime.value = "23:59";
            if (weight) weight.value = "";
            if (file) file.value = "";
            revealPublishedMaterial(publishedRow);
            showMaterialStatus(result.message, true);
        } catch (error) {
            showMaterialStatus(error.message || "Material could not be published.", false);
        } finally {
            button.classList.remove("pointer-events-none", "opacity-60");
            button.removeAttribute("aria-disabled");
            if (label) label.textContent = "Publish material";
        }
    }

    function showWeightUpdateStatus(message, success) {
        var status = document.querySelector("[data-weight-update-status]");
        if (!status) return;
        status.textContent = message || "";
        status.classList.remove(
            "hidden", "border-emerald-200", "bg-emerald-50", "text-emerald-800",
            "border-amber-200", "bg-amber-50", "text-amber-800"
        );
        status.classList.add(
            success ? "border-emerald-200" : "border-amber-200",
            success ? "bg-emerald-50" : "bg-amber-50",
            success ? "text-emerald-800" : "text-amber-800"
        );
        window.setTimeout(function () { status.classList.add("hidden"); }, 4500);
    }

    function openWeightEditor(button) {
        var dialog = document.querySelector("[data-weight-editor]");
        if (!dialog || typeof dialog.showModal !== "function") return;
        var input = dialog.querySelector("[data-weight-editor-input]");
        var title = dialog.querySelector("[data-weight-editor-title]");
        var error = dialog.querySelector("[data-weight-editor-error]");
        dialog.setAttribute("data-material-id", button.getAttribute("data-edit-material-weight") || "");
        if (title) title.textContent = button.getAttribute("data-material-title") || "Assessment";
        if (input) input.value = button.getAttribute("data-current-weight") || "";
        if (error) {
            error.textContent = "";
            error.classList.add("hidden");
        }
        dialog.showModal();
        if (input) {
            input.focus();
            input.select();
        }
        if (window.lucide) window.lucide.createIcons();
    }

    function closeWeightEditor(dialog) {
        if (dialog && dialog.open) dialog.close();
    }

    async function saveMaterialWeight(button) {
        var dialog = button.closest("[data-weight-editor]");
        var input = dialog && dialog.querySelector("[data-weight-editor-input]");
        var error = dialog && dialog.querySelector("[data-weight-editor-error]");
        var materialId = dialog ? Number(dialog.getAttribute("data-material-id")) : 0;
        var weight = input ? Number(input.value) : 0;

        if (!materialId || !Number.isFinite(weight) || weight <= 0 || weight > 100) {
            if (error) {
                error.textContent = "Enter a course weight greater than 0% and no more than 100%.";
                error.classList.remove("hidden");
            }
            if (input) input.focus();
            return;
        }

        button.disabled = true;
        button.classList.add("opacity-60");
        if (error) error.classList.add("hidden");
        try {
            var response = await fetch(window.location.pathname + "/UpdateMaterialWeight", {
                method: "POST",
                credentials: "same-origin",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify({ materialId: materialId, weight: weight })
            });
            if (!response.ok) throw new Error("The material server could not be reached.");
            var body = await response.json();
            var result = body && Object.prototype.hasOwnProperty.call(body, "d") ? body.d : body;
            if (!result || !result.success)
                throw new Error(result && result.message ? result.message : "Course weight could not be updated.");

            var formatted = Number(result.weight).toFixed(2).replace(/\.?0+$/, "") + "%";
            document.querySelectorAll('[data-material-weight-display="' + materialId + '"]').forEach(function (display) {
                display.textContent = formatted;
            });
            document.querySelectorAll('[data-edit-material-weight="' + materialId + '"]').forEach(function (editor) {
                editor.setAttribute("data-current-weight", String(result.weight));
            });
            closeWeightEditor(dialog);
            showWeightUpdateStatus(result.message, true);
        } catch (saveError) {
            if (error) {
                error.textContent = saveError.message || "Course weight could not be updated.";
                error.classList.remove("hidden");
            }
        } finally {
            button.disabled = false;
            button.classList.remove("opacity-60");
        }
    }

    document.addEventListener("pointerdown", function (event) {
        if (event.target.matches("[data-weight-editor]")) {
            weightBackdropPointerDown = { x: event.clientX, y: event.clientY };
        } else if (event.target.closest("[data-weight-editor]")) {
            weightBackdropPointerDown = null;
        }
    });

    document.addEventListener("pointercancel", function () {
        weightBackdropPointerDown = null;
    });

    document.addEventListener("click", function (event) {
        var weightEditorButton = event.target.closest("[data-edit-material-weight]");
        if (weightEditorButton) {
            event.preventDefault();
            event.stopPropagation();
            openWeightEditor(weightEditorButton);
            return;
        }

        var weightEditorClose = event.target.closest("[data-weight-editor-close]");
        if (weightEditorClose) {
            event.preventDefault();
            closeWeightEditor(weightEditorClose.closest("[data-weight-editor]"));
            return;
        }

        var weightSaveButton = event.target.closest("[data-save-material-weight]");
        if (weightSaveButton) {
            event.preventDefault();
            saveMaterialWeight(weightSaveButton);
            return;
        }

        if (event.target.matches("[data-weight-editor]")) {
            var weightDialogRect = event.target.getBoundingClientRect();
            var insideWeightDialog =
                event.clientX >= weightDialogRect.left &&
                event.clientX <= weightDialogRect.right &&
                event.clientY >= weightDialogRect.top &&
                event.clientY <= weightDialogRect.bottom;
            var backdropPress = weightBackdropPointerDown;
            weightBackdropPointerDown = null;
            var barelyMoved = backdropPress &&
                Math.abs(event.clientX - backdropPress.x) <= 5 &&
                Math.abs(event.clientY - backdropPress.y) <= 5;
            if (!insideWeightDialog && barelyMoved) closeWeightEditor(event.target);
            return;
        }

        var tab = event.target.closest("[data-material-tab]");
        if (tab) {
            event.preventDefault();
            selectMaterialTab(tab);
            return;
        }

        var deleteButton = event.target.closest("[data-delete-material]");
        if (deleteButton) {
            event.preventDefault();
            event.stopPropagation();
            deleteMaterial(deleteButton);
            return;
        }

        var publishButton = event.target.closest("[data-publish-material]");
        if (publishButton) {
            event.preventDefault();
            publishMaterial(publishButton);
            return;
        }

        var row = event.target.closest("[data-material-url]");
        if (!row || event.target.closest("a,button,input,select,textarea")) return;
        var url = row.getAttribute("data-material-url");
        if (url) window.location.href = url;
    });

    document.addEventListener("DOMContentLoaded", function () {
        var search = document.querySelector("[data-filter-target='[data-material]']");
        var courseSelect = document.querySelector("[data-material-course-filter]");
        var yearFilter = document.querySelector("[data-material-year-filter]");
        var semesterFilter = document.querySelector("[data-material-semester-filter]");
        var typeSelect = document.querySelector("[data-material-type-select]");
        if (search) search.addEventListener("input", function () {
            saveMaterialView();
            applyMaterialFilters();
        });
        if (courseSelect) courseSelect.addEventListener("change", function () {
            saveMaterialView();
            applyMaterialFilters();
        });
        [yearFilter, semesterFilter].forEach(function (filter) {
            if (!filter) return;
            filter.addEventListener("change", function () {
                saveMaterialView();
                applyMaterialFilters();
            });
        });
        if (typeSelect) typeSelect.addEventListener("change", updateMaterialForm);

        document.querySelectorAll("[data-material-url]").forEach(function (row) {
            row.setAttribute("tabindex", "0");
            row.setAttribute("role", "link");
            row.addEventListener("keydown", function (event) {
                if (event.key !== "Enter" && event.key !== " ") return;
                if (event.target.closest("a,button,input,select,textarea")) return;
                event.preventDefault();
                var url = row.getAttribute("data-material-url");
                if (url) window.location.href = url;
            });
        });
        restoreMaterialView();
        initPublishedCourseFilters();
        initUploadCourseFilters();
        syncPublishTypeToTab();
        applyMaterialFilters();
        updateMaterialForm();
    });
})();
﻿
