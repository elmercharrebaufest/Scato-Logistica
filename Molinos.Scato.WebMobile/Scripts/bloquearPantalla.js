
$(document).ajaxStop($.unblockUI);

$(document).ready(function () {
    $('.bloquear').click(function () {
        $.blockUI();

    });

});

