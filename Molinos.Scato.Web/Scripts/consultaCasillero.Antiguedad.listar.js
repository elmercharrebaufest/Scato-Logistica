$(document).ready(function () {
    var cantidadDias = $("#DiasDeAntiguedad").val().replace("_", "").replace("_", "");
    if (cantidadDias != "")
    {
        $("#grid thead tr th a")[1].text = $("#grid thead tr th a")[1].text.replace("X", cantidadDias);
        $("#grid thead tr th a")[2].text = $("#grid thead tr th a")[2].text.replace("X", cantidadDias);
    }

    //if ($("#grid tbody tr").length > 0)
    //    $("#btnVerGraficoAntiguedad").show();
    //else 
    //    $("#btnVerGraficoAntiguedad").hide();
});
