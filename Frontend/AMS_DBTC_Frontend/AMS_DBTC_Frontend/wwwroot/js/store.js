// State management store
window.Store = {
    // Application state
    state: {
        currentPage: null,
        isLoading: false,
        user: null,
        theme: 'light',
        notifications: []
    },

    // Get state value
    get: function (key) {
        try {
            return this.state[key];
        } catch (error) {
            console.error('Store get error:', error);
            return undefined;
        }
    },

    // Set state value
    set: function (key, value) {
        try {
            this.state[key] = value;
            this.trigger('change', { key: key, value: value });
        } catch (error) {
            console.error('Store set error:', error);
        }
    },

    // Event listeners
    listeners: {},

    // Subscribe to state changes
    on: function (event, callback) {
        if (typeof callback !== 'function') {
            console.error('Callback must be a function');
            return;
        }

        if (!this.listeners[event]) {
            this.listeners[event] = [];
        }

        this.listeners[event].push(callback);
    },

    // Trigger event
    trigger: function (event, data) {
        try {
            if (this.listeners[event]) {
                this.listeners[event].forEach(callback => {
                    try {
                        callback(data);
                    } catch (error) {
                        console.error('Event callback error:', error);
                    }
                });
            }
        } catch (error) {
            console.error('Trigger error:', error);
        }
    },

    // Initialize store
    init: function () {
        try {
            // Load saved state from localStorage if available
            const savedState = localStorage.getItem('eduAttendState');
            if (savedState) {
                const parsed = JSON.parse(savedState);
                this.state = { ...this.state, ...parsed };
            }

            // Save state changes to localStorage
            this.on('change', (data) => {
                try {
                    localStorage.setItem('eduAttendState', JSON.stringify(this.state));
                } catch (error) {
                    console.error('LocalStorage save error:', error);
                }
            });

            console.log('Store initialized successfully');
        } catch (error) {
            console.error('Store initialization error:', error);
        }
    }
};