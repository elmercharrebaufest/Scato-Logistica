$(document).ready(function ($) {   
    $('#TipoDocumentoIngreso option').filter(function () {
        return (this.value != "CartaPorte" && this.value != "Remito");
    }).remove();

    $(".numero").mask("99999-99999999");
    $(".numeroctg").mask("999999999999");

    $("#FechaMovimiento").click(function() {
        $("#FechaCP").mask("99/99/9999");
    });
    
    DefinirAutocompletar('#MaterialDesc', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    $("#MaterialDesc").autocomplete("option", "appendTo", "#dialogo-editar");
    
    // para que el campo retome el foco al seleccionar una fecha
    $('input.date').datepicker("option", "onSelect", function () {
        $(this).focus();
    });
    
    $('#pesoBrutoIngreso').change(function () {
        var pesoNetoIngreso = parseFloat($('#pesoNetoIngreso').val());
        var pesoNetoEgreso = parseFloat($('#pesoNetoEgreso').val());

        if (pesoNetoIngreso > "0" || pesoNetoEgreso > "0" || $(this).val() == "")
            $(this).val("0");
    });
    
    $('#pesoNetoIngreso').change(function () {
        var pesoBrutoIngreso = parseFloat($('#pesoBrutoIngreso').val());
        var pesoNetoEgreso = parseFloat($('#pesoNetoEgreso').val());

        if (pesoBrutoIngreso > "0" || pesoNetoEgreso > "0" || $(this).val() == "")
            $(this).val("0");
    });

    $('#pesoNetoEgreso').change(function () {
        var pesoBrutoIngreso = parseFloat($('#pesoBrutoIngreso').val());
        var pesoNetoIngreso = parseFloat($('#pesoNetoIngreso').val());

        if (pesoBrutoIngreso > "0" || pesoNetoIngreso > "0" || $(this).val() == "")
            $(this).val("0");
    });
    
    if ($('#mensajeInvalido').data() != null && $('#mensajeInvalido').data().invalido != null) {
        $.validator.addMethod("valorDebeSerValido", function (value, element) {
            return ($("#MaterialId").val() != 0);
        }, $('#mensajeInvalido').data().invalido);
    }
});