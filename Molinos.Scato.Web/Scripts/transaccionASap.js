$(document).ready(function () {
    DefinirAutocompletar('#MaterialDesc', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    $("#MaterialDesc").autocomplete("option", "appendTo", "#dialogo-editar");
});