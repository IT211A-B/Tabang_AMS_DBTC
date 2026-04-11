var AppController = {

    init: function () {
        AppController._applyUser();
        AppController._setDate();
    },

    _applyUser: function () {
        var u = STATE.currentUser;
        if (!u) return;
        $('#uAv').text(u.initials ? u.initials() : '??');
        $('#uNm').text(u.name || '');
        $('#uRl').text(STATE.currentRole === 'admin' ? 'Administrator' : (u.section || ''));
        $('#vbadge').text(STATE.currentRole === 'admin' ? 'Admin View' : 'Teacher View');
        $('.rb').removeClass('on');
        $('.rb[data-role="' + STATE.currentRole + '"]').addClass('on');
    },

    _setDate: function () {
        $('#topDate').text(Helpers.formatDate());
    },

    switchRole: function (role, btn) {
        STATE.currentRole = role;
        if (STATE.currentUser) Session.save(STATE.currentUser, role);
        $('.rb').removeClass('on');
        $(btn).addClass('on');
        $('#vbadge').text(role === 'admin' ? 'Admin View' : 'Teacher View');
        if (STATE.currentUser) {
            $('#uRl').text(role === 'admin' ? 'Administrator' : (STATE.currentUser.section || ''));
        }
        Helpers.toast('Switched to ' + (role === 'admin' ? 'Admin' : 'Teacher') + ' view');
    },

    onClassChange: function () {
        var cls = $('#classSel').val();
        Helpers.addActivity('🏫', 'Class changed', cls);
    }
};
