$(document).ready(function ($) {
    $("#Cuit").change(function () {
        $("#NumeroDeDocumento").val($("#Cuit").val().split('-')[1]);
    });
//ANS-2382
    //$("#btnGuardarClienteProvisorio").click(function () {
    //    limpiarErrorModal()

    //    if ($("#Cuit").val() != '' && $("#Descripcion").val() != '') {

    //        $("#registrarClienteProvisorio-form").submit();
    //        $('#Descripcion').val('');
    //        $('#Cuit').val('');
    //        modalCrearCliente.close()
    //    }
    //    if ($("#Descripcion").val() == '') {
    //        $("#error-descripcion").addClass("field-validation-error");
    //        $("#error-descripcion").show();

    //    }

    //    if ($("#Cuit").val() == '') {
    //        $("#error-cuit").addClass("field-validation-error");
    //        $("#error-cuit").show();
    //    }

    //})
    //$("#btnCerrarClienteProvisorio").click(function () {
    //    limpiarErrorModal()
    //    modalCrearCliente.close()
    //})

    $("#Cuit").mask("99-99999999-9");
});
//ANS-2382
//var fnResponse = function (response) {
//    if (response.error) {
//        MostrarAlertaError(response.mensajeError);
//    }
//    limpiarErrorModal()
//}
//ANS - 2382
//function limpiarErrorModal() {

//    $("#error-descripcion").hide();
//    $("#error-cuit").hide();
//}