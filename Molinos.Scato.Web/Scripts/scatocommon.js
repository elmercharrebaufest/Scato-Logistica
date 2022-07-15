function attachDataPickers() {
    $('input.date').each(function () {
        var $this = $(this);
        $this.datepicker();
    });
}
var deleteLinkObj;
var confirmarLinkObj;
var mensajeAplicacionCompleto;
var mensajeAplicacionDetalle = { header: '', body: '' };
$(document).ready(function () {

    $("#dialogo-editar").draggable({
        handle: ".modal-header"
    });

    if (!$("#alerta").hasClass("hide")) {
        if ($("#alerta").hasClass("alert-success"))
            $("#alerta").delay(500).addClass("in").fadeOut(2500);
        else
            $("#alerta").delay(500).addClass("in");
    }
    attachDataPickers();
    $(document).on('click', '#grid thead th a, #grid tfoot td a', function (evt) {
        var container = $(this).parents('#gridContainer');
        if (container.attr('data-grid-url')) {
            container.data().gridUrl = this.href;
        }
        $.get(this.href, function (data) {
            container.html(data);
        });
        return false;
    });

    $(document).on("submit", "form.causaBlock", function () {
        BlockUI($("form.causaBlock").data().mensajeEspera);
    });

    /* delete Link */
    $(document).on('click', '.ajax-borrar-link', function () {
        deleteLinkObj = $(this); /*for future use*/
        $('#dialogo-borrar-motivo').remove();
        $('#dialogo-borrar').modal({
            //keyboard: false
        });
        return false; /* prevents the default behaviour */
    });

    /* delete Link */
    $(document).on('click', '.ajax-borrar-con-motivo-link', function () {
        deleteLinkObj = $(this); /*for future use*/
        $('#dialogo-borrar').modal({
            //keyboard: false
        });
        return false; /* prevents the default behaviour */
    });

    $('#dialogo-borrar').on('shown', function () {
        $('#dialogo-borrar-cancelar').focus();
    });


    $(function () {
        $("[data-hide]").on("click", function () {
            $(this).closest("." + $(this).attr("data-hide")).hide();
            // -or-, see below
            // $(this).closest("." + $(this).attr("data-hide")).hide();
        });
    });

    $('#dialogo-borrar-cancelar').click(function () {
        $('#dialogo-borrar').modal('hide');
    });

    $('#dialogo-borrar-confirmar').click(function () {
        $.post(deleteLinkObj[0].href, function (data) { /*Post to action*/
            if (data == 'true') {
                deleteLinkObj.closest("tr").hide(); /*Hide Row*/
                MostrarAlertaExitosa();
            } else {
                MostrarAlertaError(data);

            }
        })
            .error(
                function (message) {
                    alert(message.responseText);
                });
        $('#dialogo-borrar').modal('hide');
    });

    $('#dialogo-editar-cancelar').click(function () {
        $('#dialogo-editar').modal('hide');
    });

    $('#dialogo-editar-guardar').click(function () {
        $('#dialogo-editar form').submit();
        if ($('#dialogo-editar form').valid())
            $("#dialogo-editar-guardar").attr("disabled", true);
    });

    $(document).on('click', '.ajax-editar-link', function () {
        $.get(this.href, cargarDialogoEditar);
        return false;
    });

    $('#dialogo-ver-ok').click(function () {
        $('#dialogo-ver').modal('hide');
    });

    $(document).on('click', '.ajax-ver-link', function () {
        $.get(this.href, cargarDialogoVer);
        return false;
    });
    $(document).on('click', '.ajax-ver-link-centrado', function () {
        $.get(this.href, cargarDialogoVerCentrado);
        return false;
    });

    $(document).on('click', '.ajax-popup-link', function (event) {
        window.open(this.href, "popupWindow", "width=1000, height=561, scrollbars=yes,directories=no,location=no");
        return false;
    });

    $(document).on('click', '.ajax-popup-link2', function () {
        window.open(this.href, "popupWindow", "width=1250, height=561, scrollbars=yes,directories=no,location=no");
        return false;
    });

    $(document).on('click', '.ajax-popup-link3', function (event) {
        window.open($(this).data().url, "popupWindow", "width=1000, height=561, scrollbars=yes,directories=no,location=no");
        event.preventDefault();
        return false;
    });

    /* Generic Ajax error handling */
    $("#error-box").ajaxError(function (event, jqXhr, ajaxSettings, thrownError) {
        var $this = $(this);
        var errorText = $this.data().genericError + jqXhr.responseText;
        $this.html(errorText);
        $('#error-box-container').slideDown('slow');
    });

    $('#dialogo-editar').on('show', function () {
        $(this).find('.modal-body').css({
            height: 'auto', 'max-height': '400px', 'padding-right': '50px'
        });
    });

    $('#dialogo-editar').on('shown', function () {
        $(this).find('.modal-body').find(':input:enabled:visible:first').focus();
    });

    $('td:first-child input').change(function () {
        $(this).closest('tr').toggleClass("highlight", this.checked);
    });

    $(document).on('submit', '#search-form', function (data) {
        var container = $('#gridContainer');
        if (container.attr('data-grid-url')) {
            container.data().gridUrl = RemoverParametrosDeUrl(container.data().gridUrl) + "?filtro=" + $('#search-form input[name="filtro"]').val();
        }
    });

    ObtenerPuestoDeTrabajo();
    RedireccionarAListaAutomatizada();
    RedireccionarABalanzaAutomatizada();
    Mousetrap.bind('ctrl+alt+b', mostrarCreditos);
    //setTimeout(mensualCreditos,2000);

    $("#alertaFija").attr("disabled", true);
    $("#alertaFija-span").attr("disabled", true);
    $("#alertaFija").hide();

    obtenerMensajesAplicacion();
    setInterval(obtenerMensajesAplicacion, 60000);


    $("#alertaFija").click(function () {
        cargarDialogoVerCentrado(mensajeAplicacionDetalle);
        $('#dialogo-ver').css('margin-right', '360px');
        $('#dialogo-ver').modal('show');
    })
});

function BlockUI(message) {
    message = message != undefined ? message : '';
    message = $("#Procesando").val() + " " + message + ", " + $("#PorFavorEspere").val();
    $('#loading').text(message);
    var loading = $('#loading');
    var height = $(window).height();
    var width = $(document).width();
    $.blockUI.defaults.css = {
        left: width / 2 - (loading.width() / 2),
        top: height / 3 - (loading.height() / 3),
        backgroundColor: 'white',
        border: '1px solid #B94A41',
        color: '#0055A5',
        "padding-right": 20,
        paddingTop: 10,
        paddingBottom: 10,
        "padding-left": 20,

    };
    $.blockUI({
        overlayCSS: { backgroundColor: 'white' },
        message: $('#loading').html(),
        baseZ: 2000
    });
}

function cargarDialogoEditar(data) {
    $('#dialogo-editar-body').html(data);
    $("#dialogo-editar-guardar").attr("disabled", false);
    $('#dialogo-editar-title').html($('#dialogo-editar-body form').data().dialogoTitulo);
    $('#dialogo-editar-body form').attr('data-ajax-success', 'editarRepuestaFormulario');
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
            return -($(this).height() / 2.6);
        }
    });
    attachDataPickers();
}

function cargarDialogoVer(data) {
    $('#dialogo-ver-body').html(data);
    $('#dialogo-ver-body form').attr('data-ajax-success', 'editarRepuestaFormulario');
    $('#dialogo-ver').modal({
        backdrop: 'static', keyboard: false
    }).css({
        'top': '40%',
        //'margin-left': function () {
        //    return -($(this).width() / 2);
        //},
        //'left': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
            //return -($(this).height() / 2.6);
        }

    });
}


function cargarDialogoVerCentrado(data) {

    $('#dialogo-ver-body').html(data.body);
    $('#dialogo-ver-header').html(data.header);

    $('#dialogo-ver-body form').attr('data-ajax-success', 'editarRepuestaFormulario');
    $('#dialogo-ver-body').find('*').attr('disabled', true);
    $('#agregarVagon').attr("style", "visibility: hidden");
    $('#dialogo-ver').modal({
        backdrop: 'static', keyboard: false
    }).css({
        'top': '40%',
        'margin-top': '-209.5px',
        'margin-left': '-455px',
        'overflow': 'auto',

    });
    $('body').css({
        'overflow': 'hidden'
    });
}

function editarRepuestaFormulario(respuesta) {
    if (respuesta == window.ajaxEditSuccess) {
        window.location.href = window.location.href;
    } else {
        cargarDialogoEditar(respuesta);
    }
}

function editarRepuestaFormulario(respuesta) {
    if (respuesta == window.ajaxEditSuccess) {
        if ($('#search-form').length == 0) {
            window.location.href = window.location.href;
        } else {
            $('#dialogo-editar').modal('hide');
            var container = $('#gridContainer');
            if (container.length > 0 && container.is("[data-grid-url]")) {
                $.get(container.data().gridUrl, function (data) {
                    container.html(data);
                });
            } else {
                $('#search-form').submit();
            }
            MostrarAlertaExitosa();
        }
    } else {
        cargarDialogoEditar(respuesta);
    }
}

function MostrarAlertaError(data) {

    if (data != null) {
        $("#alertaError span").html(data);
    } else {
        $("#alertaError span").html($("#alertaError").data().mensaje);
    }
    $("#alertaError").show();
    $("#alertaError").delay(500).addClass("in");
}

function MostrarAlertaAdvertencia(data) {
    if (data != null) {
        $("#alertaAdvertencia span").html(data);
    } else {
        $("#alertaAdvertencia span").html($("#alertaAdvertencia").data().mensaje);
    }
    $("#alertaAdvertencia").show();
    $("#alertaAdvertencia").delay(500).addClass("in");

}

function MostrarAlertaExitosa(data, delay) {
    if (data != null) {
        $("#alertaExitosa span").html(data);
    }
    $("#alertaExitosa").show();
    $("#alertaExitosa").delay(500).addClass("in").fadeOut(delay ? delay : 2500);
}

function MostrarAlertaInfo(data) {
    if (data != null) {
        $("#alertaInfo span").html(data);
    }
    $("#alertaInfo").show();
    $("#alertaInfo").delay(500).addClass("in").fadeOut(10000);
}

function MostrarAlertaCancelada() {
    $("#alertaCancelada").show();
    $("#alertaCancelada").delay(500).addClass("in").fadeOut(2000);
}
function ActualizarEstadoServicios(mensaje) {
    $('#serviciosWeb').tooltip('destroy');
    if (mensaje != null && mensaje != undefined && mensaje != "Ok") {
        $("#serviciosWeb").css("color", "red");
        $('#serviciosWeb').tooltip({ 'title': "Los/el siguiente servicio se encuentran caidos: \n" + mensaje, 'placement': 'bottom', 'trigger': 'hover' });
    } else {
        $("#serviciosWeb").css("color", "rgb(0, 163, 24)");

    }
}
$(document).ready(function () {
    /*Centrar la primera vez*/
    CentrarPosicionElemento();
    /*Centrar por redimensión de pantalla*/
    $(window).resize(function (e) { e.preventDefault(); CentrarPosicionElemento(); });
});

function CentrarPosicionElemento() {
    $('.centro-pantalla').css({
        position: 'fixed',
        left: ($(window).width() - $('.centro-pantalla').outerWidth()) / 2,
        top: ($(window).height() - $('.centro-pantalla').outerHeight()) / 3
    });
}

function ObtenerNombrePCPorActiveX() {
    try {
        var x = new ActiveXObject("WScript.Shell");
        var prComputerName = x.Exec('cmd.exe /c echo %COMPUTERNAME% FIN%CLIENTNAME% FIN');
        var nombre = '';
        while (true) {
            if (!prComputerName.StdOut.AtEndOfStream) {
                nombre += prComputerName.StdOut.Read(1);
                if (nombre.indexOf(" FIN") != -1)
                    break;
            }
        }
        nombre = nombre.replace(' FIN', '');
        var nombre2 = '';
        while (true) {
            if (!prComputerName.StdOut.AtEndOfStream) {
                nombre2 += prComputerName.StdOut.Read(1);
                if (nombre2.indexOf(" FIN") != -1)
                    break;
            }
        }
        nombre2 = nombre2.replace(' FIN', '');
        if (nombre2.indexOf('CLIENTNAME') == -1) //checkeo si se conectó a traves de escritorio remoto
            nombre = nombre2;
        $.cookie('NombrePc', nombre);
        if (nombre.length == 0) {
            MostrarAlertaError($("#PuestoDeTrabajoErrorMensaje").val());
        } else {
            $("#nombrePc").text(nombre);
        }
        return nombre;
    } catch (e) {
        $.cookie('NombrePc', "NoTienePuesto");
        MostrarAlertaError($("#PuestoDeTrabajoErrorMensaje").val());
        return "";
    }
}

function ObtenerNombrePC() {
    var cookie = $.cookie('NombrePc');
    if (cookie != undefined && cookie.length > 0) { //existe el valor en la cookie
        return cookie;
    }
    var nombrePc;
    $.ajax({
        dataType: "json",
        url: $("#ObtenerNombrePcUrl").val(),
        async: false,
        data: {},
        success: function (nombre) {
            $.cookie('NombrePc', nombre, { expires: 365 });
            if (nombre.length == 0) {
                nombrePc = ObtenerNombrePCPorActiveX();
            } else {
                $("#nombrePc").text(nombre);
                nombrePc = nombre;
            }
        },
        error: function () {
            nombrePc = ObtenerNombrePCPorActiveX();
        }
    });
    return nombrePc;

    //$.getJSON($("#ObtenerNombrePcUrl").val(), {},
    //    function (nombre) {
    //        $.cookie('NombrePc', nombre);
    //        if (nombre.length == 0) {
    //            ObtenerNombrePCPorActiveX();
    //        } else {
    //            $("#nombrePc").text(nombre);
    //        }
    //    }).error(function () {
    //        ObtenerNombrePCPorActiveX();
    //    });
}

function ObtenerPuestoDeTrabajo() {
    var cookiePuesto = $.cookie('PuestoDeTrabajoId');
    try {
        if (cookiePuesto != undefined && cookiePuesto.length > 0 && cookiePuesto != '0') { //Ya existe el valor en la cookie
            return;
        }
        var nombrePc = ObtenerNombrePC();
        $.getJSON($("#ObtenerPuestoDeTrabajoUrl").val(), { nombrePc: nombrePc },
            function (data) {
                $.cookie('PuestoDeTrabajoId', data.Id, { expires: 365 });
            }).error(function () {
                $.cookie('PuestoDeTrabajoId', 0);
            });
    } catch (e) {
        $.cookie('PuestoDeTrabajoId', 0);
    }
}
function RedireccionarABalanzaAutomatizada() {
    var cookieRedireccion = $.cookie('RedireccionarABalanzaAutomatizada');
    try {
        if ((cookieRedireccion != undefined && cookieRedireccion.length > 0)) { //Ya existe el valor en la cookie
            return;
        }
        var nombrePc = ObtenerNombrePC();

        $.ajax({
            url: $("#RedireccionarABalanzaAutomatizadaUrl").val(),
            dataType: 'json',
            async: false,
            data: { nombrePc: nombrePc },
            success: function (data) {
                $.cookie('RedireccionarABalanzaAutomatizada', data);
            },
            error: function (data) {
                $.cookie('RedireccionarABalanzaAutomatizada', false);
            }
        });
    } catch (e) {
        $.cookie('RedireccionarABalanzaAutomatizada', false);
    }
}
function RedireccionarAListaAutomatizada() {
    var cookieRedireccion = $.cookie('RedireccionarAListaAutomatizada');
    try {
        if ((cookieRedireccion != undefined && cookieRedireccion.length > 0)) { //Ya existe el valor en la cookie
            return;
        }
        var nombrePc = ObtenerNombrePC();

        $.ajax({
            url: $("#RedireccionarAListaAutomatizadaUrl").val(),
            dataType: 'json',
            async: false,
            data: { nombrePc: nombrePc },
            success: function (data) {
                $.cookie('RedireccionarAListaAutomatizada', data);
            },
            error: function (data) {
                $.cookie('RedireccionarAListaAutomatizada', false);
            }
        });


        //$.getJSON($("#RedireccionarAListaAutomatizadaUrl").val(), { nombrePc: nombrePc }, async: false,
        //function (data) {
        //    $.cookie('RedireccionarAListaAutomatizada', data);
        //}).error(function () {
        //    $.cookie('RedireccionarAListaAutomatizada', false);
        //});
    } catch (e) {
        $.cookie('RedireccionarAListaAutomatizada', false);
    }
}

function RemoverParametrosDeUrl(oldUrl) {
    var index = 0;
    var newUrl = oldUrl;
    index = oldUrl.indexOf('?');
    if (index != -1) {
        newUrl = oldUrl.substring(0, index);
    }
    return newUrl;

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


function mostrarCreditos() {
    $.showCredits({
        title: 'Scato',
        version: '9',
        fecha: '22/05/2018',
        sdl: 'Alexis Zunino',
        analista: 'Jesus Gutierrez',
        tel: 'Alejandro Olivera',
        desarrolladores: 'Damian Sanchez, Diego Romero',
        testers: '',
        logoBaufest: '/Scato.web/Content/images/icon-baufest.png',
        logoCliente: '/Firma/Logo',
        liderCliente: 'Miguel Sarri',
        analistaCliente: 'Alejandro Vozella, Patricio Manna, Claudio Paz'
    });
}

function mensualCreditos() {
    var today = new Date();
    if (today.getDate() == 10 && today.getHours() == 11) {

        var credit = $.cookie("credit");
        if (credit == null) {
            mostrarCreditos();
            var minutes = 90;
            today.setTime(today.getTime() + (minutes * 60 * 1000));
            $.cookie("credit", "1", { expires: today });
        }
    }
}


function Totalizador(elemento, columnas, columnasTotales, columnaTotal) {
    var i;
    var fila = '<tr style="font-weight: bold;">';
    for (i = 1; i < columnasTotales; i++) {
        var suma = 0;
        if (columnas.indexOf(i) !== -1) {
            $(elemento + ' tbody tr').each(function (index) {
                suma += parseInt($('#grid tbody tr:nth-child(' + (index + 1) + ')  td:nth-child(' + i + ')').text(), 10);
            });
            fila += '<td>' + suma + '</td>';
        } else {
            if (columnaTotal == i) {
                fila += '<td>Total:</td>';
            } else {
                fila += '<td></td>';
            }
        }
    }
    fila += '</tr>';
    $(elemento + ' tbody:last').append(fila);
}

function byte2Hex(n) {
    var nybHexString = "0123456789ABCDEF";
    return String(nybHexString.substr((n >> 4) & 0x0F, 1)) + nybHexString.substr(n & 0x0F, 1);
}

function RGB2Color(r, g, b) {
    return '#' + byte2Hex(r) + byte2Hex(g) + byte2Hex(b);
}

function makeColorGradient(frequency1, frequency2, frequency3,
    phase1, phase2, phase3,
    center, width, len) {
    if (center == undefined) center = 128;
    if (width == undefined) width = 127;
    if (len == undefined) len = 50;

    var list = [];

    for (var i = 0; i < len; ++i) {
        var red = Math.sin(frequency1 * i + phase1) * width + center;
        var grn = Math.sin(frequency2 * i + phase2) * width + center;
        var blu = Math.sin(frequency3 * i + phase3) * width + center;

        list.push(RGB2Color(red, grn, blu));
    }

    return list;
}

function obtenerColoresParaGraficos(cantidad) {
    return makeColorGradient(2.4, 2.4, 2.4, 0, 2, 4, 128, 127, cantidad);
}

function reemplazarSaltosDeLinea(cadena) {
    while (cadena.indexOf('\n') > 0) {
        cadena = cadena.replace('\n', '<br />');
    }

    return cadena;
}

function obtenerMensajesAplicacion() {
    $.ajax({
        url: $(ObtenerMensajeFijoUrl).val(),
        type: 'GET',
        cache: false
    }).done(function (result) {

        if (result && result.TipoAccion != "Baja" && result.TipoAccion.length > 1) {
            mensajeAplicacionDetalle = { header: result.FechaString + 'hs <br /> ' + result.Titulo, body: reemplazarSaltosDeLinea(result.Detalle) };
            var mensajeCompleto = " " + result.FechaString + "hs : " + "<strong>" + result.Titulo + "</strong>" + ", " + (result.Detalle.length > 10 ? result.Detalle.substring(0, 30) + "... (Haga Click aqui para ver mas detalle)" : result.Detalle);
            $("#alertaFija span").html(mensajeCompleto);
            $("#alertaFija").show();
        }
        else {
            //msjAplicacion;
            $("#alertaFija").attr("disabled", true);
            $("#alertaFija-span").attr("disabled", true);
            $("#alertaFija").hide();
        }
    });
}

function NotificarCambioEstadoBarrera(mensaje) {
    var estadoBarreras = JSON.parse(mensaje);

    try {

        for (var j = 0; j < estadoBarreras.SensoresArriba.length; j++) {
            var item = estadoBarreras.SensoresArriba[j];
            if (item.Estado) {
                $("#barrera-" + item.Id + "-" + item.Barrera).removeClass("icon-barrera-cerrada");
                $("#barrera-" + item.Id + "-" + item.Barrera).addClass("icon-barrera-abierta").css({ 'color': 'forestgreen' });
                $("#barrera-" + item.Barrera).removeClass("icon-barrera-cerrada");
                $("#barrera-" + item.Barrera).addClass("icon-barrera-abierta").css({ 'color': 'forestgreen' });
                $("#barrera-supervisor-" + item.Id + "-" + item.Barrera).removeClass("icon-barrera-cerrada");
                $("#barrera-supervisor-" + item.Id + "-" + item.Barrera).addClass("icon-barrera-abierta").css({ 'color': 'forestgreen' });
            }
        }

        for (var k = 0; k < estadoBarreras.SensoresAbajo.length; k++) {
            var item = estadoBarreras.SensoresAbajo[k];
            if (item.Estado) {
                $("#barrera-" + item.Id + "-" + item.Barrera).removeClass("icon-barrera-abierta");
                $("#barrera-" + item.Id + "-" + item.Barrera).addClass("icon-barrera-cerrada").css({ 'color': 'red' });
                $("#barrera-" + item.Barrera).removeClass("icon-barrera-abierta");
                $("#barrera-" + item.Barrera).addClass("icon-barrera-cerrada").css({ 'color': 'red' });
            }
        }
    } catch (e) {
        console.log(e)
    }
}