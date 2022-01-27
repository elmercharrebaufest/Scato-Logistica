$(document).ready(function ($) {
    $(document).on('click','.contingencia', function () {
        habilitarContingencia(urlContingencia);
    });
    $(document).on('click', '.sinAfip', function () {
        habilitarContingencia(urlSinAfip);
    });
    $(document).on('click', '.sinCupo', function () {
        habilitarContingencia(urlSinCupo);
    });
    $(document).on('click', '.sinFotoCartaPorte', function () {
        habilitarContingencia(urlSinFotoCartaPorte);
    }); 
    $(document).on('click', '.imprimeCartaPorte', function () {
        habilitarContingencia(urlImprimeCartaPorte);
    });
    $(document).on('click', '.imprimeTarjetaDeAcceso', function () {
        habilitarContingencia(urlImprimeTarjetaDeAcceso);
    });
    $(document).on('click', '.noAsignaCalleEnGaritaEntrada', function () {
        habilitarContingencia(urlNoAsignaCalleEnGaritaEntrada);
    });
    $(document).on('click', '.tomarFotoCartaDePorteEnCentro', function () {
        habilitarContingencia(urlTomarFotoCartaDePorteEnCentro);
    });
    $(document).on('click', '.descargaCartaPortePorCtg', function () {
        habilitarContingencia(urlDescargaCartaPortePorCtg);
    });
    $(document).on('click', '.solicitaConfirmarCTG', function () {
        habilitarContingencia(urlSolicitaConfirmarCTG);
    });
    $(document).on('click', '.validarCupo', function () {
        habilitarContingencia(urlValidarCupo);
    });
    $(document).on('click', '.encolaBajaCtgAutomatico', function () {
        habilitarContingencia(urlEncolaBajaCtgAutomatico);
    });

    $(document).on('click', '.nirsManual', function () {
        habilitarContingencia(urlNirsManual);
    });
    $(document).on('click', '.humedimetroManual', function () {
        habilitarContingencia(urlHumedimetroManual);
    });
    $(document).on('click', '.balanzaManual', function () {
        habilitarContingencia(urlBalanzaManual);
    });
    $(document).on('click', '.informarArriboACircular', function () {
        habilitarContingencia(urlInformarArriboACircularEnCentro);
    });
    $(document).on('click', '.contingenciaAfipCpe', function () {
        habilitarContingencia(urlContingenciaAfipCpe);
    });

    function habilitarContingencia(url) {
        if (ValidarMotivo()) { return false; }

        BlockUI();
        $.getJSON(url, { id: $("#elemento-id").val(), motivo: $("#motivo").val() }, function (data) {
            if (data !== "") {
                MostrarAlertaError(data.value);

            } else {
                MostrarAlertaExitosa();
            }
        }).complete(function () {
            $.unblockUI();
            $("#contingenciaModal").modal("hide");
        });
    }

    $(document).on('click', '.impresionTicketDeSalida', function () {
        if (ValidarMotivo()) { return false; }

        BlockUI();
        $.getJSON(urlImpresionTicketDeSalida, { motivo: $("#motivo").val() }, function (data) {
            if (data !== "") {
                MostrarAlertaError(data.value);

            } else {
                MostrarAlertaExitosa();
            }
        }).complete(function () {
            $.unblockUI();
            $("#contingenciaModal").modal("hide");
        });
    });

    $(document).on('click', '.habilitarGranos', function () {
        if (ValidarMotivo()) { return false; }

        BlockUI();
        $.getJSON(urlContingenciaGranos, {
            id: $("#elemento-id").val(),
            contingencia: $("#elemento-grano").val() == "true" ? $("#granos:checked").val() == "on" : $("#no-granos:checked").val() == "on",
            esGranos: $("#elemento-grano").val(),
            motivo: $("#motivo").val()
        }, function (data) {
            if (data !== "") {
                MostrarAlertaError(data.value);

            } else {
                MostrarAlertaExitosa();
            }
        }).complete(function () {
            $.unblockUI();
            $("#contingenciaModal").modal("hide");
        });
    });

    $(document).on('click', '.slider', function () {
        $("#elemento-id").val($(this)[0].getAttribute('data-id'));
        $("#elemento-grano").val($(this)[0].getAttribute('data-granos'));
        $("#elemento-contingencia").val($(this)[0].getAttribute('data-contingencia'));
        $("#guardar").removeClass();
        $("#guardar").addClass('btn btn-primary');
        $("#guardar").addClass($(this)[0].getAttribute('data-contingencia'));
        $("#motivo").val("");
        $("#error-requerido").hide(); 
        $("#error-largo").hide(); 

        $("#contingenciaModal").modal("show");
    });

    $(document).on('click', '#cancelar', function () {
        var contingencia = $("#elemento-contingencia").val();
        var id = $("#elemento-id").val();
        var inputs = [];
        if (id == "") {
            inputs = $('[data-contingencia=' + contingencia + ']').siblings();
        } else  {
            inputs = $('[data-contingencia=' + contingencia + '][data-id=' + id + ']').siblings();
        }
        inputs.trigger('click');
        $("#contingenciaModal").modal("hide");
    });
    function ValidarMotivo() {
        $("#error-requerido").hide();
        $("#error-largo").hide(); 
        var motivo = $("#motivo").val();
        if (motivo == "") {
            $("#error-requerido").show();
            return true;
        }
        if (motivo.length < 10) {
            $("#error-largo").show();
            return true;
        }
        return false;
    }
});
