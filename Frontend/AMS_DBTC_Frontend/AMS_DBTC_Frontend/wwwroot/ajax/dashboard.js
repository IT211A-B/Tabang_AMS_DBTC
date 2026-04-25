var DashController = {

    _sections: [],

    //  Load dashboard data
    load: function () {
        var today = new Date().toISOString().split('T')[0];

        // Load all students for total count
        Api.get('/students', function (students) {
            $('#dT').text(students.length);

            // Count stats from attendance records across all sections
            var p = 0, a = 0, l = 0;
            $.each(students, function (i, s) {
                p += s.totalPresent || 0;
                a += s.totalAbsent || 0;
                l += s.totalLate || 0;
            });

            var total = students.length;
            $('#dP').text(p);
            $('#dA').text(a);
            $('#dL').text(l);
            $('#dPpct').text(total ? Math.round(p / (p + a + l || 1) * 100) + '% of class' : '—');
            $('#dApct').text(total ? Math.round(a / (p + a + l || 1) * 100) + '% of class' : '—');
            $('#dLpct').text(total ? Math.round(l / (p + a + l || 1) * 100) + '% of class' : '—');
            $('#gP').text(p);
            $('#gAL').text(a + l);

            // At-risk students
            var atRisk = $.grep(students, function (s) {
                var tot = s.totalPresent + s.totalAbsent + s.totalLate;
                return tot > 0 && Math.round(s.totalPresent / tot * 100) < 75;
            });

            var alertHtml = '';
            $.each(atRisk.slice(0, 5), function (i, s) {
                var tot = s.totalPresent + s.totalAbsent + s.totalLate;
                var rate = tot > 0 ? Math.round(s.totalPresent / tot * 100) : 0;
                alertHtml += '<div class="al-item al-danger">' +
                    '<div class="al-ico">⚠️</div>' +
                    '<div class="al-t"><strong>' + s.firstName + ' ' + s.lastName + '</strong>' +
                    '<span>' + rate + '% rate · ' + s.totalAbsent + ' absences</span></div>' +
                    '</div>';
            });
            if (!alertHtml) alertHtml = '<div class="al-item al-success"><div class="al-ico">✅</div>' +
                '<div class="al-t"><strong>All good!</strong><span>No attendance alerts.</span></div></div>';
            $('#dashAlerts').html(alertHtml);

            // Rate glance
            var allTot = p + a + l;
            $('#gRate').text(allTot > 0 ? Math.round(p / allTot * 100) + '%' : '—');
        });

        // Load sections for the chart
        Api.get('/sections?pageSize=100', function (data) {
            DashController._sections = data.data || data;
            DashController._renderSectionStats();
        });

        // Activity log
        DashController._renderActivity();
    },

    //  Section stats bar chart 
    _renderSectionStats: function () {
        if (!DashController._sections.length) return;

        var bars = '';
        var labels = '';
        var max = 0;

        // Find max student count for scaling
        $.each(DashController._sections, function (i, sec) {
            if ((sec.studentCount || 0) > max) max = sec.studentCount;
        });

        $.each(DashController._sections.slice(0, 5), function (i, sec) {
            var pct = max > 0 ? Math.round((sec.studentCount || 0) / max * 100) : 0;
            bars += '<div class="bc-g"><div class="bc-b" style="background:var(--blue);height:' + pct + '%"></div></div>';
            labels += '<span class="bc-lbl">' + (sec.name.length > 8 ? sec.name.substring(0, 8) + '…' : sec.name) + '</span>';
        });

        $('#barchart').html(bars);
        $('#barlabels').html(labels);
    },

    // Activity log 
    _renderActivity: function () {
        if (!STATE.activityLog.length) {
            $('#actLog').html('<p style="font-size:11px;color:var(--g400);">No recent activity.</p>');
            return;
        }
        var html = '';
        $.each(STATE.activityLog.slice(0, 8), function (i, entry) {
            html += '<div class="act-row">' +
                '<div class="act-ic" style="background:var(--blue-light)">' + entry.icon + '</div>' +
                '<div class="act-txt"><strong>' + entry.action + '</strong><span>' + entry.detail + '</span></div>' +
                '<div class="act-time">' + entry.time + '</div>' +
                '</div>';
        });
        $('#actLog').html(html);
    },

    //Refresh activity (called after any action) 
    refreshActivity: function () {
        DashController._renderActivity();
    }
};
