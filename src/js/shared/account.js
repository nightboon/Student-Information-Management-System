(function () {
    // Profile save
    var profileSaveBtn = document.getElementById('profile-save-btn');
    var profileSaved = document.getElementById('profile-saved');
    var profilePhone = document.getElementById('phone');
    var profileAddress = document.getElementById('address');
    var profileTimer = null;
    if (profileSaveBtn && profileSaved && profilePhone && profileAddress) {
        function setProfileMessage(ok, text) {
            profileSaved.className = 'inline-flex items-center gap-1 ' + (ok ? 'text-emerald-600' : 'text-[#a01020]');
            profileSaved.innerHTML = '<i data-lucide="' + (ok ? 'check' : 'alert-circle') + '" class="h-4 w-4"></i> <span></span>';
            profileSaved.querySelector('span').textContent = text;
            if (window.lucide) lucide.createIcons();

            if (profileTimer) clearTimeout(profileTimer);
            if (ok) {
                profileTimer = setTimeout(function () {
                    profileSaved.classList.add('hidden');
                    profileSaved.classList.remove('inline-flex');
                }, 2200);
            }
        }

        profileSaveBtn.addEventListener('click', function () {
            profileSaveBtn.disabled = true;
            var originalLabel = profileSaveBtn.textContent;
            profileSaveBtn.textContent = 'Saving...';

            fetch(window.location.pathname + '/SaveProfile', {
                method: 'POST',
                credentials: 'same-origin',
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                body: JSON.stringify({
                    phone: profilePhone.value,
                    mailingAddress: profileAddress.value
                })
            }).then(function (response) {
                if (!response.ok) throw new Error('Request failed');
                return response.json();
            }).then(function (json) {
                var result = json.d || json;
                if (result && result.ok) {
                    setProfileMessage(true, result.message || 'Profile saved.');
                } else {
                    setProfileMessage(false, (result && result.message) || 'Profile could not be saved.');
                }
            }).catch(function () {
                setProfileMessage(false, 'Profile could not be saved. Please try again.');
            }).then(function () {
                profileSaveBtn.disabled = false;
                profileSaveBtn.textContent = originalLabel;
            });
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

    // Password change. The form posts to the ChangePassword page method on the
    // current account page (student/lecturer/admin all expose the same method),
    // which verifies the current password and writes the new hash to the DB.
    var pwSubmitBtn = document.getElementById('pw-submit-btn');
    var pwMsg = document.getElementById('pw-msg');
    var pwMsgText = document.getElementById('pw-msg-text');
    if (pwSubmitBtn && pwMsg && pwMsgText) {
        var pwCurrent = document.getElementById('pw-current');
        var pwNew = document.getElementById('pw-new');
        var pwConfirm = document.getElementById('pw-confirm');

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

        // Mirror of AccountPasswordService.ValidateStrength so the user gets
        // instant feedback; the server re-checks regardless. Returns an error
        // string, or null when the password is acceptable.
        function strengthError(pw) {
            if (!pw || pw.length < 8) return 'New password must be at least 8 characters.';
            if (!/[A-Z]/.test(pw)) return 'New password must include an uppercase letter.';
            if (!/[a-z]/.test(pw)) return 'New password must include a lowercase letter.';
            if (!/[0-9]/.test(pw)) return 'New password must include a number.';
            if (!/[^A-Za-z0-9]/.test(pw)) return 'New password must include a symbol.';
            return null;
        }

        pwSubmitBtn.addEventListener('click', function () {
            var current = pwCurrent.value;
            var next = pwNew.value;
            var confirm = pwConfirm.value;

            if (!current || !next || !confirm) return setError('Please fill in all password fields.');
            var strength = strengthError(next);
            if (strength) return setError(strength);
            if (next !== confirm) return setError('New passwords do not match.');
            if (next === current) return setError('New password must differ from the current one.');

            pwSubmitBtn.disabled = true;
            var originalLabel = pwSubmitBtn.textContent;
            pwSubmitBtn.textContent = 'Updating...';

            fetch(window.location.pathname + '/ChangePassword', {
                method: 'POST',
                credentials: 'same-origin',
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                body: JSON.stringify({ currentPassword: current, newPassword: next })
            }).then(function (response) {
                if (!response.ok) throw new Error('Request failed');
                return response.json();
            }).then(function (json) {
                var result = json.d || json;
                if (result && result.ok) {
                    setOk(result.message || 'Password updated successfully.');
                    pwCurrent.value = '';
                    pwNew.value = '';
                    pwConfirm.value = '';
                } else {
                    setError((result && result.message) || 'Could not update password.');
                }
            }).catch(function () {
                setError('Could not update password. Please try again.');
            }).then(function () {
                pwSubmitBtn.disabled = false;
                pwSubmitBtn.textContent = originalLabel;
            });
        });
    }

    // Profile icon upload
    var changeIconBtn = document.getElementById('change-icon-btn');
    var iconFileInput = document.getElementById('icon-file-input');
    var avatarTrigger = document.getElementById('avatar-trigger');
    if (avatarTrigger && iconFileInput) {
        var uploadUrl = avatarTrigger.getAttribute('data-upload-url');
        var uploading = false;

        // The whole avatar (#avatar-trigger) opens the file picker. The camera
        // button lives inside it, so its clicks bubble up here too.
        avatarTrigger.addEventListener('click', function (e) {
            // iconFileInput.click() re-fires a bubbling click; ignore it so we
            // don't recurse, and ignore clicks while an upload is in flight.
            if (e.target === iconFileInput) return;
            if (uploading) return;
            iconFileInput.click();
        });

        iconFileInput.addEventListener('change', function () {
            var file = iconFileInput.files[0];
            if (!file) return;

            var form = new FormData();
            form.append('icon', file);

            uploading = true;
            var origInner = changeIconBtn ? changeIconBtn.innerHTML : '';
            if (changeIconBtn) {
                changeIconBtn.disabled = true;
                changeIconBtn.innerHTML = '<svg class="animate-spin h-3.5 w-3.5" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z"></path></svg>';
            }

            fetch(uploadUrl, {
                method: 'POST',
                credentials: 'same-origin',
                body: form
            }).then(function (r) {
                return r.json();
            }).then(function (data) {
                if (data && data.success) {
                    var newSrc = data.iconUrl + '?t=' + Date.now();
                    var img = document.getElementById('profile-icon-img');
                    var initials = document.getElementById('profile-icon-initials');
                    if (!img) {
                        img = document.createElement('img');
                        img.id = 'profile-icon-img';
                        img.alt = 'Profile photo';
                        img.className = 'h-20 w-20 rounded-full object-cover';
                        var anchor = initials || changeIconBtn;
                        anchor.parentNode.insertBefore(img, anchor);
                    }
                    img.src = newSrc;
                    img.style.display = '';
                    if (initials) initials.style.display = 'none';
                    var topbarImg = document.querySelector('header img[alt="Profile picture"]');
                    if (topbarImg) topbarImg.src = newSrc;
                } else {
                    alert((data && data.message) || 'Photo could not be saved.');
                }
            }).catch(function () {
                alert('Photo could not be saved. Please try again.');
            }).then(function () {
                uploading = false;
                if (changeIconBtn) {
                    changeIconBtn.disabled = false;
                    changeIconBtn.innerHTML = origInner;
                }
                if (window.lucide) lucide.createIcons();
                iconFileInput.value = '';
            });
        });
    }
})();
