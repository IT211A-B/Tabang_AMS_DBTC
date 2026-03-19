var COLORS = [
    '#4a6fa5', '#3a7d5c', '#7b5ea7', '#b06040', '#4a8a9a',
    '#5a7a3a', '#6a4a8a', '#8a3a3a', '#3a7a7a', '#7a6a2a',
    '#3a5a8a', '#8a6a2a'
];

// ── Seed Students ─────────────────────────────────────────────
var STUDENTS = [
    new StudentModel({ id: 'S001', name: 'Aaliyah Santos', color: '#4a6fa5', p: 18, a: 0, l: 2 }),
    new StudentModel({ id: 'S002', name: 'Benjamin Cruz', color: '#3a7d5c', p: 16, a: 2, l: 2 }),
    new StudentModel({ id: 'S003', name: 'Clarissa Mendoza', color: '#7b5ea7', p: 19, a: 0, l: 1 }),
    new StudentModel({ id: 'S004', name: 'Daniel Flores', color: '#b06040', p: 12, a: 6, l: 2 }),
    new StudentModel({ id: 'S005', name: 'Elaine Ramos', color: '#4a8a9a', p: 17, a: 1, l: 2 }),
    new StudentModel({ id: 'S006', name: 'Francis Lim', color: '#5a7a3a', p: 16, a: 2, l: 2 }),
    new StudentModel({ id: 'S007', name: 'Grace Torres', color: '#6a4a8a', p: 19, a: 0, l: 1 }),
    new StudentModel({ id: 'S008', name: 'Hector Villanueva', color: '#8a3a3a', p: 11, a: 5, l: 4 }),
    new StudentModel({ id: 'S009', name: 'Isabella Reyes', color: '#3a7a7a', p: 18, a: 0, l: 2 }),
    new StudentModel({ id: 'S010', name: 'Jose Miguel Delos Santos', color: '#7a6a2a', p: 15, a: 3, l: 2 }),
    new StudentModel({ id: 'S011', name: 'Karen Manalo', color: '#3a5a8a', p: 19, a: 0, l: 1 }),
    new StudentModel({ id: 'S012', name: 'Luis Alfonso Garcia', color: '#8a6a2a', p: 14, a: 3, l: 3 }),
];

// ── Seed Users ────────────────────────────────────────────────
var USERS = [
    new UserModel({ name: 'Ms. Janet Reyes', email: 'teacher@school.edu.ph', pass: 'teacher123', role: 'teacher', section: 'Grade 10 - Section B' }),
    new UserModel({ name: 'Dr. Ana Reyes', email: 'admin@school.edu.ph', pass: 'admin123', role: 'admin', section: 'All Sections' }),
];

// ── Session Helpers ───────────────────────────────────────────
var Session = {

    save: function (user, role) {
        sessionStorage.setItem('ams_user', JSON.stringify({
            name: user.name,
            email: user.email,
            pass: user.pass,
            role: role,
            section: user.section
        }));
        sessionStorage.setItem('ams_role', role);
    },

    load: function () {
        try {
            var raw = sessionStorage.getItem('ams_user');
            if (!raw) return null;
            return new UserModel(JSON.parse(raw));
        } catch (e) {
            return null;
        }
    },

    loadRole: function () {
        return sessionStorage.getItem('ams_role') || 'teacher';
    },

    clear: function () {
        sessionStorage.removeItem('ams_user');
        sessionStorage.removeItem('ams_role');
    },

    isLoggedIn: function () {
        return !!sessionStorage.getItem('ams_user');
    }
};

// ── App State ─────────────────────────────────────────────────
var STATE = {
    currentUser: Session.load(),
    currentRole: Session.loadRole(),
    attendance: {},
    activityLog: [],
    currentPage: 'dash',
    weeklyData: [
        { day: 'Mon', p: 82, a: 10, l: 8 },
        { day: 'Tue', p: 88, a: 7, l: 5 },
        { day: 'Wed', p: 79, a: 13, l: 8 },
        { day: 'Thu', p: 86, a: 9, l: 5 },
        { day: 'Fri', p: 84, a: 11, l: 5 },
    ]
};
