var RptController = {

    _students: [],

    // Load and render report 
    render: function () {
        var sectionId = $('#rptSection').val();
        var from = $('#rptFrom').val();
        var to = $('#rptTo').val();

        // Decide which endpoint to call
        var url = sectionId ? '/students/section/' + sectionId : '/students';

        $('#rptBody').html('<tr><td colspan="5" style="text-align:center;color:var(--g400);">Loading...</td></tr>');

        Api.get(url, function (students) {
            RptController._students = students;
            RptController._renderTable(students);
            RptController._renderOverview(students);
            RptController._renderAtRisk(students);
        });
    },

    // Load sections into selector 
    init: function () {
        var today = new Date();
        var firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
        $('#rptFrom').val(firstDay.toISOString().split('T')[0]);
        $('#rptTo').val(today.toISOString().split('T')[0]);

        Api.get('/sections?pageSize=100', function (data) {
            var sections = data.data || data;
            var opts = '<option value="">All Sections</option>';
            $.each(sections, function (i, sec) {
                opts += '<option value="' + sec.id + '">' + sec.name + '</option>';
            });
            $('#rptSection').html(opts);
        });
    },

    // Render summary table
    _renderTable: function (students) {
        var rows = '';
        $.each(students, function (i, s) {
            var total = s.totalPresent + s.totalAbsent + s.totalLate;
            var rate = total > 0 ? Math.round(s.totalPresent / total * 100) : 100;
            var cls = rate >= 90 ? 'bdg-p' : rate >= 75 ? 'bdg-b' : 'bdg-a';
            var color = RptController._color(s.id);
            var name = s.firstName + ' ' + s.lastName;
            var init = RptController._initials(name);

            rows += '<tr>' +
                '<td><div style="display:flex;align-items:center;gap:6px;">' +
                '<div class="s-av" style="background:' + color + ';width:22px;height:22px;font-size:9px;">' + init + '</div>' +
                name +
                '</div></td>' +
                '<td style="color:var(--green);font-weight:700">' + s.totalPresent + '</td>' +
                '<td style="color:var(--red);font-weight:700">' + s.totalAbsent + '</td>' +
                '<td style="color:var(--orange);font-weight:700">' + s.totalLate + '</td>' +
                '<td><span class="bdg ' + cls + '">' + rate + '%</span></td>' +
                '</tr>';
        });
        $('#rptBody').html(rows || '<tr><td colspan="5" style="text-align:center;color:var(--g400);">No data.</td></tr>');
        $('#rptLbl').text('Report — ' + students.length + ' students');
    },

    // Overview bars
    _renderOverview: function (students) {
        var tP = 0, tA = 0, tL = 0;
        $.each(students, function (i, s) { tP += s.totalPresent; tA += s.totalAbsent; tL += s.totalLate; });
        var gt = tP + tA + tL;
        var avg = gt > 0 ? Math.round(tP / gt * 100) : 0;
        var rc = avg >= 90 ? 'var(--green)' : avg >= 75 ? 'var(--orange)' : 'var(--red)';
        var pct = function (n) { return gt > 0 ? Math.round(n / gt * 100) : 0; };

        $('#rptOv').html('<div style="display:flex;flex-direction:column;gap:9px;">' +
            RptController._bar('Avg. Rate', avg + '%', rc, avg) +
            RptController._bar('Total Present', tP, 'var(--green)', pct(tP)) +
            RptController._bar('Total Absent', tA, 'var(--red)', pct(tA)) +
            RptController._bar('Total Late', tL, 'var(--orange)', pct(tL)) +
            '</div>');
    },

    // At-risk students
    _renderAtRisk: function (students) {
        var atRisk = $.grep(students, function (s) {
            var total = s.totalPresent + s.totalAbsent + s.totalLate;
            return total > 0 && Math.round(s.totalPresent / total * 100) < 75;
        });

        var html = '';
        $.each(atRisk, function (i, s) {
            var total = s.totalPresent + s.totalAbsent + s.totalLate;
            var rate = total > 0 ? Math.round(s.totalPresent / total * 100) : 0;
            html += '<div class="al-item al-danger"><div class="al-ico">⚠️</div>' +
                '<div class="al-t"><strong>' + s.firstName + ' ' + s.lastName + '</strong>' +
                '<span>' + rate + '% · ' + s.totalAbsent + ' absent, ' + s.totalLate + ' late</span></div></div>';
        });
        $('#rptAR').html(html || '<p style="font-size:11px;color:var(--green);font-weight:600;">✓ No students at risk!</p>');
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
        var csv = 'ID,Name,Section,Present,Absent,Late,Rate\n';
        $.each(RptController._students, function (i, s) {
            var total = s.totalPresent + s.totalAbsent + s.totalLate;
            var rate = total > 0 ? Math.round(s.totalPresent / total * 100) : 100;
            csv += (s.studentNumber || s.id) + ',"' + s.firstName + ' ' + s.lastName + '",' +
                (s.sectionName || '') + ',' + s.totalPresent + ',' + s.totalAbsent + ',' + s.totalLate + ',' + rate + '%\n';
        });
        var $a = $('<a>').attr('href', 'data:text/csv;charset=utf-8,' + encodeURIComponent(csv))
            .attr('download', 'attendance_report.csv');
        $('body').append($a);
        $a[0].click();
        $a.remove();
        Helpers.toast('CSV exported!', 'ok');
    },

    _initials: function (name) {
        return $.map((name || '').split(' '), function (n) { return n ? n[0].toUpperCase() : null; }).slice(0, 2).join('');
    },

    _color: function (id) {
        var colors = ['#4a6fa5', '#3a7d5c', '#7b5ea7', '#b06040', '#4a8a9a', '#5a7a3a', '#6a4a8a', '#8a3a3a', '#3a7a7a', '#7a6a2a'];
        return colors[id % colors.length];
    }
};
