// Models namespace
window.Models = {
    // Session model
    Session: {
        user: null,
        isLoggedIn: function () {
            try {
                return this.user !== null &&
                    this.user.id !== undefined &&
                    this.user.id !== null;
            } catch (error) {
                console.error('Session check error:', error);
                return false;
            }
        },
        getUser: function () {
            return this.user;
        },
        setUser: function (userData) {
            this.user = userData;
        },
        logout: function () {
            this.user = null;
            window.location.href = '/Auth/Login';
        }
    },

    // User model
    User: {
        create: function (data) {
            return {
                id: data.id || null,
                username: data.username || '',
                email: data.email || '',
                role: data.role || 'user',
                firstName: data.firstName || '',
                lastName: data.lastName || '',
                fullName: function () {
                    return this.firstName + ' ' + this.lastName;
                }
            };
        }
    },

    // Attendance model
    Attendance: {
        records: [],

        addRecord: function (record) {
            if (!record || typeof record !== 'object') {
                console.error('Invalid attendance record');
                return false;
            }
            this.records.push(record);
            return true;
        },

        getRecords: function () {
            return this.records;
        },

        clear: function () {
            this.records = [];
        }
    }
};