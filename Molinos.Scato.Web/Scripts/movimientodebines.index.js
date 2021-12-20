jQuery(document).ready(function ($) {
    $("#CentroDescripcion").attr("readonly", false);

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
    if ($("#modifica").val() == "0") {      
        $("#ProveedorDescripcion").val('');
        $('#ProveedorId').val('0');
        $("#VinedoPropioDescripcion").val('');
        $('#VinedoPropioId').val('0');
    }
    $("#modifica").val('0');
    if ($("select#dropdownTipos option:selected").val() == "Centro") {
        $("#Proveedor").hide();
        $("#VinedoPropio").hide();
        $("#Centro").show();
        $('#CentroDescripcion').addClass("campoRequerido");
        $('#ProveedorDescripcion').removeClass("campoRequerido");
        $('#VinedoPropioDescripcion').removeClass("campoRequerido");
        $('#dropdownMovimiento').find('option[value="Ingreso"]').text('Ingreso');
        $('#dropdownMovimiento').find('option[value="Egreso"]').text('Egreso');
        opcion = "Centro";

    } else if ($("select#dropdownTipos option:selected").val() == "Productor") {
        $("#Proveedor").show();
        $("#ProveedorDescripcion").prop('readonly', false);
        $("#Centro").hide();
        $("#VinedoPropio").hide();
        $('#ProveedorDescripcion').addClass("campoRequerido");
        $('#CentroDescripcion').removeClass("campoRequerido");
        $('#VinedoPropioDescripcion').removeClass("campoRequerido");
        $('#dropdownMovimiento').find('option[value="Ingreso"]').text('Retira');
        $('#dropdownMovimiento').find('option[value="Egreso"]').text('Entrega');
        opcion = "Terceros";
    } else if ($("select#dropdownTipos option:selected").val() == "VinedoPropio") {
        $("#Proveedor").hide();
        $("#VinedoPropioDescripcion").prop('readonly', false);
        $("#Centro").hide();
        $("#VinedoPropio").show();
        $('#ProveedorDescripcion').removeClass("campoRequerido");
        $('#CentroDescripcion').removeClass("campoRequerido");
        $('#VinedoPropioDescripcion').addClass("campoRequerido");
        $('#dropdownMovimiento').find('option[value="Ingreso"]').text('Retira');
        $('#dropdownMovimiento').find('option[value="Egreso"]').text('Entrega');
        opcion = "Propios";
    }else {
        $("#Proveedor").show();
        $("#ProveedorDescripcion").prop('readonly', true);
        $("#VinedoPropioDescripcion").prop('readonly', true);
        $("#Centro").hide();
        $("#VinedoPropio").hide();
        $('#ProveedorDescripcion').removeClass("campoRequerido");
        $('#CentroDescripcion').removeClass("campoRequerido");
        $('#VinedoPropioDescripcion').removeClass("campoRequerido");
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