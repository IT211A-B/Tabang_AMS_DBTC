var AppController = {

    //Init on every protected page
    init: function () {
        if (!AuthController.requireAuth()) return;
        AppController._applyUser();
        AppController._setDate();
    },

    //Apply user info to sidebar
    _applyUser: function () {
        var u = STATE.currentUser;
        if (!u) return;

        $('#uAv').text(u.initials());
        $('#uNm').text(u.name);
        $('#uRl').text(STATE.currentRole === 'admin' ? 'Administrator' : u.section);
        $('#vbadge').text(STATE.currentRole === 'admin' ? 'Admin View' : 'Teacher View');

        $('.rb').removeClass('on');
        $('.rb[data-role="' + STATE.currentRole + '"]').addClass('on');
    },

    // Set topbar date
    _setDate: function () {
        $('#topDate').text(Helpers.formatDate());
    },

    // Switch role
    switchRole: function (role, btn) {
        STATE.currentRole = role;

        if (STATE.currentUser) {
            Session.save(STATE.currentUser, role);
        }

        $('.rb').removeClass('on');
        $(btn).addClass('on');
        $('#vbadge').text(role === 'admin' ? 'Admin View' : 'Teacher View');

        var u = STATE.currentUser;
        if (u) {
            $('#uRl').text(role === 'admin' ? 'Administrator' : u.section);
        }

        Helpers.toast('Switched to ' + (role === 'admin' ? 'Admin' : 'Teacher') + ' view');
    },

    // Navigate between pages
    navTo: function (page, el) {
        STATE.currentPage = page;

        $('.ni').removeClass('on');
        $(el).addClass('on');

        var pageMap = {
            dash: '#dashPage',
            att: '#attPage',
            stu: '#stuPage',
            rpt: '#rptPage',
            set: '#setPage'
        };

        var titleMap = {
            dash: 'Dashboard',
            att: 'Daily Attendance',
            stu: 'Students',
            rpt: 'Reports',
            set: 'Settings'
        };

        $('.page').hide();
        $(pageMap[page]).show();
        $('#topTitle').text(titleMap[page]);

        if (page === 'dash') DashController.render();
        if (page === 'att') AttController.render();
        if (page === 'stu') StuController.render();
        if (page === 'rpt') RptController.render();
        if (page === 'set') SetController.init();
    },

    // Class selector change
    onClassChange: function () {
        var cls = $('#classSel').val();
        Helpers.addActivity('🏫', 'Class changed', cls);
        if (STATE.currentPage === 'dash') DashController.render();
    }
};