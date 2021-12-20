$(document).ready(function () {
    $('.accordion').on('show hide', function (n) {
        $(n.target).siblings('.accordion-heading').find('.accordion-toggle i').toggleClass('icon-chevron-right icon-chevron-down');
    });
});