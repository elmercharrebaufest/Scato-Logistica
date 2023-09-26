$(document).ready(function () {
    
    var listarCorredores = $('#links').data().urlBuscarProveedores;
    var obtenerCorredor = $('#links').data().urlBuscarProveedor;
    var obtenerCorredorSap = $('#links').data().urlObtenerProveedoresSap;
    
    DefinirAutocompletarConSAP('#Corredor', '#CorredorId', '#autocompleteCorred', listarCorredores, obtenerCorredor, obtenerCorredorSap, null, null, true, false, false);
    $("#Corredor").autocomplete("option", "appendTo", "#dialogo-editar");

});