jQuery(document).ready(function ($) {

    DefinirAutocompletarPuerto('#VaporDesc', '#IdVapor', $('#links').data().urlBuscarVapores, $('#links').data().urlBuscarVapor);
    DefinirAutocompletarPuerto('#BodegaDesc', '#IdBodega', $('#links').data().urlBuscarBodegas, $('#links').data().urlBuscarBodega);
    DefinirAutocompletarPuerto('#MaterialDesc', '#IdMaterial', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    DefinirAutocompletarPuerto('#ExportadorDesc', '#IdExportador', $('#links').data().urlBuscarExportadores, $('#links').data().urlBuscarExportador);
    DefinirAutocompletarPuerto('#DestinoDesc', '#IdDestino', $('#links').data().urlBuscarDestinos, $('#links').data().urlBuscarDestino);

    $("#FechaDesde").attr("autocomplete", "off");
    $("#FechaHasta").attr("autocomplete", "off");

 

});