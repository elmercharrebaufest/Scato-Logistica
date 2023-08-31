$(document).ready(function () {
    enfocador();

    $('#dropdownTipos').change(function () {
        enfocador();
    });

    $.validator.addMethod("campoRequerido", function (value, element) {
        return value.length > 0;
    }, $('#CentroDescripcion').data().errorRequerido);

    $.validator.addMethod("campoRequerido", function (value, element) {
        return value.length > 0;
    }, $('#ClienteDescripcion').data().errorRequerido);

    $.validator.addMethod("campoRequerido", function (value, element) {
        return value.length > 0;
    }, $('#ProveedorDescripcion').data().errorRequerido);

    DefinirAutocompletar('#RazonSocial', '#TransportistaId', $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico);
    $("#RazonSocial").autocomplete("option", "appendTo", "#dialogo-editar");
    
    DefinirAutocompletar('#MaterialDesc', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    $("#MaterialDesc").autocomplete("option", "appendTo", "#dialogo-editar");
});

function enfocador() {
    var opcion;   
    if ($("select#dropdownTipos option:selected").val() == "Centro") {
        opcion = "Centro";
        $("#Centro").show();
        $('#CentroDescripcion').addClass("campoRequerido");

        $("#Cliente").hide();
        $('#ClienteDescripcion').removeClass("campoRequerido");
        $("#ClienteDescripcion").val('');
        $('#ClienteDestinoId').val('');

        $("#Proveedor").hide();
        $('#ProveedorDescripcion').removeClass("campoRequerido");
        $("#ProveedorDescripcion").val('');
        $('#ProveedorDestinoId').val('');
    } else if ($("select#dropdownTipos option:selected").val() == "Cliente") {
        opcion = "Cliente";
        $("#Cliente").show();
        $('#ClienteDescripcion').addClass("campoRequerido");

        $("#Centro").hide();
        $('#CentroDescripcion').removeClass("campoRequerido");
        $("#CentroDescripcion").val('');
        $('#CentroDestinoId').val('');

        $("#Proveedor").hide();
        $('#ProveedorDescripcion').removeClass("campoRequerido");
        $("#ProveedorDescripcion").val('');
        $('#ProveedorDestinoId').val('');
    } else if ($("select#dropdownTipos option:selected").val() == "Proveedor") {
        opcion = "Proveedor";
        $("#Proveedor").show();
        $('#CentroDescripcion').removeClass("campoRequerido");

        $("#Centro").hide();
        $('#CentroDescripcion').addClass("campoRequerido");
        $("#CentroDescripcion").val('');
        $('#CentroDestinoId').val('');

        $("#Cliente").hide();
        $('#ClienteDescripcion').removeClass("campoRequerido");
        $("#ClienteDescripcion").val('');
        $('#ClienteDestinoId').val('');
    }

    if (opcion == "Cliente") {
        DefinirAutocompletar('#ClienteDescripcion', '#ClienteDestinoId', $('#links').data().urlBuscarClientes, $('#links').data().urlBuscarCliente);
        $("#ClienteDescripcion").autocomplete("option", "appendTo", "#dialogo-editar");
    }
    if (opcion == "Centro") {
        DefinirAutocompletar('#CentroDescripcion', '#CentroDestinoId', $('#links').data().urlBuscarCentros, $('#links').data().urlBuscarCentro);
        $('#CentroDescripcion').autocomplete("option", "appendTo", "#dialogo-editar");
    }
    if (opcion == "Proveedor") {
        DefinirAutocompletar('#ProveedorDescripcion', '#ProveedorDestinoId', $('#links').data().urlBuscarProveedores, $('#links').data().urlBuscarProveedor);
        $('#ProveedorDescripcion').autocomplete("option", "appendTo", "#dialogo-editar");
    }
}