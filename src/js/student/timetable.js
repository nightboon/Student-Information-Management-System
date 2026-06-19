document.addEventListener('DOMContentLoaded', function () {
    const calendarEl = document.getElementById('studentTimetable');

    if (!calendarEl || typeof FullCalendar === 'undefined') {
        return;
    }

    const timetableData = window.studentTimetableData || {};
    const events = Array.isArray(timetableData.events) ? timetableData.events : [];

    function escapeHtml(value) {
        return String(value || '')
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }

    function formatTimeRange(event) {
        const options = { hour: 'numeric', minute: '2-digit', hour12: true };
        return `${event.start.toLocaleTimeString([], options)} - ${event.end.toLocaleTimeString([], options)}`;
    }

    const calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'timeGridWeek',
        initialDate: timetableData.initialDate || new Date().toISOString().slice(0, 10),
        firstDay: 1,
        weekends: timetableData.weekends === true,
        allDaySlot: false,
        expandRows: true,
        height: 'auto',
        slotMinTime: timetableData.slotMinTime || '08:00:00',
        slotMaxTime: timetableData.slotMaxTime || '18:00:00',
        slotDuration: '01:00:00',
        slotLabelInterval: '01:00:00',
        nowIndicator: false,
        headerToolbar: false,
        dayHeaderFormat: { weekday: 'long' },
        slotLabelFormat: { hour: 'numeric', minute: '2-digit', hour12: true },
        eventTimeFormat: { hour: 'numeric', minute: '2-digit', hour12: true },
        events: events,
        eventDidMount: function (info) {
            const color = info.event.extendedProps.color || '#e0162b';
            info.el.style.boxShadow = `inset 3px 0 0 ${color}`;
        },
        eventContent: function (arg) {
            const props = arg.event.extendedProps;
            const color = props.color || '#e0162b';
            const timeText = formatTimeRange(arg.event);

            return {
                html: `
                            <div class="tt-event">
                                <div class="tt-badges">
                                    <span class="tt-code" style="background:${escapeHtml(color)}">${escapeHtml(props.code)}</span>
                                    <span class="tt-type">${escapeHtml(props.type || 'Class')}</span>
                                </div>
                                <div class="tt-title">${escapeHtml(arg.event.title)}</div>
                                <div class="tt-meta">Lecturer: ${escapeHtml(props.lecturer || 'Not assigned')}</div>
                                <div class="tt-meta">Room: ${escapeHtml(props.room)}</div>
                                <div class="tt-time">${escapeHtml(timeText)}</div>
                            </div>`
            };
        }
    });

    calendar.render();
});
