$(document).ready(function () {
    
    var listarProveedores = $('#links').data().urlBuscarProveedores;
    var obtenerProveedor = $('#links').data().urlBuscarProveedor;
    var obtenerProveedorSap = $('#links').data().urlObtenerProveedoresSap;
    
    DefinirAutocompletarConSAP('#Proveedor', '#ProveedorId', '#autocompleteProv', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    $("#Proveedor").autocomplete("option", "appendTo", "#dialogo-editar");

});