(function () {
    var fab = document.getElementById('assistant-fab');
    var panel = document.getElementById('assistant-panel');
    var closeBtn = document.getElementById('assistant-close');
    var messagesEl = document.getElementById('assistant-messages');
    var input = document.getElementById('assistant-input');
    var sendBtn = document.getElementById('assistant-send');

    if (!fab || !panel) return;

    // Conversation history sent to the server on every turn.
    var history = [];
    var busy = false;

    function openPanel() {
        panel.classList.remove('hidden');
        panel.classList.add('flex');
        input.focus();
    }

    function closePanel() {
        panel.classList.add('hidden');
        panel.classList.remove('flex');
    }

    fab.addEventListener('click', function () {
        if (panel.classList.contains('hidden')) {
            openPanel();
        } else {
            closePanel();
        }
    });
    closeBtn.addEventListener('click', closePanel);

    // Append a chat bubble and return the element so it can be updated later.
    function addBubble(text, who) {
        var wrap = document.createElement('div');
        wrap.className = who === 'user' ? 'flex justify-end' : 'flex justify-start';

        var bubble = document.createElement('div');
        bubble.className = who === 'user'
            ? 'max-w-[80%] rounded-2xl bg-[#e0162b] px-3 py-2 text-white'
            : 'max-w-[80%] rounded-2xl bg-white px-3 py-2 text-slate-800 border border-slate-200';
        bubble.style.whiteSpace = 'pre-wrap';
        bubble.textContent = text;

        wrap.appendChild(bubble);
        messagesEl.appendChild(wrap);
        messagesEl.scrollTop = messagesEl.scrollHeight;
        return bubble;
    }

    function setBusy(value) {
        busy = value;
        sendBtn.disabled = value;
        input.disabled = value;
    }

    function send() {
        if (busy) return;
        var text = input.value.trim();
        if (!text) return;

        addBubble(text, 'user');
        input.value = '';
        setBusy(true);

        var pending = addBubble('Thinking…', 'ai');

        fetch('/student/student_dashboard.aspx/Chat', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            body: JSON.stringify({ history: history, message: text })
        })
            .then(function (resp) {
                if (!resp.ok) {
                    throw new Error('HTTP ' + resp.status);
                }
                return resp.json();
            })
            .then(function (data) {
                // ASP.NET page methods wrap the payload in { "d": ... }.
                var result = (data && typeof data.d !== 'undefined') ? data.d : data;
                var reply = (result && result.reply) ? result.reply : '(no reply)';
                pending.textContent = reply;
                history.push({ role: 'user', content: text });
                history.push({ role: 'assistant', content: reply });
                messagesEl.scrollTop = messagesEl.scrollHeight;
            })
            .catch(function () {
                pending.textContent = 'Something went wrong. Please try again.';
            })
            .then(function () {
                setBusy(false);
                input.focus();
            });
    }

    sendBtn.addEventListener('click', send);
    input.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            send();
        }
    });
})();
