jQuery(document).ready(function ($) {

    //Devolver estilo italic a elementos previamente seleccionados por el autocompletar
    $(".ui-autocomplete-input").each(function () {
        if ($(this).val().length > 0)
            $(this).addClass("italic");
    });

    if ($('#Transportista').data() != null && $('#Transportista').data().errorRequerido != null) {
        $.validator.addMethod("transportistaRequerido", function (value, element) {
            return $('#TransportistaId').val() > 0;
        }, $('#Transportista').data().errorRequerido);
    }

    if ($('#mensajeInvalido').data() != null && $('#mensajeInvalido').data().invalido != null) {
        $.validator.addMethod("valorDebeSerValido", function (value, element) {
            return value.length == 0 || (value.length > 0 && $('#' + $(element).attr('id') + 'Id').val() != 0);
        }, $('#mensajeInvalido').data().invalido);
    }


    $.validator.addMethod("validaciondummy", function (value, element) {
        return true;
    }, "");
});

function cargarMascara() {
    var docChofer = $('#Chofer_NumeroDeDocumento');
    var extranjero = completarExtranjero(docChofer.val());
    if ($("#esExtranjero").is(":checked") && docChofer.val() != "") {
        $('#Chofer_Cuil').val("99-" + extranjero + "-9")
        $('#Chofer_Cuil').focus();
        $('#Chofer_Cuil').focusout();
        }
}
function DefinirAutocompletarGenerico(elemento, elementoId, linkListar, linkUnico, funcbordeColor, onSelect, onInvalid, data, minlength) {
    if (data) {
        linkListar = linkListar + "?" + $.param(data);
        linkUnico = linkUnico + "?" + $.param(data);
    }
    if (funcbordeColor != null) funcbordeColor(elemento, elementoId);
    $(elemento).autocomplete(
        {
            delay: 40,
            source: linkListar,
            autoFocus: true,
            max: 15,
            minLength: minlength != null ? minlength : 4,
            response: function (event, ui) {
                $.grep(ui.content, function (val, index) {
                    return val != null;
                });
            },
            open: function () {
                $(elemento).data('is_open', true);
            },
            close: function () {
                $(elemento).data('is_open', false);
            },
            select: function (event, ui) {
                $(elementoId).val(ui.item.Id);
                $(elemento).val(ui.item.label);
                if (funcbordeColor != null) funcbordeColor(elemento, elementoId);
                if (onSelect != null) onSelect();
                return false;
            }
        });

    InvalidarCampoAlPresionarTecla(elemento, elementoId, funcbordeColor, onInvalid);
    $(elemento).off('focusout');
    $(elemento).focusout(function () {
        if (!$(this).data('is_open') && linkUnico != null) {
            CargarValorUnico(elemento, elementoId, linkUnico, funcbordeColor, onSelect, onInvalid);
            if (funcbordeColor != null) funcbordeColor(elemento, elementoId);
        }
    });
}

function DefinirAutocompletar(elemento, elementoId, linkListar, linkUnico, onSelect, onInvalid, data, minlength) {
    DefinirAutocompletarGenerico(elemento, elementoId, linkListar, linkUnico, BordeColor, onSelect, onInvalid, data, minlength);
    $(elemento).addClass('valorDebeSerValido');
}


function DefinirAutocompletarChofer() {
    $("#Chofer_NumeroDeDocumento").focusout(function () {
        if (!$("#Chofer_NumeroDeDocumento").is('[readonly]')) {
            var elemento = $('#Chofer_Cuil');
            var docChofer = $('#Chofer_NumeroDeDocumento');
            var linkUnico = $('#links').data().urlBuscarChoferUnico;
            var primerDigitoExtranjero = "99-";
            var segundoDigitoExtranjero = "-9";
            var extranjero = completarExtranjero(docChofer.val());

            if (elemento.val() != (primerDigitoExtranjero + docChofer.val() + segundoDigitoExtranjero)) {
                $('#Chofer_Cuil').val("99-" + extranjero + "-9")
            }
        }
    });
    cargarExtranjeroInicial();
    cargarMascara();

    //Deshabilito el número de documento
    if ($('#esExtranjero').is(':not(:checked)')) {
        $('#Chofer_NumeroDeDocumento').attr("readonly", "readonly");
        $('#Chofer_NumeroDeDocumento').attr("tabindex", "-1");
    }
    //Seteo el mínimo de caracteres para el autocompletar de CUIL
    $('#Chofer_Cuil').keydown(function () {
        $("#Chofer_Cuil").autocomplete("option", "minLength", $(this).val().replace(/\D/g, "").length >= 9 ? 10 : 50);
    });

    //Inicializo el Chofer Id
    if ($("#Chofer_Id").val().length == 0) {
        $("#Chofer_Id").val(0);
    }

    if ($("#Chofer_Id").val().length != 0 && $("#Chofer_Id").val() != 0) {
        $('#Chofer_TipoDocumentoIdentidadId').attr("disabled", "disabled");
    }

    //Inicializo colores
    BordeColorChofer($("#Chofer_Cuil"), $('#Chofer_Id'));
    BordeColorChofer($("#Chofer_TipoDocumentoIdentidadId"), $('#Chofer_Id'));
    BordeColorChofer($("#Chofer_NumeroDeDocumento"), $('#Chofer_Id'));
    BordeColorChofer($("#Chofer_Nombre"), $('#Chofer_Id'));
    BordeColorChofer($("#Chofer_Apellido"), $('#Chofer_Id'));


    $("#Chofer_Cuil").mask("99-99999999-9");
    $("#Chofer_Cuil").focusout(function (event) {
        if ($("#Chofer_Id").val() == "0") {
            $("#Chofer_Cuil").mask("99-99999999-9");
            $("#Chofer_TipoDocumentoIdentidadId").css('border', 'solid 1px #CCCCCC');
            $("#Chofer_NumeroDeDocumento").css('border', 'solid 1px #CCCCCC');
            $("#Chofer_Nombre").css('border', 'solid 1px #CCCCCC');
            $("#Chofer_Apellido").css('border', 'solid 1px #CCCCCC');
            $("#Chofer_TipoDocumentoIdentidadId").removeClass("italic");
            $("#Chofer_NumeroDeDocumento").removeClass("italic");
            $("#Chofer_Nombre").removeClass("italic");
            $("#Chofer_Apellido").removeClass("italic");
        }
        //Completo el documento cuando se cambia el CUIL
        $("#Chofer_NumeroDeDocumento").val($(this).val().split('-')[1]);
    });
    //Preparo cuando cambio a amarillo que es modificación
    $("input[name^='Chofer']").keyup(function (e) {
        var code = e.keyCode || e.which;
        if (this.id == "Chofer_Cuil") {
            $("#Chofer_Id").val("0");
            $('#Chofer_TipoDocumentoIdentidadId').removeAttr("disabled");
        }
        if ($('#esExtranjero').is(':checked')) {
            $('#Chofer_TipoDocumentoIdentidadId').removeAttr("disabled");
            $("#Chofer_TipoDocumentoIdentidadId").removeAttr("readonly", "readonly");

        }
        if (!this.value || this.id == "Chofer_Cuil" || $("#Chofer_Id").val() == "0") {
            $(this).css('border', 'solid 1px #CCCCCC');
            $(this).removeClass("italic");
        }
        else if (!$(this).attr("readonly") && code != '9') {
            $(this).css('border', 'solid 1px #FFD800');
            $(this).css('border-right', 'solid 5px #FFD800');
            $(this).removeClass("italic");
        }
    });
    //Preparo cuando cambio a amarillo que es modificación
    $("#Chofer_TipoDocumentoIdentidadId").change(function () {
        if ($("#Chofer_Id").val() == "0") {
            $(this).css('border', 'solid 1px #CCCCCC');
            $(this).removeClass("italic");
        }
        else {
            $(this).css('border', 'solid 1px #FFD800');
            $(this).css('border-right', 'solid 5px #FFD800');
            $(this).removeClass("italic");
        }
    });

    //Habilitar el tipo de docuemento
    $("form").submit(function () {
        $("#Chofer_TipoDocumentoIdentidadId").prop("disabled", false);
    });

    var elemento = $('#Chofer_Cuil');
    var elementoDoc = $('#Chofer_NumeroDeDocumento');
    var elementoId = $('#Chofer_Id');
    var linkListar = $('#links').data().urlBuscarChoferes;
    var linkUnico = $('#links').data().urlBuscarChoferUnico;

    AutocompletarChofer(elemento, elementoId, linkListar, linkUnico);
    AutocompletarChofer(elementoDoc, elementoId, linkListar, linkUnico);

    $('#esExtranjero').change(cargarExtranjero);
}

function cargarExtranjero() {
    var elemento = $('#Chofer_Cuil');
    var elementoDoc = $('#Chofer_NumeroDeDocumento');
    if ($('#esExtranjero').is(':checked')) {
        elemento.css("display", "none");
        elemento.val("");
        $('#sinCuil').css("display", "block");
        elementoDoc.removeAttr("readonly", "readonly");
        elementoDoc.removeAttr("tabindex", "-1");
    } else {
        elemento.css("display", "block");
        $('#sinCuil').css("display", "none");
        elementoDoc.val("");
        elementoDoc.attr("readonly", "readonly");
        elementoDoc.attr("tabindex", "-1");
    };
};

function cargarExtranjeroInicial() {
    var elemento = $('#Chofer_Cuil');
    var elementoDoc = $('#Chofer_NumeroDeDocumento');
    if ($('#esExtranjero').is(':checked')) {
        elemento.css("display", "none");
        $('#sinCuil').css("display", "block");
        elementoDoc.removeAttr("readonly", "readonly");
    } else {
        elemento.css("display", "block");
        $('#sinCuil').css("display", "none");
        elementoDoc.attr("readonly", "readonly");
    };
};

function AutocompletarChofer(elemento, elementoId, linkListar, linkUnico) {
    BordeColorChofer(elemento, elementoId);
    $(elemento).autocomplete(
        {
            delay: 40,
            source: linkListar,
            autoFocus: true,
            max: 15,
            minLength: 4,
            response: function (event, ui) {
                $.grep(ui.content, function (val, index) {
                    return val != null;
                });
            },
            open: function () {
                $(elemento).data('is_open', true);
            },
            close: function () {
                $(elemento).data('is_open', false);
            },
            select: function (event, ui) {
                SetearChofer(ui.item);
                BordeColorChofer($("#Chofer_Cuil"), $('#Chofer_Id'));
                BordeColorChofer($("#Chofer_TipoDocumentoIdentidadId"), $('#Chofer_Id'));
                BordeColorChofer($("#Chofer_NumeroDeDocumento"), $('#Chofer_Id'));
                BordeColorChofer($("#Chofer_Nombre"), $('#Chofer_Id'));
                BordeColorChofer($("#Chofer_Apellido"), $('#Chofer_Id'));
                return false;
            }
        });
    InvalidarCampoAlPresionarTecla(elemento, elementoId, BordeColor);
    $(elemento).focusout(function () {
        if (linkUnico != null) {
            CargarChoferUnico(elemento, elementoId, linkUnico);
            BordeColorChofer(elemento, elementoId);
        }
    });

    $(elemento).addClass('valorDebeSerValido');
}

function SetearChofer(chofer) {
    if (chofer != null) {
        $('#Chofer_Id').val(chofer.Id);
        $('#Chofer_Cuil').val(chofer.Cuil);
        $('#Chofer_Nombre').val(chofer.Nombre);
        $('#Chofer_Apellido').val(chofer.Apellido);
        $('#Chofer_NumeroDeDocumento').val(chofer.NumeroDeDocumento);
        $('#Chofer_TipoDocumentoIdentidadId').val(chofer.TipoDocumentoIdentidadId);
        $('#Chofer_TipoDocumentoIdentidadId').attr("disabled", "disabled");
        ValidarObjeto($("form"), $("#Chofer_Cuil"));
        ValidarObjeto($("form"), $("#Chofer_TipoDocumentoIdentidadId"));
        ValidarObjeto($("form"), $("#Chofer_NumeroDeDocumento"));
        ValidarObjeto($("form"), $("#Chofer_Nombre"));
        ValidarObjeto($("form"), $("#Chofer_Apellido"));
    }
}

function ValidarObjeto(formulario, elemento) {
    formulario.validate().element(elemento);
    var controlGroup = elemento.closest("div.control-group");
    if (controlGroup.find('span.field-validation-error').length == 0) {
        controlGroup.removeClass('error');
        return 1;
    }
    else {
        controlGroup.addClass('error');
        return 0;
    }
}

function DefinirAutocompletarConSAP(elemento, elementoId, divAutocompletar, linkListar, linkUnico, obtenerProveedoresSap, onSelect, onInvalid, pr, cm, am) {
    BordeColor(elemento, elementoId);
    $(elemento).autocomplete(
        {
            delay: 40,
            source: function (request, response) {
                $.ajax({
                    url: linkListar,
                    dataType: "json",
                    data: {
                        term: request.term, PR: pr, CM: cm, AM: am
                    },
                    success: function (data) {
                        response(data);
                    }
                });
            },
            autoFocus: true,
            max: 15,
            minLength: 8,
            appendTo: divAutocompletar,
            response: function (event, ui) {
                ui.content.push({
                    label: 'Consultar en SAP...'
                });
            },
            open: function (event, ui) {
                $(elemento).data('is_open', true);
                $(divAutocompletar).find("a").last().click(function () {
                    BlockUI();
                    $.getJSON(obtenerProveedoresSap, { term: $(elemento).val(), PR: pr, CM: cm, AM: am }, function (data) {
                        jQuery.each(data, function (index) {
                            $(divAutocompletar).find("ul").append("<li class='ui-menu-item' role='presentation'><a class='ui-corner-all newPro sap-li-item' id='datosNuevos" + index + "' tabindex='-1'>" + data[index].label + "</a></li>");
                        });
                        $(divAutocompletar).find("ul").show();

                        $('.newPro').click(function () {
                            $(elementoId).val(findItem($('#' + this.id).text(), data)[0].Id);
                            $(elemento).val($('#' + this.id).text());
                            BordeColor(elemento, elementoId);
                            $(divAutocompletar).find("ul").hide();
                            if (onSelect != null) onSelect();
                        });
                        $(divAutocompletar).find("a:contains('Consultar en SAP...')").hide();
                    }).complete(function () {
                        $.unblockUI();
                    });
                });
            },
            close: function () {
                $(elemento).data('is_open', false);
            },
            select: function (event, data) {
                if (data.item != null && data.item.label != "Consultar en SAP...") {
                    $(elementoId).val(data.item.Id);
                    $(elemento).val(data.item.label);
                    BordeColor(elemento, elementoId);
                    if (onSelect != null) onSelect();
                }
                return false;

            }
        });
    InvalidarCampoAlPresionarTecla(elemento, elementoId, BordeColor, onInvalid);
    $(elemento).off('focusout');
    $(elemento).focusout(function () {
        if (!$(this).data('is_open') && linkUnico != null) {
            CargarValorUnico(elemento, elementoId, linkUnico, BordeColor, onSelect, onInvalid, pr, cm, am);
            BordeColor(elemento, elementoId);
        }
    });
    $(elemento).addClass('valorDebeSerValido');
}

function DefinirAutocompletarTransportista(elemento, elementoId, divAutocompletar, linkListarProveedor, linkUnicoProveedor, obtenerProveedoresSap, linkListarTransportista, linkUnicoTransportista, inicializar, tipoComercialId, tiposComercialesConTransportista, onSelectProveedor, onSelectTransportista, pr, cm, am) {
    var resultado = jQuery.inArray($(tipoComercialId).val(), tiposComercialesConTransportista);
    if (inicializar == true) {
        $(elementoId).val('0');
        $(elemento).val('');
        $(elemento).css('border', 'solid 1px #CCCCCC');
    }
    if (resultado == -1) {
        $(elemento).addClass('transportistaRequerido');
        if (onSelectProveedor != null) onSelectProveedor();
        DefinirAutocompletarConSAP(elemento, elementoId, divAutocompletar, linkListarProveedor, linkUnicoProveedor, obtenerProveedoresSap, onSelectProveedor, null, pr, cm, am);

    } else {
        $(elemento).removeClass('transportistaRequerido');
        if (onSelectTransportista != null) onSelectTransportista();
        DefinirAutocompletar(elemento, elementoId, linkListarTransportista, linkUnicoTransportista, onSelectTransportista);
    }
}

function DefinirAutocompletarTransportistaCartaPorte(elemento, elementoId, divAutocompletar, linkListarProveedor, linkUnicoProveedor, obtenerProveedoresSap, linkListarTransportista, linkUnicoTransportista, inicializar, tipoComercialId, tiposComercialesConTransportista, onSelectProveedor, onSelectTransportista, pr, cm, am) {
    $(elemento).removeClass('validaciondummy');
    var resultado = jQuery.inArray($(tipoComercialId).val(), tiposComercialesConTransportista);
    if (inicializar == true) {
        $(elementoId).val('0');
        $(elemento).val('');
        $(elemento).css('border', 'solid 1px #CCCCCC');
    }
    if (resultado == -1) {
        $(elemento).addClass('transportistaRequerido');
        DefinirAutocompletarConSAP(elemento, elementoId, divAutocompletar, linkListarProveedor, linkUnicoProveedor, obtenerProveedoresSap, onSelectProveedor, null, pr, cm, am);
        if ($(elementoId).val() == '0' || $(elementoId).val() == '') {
            $('#EsTransportista').val(false);
        }
    } else {
        $(elemento).removeClass('valorDebeSerValido');
        $(elemento).removeClass('transportistaRequerido');
        $(elemento).addClass('validaciondummy');
        ValidarObjeto($("form"), $(elemento));
        DefinirAutocompletarGenerico(elemento, elementoId, linkListarTransportista, linkUnicoTransportista, BordeColorTransportista, onSelectTransportista);
    }
}

function DefinirAutocompletarTransportistaTramo2CartaPorte(elemento, elementoId, divAutocompletar, linkListarProveedor, linkUnicoProveedor, obtenerProveedoresSap, linkListarTransportista, linkUnicoTransportista, inicializar, tipoComercialId, tiposComercialesConTransportista, onSelectProveedor, onSelectTransportista, pr, cm, am) {
    $(elemento).removeClass('validaciondummy');
    var resultado = jQuery.inArray($(tipoComercialId).val(), tiposComercialesConTransportista);
    if (inicializar == true) {
        $(elementoId).val('0');
        $(elemento).val('');
        $(elemento).css('border', 'solid 1px #CCCCCC');
    }
    if (resultado == -1) {
        DefinirAutocompletarConSAP(elemento, elementoId, divAutocompletar, linkListarProveedor, linkUnicoProveedor, obtenerProveedoresSap, onSelectProveedor, null, pr, cm, am);
        if ($(elementoId).val() == '0' || $(elementoId).val() == '') {
            $('#EsTransportistaTramo2').val(false);
        }
    } else {
        $(elemento).removeClass('valorDebeSerValido');
        $(elemento).addClass('validaciondummy');
        ValidarObjeto($("form"), $(elemento));
        DefinirAutocompletarGenerico(elemento, elementoId, linkListarTransportista, linkUnicoTransportista, BordeColorTransportista, onSelectTransportista);
    }
}

function BordeColor(elemento, elementoId) {
    if ($(elementoId).val() != 0) {
        $(elemento).css('border', 'solid 1px green');
        $(elemento).css('border-right', 'solid 5px green');
        $(elemento).addClass("italic");
    } else {
        $(elemento).css('border', 'solid 1px red');
        $(elemento).css('border-right', 'solid 5px red');
        $(elemento).removeClass("italic");
    }
    if ($(elemento).val() == "") {
        $(elemento).css('border', 'solid 1px #CCCCCC');
        $(elemento).removeClass("italic");
    }
    if ($(elemento).val() == "Todos") {
        $(elemento).css('border', 'solid 1px green');
        $(elemento).css('border-right', 'solid 5px green');
        $(elemento).addClass("italic");
    }
}

function BordeColorChofer(elemento, elementoId) {
    if ($(elementoId).val() != 0) {
        $(elemento).css('border', 'solid 1px green');
        $(elemento).css('border-right', 'solid 5px green');
        $(elemento).addClass("italic");
    }
    if ($(elemento).val() == "") {
        $(elemento).css('border', 'solid 1px #CCCCCC');
        $(elemento).removeClass("italic");
    }
}

function BordeColorTransportista(elemento, elementoId) {
    if ($(elementoId).val() != 0) {
        $(elemento).css('border', 'solid 1px green');
        $(elemento).css('border-right', 'solid 5px green');
        $(elemento).addClass("italic");
    } else {
        $(elemento).css('border', 'solid 1px #FFD800');
        $(elemento).css('border-right', 'solid 5px #FFD800');
        $(elemento).removeClass("italic");
        if (elemento === '#Transportista') {
            if ($('#EsTransportista').length != 0) { $('#EsTransportista').val(true); }
        }
        else if (elemento === '#TransportistaTramo2') {            
            if ($('#EsTransportistaTramo2').length != 0) { $('#EsTransportistaTramo2').val(true); }
        }
    }
    if ($(elemento).val() == "") {
        $(elemento).css('border', 'solid 1px #CCCCCC');
        $(elemento).removeClass("italic");
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

function InvalidarCampoAlPresionarTecla(elemento, elementoId, funcbordeColor, onInvalid) {
    $(elemento).keydown(function (e) {
        if (e.keyCode != 13 && e.keyCode != 9 && !e.shiftKey && $(elemento).attr('readonly') != 'readonly' && $(elemento).attr('disable') != 'disable') {
            $(elementoId).val("0");
            if (funcbordeColor != null) funcbordeColor(elemento, elementoId);
            if (onInvalid != null) onInvalid();
        }
    });
}

function CargarValorUnico(elemento, elementoId, linkUnico, funcbordeColor, onSelect, onInvalid, pr, cm, am) {
    if ($(elementoId).val() == "0" && $(elemento).val() != "" && $(elemento).val() != "Todos" && $(elemento).val().length >= 4) {
        $.getJSON(linkUnico, { term: $(elemento).val(), PR: pr, CM: cm, AM: am }, function (data) {
            if (data != "") {
                $(elementoId).val(data.Id);
                $(elemento).val(data.label);
                if (onSelect != null) onSelect();
            } else {
                $(elementoId).val("0");
                if (onInvalid != null) onInvalid();
            }
            ValidarObjeto($("form"), $(elemento));
            if (funcbordeColor != null) funcbordeColor(elemento, elementoId);
        });
    }
    else if ($(elementoId).val() != "0" && $(elemento).val() == "") {
        $(elementoId).val("0");
        if (funcbordeColor != null) funcbordeColor(elemento, elementoId);
        if (onInvalid != null) onInvalid();
    }
}

function CargarChoferUnico(elemento, elementoId, linkUnico) {
    var valor = elemento.val().replace("_", "").replace("-", "");
    if ($(elementoId).val() == "0" && valor != "" && $(elemento).val().length >= 4) {
        $.getJSON(linkUnico, { term: $(elemento).val() }, function (data) {
            if (data != "") {
                $(elementoId).val(data.Id);
                $(elemento).val(data.label);
                SetearChofer(data);
            } else {
                $(elementoId).val("0");
            }
            BordeColorChofer($("#Chofer_Cuil"), elementoId);
            BordeColorChofer($("#Chofer_Documento"), elementoId);
            BordeColorChofer($("#Chofer_TipoDocumentoIdentidadId"), elementoId);
            BordeColorChofer($("#Chofer_NumeroDeDocumento"), elementoId);
            BordeColorChofer($("#Chofer_Nombre"), elementoId);
            BordeColorChofer($("#Chofer_Apellido"), elementoId);
        });
    }
}

function DefinirAutocompletarParaPuerto(elemento, elementoId, linkListar, linkUnico, funcbordeColor, onSelect, onInvalid, data) {

    if (data) {
        linkListar = linkListar + "?" + $.param(data);
        linkUnico = linkUnico + "?" + $.param(data);
    }

    if (funcbordeColor != null) funcbordeColor(elemento, elementoId);
    $(elemento).autocomplete(
        {
            delay: 40,
            source: linkListar,
            autoFocus: true,
            max: 15,
            minLength: 1,
            response: function (event, ui) {
                $.grep(ui.content, function (val, index) {
                    return val != null;
                });
            },
            open: function () {
                $(elemento).data('is_open', true);
            },
            close: function () {
                $(elemento).data('is_open', false);
            },
            select: function (event, ui) {
                $(elementoId).val(ui.item.Id);
                $(elemento).val(ui.item.label);
                if (funcbordeColor != null) funcbordeColor(elemento, elementoId);
                if (onSelect != null) onSelect();
                return false;
            }
        });

    InvalidarCampoAlPresionarTecla(elemento, elementoId, funcbordeColor, onInvalid);
    $(elemento).off('focusout');
    $(elemento).focusout(function () {
        if ($(this).data('is_open') && linkUnico != null) {
            CargarValorUnico(elemento, elementoId, linkUnico, funcbordeColor, onSelect, onInvalid);
            if (funcbordeColor != null) funcbordeColor(elemento, elementoId);
        }
    });
}

function DefinirAutocompletarPuerto(elemento, elementoId, linkListar, linkUnico, onSelect, onInvalid, data) {
    DefinirAutocompletarParaPuerto(elemento, elementoId, linkListar, linkUnico, BordeColor, onSelect, onInvalid, data);
    $(elemento).addClass('valorDebeSerValido');
}

function completarExtranjero (i) {
  
    while (i.length < 8) {
        i = 0 + i;
    }
    return i;
};