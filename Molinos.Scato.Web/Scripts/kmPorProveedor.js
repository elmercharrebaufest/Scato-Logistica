$(document).ready(function () {

    DefinirAutocompletarConSAP('#ClienteDescripcion', '#ClienteId', '#autocompleteCorr', $('#links').data().urlBuscarClientes, $('#links').data().urlBuscarCliente, $('#links').data().urlObtenerClientesSap, null, null, false, true, false);
    $("#ClienteDescripcion").autocomplete("option", "appendTo", "#dialogo-editar");
});