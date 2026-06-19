// Course enrollment page: live selection totals + persist on "Proceed to Payment".
// Each enrollable <article data-course-row> carries:
//   data-credits   – integer credit hours
//   data-fee       – fee for this course (credits * rate)
// and contains a checkbox [data-action="toggle-enroll"][data-offering="<id>"].
(function () {
    "use strict";

    function rows() {
        return Array.prototype.slice.call(document.querySelectorAll("[data-course-row]"));
    }

    function checkbox(row) {
        return row.querySelector('[data-action="toggle-enroll"]');
    }

    function selectedRows() {
        return rows().filter(function (row) {
            var cb = checkbox(row);
            return cb && cb.checked;
        });
    }

    function setText(id, value) {
        var el = document.getElementById(id);
        if (el) el.textContent = value;
    }

    function recompute() {
        var selected = selectedRows();
        var count = selected.length;
        var credits = 0;
        var fee = 0;
        selected.forEach(function (row) {
            credits += parseInt(row.getAttribute("data-credits"), 10) || 0;
            fee += parseFloat(row.getAttribute("data-fee")) || 0;
        });

        setText("enroll-count", count);
        setText("enroll-credits", credits);
        setText("enroll-total", fee);
        setText("enroll-count-footer", count);
        setText("enroll-credits-footer", credits);
        setText("enroll-total-footer", fee.toFixed(2));

        var submit = document.getElementById("enroll-submit");
        if (submit) {
            var enabled = count > 0;
            submit.disabled = !enabled;
            submit.classList.toggle("bg-slate-100", !enabled);
            submit.classList.toggle("text-slate-400", !enabled);
            submit.classList.toggle("cursor-not-allowed", !enabled);
            submit.classList.toggle("bg-[#e0162b]", enabled);
            submit.classList.toggle("text-white", enabled);
            submit.classList.toggle("hover:bg-[#a01020]", enabled);
        }
    }

    function selectedOfferingIds() {
        return selectedRows().map(function (row) {
            return parseInt(checkbox(row).getAttribute("data-offering"), 10);
        }).filter(function (n) { return !isNaN(n); });
    }

    function proceed() {
        var ids = selectedOfferingIds();
        if (ids.length === 0) return;

        var submit = document.getElementById("enroll-submit");
        if (submit) submit.disabled = true;

        fetch("/student/enrollment.aspx/Enrol", {
            method: "POST",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify({ offeringIds: ids })
        }).then(function (res) {
            if (!res.ok) throw new Error("enrol failed");
            // Reload so newly enrolled courses render as Registered.
            window.location.reload();
        }).catch(function () {
            if (submit) submit.disabled = false;
            alert("Enrollment could not be completed. Please try again.");
        });
    }

    document.addEventListener("change", function (e) {
        if (e.target && e.target.matches('[data-action="toggle-enroll"]')) {
            recompute();
        }
    });

    document.addEventListener("click", function (e) {
        var btn = e.target.closest ? e.target.closest('[data-action="proceed-to-payment"]') : null;
        if (btn) {
            e.preventDefault();
            proceed();
        }
    });

    recompute();
})();
