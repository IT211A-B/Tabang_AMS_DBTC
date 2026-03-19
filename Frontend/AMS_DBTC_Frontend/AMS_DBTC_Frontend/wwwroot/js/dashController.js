var DashController = {

    render: function () {
        DashController._renderStats();
        DashController._renderChart();
        DashController._renderAlerts();
        DashController._renderActivity();
    },

    // ── Top stat cards ────────────────────────────────────────
    _renderStats: function () {
        var recs = $.map(STATE.attendance, function (r) { return r; });
        var total = STUDENTS.length;
        var p = 0, a = 0, l = 0;

        $.each(recs, function (i, r) {
            if (r.status === 'P') p++;
            else if (r.status === 'A') a++;
            else if (r.status === 'L') l++;
        });

        var rate = total > 0 ? Math.round(p / total * 100) : 0;

        $('#dT').text(total);
        $('#dP').text(p);
        $('#dA').text(a);
        $('#dL').text(l);
        $('#dPpct').text(total ? rate + '% of class' : '—');
        $('#dApct').text(total ? Math.round(a / total * 100) + '% of class' : '—');
        $('#dLpct').text(total ? Math.round(l / total * 100) + '% of class' : '—');

        $('#gRate').text(recs.length ? rate + '%' : '—');
        $('#gP').text(p);
        $('#gAL').text(a + l);
    },

    // ── Weekly bar chart ──────────────────────────────────────
    _renderChart: function () {
        var bars = '';
        var labels = '';

        $.each(STATE.weeklyData, function (i, d) {
            bars += '<div class="bc-g"><div class="bc-b" style="background:var(--green);height:' + d.p + '%"></div></div>' +
                '<div class="bc-g"><div class="bc-b" style="background:var(--red);height:' + d.a + '%"></div></div>' +
                '<div class="bc-g"><div class="bc-b" style="background:var(--orange);height:' + d.l + '%"></div></div>';

            if (i < STATE.weeklyData.length - 1) {
                bars += '<div class="bc-divider"></div>';
            }

            labels += '<span class="bc-lbl">' + d.day + '</span>';
        });

        $('#barchart').html(bars);
        $('#barlabels').html(labels);
    },

    // ── Alerts panel ──────────────────────────────────────────
    _renderAlerts: function () {
        var html = '';

        $.each(STUDENTS, function (i, s) {
            if (s.isAtRisk()) {
                html += '<div class="al-item al-danger">' +
                    '<div class="al-ico">⚠️</div>' +
                    '<div class="al-t">' +
                    '<strong>' + s.name + '</strong>' +
                    '<span>' + s.rate() + '% rate · ' + s.a + ' absences</span>' +
                    '</div>' +
                    '</div>';
            }
        });

        $.each(STUDENTS, function (i, s) {
            if (s.l >= 3 && !s.isAtRisk()) {
                html += '<div class="al-item al-warn">' +
                    '<div class="al-ico">🕐</div>' +
                    '<div class="al-t">' +
                    '<strong>' + s.name + '</strong>' +
                    '<span>Late ' + s.l + 'x this period</span>' +
                    '</div>' +
                    '</div>';
            }
        });

        if (!html) {
            html = '<div class="al-item al-success">' +
                '<div class="al-ico">✅</div>' +
                '<div class="al-t"><strong>All good!</strong><span>No attendance alerts.</span></div>' +
                '</div>';
        }

        $('#dashAlerts').html(html);
    },

    // ── Activity log ──────────────────────────────────────────
    _renderActivity: function () {
        if (!STATE.activityLog.length) {
            $('#actLog').html('<p style="font-size:11px;color:var(--g400);">No recent activity.</p>');
            return;
        }

        var html = '';
        var logs = STATE.activityLog.slice(0, 8);

        $.each(logs, function (i, entry) {
            html += '<div class="act-row">' +
                '<div class="act-ic" style="background:var(--blue-light)">' + entry.icon + '</div>' +
                '<div class="act-txt">' +
                '<strong>' + entry.action + '</strong>' +
                '<span>' + entry.detail + '</span>' +
                '</div>' +
                '<div class="act-time">' + entry.time + '</div>' +
                '</div>';
        });

        $('#actLog').html(html);
    }
};
