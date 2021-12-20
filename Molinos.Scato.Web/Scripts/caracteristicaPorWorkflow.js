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
    $.getJSON(urlObtenerCaracteristicas, { materialId: $("#MaterialId").val() },
            function (response) {
                var options = '';
                for (var i = 0; i < response.length; i++) {
                    options += '<div><input type="checkbox" value="' + response[i].Id + '" name="CaracteristicasDeCalidadId" ' + (caracteristicaCalidadId != undefined && caracteristicaCalidadId.indexOf(response[i].Id) >= 0 ? "checked='true'" : "") + '/>' + response[i].Descripcion + '</div>';
                }
                $('#caracteristicasDropdown').html(options);
               
                $('#caracteristicasDropdown').removeAttr("disabled");
                $('#caracteristicasDropdown').focus();
            });
}