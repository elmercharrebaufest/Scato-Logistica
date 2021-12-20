jQuery(document).ready(function ($) {
    DefinirAutocompletar('#MaterialDesc', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    DefinirAutocompletarConSAP('#ClienteDesc', '#ClienteId', '#autocompleteCliente', $('#links').data().urlBuscarClientes, $('#links').data().urlBuscarClienteUnico, $('#links').data().urlObtenerClientesSap);

    $("#MaterialDesc").autocomplete("option", "appendTo", "#dialogo-editar");
    $("#ClienteDesc").autocomplete("option", "appendTo", "#dialogo-editar");
});
