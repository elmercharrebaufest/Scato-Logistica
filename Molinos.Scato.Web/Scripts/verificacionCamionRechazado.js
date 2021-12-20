$(document).ready(function () {
    $(".no-rechazar").click(function () {
        $(".valorDecision").val("false");
    });

    $(".rechazar").click(function () {
        $(".valorDecision").val("true");
    });

});