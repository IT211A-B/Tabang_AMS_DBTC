var AttController = {

    render: function (list) {
        list = list || STUDENTS;
        AttController._renderMinis();
        AttController._renderRows(list);
        AttController._renderBottom();
    },

    // ── Mini stat counters 
    _renderMinis: function () {
        var p = 0, a = 0, l = 0;
        $.each(STATE.attendance, function (id, r) {
            if (r.status === 'P') p++;
            else if (r.status === 'A') a++;
            else if (r.status === 'L') l++;
        });

        $('#amT').text(STUDENTS.length);
        $('#amP').text(p);
        $('#amA').text(a);
        $('#amL').text(l);
    },

    // ── Student table rows 
    _renderRows: function (list) {
        var html = '';

        $.each(list, function (i, s) {
            var rec = STATE.attendance[s.id] || null;
            var status = rec ? rec.status : null;

            var badge = status
                ? '<span class="bdg ' + rec.badgeClass() + '">' + rec.label() + '</span>'
                : '<span class="bdg bdg-n">—</span>';

            var pOn = status === 'P' ? 'p-on' : '';
            var aOn = status === 'A' ? 'a-on' : '';
            var lOn = status === 'L' ? 'l-on' : '';

            html += '<div class="trow tbl-grid" id="row-' + s.id + '">' +
                '<div class="td" style="color:var(--g400);font-size:11px;font-weight:600;">' + Helpers.pad(i + 1) + '</div>' +
                '<div class="td">' +
                '<div class="s-info">' +
                '<div class="s-av" style="background:' + s.color + '">' + s.initials() + '</div>' +
                '<div>' +
                '<div class="s-nm">' + s.name + '</div>' +
                '<div class="s-id">' + s.id + '</div>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '<div class="td" id="badge-' + s.id + '">' + badge + '</div>' +
                '<div class="td">' +
                '<div class="mbs">' +
                '<button class="mb ' + pOn + '" onclick="AttController.mark(\'' + s.id + '\',\'P\',this)">P</button>' +
                '<button class="mb ' + aOn + '" onclick="AttController.mark(\'' + s.id + '\',\'A\',this)">A</button>' +
                '<button class="mb ' + lOn + '" onclick="AttController.mark(\'' + s.id + '\',\'L\',this)">L</button>' +
                '</div>' +
                '</div>' +
                '<div class="td">' +
                '<input class="note-i" type="text" placeholder="Optional note…" ' +
                'value="' + (rec ? rec.remark : '') + '" ' +
                'onchange="AttController.setRemark(\'' + s.id + '\', this.value)" />' +
                '</div>' +
                '</div>';
        });

        $('#attRows').html(html);
    },

    // ── Bottom marked counter 
    _renderBottom: function () {
        var marked = 0;
        $.each(STATE.attendance, function () { marked++; });
        $('#attMarked').text(marked);
        $('#attTotal').text(STUDENTS.length);
    },

    // ── Mark a student P / A / L
    mark: function (id, status, btn) {
        var rec = new AttendanceRecord(id, status);

        if (STATE.attendance[id]) {
            rec.remark = STATE.attendance[id].remark;
        }
        STATE.attendance[id] = rec;

        $('#row-' + id + ' .mb').removeClass('p-on a-on l-on');
        $(btn).addClass(rec.markClass());

        $('#badge-' + id).html('<span class="bdg ' + rec.badgeClass() + '">' + rec.label() + '</span>');

        AttController._renderMinis();
        AttController._renderBottom();
        DashController._renderStats();
    },

    // ── Save remark 
    setRemark: function (id, value) {
        if (STATE.attendance[id]) {
            STATE.attendance[id].remark = value;
        }
    },

    // ── Mark all students present 
    markAllPresent: function () {
        $.each(STUDENTS, function (i, s) {
            STATE.attendance[s.id] = new AttendanceRecord(s.id, 'P');
        });
        AttController.render();
        Helpers.toast('All students marked Present.', 'ok');
        Helpers.addActivity('✅', 'Mark All Present', $('#classSel').val());
    },

    // ── Filter rows by search query 
    filter: function (q) {
        q = $.trim(q).toLowerCase();
        var list = !q ? STUDENTS : $.grep(STUDENTS, function (s) {
            return s.name.toLowerCase().indexOf(q) > -1 || s.id.indexOf(q) > -1;
        });
        AttController._renderRows(list);
    },

    // ── Submit attendance 
    submit: function () {
        var marked = 0;
        $.each(STATE.attendance, function () { marked++; });

        if (marked < STUDENTS.length) {
            if (!confirm('Only ' + marked + ' of ' + STUDENTS.length + ' students are marked. Submit anyway?')) return;
        }

        Helpers.addActivity('📋', 'Attendance submitted', $('#classSel').val());
        DashController.render();
        Helpers.toast('Attendance submitted successfully!', 'ok');
    }
};
