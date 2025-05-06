// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// jQuery দিয়ে Navbar Collapse Hide on Click
$(document).ready(function () {
    $('.navbar-nav a:not(.dropdown-toggle)').on('click', function () {
        $('.navbar-collapse').collapse('hide');
    });
});