$(document).ready(function () {
    $(".patente-internacional").mask("?*******", {placeholder: ""});
    var loading = $('#gridContainer');
    var height = $(window).height();
    var width = $(document).width();
    $.blockUI.defaults.css = {
        left: width / 2 - (loading.width() / 2),
        top: height / 3 - (loading.height() / 3),
        backgroundColor: 'white',
        border: '1px solid #B94A41',
        color: '#0055A5',
        padding: 10

    };

    $("#gridContainer").block({
        overlayCSS: { backgroundColor: 'white' },
        message: $('#Cargando').val(),
        onBlock: function () {
            $(".blockPage").addClass("alert alert-info");
        }
    });
    CargarGrilla(function () { $("#gridContainer").unblock(); });
    
    
    var intervalo = Autorefresco(null);
    $('#modoDeRefresco').change(function () {
        CargarGrilla();
        intervalo = Autorefresco(intervalo);
    });
    

    $('#DropDownColumnas').bind('hide', function () {
        CargarGrilla();
    });

    $('.columnasdropdown input, .columnasdropdown label').click(function (e) {
        e.stopPropagation();
    });
    
    $('#NumeroDocumentoDeIngreso').attr("disabled", "disabled");
    $('#TipoDocumentoDeIngreso').change(function () {
        if ($(this).val() != "") {
            $('#NumeroDocumentoDeIngreso').removeAttr("disabled");
        } else {
            $('#NumeroDocumentoDeIngreso').attr("disabled", "disabled");
        }
    });
    
    DefinirAutocompletar('#MaterialDesc', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    
    
    countChecked();

    $(document).on('click', ".columna-checkbox", countChecked);
    
    $(document).on('click', '.ajax-editar-asignacion-link', function () {
        $.get(this.href, cargarDialogoEditarAsignacion);
        return false;
    });
    //-------------------------------------------------------------
    $(document).on('click', '.rechazar-boton', function () {
        var n = $(".columna-checkbox:checked");
        var instanceIds = "";
        //Armo el Actionlink para Rechazar
        $.each(n, function (index, value) {
            if (instanceIds == "") {
                instanceIds += $(value).attr('id');
            }
            else {
                instanceIds += "," + $(value).attr('id');
            }
        });
        
        $.get(this.href, { instancesId: instanceIds }, cargarDialogoRechazar); return false;

    });


    $(document).on('click', '.dialogo-rechazar-cerrar', function () {
        $("#dialogo-rechazar").modal('hide');
        $("#mensajeRechazar").html("");
        return false;
    });
    //-----------------------------------------------------------------
    cargarTiposVehiculo(true)
});

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
   }

    function countChecked() {
        var n = $(".columna-checkbox:checked");
        var nMaterialesId = [];
        var nSonSustentables = [];
        var instanceIds = "InstanceIds=";
        var instanceIdsHidden = "";
        //Armo el Actionlink para Asignar
        $.each(n, function (index, value) {
            nMaterialesId.push(value.getAttribute('data-materialid'));
            nSonSustentables.push(value.getAttribute('data-esSustentable'));
            if (instanceIds == "InstanceIds=") {
                instanceIds += $(value).attr('id');
                instanceIdsHidden = $(value).attr('id');
            }
            else {
                instanceIds += "," + $(value).attr('id');
                instanceIdsHidden += "," + $(value).attr('id');
            }
        });
        $("#AsignarSeleccionados").attr("href", $("#AsignarSeleccionados").data().url + "?" + instanceIds);
        $("#InstanceIds").val(instanceIdsHidden);
        //Habilita el boton ASIGNNAR si hay seleccionados y con diferentes materiales

        if ($("#separarAlmacenSustentable").val() == "true") {

            if (n.length > 0 && jQuery.unique(nMaterialesId).length == 1 && jQuery.unique(nSonSustentables).length == 1) {
                $("#AsignarSeleccionados").attr("disabled", false);
            } else {
                $("#AsignarSeleccionados").attr("disabled", true);
            }
            //Habilita el boton RECHAZAR si hay seleccionados
            if (n.length > 0) {
                $("#RechazarSeleccionados").removeClass("disabled");
            } else {
                $("#RechazarSeleccionados").addClass("disabled");
            }
            if (jQuery.unique(nMaterialesId).length <= 1 && jQuery.unique(nSonSustentables).length <= 1) {
                $("#AsignarSeleccionadosValid").html("");
                $("#AsignarSeleccionadosValid").addClass('field-validation-valid');
                $("#AsignarSeleccionadosValid").removeClass('field-validation-error');
            } else {
                $("#AsignarSeleccionadosValid").html((jQuery.unique(nMaterialesId).length > 1) ? $("#gridContainer").data().errorMaterial : $("#gridContainer").data().errorSustentable);
                $("#AsignarSeleccionadosValid").addClass('field-validation-error');
                $("#AsignarSeleccionadosValid").removeClass('field-validation-valid');
            }
        } else {

            if (n.length > 0 && jQuery.unique(nMaterialesId).length == 1) {
                $("#AsignarSeleccionados").attr("disabled", false);
            } else {
                $("#AsignarSeleccionados").attr("disabled", true);
            }
            //Habilita el boton RECHAZAR si hay seleccionados
            if (n.length > 0) {
                $("#RechazarSeleccionados").removeClass("disabled");
            } else {
                $("#RechazarSeleccionados").addClass("disabled");
            }
            if (jQuery.unique(nMaterialesId).length <= 1) {
                $("#AsignarSeleccionadosValid").html("");
                $("#AsignarSeleccionadosValid").addClass('field-validation-valid');
                $("#AsignarSeleccionadosValid").removeClass('field-validation-error');
            } else {
                $("#AsignarSeleccionadosValid").html((jQuery.unique(nMaterialesId).length > 1) ? $("#gridContainer").data().errorMaterial : $("#gridContainer").data().errorSustentable);
                $("#AsignarSeleccionadosValid").addClass('field-validation-error');
                $("#AsignarSeleccionadosValid").removeClass('field-validation-valid');
            }
        }
    };

    function Autorefresco(intervalo) {
        if ($("#modoDeRefresco").is(':checked')) {
            return setInterval(function () {
                CargarGrilla();
            }, 30000);
        } else {
            if (intervalo != null) clearInterval(intervalo);
            return null;
        }
    }

    function CargarGrilla(callback) {
        var container = $('#gridContainer');
        //Obtengo url de la grilla
        var url = container.data().gridUrl;
        //Verifico si el atributo refresco no está seteado
        url = UpdateQueryString("refresco", $("#modoDeRefresco").is(':checked'), url);

        var checkTipoVehiculo = $("#checkTipoVehiculo").is(':checked') ? "TipoVehiculo|" : "";
        var checkPatente = $("#checkPatente").is(':checked') ? "Patente|" : "";
        var checkProximaEtapa = $("#checkProximaEtapa").is(':checked') ? "ProximaEtapa|" : "";
        var checkWorkflow = $("#checkWorkflow").is(':checked') ? "Workflow|" : "";
        var checkMaterialId = $("#checkMaterialId").is(':checked') ? "MaterialId|" : "";
        var checkMaterial = $("#checkMaterial").is(':checked') ? "MaterialDescripcion|" : "";
        var checkTransportista = $("#checkTransportista").is(':checked') ? "Transportista|" : "";
        var checkCuit = $("#checkCuit").is(':checked') ? "Cuit|" : "";
        var checkTipoDocumentoIngreso = $("#checkTipoDocumentoIngreso").is(':checked') ? "TipoDocumentoIngreso|" : "";
        var checkNumeroDocumentoIngreso = $("#checkNumeroDocumentoIngreso").is(':checked') ? "NumeroDocumentoIngreso|" : "";
        var checkCalidad = $("#checkCalidad").is(':checked') ? "Calidad|" : "";
        var checkCentroId = $("#checkCentroId").is(':checked') ? "CentroId|" : "";
        var checkCentro = $("#checkCentro").is(':checked') ? "Centro|" : "";
        var checkfechaCreacion = $("#checkFechaCreacion").is(':checked') ? "FechaCreacion|" : "";
        var checkFechaUltimaModificacion = $("#checkFechaUltimaModificacion").is(':checked') ? "FechaUltimaModificacion|" : "";
        var checkTipoComercial = $("#checkTipoComercial").is(':checked') ? "TipoComercial.Descripcion|" : "";
        var checkHumedad = $("#checkHumedad").is(':checked') ? "Humedad|" : "";
        var checkCalle = $("#checkCalle").is(':checked') ? "Calle|" : "";

        var columnas = checkCalidad + checkCentroId + checkCuit + checkMaterial + checkTipoDocumentoIngreso + checkNumeroDocumentoIngreso
            + checkPatente + checkProximaEtapa + checkTransportista + checkWorkflow + checkfechaCreacion + checkFechaUltimaModificacion + checkTipoComercial + checkCentro + checkMaterialId + checkTipoVehiculo + checkHumedad + checkCalle;
        if (columnas[columnas.length - 1] == "|")
            columnas = columnas.slice(0, -1);

        //Agrego filtros
        url = UpdateQueryString("Columnas", columnas, url);
        url = UpdateQueryString("TipoDocumentoDeIngreso", $("#filtroTipoDocumentoDeIngreso").val(), url);
        url = UpdateQueryString("NumeroDocumentoDeIngreso", $("#filtroNumeroDocumentoDeIngreso").val(), url);
        url = UpdateQueryString("NumeroDeTarjeta", $("#filtroNumeroDeTarjeta").val(), url);
        url = UpdateQueryString("Patente", $("#filtroPatente").val(), url);

        url = UpdateQueryString("Workflow", $("#filtroWorkflow").val(), url);
        url = UpdateQueryString("ProximaAccion", $("#filtroProximaAccion").val(), url);
        url = UpdateQueryString("TipoComercialId", $("#filtroTipoComercialId").val(), url);
        url = UpdateQueryString("MaterialId", $("#filtroMaterialId").val(), url);

        url = UpdateQueryString("Calidad", $("#filtroCalidad").val(), url);
        url = UpdateQueryString("TipoDeSoja", $("#filtroTipoDeSoja").val(), url);
        url = UpdateQueryString("TipoEstado", $("#filtroTipoEstado").val(), url);
        url = UpdateQueryString("CantidadDeResultados", $("#filtroCantidadDeResultados").val(), url);
        url = UpdateQueryString("TipoDeProteina", $("#filtroEsProteina").val(), url);

        url = UpdateQueryString("SoloNoAsignados", $("#filtroSoloNoAsignados").val(), url);
        url = UpdateQueryString("SoloSinDescuentos", $("#filtroSoloSinDescuentos").val(), url);
        url = UpdateQueryString("TipoVehiculo", $("#filtroTipoVehiculo").val(), url);

        url = UpdateQueryString("TipoMaterial", $("#filtroTipoMaterial").val(), url);
        url = UpdateQueryString("CalleId", $("#filtroCalleId").val(), url);

        $.get(url, function (data) {
            container.html(data);
            var seleccionados = $("#InstanceIds").val().split(',');
            $.each(seleccionados, function (index, value) {
                $("#" + value).attr('checked', 'checked');
            });
            if ($("#grid tbody tr").length == $(".columna-checkbox:checkbox:checked").length) {
                $("#grid thead tr th:first").html("<input type='checkbox' id='SeleccionarTodos' checked/>");
            } else {
                $("#grid thead tr th:first").html("<input type='checkbox' id='SeleccionarTodos'/>");
            }
            $(".columna-checkbox").change(function () {
                if ($(this).is(':checked')) {
                    if ($("#grid tbody tr").length == $(".columna-checkbox:checkbox:checked").length) {
                        $("#SeleccionarTodos").prop("checked", true);
                    }

                } else {
                    $("#SeleccionarTodos").prop("checked", false);
                }
            });

            $("#SeleccionarTodos").change(function () {
                if ($(this).is(':checked')) {
                    $(".columna-checkbox").prop("checked", true);
                    $(".columna-checkbox").trigger("change");
                } else {
                    $(".columna-checkbox").prop("checked", false);
                    $(".columna-checkbox").trigger("change");
                }
                countChecked();
            });

            if (callback != null) {
                callback();
            }
        });
    }

    function CopiarFiltros() {
        $("#filtroTipoDocumentoDeIngreso").val($("#TipoDocumentoDeIngreso").val());
        $("#filtroNumeroDocumentoDeIngreso").val($("#NumeroDocumentoDeIngreso").val());
        $("#filtroNumeroDeTarjeta").val($("#NumeroDeTarjeta").val());
        $("#filtroPatente").val($("#Patente").val());

        $("#filtroWorkflow").val($("#Workflow").val());
        $("#filtroProximaAccion").val($("#ProximaAccion").val());
        $("#filtroTipoComercialId").val($("#TipoComercialId").val());
        $("#filtroMaterialId").val($("#MaterialId").val());

        $("#filtroCalidad").val($("#Calidad").val());
        $("#filtroTipoDeSoja").val($("#TipoDeSoja").val());
        $("#filtroTipoEstado").val($("#TipoEstado").val());
        $("#filtroCantidadDeResultados").val($("#CantidadDeResultados").val());

        $("#filtroEsProteina").val($("#TipoDeProteina").val());
        $("#filtroSoloNoAsignados").val($("#SoloNoAsignados").is(':checked'));
        $("#filtroSoloSinDescuentos").val($("#SoloSinDescuentos").is(':checked'));
        $("#filtroTipoVehiculo").val($("#TipoVehiculo").val());
        $("#filtroTipoMaterial").val($("#TipoMaterial").val());
        $("#filtroCalleId").val($("#CalleId").val());

    }

    function UpdateQueryString(key, value, url) {
        if (!url) url = window.location.href;
        var re = new RegExp("([?|&])" + key + "=.*?(&|#|$)(.*)", "gi");

        if (re.test(url)) {
            if (typeof value !== 'undefined' && value !== null)
                return url.replace(re, '$1' + key + "=" + value + '$2$3');
            else {
                var hash = url.split('#');
                url = hash[0].replace(re, '$1$3').replace(/(&|\?)$/, '');
                if (typeof hash[1] !== 'undefined' && hash[1] !== null)
                    url += '#' + hash[1];
                return url;
            }
        }
        else {
            if (typeof value !== 'undefined' && value !== null) {
                var separator = url.indexOf('?') !== -1 ? '&' : '?', hash2 = url.split('#');
                url = hash2[0] + separator + key + '=' + value;
                if (typeof hash2[1] !== 'undefined' && hash2[1] !== null)
                    url += '#' + hash2[1];
                return url;
            }
            else
                return url;
        }
    }

    function cargarDialogoEditarAsignacion(data) {
        $('#dialogo-editar-body').html(data);
        $("#dialogo-editar-guardar").attr("disabled", false);
        $('#dialogo-editar-title').html($('#dialogo-editar-body form').data().dialogoTitulo);
        $('#dialogo-editar-body form').attr('data-ajax-success', 'editarRepuestaFormularioPuestoComando');
        if ($('#dialogo-editar-body form').data().dialogoExtraclass) {
            $('#dialogo-editar').addClass($('#dialogo-editar-body form').data().dialogoExtraclass);
        }

        $('#dialogo-editar').modal({
            backdrop: 'static', keyboard: false
        }).css({
            'top': '30%',
            'margin-left': function () {
                return -($(this).width() / 2);
            },
            'left': '50%',
            'margin-top': function () {
                return -($(this).height() / 3.4);
            }
        });
        attachDataPickers();
    }

function editarRepuestaFormularioPuestoComando(respuesta) {
    $('#dialogo-rechazar').modal('hide');
        if (respuesta != window.ajaxEditSuccess) {
            cargarDialogoEditar(respuesta);
        } else {
            $('#dialogo-editar').modal('hide');
            MostrarAlertaExitosa();
        }
}
function editarRepuestaFormularioRechazado(respuesta) {
    $('#dialogo-rechazar').modal('hide');
    if (respuesta != window.ajaxEditSuccess) {
        cargarDialogoRechazar(respuesta);
    } else {
        MostrarAlertaExitosa();
    }
}
function cargarTiposVehiculo(bool) {
    $.getJSON($('#links').data().urlObtenertiposvehiculo, { conTren: bool },
        function (response) {
            response.splice(0, 0, { tipoVehiculoText: "Camiones", tipoVehiculoValue: "-1" })
            var options = '';
            options = "<option data-netoMaximo='" + "null" + "'  data-brutoMaximo='" + "null" + "'  data-netoMinimo='" + "null" + "' value='" + "null" + "'" + ">"
                + "(tipo vehiculo)" + "</option>";

            for (var i = 0; i < response.length; i++) {
                options += "<option data-netoMaximo='" + response[i].netoMaximo + "'  data-brutoMaximo='" + response[i].brutoMaximoEgreso + "'  data-netoMinimo='" + response[i].netoMinimo + "' value='" + response[i].tipoVehiculoValue + "'" + ">"
                    + response[i].tipoVehiculoText + "</option>";
            }
            $('#TipoVehiculo').html(options);
            $('#TipoVehiculo').val(null);
            $('#filtroTipoVehiculo').val(null);
            //console.log(options);
            //if ($("#tipoVehiculoDropdown").val() != 1) {
            //    $('#tabFotos li[class="fotoLi2"]').hide();
            //}
        });
}

    