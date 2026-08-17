(function () {
    // Generate a branded invoice PDF for a single payment row using jsPDF.
    // Row data arrives via data-* attributes on each ".js-invoice-pdf" button;
    // student identity arrives via window.SIMS_STUDENT (rendered server-side).

    document.addEventListener("click", function (e) {
        var btn = e.target.closest ? e.target.closest(".js-invoice-pdf") : null;
        if (!btn) return;

        var original = btn.innerHTML;
        btn.disabled = true;
        btn.innerHTML = "Generating…";

        try {
            generateInvoicePdf(btn.dataset);
        } catch (err) {
            console.error("Invoice PDF error:", err);
            alert("PDF generation failed. Open the console to see the error.");
        }

        btn.disabled = false;
        btn.innerHTML = original;
    });

    // ----- Method + year filtering of the transactions table -----
    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initPaymentFilters);
    } else {
        initPaymentFilters();
    }

    function initPaymentFilters() {
        var methodSel = document.getElementById("pm-filter-method");
        var yearSel = document.getElementById("pm-filter-year");
        var countEl = document.getElementById("pm-count");
        var rows = document.querySelectorAll(".js-pm-row");
        if ((!methodSel && !yearSel) || !rows.length) return;

        // Build the year dropdown from the years that actually appear in the
        // payments, newest first — so it never lists a year with no records.
        if (yearSel) {
            var years = {};
            for (var r = 0; r < rows.length; r++) {
                var y = rows[r].getAttribute("data-year");
                if (y) years[y] = true;
            }
            Object.keys(years).sort().reverse().forEach(function (y) {
                var opt = document.createElement("option");
                opt.value = y;
                opt.textContent = y;
                yearSel.appendChild(opt);
            });
        }

        function applyFilters() {
            var method = methodSel ? methodSel.value : "all";
            var year = yearSel ? yearSel.value : "all";
            var visible = 0;

            for (var i = 0; i < rows.length; i++) {
                var row = rows[i];
                var methodOk = method === "all" || row.getAttribute("data-method") === method;
                var yearOk = year === "all" || row.getAttribute("data-year") === year;
                var show = methodOk && yearOk;
                row.style.display = show ? "" : "none";
                if (show) visible++;
            }

            if (countEl) countEl.textContent = visible + " of " + rows.length + " shown";
        }

        if (methodSel) methodSel.addEventListener("change", applyFilters);
        if (yearSel) yearSel.addEventListener("change", applyFilters);
        applyFilters();
    }

    function generateInvoicePdf(data) {
        if (!window.jspdf || !window.jspdf.jsPDF) {
            alert("jsPDF is not loaded.");
            return;
        }

        var student = window.SIMS_STUDENT || { name: "", id: "", programme: "" };

        var amount = parseFloat(data.amount) || 0;       // gross total (tax inclusive)
        var subtotal = amount / 1.06;                    // matches the 6% tax used at checkout
        var tax = amount - subtotal;

        var invoiceNo = data.invoice || "INV";
        var status = (data.status || "PAID").toUpperCase();
        var isRefunded = status === "REFUNDED";

        var jsPDF = window.jspdf.jsPDF;
        var doc = new jsPDF({ orientation: "portrait", unit: "mm", format: "a4" });

        var pageWidth = doc.internal.pageSize.getWidth();
        var margin = 18;
        var contentRight = pageWidth - margin;
        var y = margin;

        // ----- Header -----
        doc.setTextColor(224, 22, 43); // INTI red (#e0162b)
        doc.setFont("helvetica", "bold");
        doc.setFontSize(18);
        doc.text("INTI INTERNATIONAL UNIVERSITY", margin, y);

        y += 7;
        doc.setTextColor(100, 116, 139);
        doc.setFont("helvetica", "normal");
        doc.setFontSize(11);
        doc.text("Official Payment Receipt", margin, y);

        // Invoice meta (right aligned)
        doc.setTextColor(15, 23, 42);
        doc.setFont("helvetica", "bold");
        doc.setFontSize(11);
        doc.text("Invoice " + invoiceNo, contentRight, margin, { align: "right" });
        doc.setFont("helvetica", "normal");
        doc.setTextColor(100, 116, 139);
        doc.text("Date: " + (data.date || "—"), contentRight, margin + 6, { align: "right" });

        y += 8;
        doc.setDrawColor(226, 232, 240);
        doc.line(margin, y, contentRight, y);

        // ----- Billed to -----
        y += 10;
        doc.setTextColor(100, 116, 139);
        doc.setFontSize(9);
        doc.text("BILLED TO", margin, y);

        y += 6;
        doc.setTextColor(15, 23, 42);
        doc.setFont("helvetica", "bold");
        doc.setFontSize(12);
        doc.text(student.name || "Student", margin, y);

        doc.setFont("helvetica", "normal");
        doc.setFontSize(10);
        doc.setTextColor(100, 116, 139);
        if (student.id) {
            y += 5;
            doc.text("Student ID: " + student.id, margin, y);
        }
        if (student.programme) {
            y += 5;
            doc.text(student.programme, margin, y);
        }

        // ----- Line items table -----
        y += 12;
        var rowH = 9;
        var colDescX = margin;
        var colTermX = margin + 95;
        var colAmtX = contentRight;

        doc.setFillColor(248, 250, 252);
        doc.rect(margin, y - 6, contentRight - margin, rowH, "F");
        doc.setTextColor(100, 116, 139);
        doc.setFont("helvetica", "bold");
        doc.setFontSize(9);
        doc.text("DESCRIPTION", colDescX + 2, y);
        doc.text("TERM", colTermX, y);
        doc.text("AMOUNT", colAmtX, y, { align: "right" });

        y += rowH;
        doc.setTextColor(15, 23, 42);
        doc.setFont("helvetica", "normal");
        doc.setFontSize(10);
        doc.text(data.desc || "—", colDescX + 2, y);
        doc.setTextColor(100, 116, 139);
        doc.text(data.term || "—", colTermX, y);
        doc.setTextColor(15, 23, 42);
        doc.text("RM " + money(amount), colAmtX, y, { align: "right" });

        // ----- Totals -----
        y += 8;
        doc.setDrawColor(226, 232, 240);
        doc.line(colTermX, y, contentRight, y);

        y += 7;
        totalsLine(doc, colTermX, colAmtX, y, "Subtotal", "RM " + money(subtotal), false);
        y += 6;
        totalsLine(doc, colTermX, colAmtX, y, "Tax (6%)", "RM " + money(tax), false);
        y += 8;
        totalsLine(doc, colTermX, colAmtX, y, "TOTAL", "RM " + money(amount), true);

        // ----- Method + status badge -----
        y += 16;
        doc.setFont("helvetica", "normal");
        doc.setFontSize(10);
        doc.setTextColor(100, 116, 139);
        doc.text("Payment method: " + (data.method || "—"), margin, y);

        // status badge
        var badgeText = isRefunded ? "REFUNDED" : "PAID";
        doc.setFont("helvetica", "bold");
        doc.setFontSize(10);
        var badgeW = doc.getTextWidth(badgeText) + 10;
        var badgeX = contentRight - badgeW;
        if (isRefunded) {
            doc.setFillColor(254, 242, 242);
            doc.setTextColor(190, 18, 60);
        } else {
            doc.setFillColor(236, 253, 245);
            doc.setTextColor(4, 120, 87);
        }
        doc.roundedRect(badgeX, y - 5, badgeW, 8, 2, 2, "F");
        doc.text(badgeText, badgeX + 5, y);

        // ----- Footer -----
        var footerY = doc.internal.pageSize.getHeight() - margin;
        doc.setDrawColor(226, 232, 240);
        doc.line(margin, footerY - 8, contentRight, footerY - 8);
        doc.setFont("helvetica", "normal");
        doc.setFontSize(8);
        doc.setTextColor(148, 163, 184);
        doc.text(
            "This is a computer-generated receipt and does not require a signature. INTI International University.",
            margin,
            footerY
        );

        doc.save(invoiceNo + ".pdf");
    }

    function totalsLine(doc, labelX, valueX, y, label, value, emphasize) {
        doc.setFont("helvetica", emphasize ? "bold" : "normal");
        doc.setFontSize(emphasize ? 12 : 10);
        doc.setTextColor(emphasize ? 15 : 100, emphasize ? 23 : 116, emphasize ? 42 : 139);
        doc.text(label, labelX, y);
        doc.setTextColor(15, 23, 42);
        doc.text(value, valueX, y, { align: "right" });
    }

    function money(n) {
        return n.toLocaleString("en-MY", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }
})();
