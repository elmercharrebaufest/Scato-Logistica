var ordenesSAP;
jQuery(document).ready(function () {
    $("#controlComisionista").hide();
    $("#controlRemitente").hide();
    $("#controlCuitDestinatario").hide();

    $(".patente-internacional").mask("?*******", { placeholder: "" });
    if (!$("#EsModificacion")) {
        //Inicializo chofer
        $('#Chofer_Id').val("0");
        $(".btn-primary").attr("disabled", true);
        //Inicializo campos deshabilitados
        $("input[readonly='readonly']").attr("disabled", true);
        $("input").removeAttr("readonly");
        $("select[readonly='readonly']").attr("disabled", true);
        $("select").removeAttr("readonly");
    } else {
        $("input[name^='Chofer']").removeAttr("disabled");
        $("input[name^='Chofer']").addClass("italic");
        $("select[name^='Chofer']").removeAttr("disabled");
        $("#TipoComercialId").removeAttr("disabled");
    }

    $(".close").click(function () {
        $("#alertaError").hide();
    });
    if ($("#PatenteCamion").val().length > 0 && $("#EsModificacion").val()!= "True") {
        ObtenerDatosSap();
    }
    $('#PatenteCamion').on("focusout", function () {
        if ($('#PatenteCamion').val().replace("_", "").length >= 6) {
            $('.datosap').val("");
            $('#NumeroOrden').empty();
            $('#NumeroOrden').attr("disabled", true);
            ObtenerDatosSap();
        }
    });

    $('.cargaFas').change(function () {
        $(".cargaFas option:selected").each(function () {
            var val = $(this).val();
            var orden = $.grep(ordenesSAP, function (p) {
                return p.NumeroOrden === val;
            });
            if (orden.length > 0) {
                LlenarDatos(orden[0]);
                $(".btn-primary").attr("disabled", false);
                $("#TipoComercialId").attr("disabled", false);
                completarKmRecorrerYLocalidad();
            }
        });
    });

    if ($('#PagadorFlete').length > 0) {
        DefinirAutocompletarConSAP(
            '#PagadorFlete',
            '#PagadorFleteId',
            '#autocompletePagadorFlete',
            $('#links').data().urlBuscarClientes,
            $('#links').data().urlBuscarClienteUnico,
            $('#links').data().urlObtenerClientesSap,
            function () {
            },
            function () {
            }
        );
    }

    if ($('#Corredor').length > 0) {
        DefinirAutocompletarConSAP(
            '#Corredor',
            '#CorredorId',
            '#autocompleteCorredor',
            $('#links').data().urlBuscarProveedores,
            $('#links').data().urlBuscarProveedorUnico,
            $('#links').data().urlObtenerProveedoresSap,
            function () {
            },
            function () {
            }
        );
    }

    //Remuevo estilo italic si modifico lo seleccionado
    $('.autocompletado-obligatorio').keydown(function (e) {
        if (e.keyCode != 13 && e.keyCode != 9)
            $(this).removeClass("italic");
    });

    //Devolver estilo italic a elementos previamente seleccionados por el autocompletar
    $(".ui-autocomplete-input").each(function () {
        if ($(this).val().length > 0)
            $(this).addClass("italic");
    });
    DefinirAutocompletarChofer();
    completarKmRecorrerYLocalidad();
    $('#localidadDestinoDropdown').change(function () {
        $('#KmARecorrer').val($('#localidadDestinoDropdown :selected').data('kilometros'));
    });

    $('#PatenteCamion').change(ValidarPatenteCnrt);
    $('#PatenteAcoplado').change(ValidarPatenteCnrt);
    if ($('#PatenteCamion').val() != "") {
        $('#PatenteCamion').trigger("change")
    }

    $("#pendiente").click(DemorarCamion);
    $("#PatenteCamion").focus(function () { $("#valPatente").hide() });
    $("#dialogo-demorar-confirmar").click(AceptarDemora);
    $("#dialogo-demorar").on("hidden.bs.modal", function () {
        $("#Material").val("");
        $("#MotivoDemora").val("");
    });
    $("#VehiculoDemorado").val(false);

    if ($('#ClienteId').length > 0) {
        CargarPlantas();
        CargarDomicilios();
    }

    $("#btnRechazarOrdenCargaFas").click(function () {
        let valido = true;

        if ($("#MotivoRechazo").val() == '') {
            valido = false;
            $("#error-rechazo-requerido").show();
            $("#error-rechazo-largo").hide();
        } else if ($("#MotivoRechazo").val().length < 10) {
            valido = false;
            $("#error-rechazo-largo").show();
            $("#error-rechazo-requerido").hide();
        }

        if ($("#VehiculoDemorado").length > 0) {
            $("#VehiculoDemorado").val("False");
        }

        if ($("#Rechazado").length > 0) {
            $("#Rechazado").val("True");
        }

        if (valido) {
            modalRechazarOrdenCargaFas.close();
            $("#ordenCargaFas-form").submit();
        }
    })

    $("#OrdenDomicilioDestino").change(function () {
        let tipoDestino = $("#OrdenDomicilioDestino :selected").data().tipo;
        $("#TipoDomicilioDestino").val(tipoDestino);
    })
});

function deshabilitarKmRecorrerYLocalidad() {

    if ($('#ClienteId').length == 0 || $('#ClienteId').val() == null || $('#ClienteId').val() == '0' || $('#localidadDestinoDropdown option').length == 0) {
        $('#KmARecorrer').val("");
        $('#LocalidadDestinoId').val(0);
        $('#localidadDestinoDropdown').html(null);
        $('#KmARecorrer').attr("disabled", true);
        $('#localidadDestinoDropdown').attr("disabled", true);
    }
}

function completarKmRecorrerYLocalidad() {
    var clienteId;
    if ($('#Destino').length > 0) {
        clienteId = $('#DestinoId').val();
    }
    if ($('#ClienteDesc').length > 0) {
        clienteId = $('#ClienteId').val();
    }
    if ($('#RemitenteId').val() > 0) {
        clienteId = $('#RemitenteId').val();
    } else if ($('#ComisionistaId').val() > 0) {
        clienteId = $('#ComisionistaId').val();
    }
    if (clienteId > 0) {
        $.getJSON($('#links').data().urlBuscarKmporproveedor, { clienteId: clienteId },
            function (response) {
                var options = '';
                for (var i = 0; i < response.length; i++) {
                    options += "<option data-kilometros='" + response[i].kmRecorrer + "' value='" + response[i].localidadDestinoId + "'" + ">"
                        + response[i].localidadDescripcion + "</option>";
                }
                if (response.length > 0) {
                    var optdefault = '';
                    optdefault = "<option value=''> (Localidad) </option>";
                    optdefault += options;
                    $('#localidadDestinoDropdown').html(optdefault);
                    let localidadSeleccionada = $("#LocalidadSeleccionada").val();
                    let localidadesIds = response.map(localidad => localidad.localidadDestinoId);
                    if (localidadSeleccionada.length > 0 && localidadesIds.includes(parseInt(localidadSeleccionada))) {
                        $('#localidadDestinoDropdown').val(parseInt(localidadSeleccionada))
                    }
                    $('#localidadDestinoDropdown').removeAttr("disabled");
                    $('#KmARecorrer').removeAttr("disabled");

                    if ($('#LocalidadDestinoId').val() > 0) {
                        $('#localidadDestinoDropdown').val($('#LocalidadDestinoId').val());
                        $('#KmARecorrer').val($('#localidadDestinoDropdown :selected').data('kilometros'));
                    }
                } else {
                    deshabilitarKmRecorrerYLocalidad();
                    MostrarAlertaAdvertencia("El cliente no tiene km a recorrer asociados");
                }

            });
    } else {
        deshabilitarKmRecorrerYLocalidad();
    }

}

function findItem(term, data) {
    var items = [];
    for (var i = 0; i < data.length; i++) {
        var item = data[i];
        for (var prop in item) {
            var detail = item[prop].toString();
            if (detail.indexOf(term) > -1) {
                items.push(item);
                break;
            }
        }
    }
    return items;
}


function ObtenerDatosSap() {
    if ($('#PatenteCamion').val().length == 0) {
        $('#PatenteCamion').removeAttr('disabled');
    } else {
        LimpiarChofer();
        BlockUI($("#MensajeBuscandoDatos").val());
        $.getJSON($("#PatenteCamion").data().numeroUrl, { numero: $('#PatenteCamion').val(), workflow: $("#WorkflowDescripcion").val() }, function (data) {
            if (data.datosSap == -1) {
                $('.btn.btn-primary').attr('disabled', 'disabled');
                MostrarAlertaError(data.error);
            }
            else {
                LlenarCombo(data.datosSap);
                $('.btn').removeAttr('disabled');
            }
        }).complete(function () {
            $.unblockUI();
        });
    }
}

function LlenarCombo(datos) {
    ordenesSAP = datos;
    var myOptions = [];
    $.each(datos, function (index) {
        myOptions.push(datos[index].NumeroOrden);
    });
    var mySelect = $('#NumeroOrden');
    var textVal = ''
    mySelect.empty();
    mySelect.append("<option value=''>" + mySelect.data().firstOption + "</option>");
    $.each(myOptions, function (index, text) {
        if (myOptions.length == 1) {
            textVal = text;
        }
        mySelect.append("<option value=" + text + ">" + text + "</option>");
    });
    $('#NumeroOrden').attr("disabled", false);
    $('#TipoComercialId').attr("disabled", false);
    if (textVal != '') {
        $('#NumeroOrden').val(textVal);
        $('#NumeroOrden').change();
    }
}

function LlenarDatos(datos) {
    if (datos) {
        $('#PatenteAcoplado').val(datos.PatenteAcoplado);
        $('#PatenteAcoplado').change();
        $('#ClienteId').val(datos.ClienteId);
        $('#ClienteDesc').val(datos.ClienteDesc);
        $('#ClienteDesc').addClass("italic");

        $('#TransportistaId').val(datos.TransportistaId);
        $('#TransportistaDesc').val(datos.TransportistaDesc);
        $('#CuitTransporte').val(datos.CuitTransporte);
        $('#TransportistaDesc').addClass("italic");

        $('#MaterialDesc').val(datos.MaterialDesc);
        $('#MaterialId').val(datos.MaterialId);
        $('#MaterialDesc').addClass("italic");

        $('#NumeroOrden').val(datos.NumeroOrden);
        $('#NumeroOrdenId').val(datos.NumeroOrdenId);
        $('#ValidaCompliance').val(datos.ValidaCompliance);
        $('#TipoComercialId').val(datos.TipoComercialId);
        $('#TipoComercialDEsc').val(datos.TipoComercialDesc);
        if ($('#LocalidadSeleccionada').val() == '0') {
            $('#KmARecorrer').val("")
        }
        $('#PlantaSeleccionada').val(datos.PlantaDGDestino);
        $('#DomicilioSeleccionado').val(datos.OrdenDomicilioDestino);
        $('#PagadorFlete').val(datos.PagadorFlete);
        $('#PagadorFleteId').val(datos.PagadorFleteId);
        $('#Inhabilitado').val(datos.Inhabilitado);
        $('#Corredor').val(datos.Corredor);
        $('#CorredorId').val(datos.CorredorId);
        $('#Comisionista').val(datos.Comisionista);
        $('#ComisionistaId').val(datos.ComisionistaId);
        $('#Remitente').val(datos.Remitente);
        $('#RemitenteId').val(datos.RemitenteId);
        $('#CuitDestinatario').val(datos.CuitDestinatario);
        ValidarDerivadoGranario();
        CargarPlantas();
        CargarDomicilios();

        if (datos.Chofer) {
            $('#Chofer_Id').val(datos.Chofer.Id);
            $('#Chofer_Nombre').val(datos.Chofer.Nombre);
            $('#Chofer_Apellido').val(datos.Chofer.Apellido);
            $('#Chofer_TipoDocumentoIdentidadId').val(datos.Chofer.TipoDocumentoIdentidadId);
            $('#Chofer_NumeroDeDocumento').val(datos.Chofer.NumeroDeDocumento);
            $('#Chofer_Cuil').val(datos.Chofer.Cuil);
        } else {
            $("input[name^='Chofer']").removeAttr("disabled");
            $("select[name^='Chofer']").removeAttr("disabled");
        }

        if ($("#ComisionistaId").val() != '') {
            $("#controlCliente").hide();
            $("#controlComisionista").show();
            $("#controlCuitDestinatario").show();
        } else if ($("#RemitenteId").val() != '') {
            $("#controlCliente").hide();
            $("#controlRemitente").show();
            $("#controlCuitDestinatario").show();
        } else {
            $("#controlCliente").show();
        }
    }
}


function MostrarAlertaError(data) {
    if (data != null) {
        $("#alertaError span").text(data);
    } else {
        $("#alertaError span").text($("#alertaError").data().mensaje);
    }
    $("#alertaError").show();
    $("#alertaError").delay(500).addClass("in");
}

function ValidarPatenteCnrt() {
    var patente = $("#PatenteCamion").val();
    var acoplado = $("#PatenteAcoplado").val();
    if (patente != '' && acoplado != '') {
        ActualizarTipoVehiculo(patente, acoplado, function () { BlockUI(" consulta de tipo de vehiculo por patente"); }, function () { $.unblockUI(); });
    }
}
function ActualizarTipoVehiculo(patente, acoplado, before, callback) {
    if (before != null) before();
    $.getJSON($("#links").data().urlObtenerTipovehiculoPorPatente, { patente: patente, acoplado: acoplado, workflow: $('#WorkflowDescripcion').val() }, function (data) {
        if (data.CodigoDeError == 0) {
            if (data.Categoria != null) {
                if ($('#TipoVehiculo option[value=' + data.Categoria + ']').length == 0) {
                    MostrarAlertaError("La categoría del vehículo " + data.CategoriaDesc + " no esta configurada para el centro actual");
                } else {
                    $('#TipoVehiculo').val(data.Categoria);
                }
            } else {
                MostrarAlertaError("El servicio CNRT no devolvió información sobre la categoría del vehículo, debe ingresarla manualmente.");
            }
        } else if (data.CodigoDeError == 1) {
            $('.btn').removeAttr('disabled');
        } else {
            MostrarAlertaAdvertencia(data.Error);
        }
    }).complete(function () {
        if (callback != null) callback();
    });
}
function LimpiarChofer() {
    $('#Chofer_Id').val(0);
    $('#Chofer_Nombre').val("");
    $('#Chofer_Apellido').val("");
    $('#Chofer_TipoDocumentoIdentidadId').val();
    $('#Chofer_NumeroDeDocumento').val("");
    $('#Chofer_Cuil').val("");
    $('#esExtranjero').prop("checked", false);
}

function DemorarCamion() {
    if ($("#PatenteCamion").val() == "") $("#valPatente").show();
    if ($("#PatenteCamion").val() != "" ) {
        $("#dialogo-demorar").modal("show");
    }
}

function AceptarDemora() {
    var valido = true;
    $("#error-material").hide();
    if ($("#Material").val() == "") {
        valido = false;
        $("#error-material").show();
    }
    $("#error-demorado-requerido").hide();
    $("#error--demorado-largo").hide();
    var motivo = $("#MotivoDemora").val();
    if (motivo == "") {
        $("#error-demorado-requerido").dis;
        valido = false;
    }
    if (motivo.length < 10) {
        $("#error--demorado-largo").show();
        valido = false;
    }

    if (valido == false) return false;
    $("#VehiculoDemorado").val(true);
    if ($("#ClienteId").val() == "" || $("#ClienteId").val() == "0") $("#ClienteId").val(99999)
    if ($("#TransportistaId").val() == "" || $("#TransportistaId").val() == "0") $("#TransportistaId").val(99999)
    if ($("#MaterialDesc").val() == "") $("#MaterialDesc").val($("#Material").find(":selected").text())
    if ($("#MaterialId").val() == "" || $("#MaterialId").val() == "0") $("#MaterialId").val($("#Material").val())
    if ($("#NumeroOrden").val() == "") $("#NumeroOrden").val(0)
    if ($("#Chofer_Cuil").val() == "") $("#Chofer_Cuil").val("99-99999999-9")
    if ($("#Chofer_NumeroDeDocumento").val() == "") $("#Chofer_NumeroDeDocumento").val("99999999")
    if ($("#Chofer_Nombre").val() == "") $("#Chofer_Nombre").val("a")
    if ($("#Chofer_Apellido").val() == "") $("#Chofer_Apellido").val("a")

    $("form").submit();       

}

function CargarPlantas() {
    let plantaSeleccionada = $("#PlantaSeleccionada").val();
    let cliente = $("#ClienteId").val();
    let cuit = ''
    if ($('#RemitenteId').val() > 0 || $('#ComisionistaId').val() > 0) {
        cuit = $('#CuitDestinatario').val();
    }
    if ($("#DerivadoGranarioHabilitado").val().toLowerCase() === 'true' && cliente.length > 0) {
        $.getJSON($('#links').data().urlObtenerPlantasPorCliente, { clienteId: cliente, clienteCuit: cuit },
            function (allData) {
                let options = '<option value="">(nro. planta)</option>';
                $('#PlantaDGDestino').html(options);
                if (!allData.HayErrores) {
                    for (let i = 0; i < allData.Plantas.length; i++) {
                        options += `<option value="${allData.Plantas[i]}">Planta Nro. ${allData.Plantas[i]}</option>`;
                    }
                    $('#PlantaDGDestino').html(options);
                    if (plantaSeleccionada.length > 0 && allData.Plantas.includes(parseInt(plantaSeleccionada))) {
                        $('#PlantaDGDestino').val(parseInt(plantaSeleccionada))
                    }
                }
            }
        );
    }
}

function CargarDomicilios() {
    let ordenDomicilioSeleccionado = $("#DomicilioSeleccionado").val();
    let tipoDomicilioSeleccionado = $("#TipoDomicilioDestino").val();
    let cliente = $("#ClienteId").val();
    let cuit = ''
    if ($('#RemitenteId').val() > 0 || $('#ComisionistaId').val() > 0) {
        cuit = $('#CuitDestinatario').val();
    }
    if ($("#DerivadoGranarioHabilitado").val().toLowerCase() === 'true' && cliente.length > 0) {
        $.getJSON($('#links').data().urlObtenerDomiciliosDerivadoGranarioPorCliente, { clienteId: cliente, clienteCuit: cuit },
            function (allData) {
                let options = '<option data-tipo="" value="">(domicilio)</option>';
                $('#OrdenDomicilioDestino').html(options);
                if (!allData.HayErrores) {
                    for (let i = 0; i < allData.Domicilios.length; i++) {
                        options += `<option data-tipo="${allData.Domicilios[i].Tipo}" value="${allData.Domicilios[i].Orden}">(${allData.Domicilios[i].Tipo} - ${allData.Domicilios[i].Orden}) ${allData.Domicilios[i].Descripcion}</option>`;
                    }
                    $('#OrdenDomicilioDestino').html(options);
                    if (ordenDomicilioSeleccionado.length > 0
                        && tipoDomicilioSeleccionado.length > 0
                        && allData.Domicilios.some(domicilio => domicilio.Orden == ordenDomicilioSeleccionado && domicilio.Tipo == tipoDomicilioSeleccionado)) {
                        $('#OrdenDomicilioDestino').val(parseInt(ordenDomicilioSeleccionado))
                    }
                }
            }
        );
    }
}

function ValidarDerivadoGranario() {
    let materialId = $("#MaterialId").val();
    let materialesDerivadoGranario = JSON.parse($("#ListaMaterialesDerivadoGranario").val())
    if (materialesDerivadoGranario.includes(parseInt(materialId))) {
        $('#DerivadoGranarioHabilitado').val('true')
        $('.derivadoGranario').removeClass('hidden');
    } else {
        $('#DerivadoGranarioHabilitado').val('false')
        $('.derivadoGranario').addClass('hidden');
        $('#PlantaDGDestino').val('');
        $('#OrdenDomicilioDestino').val('');
        $('#PagadorFlete').val('');
        $('#PagadorFleteId').val('');
    }
}