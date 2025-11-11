jQuery(document).ready(function ($) {

    InicializarPantalla();
    
    $('#dropdownEstablecimientos').change(function() {
        validarCambioVariedadComun();
        procedenciaIgualALocalidad();
    });

    $('#dropdownEstablecimientos').focus();
    $(document).on('click', '.rechazar-boton', MostrarDialogoRechazar);
    $(document).on('click', '.demorar-boton', MostrarDialogoDemorar);

    $(window).keydown(function (e) {
        if (rechazar && e.keyCode === 13) {
            e.preventDefault();
            MostrarDialogoRechazar();
            return false;
        }
    });
});
rechazar = false;
function MostrarOcultarBtnRechazado(data) {
    if (data.establecimientoEsIgualAProcedencia) {
        $('#BotonAceptar').attr('disabled', false);
        $('#BotonAceptar').show();
        $('#errorProcedenciaLocalidad').hide();
        rechazar = false;
    } else {
        $('#BotonAceptar').attr('disabled', true);
        $('#BotonAceptar').hide();
        $('#errorProcedenciaLocalidad:first-child').text(data.errorMessage);
        $('#errorProcedenciaLocalidad').show();
        rechazar = true;
    }
}

function MostrarDialogoRechazar() {
    BlockUI();
    $.get($('.rechazar-boton').attr('href'), cargarDialogoRechazar);
    return false;
}

function MostrarDialogoDemorar() {
    BlockUI();
    $.get($('.demorar-boton').attr('href'), cargarDialogoDemorar);
    return false;
}

function InicializarPantalla()
{
    if ($('#hayErrores').val() == "True") {
        $('#BotonAceptar').attr('disabled', true);
        $('#BotonAceptar').hide();
    } else {
        $('#BotonAceptar').attr('disabled', true);
        $('#BotonAceptar').show();
        $('#errorProcedenciaLocalidad').hide();
    }
}

function procedenciaIgualALocalidad() {
    BlockUI();
    var url = $(links).data().urlProcedenciaEsIgualALocalidad;
    InicializarPantalla();
    $.ajax({
        url: url,
        data: { instanceId: $('#InstanciaWorkflow').val(), establecimientoId: $('#dropdownEstablecimientos').val() },
        dataType: "json",
        type: "GET",
        error: function() {
            alert("Ocurrió un error al obtener la procedencia");
        },
        success: function(data) {
            MostrarOcultarBtnRechazado(data);
            $.unblockUI();
        }
    });
}


function cargarDialogoRechazar(data) {
    $("#mensajeRechazar").html(data);

    $('#dialogo-rechazar').modal({
        backdrop: 'static', keyboard: false
    }).css({
        width: function () {
            return $('#dialogo-rechazar').outerWidth();
        }, 'margin-left': function () {
            return -($(this).width() / 2);
        },
        'top': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }
    });
    $.unblockUI();
}

function cargarDialogoDemorar(data) {
    $("#mensajeRechazar").html(data);

    $('#dialogo-demorar').modal({
        backdrop: 'static', keyboard: false
    }).css({
        width: function () {
            return $('#dialogo-rechazar').outerWidth();
        }, 'margin-left': function () {
            return -($(this).width() / 2);
        },
        'top': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }
    });
    $.unblockUI();
}

function validarCambioVariedadComun() {
    var tipoVariedadCodigo = $('#TipoVariedadCodigo').val();
    var esVisec = tipoVariedadCodigo && (tipoVariedadCodigo.toUpperCase() === "EPAEUDR" || tipoVariedadCodigo.toUpperCase() == "EUDR");
    var establecimiento = $('#dropdownEstablecimientos').val();
    if (esVisec && establecimiento == '-1') {
        MostrarAlertaAdvertencia("Se cambiará la variedad a Común");
    }
}