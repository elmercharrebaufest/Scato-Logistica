$(document).ready(function () {
    $.unblockUI();
    $("#orden-form").keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });
    if ($('td:first-child input:checked').length > 0 && $('descargarBoton').length > 0) {
        $("#btnRechazar").attr("disabled", false);
    } else {
        $("#btnRechazar").attr("disabled", true);
    }

    $('td:first-child input:checked').each(function() {
        $(this).closest('tr').toggleClass("highlight", this.checked);
    });

    $("#btnTerminar").prop('disabled', $("#Estado").val() == $("#EstadoFinalizado").val());
    $("#Observaciones").prop('disabled', $("#Estado").val() == $("#EstadoFinalizado").val());

    $('td:first-child input').change(function () {
        $(this).closest('tr').toggleClass("highlight", this.checked);
        if ($('td:first-child input:checked').length > 0) {
            $("#btnRechazar").attr("disabled", false);
        } else {
            $("#btnRechazar").attr("disabled", true);
        }
    });
    
    $('#NumeroIndex').val($('#Numero').val());
    
    var proveedor = $('#ProveedorDescripcion').html();
    if (proveedor.split('^-')[0] == "distintos") {
        $('#ProveedorDescripcion').html(proveedor.split('^-')[1]);
        $('#ProveedorDescripcion').addClass("label-warning");
        $('#ProveedorDescripcion').removeClass("label-success");
        $('#ProveedorDescripcion').attr('data-toggle', 'tooltip');
        $('#ProveedorDescripcion').attr('data-placement', 'bottom');
        $('#ProveedorDescripcion').attr('title', $('#ProveedorDescripcion').data().warning);
    }
    else {
        $('#ProveedorDescripcion').addClass("label-success");
        $('#ProveedorDescripcion').removeClass("label-warning");
        $('#ProveedorDescripcion').removeAttr('data-toggle');
        $('#ProveedorDescripcion').removeAttr('data-placement');
        $('#ProveedorDescripcion').removeAttr('title');
    }
    

    if ($('#Numero').val() == 0 || $('#numeroPedidoSap').val() == "") {
        $('#descargarBoton').prop('disabled', true); 
        $('#btnRechazar').prop('disabled', true);
        $('#btnAceptar').prop('disabled', true);
        $('#btnTerminar').prop('disabled', true);
        $('#btnTerminarVehiculo').prop('disabled', true);
    }
    else {
        $("#btnTerminar").prop('disabled', $("#Estado").val() == $("#EstadoFinalizado").val());
        $("#Observaciones").prop('disabled', $("#Estado").val() == $("#EstadoFinalizado").val());
        $('#btnAceptar').prop('disabled', false);

        $('#btnTerminarVehiculo').prop('disabled', false);
    }

    if ($('#numeroPedidoSap').val() != "") {
        $('#numeroPedidoSap').prop('disabled', true);
    }
    else {
        $('#numeroPedidoSap').prop('disabled', false);

        $('#numeroPedidoSap').change(function () {
            if (($('#numeroPedidoSap').val().length != 10 || !$.isNumeric($('#numeroPedidoSap').val())) && $('#numeroPedidoSap').val().length != 0) {
                MostrarAlertaError($("#NumeroPedidoSapInvalido").val());
                return false;
            } else if ($('#numeroPedidoSap').val().length == 0) {
                return false;
            }
            BlockUI($("#numeroPedidoSap").data().buscandoPedido);
            $.getJSON($("#numeroPedidoSap").data().cargarPedidoUrl, { nroPedido: $('#numeroPedidoSap').val(), workflowInstanceId: $('#workflowInstanceId').val() }, function (data) {
                if (data.error != "" && data.error != null) {
                    MostrarAlertaError(data.error);
                } else {
                    $('#numeroPedidoSap').prop('disabled', true);
                    $("#btnTerminar").prop('disabled', $("#Estado").val() == $("#EstadoFinalizado").val());
                    $("#Observaciones").prop('disabled', $("#Estado").val() == $("#EstadoFinalizado").val());
                    //$('#btnRechazar').prop('disabled', false);
                    $('#btnAceptar').prop('disabled', false);

                    $('#btnTerminarVehiculo').prop('disabled', false);
                    $('#dropDownNumerosRomaneo').append(
                        $('<option></option>').val(data.romaneoNumero).html(pad(data.romaneoNumero, 10)).attr('selected', 'selected')
                    );
                    $('#dropDownNumerosRomaneo').html($('#dropDownNumerosRomaneo option').sort(
                        function (x, y) {
                            return $(x).text() < $(y).text() ? -1 : 1;
                        }));
                    window.location = window.location;
                }
            }).complete(function () {
                $.unblockUI();
            });
        });
    }
});

function pad(str, max) {
    str = str.toString();
    return str.length < max ? pad("0" + str, max) : str;
}



