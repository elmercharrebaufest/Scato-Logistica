$(document).ready(function () {
    var formatoFecha = Globalize.culture().calendars.standard.patterns.d.replace(/[a-z]/g, '9');
    formatoFecha = formatoFecha.replace(/[A-Z]/g, '9');
    
    var formatoHora = Globalize.culture().calendar.patterns.t.replace(/[a-su-zA-SU-Z]/g, '9');
    formatoHora = formatoHora.replace(/[t]/g, 'a');
    $('.FechaDesde input').mask(formatoFecha + " " + formatoHora);
    $('.FechaHasta input').mask(formatoFecha + " " + formatoHora);
    

    
    if ($(".datetimeFormat").length > 0) {
        $(".datetimeFormat").html(Globalize.culture().calendar.patterns.d + " " + Globalize.culture().calendar.patterns.t);
    }
    $.validator.addMethod("date", function (value, element) {
        return Globalize.parseDate($('.FechaDesde input:first').val()) <= Globalize.parseDate(value) || $('.FechaHasta input:first').val() == "" || $('.FechaDesde input:first').val() == "";
    }, $('.FechaHasta').data().secondDateValidation);
});//onSelect
