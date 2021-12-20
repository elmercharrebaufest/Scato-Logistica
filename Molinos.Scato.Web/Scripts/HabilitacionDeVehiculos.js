jQuery(document).ready(function ($) {

    $(':checkbox').change(function () {

        BlockUI();
        $.getJSON(url, { id: this.id }, function (data) {
            if (data !== "") {
                MostrarAlertaError(data.value)

            } else {
                MostrarAlertaExitosa();
            }
        }).complete(function () {
            $.unblockUI();
        });
    });
});
