$(document).ready(function() {
    $('.patente-internacional').mask('?*******');
    
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

    $('#dialogo-editar').show(function() {
        $('#dialogo-editar-guardar').removeAttr('disabled');
    });
    
    $('#btn-filtrar').click();

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

    DefinirAutocompletar('#MaterialDesc', '#MaterialId', $('#linksRechazados').data().urlBuscarMateriales, $('#linksRechazados').data().urlBuscarMaterial);
    
});

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

function CargarGrilla() {
    var container = $('#gridContainer');
    //Obtengo url de la grilla
    var url = container.data().gridUrl;
    //Verifico si el atributo refresco no está seteado
    url = UpdateQueryString("refresco", $("#modoDeRefresco").is(':checked'), url);

    var checkPatente = $("#checkPatente").is(':checked') ? "Patente|" : "";
    var checkMaterial = $("#checkMaterial").is(':checked') ? "Material|" : "";
    var checkNumeroDocumentoIngreso = $("#checkNumeroDocumentoIngreso").is(':checked') ? "NumeroDocumentoIngreso|" : "";
    var checkTransportista = $("#checkTransportista").is(':checked') ? "Transportista|" : "";
    var checkFoto = $("#checkFoto").is(':checked') ? "Foto|" : "";
    var checkProximaEtapa = $("#checkProximaEtapa").is(':checked') ? "ProximaEtapa|" : "";
    var checkMotivoDeRechazo = $("#checkMotivoDeRechazo").is(':checked') ? "MotivoDeRechazo|" : "";
    

    var checkNumeroDeTarjeta = $("#checkNumeroDeTarjeta").is(':checked') ? "NumeroDeTarjeta|" : "";
    var checkFechaCreacion = $("#checkFechaCreacion").is(':checked') ? "FechaCreacion|" : "";
    var checkFechaCalado = $("#checkFechaCalado").is(':checked') ? "FechaCalado|" : "";
    var checkTieneEntregador = $("#checkTieneEntregador").is(':checked') ? "TieneEntregador|" : "";
    var checkEnPlanta = $("#checkEnPlanta").is(':checked') ? "EnPlanta|" : "";
    var checkProveedor = $("#checkProveedor").is(':checked') ? "Proveedor|" : "";
    var checkRechazoEtapa = $("#checkRechazoEtapa").is(':checked') ? "RechazoEtapa|" : "";
    
    var columnas = checkMaterial + checkNumeroDocumentoIngreso
        + checkPatente + checkTransportista + checkFoto + checkProximaEtapa + checkMotivoDeRechazo + checkNumeroDeTarjeta + checkFechaCreacion + checkEnPlanta + checkProveedor + checkFechaCalado + checkTieneEntregador + checkRechazoEtapa;
    if (columnas[columnas.length - 1] == "|")
        columnas = columnas.slice(0, -1);

    //Agrego filtros
    url = UpdateQueryString("Columnas", columnas, url);
    url = UpdateQueryString("NumeroDocumentoDeIngreso", $("#filtroNumeroDocumentoDeIngreso").val(), url);
    url = UpdateQueryString("Patente", $("#filtroPatente").val(), url);
    url = UpdateQueryString("Transportista", $("#filtroTransportista").val(), url);
    url = UpdateQueryString("Material", $("#filtroMaterial").val(), url);

    $.get(url, function (data) {
        container.html(data);
    });
}

function CopiarFiltros() {
    $("#filtroNumeroDocumentoDeIngreso").val($("#NumeroDocumentoDeIngreso").val());
    $("#filtroPatente").val($("#Patente").val());
    $("#filtroTransportista").val($("#Transportista").val());
    $("#filtroMaterial").val($("#Material").val());
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

function editarRepuestaFormulario(respuesta) {
    if (respuesta == window.ajaxEditSuccess) {
        CargarGrilla();
        $('#dialogo-editar').modal('hide');
        MostrarAlertaExitosa();        
    } else {
        cargarDialogoEditar(respuesta);
    }
}
