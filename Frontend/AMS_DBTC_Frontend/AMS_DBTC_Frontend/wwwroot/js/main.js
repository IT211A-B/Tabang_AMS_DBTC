
$(document).ready(function () {

    // ── Set topbar date ───────────────────────────────────────
    $('#topDate').text(Helpers.formatDate());

    // ── Seed activity log if empty ────────────────────────────
    if (STATE.activityLog.length === 0) {
        Helpers.addActivity('🔑', 'System started', 'EduAttend AMS loaded');
        Helpers.addActivity('📋', 'Attendance ready', "Waiting for today's entries");
    }

    // ── ESC key closes any open overlay ──────────────────────
    $(document).on('keydown', function (e) {
        if (e.key === 'Escape') {
            $('.overlay.show').removeClass('show');
        }
    });

    // ── Click outside modal closes it ────────────────────────
    $(document).on('click', '.overlay', function (e) {
        if ($(e.target).hasClass('overlay')) {
            $(this).removeClass('show');
        }
    });

});
