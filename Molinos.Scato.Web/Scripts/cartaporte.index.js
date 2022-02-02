
vehiculoDemorado = false;
jQuery(document).ready(function ($) { 
    activarCPE()
    $("#VehiculoDemorado").val(false);
    $('#FechaEmision').attr("readonly", "readonly");
    $('#FleteAPagar').prop('checked', true);
    $('#FletePagado').change(
        function () {
            $('#FletePagado').val(true);
            $('#FleteAPagar').prop('checked', false);
        }
    );
    $('#FleteAPagar').change(
        function () {
            $('#FleteAPagar').val(true);
            $('#FletePagado').prop('checked', false);
        }
    );
    //SignalR connection
    var notificador = $.connection.notificarUsuario;

    // Start the connection
    window.hubReady.done(function () {
        var centroId = $("#Centro_Id").val();
        notificador.server.unirseAGrupo(centroId + "|mesaDeEntrada");
       
    });

    

    //Validaciones
    $.validator.addMethod("cosechaValidacion", function (value, element) {
        if (vehiculoDemorado == true) return true;
        var cosecha = $('#Cosecha').val().split('-');
        if (parseInt(cosecha[0]) > parseInt(cosecha[1]))
            return false;
        return true;
    }, $('#Cosecha').data().error);

    $.validator.addMethod("procedenciaRequerido", function (value, element) {
        if (vehiculoDemorado == true) return true;
        var cosecha = $('#Procedencia').val();
        if (cosecha == "" || cosecha == null)
            return false;
        return true;
    }, $('#Procedencia').data().error);

    $.validator.addMethod("cosechaValidacionDiferencia", function (value, element) {
        if (vehiculoDemorado == true) return true;
        var cosecha = $('#Cosecha').val().split('-');
        if (parseInt(cosecha[1]) - parseInt(cosecha[0]) !== 1)
            return false;
        return true;
    }, $('#Cosecha').data().errordiferencia);

    $.validator.addMethod("cosechaValidacionMaterial", function (value, element) {
        if (vehiculoDemorado == true) return true;

        var cosecha = $('#Cosecha').val().split('-');
        var desde = parseInt(cosecha[0]);
        var hasta = parseInt(cosecha[1]);
        var materialId = $('#MaterialId').val();
        var lista = JSON.parse($("#ListaMateriales").val());
        var material ;
        for (var i = 0; i < lista.length; i++) {
            if (parseInt(lista[i].MaterialId) == parseInt(materialId)) {
                material = lista[i];
                break;
            }
        }
        if (material==null) {
            return false;
        }
       
        if (material.EsCosecha) {
            if (desde < material.VigenciaDesde || hasta > material.VigenciaHasta) {
                return false;
            } else {
                return true;
            }
        } else {
            return true;
        }
        
    }, $('#Cosecha').data().errorcosechavalidacionmaterial);

    //$.validator.addMethod("rtteComercialValidacion", function (value, element) {
    //    if (vehiculoDemorado == true) return true;
    //    if ($('#Intermediario').val().length > 0 && ($('#RtteComercial').val().length == 0))
    //        return false;
    //    return true;
    //}, $('#RtteComercial').data().error);

    $.validator.addMethod("FechaCPValidacion", function (value, element) {
        if (vehiculoDemorado == true) return true;
        if (Globalize.parseDate($('#FechaCP').val()) > Globalize.parseDate($('#FechaEmision').val()))
            return false;
        return true;
    }, $('#FechaCP').data().error);

    $.validator.addMethod("FechaCPValidacion", function (value, element) {
        if (vehiculoDemorado == true) return true;
        if (addDays(Globalize.parseDate($('#FechaCP').val()), parseInt($('#dias').val())) < Globalize.parseDate($('#FechaEmision').val()))
            return false;
        return true;
    }, $('#FechaCP').data().error2);

    $.validator.addMethod("FechaVtoValidacion", function (value, element) {
        if (vehiculoDemorado== true) return true;
        if (Globalize.parseDate($('#FechaEmision').val()) > Globalize.parseDate($('#FechaVto').val()))
            return false;
        return true;
    }, $('#FechaVto').data().error);

    $.validator.addMethod("FechaVtoValidacionMaxima", function (value, element) {
        if (vehiculoDemorado== true) return true;
        var fechaActual = Globalize.parseDate($('#FechaEmision').val());
        fechaActual.setDate(fechaActual.getDate() + 61);
        if (fechaActual < Globalize.parseDate($('#FechaVto').val()))
            return false;
        return true;
    }, $('#FechaVto').data().errorMaxima);

    $.validator.addMethod("anexoRequerido", function (value, element) {
        return value.length > 0;
    }, $('#mensajeAnexo').data().errorRequerido);

    $.validator.addMethod("codEstabRequerido", function (value, element) {
        return value.length > 0;
    }, $('#CodEstab').data().error);

    if ($('#RequiereCupo').val() == "True") {
        $.validator.addMethod("cupoValidacion", function (value, element) {
            if ($('#esIngreso').val() == "True") {
                return /^MOL[0-9]{4}\/[0-9]{8}$/.test(value);
            } else {
                return value.length <= 30 && value.length > 0;
            }
        }, $('#Cupo').data().error);
    }
    if ($('#RequiereNumeroAduana').val() == "True") {
        $.validator.addMethod("numeroAduanaValidacion", function (value, element) {
            return value.length > 0;
        }, $('#NumeroAduana').data().error);
        $.mask.definitions['9'] = '';
        $.mask.definitions['#'] = '[0-9]';
        $("#NumeroAduana").mask(new Date().getFullYear().toString().substr(-2) + "052IT65######a");
        $.mask.definitions['9'] = '[0-9]';
        $.mask.definitions['#'] = '';
    }
    $('#checkSinCupo').change(function () {
        if ($('#checkSinCupo').is(':checked')) {
            $("#Cupo").val("MOL1111/11111111");
            $('#divSpan6').removeClass('error');
            $('#Cupo').valid();
        } else {
            $("#Cupo").val("");
        };
    });
    $("#Cupo").on('keydown', function () {
        $('#checkSinCupo').prop('checked', false);
    });

    //Máscaras
    $('#FechaEmision').attr("readonly", "readonly");
    $('#FleteAPagar').prop('checked', true);
    $('#FletePagado').change(
        function () {
            $('#FleteAPagar').prop('checked', false);
        }
    );
    $('#FleteAPagar').change(
        function () {
            $('#FletePagado').prop('checked', false);
        }
    );
    DefinirFechas();
    $(".patente-internacional").mask("?*******", { placeholder: "" });
    $("#Cosecha").mask("99-99");
    if ($('#esIngreso').val() == "True") {
        $("#Cupo").mask("MOL9999/99999999")
    } 
    $('#bocaDestino').attr("disabled", true);
    $('#NroCartaPorte').addClass('cargarCartaPorte');
    $('#CTG').addClass('cargarCartaPorteCTG');
    $('#NumeroOperativo').addClass('cargarOperativo');

    if ($('#esIngreso').val() == "True") {
        MostrarAnexo();
        MostrarTecnologia();
        $('#MaterialId').change(function () {
            MostrarAnexo();
            MostrarTecnologia();
        });
        if ($("#EsClienteDestinatario").val() == "False") {
            $('#Destino').attr("readonly", true);
            BordeColor('#Destino', '#DestinoId');
        }
    }


    if ($('#esIngreso').val() == "True" || $('#habilitarSiempreCTG').val() == "True") {
        $.validator.addMethod("ctgRequerido", function (value, element) {
            return value.length > 0;
        }, $('#CTG').data().error);
    }
    else {
        $('#CTG').attr("readonly", "readonly");
        $('#TarifaReferencia').attr("readonly", "readonly");
        $('#TarifaReferencia').attr('tabindex', -1);
    }

    if ($('#esIngreso').val() != "True" && $("#habilitarSiemprePeso").val() != "True") {
        $("#DvPeso :input").attr("disabled", true);
    }
    if ($('#esIngreso').val() != "True" && $("#habilitarSiempreProcedencia").val() != "True") {
        DeshabilitarCampo($('#Procedencia'));
        $('#CodEstab').attr('readonly', true);
    }

    if ($("#deshabilitarTitular").val() == "True") {
        DeshabilitarCampo($('#TitularCartaPorte'));
    }
    if ($("#deshabilitarDestinatario").val() == "True") {
        DeshabilitarCampo($('#Destinatario'));
    }
    if ($("#deshabilitarEntregador").val() == "True") {
        DeshabilitarCampo($('#Entregador'));
    }
    if ($("#deshabilitarTransportista").val() == "True") {
        DeshabilitarCampo($('#Transportista'));
    }
    if ($("#deshabilitarPatentes").val() == "True") {
        DeshabilitarCampo($('#Patente'));
        DeshabilitarCampo($('#PatenteAcoplado'));
        DeshabilitarCampo($('#PatenteAcoplado2'));
    }
    if ($("#fechaVtoVacia").val() == "True") {
        $("#FechaVto").val("");
    }
    $('#DivNroCartaPorteOrigen').hide();
    if ($('#NroCartaPorteOrigen').val().length > 0) {
        $('#DivNroCartaPorteOrigen').show();
        DeshabilitarCampo($('#NroCartaPorteOrigen'));
    }

    //Eventos
    var cartaDePorte;
    $('.cargarCartaPorte').change(function () {
        var nroCartaPorte = $('.cargarCartaPorte').val();
        var usuario = $('#Usuario').val();
        if (nroCartaPorte.length == 12 && $.isNumeric(nroCartaPorte) && !$('#Cpe').is(':checked')) {
            BlockUI($("#MensajeBuscandoCartaPorte").val());
            // Mandar notificacion 
            window.hubReady.done(function () {
                notificador.server.notificar({
                    Mensaje: nroCartaPorte + ";" + usuario+ ";false",
                    Grupo: $("#Centro_Id").val() + "|mesaDeEntrada",
                    TipoAlerta: 9
                });
            });
            //
            //})
            $.getJSON($("#links").data().urlObtenerCartaPorte, { numero: nroCartaPorte, workflow: $('#workflow').val() }, function (data) {
               
                if (data.CodigoDeError == 0) {
                    cartaDePorte = data.CartaPorte;
                    // Muestro dialogo: Reutilizar: si, no
                    $("#dialogo-confirmar").modal('show');
                } else if (data.CodigoDeError == 1) {
                    $('.btn').removeAttr('disabled');
                    setearPatente();
                } else {
                    MostrarAlertaError(data.Error);
                    setearPatente();
                }
            }).complete(function () {
                $.unblockUI();
            });
        }

        
    });

    $(document).on('keydown', '.cargarCartaPorte', function (e) {
        var keyCode = e.keyCode || e.which;

        var ctg = $('.cargarCartaPorteCTG').val();
        if (keyCode === 9 && ctg.length === 8 && $.isNumeric(ctg)) {
            e.preventDefault();
            if ($('#FechaCP').val() === '' || $('#FechaCP').val() === null) {
                $('#FechaCP').focus();
            } else {
                $('#FechaVto').focus();
            }
        }
    });
    
    $('.cargarCartaPorteCTG').change(function () {
        var ctg = $('.cargarCartaPorteCTG').val();
        if (ctg.length == 8 && $.isNumeric(ctg) && !$('#Cpe').is(':checked')) {
            cargarCartaPortePorCtg();
            if ($("#FotoMesaDigitalizacion1").val() == "") {
                tomarFotoMesaDigitalizacion(1);
            }
        }
    });
    //var ctg = $('.cargarCartaPorteCTG').val();
    //var nroCartaPorteInicial = $('.cargarCartaPorte').val();

    //if (ctg.length == 8 && $.isNumeric(ctg) && nroCartaPorteInicial == '') {
    //    cargarCartaPortePorCtg();
    //    if ($("#FotoMesaDigitalizacion1").val() == "") {
    //        tomarFotoMesaDigitalizacion(1);
    //    }
    //}

    $('.tomarFoto1').click(function () {
        tomarFotoMesaDigitalizacion(1);
    });
    $('#foto1').mouseover(function () {
        if ($('.tomarFoto1:hover').length == 0) {
            $("#place-holder-1").attr("src", $('#foto1').attr("src"));

        }
    }).mouseout(function () {
        $("#place-holder-1").attr("src", '');
    });

    $('.tomarFoto2').click(function () {
        tomarFotoMesaDigitalizacion(2);
    });
    $('#foto2').mouseover(function () {
        if ($('.tomarFoto2:hover').length == 0) {
            $("#place-holder-2").attr("src", $('#foto2').attr("src"));

        }
    }).mouseout(function () {
        $("#place-holder-2").attr("src", '');
    });

    $('#dialogo-confirmar').bind('shown', function (event) {
        $("#dialogo-confirmar-confirmar").focus();
    });

    $('#dialogo-confirmar').bind('keypress', function (event) {
        if (event.keyCode == 27) {
            $('.close').click();
            return false;
        }
    });
    $('#dialogo-confirmar').bind('keypress', function (event) {
        if (event.keyCode === 13) {
            event.target.click();
            return false;
        }
    });

    $("#dialogo-confirmar-confirmar").on('click', function () {
        cargarCartaDePorte(cartaDePorte);
        //ValidarObjeto($("#orden-form"), $("#CEE"));
        return false;
    });

    $('#tipoVehiculoDropdown').change(
        function () {
            clearValidation();
            configuracionCPE();
            setearVistaTipoVehiculo();
        }
    );

    $(".aceptar-carga-porte").click(function () {
        $("#VehiculoDemorado").val(false);
        vehiculoDemorado = false;
        $("#MaterialId").attr("disabled", false);
        $("form").submit();

    });
    $("#pendiente").click(function () {
        var valido = true;
        var val = true;
        val = $("#NroCartaPorte").valid();
        if (val == false) { $("#NroCartaPorte").focus(); $("#NroCartaPorte").blur(); }
        valido = valido == true ? val : false;

        val = $("form").validate().element("#CTG");
        if (val == false) { $("#CTG").focus(); $("#CTG").blur(); }
        valido = valido == true ? val : false;

        val = $("form").validate().element("#TitularCartaPorte");
        valido = valido == true ? val : false;

        val = $("form").validate().element("#Entregador");
        valido = valido == true ? val : false;

        val = $("form").validate().element("#Patente");
        valido = valido == true ? val : false;
        if (val == false) { $("#Patente").focus(); $("#Patente").blur(); }

        val = $("form").validate().element("#TitularCartaPorte");
        valido = valido == true ? val : false;

        val = $("form").validate().element("#Chofer_Cuil");
        valido = valido == true ? val : false;

        val = $("form").validate().element("#Chofer_NumeroDeDocumento");
        valido = valido == true ? val : false;

        val = $("form").validate().element("#Chofer_Nombre");
        valido = valido == true ? val : false;

        val = $("form").validate().element("#Chofer_Apellido");
        valido = valido == true ? val : false;

        val = $("form").validate().element("#CodEstab");
        valido = valido == true ? val : false;
        if (val == false) { $("#CodEstab").focus(); $("#CodEstab").blur(); }

        val = $("form").validate().element("#Cupo");
        valido = valido == true ? val : false;
        if (val == false) { $("#Cupo").focus(); $("#Cupo").blur(); }

        val = $("form").validate().element("#MaterialId");
        valido = valido == true ? val : false;
        if (val == false) { $("#MaterialId").focus(); $("#MaterialId").blur(); }

        val = $("form").validate().element("#Procedencia");
        valido = valido == true ? val : false;
        if (val == false) { $("#Procedencia").focus(); $("#Procedencia").blur(); }

        if (valido) {

            $("#dialogo-demorar").modal("show");
        } else {
            return false;
        }

    });
   
    function ValidarMotivo() {
        $("#error-requerido").hide();
        $("#error-largo").hide();
        var motivo = $("#MotivoDemora").val();
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

    $("#dialogo-demorar-confirmar").click(function () {
        if (ValidarMotivo()) { return false; }
        $("#VehiculoDemorado").val(true);
        vehiculoDemorado = true;
        if ($("#FechaCP").val() == "") $("#FechaCP").val("31/01/1980");
        if ($("#FechaVto").val() == "") $("#FechaVto").val("31/01/2100");
        if ($("#TipoCategoriaId").val() == "") $("#TipoCategoriaId :first").val("1000");
        if ($("#Cosecha").val() == "") $("#Cosecha").val("99-99");
        if ($("#PesoBrutoOrigen").val() == "") $("#PesoBrutoOrigen").val("99999");
        if ($("#PesoTaraOrigen").val() == "") $("#PesoTaraOrigen").val("99999");
        if ($("#PesoNetoOrigen").val() == "") $("#PesoNetoOrigen").val("99999");
        if ($("#KmRecorrer").val() == "") $("#KmRecorrer").val("9999");
        if ($("#TarifaReferencia").val() == "") $("#TarifaReferencia").val("9999999999");
        
        $("form").submit();       
    });

    // para que el campo retome el foco al seleccionar una fecha
    $('input.date').datepicker("option", "onSelect", function () {
        $(this).focus();
    });

    var listarProveedores = $('#links').data().urlBuscarProveedores;
    var obtenerProveedor = $('#links').data().urlBuscarProveedor;
    var obtenerProveedorSap = $('#links').data().urlObtenerProveedoresSap;
    DefinirAutocompletarChofer();
    DefinirAutocompletarConSAP('#TitularCartaPorte', '#TitularCartaPorteId', '#autocompleteTCP', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    DefinirAutocompletarConSAP('#Intermediario', '#IntermediarioId', '#autocompleteInt', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    DefinirAutocompletarConSAP('#RtteComercial', '#RtteComercialId', '#autocompleteRtte', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    DefinirAutocompletarConSAP('#Corredor', '#CorredorId', '#autocompleteCorr', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, false, true, false);
    DefinirAutocompletarConSAP('#CorredorVendedor', '#CorredorVendedorId', '#autocompleteCorr', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, false, true, false);
    DefinirAutocompletarConSAP('#IntermediarioFlete', '#IntermediarioFleteId', '#autocompleteCorr', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    DefinirAutocompletarConSAP('#AgenteCompras', '#AgenteComprasId', '#autocompleteAgc', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, false, false, true);
    DefinirAutocompletarConSAP('#Destinatario', '#DestinatarioId', '#autocompleteDest', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    DefinirAutocompletarConSAP('#RtteComercialVentaSecundario', '#RtteComercialVentaSecundarioId', '#autocompleteRtte', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    DefinirAutocompletarConSAP('#CorredorVendedorSecundario', '#CorredorVendedorSecundarioId', '#autocompleteRtte', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, false, true, false);
    DefinirAutocompletarConSAP('#RtteComercialVentaSecundario2', '#RtteComercialVentaSecundario2Id', '#autocompleteRtte', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    if ($("#EsClienteDestinatario").val() == "True") {
        DefinirAutocompletarConSAP('#Destinatario', '#DestinatarioId', '#autocompleteDest', $('#links').data().urlBuscarClientes, $('#links').data().urlBuscarClienteUnico, $('#links').data().urlObtenerClientesSap);
        DefinirAutocompletarConSAP('#Destino', '#DestinoId', '#autocompleteDestino', $('#links').data().urlBuscarClientes, $('#links').data().urlBuscarClienteUnico, $('#links').data().urlObtenerClientesSap);

    } else {
        DefinirAutocompletarConSAP('#Destinatario', '#DestinatarioId', '#autocompleteDest', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
        DefinirAutocompletar('#Destino', '#DestinoId', $('#links').data().urlBuscarCentros, $('#links').data().urlBuscarCentroUnico);
    }
    DefinirAutocompletar('#Prestador', '#PrestadorId', $('#links').data().urlBuscarProveedoresconboca, $('#links').data().urlBuscarProveedorconbocaUnico, BuscarBocasDestino, BloquearBocasDestino);
    DefinirAutocompletar('#Procedencia', '#ProcedenciaId', $('#links').data().urlBuscarProcedencias, $('#links').data().urlBuscarProcedenciaUnica);
    DefinirAutocompletar('#Entregador', '#EntregadorId', $('#links').data().urlBuscarEntregadores, $('#links').data().urlBuscarEntregador, null, null, null, 7);
    DefinirAutocompletarTransportistaCartaPorte('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, false, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
    DefinirAutocompletarTransportistaCartaPorte('#TransportistaTramo2', '#TransportistaTramo2Id', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, false, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedorTramo2, onSelectTransportistaTramo2, true, false, false);
    $('#TipoComercialId').change(function () {
        DefinirAutocompletarTransportistaCartaPorte('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, false, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
        if (!$('#Transportista').hasClass('transportistaRequerido')) ValidarObjeto($("#orden-form"), $("#Transportista"));

        DefinirAutocompletarTransportistaCartaPorte('#TransportistaTramo2', '#TransportistaTramo2Id', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, false, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedorTramo2, onSelectTransportistaTramo2, true, false, false);
    });
    DefinirAutocompletarConSAP('#PagadorFlete', '#PagadorFleteId', '#autocompleteCorr', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    DefinirAutocompletar('#RepresentanteRecibidor', '#RepresentanteRecibidorId', $('#links').data().urlBuscarEntregadores, $('#links').data().urlBuscarEntregador, null, null, null, 7);

    if ($('#DescargaCartaPortePorCtg').is(':checked')) {
        $('#CTG').focus();
    } else {
        if (!$('#NroCartaPorte').val().length == 11 || !$('#NroCartaPorte').val().length == 12 || $('#NroCartaPorte').val().length == 0) {
            $('#NroCartaPorte').focus();
        }        
    }

    cargarTiposVehiculo(true);
    if ($("#FotoMesaDigitalizacion1").val() != "") {
        $("#foto1").attr("src", 'data:image/jpeg;base64,' +$("#FotoMesaDigitalizacion1").val());
        $('#fotoDiv').css('display', 'inline');
        $('.tomarFoto1').css('display', 'inline');
        $('.tomarFoto2').css('display', 'inline');
        $('.ocultar').hide();
        $('#tabFotos a[href="#fotoDiv1"]').tab('show');
        $('#foto1').elevateZoom({
            zoomType: "inner",
            cursor: "crosshair"
        });
    }
    if ($("#FotoMesaDigitalizacion2").val() != "") {
        $("#foto2").attr("src", 'data:image/jpeg;base64,' + $("#FotoMesaDigitalizacion2").val());
        $('#foto2').elevateZoom({
            zoomType: "inner",
            cursor: "crosshair"
        });
    }

    $(document).on('focus click', 'input', function (e) {
        setearSombreado(this.name);
    });
    $(document).on('focus click', 'select', function (e) {
        setearSombreado(this.name);
    });
    
    if ($.cookie("alineacionCP") !== undefined) {
        $('#alineacion').prop('checked', true);
        $('#primeraColumna').addClass("alinear-izquierda");
    } else {
        $('#primeraColumna').removeClass('alinear-izquierda');
    }

    $('#alineacion').change(function () {
        if (this.checked) {
            $('#primeraColumna').addClass("alinear-izquierda");
            $.cookie("alineacionCP", 1);
        } else {
            $('#primeraColumna').removeClass('alinear-izquierda');
            $.removeCookie("alineacionCP");
        }
    });

    $('#Cpe').change(function () {
        clearValidation();
        configuracionCPE();
    });

    $('#Sucursal').hide();
    $('#Sucursal').removeAttr("required");
    $('#guion-concatenacion').hide();
    $("#NroCartaPorte").rules("add", { regex: /^00\d{10}$/ });
    $('#NroCartaPorte').attr("maxlength", "12");
    callEventsCpe();

    $('#Sucursal').change(function () {
        if ($('#Cpe').is(':checked')) {
            leadingZerosSucursal();
        }
    });

    if ($('#Cpe').is(':checked')) {
        leadingZerosSucursal();
    }

    if ($('#Cpe').is(':checked') && ($('.cargarCartaPorte').val().length == 11 || $('.cargarCartaPorte').val().length == 12) && $('#cartaPorteId').val() == 0) {
        $('#NroCartaPorte').trigger('change');
    }

    $('#NroCartaPorte').change(function () {
        if ($('#Cpe').is(':checked')) {
            var nroCartaPorte = $('.cargarCartaPorte').val();
            var usuario = $('#Usuario').val();            
            if ((nroCartaPorte.length == 11 || nroCartaPorte.length == 12) && $.isNumeric(nroCartaPorte)) {
                BlockUI($("#MensajeBuscandoCartaPorte").val());
                // Mandar notificacion 
                window.hubReady.done(function () {
                    notificador.server.notificar({
                        Mensaje: nroCartaPorte + ";" + usuario + ";false",
                        Grupo: $("#Centro_Id").val() + "|mesaDeEntrada",
                        TipoAlerta: 9
                    });
                });

                $.getJSON($("#links").data().urlObtenerCartaPorteCtgCompragranos, { numeroCtg: nroCartaPorte, workflow: $('#workflow').val(), tipoVehiculo: !$("#tipoVehiculoDropdown").val() ? "0" : $("#tipoVehiculoDropdown").val(), consultactg: $('#DescargaCartaPortePorCtg').is(':checked') }, function (data) {
                    var nroIngresado = $('#NroCartaPorte').val();
                    console.log(data);
                    if (data.CodigoDeError + "" === "0" || data.CodigoDeError + "" === "1" || data.CodigoDeError + "" === "3" || data.CodigoDeError + "" === "4") {
                        if (data.CodigoDeError === "0" && nroIngresado != "" && nroIngresado != null && data.Cpe.NroCartaPorte != "" && data.Cpe.NroCartaPorte != null && nroIngresado != data.Cpe.NroCartaPorte) {
                            MostrarAlertaError("El número de CTG obtenido de afip (" + data.Cpe.NroCartaPorte + ") no coincide con el ingresado (" + nroIngresado + ")");
                            data.CodigoDeError = 1;
                            setearPatente();
                        } else if (data.CodigoDeError + "" === "1") {
                            MostrarAlertaInfo(data.Error);
                            $('.btn').removeAttr('disabled');
                            setearPatente();
                        } else if (data.CodigoDeError + "" === "0") {
                            cartaDePorte = data.Cpe;
                            // Muestro dialogo: Reutilizar: si, no
                            $("#dialogo-confirmar").modal('show');
                        } else if (data.CodigoDeError + "" === "3") {
                            cargarCartaDePorte(data.Cpe);
                        } else if (data.CodigoDeError + "" === "4") {
                            MostrarAlertaAdvertencia(data.Error);
                            cargarCartaDePorte(data.Cpe, true);
                        }
                    } else {
                        MostrarAlertaError(data.Error);
                        $('.btn').removeAttr('disabled');
                        setearPatente();
                    }

                    elementoFocusOut('Destinatario');
                    elementoFocusOut('Transportista');
                    elementoFocusOut('RtteComercial');
                    elementoFocusOut('TitularCartaPorte');
                    elementoFocusOut('Corredor');
                    elementoFocusOut('Cosecha');
                    elementoFocusOut('Cupo');
                    $("#FechaCP").datepicker("hide");
                }).complete(function () {
                    $.unblockUI();
                });
            }
        };
    });

    //FechaCP
    if ($('#fechaCPIni').val() != null && $('#fechaCPIni').val() != "") {
        $('#FechaCP').val($('#fechaCPIni').val());
    }
    //FechaVto
    if ($('#fechaVTOcp').val() != null && $('#fechaVTOcp').val() != "") {
        $('#FechaVto').val($('#fechaVTOcp').val());
    }

    //Patente
    setearPatente();
    configuracionCPE();

    if ($("#EsModificacion").val() == 'True' && $("#esIngreso").val() != 'True') {
        $('#NroCartaPorte').prop('required', false);
        $('#NroCartaPorte').rules('remove', 'required');
        $('#Sucursal').prop('required', false);
        $('#Sucursal').rules('remove', 'required');
        $('#CTG').rules('remove', 'required');
        $('#CTG').prop('required', false);
        $('#CTG').removeClass('ctgRequerido');
        $('#CTG').attr("readonly", true);
    }

    //Si es Egreso bloqueo CTG y CPE
    if ($("#esEgreso").val() == 'True') {
        $('#NroCartaPorte').attr("readonly", true);
        $('#Sucursal').attr("readonly", true);
        $('#Cupo').attr("maxlength", "30");
    }

    $('#DescargaCartaPortePorCtg').change(
        function () {
            clearValidation();
            configuracionCPE();
        }
    );

    $('.cargarOperativo').change(function () {
        if ($('#Cpe').is(':checked')) {
            var nroCartaPorte = $('#NumeroOperativo').val();
            var usuario = $('#Usuario').val();
            var tipoVehiculo = $('#tipoVehiculoDropdown :selected').text();

            if (nroCartaPorte.length >= 1 && $.isNumeric(nroCartaPorte) && tipoVehiculo == "Tren" && $('#esIngreso').val() == "True") {
                BlockUI($("#MensajeBuscandoCartaPorte").val());
                // Mandar notificacion 
                window.hubReady.done(function () {
                    notificador.server.notificar({
                        Mensaje: nroCartaPorte + ";" + usuario + ";false",
                        Grupo: $("#Centro_Id").val() + "|mesaDeEntrada",
                        TipoAlerta: 9
                    });
                });

                $.getJSON($("#links").data().urlObtenerCartaPorteCtgCompragranos, { numeroCtg: nroCartaPorte, workflow: $('#workflow').val(), tipoVehiculo: !$("#tipoVehiculoDropdown").val() ? "0" : $("#tipoVehiculoDropdown").val(), consultactg: $('#DescargaCartaPortePorCtg').is(':checked') }, function (data) {
                    var nroIngresado = $('#NroCartaPorte').val();
                    
                    if (data.CodigoDeError + "" === "0" || data.CodigoDeError + "" === "1" || data.CodigoDeError + "" === "3" || data.CodigoDeError + "" === "4") {
                        if (data.CodigoDeError === "0" && nroIngresado != "" && nroIngresado != null && data.Cpe.NroCartaPorte != "" && data.Cpe.NroCartaPorte != null && nroIngresado != data.Cpe.NroCartaPorte) {
                            MostrarAlertaError("El número de CTG obtenido de afip (" + data.Cpe.NroCartaPorte + ") no coincide con el ingresado (" + nroIngresado + ")");
                            data.CodigoDeError = 1;
                            setearPatente();
                        } else if (data.CodigoDeError + "" === "1") {
                            MostrarAlertaInfo(data.Error);
                            $('.btn').removeAttr('disabled');
                            setearPatente();
                        } else if (data.CodigoDeError + "" === "0") {
                            cartaDePorte = data.Cpe;
                            // Muestro dialogo: Reutilizar: si, no
                            $("#dialogo-confirmar").modal('show');
                        } else if (data.CodigoDeError + "" === "3") {
                            cargarCartaDePorte(data.Cpe);
                        } else if (data.CodigoDeError + "" === "4") {
                            MostrarAlertaAdvertencia(data.Error);
                            cargarCartaDePorte(data.Cpe, true);
                        }
                    } else {
                        MostrarAlertaError(data.Error);
                        $('.btn').removeAttr('disabled');
                        setearPatente();
                    }

                    elementoFocusOut('Destinatario');
                    elementoFocusOut('Transportista');
                    elementoFocusOut('RtteComercial');
                    elementoFocusOut('TitularCartaPorte');
                    elementoFocusOut('Corredor');
                    elementoFocusOut('Cosecha');
                    elementoFocusOut('Cupo');
                    $("#FechaCP").datepicker("hide");
                }).complete(function () {
                    $.unblockUI();
                });
            }
        };
    });

    $(".foto1-conteiner").attr("style", "margin-top: 0px !import;");
});



function cargarCartaPortePorCtg() {
    if ($('#DescargaCartaPortePorCtg').is(':checked')) {
        BlockUI($("#MensajeBuscandoCartaPorte").val());
        var ctg = $('.cargarCartaPorteCTG').val();
        $.getJSON($("#links").data().urlObtenerCartaPorteCtg, { numeroCtg: ctg }, function (data) {
            if (data.CodigoDeError === "0" || data.CodigoDeError == 1) {
                var nro = data.CartaPorte.NroCartaPorte;
                var nroIngresado = $('#NroCartaPorte').val();
                if (data.CodigoDeError == 1) {
                    MostrarAlertaInfo(data.Error);
                }
                else if (nroIngresado != "" && nroIngresado != null && nro != "" && nro != null && nroIngresado != nro) {
                    MostrarAlertaError("El número de carta de porte obtenido de afip (" + nro + ") no coincide con el ingresado (" + nroIngresado + ")");
                    data.CodigoDeError = 1;
                }

                cargarCartaDePorte(data.CartaPorte, data.CodigoDeError == 1);
                

                elementoFocusOut('Destinatario');
                elementoFocusOut('Transportista');
                elementoFocusOut('RtteComercial');
                elementoFocusOut('TitularCartaPorte');
                elementoFocusOut('Corredor');
                $("#FechaVto").val("");
            } else {
                MostrarAlertaInfo(data.Error);
                $('.btn').removeAttr('disabled');
            }
            $("#FechaCP").datepicker("hide");
        }).complete(function () {
            $('.cargarCartaPorte').focus();
            $.unblockUI();
        });
    }
}

function tomarFotoMesaDigitalizacion(nroFoto) {
    $.getJSON($("#links").data().urlObtenerFotoMesa, null,
        function (data) {
            if (data.CodigoDeError == 0) {
                $('#foto' + nroFoto).attr("src", 'data:image/jpeg;base64,' + data.Foto);
                //$('#fotoDiv' + nroFoto).css('width', '290px');

                $('#fotoDiv').css('display', 'inline');
                $('.tomarFoto1').css('display', 'inline');
                $('.tomarFoto2').css('display', 'inline');
                $('#FotoMesaDigitalizacion' + nroFoto).val(data.Foto);
                $('#PuestoDeTrabajo').val(data.PuestoDeTrabajo);
                $('.ocultar').hide();
                $('#tabFotos a[href="#fotoDiv' + nroFoto + '"]').tab('show');
                $('#foto' + nroFoto).elevateZoom({
                    zoomType: "inner",
                    cursor: "crosshair"
                });
            } else if (data.CodigoDeError == 1 || data.CodigoDeError == 2) {
                MostrarAlertaInfo(data.Error.Descripcion);
            } else {
                MostrarAlertaError(data.Error.Descripcion);
            }
        });
}

function cargarCartaDePorte(cartaDePorte, sinAlerta) {
    //LimpiarCartaPorte();
    ReutilizarCartaPorte(cartaDePorte);
    $('.btn').removeAttr('disabled');
    if (sinAlerta == undefined || sinAlerta == null || sinAlerta == false) {
        MostrarAlertaExitosa();
    }
    $("#dialogo-confirmar").modal('hide');
    setearVistaTipoVehiculo(cartaDePorte);
}

function elementoFocusOut(elemento) {
    if ($('#' + elemento + 'Id').val() != 0) {
        $('#' + elemento).css('border', 'solid 1px green');
        $('#' + elemento).css('border-right', 'solid 5px green');
    } else {
        $('#' + elemento).css('border', 'solid 1px red');
        $('#' + elemento).css('border-right', 'solid 5px red');
    }
    if ($('#' + elemento).val() == "")
        $('#' + elemento).css('border', 'solid 1px #CCCCCC');
}

function MostarAnexo() {
    var resultado = jQuery.inArray($('#MaterialId').val(), $('#divanexo').data().materialesAnexos);

    if (resultado == -1) {
        $('#RequiereAnexoInase').val(false);
        $('#divanexo').addClass('hide');
        $('#CodigoAnexo').val('');
        $('#CodigoAnexo').removeClass('anexoRequerido');
    } else {
        $('#RequiereAnexoInase').val(true);
        $('#divanexo').removeClass('hide');
        //$('#CodigoAnexo').val('');
        $('#CodigoAnexo').addClass('anexoRequerido');
    }
}

function BuscarBocasDestino() {
    $.getJSON($('#links').data().urlBuscarBocas, { proveedorId: $("#PrestadorId").val() },
        function (response) {
            var options = '';
            for (var i = 0; i < response.length; i++) {
                options += "<option value='" + response[i].Value + "'>"
                    + response[i].Text + "</option>";
            }
            $('#bocaDestino').html(options);
            $('#bocaDestino').attr("disabled", false);
        });
}

function BloquearBocasDestino() {
    $('#bocaDestino').attr("disabled", true);
    $('#bocaDestino').html('');
}

function DefinirFechas() {
    var formatoFecha = Globalize.culture().calendars.standard.patterns.d.replace(/[a-z]/g, '9');
    formatoFecha = formatoFecha.replace(/[A-Z]/g, '9');
    $('.FechaCP input').mask(formatoFecha);
    $('.FechaVto input').mask(formatoFecha);
    $(".FechaCP input").datepicker("option", "maxDate", $(".FechaEmision input").val());
    $(".FechaVto input").datepicker("option", "minDate", $(".FechaEmision input").val());
    var fechaActual = Globalize.parseDate($('#FechaEmision').val());
    if (fechaActual != null) {
        fechaActual.setDate(fechaActual.getDate() + 61);
        $(".FechaVto input").datepicker("option", "maxDate", fechaActual);
    }
}

function onSelectProveedor() {
    $('#EsTransportista').val(false);
}

function onSelectTransportista() {
    $('#EsTransportista').val(true);
}

function onSelectProveedorTramo2() {
    $('#EsTransportistaTramo2').val(false);
}

function onSelectTransportistaTramo2() {
    $('#EsTransportistaTramo2').val(true);
}

function addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}

function cargarTiposVehiculo(bool) {
    $.getJSON($('#links').data().urlObtenertiposvehiculo, { conTren: bool },
        function (response) {
            var options = '';
            for (var i = 0; i < response.length; i++) {
                options += "<option data-netoMaximo='" + response[i].netoMaximo + "'  data-brutoMaximo='" + response[i].brutoMaximoEgreso + "'  data-netoMinimo='" + response[i].netoMinimo + "' value='" + response[i].tipoVehiculoValue + "'" + ">"
                    + response[i].tipoVehiculoText + "</option>";
            }
            $('#tipoVehiculoDropdown').html(options);
            $('#tipoVehiculoDropdown').val($('#TipoVehiculoInt').val());
            if ($("#tipoVehiculoDropdown").val() != 1) {
                $('#tabFotos li[class="fotoLi2"]').hide();
            }
        });
}

function setearSombreado(nombre) {
    var alturaImagen = 1424.05;
    var anchoImagen = 812.3;
    //Hsup  Hinf scroll margin posicionOriginal 
    var posicion = [
        [["TipoVehiculo", "TipoComercialId", "NroCartaPorte", "CEE", "CTG", "FechaEmision", "FechaCP", "FechaVto", "TipoCategoriaId"], [60, 1070, 0, 0, 141]],
        [["TitularCartaPorte", "Intermediario", "RtteComercial", "Corredor", "AgenteCompras", "CorredorVendedor", "Entregador", "Destinatario", "Destino", "IntermediarioFlete", "Transportista"], [180, 740, 180, 0, 327]],
        [["Chofer.Cuil", "EsExtranjero", "Chofer.TipoDocumentoIdentidadId", "Chofer.NumeroDeDocumento", "Chofer.Nombre", "Chofer.Apellido"], [180, 740, 350, 70, 688]],
        [["MaterialId", "Variedad", "Cosecha", "Procedencia", "CodEstab", "TrigoEspecial", "Cupo", "SinCupo", "PesoBrutoOrigen", "PesoTaraOrigen", "PesoNetoOrigen"], [500, 580, 500, 110, 818]],
        [["OrigenVehiculo", "Patente", "PatenteAcoplado", "PatenteAcoplado2", "KmRecorrer", "TarifaReferencia", "TarifaTonelada"], [700, 430, 800, 200, 1095]]
    ];
    if ($(window).width() >= 1920) {
        alturaImagen = 1670;
        anchoImagen = 1080;
        posicion[0][1] = [70, 1254, 0, 0, 141];
        posicion[1][1] = [231, 820, 0, 0, 327];
        posicion[2][1] = [231, 820, 0, 0, 688];
        posicion[3][1] = [670, 610, 329, 0, 818];
        posicion[4][1] = [970, 410, 329, 0, 1095];
    }
    var valores = posicion.filter(function (e) { return e[0].filter(function (x) { return x == nombre; })[0] != undefined; });
    var alto = 0;
    var post = 0;
    var margin = 0;
    if (valores.length > 0 && $('[name="' + valores[0][0][0] + '"]').length > 0) {
        var sector = valores[0][1];

        alto = sector[0];
        post = sector[1];
        var deltaPosicion = $('[name="' + valores[0][0][0] + '"]').offset().top - sector[4];

        if ($(window).scrollTop() < (sector[2] + deltaPosicion)) {
            scrollTo(0, (sector[2] + deltaPosicion));
        }

        margin = sector[3] + deltaPosicion;
    }
    //$('.columnaFoto').css('margin-top', margin);

    //$('#opacitysup').css('height', alto);
    //$('.sombrado').css('width', anchoImagen + 'px');
    //$('#opacityinf').css('height', (post) + 'px');
    //$('#opacityinf').css('top', (alturaImagen - post + margin) + 'px');
}

function setearVistaTipoVehiculo(cartaPorte) {
    var tipoVehiculo = $('#tipoVehiculoDropdown :selected').text();
    if (tipoVehiculo == "Tren") {
        $.post($('#vehiculo').data().vagonjson, { model: JSON.stringify(cartaPorte) }, function (data) {
            $('#vehiculo').html(data);
        });
        $('#tabFotos li[class="fotoLi2"]').show();
        //$('#tabFotos li[class="fotoLi2"]').removeClass('disabled');
        //$('#tabFotos li[class="fotoLi2"]').find('a').attr("data-toggle", "tab");
    } else if ($('#DvDatosVagon').length > 0 || cartaPorte !== undefined) {
        $.post($('#vehiculo').data().camionjson, { model: JSON.stringify(cartaPorte) }, function (data) {
            $('#vehiculo').html(data);
        });
        $('#tabFotos a[href="#fotoDiv1"]').tab('show')
        $('#tabFotos li[class="fotoLi2"]').hide();
        $("#place-holder-2").attr("src","");
        $("#foto2").attr("src", "");
        $("#fotoMesaDigitalizacion2").val("");
        
        //$('#tabFotos li[class="fotoLi2"]').addClass('disabled');
        //$('#tabFotos li[class="fotoLi2"]').find('a').removeAttr("data-toggle");
    } else {
        ValidarObjeto($("form"), $("#PesoBrutoOrigen"));
        ValidarObjeto($("form"), $("#PesoNetoOrigen"));
    }
}

function NotificarCPMesaEntrada(mensaje) {
    var cp = Number($('#NroCartaPorte').val());
    var usuario = $('#Usuario').val();
    var m = mensaje.split(";");    
    var notificador = $.connection.notificarUsuario;
    if (Number(m[0]) == cp && m[2] == "false") {
        window.hubReady.done(function () {
            notificador.server.notificar({
                Mensaje: $('#NroCartaPorte').val() + ";" + usuario + ";true" ,
                Grupo: $("#Centro_Id").val() + "|mesaDeEntrada",
                TipoAlerta: 9
            });
        });
    } else if (Number(m[0]) == cp && m[2] == "true") {
        MostrarAlertaError("La carta de porte Nro:" + m[0] + " esta siendo cargada por " + m[1]);
    }

}

function ClearCpe() {
    $("#NroCartaPorte").val("");
    $("#Sucursal").val("");
    $("#CTG").val("");
}

function isNumberKey(e) {
    if (e.keyCode == '9' || e.keyCode == '16') {
        return;
    }
    var code;
    if (e.keyCode) code = e.keyCode;
    else if (e.which) code = e.which;
    if (e.which == 46)
        return false;
    if (code == 8 || code == 46)
        return true;
    if (code < 48 || code > 57)
        return false;
}

function clearValidation() {
    /*$('.validation-summary-errors').hide();*/
    $(".error").removeClass("error");
    $(".field-validation-error").html("");
    $(".field-validation-error").removeClass("field-validation-error").addClass("field-validation-valid");
}

function showTextBoxCpe() {
    $("label[for*='NroCartaPorte']").text("Nº de CTG");
    $("label[for*='CTG']").text("N° CPE (Sucursal - Orden)");
    $('#Sucursal').rules('add', 'required');
    $('#NroCartaPorte').rules('remove', 'regex');
    $('#Sucursal').show();
    $('#guion-concatenacion').show();
    $('#NroCartaPorte').attr("minlength", "11");
    $('#NroCartaPorte').attr("maxlength", "12");
    $.validator.messages.minlength = "El campo 'Nº de CTG' debe tener una longitud mínima de 11";
    $.validator.messages.maxlength = "El campo 'Nº de CTG' debe tener una longitud máxima de 12";
    $('input[name="NroCartaPorte"]').rules('add', {
        messages: {
            required: "El Campo 'Nº de CTG' es requerido"
        }
    });
    $('input[name="Sucursal"]').rules('add', {
        messages: {
            required: "Sucursal es requerida",
        }
    });
    $("#observaciones-div").show();
}

function hiddenTextBoxCpe() {
    $("label[for*='NroCartaPorte']").text("Nº de Carta de Porte");
    $("label[for*='CTG']").text("C.T.G");
    $('#Sucursal').removeAttr("required");
    $("#NroCartaPorte").rules("add", { regex: /^00\d{10}$/ });
    $('#Sucursal').hide();
    $('#guion-concatenacion').hide();
    $('#NroCartaPorte').removeAttr("minlength");
    $('#NroCartaPorte').attr("maxlength", "12");
    $('input[name="NroCartaPorte"]').rules('add', {
        messages: {
            required: "El Campo 'Nº de Carta de Porte' es requerido",
            regex: "El N° de carta de porte debe tener 12 dígitos y los dos primeros deben ser ceros"
        }
    });
    $("#Observacion").val("");
    $("#observaciones-div").hide();

}

function callEventsCpe() {    
    if ($('#Cpe').is(':checked')) {
        showTextBoxCpe();
        if (!$("#NroCartaPorte").val()) {
            $("#NroCartaPorte").val("");
        }        
    } else {
        hiddenTextBoxCpe();
        $("#Sucursal").val("");
    }
}

function leadingZerosSucursal() {
    if ($('#Sucursal').val().length > 0) {
        var cadena = "000000000" + $("#Sucursal").val();
        var format = cadena.substr(cadena.length - 5);
        $('#Sucursal').val(format);
    }
}

function setearPatente() {
    if ($('#vehiculoJsonModel').val() != null && $('#vehiculoJsonModel').val() != "") {
        $("#VehiculoJson").val($('#vehiculoJsonModel').val());
    }
}

function validarVehiculoFerroviarioCPE() {
    var tipoVehiculo = $('#tipoVehiculoDropdown :selected').text();
    if ((tipoVehiculo == "Tren" || $('#TipoVehiculoModel').val() === "1") && $('#Cpe').is(':checked')) {
        $("label[for*='NroCartaPorte']").text("Nº de Operativo");
        $('#NroCartaPorte').attr("minlength", "1");
        $('#NroCartaPorte').attr("maxlength", "10");
        $.validator.messages.minlength = "El campo 'Nº de Operativo' debe tener una longitud mínima de 1";
        $.validator.messages.maxlength = "El campo 'Nº de Operativo' debe tener una longitud máxima de 10";
        $('input[name="NroCartaPorte"]').rules('add', {
            messages: {
                required: "El Campo 'Nº de Operativo' es requerido"
            }
        });
        $('#Sucursal').prop('required', false);
        $('#Sucursal').rules('remove', 'required');
        $('#CTG').rules('remove', 'required');
        $('#CTG').prop('required', false);
        $('#CTG').removeClass('ctgRequerido');
        $('.row-tren-ferroviario-hidden').removeClass('row-tren-ferroviario-hidden').addClass('row-tren-ferroviario-visibility');
    } else {
        $("label[for*='NroCartaPorte']").text("Nº de Carta de Porte");
        $('#CTG').prop('required', true);
        $('#CTG').rules('add', 'required');
        $('#CTG').addClass('ctgRequerido');
        callEventsCpe();
        $('.row-tren-ferroviario-visibility').removeClass('row-tren-ferroviario-visibility').addClass('row-tren-ferroviario-hidden');
    }
}

function ConfiguracionesCPE() {
    var tipoVehiculo = $('#tipoVehiculoDropdown :selected').text();
    if (tipoVehiculo == "Tren" && $('#Cpe').is(':checked')) {
        validarVehiculoFerroviarioCPE();
        $("#Cpe").val(true);
        if ($('#esIngreso').val() == "True") {
            $('.row-sucursal-ctg').hide();
        } else {
            $('.row-sucursal-ctg').show();
        }
        $('.row-ramal-precinto').show()
    } else {
        $('#CTG').addClass('ctgRequerido');
        callEventsCpe();
        $('.row-tren-ferroviario-visibility').removeClass('row-tren-ferroviario-visibility').addClass('row-tren-ferroviario-hidden');
        $('.row-ramal-precinto').hide()
    }
    $('#NroCartaPorte').focus();
}

function configuracionCPE() {
    var tipoVehiculo = $('#tipoVehiculoDropdown :selected').text();
    var tipoVehiculoModel = $('#TipoVehiculoModel').val() === "1";
    var esIngreso = $('#esIngreso').val() == "True";

    if (tipoVehiculo == "Tren" || tipoVehiculoModel) { //Si es un Tren
        if ($('#Cpe').is(':checked')) {
            $("#Cpe").val(true);

            //Valido si es ingreso.
            if (esIngreso) {
                //Configuracion CPE Ferroviaria con 
                //$("label[for*='NroCartaPorte']").text("Nº de Operativo");
                //$('#NroCartaPorte').attr("minlength", "1");
                //$('#NroCartaPorte').attr("maxlength", "10");
                //$.validator.messages.minlength = "El campo 'Nº de Operativo' debe tener una longitud mínima de 1";
                //$.validator.messages.maxlength = "El campo 'Nº de Operativo' debe tener una longitud máxima de 10";
                //$('input[name="NroCartaPorte"]').rules('add', {
                //    messages: {
                //        required: "El Campo 'Nº de Operativo' es requerido"
                //    }
                //});

                $('#Sucursal').prop('required', false);
                $('#Sucursal').rules('remove', 'required');
                $('#CTG').rules('remove', 'required');
                $('#CTG').prop('required', false);
                $('#CTG').removeClass('ctgRequerido');
                $('#NroCartaPorte').rules('remove', 'regex');
                $('.row-tren-ferroviario-hidden').removeClass('row-tren-ferroviario-hidden').addClass('row-tren-ferroviario-visibility');

                //Configuracion Ramal y Numeor de Precinto
                $('#CodigoRamalId').prop('required', true);
                $('select[name="CodigoRamalId"]').rules('add', {
                    messages: {
                        required: "El Campo 'Ramal' es requerido"
                    }
                });
                $('#NumeroPrecinto').prop('required', true);
                $('input[name="NumeroPrecinto"]').rules('add', {
                    messages: {
                        required: "El Campo 'Número de Precinto' es requerido"
                    }
                });                
            } else {
                callEventsCpe();
                //Configuracion Ramal y Numeor de Precinto
                $('#CodigoRamalId').prop('required', true);
                $('select[name="CodigoRamalId"]').rules('add', {
                    messages: {
                        required: "El Campo 'Ramal' es requerido"
                    }
                });
                $('#NumeroPrecinto').prop('required', true);
                $('input[name="NumeroPrecinto"]').rules('add', {
                    messages: {
                        required: "El Campo 'Número de Precinto' es requerido"
                    }
                });
            }
            $('.numero-operativo').show();
            if ($('#DescargaCartaPortePorCtg').is(':checked')) {
                configuracionCargarFerroviarioPorCtg();
            } else {
                configuracionCargarFerroviarioPorOperativo();
            }
        } else {
            hiddenTextBoxCpe();
            $('#CodigoRamalId').prop('required', false);
            $('#NumeroPrecinto').prop('required', false);
            $('.numero-operativo').hide();
        }
        
        if ($('#Cpe').is(':checked') && $('#esIngreso').val() == "True") { //Si es tren con el check de CPE y es Ingreso
            $('.row-sucursal-ctg').hide();
        } else {
            $('.row-sucursal-ctg').show();
        }

        $('.row-ramal-precinto').show();
        $('.row-tranportista-tramo2').show();
        
    } else { //Si es un Camion
        $('.row-sucursal-ctg').show();
        $('.row-ramal-precinto').hide();
        $('.numero-operativo').hide();        
        $('#CTG').addClass('ctgRequerido');
        $('.row-tren-ferroviario-visibility').removeClass('row-tren-ferroviario-visibility').addClass('row-tren-ferroviario-hidden');

        //Quitando la Configuracion Ferroviaria CPE
        $("label[for*='NroCartaPorte']").text("Nº de Carta de Porte");
        $('#CTG').prop('required', true);
        $('#CTG').rules('add', 'required');
        $('#CTG').addClass('ctgRequerido');

        if ($('#Cpe').is(':checked')) {
            showTextBoxCpe();
            if (!$("#NroCartaPorte").val()) {
                $("#NroCartaPorte").val("");
            }
        } else {
            hiddenTextBoxCpe();
            $("#Sucursal").val("");
        }

        $('#CodigoRamalId').prop('required', false);
        $('#NumeroPrecinto').prop('required', false);
        $('.row-tranportista-tramo2').hide();
        $('.numero-operativo').hide();
    }
}
function activarCPE() {
    $('#Cpe').prop('checked', true);
    $('#Cpe').val(true)
    $('#Cpe').trigger("change");
    $('.check-cpe').hide();
}

function obtenerFotoCartaPorteElectronica(nroCTG) {
    try {
        if (nroCTG != '') {
            //Clear
            $('#foto1').attr("src", '');
            $('#fotoDiv').css('display', 'inline');
            $('.tomarFoto1').css('display', 'inline');
            $('.tomarFoto2').css('display', 'inline');
            $('#FotoMesaDigitalizacion1').val('');
            $('.ocultar').hide();
            $('#tabFotos a[href="#fotoDiv1"]').tab('show');
            $('#foto1').elevateZoom({
                zoomType: "inner",
                cursor: "crosshair"
            });

            BlockUI("Obteniendo Imagen...");
            $.getJSON($("#links").data().urlObtenerImagenCpe, { nroCtg: nroCTG }, function (data) {
                if (data.CodigoDeError == 1) {
                    MostrarAlertaInfo(data.Error);
                } else if (data.CodigoDeError == 0) {
                    $('#foto1').attr("src", 'data:image/jpeg;base64,' + data.PdfImageBase64);
                    $('#fotoDiv').css('display', 'inline');
                    $('.tomarFoto1').css('display', 'inline');
                    $('.tomarFoto2').css('display', 'inline');
                    $('#FotoMesaDigitalizacion1').val(data.PdfImageBase64);
                    $('.ocultar').hide();
                    $('#tabFotos a[href="#fotoDiv1"]').tab('show');
                    $('#foto1').elevateZoom({
                        zoomType: "inner",
                        cursor: "crosshair"
                    });
                } else {
                    MostrarAlertaError(data.Error);
                }
            }).complete(function () {
                $.unblockUI();
            });
        }
    } catch (e) {
        $.unblockUI();
    }
}

function configuracionCargarFerroviarioPorCtg() {
    $('#NroCartaPorte').prop('required', true);
    $('#NroCartaPorte').rules('add', 'required');
    $('#NroCartaPorte').attr("readonly", false);
    $('#NroCartaPorte').attr("disabled", false);
    $('#NumeroOperativo').prop('required', false);
    $('#NumeroOperativo').attr("readonly", true);
    $('#NroCartaPorte').focus();
}

function configuracionCargarFerroviarioPorOperativo() {
    $('#NroCartaPorte').prop('required', false);
    $('#NroCartaPorte').rules('remove', 'required');
    $('#NroCartaPorte').attr("readonly", true);
    $('#NroCartaPorte').attr("disabled", true);
    $('#NumeroOperativo').attr("readonly", false);
    $('#NumeroOperativo').prop('required', true);
    $('input[name="NumeroOperativo"]').rules('add', {
        messages: {
            required: "El Campo 'Nº de Operativo' es requerido"
        }
    });
    $('#NumeroOperativo').focus();
}

function consultarImagenCpe() {
    var nroCartaPorte = $('.cargarCartaPorte').val();
    if ($('#esIngreso').val() == "True" && (nroCartaPorte.length == 11 || nroCartaPorte.length == 12) && $.isNumeric(nroCartaPorte)) {
        obtenerFotoCartaPorteElectronica(nroCartaPorte);
    }
}