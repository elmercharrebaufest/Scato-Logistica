$(document).ready(function() {
    let loading = $('#gridContainer');
    let height = $(window).height();
    let width = $(document).width();

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
  
    $(document).on('click', '.abrirModal', function (e) {
        e.preventDefault();
        
        $('#FiltroEditar_Id').val($(this).data('id'));
        $('#FiltroEditar_PatenteActual').val($(this).data('patente'));
        $('#modalEditar').modal('show');
    });

    $(document).on('click', '.abrirModalBorrar', function (e) {
        e.preventDefault();

        $('#idBorrar').val($(this).data('id'));
        $('#modalBorrar').modal('show');
    });
});

function Autorefresco(intervalo) {
    if ($("#modoDeRefresco").is(':checked')) {
        return setInterval(function () {
            CargarGrilla();
            $('#FiltroPatente').val(null);
        }, 25000);
    } else {
        if (intervalo != null)
            clearInterval(intervalo);
        return null;
    }
}

function CargarGrilla() {
    let container = $('#gridContainer');

    $.get(
        UpdateQueryString("refresco", $("#modoDeRefresco").is(':checked'), container.data().gridUrl),
        function (data) {
            container.html(data);
        }
    );
}

function onFormSuccess(result) {
    LimpiarFormulario();
}

function onFormBorrarSuccess(result) {
    $('#modalBorrar').modal('hide');
}

function onFormEditarSuccess(result) {
    $('#modalEditar').modal('hide');
    LimpiarFormulario();
}

function onFormError(xhr, status, error) {
    let mensaje = "Error al procesar la solicitud.";

    if (xhr) {
        if (xhr.responseJSON && xhr.responseJSON.mensaje) {
            mensaje += "\n" + xhr.responseJSON.mensaje;
        } else if (xhr.responseText) {
            mensaje += "\n" + xhr.responseText;
        } else if (xhr.statusText) {
            mensaje += "\n" + xhr.status + " " + xhr.statusText;
        }
    }

    LimpiarFormulario();
    MostrarAlertaError(mensaje);
}

function LimpiarFormulario() {
    $('#Patente').val(null);
}

function UpdateQueryString(key, value, url) {
    if (!url)
        url = window.location.href;

    let regex = new RegExp("([?|&])" + key + "=.*?(&|#|$)(.*)", "gi");

    if (regex.test(url)) {
        if (typeof value !== 'undefined' && value !== null)
            return url.replace(regex, '$1' + key + "=" + value + '$2$3');
        else {
            let hash = url.split('#');
            url = hash[0].replace(regex, '$1$3').replace(/(&|\?)$/, '');
            if (typeof hash[1] !== 'undefined' && hash[1] !== null)
                url += '#' + hash[1];
            return url;
        }
    }
    else {
        if (typeof value !== 'undefined' && value !== null) {
            let separator = url.indexOf('?') !== -1 ? '&' : '?', hash2 = url.split('#');
            url = hash2[0] + separator + key + '=' + value;
            if (typeof hash2[1] !== 'undefined' && hash2[1] !== null)
                url += '#' + hash2[1];
            return url;
        }
        else
            return url;
    }
}

