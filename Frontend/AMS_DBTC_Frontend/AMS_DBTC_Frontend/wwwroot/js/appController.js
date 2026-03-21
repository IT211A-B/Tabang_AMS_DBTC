/// <reference path="pagecontroller.js" />
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>@ViewData["Title"] — EduAttend AMS</title>
        <link href="https://fonts.googleapis.com/css2?family=Nunito:wght@700;800;900&family=Nunito+Sans:wght@400;600&display=swap" rel="stylesheet" />
        <link rel="stylesheet" href="~/css/root.css" />
        <link rel="stylesheet" href="~/css/loading.css" />
        <link rel="stylesheet" href="~/css/auth.css" />
        <link rel="stylesheet" href="~/css/dashboard.css" />
        <link rel="stylesheet" href="~/css/pages.css" />
            <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    </head>
    <body>

        <div id="loadingScreen">
            <div class="ls-circles">
                <div class="ls-circle"></div>
                <div class="ls-circle"></div>
                <div class="ls-circle"></div>
                <div class="ls-circle"></div>
            </div>
            <div class="ls-card">
                <div class="ls-icon">
                    <img src="~/css/assets/donboscologo.jpg" alt="School Logo"
                        onerror="$(this).hide().next().show();" />
                    <span style="display:none;">📋</span>
                </div>
                <div class="ls-name">DBTC</div>
                <div class="ls-tagline">Attendance Management System</div>
                <div class="ls-dots">
                    <div class="ls-dot"></div>
                    <div class="ls-dot"></div>
                    <div class="ls-dot"></div>
                </div>
                <div class="ls-text" id="lsStatus">Loading, please wait…</div>
                <div class="ls-bar-track">
                    <div class="ls-bar-fill" id="lsBar"></div>
                </div>
                <div class="ls-pct" id="lsPct">0%</div>
            </div>
        </div>

        <div id="toast"></div>

        @RenderBody()

        <script src="~/js/models.js"></script>
        <script src="~/js/store.js"></script>
        <script src="~/js/helpers.js"></script>
        <script src="~/js/authController.js"></script>
        <script src="~/js/appController.js"></script>
        <script src="~/js/dashController.js"></script>
        <script src="~/js/attController.js"></script>
        <script src="~/js/pageController.js"></script>
        <script src="~/js/main.js"></script>

        @await RenderSectionAsync("Scripts", required: false)

        <script>
            function dismissLoadingScreen() {
                var bar = document.getElementById('lsBar');
                var status = document.getElementById('lsStatus');
                var pct = document.getElementById('lsPct');
                var screen = document.getElementById('loadingScreen');

                if (!screen) return;

                var steps = [
                    { at: 100, p: 25, msg: 'Loading resources...' },
                    { at: 300, p: 55, msg: 'Initializing system...' },
                    { at: 500, p: 85, msg: 'Almost ready...' },
                    { at: 650, p: 100, msg: 'Done!' }
                ];

                for (var i = 0; i < steps.length; i++) {
                    (function (step) {
                        setTimeout(function () {
                            if (bar) bar.style.width = step.p + '%';
                            if (pct) pct.textContent = step.p + '%';
                            if (status) status.textContent = step.msg;
                        }, step.at);
                    })(steps[i]);
                }

                setTimeout(function () {
                    if (screen) {
                        screen.style.transition = 'opacity 0.45s ease';
                        screen.style.opacity = '0';
                        setTimeout(function () {
                            if (screen && screen.parentNode) {
                                screen.parentNode.removeChild(screen);
                            }
                        }, 450);
                    }
                }, 800);
            }

           function initApp() {
           /*     try {
                    if (typeof Session !== 'undefined' && !Session.isLoggedIn()) {
                    window.location.href = '/Auth/Login';
                        return;
                    }
                } catch (e) {
                    console.error('Session check error:', e);
                  window.location.href = '/Auth/Login';
                    return;
                }
                */
                try {
                    if (typeof AppController !== 'undefined') {
                        AppController.init();
                    }
                } catch (e) {
                    console.error('AppController.init error:', e);
                }
            }

            window.addEventListener('load', function () {
                dismissLoadingScreen();
                initApp();
            });
        </script>

    </body>
</html>