$(document).ready(function () {
    $(".no-rechazar").click(function () {
        $(".valorDecision").val("false");
        $(".RevierteRechazo").val("true");
    });

    $(".rechazar").click(function () {
        $(".valorDecision").val("true");
        $(".RevierteRechazo").val("false");
    });

});