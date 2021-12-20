$(document).ready(function () {

    $("#PermisoEmbarque").mask("99999aa99999999a");

    $("#PermisoEmbarque").change(function(e) {
        var valor = $(this).val();
        if (valor.length > 0) {
            $("#PermisoEmbarque").val($("#PermisoEmbarque").val().toUpperCase());
        }
        return true;
    });
    
    //DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, true, null, null, onSelectProveedor, onSelectTransportista, true, false, false);
    $('#Transportista').addClass('transportistaRequerido');
    DefinirAutocompletar('#Transportista', '#TransportistaId', $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico);
});