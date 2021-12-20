
function DeshabilitarCampo(elemento) {
    elemento.attr('readonly', 'readonly');
    elemento.addClass("italic");
    elemento.unbind('focusout');
    elemento.unbind('keydown');  
    elemento.unbind('change');
    elemento.autocomplete({ disabled: true });
}

function HabilitarCampos() {
    $('input').attr("readonly", false);
    $('select').attr("readonly", false);
    $('select').removeAttr('disabled');
    $('input').removeAttr('disabled');
    $('#aceptar').attr("readonly", false);
    DeshabilitarCampo($('#Destinatario'));
    DeshabilitarCampo($('#Procedencia'));
    DeshabilitarCampo($('#TitularCartaPorte'));
    $('#CodEstab').attr('readonly', true);
    $('#bocaDestino').attr("readonly");
    $('#bocaDestino').attr('disabled', 'disabled');
    //$('#NroCartaPorte').addClass('validarCartaPorte');
};

function DesHabilitarCampos() {
    $('input').attr('readonly', true);
    $('select').attr('readonly', true);
    $('input').attr('disabled', 'disabled');
    $('select').attr('disabled', 'disabled');
    $('#aceptar').attr('readonly', true);
    $('#TipoComercialId').attr('readonly', false);
    $('#TipoComercialId').attr('disabled', false);
    //$('#NroCartaPorte').removeClass('validarCartaPorte');
}

function ReutilizarCartaPorte(cartaPorte) {
    LlenarCartaPorteRedespacho(cartaPorte);
    //TipoComercial
    $('#TipoComercial').val(cartaPorte.TipoComercialId);
    if (cartaPorte.EsExtranjero == true) {
        $("#esExtranjero").attr("checked", true);
        cargarExtranjero();
        cargarMascara();
    }
}

function LlenarCartaPorteRedespacho(cartaPorte) {
    ////fecha emision
    //if (cartaPorte.FechaEmision != null && cartaPorte.FechaEmision != "") {
    //    var fechaEmision = new Date(parseInt(cartaPorte.FechaEmision.substr(6)));
    //    var fechaEmisionFormateada = Globalize.format(fechaEmision, 'd');
    //    $('#FechaEmision').removeAttr('readonly');
    //    $('#FechaEmision').val(fechaEmisionFormateada);
    //}

    //FechaCP
    if (cartaPorte.FechaCP != null && cartaPorte.FechaCP != "") {
        var fechaCp = new Date(parseInt(cartaPorte.FechaCP.substr(6)));
        var fechaCpFormateada = Globalize.format(fechaCp, 'd');
        $('#FechaCP').removeAttr('readonly');
        $('#FechaCP').val(fechaCpFormateada);
    }

    //FechaVto
    if (cartaPorte.FechaVto != null && cartaPorte.FechaVto != "") {
        var fechaVto = new Date(parseInt(cartaPorte.FechaVto.substr(6)));
        var fechaVtoFormateada = Globalize.format(fechaVto, 'd');
        $('#FechaVto').val(fechaVtoFormateada);
    }

    //centro destino
    $('#DestinoId').val(cartaPorte.DestinoId);
    $('#Destino').val(cartaPorte.Destino);
    $('#Destino').addClass("italic");

    //CTG
    $('#CTG').val(cartaPorte.CTG);
    if (cartaPorte.Cupo != null && cartaPorte.Cupo.length > 0) {
        if (!$('#cupoModel').val()) {
            if (cartaPorte.Cupo.length <= 4) {
                $('#Cupo').val(cartaPorte.Cupo + ObtenerFechaActualCupo());
            } else {
                $('#Cupo').val(cartaPorte.Cupo);
            }
        } else {
            $('#Cupo').val($('#cupoModel').val());
        }
    } else {
        if (!$('#cupoModel').val()) {
            $('#checkSinCupo').prop('checked', true);
            $('#checkSinCupo').trigger('change');
        } else {
            $('#Cupo').val($('#cupoModel').val());
        }        
    }

    if (cartaPorte.NumeroAduana != null && cartaPorte.NumeroAduana.length > 0) {        
        $('#NumeroAduana').val(cartaPorte.NumeroAduana);
    }
    
    //CTG
    //$('#CEE').val(cartaPorte.CEE);

    //PatenteCamion
    $('#PatenteCamion').val(cartaPorte.PatenteCamion);
    $('#PatenteAcoplado').val(cartaPorte.PatenteAcoplado);

    //Desvio
    $('#Desvio').prop('checked', cartaPorte.Desvio);

    //Extranjero
    $('#esExtranjero').prop('checked', cartaPorte.EsExtranjero);


    //TitularCartaPorte
    $('#TitularCartaPorteId').val(cartaPorte.TitularCartaPorteId);
    $('#TitularCartaPorte').val(cartaPorte.TitularCartaPorte);
    $('#TitularCartaPorte').addClass("italic");

    //Intermediario
    $('#IntermediarioId').val(cartaPorte.IntermediarioId);
    $('#Intermediario').val(cartaPorte.Intermediario);
    $('#Intermediario').addClass("italic");

    //RtteComercial
    $('#RtteComercialId').val(cartaPorte.RtteComercialId);
    $('#RtteComercial').val(cartaPorte.RtteComercial);
    $('#RtteComercial').addClass("italic");

    //RtteComercialVentaSecundario
    $('#RtteComercialVentaSecundarioId').val(cartaPorte.RtteComercialVentaSecundarioId);
    $('#RtteComercialVentaSecundario').val(cartaPorte.RtteComercialVentaSecundario);
    $('#RtteComercialVentaSecundario').addClass("italic");

    //RtteComercialProductor
    $('#RtteComercialProductorid').val(cartaPorte.RtteComercialProductorId);
    $('#RtteComercialProductor').val(cartaPorte.RtteComercialProductor);
    $('#RtteComercialProductor').addClass("italic");

    //RtteComercialVentaSecundario2
    $('#RtteComercialVentaSecundario2Id').val(cartaPorte.RtteComercialVentaSecundario2Id);
    $('#RtteComercialVentaSecundario2').val(cartaPorte.RtteComercialVentaSecundario2);
    $('#RtteComercialVentaSecundario2').addClass("italic");

    //Corredor
    $('#CorredorId').val(cartaPorte.CorredorId);
    $('#Corredor').val(cartaPorte.Corredor);
    $('#Corredor').addClass("italic");

    //Corredor
    $('#CorredorSecundarioId').val(cartaPorte.CorredorId);
    $('#CorredorSecundario').val(cartaPorte.Corredor);
    $('#CorredorSecundario').addClass("italic");

    //CorredorVendedor
    $('#CorredorVendedorId').val(cartaPorte.CorredorVendedorId);
    $('#CorredorVendedor').val(cartaPorte.CorredorVendedor);
    $('#CorredorVendedor').addClass("italic");

    //CorredorVendedorSecundario
    $('#CorredorVendedorSecundarioId').val(cartaPorte.CorredorVendedorSecundarioId);
    $('#CorredorVendedorSecundario').val(cartaPorte.CorredorVendedorSecundario);
    $('#CorredorVendedorSecundario').addClass("italic");

    //IntermediarioFlete
    $('#IntermediarioFleteId').val(cartaPorte.IntermediarioFleteId);
    $('#IntermediarioFlete').val(cartaPorte.IntermediarioFlete);
    $('#IntermediarioFlete').addClass("italic");
    //Entregador
    $('#EntregadorId').val(cartaPorte.EntregadorId);
    $('#Entregador').val(cartaPorte.Entregador);
    $('#Entregador').addClass("italic");

    //AgenteCompras
    $('#AgenteComprasId').val(cartaPorte.AgenteComprasId);
    $('#AgenteCompras').val(cartaPorte.AgenteCompras);
    $('#AgenteCompras').addClass("italic");

    //Destinatario
    $('#DestinatarioId').val(cartaPorte.DestinatarioId);
    $('#Destinatario').val(cartaPorte.Destinatario);
    $('#Destinatario').addClass("italic");

    //Transportista
    $('#TransportistaId').val(cartaPorte.TransportistaId);
    $('#Transportista').val(cartaPorte.Transportista);
    $('#Transportista').addClass("italic");
    $('#EsTransportista').val(true);
    
    //Variedad
    $('#Variedad').val(cartaPorte.Variedad);

    //Cosecha
    if (cartaPorte.Cosecha != null && cartaPorte.Cosecha != "") {
        $('#Cosecha').val(cartaPorte.Cosecha.replace("-", "").slice(0, 2) + '-' + cartaPorte.Cosecha.replace("-", "").slice(2, 4));
    }    

    //Procedencia
    $('#ProcedenciaId').val(cartaPorte.ProcedenciaId);
    $('#Procedencia').val(cartaPorte.Procedencia);
    $('#Procedencia').addClass("italic");

    //CodEstab
    $('#CodEstab').val(cartaPorte.CodEstab);

    //chofer
    SetearChofer(cartaPorte.Chofer);

    //material
    $('#MaterialId').val(cartaPorte.MaterialId);

    if (cartaPorte.TipoCategoriaId != null && cartaPorte.TipoCategoriaId != 0) {
        $('#TipoCategoriaId').val(cartaPorte.TipoCategoriaId);
    }
    

    $('#tipoVehiculoDropdown').val(cartaPorte.TipoVehiculo);
    if (cartaPorte.TipoVehiculo != 1) {
        $('#tabFotos li[class="fotoLi2"]').hide();
    }
    $("#VehiculoJson").val(cartaPorte.VehiculoJson);

    //Prestador
    $('#PrestadorId').val(cartaPorte.PrestadorId);
    $('#Prestador').val(cartaPorte.Prestador);
    $('#Prestador').addClass("italic");

    if ($('#PrestadorId').val() != '0') {
        RecargarBoca(cartaPorte);
    }
    
    //patente
    $('.boca').val(cartaPorte.BocaDestinoId);

    //AcuerdoMarco
    $('#AcuerdoMarco').val(cartaPorte.AcuerdoMarco);

    //Caratula
    $('#Caratula').val(cartaPorte.Caratula);

    //habilito boton aceptar
    $('#aceptar').removeAttr('disabled');

    //Sucursal
    if (cartaPorte.Sucursal >= 0 && $('#Cpe').is(':checked')) {        
        var cadena = "000000000" + cartaPorte.Sucursal;
        var format = cadena.substr(cadena.length - 5);
        $('#Sucursal').val(format);
    }

    //NumeroOrden
    if ($('#Cpe').is(':checked') && cartaPorte.NroOrden != null && cartaPorte.NroOrden != 0) {
        var cadena = "000000000" + cartaPorte.NroOrden;
        var format = cadena.substr(cadena.length - 8);
        $('#CTG').val(format);
    }

    MostrarAnexo(cartaPorte.CodigoAnexo);
    MostrarTecnologia(cartaPorte.TecnologiaId);

    var patente = cartaPorte.Vehiculos[0]["Patente"];
    var acoplado = cartaPorte.Vehiculos[0]["PatenteAcoplado"];
    var acoplado2 = cartaPorte.Vehiculos[0]["PatenteAcoplado2"];
    if (cartaPorte.Vehiculos.length > 0 && patente != '' && acoplado != '' && acoplado2 != '') {
        flag = true;
        ActualizarTipoVehiculo(patente, acoplado, acoplado2, function () { BlockUI(" consulta de tipo de vehiculo por patente"); }, function () { $.unblockUI(); });
    }

    $('#observacion').val(cartaPorte.Observacion);

    if (cartaPorte.CodigoRamalId != null && cartaPorte.CodigoRamalId != 0) {
        $('#CodigoRamalId').val(cartaPorte.CodigoRamalId);
    }

    //AcuerdoMarco
    $('#NumeroPrecinto').val(cartaPorte.NumeroPrecinto);

    //Imagen CPE
    if ($('#esIngreso').val() == "True" && $('#cartaPorteId').val() == 0) {
        //obtenerFotoCartaPorteElectronica(cartaPorte.NroCartaPorte);
    }

    //Numero Operativo
    if (cartaPorte.TipoVehiculoInt === 1) {
        $('#NumeroOperativo').val(cartaPorte.NumeroOperativo);
    }

    //Transportista Tramo2
    if (cartaPorte.TransportistaTramo2Id != 0) {
        $('#TransportistaTramo2Id').val(cartaPorte.TransportistaTramo2Id);
        $('#TransportistaTramo2').val(cartaPorte.TransportistaTramo2);
        $('#TransportistaTramo2').addClass("italic");
        $('#EsTransportistaTramo2').val(true);
    }
}

function LimpiarCartaPorte() {
    //centro destino
    $('#DestinoId').val('0');
    $('#Destino').val('');
    $('#CTG').val('');
    //$('#CEE').val('');
    $('#FechaCP').val('');
    $('#FechaVto').val('');
    $('#TipoComercial').val('');
    $('#PatenteCamion').val('');
    $('#PatenteAcoplado').val('');
    $('#Desvio').prop('checked', false);
    $('#esExtranjero').prop('checked', false);
    $('#TitularCartaPorteId').val('');
    $('#TitularCartaPorte').val('');
    $('#IntermediarioId').val('0');
    $('#Intermediario').val('');
    $('#RtteComercialId').val('0');
    $('#RtteComercial').val('');
    $('#CorredorId').val('0');
    $('#Corredor').val('');
    $('#EntregadorId').val('0');
    $('#Entregador').val('');
    $('#AgenteComprasId').val('0');
    $('#AgenteCompras').val('');
    $('#DestinatarioId').val('0');
    $('#Destinatario').val('');
    $('#TransportistaId').val('0');
    $('#Transportista').val('');
    $('#Variedad').val('');
    $('#Cosecha').val('');
    $('#ProcedenciaId').val('0');
    $('#Procedencia').val('');
    $('#CodEstab').val('');
    $('#Chofer_Id').val('0');
    $('#Chofer_Cuil').val('');
    $('#Chofer_Nombre').val('');
    $('#Chofer_Apellido').val('');
    $('#Chofer_NumeroDeDocumento').val('');
    $('#Chofer_TipoDocumentoIdentidadId').val('');
    $('.materialId').val('0');
    $('#PrestadorId').val('0');
    $('#Prestador').val('');
    $('#BocaDestinoId').val('0');
    $('#BocaDestino').val('');
    $('.boca').val('');
    $('#AcuerdoMarco').val('');
    $('#Caratula').val('');
    $('#Cupo').val('');
    $('#checkSinCupo').prop('checked', false);
    $('#tipoVehiculoDropdown').get(0).selectedIndex = 0;
    $('#Sucursal').val('');
    MostrarAnexo();
    MostrarTecnologia();
}

function MostrarAnexo(codigoAnexo) {
    if ($('#divanexo').data() != null) {
        var resultado = jQuery.inArray($('#MaterialId').val(), $('#divanexo').data().materialesAnexos);

        if (resultado == -1) {
            $('#RequiereAnexoInase').val(false);
            $('#divanexo').addClass('hide');
            $('#CodigoAnexo').val('');
            $('#CodigoAnexo').removeClass('anexoRequerido');
        } else {
            $('#RequiereAnexoInase').val(true);
            $('#divanexo').removeClass('hide');
            $('#CodigoAnexo').addClass('anexoRequerido');
            if (codigoAnexo != null) $('#CodigoAnexo').val(codigoAnexo);
        }
    }
}

function MostrarTecnologia(tecnologiaId) {
    if ($('#divtecnologia').data() != null) {
        var resultado = jQuery.inArray($('#MaterialId').val(), $('#divtecnologia').data().materialesTecnologias);

        if (resultado == -1) {
            $('#RequiereTecnologia').val(false);
            $('#divtecnologia').addClass('hide');
            $('#TecnologiaId').val('');
        } else {
            $('#RequiereTecnologia').val(true);
            $('#divtecnologia').removeClass('hide');
            if (tecnologiaId != null) $('#TecnologiaId').val(tecnologiaId);
        }
    }
}

function RecargarBoca(cartaPorte) {
    $.getJSON($('#links').data().urlBuscarBocas, { proveedorId: $("#PrestadorId").val() },
        function (response) {
            var options = '';
            for (var i = 0; i < response.length; i++) {
                options += "<option value='" + response[i].Value + "'>"
                        + response[i].Text + "</option>";
            }
            $('#bocaDestino').html(options);
            $('#bocaDestino').attr("disabled", false);

            $('#BocaDestino').val(cartaPorte.BocaDestino);
            $('#BocaDestinoId').val(cartaPorte.BocaDestinoId);
            $('#bocaDestino').val(cartaPorte.BocaDestinoId);
        });
}

function ObtenerFechaActualCupo() {
    var today = new Date();
    var dd = today.getDate().toString();
    var mm = today.getMonth() + 1;
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }

    return today = dd + mm + yyyy;
}