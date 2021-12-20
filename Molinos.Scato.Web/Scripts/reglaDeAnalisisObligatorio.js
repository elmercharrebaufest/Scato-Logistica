

$(document).ready(function () {

    $("#Cosecha").mask("99-99");
    DefinirAutocompletar('#MaterialDescripcion', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    $("#MaterialDescripcion").autocomplete("option", "appendTo", "#dialogo-editar");
    DefinirAutocompletar('#LocalidadDescripcion', '#LocalidadId', $('#links').data().urlBuscarLocalidades, $('#links').data().urlBuscarLocalidad);
    $("#LocalidadDescripcion").autocomplete("option", "appendTo", "#dialogo-editar");
});
