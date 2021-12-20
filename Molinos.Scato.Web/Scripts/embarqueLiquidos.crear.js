jQuery(document).ready(function ($) {

    DefinirAutocompletarPuerto('#Vapor', '#VaporId', $('#links').data().urlBuscarVapores, $('#links').data().urlBuscarVapor);
    DefinirAutocompletarPuerto('#Bodega', '#BodegaId', $('#links').data().urlBuscarBodegas, $('#links').data().urlBuscarBodega);
    DefinirAutocompletarPuerto('#Material', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    DefinirAutocompletarPuerto('#Exportador', '#ExportadorId', $('#links').data().urlBuscarExportadores, $('#links').data().urlBuscarExportador);
    DefinirAutocompletarPuerto('#Destino', '#DestinoId', $('#links').data().urlBuscarDestinos, $('#links').data().urlBuscarDestino);

    $("#Vapor").autocomplete("option", "appendTo", "#dialogo-editar");
    $("#Bodega").autocomplete("option", "appendTo", "#dialogo-editar");
    $("#Material").autocomplete("option", "appendTo", "#dialogo-editar");
    $("#Exportador").autocomplete("option", "appendTo", "#dialogo-editar");
    $("#Destino").autocomplete("option", "appendTo", "#dialogo-editar");

    $('#Fecha').datetimepicker({
        format: 'dd/mm/yyyy hh:ii',
        autoclose: true
    });
});