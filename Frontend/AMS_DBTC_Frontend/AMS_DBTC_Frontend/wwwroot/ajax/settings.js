var SetController = {

    init: function () {
        var u = STATE.currentUser;
        if (!u) return;
        $('#setName').val(u.name);
        $('#setEmail').val(u.email);
    },

    tab: function (el, panelId) {
        $('.set-mi').removeClass('on');
        $(el).addClass('on');
        $.each(['sPP', 'sCP', 'sNP', 'sSP'], function (i, id) {
            $('#' + id).toggle(id === panelId);
        });
    },

    saveProfile: function () {
        var name = $.trim($('#setName').val());
        var email = $.trim($('#setEmail').val());
        if (!name || !email) { Helpers.toast('Fill in all fields.', 'err'); return; }
        if (!Helpers.isValidEmail(email)) { Helpers.toast('Invalid email.', 'err'); return; }
        STATE.currentUser.name = name;
        STATE.currentUser.email = email;
        Session.save(STATE.currentUser, STATE.currentRole);
        AppController._applyUser();
        Helpers.toast('Profile saved!', 'ok');
    },

    changePass: function () {
        var cur = $('#sCur').val();
        var nw = $('#sNew').val();
        var con = $('#sCon').val();
        if (!cur || !nw || !con) { Helpers.toast('Fill in all fields.', 'err'); return; }
        if (cur !== STATE.currentUser.pass) { Helpers.toast('Current password incorrect.', 'err'); return; }
        if (nw.length < 6) { Helpers.toast('Min. 6 characters.', 'err'); return; }
        if (nw !== con) { Helpers.toast('Passwords do not match.', 'err'); return; }
        STATE.currentUser.pass = nw;
        $.each(USERS, function (i, u) { if (u.email === STATE.currentUser.email) { u.pass = nw; return false; } });
        $('#sCur, #sNew, #sCon').val('');
        Helpers.toast('Password updated!', 'ok');
    }
};
