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

    DefinirAutocompletar('#RazonSocial', '#ProveedorId', $('#links').data().urlBuscarProveedores, $('#links').data().urlBuscarProveedorUnico);
    $("#RazonSocial").autocomplete("option", "appendTo", "#dialogo-editar");
    
    DefinirAutocompletar('#MaterialDesc', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    $("#MaterialDesc").autocomplete("option", "appendTo", "#dialogo-editar");
});

function enfocador() {
    var opcion;   
    if ($("select#dropdownTipos option:selected").val() == "Centro") {
        $("#Cliente").hide();
        $("#Centro").show();
        $('#CentroDescripcion').addClass("campoRequerido");
        $('#ClienteDescripcion').removeClass("campoRequerido");
        $('#ClienteDestinoId').val('');
        $("#ClienteDescripcion").val('');
        opcion = "Centro";

    } else if ($("select#dropdownTipos option:selected").val() == "Cliente") {
        $("#Centro").hide();
        $("#Cliente").show();
        $('#ClienteDescripcion').addClass("campoRequerido");
        $('#CentroDescripcion').removeClass("campoRequerido");
        $('#CentroDestinoId').val('');
        $("#CentroDescripcion").val('');
        opcion = "Cliente";
    } else {
    }
    if (opcion == "Cliente") {
        DefinirAutocompletar('#ClienteDescripcion', '#ClienteDestinoId', $('#links').data().urlBuscarClientes, $('#links').data().urlBuscarCliente);
        $("#ClienteDescripcion").autocomplete("option", "appendTo", "#dialogo-editar");
    }
    if (opcion == "Centro") {
        DefinirAutocompletar('#CentroDescripcion', '#CentroDestinoId', $('#links').data().urlBuscarCentros, $('#links').data().urlBuscarCentro);
        $('#CentroDescripcion').autocomplete("option", "appendTo", "#dialogo-editar");
    }
}