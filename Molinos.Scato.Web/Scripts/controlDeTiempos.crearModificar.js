$(document).ready(function () {
    //Inicializo el id que es obligatorio
    if ($('#Id').val() == '') {
        $('#Id').val(0);
    }
    //CargarActividades($('#WorkflowId').val());
    $('#WorkflowId').change(function () {
        CargarActividades($(this).val());
    });
    if ($("#WorkflowId").val() == "") {
        $('#ActividadDesde').prop('disabled', true);
        $('#ActividadHasta').prop('disabled', true);
    }
});

/*Carga de segundo combo dependiendo del workflow elegido*/
function CargarActividades(selectedWf) {
    $('#ActividadDesde, #ActividadHasta').prop('disabled', true);
    if (selectedWf != null && selectedWf != '') {
        BlockUI($("#BuscandoActividadesMensaje").val());
        $.getJSON($("#ObtenerActividadesUrl").val(), { workflowId: selectedWf }, function (report) {
            $('#ActividadDesde, #ActividadHasta').prop('disabled', false);
            $('#ActividadDesde, #ActividadHasta').each(function () {
                var value = $(this).val();
                var combo = $(this);
                combo.empty();
                combo.append($('<option/>', {
                    value: "",
                    text: ""
                }));
                $.each(report, function (index, data) {
                    combo.append($('<option/>', {
                        value: data.value,
                        text: data.text,
                        selected: data.value == value
                    }));
                });
                combo.val(value);
            });
        }).complete(function() {
            $.unblockUI();
        });
        
    }
}