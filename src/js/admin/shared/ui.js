(function () {
  function lockBody(lock) { document.body.style.overflow = lock ? "hidden" : ""; }

  // ---- Modal ----
  function openModal(id) {
    var m = document.getElementById(id);
    if (!m) return;
    m.style.display = "flex"; lockBody(true);
    if (window.renderIcons) window.renderIcons();
  }
  function closeModal(m) { if (m) { m.style.display = "none"; lockBody(false); } }

  // ---- Drawer ----
  function openDrawer(id) {
    var d = document.getElementById(id);
    if (!d) return;
    d.style.display = "block";
    var panel = d.querySelector("[data-drawer-panel]");
    requestAnimationFrame(function () { if (panel) panel.style.transform = "translateX(0)"; });
    lockBody(true);
    if (window.renderIcons) window.renderIcons();
  }
  function closeDrawer(d) {
    if (!d) return;
    var panel = d.querySelector("[data-drawer-panel]");
    if (panel) panel.style.transform = "translateX(100%)";
    setTimeout(function () { d.style.display = "none"; }, 220);
    lockBody(false);
  }

  // ---- Tabs ----
  function activateTab(group, key) {
    if (!group) return;
    group.querySelectorAll("[data-tab]").forEach(function (b) {
      b.setAttribute("data-active", b.getAttribute("data-tab") === key ? "true" : "false");
    });
    group.querySelectorAll("[data-tab-panel]").forEach(function (p) {
      p.style.display = p.getAttribute("data-tab-panel") === key ? "" : "none";
    });
    if (window.renderIcons) window.renderIcons();
  }

  // ---- Dropdown ----
  function closeAllDropdowns() {
    document.querySelectorAll("[data-dropdown-menu]").forEach(function (m) { m.style.display = "none"; });
  }

  // ---- Confirm (shared dialog built once) ----
  function ensureConfirm() {
    var c = document.getElementById("__confirm");
    if (c) return c;
    c = document.createElement("div");
    c.id = "__confirm";
    c.style.cssText = "position:fixed;inset:0;z-index:70;display:none;align-items:center;justify-content:center;padding:16px";
    c.innerHTML =
      '<div data-confirm-backdrop style="position:absolute;inset:0;background:rgba(15,23,42,.4);backdrop-filter:blur(2px)"></div>' +
      '<div style="position:relative;width:100%;max-width:28rem;background:#fff;border:1px solid #e2e8f0;border-radius:16px;box-shadow:0 20px 40px -12px rgba(15,23,42,.45);overflow:hidden">' +
        '<div style="padding:16px 24px;border-bottom:1px solid #f1f5f9"><h2 id="__confirm-title" style="font-size:17px;font-weight:700;color:#0f172a;letter-spacing:-0.01em"></h2></div>' +
        '<div style="padding:20px 24px"><p id="__confirm-msg" style="font-size:13.5px;line-height:1.6;color:#475569"></p></div>' +
        '<div style="display:flex;justify-content:flex-end;gap:8px;padding:16px 24px;border-top:1px solid #f1f5f9;background:rgba(248,250,252,.4)">' +
          '<button type="button" id="__confirm-cancel" style="height:40px;padding:0 16px;border-radius:6px;font-size:13px;font-weight:600;color:#475569;background:transparent;border:0;cursor:pointer">Cancel</button>' +
          '<button type="button" id="__confirm-ok" style="height:40px;padding:0 16px;border-radius:6px;font-size:13px;font-weight:600;color:#fff;background:#0f172a;border:0;cursor:pointer"></button>' +
        '</div>' +
      '</div>';
    document.body.appendChild(c);
    c.querySelector("[data-confirm-backdrop]").addEventListener("click", function () { c.style.display = "none"; lockBody(false); });
    c.querySelector("#__confirm-cancel").addEventListener("click", function () { c.style.display = "none"; lockBody(false); });
    return c;
  }
  function openConfirm(opts) {
    var c = ensureConfirm();
    c.querySelector("#__confirm-title").textContent = opts.title || "Are you sure?";
    c.querySelector("#__confirm-msg").textContent = opts.message || "";
    var ok = c.querySelector("#__confirm-ok");
    ok.textContent = opts.label || "Confirm";
    ok.style.background = opts.danger ? "#e0162b" : "#0f172a";
    ok.onclick = function () {
      c.style.display = "none"; lockBody(false);
      if (opts.toast) window.toast.success(opts.toast);
    };
    c.style.display = "flex"; lockBody(true);
  }

  // ---- Global delegation ----
  document.addEventListener("click", function (e) {
    var o;
    if ((o = e.target.closest("[data-modal-open]"))) { openModal(o.getAttribute("data-modal-open")); return; }
    if ((o = e.target.closest("[data-modal-close]"))) { closeModal(o.closest("[data-modal]")); return; }
    if (e.target.hasAttribute && e.target.hasAttribute("data-modal-backdrop")) { closeModal(e.target.closest("[data-modal]")); return; }

    if ((o = e.target.closest("[data-drawer-open]"))) { openDrawer(o.getAttribute("data-drawer-open")); return; }
    if ((o = e.target.closest("[data-drawer-close]"))) { closeDrawer(o.closest("[data-drawer]")); return; }
    if (e.target.hasAttribute && e.target.hasAttribute("data-drawer-backdrop")) { closeDrawer(e.target.closest("[data-drawer]")); return; }

    if ((o = e.target.closest("[data-tab]"))) { activateTab(o.closest("[data-tabs]"), o.getAttribute("data-tab")); return; }

    if ((o = e.target.closest("[data-dropdown-toggle]"))) {
      var menu = o.closest("[data-dropdown]").querySelector("[data-dropdown-menu]");
      var isOpen = menu.style.display === "block";
      closeAllDropdowns();
      menu.style.display = isOpen ? "none" : "block";
      e.stopPropagation();
      return;
    }
    // Clicks outside any open menu close them; clicks inside a menu keep it open.
    if (!e.target.closest("[data-dropdown-menu]")) closeAllDropdowns();

    if ((o = e.target.closest("[data-confirm]"))) {
      openConfirm({
        title: o.getAttribute("data-confirm-title"),
        message: o.getAttribute("data-confirm-message"),
        label: o.getAttribute("data-confirm-label"),
        danger: o.getAttribute("data-confirm-danger") === "true",
        toast: o.getAttribute("data-confirm-toast")
      });
      return;
    }
    if ((o = e.target.closest("[data-toast]"))) {
      var toastType = o.getAttribute("data-toast-type") || "success";
      var toastMethod = window.toast[toastType] || window.toast.info;
      toastMethod(o.getAttribute("data-toast"), o.getAttribute("data-toast-desc") || undefined);
      closeAllDropdowns();
      return;
    }
  });

  document.addEventListener("keydown", function (e) {
    if (e.key !== "Escape") return;
    document.querySelectorAll("[data-modal]").forEach(function (m) { if (m.style.display !== "none") closeModal(m); });
    document.querySelectorAll("[data-drawer]").forEach(function (d) { if (d.style.display !== "none") closeDrawer(d); });
    var c = document.getElementById("__confirm"); if (c && c.style.display !== "none") { c.style.display = "none"; lockBody(false); }
    closeAllDropdowns();
  });

  // Init: first tab active in each group; drawer panels start off-screen
  document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll("[data-tabs]").forEach(function (g) {
      var first = g.querySelector("[data-tab]");
      if (first) activateTab(g, first.getAttribute("data-tab"));
    });
    document.querySelectorAll("[data-drawer] [data-drawer-panel]").forEach(function (p) {
      p.style.transition = "transform .22s ease"; p.style.transform = "translateX(100%)";
    });
  });
})();
