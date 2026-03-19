var AuthController = {

    // ── Login 
    login: function () {
        var email = $.trim($('#liEmail').val());
        var pass = $.trim($('#liPass').val());
        var role = $('#liRole').val();

        if (!email || !pass) {
            AuthController._showErr('#loginErr', 'Please fill in all fields.');
            return;
        }

        var user = null;
        $.each(USERS, function (i, u) {
            if (u.email === email && u.pass === pass) {
                user = u;
                return false; // break $.each
            }
        });

        if (!user) {
            AuthController._showErr('#loginErr', 'Incorrect email or password.');
            return;
        }

        Session.save(user, role);
        STATE.currentUser = user;
        STATE.currentRole = role;

        window.location.href = '/AMS/DashBoard/Index';
    },

    // ── Register
    register: function () {
        var fn = $.trim($('#regFn').val());
        var ln = $.trim($('#regLn').val());
        var email = $.trim($('#regEmail').val());
        var pass = $('#regPass').val();
        var pass2 = $('#regPass2').val();
        var role = $('#regRole').val();
        var section = $('#regSec').val();

        if (!fn || !ln || !email || !pass || !pass2) {
            AuthController._showErr('#regErr', 'Please fill in all fields.');
            return;
        }
        if (!Helpers.isValidEmail(email)) {
            AuthController._showErr('#regErr', 'Please enter a valid email.');
            return;
        }
        if (pass.length < 6) {
            AuthController._showErr('#regErr', 'Password must be at least 6 characters.');
            return;
        }
        if (pass !== pass2) {
            AuthController._showErr('#regErr', 'Passwords do not match.');
            return;
        }

        var exists = false;
        $.each(USERS, function (i, u) {
            if (u.email === email) { exists = true; return false; }
        });

        if (exists) {
            AuthController._showErr('#regErr', 'Email already registered.');
            return;
        }

        var newUser = new UserModel({
            name: fn + ' ' + ln,
            email: email,
            pass: pass,
            role: role,
            section: section
        });
        USERS.push(newUser);

        Session.save(newUser, role);
        STATE.currentUser = newUser;
        STATE.currentRole = role;

        window.location.href = '/AMS/DashBoard/Index';
    },

    // ── Logout 
    logout: function () {
        Session.clear();
        STATE.currentUser = null;
        STATE.attendance = {};
        window.location.href = '/AMS/Auth/Login';
    },

    // ── Auth guard: redirect if not logged in
    requireAuth: function () {
        if (!Session.isLoggedIn()) {
            window.location.href = '/AMS/Auth/Login';
            return false;
        }
        return true;
    },

    // ── Private: show error message 
    _showErr: function (selector, msg) {
        $(selector).text(msg).addClass('show');
        setTimeout(function () {
            $(selector).removeClass('show');
        }, 3000);
    }
};
