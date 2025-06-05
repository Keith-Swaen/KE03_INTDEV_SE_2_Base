// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Wacht tot de DOM geladen is
document.addEventListener('DOMContentLoaded', function () {
    // Zijbalk schakel functionaliteit
    const sidebarCollapse = document.getElementById('sidebarCollapse');
    const sidebar = document.getElementById('sidebar');
    
    if (sidebarCollapse && sidebar) {
        // Alleen klik events afhandelen op mobiel
        sidebarCollapse.addEventListener('click', function (e) {
            if (window.innerWidth <= 768) {
                e.stopPropagation();
                sidebar.classList.toggle('active');
            }
        });
    }

    // Sluit zijbalk op mobiel bij klik buiten menu
    document.addEventListener('click', function (event) {
        if (window.innerWidth <= 768) {
            const isClickInsideSidebar = sidebar.contains(event.target);
            const isClickOnToggleButton = sidebarCollapse.contains(event.target);
            
            if (!isClickInsideSidebar && !isClickOnToggleButton) {
                sidebar.classList.remove('active');
            }
        }
    });

    // Afhandeling van venstergrootte aanpassing
    let resizeTimer;
    window.addEventListener('resize', function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function() {
            if (window.innerWidth > 768) {
                sidebar.classList.remove('active');
            }
        }, 250);
    });
});
