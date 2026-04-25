var StuController = {

    // Holds current student list from API
    _data: [],

    // Load all students
    load: function () {
        $('#stuGrid').html('<p style="color:var(--g400);font-size:12px;">Loading students...</p>');

        Api.get('/students', function (data) {
            StuController._data = data;
            StuController.render(data);
        }, function (err) {
            $('#stuGrid').html('<p style="color:var(--red);font-size:12px;">Failed to load students: ' + err + '</p>');
        });
    },

    //Load students by section 
    loadBySection: function (sectionId) {
        $('#stuGrid').html('<p style="color:var(--g400);font-size:12px;">Loading...</p>');

        Api.get('/students/section/' + sectionId, function (data) {
            StuController._data = data;
            StuController.render(data);
        });
    },

    // Render student cards
    render: function (list) {
        list = list || StuController._data;
        if (!list || !list.length) {
            $('#stuGrid').html('<p style="color:var(--g400);font-size:12px;">No students found.</p>');
            return;
        }

        var html = '';
        $.each(list, function (i, s) {
            var fullName = s.firstName + ' ' + s.lastName;
            var initials = StuController._initials(fullName);
            var color = StuController._color(s.id);
            var total = s.totalPresent + s.totalAbsent + s.totalLate;
            var rate = total > 0 ? Math.round(s.totalPresent / total * 100) : 100;
            var rateColor = rate >= 90 ? 'var(--green)' : rate >= 75 ? 'var(--orange)' : 'var(--red)';

            html += '<div class="stu-card">' +
                '<div class="stu-top">' +
                '<div class="stu-av" style="background:' + color + '">' + initials + '</div>' +
                '<div>' +
                '<div class="stu-nm">' + fullName + '</div>' +
                '<div class="stu-sid">' + (s.studentNumber || 'No ID') + '</div>' +
                '</div>' +
                '</div>' +
                '<div style="margin:6px 0 8px;">' +
                '<span class="bdg bdg-b" style="font-size:10px;">🏫 ' + (s.sectionName || '—') + '</span>' +
                '</div>' +
                '<div class="stu-stats">' +
                '<div class="stu-s"><div class="stu-sv" style="color:var(--green)">' + s.totalPresent + '</div><div class="stu-sl">Present</div></div>' +
                '<div class="stu-s"><div class="stu-sv" style="color:var(--red)">' + s.totalAbsent + '</div><div class="stu-sl">Absent</div></div>' +
                '<div class="stu-s"><div class="stu-sv" style="color:var(--orange)">' + s.totalLate + '</div><div class="stu-sl">Late</div></div>' +
                '<div class="stu-s"><div class="stu-sv" style="color:var(--blue)">' + s.totalExcused + '</div><div class="stu-sl">Excused</div></div>' +
                '</div>' +
                '<div style="margin-top:9px;">' +
                '<div style="display:flex;justify-content:space-between;font-size:10px;margin-bottom:2px;">' +
                '<span style="color:var(--g600)">Attendance Rate</span>' +
                '<strong style="color:' + rateColor + '">' + rate + '%</strong>' +
                '</div>' +
                '<div class="rbar"><div class="rfill" style="width:' + rate + '%;background:' + rateColor + '"></div></div>' +
                '</div>' +
                '<div class="stu-actions">' +
                '<button class="btn btn-o btn-xs" style="flex:1" onclick="StuController.view(' + s.id + ')">View</button>' +
                '<button class="btn btn-p btn-xs" onclick="StuController.openEdit(' + s.id + ')">✏️</button>' +
                '<button class="btn btn-d btn-xs" onclick="StuController.remove(' + s.id + ')">✕</button>' +
                '</div>' +
                '</div>';
        });

        $('#stuGrid').html(html);
    },

    // Search students 
    filter: function (q) {
        q = $.trim(q);
        if (!q) { StuController.render(StuController._data); return; }

        Api.get('/students/search?name=' + encodeURIComponent(q), function (data) {
            StuController.render(data);
        });
    },

    // View student detail 
    view: function (id) {
        Api.get('/students/' + id, function (s) {
            var fullName = s.firstName + ' ' + s.lastName;
            var total = s.totalPresent + s.totalAbsent + s.totalLate;
            var rate = total > 0 ? Math.round(s.totalPresent / total * 100) : 100;
            var rateColor = rate >= 90 ? 'var(--green)' : rate >= 75 ? 'var(--orange)' : 'var(--red)';

            $('#vName').text(fullName);
            $('#vId').text('ID: ' + (s.studentNumber || s.id));
            $('#vSection').text(s.sectionName || '—');
            $('#vP').text(s.totalPresent);
            $('#vA').text(s.totalAbsent);
            $('#vL').text(s.totalLate);
            $('#vRate').text(rate + '%').css('color', rateColor);
            $('#vRfill').css({ width: rate + '%', background: rateColor });
            $('#viewStuOverlay').addClass('show');
        });
    },

    // Open Add modal 
    openAdd: function () {
        $('#mFn, #mLn, #mStudentNo, #mEmail').val('');
        // Load sections into dropdown
        Api.get('/sections?pageSize=100', function (data) {
            var opts = '<option value="">— Select Section —</option>';
            $.each(data.data || data, function (i, sec) {
                opts += '<option value="' + sec.id + '">' + sec.name + ' (' + sec.schoolYear + ')</option>';
            });
            $('#mSectionId').html(opts);
        });
        $('#addStuOverlay').addClass('show');
    },

    // Add student 
    add: function () {
        var fn = $.trim($('#mFn').val());
        var ln = $.trim($('#mLn').val());
        var stuNo = $.trim($('#mStudentNo').val());
        var email = $.trim($('#mEmail').val());
        var sectionId = parseInt($('#mSectionId').val());

        if (!fn || !ln) { Helpers.toast('First and last name are required.', 'err'); return; }
        if (!sectionId) { Helpers.toast('Please select a section.', 'err'); return; }

        var body = {
            firstName: fn,
            lastName: ln,
            studentNumber: stuNo || null,
            email: email || null,
            sectionId: sectionId
        };

        Api.post('/students', body, function () {
            StuController.closeModal('addStuOverlay');
            StuController.load();
            Helpers.toast(fn + ' ' + ln + ' added!', 'ok');
            Helpers.addActivity('👤', 'Student added', fn + ' ' + ln);
        });
    },

    // Open Edit modal
    openEdit: function (id) {
        Api.get('/students/' + id, function (s) {
            $('#eStudentId').val(s.id);
            $('#eFn').val(s.firstName);
            $('#eLn').val(s.lastName);
            $('#eStudentNo').val(s.studentNumber || '');
            $('#eEmail').val(s.email || '');

            Api.get('/sections?pageSize=100', function (data) {
                var opts = '';
                $.each(data.data || data, function (i, sec) {
                    opts += '<option value="' + sec.id + '"' + (sec.id === s.sectionId ? ' selected' : '') + '>' +
                        sec.name + ' (' + sec.schoolYear + ')</option>';
                });
                $('#eSectionId').html(opts);
            });

            $('#editStuOverlay').addClass('show');
        });
    },

    // Save edit 
    saveEdit: function () {
        var id = parseInt($('#eStudentId').val());
        var fn = $.trim($('#eFn').val());
        var ln = $.trim($('#eLn').val());
        var stuNo = $.trim($('#eStudentNo').val());
        var email = $.trim($('#eEmail').val());
        var secId = parseInt($('#eSectionId').val());

        if (!fn || !ln) { Helpers.toast('First and last name are required.', 'err'); return; }

        var body = {
            firstName: fn,
            lastName: ln,
            studentNumber: stuNo || null,
            email: email || null,
            sectionId: secId || null
        };

        Api.put('/students/' + id, body, function () {
            StuController.closeModal('editStuOverlay');
            StuController.load();
            Helpers.toast(fn + ' ' + ln + ' updated!', 'ok');
            Helpers.addActivity('✏️', 'Student updated', fn + ' ' + ln);
        });
    },

    // Delete student
    remove: function (id) {
        var s = null;
        $.each(StuController._data, function (i, x) { if (x.id === id) { s = x; return false; } });
        var name = s ? (s.firstName + ' ' + s.lastName) : 'this student';
        if (!confirm('Remove ' + name + '? This will also delete their attendance records.')) return;

        Api.del('/students/' + id, function () {
            StuController.load();
            Helpers.toast(name + ' removed.');
            Helpers.addActivity('🗑️', 'Student removed', name);
        });
    },

    closeModal: function (id) { $('#' + id).removeClass('show'); },

    //  Helpers 
    _initials: function (name) {
        return $.map((name || '').split(' '), function (n) {
            return n ? n[0].toUpperCase() : null;
        }).slice(0, 2).join('');
    },

    _color: function (id) {
        var colors = ['#4a6fa5', '#3a7d5c', '#7b5ea7', '#b06040', '#4a8a9a', '#5a7a3a', '#6a4a8a', '#8a3a3a', '#3a7a7a', '#7a6a2a'];
        return colors[id % colors.length];
    }
};
