(function () {
    var fab = document.getElementById('assistant-fab');
    var panel = document.getElementById('assistant-panel');
    var closeBtn = document.getElementById('assistant-close');
    var messagesEl = document.getElementById('assistant-messages');
    var input = document.getElementById('assistant-input');
    var sendBtn = document.getElementById('assistant-send');

    if (!fab || !panel) return;

    // The chat endpoint is provided by the host control (data-endpoint), so the
    // same script works on any page or role without a hard-coded URL.
    var endpoint = panel.getAttribute('data-endpoint');
    if (!endpoint) return;

    var busy = false;

    // Chat state survives same-tab page navigation via sessionStorage, but a
    // manual refresh (or closing the tab) starts a fresh conversation.
    var STORAGE_KEY = 'sims-assistant-chat';

    function newConversationId() {
        return (window.crypto && crypto.randomUUID)
            ? crypto.randomUUID()
            : 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0;
                return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
            });
    }

    // True only for an explicit page refresh, so we can clear the chat on F5
    // but keep it when navigating between pages.
    function isReload() {
        try {
            var nav = performance.getEntriesByType && performance.getEntriesByType('navigation')[0];
            if (nav && nav.type) return nav.type === 'reload';
            // Deprecated fallback for older browsers (1 === TYPE_RELOAD).
            return !!(performance.navigation && performance.navigation.type === 1);
        } catch (e) {
            return false;
        }
    }

    // Conversation history sent to the server on every turn, plus its log id.
    var history = [];
    var conversationId;

    if (isReload()) {
        sessionStorage.removeItem(STORAGE_KEY);
    }

    var saved = null;
    try { saved = JSON.parse(sessionStorage.getItem(STORAGE_KEY)); } catch (e) { saved = null; }

    if (saved && saved.conversationId && saved.history) {
        conversationId = saved.conversationId;
        history = saved.history;
    } else {
        conversationId = newConversationId();
    }

    // Save the current conversation so it can be restored on the next page.
    function persist() {
        try {
            sessionStorage.setItem(STORAGE_KEY, JSON.stringify({
                conversationId: conversationId,
                history: history
            }));
        } catch (e) { /* storage full/unavailable: degrade silently */ }
    }

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

    // Render an assistant reply as Markdown. Falls back to plain text if the
    // marked CDN failed to load.
    function renderReply(bubble, markdown) {
        if (typeof marked !== 'undefined') {
            bubble.classList.add('assistant-md');
            bubble.style.whiteSpace = 'normal';
            bubble.innerHTML = marked.parse(markdown);
        } else {
            bubble.textContent = markdown;
        }
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

        fetch(endpoint, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            body: JSON.stringify({ history: history, message: text, conversationId: conversationId })
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
                renderReply(pending, reply);
                history.push({ role: 'user', content: text });
                history.push({ role: 'assistant', content: reply });
                persist();
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

    // Re-render any conversation restored from a previous page in this tab.
    for (var i = 0; i < history.length; i++) {
        var turn = history[i];
        if (turn.role === 'user') {
            addBubble(turn.content, 'user');
        } else if (turn.role === 'assistant') {
            renderReply(addBubble('', 'ai'), turn.content);
        }
    }
    if (history.length) {
        messagesEl.scrollTop = messagesEl.scrollHeight;
    }
})();
