// Main application entry point
(function () {
    'use strict';

    // Wait for all dependencies to load
    function initialize() {
        try {
            console.log('Initializing EduAttend AMS...');

            // Initialize store
            if (typeof Store !== 'undefined' && typeof Store.init === 'function') {
                Store.init();
            }

            // Initialize page controller if on a page
            if (typeof PageController !== 'undefined') {
                if (typeof PageController.init === 'function') {
                    PageController.init();
                }
            }

            console.log('Application initialized successfully');
        } catch (error) {
            console.error('Application initialization error:', error);
        }
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initialize);
    } else {
        initialize();
    }
})();