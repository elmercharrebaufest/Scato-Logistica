$(document).ready(function () {


    $('#Fecha').datetimepicker({
        format: 'dd/mm/yyyy hh:ii',
        autoclose: true,
        pickerPosition: "bottom-left"
    });
    $('#PesoBruto, #PesoTara').on("change paste keyup", function () {
        if ($("#PesoBruto").val() != "" && $("#PesoTara").val() != "") {
            $("#PesoNeto").val($("#PesoBruto").val() - $("#PesoTara").val());
        }
    });
    $('#PesoNeto, #PesoBruto, #PesoTara').on("change paste keyup", function () {
        if ($("#PesoNeto").val() != $("#PesoBruto").val() - $("#PesoTara").val()) {
            $("#PesoNetoError").show();
        } else {
            $("#PesoNetoError").hide();
        }
    });
});