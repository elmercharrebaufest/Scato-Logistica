jQuery(document).ready(function ($) {
    enfocadorCentro_Productor();
    
    $('#dropdownTipos').change(function () {

        enfocadorCentro_Productor();
    });
    
    $.validator.addMethod("campoRequerido", function (value, element) {
        return value.length > 0;
    }, $('#CentroDescripcion').data().errorRequerido);
    
    $.validator.addMethod("campoRequerido", function (value, element) {
        return value.length > 0;
    }, $('#ProveedorDescripcion').data().errorRequerido);
    $.validator.addMethod("campoRequerido", function (value, element) {
        return value.length > 0;
    }, $('#VinedoPropioDescripcion').data().errorRequerido);

});

function enfocadorCentro_Productor() {
    var opcion;
    $("#CentroDescripcion").val('');
    $('#CentroId').val('0');
    $("#ProveedorDescripcion").val('');
    $('#ProveedorId').val('0');
    $("#VinedoPropioDescripcion").val('');
    $('#VinedoPropioId').val('0');
    if ($("select#dropdownTipos option:selected").val() == "Centro") {
        $("#VinedoPropio").hide();
        $("#Proveedor").hide();
        $("#Centro").show();
        $('#CentroDescripcion').addClass("campoRequerido");
        $('#ProveedorDescripcion').removeClass("campoRequerido");
        $('#VinedoPropioDescripcion').removeClass("campoRequerido");      
        opcion = "Centro";

    } else if ($("select#dropdownTipos option:selected").val() == "Productor") {
        $("#Proveedor").show();
        $("#Centro").hide();
        $("#VinedoPropio").hide();
        $('#ProveedorDescripcion').addClass("campoRequerido");
        $('#CentroDescripcion').removeClass("campoRequerido");
        $('#VinedoPropioDescripcion').removeClass("campoRequerido");
        opcion = "Terceros";
    } else {
        $("#Proveedor").hide();
        $("#Centro").hide();
        $("#VinedoPropio").show();
        $('#ProveedorDescripcion').removeClass("campoRequerido");
        $('#CentroDescripcion').removeClass("campoRequerido");
        $('#VinedoPropioDescripcion').addClass("campoRequerido");
        opcion = "Propios";       
    }
    if (opcion == "Terceros") {
        DefinirAutocompletar('#ProveedorDescripcion', '#ProveedorId', $('#links').data().urlBuscarVinedosTercerosProveedores, $('#links').data().urlBuscarVinedoTerceroProveedor);
        $("#ProveedorDescripcion").autocomplete("option", "appendTo", "#dialogo-editar");
    }

    if (opcion == "Propios") { 
        DefinirAutocompletar('#VinedoPropioDescripcion', '#VinedoPropioId', $('#links').data().urlBuscarVinedosPropios, $('#links').data().urlBuscarVinedoPropio);
        $("#VinedoPropioDescripcion").autocomplete("option", "appendTo", "#dialogo-editar");
    }

    if (opcion == "Centro") {
        DefinirAutocompletar('#CentroDescripcion', '#CentroId', $('#links').data().urlBuscarCentrosBodega, $('#links').data().urlBuscarCentroBodega);
        $('#CentroDescripcion').autocomplete("option", "appendTo", "#dialogo-editar");
    }
}