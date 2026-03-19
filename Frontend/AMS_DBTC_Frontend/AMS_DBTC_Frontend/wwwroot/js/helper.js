var Helpers = {

    // ── Format date 
    formatDate: function (date) {
        date = date || new Date();
        return date.toLocaleDateString('en-US', {
            weekday: 'long', year: 'numeric', month: 'long', day: 'numeric'
        });
    },

    // ── Format time 
    formatTime: function (date) {
        date = date || new Date();
        return date.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' });
    },

    // ── Get initials from full name 
    initials: function (name) {
        return $.map(name.split(' '), function (n) {
            return n ? n[0].toUpperCase() : null;
        }).slice(0, 2).join('');
    },

    // ── Show toast notification 
    toast: function (msg, type) {
        type = type || '';
        var $t = $('#toast');
        $t.text(msg)
            .css('background', type === 'err' ? 'var(--red)' : 'var(--g800)')
            .addClass('show');
        setTimeout(function () {
            $t.removeClass('show');
        }, 2500);
    },

    // ── Validate email 
    isValidEmail: function (email) {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    },

    // ── Rate color 
    rateColor: function (rate) {
        if (rate >= 90) return 'var(--green)';
        if (rate >= 75) return 'var(--orange)';
        return 'var(--red)';
    },

    // ── Export CSV 
    exportCSV: function (data, filename) {
        filename = filename || 'attendance.csv';
        var csv = 'ID,Name,Present,Absent,Late,Rate\n';
        $.each(data, function (i, s) {
            csv += s.id + ',"' + s.name + '",' + s.p + ',' + s.a + ',' + s.l + ',' + s.rate() + '%\n';
        });
        var $a = $('<a>')
            .attr('href', 'data:text/csv;charset=utf-8,' + encodeURIComponent(csv))
            .attr('download', filename);
        $('body').append($a);
        $a[0].click();
        $a.remove();
    },

    // ── Add activity log entry 
    addActivity: function (icon, action, detail) {
        STATE.activityLog.unshift({
            icon: icon,
            action: action,
            detail: detail,
            time: Helpers.formatTime()
        });
        if (STATE.activityLog.length > 20) {
            STATE.activityLog.pop();
        }
    },

    // ── Pad number to 2 digits 
    pad: function (n) {
        return n < 10 ? '0' + n : '' + n;
    }
};
