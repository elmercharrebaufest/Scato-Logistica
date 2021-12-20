/* Se sobreescriben las valildaciones de numeros y fechas para que usen Globalize*/

function EscaparPunto(value) {
    return value == "." ? "\\." : value;
}

$.validator.methods.number = function (value, element) {
    var groupSep = EscaparPunto(Globalize.culture().numberFormat[","]);
    var decimalSep = EscaparPunto(Globalize.culture().numberFormat["."]);
    //var exp = new RegExp("^-?\(?:\\d+|\\d{1,3}\(?:" + groupSep + "\\d{3}\)+\)?\(?:" + decimalSep + "\\d+\)?$");
    var exp = new RegExp("^-?\(?:\\d+)?\(?:" + decimalSep + "\\d+\)?$");
    return this.optional(element) || exp.test(value);
};

$.validator.methods.date = function (value, element) {
    return this.optional(element) || Globalize.parseDate(value) != null;
};

$.validator.methods.range = function (value, element, param) {
    var val = Globalize.parseFloat(value);
    return this.optional(element) || ( val >= param[0] && val <= param[1]);
};

/* Localización del date picker*/
$(function() {
    if ($.datepicker.regional[Globalize.culture().culture]) {
        /* Si UI tiene la cultura, usarla*/
        $.datepicker.setDefaults($.datepicker.regional[Globalize.culture().culture]); 
    } else if ($.datepicker.regional[Globalize.culture().language]) {
        /* Si no tiene la cultura pero tiene el lenguaje usarlo   */
        $.datepicker.setDefaults($.datepicker.regional[Globalize.culture().language]);
    } else {
        /*Si no tiene ninguno de los dos dejamos el valor por defecto (Ingles)*/
        $.datepicker.setDefaults($.datepicker.regional[""]);
    }
   
});
