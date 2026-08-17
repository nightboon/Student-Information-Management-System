(function () {
    // Category -> badge classes and status-dot colour. The server stamps each
    // item with data-category ("ANNOUNCEMENT" or "SYSTEM"); styling lives here
    // so the list and the detail panel stay in sync from one source.
    var CATS = {
        ANNOUNCEMENT: { badge: 'bg-sky-50 text-sky-700 border-sky-100', dot: '#0ea5e9' },
        GRADE: { badge: 'bg-emerald-50 text-emerald-700 border-emerald-100', dot: '#10b981' },
        EXTENSION: { badge: 'bg-amber-50 text-amber-700 border-amber-100', dot: '#f59e0b' },
        SUBMISSION: { badge: 'bg-violet-50 text-violet-700 border-violet-100', dot: '#8b5cf6' },
        SYSTEM: { badge: 'bg-slate-100 text-slate-700 border-slate-200', dot: '#64748b' }
    };

    var list = document.getElementById('notif-list');
    if (!list) return;
    var endpointBase = list.getAttribute('data-notification-endpoint') || '/student/notifications.aspx';

    var items = Array.prototype.slice.call(list.querySelectorAll('.notif-item'));
    var unreadEl = document.getElementById('unread-count');
    var emptyEl = document.getElementById('detail-empty');
    var cardEl = document.getElementById('detail-card');
    var current = null;
    var yearFilter = document.querySelector('[data-notification-year-filter]');
    var semesterFilter = document.querySelector('[data-notification-semester-filter]');
    var courseFilter = document.querySelector('[data-notification-course-filter]');
    var search = document.getElementById('notif-search');
    var courseOptions = courseFilter ? Array.prototype.slice.call(courseFilter.options, 1).map(function (option) {
        return {
            value: option.value,
            label: option.textContent,
            year: option.getAttribute('data-year') || '',
            semester: option.getAttribute('data-semester') || ''
        };
    }) : [];

    function badgeText(n) {
        return n > 9 ? '9+' : String(n);
    }

    function localUnreadCount() {
        var n = 0;
        for (var i = 0; i < items.length; i++) {
            if (items[i].getAttribute('data-read') !== 'true') n++;
        }
        return n;
    }

    function syncBadges(unreadCount) {
        if (unreadEl) unreadEl.textContent = unreadCount;

        var badges = document.querySelectorAll('[data-notification-count-badge]');
        for (var i = 0; i < badges.length; i++) {
            badges[i].textContent = badgeText(unreadCount);
            if (unreadCount > 0) {
                badges[i].classList.remove('hidden');
            } else {
                badges[i].classList.add('hidden');
            }
        }
    }

    function postJson(url, payload) {
        return fetch(url, {
            method: 'POST',
            credentials: 'same-origin',
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            body: JSON.stringify(payload || {})
        }).then(function (response) {
            if (!response.ok) throw new Error('Notification update failed');
            return response.json();
        }).then(function (json) {
            return json.d || json;
        });
    }

    function applyServerCount(result) {
        if (result && typeof result.unreadCount === 'number') {
            syncBadges(result.unreadCount);
        } else {
            syncBadges(localUnreadCount());
        }
    }

    function cat(item) {
        return CATS[item.getAttribute('data-category')] || CATS.ANNOUNCEMENT;
    }

    // Toggle the detail panel vs. the empty-state. Uses inline display rather
    // than the `hidden` attribute because #detail-empty carries the Tailwind
    // `flex` utility, which would otherwise override `[hidden] { display:none }`.
    function showDetail(hasItem) {
        if (emptyEl) emptyEl.style.display = hasItem ? 'none' : 'flex';
        if (cardEl) cardEl.style.display = hasItem ? '' : 'none';
    }

    function paintDot(item) {
        var dot = item.querySelector('.notif-dot');
        if (!dot) return;
        if (item.getAttribute('data-read') === 'true') {
            dot.style.backgroundColor = 'transparent';
            dot.style.outline = 'none';
        } else {
            var c = cat(item);
            dot.style.backgroundColor = c.dot;
            dot.style.outline = '3px solid ' + c.dot + '22';
        }
    }

    function styleItem(item) {
        var badge = item.querySelector('.notif-badge');
        if (badge) badge.className = 'notif-badge rounded border px-1.5 py-0.5 ' + cat(item).badge;
        var pin = item.querySelector('.notif-pin');
        if (pin) pin.hidden = item.getAttribute('data-pinned') !== 'true';
        item.style.opacity = item.getAttribute('data-read') === 'true' ? '0.7' : '';
        paintDot(item);
    }

    function recount() {
        syncBadges(localUnreadCount());
    }

    function applyReadState(item, read) {
        item.setAttribute('data-read', read ? 'true' : 'false');
        item.style.opacity = read ? '0.7' : '';
        paintDot(item);
        if (item === current) syncReadButton(item);
    }

    function syncReadButton(item) {
        var button = document.getElementById('mark-unread');
        if (!button) return;
        var read = item && item.getAttribute('data-read') === 'true';
        button.textContent = read ? 'Mark unread' : 'Mark read';
        button.disabled = !item;
    }

    function setRead(item, read) {
        var currentRead = item.getAttribute('data-read') === 'true';
        if (currentRead === read) {
            recount();
            return;
        }

        applyReadState(item, read);
        recount();

        var id = parseInt(item.getAttribute('data-id'), 10);
        var type = item.getAttribute('data-type') || 'ANNOUNCEMENT';
        var endpoint = endpointBase + (read ? '/MarkRead' : '/MarkUnread');

        postJson(endpoint, { notificationType: type, notificationId: id }).then(function (result) {
            applyServerCount(result);
        }).catch(function () {
            applyReadState(item, currentRead);
            recount();
        });
    }

    function select(item) {
        for (var i = 0; i < items.length; i++) items[i].style.backgroundColor = '';
        item.style.backgroundColor = 'rgba(224,22,43,0.04)';
        current = item;

        var c = cat(item);
        var badge = document.getElementById('detail-badge');
        if (badge) {
            badge.textContent = item.getAttribute('data-category');
            badge.className = 'rounded border px-1.5 py-0.5 ' + c.badge;
        }
        setText('detail-course', item.getAttribute('data-course'));
        setText('detail-title', item.getAttribute('data-title'));
        setText('detail-time', item.getAttribute('data-fulltime'));
        setText('detail-author', item.getAttribute('data-author'));

        var body = item.querySelector('.notif-content');
        var detailContent = document.getElementById('detail-content');
        if (detailContent) detailContent.textContent = body ? body.textContent : '';

        var fileUrl = item.getAttribute('data-fileurl') || '';
        var attachEl = document.getElementById('detail-attachment');
        if (attachEl) {
            // attachEl carries the `inline-flex` utility class, which (like
            // #detail-empty/#detail-card above) beats the `[hidden]` attribute
            // in Tailwind's CDN stylesheet order — so toggle `display` directly.
            if (fileUrl) {
                attachEl.href = fileUrl;
                attachEl.hidden = false;
                attachEl.style.display = '';
            } else {
                attachEl.hidden = true;
                attachEl.style.display = 'none';
            }
        }

        var pin = document.getElementById('detail-pin');
        if (pin) pin.hidden = item.getAttribute('data-pinned') !== 'true';

        showDetail(true);

        setRead(item, true);
        if (window.lucide) window.lucide.createIcons();
    }

    function setText(id, val) {
        var el = document.getElementById(id);
        if (el) el.textContent = val || '';
    }

    function applyFilters() {
        var year = yearFilter ? yearFilter.value : 'all';
        var semester = semesterFilter ? semesterFilter.value : 'all';
        var course = courseFilter ? courseFilter.value : 'all';
        var q = search ? search.value.toLowerCase().trim() : '';

        items.forEach(function (item) {
            var hay = (item.getAttribute('data-title') + ' ' + item.getAttribute('data-course') + ' ' + item.getAttribute('data-author')).toLowerCase();
            var visible =
                (year === 'all' || item.getAttribute('data-year') === year) &&
                (semester === 'all' || item.getAttribute('data-semester') === semester) &&
                (course === 'all' || item.getAttribute('data-offering') === course) &&
                (!q || hay.indexOf(q) !== -1);
            item.parentElement.hidden = !visible;
        });
    }

    function refreshCourseOptions() {
        if (!courseFilter) return;
        var selected = courseFilter.value;
        var year = yearFilter ? yearFilter.value : 'all';
        var semester = semesterFilter ? semesterFilter.value : 'all';
        courseFilter.innerHTML = '<option value="all">All courses</option>';
        courseOptions.forEach(function (course) {
            if (year !== 'all' && course.year !== year) return;
            if (semester !== 'all' && course.semester !== semester) return;
            var option = document.createElement('option');
            option.value = course.value;
            option.textContent = course.label;
            courseFilter.appendChild(option);
        });
        if (Array.prototype.some.call(courseFilter.options, function (option) { return option.value === selected; })) {
            courseFilter.value = selected;
        }
    }

    items.forEach(function (item) {
        styleItem(item);
        item.addEventListener('click', function () { select(item); });
    });

    var markAll = document.getElementById('mark-all-read');
    if (markAll) markAll.addEventListener('click', function () {
        var previous = items.map(function (item) {
            return item.getAttribute('data-read') === 'true';
        });

        items.forEach(function (item) { applyReadState(item, true); });
        syncBadges(0);

        postJson(endpointBase + '/MarkAllRead', {}).then(function (result) {
            applyServerCount(result);
        }).catch(function () {
            items.forEach(function (item, index) {
                applyReadState(item, previous[index]);
            });
            recount();
        });
    });

    var markUnread = document.getElementById('mark-unread');
    if (markUnread) markUnread.addEventListener('click', function () {
        if (!current) return;
        setRead(current, current.getAttribute('data-read') !== 'true');
    });

    if (search) search.addEventListener('input', function () {
        applyFilters();
    });
    [yearFilter, semesterFilter].forEach(function (filter) {
        if (!filter) return;
        filter.addEventListener('change', function () {
            refreshCourseOptions();
            applyFilters();
        });
    });
    if (courseFilter) courseFilter.addEventListener('change', applyFilters);
    refreshCourseOptions();
    applyFilters();

    recount();

    var params = new URLSearchParams(window.location.search);
    var targetId = params.get('id');
    var targetType = params.get('type');
    var initialItem = null;
    if (targetId) {
        for (var i = 0; i < items.length; i++) {
            var itemType = items[i].getAttribute('data-type') || 'ANNOUNCEMENT';
            if (items[i].getAttribute('data-id') === targetId && (!targetType || itemType === targetType)) {
                initialItem = items[i];
                break;
            }
        }
    }
    if (!initialItem && items.length) initialItem = items[0];

    if (initialItem) {
        select(initialItem);
        initialItem.scrollIntoView({ block: 'nearest' });
    } else {
        showDetail(false);
    }
})();
