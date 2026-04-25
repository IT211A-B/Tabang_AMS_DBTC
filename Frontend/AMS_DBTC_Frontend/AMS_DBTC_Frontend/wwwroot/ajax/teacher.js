var UserController = {

    _data: [],

    //Load all users
    load: function () {
        $('#userGrid').html('<p style="color:var(--g400);font-size:12px;">Loading users...</p>');

        Api.get('/users', function (data) {
            UserController._data = data;
            UserController.render(data);
        }, function (err) {
            $('#userGrid').html('<p style="color:var(--red);font-size:12px;">Failed to load: ' + err + '</p>');
        });
    },

    //Render user cards
    render: function (list) {
        if (!list || !list.length) {
            $('#userGrid').html('<p style="color:var(--g400);font-size:12px;">No users found.</p>');
            return;
        }

        var html = '';
        $.each(list, function (i, u) {
            var fullName = u.firstName + ' ' + u.lastName;
            var initials = UserController._initials(fullName);
            var color = u.role === 'Admin' ? '#b06040' : '#4a6fa5';
            var roleBadge = u.role === 'Admin'
                ? '<span class="bdg bdg-a">Admin</span>'
                : '<span class="bdg bdg-p">Teacher</span>';

            html += '<div class="stu-card">' +
                '<div class="stu-top">' +
                '<div class="stu-av" style="background:' + color + '">' + initials + '</div>' +
                '<div>' +
                '<div class="stu-nm">' + fullName + '</div>' +
                '<div class="stu-sid">' + u.email + '</div>' +
                '</div>' +
                '</div>' +
                '<div style="margin:8px 0;">' + roleBadge + '</div>' +
                '<div class="stu-actions">' +
                '<button class="btn btn-o btn-xs" style="flex:1" onclick="UserController.openEdit(' + u.id + ')">✏️ Edit</button>' +
                '<button class="btn btn-d btn-xs" onclick="UserController.remove(' + u.id + ')">✕</button>' +
                '</div>' +
                '</div>';
        });

        $('#userGrid').html(html);
    },

    //Filter by role
    filterByRole: function (role) {
        if (!role) { UserController.render(UserController._data); return; }
        var filtered = $.grep(UserController._data, function (u) { return u.role === role; });
        UserController.render(filtered);
    },

    //Open Add modal
    openAdd: function () {
        $('#uAddFn, #uAddLn, #uAddEmail, #uAddPass').val('');
        $('#uAddRole').val('Teacher');
        $('#addUserOverlay').addClass('show');
    },

    //Add user 
    add: function () {
        var fn = $.trim($('#uAddFn').val());
        var ln = $.trim($('#uAddLn').val());
        var email = $.trim($('#uAddEmail').val());
        var pass = $('#uAddPass').val();
        var role = $('#uAddRole').val();

        if (!fn || !ln || !email || !pass) { Helpers.toast('Fill in all fields.', 'err'); return; }
        if (!Helpers.isValidEmail(email)) { Helpers.toast('Invalid email.', 'err'); return; }
        if (pass.length < 6) { Helpers.toast('Password min 6 characters.', 'err'); return; }

        var body = { firstName: fn, lastName: ln, email: email, password: pass, role: role };

        Api.post('/users', body, function () {
            UserController.closeModal('addUserOverlay');
            UserController.load();
            Helpers.toast(fn + ' ' + ln + ' added!', 'ok');
            Helpers.addActivity( 'User added', fn + ' ' + ln + ' (' + role + ')');
        });
    },

    //Open Edit modal
    openEdit: function (id) {
        Api.get('/users/' + id, function (u) {
            $('#uEditId').val(u.id);
            $('#uEditFn').val(u.firstName);
            $('#uEditLn').val(u.lastName);
            $('#uEditEmail').val(u.email);
            $('#uEditPass').val('');
            $('#uEditRole').val(u.role);
            $('#editUserOverlay').addClass('show');
        });
    },

    //Save edit 
    saveEdit: function () {
        var id = parseInt($('#uEditId').val());
        var fn = $.trim($('#uEditFn').val());
        var ln = $.trim($('#uEditLn').val());
        var email = $.trim($('#uEditEmail').val());
        var pass = $('#uEditPass').val();
        var role = $('#uEditRole').val();

        if (!fn || !ln || !email) { Helpers.toast('Fill in all fields.', 'err'); return; }

        var body = { firstName: fn, lastName: ln, email: email, role: role };
        if (pass) body.password = pass;

        Api.put('/users/' + id, body, function () {
            UserController.closeModal('editUserOverlay');
            UserController.load();
            Helpers.toast(fn + ' ' + ln + ' updated!', 'ok');
            Helpers.addActivity( 'User updated', fn + ' ' + ln);
        });
    },

    // Delete user
    remove: function (id) {
        var u = null;
        $.each(UserController._data, function (i, x) { if (x.id === id) { u = x; return false; } });
        var name = u ? (u.firstName + ' ' + u.lastName) : 'this user';
        if (!confirm('Delete ' + name + '?')) return;

        Api.del('/users/' + id, function () {
            UserController.load();
            Helpers.toast(name + ' deleted.');
            Helpers.addActivity( 'User deleted', name);
        });
    },

    closeModal: function (id) { $('#' + id).removeClass('show'); },

    _initials: function (name) {
        return $.map((name || '').split(' '), function (n) {
            return n ? n[0].toUpperCase() : null;
        }).slice(0, 2).join('');
    }
};
