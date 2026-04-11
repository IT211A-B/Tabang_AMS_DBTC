var CourseController = {

    render: function (list) {
        list = list || COURSES;
        var html = '';
        $.each(list, function (i, c) {
            var enrolled = $.grep(STUDENTS, function (s) { return s.courseId === c.id; });
            var tName = '—';
            $.each(TEACHERS, function (j, t) { if (t.id === c.teacherId) { tName = t.name; return false; } });

            var avatars = '';
            $.each(enrolled.slice(0, 5), function (j, s) {
                avatars += '<div class="stu-av" style="width:26px;height:26px;font-size:10px;background:' +
                    s.color + ';margin-left:-6px;border:2px solid var(--white);">' + s.initials() + '</div>';
            });
            if (enrolled.length > 5) {
                avatars += '<div class="stu-av" style="width:26px;height:26px;font-size:9px;background:var(--g300);' +
                    'color:var(--g700);margin-left:-6px;border:2px solid var(--white);">+' + (enrolled.length - 5) + '</div>';
            }

            html += '<div class="stu-card">' +
                '<div style="display:flex;align-items:center;gap:10px;margin-bottom:10px;">' +
                '<div style="width:40px;height:40px;border-radius:10px;background:' + c.color + ';display:flex;align-items:center;justify-content:center;font-size:18px;flex-shrink:0;"></div>' +
                '<div><div class="stu-nm">' + c.name + '</div><div class="stu-sid">' + c.code + '</div></div>' +
                '</div>' +
                '<div style="font-size:11px;color:var(--g500);margin-bottom:6px;"> ' + tName + '</div>' +
                '<div style="font-size:11px;color:var(--g400);margin-bottom:10px;">' + c.description + '</div>' +
                '<div style="display:flex;align-items:center;gap:6px;margin-bottom:10px;">' +
                (avatars || '<span style="font-size:11px;color:var(--g400);">No students yet</span>') +
                '</div>' +
                '<div class="stu-stats" style="margin-bottom:10px;">' +
                '<div class="stu-s"><div class="stu-sv" style="color:var(--blue)">' + enrolled.length + '</div><div class="stu-sl">Enrolled</div></div>' +
                '</div>' +
                '<div class="stu-actions">' +
                '<button class="btn btn-p btn-xs" style="flex:1" onclick="CourseController.viewStudents(\'' + c.id + '\')">View Students</button>' +
                '<button class="btn btn-o btn-xs" onclick="CourseController.openEdit(\'' + c.id + '\')"></button>' +
                '<button class="btn btn-d btn-xs" onclick="CourseController.remove(\'' + c.id + '\')"></button>' +
                '</div>' +
                '</div>';
        });
        if (!html) html = '<p style="color:var(--g400);font-size:12px;">No courses found.</p>';
        $('#courseGrid').html(html);
    },

    filter: function (q) {
        q = $.trim(q).toLowerCase();
        var list = !q ? COURSES : $.grep(COURSES, function (c) {
            return c.name.toLowerCase().indexOf(q) > -1 || c.code.toLowerCase().indexOf(q) > -1;
        });
        CourseController.render(list);
    },

    openAdd: function () {
        $('#cAddName, #cAddCode, #cAddDesc').val('');
        var opts = '<option value="">— Assign Teacher —</option>';
        $.each(TEACHERS, function (i, t) {
            opts += '<option value="' + t.id + '">' + t.name + ' (' + t.subject + ')</option>';
        });
        $('#cAddTeacher').html(opts);
        $('#addCourseOverlay').addClass('show');
    },

    add: function () {
        var name = $.trim($('#cAddName').val());
        var code = $.trim($('#cAddCode').val()).toUpperCase();
        var desc = $.trim($('#cAddDesc').val());
        var teacherId = $('#cAddTeacher').val();
        if (!name || !code) { Helpers.toast('Name and code required.', 'err'); return; }
        var id = 'C' + ('00' + (COURSES.length + 1)).slice(-3);
        var color = COLORS[COURSES.length % COLORS.length];
        COURSES.push(new CourseModel({ id: id, name: name, code: code, description: desc, teacherId: teacherId, color: color }));
        if (teacherId) {
            $.each(TEACHERS, function (i, t) {
                if (t.id === teacherId) { t.courseIds.push(id); return false; }
            });
        }
        CourseController.closeModal('addCourseOverlay');
        CourseController.render();
        Helpers.toast(name + ' added!', 'ok');
    },

    openEdit: function (id) {
        var c = null;
        $.each(COURSES, function (i, x) { if (x.id === id) { c = x; return false; } });
        if (!c) return;
        $('#cEditId').val(c.id);
        $('#cEditName').val(c.name);
        $('#cEditCode').val(c.code);
        $('#cEditDesc').val(c.description);
        var opts = '<option value="">— Assign Teacher —</option>';
        $.each(TEACHERS, function (i, t) {
            opts += '<option value="' + t.id + '"' + (t.id === c.teacherId ? ' selected' : '') + '>' + t.name + '</option>';
        });
        $('#cEditTeacher').html(opts);
        $('#editCourseOverlay').addClass('show');
    },

    saveEdit: function () {
        var id = $('#cEditId').val();
        var name = $.trim($('#cEditName').val());
        var code = $.trim($('#cEditCode').val()).toUpperCase();
        var desc = $.trim($('#cEditDesc').val());
        var tid = $('#cEditTeacher').val();
        if (!name || !code) { Helpers.toast('Name and code required.', 'err'); return; }
        $.each(COURSES, function (i, c) {
            if (c.id === id) {
                c.name = name; c.code = code; c.description = desc; c.teacherId = tid;
                return false;
            }
        });
        CourseController.closeModal('editCourseOverlay');
        CourseController.render();
        Helpers.toast('Course updated!', 'ok');
    },

    viewStudents: function (courseId) {
        var c = null;
        $.each(COURSES, function (i, x) { if (x.id === courseId) { c = x; return false; } });
        if (!c) return;
        var enrolled = $.grep(STUDENTS, function (s) { return s.courseId === courseId; });
        $('#csCourseName').text(c.name + ' (' + c.code + ')');
        $('#csCourseId').val(courseId);
        var html = '';
        $.each(enrolled, function (i, s) {
            html += '<div style="display:flex;align-items:center;gap:10px;padding:8px 0;border-bottom:1px solid var(--g100);">' +
                '<div class="s-av" style="background:' + s.color + ';flex-shrink:0;">' + s.initials() + '</div>' +
                '<div style="flex:1;"><div style="font-weight:600;font-size:13px;">' + s.name + '</div>' +
                '<div style="font-size:11px;color:var(--g400);">' + s.id + '</div></div>' +
                '<span class="bdg" style="background:' + s.rateColor() + ';color:#fff;font-size:10px;">' + s.rate() + '%</span>' +
                '<button class="btn btn-d btn-xs" onclick="CourseController.unenrollStudent(\'' + s.id + '\',\'' + courseId + '\')">Unenroll</button>' +
                '</div>';
        });
        if (!html) html = '<p style="font-size:12px;color:var(--g400);padding:12px 0;">No students enrolled.</p>';
        $('#csCourseStudents').html(html);
        $('#courseStudentsOverlay').addClass('show');
    },

    unenrollStudent: function (studentId, courseId) {
        var s = null, cName = '';
        $.each(STUDENTS, function (i, x) { if (x.id === studentId) { s = x; return false; } });
        $.each(COURSES, function (i, c) { if (c.id === courseId) { cName = c.name; return false; } });
        if (!s || !confirm('Unenroll ' + s.name + ' from ' + cName + '?')) return;
        s.courseId = null;
        CourseController.viewStudents(courseId);
        CourseController.render();
        Helpers.toast(s.name + ' unenrolled.', 'ok');
    },

    remove: function (id) {
        var c = null;
        $.each(COURSES, function (i, x) { if (x.id === id) { c = x; return false; } });
        if (!c || !confirm('Remove ' + c.name + '? Students will be unenrolled.')) return;
        $.each(STUDENTS, function (i, s) { if (s.courseId === id) s.courseId = null; });
        $.each(TEACHERS, function (i, t) {
            t.courseIds = $.grep(t.courseIds, function (cid) { return cid !== id; });
        });
        COURSES = $.grep(COURSES, function (x) { return x.id !== id; });
        CourseController.render();
        Helpers.toast(c.name + ' removed.');
    },

    closeModal: function (id) { $('#' + id).removeClass('show'); }
};
