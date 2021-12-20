$(document).ready(function () {
    if (!$('#MaterialId').val() > 0) {
        $('#caracteristicas').attr("disabled", true);
    }
    

    var listarProveedores = $('#links').data().urlBuscarProveedores;
    var obtenerProveedor = $('#links').data().urlBuscarProveedor;
    var obtenerProveedorSap = $('#links').data().urlObtenerProveedoresSap;

    DefinirAutocompletarConSAP('#ProveedorDesc', '#ProveedorId', '#autocompleteProv', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    $("#Proveedor").autocomplete("option", "appendTo", "#dialogo-editar");
    
    DefinirAutocompletar('#EntregadorDesc', '#EntregadorId', $('#links').data().urlBuscarEntregadores, $('#links').data().urlBuscarEntregador);   
    $("#EntregadorDesc").autocomplete("option", "appendTo", "#dialogo-editar");
    
    DefinirAutocompletar('#MaterialDesc', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial, onSelectMaterial, onSelectMaterial);
    $("#MaterialDesc").autocomplete("option", "appendTo", "#dialogo-editar");

});


function onSelectMaterial() {
    if ($('#MaterialId').val() != "" && $('#MaterialId').val() != 0) {
        $.getJSON($("#ObtenerCaracteristicas").val(), { materialId: $("#MaterialId").val() },
            function(response) {
                var options = '';
                for (var i = 0; i < response.length; i++) {
                    options += '<div><input type="checkbox" value="' + response[i].Id + '" name="CaracteristicasDeCalidadId" ' + (caracteristicaCalidadId != undefined && caracteristicaCalidadId.indexOf(response[i].Id) >= 0 ? "checked='true'" : "") + '/>' + response[i].Descripcion + '</div>';
                }
                $('#caracteristicasDropdown').html(options);
                if (options == '') {
                    $('#caracteristicas').attr("disabled", true);
                } else {
                    $('#caracteristicas').removeAttr("disabled");
                }

                $('#caracteristicasDropdown').removeAttr("disabled");
            });
    } else {
        $('#caracteristicasDropdown').html(null);
        $('#caracteristicasDropdown').attr("disabled", "disabled");
    }
}