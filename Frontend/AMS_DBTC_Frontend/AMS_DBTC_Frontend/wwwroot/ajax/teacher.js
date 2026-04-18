
var TeacherController = {

    render: function (list) {
        list = list || TEACHERS;
        var html = '';
        $.each(list, function (i, t) {
            var studentCount = 0;
            var courseNames = '';
            $.each(t.courseIds, function (j, cid) {
                $.each(COURSES, function (k, c) {
                    if (c.id === cid) {
                        courseNames += '<span class="bdg bdg-b" style="font-size:10px;margin:1px 2px;">' + c.code + '</span>';
                        return false;
                    }
                });
                $.each(STUDENTS, function (k, s) { if (s.courseId === cid) studentCount++; });
            });
            if (!courseNames) courseNames = '<span style="font-size:11px;color:var(--g400);">No courses assigned</span>';

            html += '<div class="stu-card">' +
                '<div class="stu-top">' +
                '<div class="stu-av" style="background:' + t.color + '">' + t.initials() + '</div>' +
                '<div><div class="stu-nm">' + t.name + '</div><div class="stu-sid">' + t.subject + '</div></div>' +
                '</div>' +
                '<div style="font-size:11px;color:var(--g500);margin:4px 0 6px;">✉️ ' + t.email + '</div>' +
                '<div style="margin-bottom:8px;">' + courseNames + '</div>' +
                '<div class="stu-stats">' +
                '<div class="stu-s"><div class="stu-sv" style="color:var(--blue)">' + t.courseIds.length + '</div><div class="stu-sl">Courses</div></div>' +
                '<div class="stu-s"><div class="stu-sv" style="color:var(--green)">' + studentCount + '</div><div class="stu-sl">Students</div></div>' +
                '</div>' +
                '<div class="stu-actions">' +
                '<button class="btn btn-p btn-xs" style="flex:1" onclick="TeacherController.openEnroll(\'' + t.id + '\')">Enroll Student</button>' +
                '<button class="btn btn-o btn-xs" onclick="TeacherController.openEdit(\'' + t.id + '\')">✏️</button>' +
                '<button class="btn btn-d btn-xs" onclick="TeacherController.remove(\'' + t.id + '\')">✕</button>' +
                '</div>' +
                '</div>';
        });
        if (!html) html = '<p style="color:var(--g400);font-size:12px;">No teachers found.</p>';
        $('#teacherGrid').html(html);
    },

    filter: function (q) {
        q = $.trim(q).toLowerCase();
        var list = !q ? TEACHERS : $.grep(TEACHERS, function (t) {
            return t.name.toLowerCase().indexOf(q) > -1 || t.subject.toLowerCase().indexOf(q) > -1;
        });
        TeacherController.render(list);
    },

    openAdd: function () {
        $('#tAddName, #tAddEmail, #tAddSubject').val('');
        $('#addTeacherOverlay').addClass('show');
    },

    add: function () {
        var name = $.trim($('#tAddName').val());
        var email = $.trim($('#tAddEmail').val());
        var subject = $.trim($('#tAddSubject').val());
        if (!name || !email || !subject) { Helpers.toast('Fill in all fields.', 'err'); return; }
        if (!Helpers.isValidEmail(email)) { Helpers.toast('Invalid email.', 'err'); return; }
        var exists = false;
        $.each(TEACHERS, function (i, t) { if (t.email === email) { exists = true; return false; } });
        if (exists) { Helpers.toast('Email already exists.', 'err'); return; }
        var id = 'T' + ('00' + (TEACHERS.length + 1)).slice(-3);
        var color = COLORS[TEACHERS.length % COLORS.length];
        TEACHERS.push(new TeacherModel({ id: id, name: name, email: email, subject: subject, color: color }));
        TeacherController.closeModal('addTeacherOverlay');
        TeacherController.render();
        Helpers.toast(name + ' added!', 'ok');
    },

    openEdit: function (id) {
        var t = null;
        $.each(TEACHERS, function (i, x) { if (x.id === id) { t = x; return false; } });
        if (!t) return;
        $('#tEditId').val(t.id);
        $('#tEditName').val(t.name);
        $('#tEditEmail').val(t.email);
        $('#tEditSubject').val(t.subject);
        $('#editTeacherOverlay').addClass('show');
    },

    saveEdit: function () {
        var id = $('#tEditId').val();
        var name = $.trim($('#tEditName').val());
        var email = $.trim($('#tEditEmail').val());
        var subject = $.trim($('#tEditSubject').val());
        if (!name || !email || !subject) { Helpers.toast('Fill in all fields.', 'err'); return; }
        $.each(TEACHERS, function (i, t) {
            if (t.id === id) { t.name = name; t.email = email; t.subject = subject; return false; }
        });
        TeacherController.closeModal('editTeacherOverlay');
        TeacherController.render();
        Helpers.toast('Teacher updated!', 'ok');
    },

    openEnroll: function (teacherId) {
        $('#enrollTeacherId').val(teacherId);
        var t = null;
        $.each(TEACHERS, function (i, x) { if (x.id === teacherId) { t = x; return false; } });
        if (!t) return;
        var courseOpts = '<option value="">— Select course —</option>';
        $.each(t.courseIds, function (i, cid) {
            $.each(COURSES, function (j, c) {
                if (c.id === cid) { courseOpts += '<option value="' + c.id + '">' + c.name + '</option>'; return false; }
            });
        });
        $('#enrollCourseId').html(courseOpts);
        var stuOpts = '<option value="">— Select student —</option>';
        $.each(STUDENTS, function (i, s) {
            var inTeacher = false;
            $.each(t.courseIds, function (j, cid) { if (s.courseId === cid) { inTeacher = true; return false; } });
            if (!inTeacher) {
                stuOpts += '<option value="' + s.id + '">' + s.name + ' (' + s.id + ')' +
                    (s.courseId ? ' — in another course' : ' — unenrolled') + '</option>';
            }
        });
        $('#enrollStudentId').html(stuOpts);
        $('#enrollTeacherName').text(t.name);
        $('#enrollStudentOverlay').addClass('show');
    },

    enroll: function () {
        var studentId = $('#enrollStudentId').val();
        var courseId = $('#enrollCourseId').val();
        if (!studentId) { Helpers.toast('Select a student.', 'err'); return; }
        if (!courseId) { Helpers.toast('Select a course.', 'err'); return; }
        var s = null, cName = '';
        $.each(STUDENTS, function (i, x) { if (x.id === studentId) { s = x; return false; } });
        $.each(COURSES, function (i, c) { if (c.id === courseId) { cName = c.name; return false; } });
        if (!s) return;
        s.courseId = courseId;
        TeacherController.closeModal('enrollStudentOverlay');
        TeacherController.render();
        Helpers.toast(s.name + ' enrolled in ' + cName + '!', 'ok');
    },

    unenroll: function (studentId) {
        var s = null;
        $.each(STUDENTS, function (i, x) { if (x.id === studentId) { s = x; return false; } });
        if (!s || !confirm('Unenroll ' + s.name + '?')) return;
        s.courseId = null;
        TeacherController.render();
        Helpers.toast(s.name + ' unenrolled.', 'ok');
    },

    remove: function (id) {
        var t = null;
        $.each(TEACHERS, function (i, x) { if (x.id === id) { t = x; return false; } });
        if (!t || !confirm('Remove ' + t.name + '?')) return;
        TEACHERS = $.grep(TEACHERS, function (x) { return x.id !== id; });
        TeacherController.render();
        Helpers.toast(t.name + ' removed.');
    },

    closeModal: function (id) { $('#' + id).removeClass('show'); }
};
