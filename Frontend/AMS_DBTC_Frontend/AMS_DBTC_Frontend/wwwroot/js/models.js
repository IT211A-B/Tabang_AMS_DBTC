// ── Student Model ─────────────────────────────────────────────
function StudentModel(data) {
    this.id = data.id;
    this.name = data.name;
    this.color = data.color;
    this.p = data.p || 0;
    this.a = data.a || 0;
    this.l = data.l || 0;
}

StudentModel.prototype.total = function () {
    return this.p + this.a + this.l;
};

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


// ── User Model ────────────────────────────────────────────────
function UserModel(data) {
    this.name = data.name;
    this.email = data.email;
    this.pass = data.pass;
    this.role = data.role || 'teacher';
    this.section = data.section || 'Grade 10 - Section B';
}

UserModel.prototype.initials = function () {
    return $.map(this.name.split(' '), function (n) {
        return n ? n[0].toUpperCase() : null;
    }).slice(0, 2).join('');
};

UserModel.prototype.displayRole = function () {
    return this.role === 'admin' ? 'Administrator' : this.section;
};


// ── Attendance Record ─────────────────────────────────────────
function AttendanceRecord(studentId, status, remark) {
    this.studentId = studentId;
    this.status = status;
    this.remark = remark || '';
    this.timestamp = new Date().toISOString();
}

AttendanceRecord.prototype.label = function () {
    var map = { P: 'Present', A: 'Absent', L: 'Late' };
    return map[this.status] || '—';
};

AttendanceRecord.prototype.badgeClass = function () {
    var map = { P: 'bdg-p', A: 'bdg-a', L: 'bdg-l' };
    return map[this.status] || 'bdg-n';
};

AttendanceRecord.prototype.markClass = function () {
    var map = { P: 'p-on', A: 'a-on', L: 'l-on' };
    return map[this.status] || '';
};
