
// Scripts
// 
window.addEventListener('DOMContentLoaded', event => {

    // Navbar shrink function
    var navbarShrink = function () {
        const navbarCollapsible = document.body.querySelector('#mainNav');
        if (!navbarCollapsible) {
            return;
        }
        if (window.scrollY === 0) {
            navbarCollapsible.classList.remove('navbar-shrink');
        } else {
            navbarCollapsible.classList.add('navbar-shrink');
        }
    };

    // Shrink the navbar 
    navbarShrink();
    document.addEventListener('scroll', navbarShrink);

    // Scrollspy
    const mainNav = document.body.querySelector('#mainNav');
    if (mainNav) {
        new bootstrap.ScrollSpy(document.body, {
            target: '#mainNav',
            rootMargin: '0px 0px -40%',
        });
    }

    // Collapse responsive navbar when a nav link is clicked
    const navbarToggler = document.querySelector('.navbar-toggler');
    const navbarResponsive = document.getElementById('navbarResponsive');
    const responsiveNavItems = document.querySelectorAll('#navbarResponsive .nav-link');

    const bsCollapse = new bootstrap.Collapse(navbarResponsive, {
        toggle: false
    });

    responsiveNavItems.forEach((navItem) => {
        navItem.addEventListener('click', () => {
            if (navbarToggler.offsetParent !== null) { // Means toggler is visible
                bsCollapse.hide();
            }
        });
    });

});

