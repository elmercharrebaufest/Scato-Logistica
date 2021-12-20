$(document).ready(function () {
    $(".autorizar").click(function () {
        $(".valorDecision").val("true");
    });

    $(".rechazar").click(function () {
        $(".valorDecision").val("false");
    });
});