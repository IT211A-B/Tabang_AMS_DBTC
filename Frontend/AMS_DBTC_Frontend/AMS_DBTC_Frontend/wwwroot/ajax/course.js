var couController = {

    _data: [],
    _page: 1,
    _perPage: 12,
    _total: 0,

    // Load all sections 
    load: function () {
        $('#sectionGrid').html('<p style="color:var(--g400);font-size:12px;">Loading sections...</p>');

        var url = '/sections?pageNumber=' + SecController._page + '&pageSize=' + SecController._perPage;
        Api.get(url, function (data) {
            SecController._data = data.data || data;
            SecController._total = data.totalRecords || SecController._data.length;
            SecController.render(SecController._data);
            SecController._renderPagination();
        }, function (err) {
            $('#sectionGrid').html('<p style="color:var(--red);font-size:12px;">Failed to load sections: ' + err + '</p>');
        });
    },

    // Render section cards 
    render: function (list) {
        if (!list || !list.length) {
            $('#sectionGrid').html('<p style="color:var(--g400);font-size:12px;">No sections found.</p>');
            return;
        }

        var html = '';
        $.each(list, function (i, sec) {
            var color = SecController._color(sec.id);

            html += '<div class="stu-card">' +
                '<div style="display:flex;align-items:center;gap:10px;margin-bottom:10px;">' +
                '<div style="width:40px;height:40px;border-radius:10px;background:' + color + ';display:flex;align-items:center;justify-content:center;font-size:18px;flex-shrink:0;">🏫</div>' +
                '<div>' +
                '<div class="stu-nm">' + sec.name + '</div>' +
                '<div class="stu-sid">' + sec.schoolYear + ' · ' + sec.semester + '</div>' +
                '</div>' +
                '</div>' +
                '<div style="font-size:11px;color:var(--g500);margin-bottom:8px;">👨‍🏫 ' + (sec.teacherName || '—') + '</div>' +
                '<div class="stu-stats" style="margin-bottom:10px;">' +
                '<div class="stu-s"><div class="stu-sv" style="color:var(--blue)">' + (sec.studentCount || 0) + '</div><div class="stu-sl">Students</div></div>' +
                '</div>' +
                '<div class="stu-actions">' +
                '<button class="btn btn-p btn-xs" style="flex:1" onclick="SecController.viewStudents(' + sec.id + ')">View Students</button>' +
                '<button class="btn btn-o btn-xs" onclick="SecController.openEdit(' + sec.id + ')">✏️</button>' +
                '<button class="btn btn-d btn-xs" onclick="SecController.remove(' + sec.id + ')">✕</button>' +
                '</div>' +
                '</div>';
        });

        $('#sectionGrid').html(html);
    },

    //  Pagination 
    _renderPagination: function () {
        var totalPages = Math.ceil(SecController._total / SecController._perPage);
        if (totalPages <= 1) { $('#sectionPager').html(''); return; }

        var html = '<div style="display:flex;gap:6px;margin-top:12px;justify-content:center;">';
        for (var i = 1; i <= totalPages; i++) {
            var active = i === SecController._page ? 'btn-p' : 'btn-o';
            html += '<button class="btn ' + active + ' btn-xs" onclick="SecController.goPage(' + i + ')">' + i + '</button>';
        }
        html += '</div>';
        $('#sectionPager').html(html);
    },

    goPage: function (page) {
        SecController._page = page;
        SecController.load();
    },

    // View students in section 
    viewStudents: function (sectionId) {
        Api.get('/sections/' + sectionId + '/students', function (sec) {
            $('#csCourseName').text(sec.name + ' (' + sec.schoolYear + ')');
            $('#csCourseId').val(sectionId);

            var students = sec.students || [];
            var html = '';
            $.each(students, function (i, s) {
                var name = s.firstName + ' ' + s.lastName;
                var color = SecController._color(s.id);
                var init = SecController._initials(name);
                html += '<div style="display:flex;align-items:center;gap:10px;padding:8px 0;border-bottom:1px solid var(--g100);">' +
                    '<div class="s-av" style="background:' + color + ';flex-shrink:0;">' + init + '</div>' +
                    '<div style="flex:1;">' +
                    '<div style="font-weight:600;font-size:13px;">' + name + '</div>' +
                    '<div style="font-size:11px;color:var(--g400);">' + (s.studentNumber || 'No ID') + '</div>' +
                    '</div>' +
                    '<button class="btn btn-d btn-xs" onclick="SecController.removeStudent(' + s.id + ',' + sectionId + ')">Remove</button>' +
                    '</div>';
            });

            if (!html) html = '<p style="font-size:12px;color:var(--g400);padding:12px 0;">No students in this section.</p>';
            $('#csCourseStudents').html(html);
            $('#courseStudentsOverlay').addClass('show');
        });
    },

    // Remove student from section (by deleting student)
    removeStudent: function (studentId, sectionId) {
        if (!confirm('Remove this student from the section?')) return;
        Api.del('/students/' + studentId, function () {
            SecController.viewStudents(sectionId);
            SecController.load();
            Helpers.toast('Student removed from section.');
        });
    },

    // Open Add modal 
    openAdd: function () {
        $('#secAddName, #secAddYear, #secAddSem').val('');
        // Load teachers into dropdown
        Api.get('/users/role/Teacher', function (users) {
            var opts = '<option value="">— Assign Teacher —</option>';
            $.each(users, function (i, u) {
                opts += '<option value="' + u.id + '">' + u.firstName + ' ' + u.lastName + '</option>';
            });
            $('#secAddTeacher').html(opts);
        });
        $('#addSectionOverlay').addClass('show');
    },

    //  Add section 
    add: function () {
        var name = $.trim($('#secAddName').val());
        var year = $.trim($('#secAddYear').val());
        var sem = $('#secAddSem').val();
        var teacherId = parseInt($('#secAddTeacher').val());

        if (!name || !year || !sem) { Helpers.toast('Fill in all fields.', 'err'); return; }
        if (!teacherId) { Helpers.toast('Please assign a teacher.', 'err'); return; }

        var body = { name: name, schoolYear: year, semester: sem, userId: teacherId };

        Api.post('/sections', body, function () {
            SecController.closeModal('addSectionOverlay');
            SecController.load();
            Helpers.toast(name + ' section added!', 'ok');
            Helpers.addActivity('🏫', 'Section added', name);
        });
    },

    //  Open Edit modal 
    openEdit: function (id) {
        Api.get('/sections/' + id, function (sec) {
            $('#secEditId').val(sec.id);
            $('#secEditName').val(sec.name);
            $('#secEditYear').val(sec.schoolYear);
            $('#secEditSem').val(sec.semester);

            Api.get('/users/role/Teacher', function (users) {
                var opts = '';
                $.each(users, function (i, u) {
                    opts += '<option value="' + u.id + '"' + (u.id === sec.userId ? ' selected' : '') + '>' +
                        u.firstName + ' ' + u.lastName + '</option>';
                });
                $('#secEditTeacher').html(opts);
            });

            $('#editSectionOverlay').addClass('show');
        });
    },

    //  Save edit 
    saveEdit: function () {
        var id = parseInt($('#secEditId').val());
        var name = $.trim($('#secEditName').val());
        var year = $.trim($('#secEditYear').val());
        var sem = $('#secEditSem').val();
        var teacherId = parseInt($('#secEditTeacher').val());

        if (!name || !year || !sem) { Helpers.toast('Fill in all fields.', 'err'); return; }

        var body = { name: name, schoolYear: year, semester: sem, userId: teacherId || null };

        Api.put('/sections/' + id, body, function () {
            SecController.closeModal('editSectionOverlay');
            SecController.load();
            Helpers.toast(name + ' updated!', 'ok');
            Helpers.addActivity('✏️', 'Section updated', name);
        });
    },

    //  Delete section 
    remove: function (id) {
        var sec = null;
        $.each(SecController._data, function (i, x) { if (x.id === id) { sec = x; return false; } });
        var name = sec ? sec.name : 'this section';
        if (!confirm('Delete ' + name + '? This will remove all enrolled students and attendance records.')) return;

        Api.del('/sections/' + id, function () {
            SecController.load();
            Helpers.toast(name + ' deleted.');
            Helpers.addActivity('🗑️', 'Section deleted', name);
        });
    },

    closeModal: function (id) { $('#' + id).removeClass('show'); },

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
