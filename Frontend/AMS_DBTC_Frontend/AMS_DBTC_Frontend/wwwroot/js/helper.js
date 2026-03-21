// Helper functions namespace
window.Helpers = {
    // Safe element selection
    $: function (selector) {
        try {
            const element = document.querySelector(selector);
            return element || null;
        } catch (error) {
            console.error('Element selection error:', error);
            return null;
        }
    },

    // Safe AJAX request
    ajax: function (options) {
        return new Promise((resolve, reject) => {
            try {
                // Validate options
                if (!options || typeof options !== 'object') {
                    reject(new Error('Invalid AJAX options'));
                    return;
                }

                const defaults = {
                    method: 'GET',
                    url: '',
                    data: null,
                    contentType: 'application/json',
                    timeout: 30000,
                    success: null,
                    error: null
                };

                const settings = { ...defaults, ...options };

                // Create XMLHttpRequest
                const xhr = new XMLHttpRequest();

                // Setup timeout
                xhr.timeout = settings.timeout;

                // Open connection
                xhr.open(settings.method, settings.url, true);

                // Set headers
                xhr.setRequestHeader('Content-Type', settings.contentType);

                // Handle response
                xhr.onload = function () {
                    if (xhr.status >= 200 && xhr.status < 300) {
                        try {
                            const response = JSON.parse(xhr.responseText);
                            if (typeof settings.success === 'function') {
                                settings.success(response);
                            }
                            resolve(response);
                        } catch (parseError) {
                            reject(parseError);
                        }
                    } else {
                        const error = new Error(`HTTP Error: ${xhr.status}`);
                        if (typeof settings.error === 'function') {
                            settings.error(error);
                        }
                        reject(error);
                    }
                };

                // Handle errors
                xhr.onerror = function () {
                    const error = new Error('Network error');
                    if (typeof settings.error === 'function') {
                        settings.error(error);
                    }
                    reject(error);
                };

                // Handle timeout
                xhr.ontimeout = function () {
                    const error = new Error('Request timeout');
                    if (typeof settings.error === 'function') {
                        settings.error(error);
                    }
                    reject(error);
                };

                // Send request
                if (settings.data) {
                    xhr.send(JSON.stringify(settings.data));
                } else {
                    xhr.send();
                }
            } catch (error) {
                reject(error);
            }
        });
    },

    // Show toast notification
    showToast: function (message, type = 'info', duration = 3000) {
        try {
            const toast = document.getElementById('toast');
            if (!toast) {
                console.warn('Toast container not found');
                return;
            }

            const toastElement = document.createElement('div');
            toastElement.className = `toast toast-${type}`;
            toastElement.textContent = message;

            toast.appendChild(toastElement);

            // Auto remove after duration
            setTimeout(() => {
                if (toastElement.parentNode) {
                    toastElement.parentNode.removeChild(toastElement);
                }
            }, duration);

        } catch (error) {
            console.error('Toast error:', error);
        }
    },

    // Format date
    formatDate: function (date, format = 'YYYY-MM-DD') {
        try {
            const d = new Date(date);
            if (isNaN(d.getTime())) {
                return 'Invalid date';
            }

            const year = d.getFullYear();
            const month = String(d.getMonth() + 1).padStart(2, '0');
            const day = String(d.getDate()).padStart(2, '0');

            return format
                .replace('YYYY', year)
                .replace('MM', month)
                .replace('DD', day);
        } catch (error) {
            console.error('Date format error:', error);
            return 'Invalid date';
        }
    },

    // Debounce function
    debounce: function (func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func.apply(this, args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    },

    // Validate form
    validateForm: function (formId) {
        try {
            const form = document.getElementById(formId);
            if (!form) {
                console.warn('Form not found:', formId);
                return false;
            }

            const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
            let isValid = true;

            inputs.forEach(input => {
                if (!input.value.trim()) {
                    isValid = false;
                    input.classList.add('error');
                } else {
                    input.classList.remove('error');
                }
            });

            return isValid;
        } catch (error) {
            console.error('Form validation error:', error);
            return false;
        }
    }
};