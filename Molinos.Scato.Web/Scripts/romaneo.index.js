$(document).ready(function () {
    //Acción para el botón de próximo item
    $("#dialogo-editar-boton").click(function () {
        $("#proximoItem").val(true);
        $("#dialogo-editar-guardar").click();
    });

    //Sobre-escribo botones al diálogo editar
    $("#dialogo-editar-boton").removeClass("hide").addClass("btn-primary").val($("#RomaneoDescargar_ProximoItem").val());
    $("#dialogo-editar-guardar").removeClass("btn-primary").val($("#Aceptar").val());
    $("#dialogo-editar-cancelar").addClass("btn-warning");

    $("#orden-form").keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });

    //Seteo el estado finalizado
    $("#dialogo-confirmar-confirmar").click(function () {
        $("#Estado").val($("#EstadoFinalizado").val());
    });

    var itemsNuevos = false;
    $("#dialogo-editar-guardar").on('click', function () {
        itemsNuevos = true;
    });

    $("#dialogo-editar-cancelar").click(function () {
        if (itemsNuevos) {
            $.get($("#romaneoContainer").data().indexUrl, { numero: $('#Numero').val(), workflowInstanceId: $('#workflowInstanceId').val() }, function (data) {
                $('#dialogo-editar').modal('hide');
                $("#gridContainer").html(data);
                itemsNuevos = false;
            });
        }
    });


    $(document).on('click', '.confirmar-boton', function () {
        $("#dialogo-confirmar-confirmar").val($(this).data().accion);
        $("#dialogo-confirmar-body").text($(this).data().mensaje);
        $("#dialogo-confirmar").modal('show');
        return false;
    });

    $("#nuevoRomaneo").click(function () {
        $('#dropDownNumerosRomaneo').val(0);
    });
});

function editarRepuestaFormulario(respuesta) {
    if (respuesta == window.ajaxEditSuccess) {
        $.get($("#romaneoContainer").data().indexUrl, { numero: $('#Numero').val(), workflowInstanceId: $('#workflowInstanceId').val() }, function (data) {
            $('#dialogo-editar').modal('hide');
            $("#gridContainer").html(data);
            MostrarAlertaExitosa();
        }).complete(function () {
            $.unblockUI();
        });
    } else {
        cargarDialogoEditar(respuesta);
        $.unblockUI();
    }
}