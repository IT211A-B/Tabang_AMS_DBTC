// Authentication Controller
window.AuthController = {
    // Login form handler
    initLogin: function () {
        try {
            const loginForm = document.getElementById('loginForm');
            if (!loginForm) {
                console.warn('Login form not found');
                return;
            }

            loginForm.addEventListener('submit', (event) => {
                event.preventDefault();

                const username = loginForm.querySelector('#username')?.value;
                const password = loginForm.querySelector('#password')?.value;

                if (!username || !password) {
                    Helpers.showToast('Please enter username and password', 'error');
                    return;
                }

                // Disable form during submission
                const submitBtn = loginForm.querySelector('button[type="submit"]');
                if (submitBtn) {
                    submitBtn.disabled = true;
                    submitBtn.textContent = 'Logging in...';
                }

                // Make AJAX request
                Helpers.ajax({
                    method: 'POST',
                    url: '/Auth/Login',
                    data: { username, password },
                    success: (response) => {
                        try {
                            if (response.success) {
                                // Store user session
                                if (response.user) {
                                    Models.Session.setUser(response.user);
                                }
                                Helpers.showToast('Login successful!', 'success');
                                window.location.href = response.redirect || '/Dashboard';
                            } else {
                                Helpers.showToast(response.message || 'Login failed', 'error');
                            }
                        } catch (error) {
                            console.error('Login response error:', error);
                            Helpers.showToast('An error occurred', 'error');
                        }
                    },
                    error: (error) => {
                        console.error('Login error:', error);
                        Helpers.showToast('Connection error. Please try again.', 'error');
                    }
                }).finally(() => {
                    if (submitBtn) {
                        submitBtn.disabled = false;
                        submitBtn.textContent = 'Login';
                    }
                });
            });
        } catch (error) {
            console.error('AuthController init error:', error);
        }
    },

    // Logout handler
    logout: function () {
        try {
            Models.Session.logout();
        } catch (error) {
            console.error('Logout error:', error);
            window.location.href = '/Auth/Login';
        }
    },

    // Initialize authentication
    init: function () {
        try {
            this.initLogin();

            // Setup logout button
            const logoutBtn = document.getElementById('logoutBtn');
            if (logoutBtn) {
                logoutBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.logout();
                });
            }
        } catch (error) {
            console.error('AuthController initialization error:', error);
        }
    }
};