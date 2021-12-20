$(document).ready(function () {
    //Inicializo Knockout
    var viewModel = new MuestraListViewModel();
    ko.applyBindings(viewModel);
    $("#CamaraId").focus();
    //Habilito-deshabilito pantalla
    if ($("#CamaraId").val() > 0)
        HabilitarPantalla(); 
    else
        InhabilitarPantalla();
    //Manejo el cambio de cámara
    $("#CamaraId").change(function() {
        if ($("#CamaraId").val() > 0)
            HabilitarPantalla();
        else
            InhabilitarPantalla();
        if ($(".muestraRow").length > 0) {
            //Mostrar confirmación de cambio de cámara
            $('#dialogo-confirmar').modal({}); 
        }
    });
    //Guardo la cámara previamente seleccionada
    $("#CamaraId").focus(function () {
        $("#CamaraIdAnterior").val($(this).val());
    });
    //Valido si hay muestras cargadas
    $.validator.addMethod("validarHayMuestras", function () {
        return $(".muestraRow").length > 0;
    }, $("#ErrorNoHayMuestras").val());
    /* confirmar Link */
    $('#dialogo-confirmar').on('shown', function () {
        $('#dialogo-confirmar-cancelar').focus();
    });
    $('#dialogo-confirmar-cancelar').click(function () {
        $("#CamaraId").val($("#CamaraIdAnterior").val());
        $('#dialogo-confirmar').modal('hide');
    });
    $('#dialogo-confirmar-confirmar').click(function () {
        viewModel.muestrasLista([]);
        $('#dialogo-confirmar').modal('hide');
    });
    //Capturo tecla enter para ingresar muestra
    $('.ingresoMuestra').bind('keypress', function (event) {
        if (event.keyCode === 13) {
            $("#agregarMuestra").click();
            return false;
        }
    });
    
    $('#dialogo-ver').bind('keypress', function (event) {
        if (event.keyCode === 13) {
            $('#dialogo-ver-ok').click();
            return false;
        }
    });
    $('#dialogo-ver-ok').click(function () {
        $("#numeroDeMuestra").focus();
    });

    $("#numeroDeMuestra").focus(function () { $(this).select(); });
});

function InhabilitarPantalla() {
    $(".habilitado").attr("disabled", true);
    $(".habilitado:text").val("");
}
function HabilitarPantalla() {
    $(".habilitado").attr("disabled", false);
}

function ValidarSiHayMuestras() {
    if (!$("#numeroDeMuestra").valid()) {
        $("#agregarMuestraDiv")[0].className = "control-group error";
        $("#validarMuestras").show();
    }

}

function MuestraListViewModel() {
    // Data
    var self = this;
    self.muestrasLista = ko.observableArray([]);
    //Controlo si habían muestras cargadas anteriormente
    if ($("#Muestras").val().length > 0) {
        var muestras = ko.utils.parseJson($("#Muestras").val());
        self.muestrasLista(muestras);
    }
    // Operations
    self.addMuestra = function () {
        var existe = false;
        var muestraListaActuales = self.muestrasLista();
        $.each(muestraListaActuales, function (key, value) {
            if (this.NroMuestra == $("#numeroDeMuestra").val() && value._destroy != true)
                existe = true;
        });
        if (existe)
            $.get(this.href, cargarDialogoVer($('#errorExistente').val()));
        else {
            BlockUI("");
            $.getJSON($("#agregarMuestra").data().muestraUrl, { numeroDeMuestra: $("#numeroDeMuestra").val(), camaraId: $("#CamaraId").val() }, function (data) {
                if (data.MuestraId == -2) {
                    cargarDialogoVer($('#errorInexistente').val());
                }
                else if (data.MuestraId == -1) {
                    cargarDialogoVer($('#errorCamionEnPlanta').val());
                }
                else if (data.MuestraId == -3) {
                    cargarDialogoVer($('#errorCamionRechazado').val());
                }
                else if (data.MuestraId == -4) {
                    cargarDialogoVer($('#errorCamara').val());
                }
                else if (data.MuestraId == 0) {
                    cargarDialogoVer($('#errorInvalida').val());
                }
                else {
                    self.muestrasLista.push(new Muestra(data.MuestraId, data.Material, data.Vendedor, data.Corredor, data.FechaDescarga, data.NetoPlanta, data.Localidad, data.Patente, data.NumeroMuestra, $("#numeroMuestraTerceros").val()));
                    ValidarSiHayMuestras();
                    $("#numeroDeMuestra").val("");
                    $("#numeroMuestraTerceros").val("");
                    $("#numeroDeMuestra").focus();
                }
            }).complete(function (){$.unblockUI();});
        }
    };
    self.removeMuestra = function (muestra) {
        self.muestrasLista.remove(muestra);
    };
}

function Muestra(muestraId, material, vendedor, corredor, fechaDescarga, netoPlanta, localidad, patente, numeroMuestra, numeroMuestraTerceros) {

    var jsonDate = fechaDescarga;  // returns "/Date(1245398693390)/"; 
    var re = /-?\d+/;
    var m = re.exec(jsonDate);
    var descarga = m != undefined ? new Date(parseInt(m[0])) : "";
    
    this.Id = muestraId;
    this.Material = material;
    this.Vendedor = vendedor;
    this.Corredor = corredor;
    this.FechaDescargaFormateada = fechaDescarga != null ? Globalize.format(descarga, 'd') + ", " + Globalize.format(descarga, 't') : "";
    this.FechaDescarga = descarga;
    this.PesoNeto = netoPlanta;
    this.Localidad = localidad;
    this.Patente = patente;
    this.NroMuestra = numeroMuestra;
    this.NroMuestraTerceros = numeroMuestraTerceros;
}

function EvaluarResultado(data) {
    $.unblockUI();
    if (data.responseJSON.resultado == "ERROR")
        MostrarAlertaAdvertencia(data.responseJSON.mensaje);
}

function AceptarFormulario() {
    ValidarSiHayMuestras();
    if ($("#lote-form").valid() && $("#soloImprimir").val().length == 0)
        BlockUI($("#lote-form").data().mensajeEspera);
}

function Imprimir(valor) {
    if (valor)
        $('#soloImprimir').val('true');
    else
        $('#soloImprimir').val('');
}