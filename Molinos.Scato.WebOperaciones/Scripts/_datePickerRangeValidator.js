$(document).ready(function () {
    $('#DateStart').datepicker('destroy').on("dp.change", function () {
        $('#DateFin').data("DateTimePicker").minDate($(this).val());
    });
    $('#DateFin').datepicker('destroy').on("dp.change", function () {
        $('#DateStart').data("DateTimePicker").maxDate($(this).val());
    });
    $("#ui-datepicker-div").hide();



});
