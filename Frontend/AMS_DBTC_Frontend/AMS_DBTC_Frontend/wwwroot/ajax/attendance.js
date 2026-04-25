var AttController = {

    _students: [],   // students in the selected section
    _records: {},   // { studentId: attendanceRecord }
    _sectionId: null,
    _date: null, // today's date as "YYYY-MM-DD"

    // load sections into selector
    init: function () {
        AttController._date = new Date().toISOString().split('T')[0];
        $('#attDate').val(AttController._date);

        Api.get('/sections?pageSize=100', function (data) {
            var sections = data.data || data;
            var opts = '<option value="">— Select Section —</option>';
            $.each(sections, function (i, sec) {
                opts += '<option value="' + sec.id + '">' + sec.name + ' (' + sec.schoolYear + ')</option>';
            });
            $('#attSectionSel').html(opts);
        });
    },

    // Load students + existing records for selected section/date 
    loadSection: function () {
        var sectionId = parseInt($('#attSectionSel').val());
        var date = $('#attDate').val() || AttController._date;

        if (!sectionId) { Helpers.toast('Please select a section.', 'err'); return; }

        AttController._sectionId = sectionId;
        AttController._date = date;
        AttController._records = {};

        $('#attRows').html('<p style="color:var(--g400);font-size:12px;padding:12px;">Loading...</p>');

        // Load students in this section
        Api.get('/students/section/' + sectionId, function (students) {
            AttController._students = students;
            AttController._renderMinis();
            AttController._renderBottom();

            // Load existing attendance records for this date
            Api.get('/attendance/section/' + sectionId + '/date/' + date, function (records) {
                $.each(records, function (i, r) {
                    AttController._records[r.studentId] = r;
                });
                AttController._renderRows();
            }, function () {
                // No records yet for this date 
                AttController._renderRows();
            });
        });
    },

    //Mini counters
    _renderMinis: function () {
        var p = 0, a = 0, l = 0, e = 0;
        $.each(AttController._records, function (id, r) {
            if (r.status === 'Present') p++;
            else if (r.status === 'Absent') a++;
            else if (r.status === 'Late') l++;
            else if (r.status === 'Excused') e++;
        });
        $('#amT').text(AttController._students.length);
        $('#amP').text(p);
        $('#amA').text(a);
        $('#amL').text(l);
    },

    //Render student rows
    _renderRows: function () {
        var html = '';
        $.each(AttController._students, function (i, s) {
            var rec = AttController._records[s.id] || null;
            var status = rec ? rec.status : null;
            var badge = status
                ? '<span class="bdg ' + AttController._badgeClass(status) + '">' + status + '</span>'
                : '<span class="bdg bdg-n">—</span>';

            var pOn = status === 'Present' ? 'p-on' : '';
            var aOn = status === 'Absent' ? 'a-on' : '';
            var lOn = status === 'Late' ? 'l-on' : '';
            var eOn = status === 'Excused' ? 'p-on' : '';

            var fullName = s.firstName + ' ' + s.lastName;
            var color = AttController._color(s.id);
            var initials = AttController._initials(fullName);

            html += '<div class="trow tbl-grid" id="att-row-' + s.id + '">' +
                '<div class="td" style="color:var(--g400);font-size:11px;font-weight:600;">' + Helpers.pad(i + 1) + '</div>' +
                '<div class="td"><div class="s-info">' +
                '<div class="s-av" style="background:' + color + '">' + initials + '</div>' +
                '<div><div class="s-nm">' + fullName + '</div><div class="s-id">' + (s.studentNumber || s.id) + '</div></div>' +
                '</div></div>' +
                '<div class="td" id="att-badge-' + s.id + '">' + badge + '</div>' +
                '<div class="td"><div class="mbs">' +
                '<button class="mb ' + pOn + '" onclick="AttController.mark(' + s.id + ',\'Present\',this)">P</button>' +
                '<button class="mb ' + aOn + '" onclick="AttController.mark(' + s.id + ',\'Absent\',this)">A</button>' +
                '<button class="mb ' + lOn + '" onclick="AttController.mark(' + s.id + ',\'Late\',this)">L</button>' +
                '<button class="mb ' + eOn + '" onclick="AttController.mark(' + s.id + ',\'Excused\',this)">E</button>' +
                '</div></div>' +
                '<div class="td"><input class="note-i" type="text" placeholder="Optional remarks…" ' +
                'value="' + (rec ? (rec.remarks || '') : '') + '" ' +
                'onchange="AttController.setRemark(' + s.id + ', this.value)" /></div>' +
                '</div>';
        });

        if (!html) html = '<p style="color:var(--g400);font-size:12px;padding:12px;">No students in this section.</p>';
        $('#attRows').html(html);
        AttController._renderMinis();
        AttController._renderBottom();
    },

    //Bottom counter
    _renderBottom: function () {
        var marked = Object.keys(AttController._records).length;
        $('#attMarked').text(marked);
        $('#attTotal').text(AttController._students.length);
    },
    
    //Mark a student
    mark: function (studentId, status, btn) {
        var existing = AttController._records[studentId];

        if (existing) {
            // Update existing record via API
            Api.put('/attendance/' + existing.id, { status: status }, function (updated) {
                AttController._records[studentId] = updated;
                AttController._updateRow(studentId, status, btn);
            });
        } else {
            // Create new record via API
            var body = {
                studentId: studentId,
                sectionId: AttController._sectionId,
                date: AttController._date,
                status: status,
                remarks: ''
            };
            Api.post('/attendance', body, function (created) {
                AttController._records[studentId] = created;
                AttController._updateRow(studentId, status, btn);
            });
        }
    },

    //Update row UI after marking
    _updateRow: function (studentId, status, btn) {
        $('#att-row-' + studentId + ' .mb').removeClass('p-on a-on l-on');
        $(btn).addClass(status === 'Present' || status === 'Excused' ? 'p-on' : status === 'Absent' ? 'a-on' : 'l-on');
        $('#att-badge-' + studentId).html('<span class="bdg ' + AttController._badgeClass(status) + '">' + status + '</span>');
        AttController._renderMinis();
        AttController._renderBottom();
    },

    //Update remark
    setRemark: function (studentId, value) {
        var rec = AttController._records[studentId];
        if (rec) {
            Api.put('/attendance/' + rec.id, { remarks: value }, function (updated) {
                AttController._records[studentId] = updated;
            });
        }
    },

    //Mark all present
    markAllPresent: function () {
        if (!AttController._students.length) { Helpers.toast('Load a section first.', 'err'); return; }
        var pending = AttController._students.length;
        var done = 0;

        $.each(AttController._students, function (i, s) {
            var existing = AttController._records[s.id];
            if (existing) {
                Api.put('/attendance/' + existing.id, { status: 'Present' }, function (updated) {
                    AttController._records[s.id] = updated;
                    done++;
                    if (done === pending) { AttController._renderRows(); Helpers.toast('All marked Present!', 'ok'); }
                });
            } else {
                var body = { studentId: s.id, sectionId: AttController._sectionId, date: AttController._date, status: 'Present', remarks: '' };
                Api.post('/attendance', body, function (created) {
                    AttController._records[s.id] = created;
                    done++;
                    if (done === pending) { AttController._renderRows(); Helpers.toast('All marked Present!', 'ok'); }
                });
            }
        });
    },

    // Submit 
    submit: function () {
        var marked = Object.keys(AttController._records).length;
        var total = AttController._students.length;
        if (marked < total) {
            if (!confirm('Only ' + marked + ' of ' + total + ' students marked. Submit anyway?')) return;
        }
        Helpers.toast('Attendance saved to database!', 'ok');
        Helpers.addActivity( 'Attendance submitted', 'Section ' + AttController._sectionId + ' — ' + AttController._date);
    },

    //helpers 
    _badgeClass: function (status) {
        return { Present: 'bdg-p', Absent: 'bdg-a', Late: 'bdg-l', Excused: 'bdg-b' }[status] || 'bdg-n';
    },

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
