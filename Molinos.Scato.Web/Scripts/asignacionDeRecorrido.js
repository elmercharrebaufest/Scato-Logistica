$(document).ready(function() {

    DefinirAutocompletar('#Material', '#MaterialPorCentroId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial, materialSeleccionado);
    $("#Material").autocomplete("option", "appendTo", "#dialogo-editar");
});

function materialSeleccionado() {
    $.getJSON($("#CargarCalidades").val(), { materialPorCentroId: $("#MaterialPorCentroId").val() },
        function (response) {
            var options = '';
            options += "<option value='" + "'>"
                + $("#ValorDefaultCalidad").val() + "</option>";
            for (var i = 0; i < response.calidades.length; i++) {
                options += "<option value='" + response.calidades[i].Value + "'>"
                    + response.calidades[i].Text + "</option>";
            }
            $('#calidadesDropDown').html(options);
            $('#calidadesDropDown').attr("disabled", false);
        });
}