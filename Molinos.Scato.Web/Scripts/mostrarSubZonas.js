$(document).ready(function () {
    var cargoSubZonas = false;

    $("#zonasDropDown").change(function() {
        cargoSubZonas = false;
    });

    $("#zonasDropDown").blur(function () {
        CargarSubZonas(cargoSubZonas);
        cargoSubZonas = true;
    });

    $("#zonasDropDown option").click(function () {
        CargarSubZonas(cargoSubZonas);
        cargoSubZonas = true;
    });

});

function CargarSubZonas(cargoSubZonas) {
    if (!cargoSubZonas) {
        $('#subzonasDropDown').attr("disabled", "disabled");
        $.getJSON($("#CargarSubZonas").val(), { zonaId: $("#zonasDropDown").val() },
            function (response) {
                var options = '';
                options += "<option value='" + "'>"
                            + $("#ValorDefault").val() + "</option>";
                for (var i = 0; i < response.length; i++) {
                    options += "<option value='" + response[i].Value + "'>"
                            + response[i].Text + "</option>";
                }
                $('#subzonasDropDown').html(options);
                $('#subzonasDropDown').attr("disabled", false);
                $('#subzonasDropDown').focus();
            });
    }
}