$(document).ready(function () {
    
    $('.change-submits').change(function () {
        $(this).parents('form').submit();
    });
    
});