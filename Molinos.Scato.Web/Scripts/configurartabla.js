$(document).ready(function ($) {
    DefinirAutocompletar('#MaterialDescc', '#MaterialIdd', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial, cargarCaracteristicas, borrarCaracteristicas);

    $("#MaterialDescc").autocomplete("option", "appendTo", "#dialogo-editar");
    if ($('#MaterialIdd').val() != "" && $('#MaterialIdd').val() != "0") cargarCaracteristicas();
});

function cargarCaracteristicas() {    
    $.getJSON($('#links').data().urlCaracteristicas, { materialId: $('#MaterialIdd').val() }, function (data) {
        var html = "";
        
        $.each(data, function (index, value) {
            var checked = "";
            if (value.Visible == true) checked = "checked='checked'";
            html += "<div><input name='CaracteristicasDeCalidadId' type='checkbox' value='" + value.Id + "' id='CaracteristicasDeCalidadId_" + value.Id + "' " + checked + "/>" + value.Descripcion + "</div>";
        });
        $("#caracteristicasDeCalidad").html(html);
    });
}

function borrarCaracteristicas() {
    $("#caracteristicasDeCalidad").html("");
}