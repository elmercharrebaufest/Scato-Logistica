$(document).ready(function () {
    $(".repesar").click(function () {
        $(".valorDecision").val("true");
    });

    $(".rechazar").click(function () {
        $(".valorDecision").val("false");
    });

    if ($('#notifica').val() == "True") {
        $('#Mensaje').attr("readonly", true);
    } else {
    {
        $('#Mensaje').attr("readonly", false);
    }
    }
});