jQuery(document).ready(function ($) {
    DefinirAutocompletar('#Material', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    $("#Material").autocomplete("option", "appendTo", "#dialogo-editar");
});