(function () {
    function init() {
        if (window.lucide) {
            lucide.createIcons();
        }
        var path = location.pathname.toLowerCase();
        var links = document.querySelectorAll('[data-nav-link]');
        for (var i = 0; i < links.length; i++) {
            var target = links[i].getAttribute('data-nav-link').toLowerCase();
            if (path.endsWith(target)) {
                links[i].setAttribute('data-active', 'true');
            }
        }

        // Mobile menu toggle
        var menu = document.getElementById('mobile-menu');
        var backdrop = document.getElementById('mobile-menu-backdrop');
        var menuButton = document.querySelector('[data-action="toggle-menu"]');

        function setMenuOpen(open) {
            if (menu) menu.setAttribute('data-open', open ? 'true' : 'false');
            if (backdrop) backdrop.setAttribute('data-open', open ? 'true' : 'false');
            if (menuButton) menuButton.setAttribute('aria-expanded', open ? 'true' : 'false');
        }

        if (menuButton) {
            menuButton.addEventListener('click', function () {
                var current = menu && menu.getAttribute('data-open') === 'true';
                setMenuOpen(!current);
            });
        }
        var closeBtns = document.querySelectorAll('[data-action="close-menu"]');
        for (var c = 0; c < closeBtns.length; c++) {
            closeBtns[c].addEventListener('click', function () { setMenuOpen(false); });
        }
        if (backdrop) {
            backdrop.addEventListener('click', function () { setMenuOpen(false); });
        }
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') setMenuOpen(false);
        });

        var logoutBtns = document.querySelectorAll('[data-action="logout"]');
        for (var l = 0; l < logoutBtns.length; l++) {
            logoutBtns[l].addEventListener('click', function (e) {
                e.preventDefault();
                fetch('/login/login.aspx/Logout', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    body: '{}'
                }).then(function () {
                    window.location.href = '/login/login.aspx';
                }).catch(function () {
                    window.location.href = '/login/login.aspx';
                });
            });
        }
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
