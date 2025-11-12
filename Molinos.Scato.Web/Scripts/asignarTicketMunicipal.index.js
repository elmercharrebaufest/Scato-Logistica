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


    //var intervalo = Autorefresco(null);
    //$('#modoDeRefresco').change(function () {
    //    CargarGrilla();
    //    intervalo = Autorefresco(intervalo);
    //});


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

    var checkPatente = $("#checkPatente").is(':checked') ? "Patente|" : "";
    var checkWorkflow = $("#checkWorkflow").is(':checked') ? "Workflow|" : "";
    var checkMaterialId = $("#checkMaterialId").is(':checked') ? "MaterialId|" : "";
    var checkMaterial = $("#checkMaterial").is(':checked') ? "Material|" : "";
    var checkTipoDocumentoIngreso = $("#checkTipoDocumentoIngreso").is(':checked') ? "TipoDocumentoIngreso|" : "";
    var checkNumeroDocumentoIngreso = $("#checkNumeroDocumentoIngreso").is(':checked') ? "NumeroDocumentoIngreso|" : "";
    var checkCentroId = $("#checkCentroId").is(':checked') ? "CentroId|" : "";
    var checkCentro = $("#checkCentro").is(':checked') ? "Centro|" : "";
    var checkfechaCreacion = $("#checkFechaCreacion").is(':checked') ? "FechaCreacion|" : "";
    var checkTipoComercial = $("#checkTipoComercial").is(':checked') ? "TipoComercial|" : "";
    
    var columnas = checkCentroId +  checkMaterial + checkTipoDocumentoIngreso + checkNumeroDocumentoIngreso
        + checkPatente + checkWorkflow + checkfechaCreacion + checkTipoComercial + checkCentro + checkMaterialId;
    if (columnas[columnas.length - 1] == "|")
        columnas = columnas.slice(0, -1);

    //Agrego filtros
    //url = UpdateQueryString("Columnas", columnas, url);
    //url = UpdateQueryString("TipoDocumentoDeIngreso", $("#filtroTipoDocumentoDeIngreso").val(), url);
    //url = UpdateQueryString("NumeroDocumentoDeIngreso", $("#filtroNumeroDocumentoDeIngreso").val(), url);
    //url = UpdateQueryString("Patente", $("#filtroPatente").val(), url);

    //url = UpdateQueryString("Workflow", $("#filtroWorkflow").val(), url);
    //url = UpdateQueryString("TipoComercialId", $("#filtroTipoComercialId").val(), url);
    //url = UpdateQueryString("MaterialId", $("#filtroMaterialId").val(), url);
    
    $.get(url, function (data) {
        container.html(data);
    });
}

function CopiarFiltros() {
    $("#filtroTipoDocumentoDeIngreso").val($("#TipoDocumentoDeIngreso").val());
    $("#filtroNumeroDocumentoDeIngreso").val($("#NumeroDocumentoDeIngreso").val());
    $("#filtroPatente").val($("#Patente").val());

    $("#filtroWorkflow").val($("#Workflow").val());
    $("#filtroTipoComercialId").val($("#TipoComercialId").val());
    $("#filtroMaterialId").val($("#MaterialId").val());
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


//NN

//function cargarDialogoEditarPagaTicketMunicipal(data) {
//    $('#dialogo-editar-body').html(data);
//    $("#dialogo-editar-guardar").attr("disabled", false);
//    $('#dialogo-editar-title').html($('#dialogo-editar-body form').data().dialogoTitulo);
//    $('#dialogo-editar-body form').attr('data-ajax-success', 'editarRepuestaFormularioAsignacionTicketMunicipal');
//    if ($('#dialogo-editar-body form').data().dialogoExtraclass) {
//        $('#dialogo-editar').addClass($('#dialogo-editar-body form').data().dialogoExtraclass);
//    }

//    $('#dialogo-editar').modal({
//        backdrop: 'static', keyboard: false
//    }).css({
//        'top': '30%',
//        'margin-left': function () {
//            return -($(this).width() / 2);
//        },
//        'left': '50%',
//        'margin-top': function () {
//            return -($(this).height() / 2.6);
//        }
//    });
//    attachDataPickers();
//}


//function editarRepuestaFormulario(respuesta) {
//    if (respuesta == window.ajaxEditSuccess) {
//        CargarGrilla();
//        $('#dialogo-editar').modal('hide');
//        MostrarAlertaExitosa();        
//    } else {
//        cargarDialogoEditar(respuesta);
//    }
//}
