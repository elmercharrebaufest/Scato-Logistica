$(document).ready(function () {
    $.unblockUI();
    $("#orden-form").keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });

    $("#numeroPedidoSap").mask("9999999999");

    refrescarBotones();
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

    $('td:first-child input:checked').each(function () {
        $(this).closest('tr').toggleClass("highlight", this.checked);
    });
    
    $('td:first-child input').change(function () {
        $(this).closest('tr').toggleClass("highlight", this.checked);
    });
});

function pad(str, max) {
    str = str.toString();
    return str.length < max ? pad("0" + str, max) : str;
}
function refrescarBotones() {
    if ($('#Numero').val() == 0 || $('#numeroPedidoSap').val() == "") {
        $('#descargarBoton').prop('disabled', true);
        $('#btnRechazar').prop('disabled', true);
        $('#btnAceptar').prop('disabled', true);
        $('#btnTerminar').prop('disabled', true);
        $('#btnTerminarVehiculo').prop('disabled', true);
    }
    else {
        $('#descargarBoton').prop('disabled', false);
        //$('#btnRechazar').prop('disabled', false);
        $('#btnAceptar').prop('disabled', false);
        $('#btnTerminar').prop('disabled', false);
        $('#btnTerminarVehiculo').prop('disabled', false);
    }

    if ($('#numeroPedidoSap').val() != "") {
        $('#numeroPedidoSap').prop('disabled', true);
    }
    else {
        $('#numeroPedidoSap').prop('disabled', false);

        $('#numeroPedidoSap').change(function () {
            BlockUI($("#numeroPedidoSap").data().buscandoPedido);
            $.getJSON($("#numeroPedidoSap").data().cargarPedidoUrl, { nroPedido: $('#numeroPedidoSap').val(), workflowInstanceId: $('#workflowInstanceId').val() }, function (data) {
                if (data.error != "" && data.error != null) {
                    MostrarAlertaError(data.error);
                } else {
                    $('#numeroPedidoSap').prop('disabled', true);
                    $('#descargarBoton').prop('disabled', false);
                    //$('#btnRechazar').prop('disabled', false);
                    $('#btnAceptar').prop('disabled', false);
                    $('#btnTerminar').prop('disabled', false);
                    $('#btnTerminarVehiculo').prop('disabled', false);
                    $('#dropDownNumerosRomaneo').append(
                        $('<option></option>').val(data.romaneoNumero).html(pad(data.romaneoNumero, 10)).attr('selected', 'selected')
                    );
                    $('#dropDownNumerosRomaneo').html($('#dropDownNumerosRomaneo option').sort(
                        function (x, y) {
                            return $(x).text() < $(y).text() ? -1 : 1;
                        }));
                    $('#Numero').val(data.romaneoNumero);
                    $('#NumeroIndex').val(data.romaneoNumero);
                    $('#NumeroIndex').val(data.romaneoNumero);
                    $('#descargarBoton').attr('href', data.urlDescargarItems);
                    if (data.proveedor.split('^-')[0] == "distintos") {
                        $('#ProveedorDescripcion').html(data.proveedor.split('^-')[1]);
                        $('#ProveedorDescripcion').addClass("label-warning");
                        $('#ProveedorDescripcion').removeClass("label-success");
                        $('#ProveedorDescripcion').attr('data-toggle', 'tooltip');
                        $('#ProveedorDescripcion').attr('data-placement', 'bottom');
                        $('#ProveedorDescripcion').attr('title', $('#ProveedorDescripcion').data().warning);
                    }
                    else {
                        $('#ProveedorDescripcion').html(data.proveedor);
                        $('#ProveedorDescripcion').addClass("label-success");
                        $('#ProveedorDescripcion').removeClass("label-warning");
                        $('#ProveedorDescripcion').removeAttr('data-toggle');
                        $('#ProveedorDescripcion').removeAttr('data-placement');
                        $('#ProveedorDescripcion').removeAttr('title');
                    }
                }
            }).complete(function () {
                $.unblockUI();
            });
        });
    }
}