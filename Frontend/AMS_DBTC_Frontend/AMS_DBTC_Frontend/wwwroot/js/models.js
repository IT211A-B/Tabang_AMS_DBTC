// Student Model 
function StudentModel(data) {
    this.id = data.id || '';
    this.name = data.name || '';
    this.color = data.color || '#4a6fa5';
    this.p = data.p || 0;
    this.a = data.a || 0;
    this.l = data.l || 0;
    this.courseId = data.courseId || null;
}
StudentModel.prototype.total = function () { return this.p + this.a + this.l; };
StudentModel.prototype.rate = function () {
    return this.total() > 0 ? Math.round(this.p / this.total() * 100) : 100;
};
StudentModel.prototype.initials = function () {
    return $.map(this.name.split(' '), function (n) {
        return n ? n[0].toUpperCase() : null;
    }).slice(0, 2).join('');
};
StudentModel.prototype.rateColor = function () {
    if (this.rate() >= 90) return 'var(--green)';
    if (this.rate() >= 75) return 'var(--orange)';
    return 'var(--red)';
};
StudentModel.prototype.isAtRisk = function () {
    return this.total() > 0 && this.rate() < 75;
};

// Teacher Model
function TeacherModel(data) {
    this.id = data.id || '';
    this.name = data.name || '';
    this.email = data.email || '';
    this.subject = data.subject || '';
    this.color = data.color || '#4a6fa5';
    this.courseIds = data.courseIds || [];
}
TeacherModel.prototype.initials = function () {
    return $.map(this.name.split(' '), function (n) {
        return n ? n[0].toUpperCase() : null;
    }).slice(0, 2).join('');
};

// Course Model
function CourseModel(data) {
    this.id = data.id || '';
    this.name = data.name || '';
    this.code = data.code || '';
    this.description = data.description || '';
    this.teacherId = data.teacherId || null;
    this.color = data.color || '#4a6fa5';
}

// User Model 
function UserModel(data) {
    this.name = data.name || '';
    this.email = data.email || '';
    this.pass = data.pass || '';
    this.role = data.role || 'teacher';
    this.section = data.section || 'Grade 10 - Section B';
}
UserModel.prototype.initials = function () {
    return $.map(this.name.split(' '), function (n) {
        return n ? n[0].toUpperCase() : null;
    }).slice(0, 2).join('');
};

// Attendance Record 
function AttendanceRecord(studentId, status, remark) {
    this.studentId = studentId;
    this.status = status;
    this.remark = remark || '';
    this.timestamp = new Date().toISOString();
}
AttendanceRecord.prototype.label = function () {
    return { P: 'Present', A: 'Absent', L: 'Late' }[this.status] || '—';
};
AttendanceRecord.prototype.badgeClass = function () {
    return { P: 'bdg-p', A: 'bdg-a', L: 'bdg-l' }[this.status] || 'bdg-n';
};
AttendanceRecord.prototype.markClass = function () {
    return { P: 'p-on', A: 'a-on', L: 'l-on' }[this.status] || '';
};
