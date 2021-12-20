$(document).ready(function () {
    $("#paisesDropDown").change(function () {
        $.getJSON($("#CargarProvincias").val(), { paisId: $("#paisesDropDown").val() },
            function (response) {
                var options = '';
                options += "<option value='" + "'>"
                            + $("#ValorDefaultProvincia").val() + "</option>";
                for (var j = 0; j < response.provincias.length; j++) {
                    options += "<option value='" + response.provincias[j].Value + "'>"
                            + response.provincias[j].Text + "</option>";
                }
                $('#provinciasDropDown').html(options);
                $('#provinciasDropDown').attr("disabled", false);
                
                options = "<option value='" + "'>"
                            + $("#ValorDefaultLocalidad").val() + "</option>";
                for (var i = 0; i < response.localidades.length; i++) {
                    options += "<option value='" + response.localidades[i].Value + "'>"
                            + response.localidades[i].Text + "</option>";
                }
                $('#localidadesDropDown').html(options);
                $('#localidadesDropDown').attr("disabled", false);
            });
    });
    
    $("#provinciasDropDown").change(function () {
        $.getJSON($("#CargarLocalidades").val(), { provinciaId: $("#provinciasDropDown").val() },
            function (response) {
                var options = '';
                options += "<option value='" + "'>"
                            + $("#ValorDefaultLocalidad").val() + "</option>";
                for (var i = 0; i < response.length; i++) {
                    options += "<option value='" + response[i].Value + "'>"
                            + response[i].Text + "</option>";
                }   
                $('#localidadesDropDown').html(options);
                $('#localidadesDropDown').attr("disabled", false);
            });
    });
});