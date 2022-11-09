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
        $("#alertaExitosa").html(data);
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


function obtenerColoresParaGraficos(cantidad) {
    return makeColorGradient(2.4, 2.4, 2.4, 0, 2, 4, 128, 127, cantidad);
}
