(function () {
    // Profile save: show "Profile saved" for 2.2s
    var profileSaveBtn = document.getElementById('profile-save-btn');
    var profileSaved = document.getElementById('profile-saved');
    if (profileSaveBtn && profileSaved) {
        profileSaveBtn.addEventListener('click', function () {
            profileSaved.classList.remove('hidden');
            profileSaved.classList.add('inline-flex');
            setTimeout(function () {
                profileSaved.classList.add('hidden');
                profileSaved.classList.remove('inline-flex');
            }, 2200);
        });
    }

    // Password show/hide toggles
    var toggles = document.querySelectorAll('[data-toggle-pw]');
    for (var i = 0; i < toggles.length; i++) {
        (function (btn) {
            btn.addEventListener('click', function () {
                var targetId = btn.getAttribute('data-toggle-pw');
                var input = document.getElementById(targetId);
                if (!input) return;
                var icon = btn.querySelector('[data-lucide]');
                if (input.type === 'password') {
                    input.type = 'text';
                    if (icon) icon.setAttribute('data-lucide', 'eye-off');
                } else {
                    input.type = 'password';
                    if (icon) icon.setAttribute('data-lucide', 'eye');
                }
                if (window.lucide) lucide.createIcons();
            });
        })(toggles[i]);
    }

    // Password form validation
    var pwSubmitBtn = document.getElementById('pw-submit-btn');
    var pwMsg = document.getElementById('pw-msg');
    var pwMsgText = document.getElementById('pw-msg-text');
    if (pwSubmitBtn && pwMsg && pwMsgText) {
        pwSubmitBtn.addEventListener('click', function () {
            var current = document.getElementById('pw-current').value;
            var next = document.getElementById('pw-new').value;
            var confirm = document.getElementById('pw-confirm').value;

            function setError(text) {
                pwMsg.className = 'inline-flex items-start gap-2 rounded-md border px-3 py-2 bg-[#e0162b]/10 border-[#e0162b]/20 text-[#a01020]';
                pwMsg.style.fontSize = '12.5px';
                pwMsgText.textContent = text;
            }
            function setOk(text) {
                pwMsg.className = 'inline-flex items-start gap-2 rounded-md border px-3 py-2 bg-emerald-50 border-emerald-100 text-emerald-700';
                pwMsg.style.fontSize = '12.5px';
                pwMsgText.textContent = text;
            }

            if (!current || !next || !confirm) return setError('Please fill in all password fields.');
            if (next.length < 8) return setError('New password must be at least 8 characters.');
            if (next !== confirm) return setError('New passwords do not match.');
            if (next === current) return setError('New password must differ from the current one.');

            setOk('Password updated successfully.');
            document.getElementById('pw-current').value = '';
            document.getElementById('pw-new').value = '';
            document.getElementById('pw-confirm').value = '';
        });
    }

    // Email notifications toggle
    var notifBtn = document.getElementById('email-notif-toggle');
    var notifKnob = document.getElementById('email-notif-knob');
    if (notifBtn && notifKnob) {
        notifBtn.addEventListener('click', function () {
            var on = notifBtn.getAttribute('data-on') === 'true';
            var next = !on;
            notifBtn.setAttribute('data-on', String(next));
            notifBtn.setAttribute('aria-pressed', String(next));
            if (next) {
                notifBtn.className = 'relative inline-flex h-6 w-10 shrink-0 items-center rounded-full transition-colors bg-[#e0162b]';
                notifKnob.className = 'inline-block h-5 w-5 rounded-full bg-white shadow transition-transform translate-x-[18px]';
            } else {
                notifBtn.className = 'relative inline-flex h-6 w-10 shrink-0 items-center rounded-full transition-colors bg-slate-200';
                notifKnob.className = 'inline-block h-5 w-5 rounded-full bg-white shadow transition-transform translate-x-0.5';
            }
        });
    }
})();
