document.getElementById("toggleSidebar").addEventListener("click", function () {
    const sidebar = document.getElementById("sidebar");
    const content = document.getElementById("content-area");

    sidebar.classList.toggle("sidebar-collapsed");
    content.classList.toggle("sidebar-collapsed");
});

var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl)
})



//For Math Text Showing
window.MathJax = {
    tex: {
        inlineMath: [['\\(', '\\)'], ['$', '$']]
    },
    svg: {
        fontCache: 'global'
    }
};