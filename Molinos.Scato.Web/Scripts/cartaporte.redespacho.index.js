jQuery(document).ready(function ($) {
    activarCPE();
    //Eventos
    $('.cargarCartaPorte').off("change");
    $('.cargarOperativo').off("change");
    $("#dialogo-confirmar-confirmar").off("click");
    
    var cartaDePorte;
    $('.cargarCartaPorte').change(function () {
        var nroCartaPorte = $('.cargarCartaPorte').val();
        if (nroCartaPorte.length == 11 || nroCartaPorte.length == 12 && $.isNumeric(nroCartaPorte)) {
            BlockUI($("#MensajeBuscandoCartaPorte").val());
            $.getJSON($("#links").data().urlObtenerCartaPorteRedespacho, { numero: nroCartaPorte, workflow: $('#workflow').val(), esIngreso: $('#esIngreso').val(), tipoVehiculo: !$("#tipoVehiculoDropdown").val() ? "0" : $("#tipoVehiculoDropdown").val(), cpe: $('#Cpe').is(':checked'), consultactg: $('#DescargaCartaPortePorCtg').is(':checked') }, function (data) {
                if (data.CodigoDeError == 0) {
                    cartaDePorte = data.CartaPorte;
                    // Muestro dialogo: Reutilizar: si, no
                    if (data.EsRedespacho == true) {
                        cargarCartaDePorteRedespacho(cartaDePorte);
                        if (data.EstaDemorado && $('#controlarTiempoPorCTG').val() == "True") {
                            MostrarAlertaAdvertencia(data.Error);
                        }
                    } else {
                        $("#dialogo-confirmar").modal('show');
                    }
                } else if (data.CodigoDeError == 1) {
                    //MostrarAlertaAdvertencia(data.Error);
                    $('.btn').removeAttr('disabled');
                } else {
                    MostrarAlertaError(data.Error);
                }
            }).complete(function () {
                $.unblockUI();
            });
        }
    });

    $("#dialogo-confirmar-confirmar").on('click', function () {
        cargarCartaDePorte(cartaDePorte);
        ValidarObjeto($("#orden-form"), $("#CEE"));
        return false;
    });
    
    function cargarCartaDePorteRedespacho(cartaDePorteRedespacho) {
        LimpiarCartaPorte();
        LlenarCartaPorteRedespacho(cartaDePorteRedespacho);
        $('.btn').removeAttr('disabled');
        //MostrarAlertaExitosa();
        $("#dialogo-confirmar").modal('hide');
        setearVistaTipoVehiculo(cartaDePorteRedespacho);
    }

    if ($('#Cpe').is(':checked')) {
        configuracionAltaCPE();
        configuracionCPEFerroviariaAlta();
        if ($("#esEgreso").val() == 'True') {
            $('#NroCartaPorte').attr("readonly", true);
            $('#Sucursal').attr("readonly", true);
            $('#NroCartaPorte').attr("disabled", true);
        }
    }

    $('#Cpe').change(function () {
        clearValidation();
        var tipoVehiculo = $('#tipoVehiculoDropdown :selected').text();
        var tipoVehiculoModel = $('#TipoVehiculoModel').val() === "1";
        configuracionCPEFerroviariaAlta();

        if ($('#esIngreso').val() == "True") {            
            if ((tipoVehiculo == "Tren" || tipoVehiculoModel) && $('#Cpe').is(':checked')) {
                $("#Cpe").val(true);
            } else {
                $('#CTG').addClass('ctgRequerido');
                callEventsCpe();
            }
            $('#NroCartaPorte').focus();
        } else {
            if ($('#Cpe').is(':checked')) {
                configuracionAltaCPE();
                if (tipoVehiculo == "Tren" || tipoVehiculoModel) {
                    $('.numero-operativo').show();
                } else {
                    $('.numero-operativo').hide();
                }
                if ($("#esEgreso").val() == 'True') {
                    $('#NroCartaPorte').attr("readonly", true);
                    $('#Sucursal').attr("readonly", true);
                    $('#NroCartaPorte').attr("disabled", true);
                }                
            } else {
                configuracionAltaFisica();
                $('.numero-operativo').hide();
            }
        }        
    });

    function configuracionAltaCPE() {
        $('#NroCartaPorte').prop('required', false);
        $('#NroCartaPorte').rules('remove', 'required');
        $('#Sucursal').prop('required', false);
        $('#Sucursal').rules('remove', 'required');
        $('#CTG').rules('remove', 'required');
        $('#CTG').prop('required', false);
        $('#CTG').removeClass('ctgRequerido');
    }

    function configuracionAltaFisica() {
        $('#NroCartaPorte').prop('required', true);
        $('#NroCartaPorte').rules('add', 'required');
        $('#Sucursal').prop('required', false);
        $('#Sucursal').rules('remove', 'required');
        $('#CTG').prop('required', true);
        $('#CTG').rules('add', 'required');
        $('#CTG').addClass('ctgRequerido');
    }

    $('.numero-operativo').hide();

    $('#tipoVehiculoDropdown').change(
        function () {
            configuracionCPEFerroviariaAlta();
        }
    );

    function configuracionCPEFerroviariaAlta() {
        var tipoVehiculo = $('#tipoVehiculoDropdown :selected').text();
        var tipoVehiculoModel = $('#TipoVehiculoModel').val() === "1";

        if (tipoVehiculo == "Tren" || tipoVehiculoModel) {            
            if ($('#Cpe').is(':checked')) {
                $('.numero-operativo').show();
                $("label[for*='NroCartaPorte']").text("Nº de CTG");
                $('#NroCartaPorte').attr('readonly', true);
                $('#NumeroOperativo').prop('required', true);
                $('input[name="NumeroOperativo"]').rules('add', {
                    messages: {
                        required: "El Campo 'Nº de Operativo' es requerido"
                    }
                });
                $('.row-sucursal-ctg').hide();
                if ($("#esEgreso").val() == 'True') {
                    $('#NroCartaPorte').attr("readonly", true);
                    $('#Sucursal').attr("readonly", true);
                    $('#NroCartaPorte').attr("disabled", true);
                }

                if ($('#DescargaCartaPortePorCtg').is(':checked')) {
                    configuracionCargarFerroviarioPorCtg();
                } else {
                    configuracionCargarFerroviarioPorOperativo();
                }

            } else {
                $("label[for*='NroCartaPorte']").text("Nº de Carta de Porte");
                $('#NroCartaPorte').attr('readonly', false);
                $('#NumeroOperativo').prop('required', false);
                $('.row-sucursal-ctg').show();
            }
        } else {
            $('.numero-operativo').hide();
            $('#NroCartaPorte').attr('readonly', false);
            $('#NumeroOperativo').prop('required', false);
            $('.row-sucursal-ctg').show();

            if ($('#Cpe').is(':checked')) {
                $('#CTG').removeClass('ctgRequerido');
                $('#CTG').rules('remove', 'required');
                $('#CTG').prop('required', false);
                $('#Sucursal').prop('required', false);
                $('#Sucursal').rules('remove', 'required');
                if ($("#esEgreso").val() == 'True') {
                    $('#NroCartaPorte').attr("readonly", true);
                    $('#Sucursal').attr("readonly", true);
                    $('#NroCartaPorte').attr("disabled", true);
                }
            } else {
                $('#CTG').addClass('ctgRequerido');
                $('#CTG').rules('add', 'required');
                $('#CTG').prop('required', true);
                $('#Sucursal').prop('required', false);
                $('#Sucursal').rules('remove', 'required');
            }
        }

        $('#NroCartaPorte').attr("maxlength", "12");
        $.validator.messages.maxlength = "El campo 'Nº de CTG' debe tener una longitud máxima de 12";
    }

    configuracionCPEFerroviariaAlta();

    $('.cargarOperativo').change(function () {
        var tipoVehiculo = $('#tipoVehiculoDropdown :selected').text();
        var tipoVehiculoModel = $('#TipoVehiculoModel').val() === "1";
        var NumOperativo = $('#NumeroOperativo').val();

        if ((tipoVehiculo == "Tren" || tipoVehiculoModel) && NumOperativo.length > 0 && $('#esIngreso').val() == "True") {
            if ($('#Cpe').is(':checked')) {
                BlockUI($("#MensajeBuscandoCartaPorte").val());
                $.getJSON($("#links").data().urlObtenerCartaPorteRedespacho, { numero: NumOperativo, workflow: $('#workflow').val(), esIngreso: $('#esIngreso').val(), tipoVehiculo: !$("#tipoVehiculoDropdown").val() ? "0" : $("#tipoVehiculoDropdown").val(), cpe: $('#Cpe').is(':checked'), consultactg: $('#DescargaCartaPortePorCtg').is(':checked') }, function (data) {
                    if (data.CodigoDeError == 0) {
                        cartaDePorte = data.CartaPorte;
                        // Muestro dialogo: Reutilizar: si, no
                        if (data.EsRedespacho == true) {
                            cargarCartaDePorteRedespacho(cartaDePorte);
                            if (data.EstaDemorado && $('#controlarTiempoPorCTG').val() == "True") {
                                MostrarAlertaAdvertencia(data.Error);
                            }
                        } else {
                            $("#dialogo-confirmar").modal('show');
                        }
                    } else if (data.CodigoDeError == 1) {
                        MostrarAlertaAdvertencia(data.Error);
                        $('.btn').removeAttr('disabled');
                    } else {
                        MostrarAlertaError(data.Error);
                    }
                }).complete(function () {
                    $.unblockUI();
                });
            }
        }
    });

    $('#DescargaCartaPortePorCtg').change(
        function () {
            configuracionCPEFerroviariaAlta();
        }
    );
    
    if ($("#esEgreso").val() == 'True') {
        $('#Cupo').attr("maxlength", "30");
    }
});

function activarCPE() {
    $('#Cpe').prop('checked', true);
    $('#Cpe').val(true)
    configuracionCPE();
    $('.check-cpe').hide()
}