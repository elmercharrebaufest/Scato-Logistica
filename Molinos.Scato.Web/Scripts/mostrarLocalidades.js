$(document).ready(function () {
    var cargoLocalidades = false;

    $("#provinciasDropDown").change(function() {
        cargoLocalidades = false;
    });

    $("#provinciasDropDown").blur(function () {
        CargarLocalidades(cargoLocalidades);
        cargoLocalidades = true;
    });

    $("#provinciasDropDown option").click(function () {
        CargarLocalidades(cargoLocalidades);
        cargoLocalidades = true;
    });

});

function CargarLocalidades(cargoLocalidades) {
    if (!cargoLocalidades) {
        $('#localidadesDropDown').attr("disabled", "disabled");
        //$('.btn').attr("disabled", "disabled");
        $.getJSON($("#CargarLocalidades").val(), { provinciaId: $("#provinciasDropDown").val() },
            function (response) {
                var options = '';
                options += "<option value='" + "'>"
                            + $("#ValorDefault").val() + "</option>";
                for (var i = 0; i < response.length; i++) {
                    options += "<option value='" + response[i].Value + "'>"
                            + response[i].Text + "</option>";
                }
                $('#localidadesDropDown').html(options);
                $('#localidadesDropDown').attr("disabled", false);
                $('#localidadesDropDown').focus();
                //$('.btn').attr("disabled", false);
            });
    }
}