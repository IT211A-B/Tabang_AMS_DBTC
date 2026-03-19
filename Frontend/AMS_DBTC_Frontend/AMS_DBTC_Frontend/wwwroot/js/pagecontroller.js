var StuController = {

    render: function (list) {
        list = list || STUDENTS;
        var html = '';

        $.each(list, function (i, s) {
            html += '<div class="stu-card">' +
                '<div class="stu-top">' +
                '<div class="stu-av" style="background:' + s.color + '">' + s.initials() + '</div>' +
                '<div>' +
                '<div class="stu-nm">' + s.name + '</div>' +
                '<div class="stu-sid">' + s.id + '</div>' +
                '</div>' +
                '</div>' +
                '<div class="stu-stats">' +
                '<div class="stu-s"><div class="stu-sv" style="color:var(--green)">' + s.p + '</div><div class="stu-sl">Present</div></div>' +
                '<div class="stu-s"><div class="stu-sv" style="color:var(--red)">' + s.a + '</div><div class="stu-sl">Absent</div></div>' +
                '<div class="stu-s"><div class="stu-sv" style="color:var(--orange)">' + s.l + '</div><div class="stu-sl">Late</div></div>' +
                '</div>' +
                '<div style="margin-top:9px;">' +
                '<div style="display:flex;justify-content:space-between;font-size:10px;margin-bottom:2px;">' +
                '<span style="color:var(--g600)">Attendance Rate</span>' +
                '<strong style="color:' + s.rateColor() + '">' + s.rate() + '%</strong>' +
                '</div>' +
                '<div class="rbar"><div class="rfill" style="width:' + s.rate() + '%;background:' + s.rateColor() + '"></div></div>' +
                '</div>' +
                '<div class="stu-actions">' +
                '<button class="btn btn-outline btn-xs" style="flex:1" onclick="StuController.view(\'' + s.id + '\')">View</button>' +
                '<button class="btn btn-danger btn-xs" onclick="StuController.remove(\'' + s.id + '\')">✕</button>' +
                '</div>' +
                '</div>';
        });

        if (!html) {
            html = '<p style="color:var(--g400);font-size:12px;">No students found.</p>';
        }

        $('#stuGrid').html(html);
    },

    filter: function (q) {
        q = $.trim(q).toLowerCase();
        var list = !q ? STUDENTS : $.grep(STUDENTS, function (s) {
            return s.name.toLowerCase().indexOf(q) > -1 || s.id.indexOf(q) > -1;
        });
        StuController.render(list);
    },

    view: function (id) {
        var s = null;
        $.each(STUDENTS, function (i, x) {
            if (x.id === id) { s = x; return false; }
        });
        if (!s) return;

        $('#vName').text(s.name);
        $('#vId').text('ID: ' + s.id);
        $('#vP').text(s.p);
        $('#vA').text(s.a);
        $('#vL').text(s.l);
        $('#vRate').text(s.rate() + '%').css('color', s.rateColor());
        $('#vRfill').css({ width: s.rate() + '%', background: s.rateColor() });
        $('#viewStuOverlay').addClass('show');
    },

    remove: function (id) {
        var s = null;
        $.each(STUDENTS, function (i, x) {
            if (x.id === id) { s = x; return false; }
        });
        if (!s || !confirm('Remove ' + s.name + '?')) return;

        STUDENTS = $.grep(STUDENTS, function (x) { return x.id !== id; });
        delete STATE.attendance[id];

        StuController.render();
        Helpers.addActivity('🗑️', 'Student removed', s.name);
        Helpers.toast(s.name + ' removed.');
    },

    openAdd: function () {
        $('#addStuOverlay').addClass('show');
    },

    add: function () {
        var fn = $.trim($('#mFn').val());
        var ln = $.trim($('#mLn').val());
        var sid = $.trim($('#mId').val());

        if (!fn || !ln || !sid) {
            Helpers.toast('Fill in all fields.', 'err');
            return;
        }

        var exists = false;
        $.each(STUDENTS, function (i, s) {
            if (s.id === sid) { exists = true; return false; }
        });

        if (exists) {
            Helpers.toast('ID already exists.', 'err');
            return;
        }

        var color = COLORS[STUDENTS.length % COLORS.length];
        STUDENTS.push(new StudentModel({ id: sid, name: fn + ' ' + ln, color: color }));

        StuController.closeModal('addStuOverlay');
        $('#mFn, #mLn, #mId').val('');
        StuController.render();
        DashController._renderStats();
        Helpers.addActivity('👤', 'Student added', fn + ' ' + ln + ' (' + sid + ')');
        Helpers.toast(fn + ' ' + ln + ' added!', 'ok');
    },

    closeModal: function (id) {
        $('#' + id).removeClass('show');
    }
};


/* ─────────────────────────────────────────────────────────────
   RptController  —  Reports Page
   ───────────────────────────────────────────────────────────── */

var RptController = {

    render: function () {
        var period = $('#rptP').val() || 'month';
        var labels = { month: 'This Month', week: 'This Week', all: 'All Time' };
        $('#rptLbl').text(labels[period] || 'This Month');

        var tP = 0, tA = 0, tL = 0;
        var rows = '';

        $.each(STUDENTS, function (i, s) {
            tP += s.p;
            tA += s.a;
            tL += s.l;

            var cls = s.rate() >= 90 ? 'bdg-p' : s.rate() >= 75 ? 'bdg-b' : 'bdg-a';

            rows += '<tr>' +
                '<td>' +
                '<div style="display:flex;align-items:center;gap:6px;">' +
                '<div class="s-av" style="background:' + s.color + ';width:22px;height:22px;font-size:9px;">' + s.initials() + '</div>' +
                s.name +
                '</div>' +
                '</td>' +
                '<td style="color:var(--green);font-weight:700">' + s.p + '</td>' +
                '<td style="color:var(--red);font-weight:700">' + s.a + '</td>' +
                '<td style="color:var(--orange);font-weight:700">' + s.l + '</td>' +
                '<td><span class="bdg ' + cls + '">' + s.rate() + '%</span></td>' +
                '</tr>';
        });

        $('#rptBody').html(rows);

        // Overview bars
        var gt = tP + tA + tL;
        var avg = gt > 0 ? Math.round(tP / gt * 100) : 0;
        var rc = Helpers.rateColor(avg);
        var pct = function (n) { return gt > 0 ? Math.round(n / gt * 100) : 0; };

        var ovHtml = '<div style="display:flex;flex-direction:column;gap:9px;">' +
            RptController._bar('Avg. Rate', avg + '%', rc, avg) +
            RptController._bar('Total Present', tP, 'var(--green)', pct(tP)) +
            RptController._bar('Total Absent', tA, 'var(--red)', pct(tA)) +
            RptController._bar('Total Late', tL, 'var(--orange)', pct(tL)) +
            '</div>';
        $('#rptOv').html(ovHtml);

        // At-risk students
        var atRisk = $.grep(STUDENTS, function (s) { return s.isAtRisk(); });
        var arHtml = '';

        if (atRisk.length) {
            $.each(atRisk, function (i, s) {
                arHtml += '<div class="al-item al-danger">' +
                    '<div class="al-ico">⚠️</div>' +
                    '<div class="al-t">' +
                    '<strong>' + s.name + '</strong>' +
                    '<span>' + s.rate() + '% · ' + s.a + ' absent, ' + s.l + ' late</span>' +
                    '</div>' +
                    '</div>';
            });
        } else {
            arHtml = '<p style="font-size:11px;color:var(--green);font-weight:600;">✓ No students at risk!</p>';
        }

        $('#rptAR').html(arHtml);
    },

    _bar: function (label, val, color, pct) {
        return '<div>' +
            '<div style="display:flex;justify-content:space-between;font-size:11px;margin-bottom:2px;">' +
            '<span style="color:var(--g600)">' + label + '</span>' +
            '<strong style="color:' + color + '">' + val + '</strong>' +
            '</div>' +
            '<div class="rbar"><div class="rfill" style="width:' + pct + '%;background:' + color + '"></div></div>' +
            '</div>';
    },

    exportCSV: function () {
        Helpers.exportCSV(STUDENTS);
        Helpers.addActivity('📊', 'CSV exported', 'Attendance data downloaded');
        Helpers.toast('CSV exported!', 'ok');
    }
};


/* ─────────────────────────────────────────────────────────────
   SetController  —  Settings Page
   ───────────────────────────────────────────────────────────── */

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

        if (!name || !email) {
            Helpers.toast('Fill in all fields.', 'err');
            return;
        }
        if (!Helpers.isValidEmail(email)) {
            Helpers.toast('Invalid email.', 'err');
            return;
        }

        STATE.currentUser.name = name;
        STATE.currentUser.email = email;
        Session.save(STATE.currentUser, STATE.currentRole);
        AppController._applyUser();
        Helpers.addActivity('✏️', 'Profile updated', name);
        Helpers.toast('Profile saved!', 'ok');
    },

    changePass: function () {
        var cur = $('#sCur').val();
        var nw = $('#sNew').val();
        var con = $('#sCon').val();

        if (!cur || !nw || !con) {
            Helpers.toast('Fill in all fields.', 'err');
            return;
        }
        if (cur !== STATE.currentUser.pass) {
            Helpers.toast('Current password incorrect.', 'err');
            return;
        }
        if (nw.length < 6) {
            Helpers.toast('Min. 6 characters required.', 'err');
            return;
        }
        if (nw !== con) {
            Helpers.toast('Passwords do not match.', 'err');
            return;
        }

        STATE.currentUser.pass = nw;
        var u = null;
        $.each(USERS, function (i, x) {
            if (x.email === STATE.currentUser.email) { u = x; return false; }
        });
        if (u) u.pass = nw;

        $('#sCur, #sNew, #sCon').val('');
        Helpers.toast('Password updated!', 'ok');
    }
};
