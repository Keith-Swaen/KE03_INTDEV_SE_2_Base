// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Wait for the DOM to be ready
document.addEventListener('DOMContentLoaded', function () {
    // Sidebar toggle functionality
    const sidebarCollapse = document.getElementById('sidebarCollapse');
    const sidebar = document.getElementById('sidebar');
    
    if (sidebarCollapse && sidebar) {
        // Only handle click events on mobile
        sidebarCollapse.addEventListener('click', function (e) {
            if (window.innerWidth <= 768) {
                e.stopPropagation();
                sidebar.classList.toggle('active');
            }
        });
    }

    // Close sidebar on mobile when clicking outside
    document.addEventListener('click', function (event) {
        if (window.innerWidth <= 768) {
            const isClickInsideSidebar = sidebar.contains(event.target);
            const isClickOnToggleButton = sidebarCollapse.contains(event.target);
            
            if (!isClickInsideSidebar && !isClickOnToggleButton) {
                sidebar.classList.remove('active');
            }
        }
    });

    // Handle window resize
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
