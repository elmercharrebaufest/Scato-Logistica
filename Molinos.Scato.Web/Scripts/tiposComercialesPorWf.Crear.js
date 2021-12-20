$(document).ready(function () {
    CargarTiposComerciales($('#WorkflowId').val());
    $('#WorkflowId').change(function () {
        CargarTiposComerciales($(this).val());
    });
});

/*Carga de segundo combo dependiendo del workflow elegido*/
function CargarTiposComerciales(selectedWf) {
    if (selectedWf != null && selectedWf != '') {
        $('#TipoComercialId').prop('disabled', true);
        $.getJSON('TipoComercialPorWf/ObtenerTiposComerciales', { workflowId: selectedWf }, function (report) {
            var tiposCombo = $('#TipoComercialId');
            tiposCombo.empty();
            tiposCombo.append($('<option/>', {
                value: "",
                text: $('#TipoComercialId').data().primeraOpcion
            }));
            $.each(report, function (index, data) {
                if (data.disable == false) {
                    tiposCombo.append($('<option/>', {
                        value: data.value,
                        text: data.text
                    }));
                } else {
                    tiposCombo.append($('<option/>', {
                        value: data.value,
                        text: data.text,
                        disabled: 'disabled',
                        'class': 'opcionDeshabilitada'
                    }));
                }
                $('#TipoComercialId').prop('disabled', false);
            });
        });
    }
}