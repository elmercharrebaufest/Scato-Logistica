let EstadoHidraulica = {
    Inhabilitado: 0,
    Disponible: 1,
    Llamando: 2,
    Ocupado: 3
};

let habilitadoDeshabilitado;

$(function () {
    setInterval(() => {
        let hidraulicas = ActualizarHidraulicas();
        hidraulicas.map(hidraulica => ActualizarInfoHidraulica(hidraulica))
    }, 4000)
});

function ActualizarInfoHidraulica(hidraulica) {
    if (hidraulica.Id > 0) {
        $(`#tiempo-${hidraulica.Id}`).html(CalcularTiempoEnCola(hidraulica.FechaUltimaModificacionEstado));
        $(`#estado-${hidraulica.Id}`).text(Object.keys(EstadoHidraulica).find(key => EstadoHidraulica[key] === hidraulica.Estado));
        Coloreado(hidraulica.Estado, hidraulica.Id);
        $(`#nombre-${hidraulica.Id}`).text(hidraulica.HidraulicaNombre);
        if (hidraulica.UltimaPatenteLlamada && hidraulica.UltimaPatenteLlamada.length > 0) {
            $(`#patente-${hidraulica.Id}`).text(hidraulica.UltimaPatenteLlamada);
        } else {
            $(`#patente-${hidraulica.Id}`).text("-");
            $(`#patente-${hidraulica.Id}`).addClass("text-white");
        }
    }
}

function Coloreado(estado, id) {
    Inicializador(id);

    switch (estado) {
        case EstadoHidraulica.Disponible:
            $('#tiempo-' + id).addClass("bg-Disponible");
            $('#estado-' + id).addClass("text-color-Disponible");
            $('#habilitado-' + id).prop('disabled', true);
            $('#deshabilitado-' + id).prop('disabled', false);
            break;
        case EstadoHidraulica.Inhabilitado:
            $('#tiempo-' + id).addClass("bg-Inhabilitado");
            $('#estado-' + id).addClass("text-color-Inhabilitado");
            $('#habilitado-' + id).prop('disabled', false);
            $('#deshabilitado-' + id).prop('disabled', true);
            break;
        case EstadoHidraulica.Llamando:
            $('#tiempo-' + id).addClass("bg-Llamando");
            $('#estado-' + id).addClass("text-color-Llamando");
            $('#habilitado-' + id).prop('disabled', false);
            $('#deshabilitado-' + id).prop('disabled', false);
            break;
        case EstadoHidraulica.Ocupado:
            $('#tiempo-' + id).addClass("bg-Ocupado");
            $('#estado-' + id).addClass("text-color-Ocupado");
            $('#habilitado-' + id).prop('disabled', false);
            $('#deshabilitado-' + id).prop('disabled', false);
            break;
    }
}

function CalcularTiempoEnCola(fechaIngeso) {
    var fechaActual = Date.now();
    var fechaInicioDeCola = new Date(parseInt(fechaIngeso.substr(6)));

    let diffMilli = fechaActual - fechaInicioDeCola;
    let secondsInMilli = 1000;
    let minutesInMilli = secondsInMilli * 60;
    let hoursInMilli = minutesInMilli * 60;

    let diffHrs = Math.floor(diffMilli / hoursInMilli);
    diffMilli = diffMilli % hoursInMilli;

    let diffMins = Math.floor(diffMilli / minutesInMilli);
    diffMilli = diffMilli % minutesInMilli;

    diffHrs = (diffHrs < 10) ? "0" + diffHrs : diffHrs;
    diffMins = (diffMins < 10) ? "0" + diffMins : diffMins;

    return diffHrs < 01 && diffMins < 60 ? diffMins + 'm' : diffHrs + "h " + diffMins + 'm';
}

function Inicializador(id) {
    $('#tiempo-' + id).removeClass();
    $('#estado-' + id).removeClass();
    $('#patente-' + id).removeClass();
    $('#tiempo-' + id).addClass("hidraulica__tiempo");
}

function ActualizarHidraulicas() {
    let hidraulicas = [];
    $.ajax({
        url: urlHidraulicas,
        type: 'GET',
        contentType: 'application/json;',
        dataType: 'json',
        async: false,
        success: function (data) {
            hidraulicas = data?.hidraulicas;
        },
        error: function (data) {
            MostrarAlertaError();
        }
    });
    return hidraulicas;
}

function OpenModal(estado, id){
    habilitadoDeshabilitado = estado;
    document.getElementById('modalConfirmarHabilitarDeshabilitar-' + id).showModal();
}

function CambiarEstado(id, nombre) {
    let nuevoEstado = habilitadoDeshabilitado ? EstadoHidraulica.Disponible : EstadoHidraulica.Inhabilitado;
    $.ajax({
        url: urlEstado,
        type: 'POST',
        data: {
            nuevoEstado: nuevoEstado,
            id: id,
            nombre: nombre
        },
        async: false,
        success: function (data) {
            MostrarAlertaExitosa();
        },
        error: function (data) {
            MostrarAlertaError();
        },
        complete: function(){
            document.getElementById('modalConfirmarHabilitarDeshabilitar-' + id).close();
        }
    });

}
