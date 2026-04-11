
var StuController = {

    render: function (list) {
        list = list || STUDENTS;
        var html = '';
        $.each(list, function (i, s) {
            var courseName = '—';
            if (s.courseId) {
                $.each(COURSES, function (j, c) {
                    if (c.id === s.courseId) { courseName = c.name; return false; }
                });
            }
            html += '<div class="stu-card">' +
                '<div class="stu-top">' +
                '<div class="stu-av" style="background:' + s.color + '">' + s.initials() + '</div>' +
                '<div><div class="stu-nm">' + s.name + '</div><div class="stu-sid">' + s.id + '</div></div>' +
                '</div>' +
                '<div style="margin:6px 0 8px;"><span class="bdg bdg-b" style="font-size:10px;"> ' + courseName + '</span></div>' +
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
                '<button class="btn btn-o btn-xs" style="flex:1" onclick="StuController.view(\'' + s.id + '\')">View</button>' +
                '<button class="btn btn-d btn-xs" onclick="StuController.remove(\'' + s.id + '\')">✕</button>' +
                '</div>' +
                '</div>';
        });
        if (!html) html = '<p style="color:var(--g400);font-size:12px;">No students found.</p>';
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
        $.each(STUDENTS, function (i, x) { if (x.id === id) { s = x; return false; } });
        if (!s) return;
        var courseName = '—';
        if (s.courseId) {
            $.each(COURSES, function (j, c) { if (c.id === s.courseId) { courseName = c.name; return false; } });
        }
        $('#vName').text(s.name);
        $('#vId').text('ID: ' + s.id);
        $('#vCourse').text(courseName);
        $('#vP').text(s.p); $('#vA').text(s.a); $('#vL').text(s.l);
        $('#vRate').text(s.rate() + '%').css('color', s.rateColor());
        $('#vRfill').css({ width: s.rate() + '%', background: s.rateColor() });
        $('#viewStuOverlay').addClass('show');
    },

    remove: function (id) {
        var s = null;
        $.each(STUDENTS, function (i, x) { if (x.id === id) { s = x; return false; } });
        if (!s || !confirm('Remove ' + s.name + '?')) return;
        STUDENTS = $.grep(STUDENTS, function (x) { return x.id !== id; });
        delete STATE.attendance[id];
        StuController.render();
        Helpers.toast(s.name + ' removed.');
    },

    openAdd: function () { $('#addStuOverlay').addClass('show'); },

    add: function () {
        var fn = $.trim($('#mFn').val());
        var ln = $.trim($('#mLn').val());
        var sid = $.trim($('#mId').val());
        if (!fn || !ln || !sid) { Helpers.toast('Fill in all fields.', 'err'); return; }
        var exists = false;
        $.each(STUDENTS, function (i, s) { if (s.id === sid) { exists = true; return false; } });
        if (exists) { Helpers.toast('ID already exists.', 'err'); return; }
        var color = COLORS[STUDENTS.length % COLORS.length];
        STUDENTS.push(new StudentModel({ id: sid, name: fn + ' ' + ln, color: color }));
        StuController.closeModal('addStuOverlay');
        $('#mFn, #mLn, #mId').val('');
        StuController.render();
        Helpers.toast(fn + ' ' + ln + ' added!', 'ok');
    },

    closeModal: function (id) { $('#' + id).removeClass('show'); }
};
