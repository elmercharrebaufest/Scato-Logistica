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
        
        var id = $(this).data('id');
        var patente = $(this).data('patente');
        var numeroDocumentoIngreso = $(this).data('numerodocumento');
        var workflowCodigo = $(this).data('workflowcodigo');
        var workflowDescripcion = $(this).data('workflowdescripcion');
        $('#FiltroEditar_Id').val(id);
        $('#FiltroEditar_PatenteActual').val(patente);
        $('#FiltroEditar_WorkflowCodigoActual').val(workflowCodigo);
        $('#FiltroEditar_NumeroDocumentoIngresoActual').val(numeroDocumentoIngreso);
        $('#FiltroEditar_WorkflowDescripcionActual').val(workflowDescripcion);
        $('#modalEditar').modal('show');

    });


    $(document).on('click', '.abrirModalBorrar', function (e) {
        e.preventDefault();

        var id = $(this).data('id');
        $('#IdBorrar').val(id);
        $('#modalBorrar').modal('show');

    });

    $('#WorkflowCodigo').change(function () {
        var texto = $('#WorkflowCodigo option:selected').text();
        $('#WorkflowDescripcion').val(texto);
    })

    $('#FiltroEditar_WorkflowCodigoActual').change(function () {
        var texto = $('#FiltroEditar_WorkflowCodigoActual option:selected').text();
        $('#FiltroEditar_WorkflowDescripcionActual').val(texto);
    })

    $('#WorkflowModal').change(function () {
        var texto = $('#WorkflowModal option:selected').text();
        $('#WorkflowDescripcionModal').val(texto);
    })

});

function Autorefresco(intervalo) {
    if ($("#modoDeRefresco").is(':checked')) {
        return setInterval(function () {
            CargarGrilla();
        }, 5000);
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

    $.get(url, function (data) {
        container.html(data);
    });
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
    if (xhr && xhr.responseText) {
        mensaje += "\n" + xhr.responseText;
    }
    LimpiarFormulario();
    MostrarAlertaError(mensaje);
}

function LimpiarFormulario() {
    $('#Patente').val(null);
    $('#WorkflowCodigo').val(null);
    $('#NumeroDocumentoIngreso').val(null);
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

