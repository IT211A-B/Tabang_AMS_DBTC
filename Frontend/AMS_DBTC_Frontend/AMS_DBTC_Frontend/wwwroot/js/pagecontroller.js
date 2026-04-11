// Page Controller - Main controller for page-specific logic
/*window.PageController = {
    currentPage: null,
    controllers: {},

    // Register page-specific controllers
    register: function (pageName, controller) {
        try {
            if (!pageName || typeof controller !== 'object') {
                console.error('Invalid controller registration');
                return false;
            }

            this.controllers[pageName] = controller;
            return true;
        } catch (error) {
            console.error('Controller registration error:', error);
            return false;
        }
    },

    // Initialize page controller
    init: function () {
        try {
            // Get current page from data attribute or URL
            const mainContent = document.querySelector('main');
            this.currentPage = mainContent?.dataset?.page || this.detectPageFromUrl();

            console.log('Initializing page:', this.currentPage);

            // Initialize specific controller for this page
            const controller = this.controllers[this.currentPage];
            if (controller && typeof controller.init === 'function') {
                controller.init();
            } else {
                console.log('No specific controller found for page:', this.currentPage);
            }

            // Initialize common page elements
            this.initCommonElements();

        } catch (error) {
            console.error('PageController initialization error:', error);
        }
    },

    // Detect page from URL
    detectPageFromUrl: function () {
        try {
            const path = window.location.pathname.toLowerCase();

            if (path.includes('/dashboard')) return 'dashboard';
            if (path.includes('/attendance')) return 'attendance';
            if (path.includes('/students')) return 'students';
            if (path.includes('/reports')) return 'reports';
            if (path.includes('/settings')) return 'settings';

            return 'default';
        } catch (error) {
            console.error('Page detection error:', error);
            return 'default';
        }
    },

    // Initialize common page elements
    initCommonElements: function () {
        try {
            // Setup mobile menu toggle
            const menuToggle = document.getElementById('mobileMenuToggle');
            const sidebar = document.getElementById('sidebar');

            if (menuToggle && sidebar) {
                menuToggle.addEventListener('click', () => {
                    sidebar.classList.toggle('open');
                });
            }

            // Setup dropdown menus
            const dropdowns = document.querySelectorAll('.dropdown');
            dropdowns.forEach(dropdown => {
                const toggle = dropdown.querySelector('.dropdown-toggle');
                const menu = dropdown.querySelector('.dropdown-menu');

                if (toggle && menu) {
                    toggle.addEventListener('click', (e) => {
                        e.preventDefault();
                        menu.classList.toggle('show');
                    });

                    // Close on outside click
                    document.addEventListener('click', (e) => {
                        if (!dropdown.contains(e.target)) {
                            menu.classList.remove('show');
                        }
                    });
                }
            });

        } catch (error) {
            console.error('Common elements initialization error:', error);
        }
    }
};

// Register built-in controllers
PageController.register('auth', window.AuthController);
PageController.register('dashboard', window.DashController);
PageController.register('attendance', window.AttController);

// Make AppController reference the page controller
window.AppController = window.PageController;*/