$(document).ready(function () {
    CopiarFiltros();
    
    var loading = $('#gridContainer');
    var height = $(window).height();
    var width = $(document).width();
    $.blockUI.defaults.css = {
        left: width / 2 - loading.width() / 2,
        top: height / 3 - loading.height() / 3,
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

    var cookieRedireccion = $.cookie('RedireccionarAListaAutomatizada');
    var cookieRedireccionBalanza = $.cookie('RedireccionarABalanzaAutomatizada');
    var venimosDeListaDeTareasAutomatizada = $("#VenimosDeListaDeTareasAutomatizada").val();
    if (cookieRedireccionBalanza === "true" && (venimosDeListaDeTareasAutomatizada === null || venimosDeListaDeTareasAutomatizada === "False")) {
        window.location = $("#BalanzaAutomatizadaUrl").val();
    }
    else if (cookieRedireccion === "true" && (venimosDeListaDeTareasAutomatizada === null || venimosDeListaDeTareasAutomatizada === "False")) {
        window.location = $("#ListaAutomatizadaUrl").val();
    } else {
        CargarGrilla(function () { $("#gridContainer").unblock(); });

        var intervalo = Autorefresco(null);
        $('#modoDeRefresco').change(function () {
            CargarGrilla();
            intervalo = Autorefresco(intervalo);
        });


        $('#NumeroDocumentoDeIngreso').attr("disabled", "disabled");

        HabilitarNumeroDeDocumento();
        $('#TipoDocumentoDeIngreso').change(function () {
            HabilitarNumeroDeDocumento();
        });

        $('#DropDownColumnas').bind('hide', function () {
            CargarGrilla();
        });

        $('.columnasdropdown input, .columnasdropdown label').click(function (e) {
            e.stopPropagation();
        });
        
    }
});

function HabilitarNumeroDeDocumento() {
    if ($('#TipoDocumentoDeIngreso').val() !== "") {
        $('#NumeroDocumentoDeIngreso').removeAttr("disabled");
    } else {
        $('#NumeroDocumentoDeIngreso').attr("disabled", "disabled");
        $('#NumeroDocumentoDeIngreso').val("");
    }
}

function Autorefresco(intervalo) {
    if ($("#modoDeRefresco").is(':checked')) {
        return setInterval(function() {
            CargarGrilla();
        }, 5000);
    } else {
        if (intervalo !== null) clearInterval(intervalo);
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
    var checkMaterial = $("#checkMaterial").is(':checked') ? "Material|" : "";
    var checkTransportista = $("#checkTransportista").is(':checked') ? "Transportista|" : "";
    var checkCuit = $("#checkCuit").is(':checked') ? "Cuit|" : "";
    var checkTipoDocumentoIngreso = $("#checkTipoDocumentoIngreso").is(':checked') ? "TipoDocumentoIngreso|" : "";
    var checkNumeroDocumentoIngreso = $("#checkNumeroDocumentoIngreso").is(':checked') ? "NumeroDocumentoIngreso|" : "";
    var checkCalidad = $("#checkCalidad").is(':checked') ? "Calidad|" : "";
    var checkCentroId = $("#checkCentroId").is(':checked') ? "CentroId|" : "";
    var checkCentro = $("#checkCentro").is(':checked') ? "Centro|" : "";
    var checkfechaCreacion = $("#checkFechaCreacion").is(':checked') ? "FechaCreacion|" : "";
    var checkFechaUltimaModificacion = $("#checkFechaUltimaModificacion").is(':checked') ? "FechaUltimaModificacion|" : "";
    var checkNumeroDeTarjeta = $("#checkNumeroDeTarjeta").is(':checked') ? "NumeroDeTarjeta|" : "";
    var checkChoferDNI = $("#checkChoferDNI").is(':checked') ? "ChoferDNI|" : "";
    var checkChoferNombre = $("#checkChoferNombre").is(':checked') ? "ChoferNombre|" : "";
    var checkProcedencia = $("#checkProcedencia").is(':checked') ? "Procedencia|" : "";
    var checkFoto = $("#checkFoto").is(':checked') ? "Foto|" : "";
    var checkEntregador = $("#checkEntregador").is(':checked') ? "Entregador|" : "";
    var checkProteina = $("#checkProteina").is(':checked') ? "Proteina|" : "";
    var checkAlmacenDestino = $("#checkAlmacenDestino").is(':checked') ? "AlmacenDestino|" : "";
    var checkDifPeso = $("#checkDifPeso").is(':checked') ? "DiferenciaPesoNeto|" : "";

    var columnas = checkCalidad + checkCentroId + checkCuit + checkMaterial + checkTipoDocumentoIngreso + checkNumeroDocumentoIngreso
        + checkPatente + checkProximaEtapa + checkTransportista + checkWorkflow + checkfechaCreacion + checkFechaUltimaModificacion + checkCentro + checkMaterialId + checkTipoVehiculo + checkNumeroDeTarjeta + checkChoferDNI + checkChoferNombre + checkProcedencia + checkFoto + checkEntregador + checkProteina + checkAlmacenDestino + checkDifPeso;
    if (columnas[columnas.length - 1] === "|")
        columnas = columnas.slice(0, -1);


    if ($("#filtroOrdenarPor").val() != null && $("#filtroOrdenarPor").val() != "") {
        var columnasSeparadas = columnas.split('|');
        if (columnasSeparadas.filter(function (columna) { return columna == $("#filtroOrdenarPor").val() }).length == 0) {
            if (columnasSeparadas.length > 0) {
                $("#filtroOrdenarPor").val(columnasSeparadas[0]);
            }
        }
    }
    
    //Agrego filtros
    url = UpdateQueryString("Columnas", columnas, url);
    url = UpdateQueryString("Workflow", $("#filtroWorkflow").val(), url);
    url = UpdateQueryString("ProximaAccion", $("#filtroProximaAccion").val(), url);
    url = UpdateQueryString("TipoDocumentoDeIngreso", $("#filtroTipoDocumentoDeIngreso").val(), url);
    url = UpdateQueryString("NumeroDocumentoDeIngreso", $("#filtroNumeroDocumentoDeIngreso").val(), url);
    url = UpdateQueryString("Patente", $("#filtroPatente").val(), url);
    url = UpdateQueryString("SoloDemorados", $("#filtroSoloDemorados").val(), url);
    url = UpdateQueryString("ExcluirRechazados", $("#filtroExcluirRechazados").val(), url);
    url = UpdateQueryString("TiempoMaxEntreActividades", $("#filtroTiempoMaxEntreActividades").val(), url);
    url = UpdateQueryString("TipoMaterial", $("#filtroTipoMaterial").val(), url);
    url = UpdateQueryString("TieneEntregador", $("#filtroTieneEntregador").val(), url);
  


    url = UpdateQueryString("OrdenarPor", $("#filtroOrdenarPor").val(), url);
    url = UpdateQueryString("DirOrden", $("#filtroDirOrden").val(), url);

    $.get(url, function (data) {
        container.html(data);
        if (callback !== null && callback !== undefined) {
            callback();
        } 
    });
}

function CopiarFiltros() {
    $("#filtroWorkflow").val($("#Workflow").val());
    $("#filtroProximaAccion").val($("#ProximaAccion").val());
    $("#filtroTipoDocumentoDeIngreso").val($("#TipoDocumentoDeIngreso").val());
    $("#filtroNumeroDocumentoDeIngreso").val($("#NumeroDocumentoDeIngreso").val());
    $("#filtroPatente").val($("#Patente").val());
    $("#filtroSoloDemorados").val($("#SoloDemorados").is(':checked'));
    $("#filtroExcluirRechazados").val($("#ExcluirRechazados").is(':checked'));
    $("#filtroTipoMaterial").val($("#TipoMaterial").val());
    $("#filtroTieneEntregador").val($("#TieneEntregador").val());

    if ($("#hOrdenarPor").length > 0) {
        $("#filtroOrdenarPor").val($("#hOrdenarPor").val());
        $("#filtroDirOrden").val($("#hDirOrden").val());
    }
    else
    {
        $("#filtroOrdenarPor").val('');
        $("#filtroDirOrden").val('');
    }

    var container = $('#gridContainer');
    if (container.attr('data-grid-url')) {
        container.data().gridUrl = window.location.href;
    }
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
