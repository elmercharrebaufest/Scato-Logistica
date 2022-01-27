$(document).ready(function () {
    IniciarLogicaInactividad();
});

// Bloquear pantalla por inactividad
var modalVisibleInactividad = false;
var llamadaEnviadaInactividad = false;
var timerValidacionInactividad = null;

// Start timers.
function IniciarLogicaInactividad() {
    $.ajax({
        url: $('#linksRegistrarInactividad').data().urlValidaInactividad,
        data: {},
        type: "GET",
        success: function (data) {
            if (data && data.validar) {
                timerValidacionInactividad = setInterval(ConsultarUltimoCalado, 4000)
            } else if (timerValidacionInactividad){
                clearInterval(timerValidacionInactividad)
            }
        }
    });
}

function closeModal() {
    modalVisibleInactividad = false;
}

function RenderizarModalInactividadJs() {
        $.ajax({
            url: $('#riModal').data().urlRegistroInactividadModal,
            data: { },
            type: "GET",
            beforeSend: function () {
                modalVisibleInactividad = true;
            },
            success: function (data) {
                $("#riModal").html(data);
                $('#riAceptarMotivo').click(function (e) {
                    e.stopPropagation();
                    RegistrarInactividadJs();
                });
                $('#ValorComboMotivoInactividad').change(ValidarMotivoInactividad);
                $('#popup-inactividad-div-interno').modal({
                    backdrop: 'static', keyboard: false
                }).css({
                    width: function () {
                        return $('#popup-inactividad-div-interno').outerWidth() / 2;
                    }, 'margin-left': function () {
                        return -($(this).width() / 2);
                    },
                    'top': '50%',
                    'margin-top': function () {
                        return -($(this).height() / 2);
                    }
                });
                
            },
            error: function () {
                modalVisibleInactividad = false;
            }
        });
}

function RegistrarInactividadJs() {
    if (ValidarMotivoInactividad()) {
        $.ajax({
            url: $('#riAceptarMotivo').data().urlRegistrarInactividadUsuario,
            data: {
                motivoInactividadId: $('#ValorComboMotivoInactividad').val(), idRegistroInactividad: $('#RegistroInactividadId').val()
            },
            type: "POST",
            success: function (data) {
                $("#popup-inactividad-div-interno").modal("hide");
                closeModal();
            }
        });
    } else {
        $("#errorMotivoInactividadVacio").show();
    }
}

function ValidarMotivoInactividad() {
    var valor = $("#ValorComboMotivoInactividad").val();
    if (valor == "") {
        return false;
    } else {
        $("#errorMotivoInactividadVacio").hide();
        return true;
    }

}

function ConsultarUltimoCalado() {
    if (!llamadaEnviadaInactividad && !modalVisibleInactividad) {
        llamadaEnviadaInactividad = true;
        $.ajax({
            url: $('#linksRegistrarInactividad').data().urlConsultarUltimoCamionCalado,
            data: {},
            type: "GET",
            success: function (data) {
                if (data.mostrarPopUpInactividad) {
                    llamadaEnviadaInactividad = false;
                    RenderizarModalInactividadJs();
                }
            },
            error: function (data) {
                llamadaEnviadaInactividad = false;
            },
            complete: function (data) {
                llamadaEnviadaInactividad = false;
            }
        });
    }
}

// Despliega un pop up si se quiere salir de la página antes de aclarar motivo inactividad
window.addEventListener('beforeunload', function (e) {
    if (modalVisibleInactividad) {
        e.preventDefault(); //per the standard
        var mensaje = "Por favor, complete el motivo inactividad antes de salir"
        e.returnValue = mensaje;
        return mensaje;
    }
});



