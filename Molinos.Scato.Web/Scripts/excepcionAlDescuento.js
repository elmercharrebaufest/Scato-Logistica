$(document).ready(function () {

    $('#caracteristicasDropdown').attr("disabled", "disabled");

    DefinirAutocompletarConSAP('#ProveedorDescripcion', '#ProveedorId', '#autocompleteCorr', $('#links').data().urlBuscarProveedores, $('#links').data().urlBuscarProveedor, $('#links').data().urlObtenerProveedoresSap, null, null, false, true, false);
    $("#ProveedorDescripcion").autocomplete("option", "appendTo", "#dialogo-editar");

    DefinirAutocompletar('#MaterialDescripcion', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial, cargarCaracteristicas, bloquearCaracteristicas);
    $("#MaterialDescripcion").autocomplete("option", "appendTo", "#dialogo-editar");
});

function bloquearCaracteristicas() {
    $('#caracteristicasDropdown').html(null);
    $('#caracteristicasDropdown').attr("disabled", "disabled");
}

function cargarCaracteristicas() {
    //$('.btn').attr("disabled", "disabled");
    $.getJSON(urlObtenerCaracteristicas, { materialId: $("#MaterialId").val() },
            function (response) {
                var options = '';
                for (var i = 0; i < response.length; i++) {
                    options += "<option value='" + response[i].Id + "'" + (caracteristicaCalidadId == response[i].Id.toString() ? "selected = 'selected'" : "") + ">"
                            + response[i].Descripcion + "</option>";
                }
                $('#caracteristicasDropdown').html(options);
                //if (valor != 0) {
                //    $("#caracteristicasDropdown option[value='" + valor + "']");

                //}
                $('#caracteristicasDropdown').removeAttr("disabled");
                $('#caracteristicasDropdown').focus();
                //$('.btn').attr("disabled", false);
            });
    
}