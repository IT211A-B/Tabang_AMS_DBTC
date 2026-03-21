// Dashboard Controller
window.DashController = {
    // Initialize dashboard
    init: function () {
        try {
            console.log('Initializing dashboard...');

            // Check authentication
            if (typeof Models !== 'undefined' &&
                typeof Models.Session !== 'undefined' &&
                !Models.Session.isLoggedIn()) {
                window.location.href = '/Auth/Login';
                return;
            }

            // Load dashboard data
            this.loadDashboardData();

            // Setup event listeners
            this.setupEventListeners();

            console.log('Dashboard initialized');
        } catch (error) {
            console.error('Dashboard initialization error:', error);
        }
    },

    // Load dashboard data
    loadDashboardData: function () {
        try {
            Helpers.ajax({
                method: 'GET',
                url: '/Dashboard/GetData',
                success: (data) => {
                    try {
                        if (data) {
                            this.renderDashboard(data);
                        }
                    } catch (error) {
                        console.error('Dashboard data render error:', error);
                    }
                },
                error: (error) => {
                    console.error('Dashboard data load error:', error);
                    Helpers.showToast('Failed to load dashboard data', 'error');
                }
            });
        } catch (error) {
            console.error('Dashboard data loading error:', error);
        }
    },

    // Render dashboard
    renderDashboard: function (data) {
        try {
            // Update stats
            const statsContainer = document.getElementById('statsContainer');
            if (statsContainer && data.stats) {
                statsContainer.innerHTML = this.renderStats(data.stats);
            }

            // Update charts
            if (data.charts) {
                this.renderCharts(data.charts);
            }
        } catch (error) {
            console.error('Dashboard render error:', error);
        }
    },

    // Render stats
    renderStats: function (stats) {
        try {
            return stats.map(stat => `
                <div class="stat-card">
                    <div class="stat-value">${stat.value || 0}</div>
                    <div class="stat-label">${stat.label || ''}</div>
                </div>
            `).join('');
        } catch (error) {
            console.error('Stats render error:', error);
            return '';
        }
    },

    // Setup event listeners
    setupEventListeners: function () {
        try {
            // Refresh button
            const refreshBtn = document.getElementById('refreshDashboard');
            if (refreshBtn) {
                refreshBtn.addEventListener('click', () => {
                    this.loadDashboardData();
                    Helpers.showToast('Refreshing...', 'info');
                });
            }

            // Date range picker
            const dateRange = document.getElementById('dateRange');
            if (dateRange) {
                dateRange.addEventListener('change', (e) => {
                    this.loadDashboardData({ dateRange: e.target.value });
                });
            }
        } catch (error) {
            console.error('Event listeners setup error:', error);
        }
    },

    // Render charts
    renderCharts: function (charts) {
        try {
            // Add Chart.js or similar library for charts
            console.log('Rendering charts:', charts);
        } catch (error) {
            console.error('Charts render error:', error);
        }
    }
};