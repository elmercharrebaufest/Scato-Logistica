$(document).ready(function() {
    disableInputs();
    $(document).on('click', 'input:checkbox', function () {
        disableInputs();
    });
    $("td:nth-child(even)").addClass("even");
    $("td:nth-child(odd)").addClass("odd");
    
    $("#VigenciaDesde").mask("99");
    $("#VigenciaHasta").mask("99");
    $.validator.addMethod("cosechaValida", function cosechaValidaF(EsCosecha, element) {
        if ($('#EsCosecha').is(':checked') == false) {
            return true;
        } else {
            if ($("#VigenciaDesde").val() == "" || $("#VigenciaHasta").val() == "" || $("#VigenciaDesde").val() >= $("#VigenciaHasta").val()) {
                return false;
            }
        }

        return true;
    }, $('#EsCosecha').data().errorCosechaInvalida);
    
    $.validator.addMethod("porcentajeValido", function (value, element) {
        var valor = parseFloat(value.replace(',' , '.')).toFixed(2);
        if (valor < 0 || valor > 100) {
            return false;
        }
        return true;
    }, $('#muestraAuditoria').data().errorPorcentajeInvalido);
    

    $.validator.addMethod("dropDownVariedadId", function (value, element) {
        return value.length > 0 || ($('#esUva').is(':checked') == false);
    }, $('#dropDownVariedadId').data().errorRequerido);

    $("#Clase").change(function() {
        var $this = $(this);
        if ($this.val() == $this.data().claseGranel || $this.val() == "") {
            $("#Peso").val("").prop("disabled", true);
        } else {
            $("#Peso").prop("disabled", false);
        }
    }).change();
});

function disableInputs() {
    $('.disabled input').attr('disabled', 'disabled');

    if ($('#binpallet').is(':checked') == false) {
        $('.binpallet input').attr('disabled', 'disabled');
        $('.binpallet select').attr('disabled', 'disabled');
    } else {
        $('.binpallet input').removeAttr('disabled');
        $('.binpallet select').removeAttr('disabled', 'disabled');
    }

    if ($('#esUva').is(':checked') == false) {
        $('.uva input').attr('disabled', 'disabled');
        $('.uva select').attr('disabled', 'disabled');
    } else {
        $('.uva input').removeAttr('disabled');
        $('.uva select').removeAttr('disabled');
    }

    if ($('#commodity').is(':checked') == false) {
        $('.commodity select').attr('disabled', 'disabled');
    } else {
        $('.commodity select').removeAttr('disabled');
    }

    if ($('#EsCosecha').is(':checked') == false) {
        $('.cosecha input').attr('disabled', 'disabled');
    } else {
        $('.cosecha input').removeAttr('disabled');
    }
}