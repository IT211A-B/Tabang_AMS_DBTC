var RptController = {

    render: function () {
        var period = $('#rptP').val() || 'month';
        var labels = { month: 'This Month', week: 'This Week', all: 'All Time' };
        $('#rptLbl').text(labels[period] || 'This Month');

        var tP = 0, tA = 0, tL = 0, rows = '';
        $.each(STUDENTS, function (i, s) {
            tP += s.p; tA += s.a; tL += s.l;
            var cls = s.rate() >= 90 ? 'bdg-p' : s.rate() >= 75 ? 'bdg-b' : 'bdg-a';
            rows += '<tr>' +
                '<td><div style="display:flex;align-items:center;gap:6px;">' +
                '<div class="s-av" style="background:' + s.color + ';width:22px;height:22px;font-size:9px;">' + s.initials() + '</div>' + s.name +
                '</div></td>' +
                '<td style="color:var(--green);font-weight:700">' + s.p + '</td>' +
                '<td style="color:var(--red);font-weight:700">' + s.a + '</td>' +
                '<td style="color:var(--orange);font-weight:700">' + s.l + '</td>' +
                '<td><span class="bdg ' + cls + '">' + s.rate() + '%</span></td>' +
                '</tr>';
        });
        $('#rptBody').html(rows);

        var gt = tP + tA + tL;
        var avg = gt > 0 ? Math.round(tP / gt * 100) : 0;
        var rc = Helpers.rateColor(avg);
        var pct = function (n) { return gt > 0 ? Math.round(n / gt * 100) : 0; };

        $('#rptOv').html('<div style="display:flex;flex-direction:column;gap:9px;">' +
            RptController._bar('Avg. Rate', avg + '%', rc, avg) +
            RptController._bar('Total Present', tP, 'var(--green)', pct(tP)) +
            RptController._bar('Total Absent', tA, 'var(--red)', pct(tA)) +
            RptController._bar('Total Late', tL, 'var(--orange)', pct(tL)) +
            '</div>');

        var atRisk = $.grep(STUDENTS, function (s) { return s.isAtRisk(); });
        var arHtml = '';
        $.each(atRisk, function (i, s) {
            arHtml += '<div class="al-item al-danger"><div class="al-ico"></div>' +
                '<div class="al-t"><strong>' + s.name + '</strong><span>' + s.rate() + '% · ' + s.a + ' absent, ' + s.l + ' late</span></div></div>';
        });
        $('#rptAR').html(arHtml || '<p style="font-size:11px;color:var(--green);font-weight:600;"> No students at risk!</p>');
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
        Helpers.toast('CSV exported!', 'ok');
    }
};
