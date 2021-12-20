
$(document).ready(function () {
    //CargarActividades($('#WorkflowId').val());
    $('#WorkflowId').change(function () {
        CargarActividades($(this).val());
    });
    if ($("#WorkflowId").val() == "") {
        $('#Actividad').prop('disabled', true);
    }
});

/*Carga de segundo combo dependiendo del workflow elegido*/
function CargarActividades(selectedWf) {
    $('#Actividad').prop('disabled', true);
    if (selectedWf != null && selectedWf != '') {
        BlockUI($("#BuscandoActividadesMensaje").val());
        $.getJSON($("#ObtenerActividadesUrl").val(), { workflowId: selectedWf }, function (report) {
            $('#Actividad').prop('disabled', false);
            $('#Actividad').each(function () {
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
        }).complete(function () {
            $.unblockUI();
        });
    }
}