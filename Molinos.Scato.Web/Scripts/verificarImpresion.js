$(document).ready(function () {
    $(".no-reimprimir").click(function () {
        $(".valorDecision").val("false");
    });

    $(".reimprimir").click(function () {
        $(".valorDecision").val("true");
    });

});