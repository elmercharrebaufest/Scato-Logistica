jQuery(document).ready(function ($) {
    DefinirAutocompletarPuerto('#AlmacenDesc', '#Almacen_Id', $('#links').data().urlBuscarAlmacenes, $('#links').data().urlBuscarAlmacen);

    $("#AlmacenDesc").autocomplete("option", "appendTo", "#dialogo-editar");
    
});