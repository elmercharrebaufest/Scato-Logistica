$(document).ready(function () {
    var formatoFecha = Globalize.culture().calendars.standard.patterns.d.replace(/[a-z]/g, '9');
    formatoFecha = formatoFecha.replace(/[A-Z]/g, '9');
    
    //$.validator.addMethod("date", function (value, element) {
    //    return Globalize.parseDate($('.FechaDesde input').val()) <= Globalize.parseDate(value) || $('.FechaHasta input').val() == "" || $('.FechaDesde input').val() == "";
    //}, $('.FechaHasta input').data().secondDateValidation);
    //$.validator.addMethod("date", function(value, element) { return true; });
        
    $('.FechaDesde input').mask(formatoFecha);
    $('.FechaHasta input').mask(formatoFecha);
    
    $(".FechaDesde input").datepicker({
        onSelect: function (selected) {
            cambiarFechaDesde(selected);
        }
    });
    $(".FechaHasta input").datepicker({
        onSelect: function (selected) {
            cambiarFechaHasta(selected);
        }
    });

    $(".FechaHasta input").change(function (selected) {
        cambiarFechaHasta($(".FechaHasta input").val());
    });

    $(".FechaDesde input").change(function (selected) {
        cambiarFechaDesde($(".FechaDesde input").val());
    });
    
});//onSelect

function cambiarFechaDesde(selected) {
    $(".FechaHasta input").datepicker("option", "minDate", selected);
    $(".FechaDesde input").valid();
    var controlGroup = $(".FechaDesde input").closest("div.control-group");
    if (controlGroup.find('span.field-validation-error').length == 0)
        controlGroup.removeClass('error');
    else
        controlGroup.addClass('error');
}

function cambiarFechaHasta(valor) {
    $(".FechaDesde input").datepicker("option", "maxDate", valor);
    $(".FechaHasta input").valid();
    var controlGroup = $(".FechaHasta input").closest("div.control-group");
    if (controlGroup.find('span.field-validation-error').length == 0)
        controlGroup.removeClass('error');
    else
        controlGroup.addClass('error');
}