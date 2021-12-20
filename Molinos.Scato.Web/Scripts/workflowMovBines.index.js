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

    var opcion = "";
    $("#ProveedorDescripcion").val('');
    $('#ProveedorId').val('0');
    $("#VinedoPropioDescripcion").val('');
    $('#VinedoPropioId').val('0');
    if ($("select#dropdownTipos option:selected").val() == "Centro") {
        $("#Proveedor").hide();
        $("#VinedoPropio").hide();
        $("#Centro").show();
        $('#CentroDescripcion').addClass("campoRequerido");
        $('#ProveedorDescripcion').removeClass("campoRequerido");
        $('#VinedoPropioDescripcion').removeClass("campoRequerido");
        $('#dropdownMovimiento').find('option[value="Ingreso"]').text('Ingreso');
        $('#dropdownMovimiento').find('option[value="Egreso"]').text('Egreso');

    } else if ($("select#dropdownTipos option:selected").val() == "Productor") {
        $("#Proveedor").show();
        $("#VinedoPropio").hide();
        $("#ProveedorDescripcion").prop('readonly', false);
        $("#Centro").hide();
        $('#ProveedorDescripcion').addClass("campoRequerido");
        $('#CentroDescripcion').removeClass("campoRequerido");
        $('#VinedoPropioDescripcion').removeClass("campoRequerido");
        $('#dropdownMovimiento').find('option[value="Ingreso"]').text('Entrega');
        $('#dropdownMovimiento').find('option[value="Egreso"]').text('Retira');
        opcion = "Terceros";
    } else if ($("select#dropdownTipos option:selected").val() == "VinedoPropio") {
        $("#Proveedor").hide();
        $("#VinedoPropio").show();
        $("#VinedoPropioDescripcion").prop('readonly', false);
        $("#Centro").hide();
        $('#ProveedorDescripcion').removeClass("campoRequerido");
        $('#CentroDescripcion').removeClass("campoRequerido");
        $('#VinedoPropioDescripcion').addClass("campoRequerido");
        $('#dropdownMovimiento').find('option[value="Ingreso"]').text('Entrega');
        $('#dropdownMovimiento').find('option[value="Egreso"]').text('Retira');
        opcion = "Propios";
    } else {
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
    }

    if (opcion == "Propios") {
        DefinirAutocompletar('#VinedoPropioDescripcion', '#VinedoPropioId', $('#links').data().urlBuscarVinedosPropios, $('#links').data().urlBuscarVinedoPropio);
    }
}