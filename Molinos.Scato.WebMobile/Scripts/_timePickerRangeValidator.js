$(document).ready(function () {
    $('#BeginTime').datepicker('destroy').on("dp.change", function () {
        $('#EndTime').data("DateTimePicker").minDate($(this).val());
    });
    $('#EndTime').datepicker('destroy').on("dp.change", function () {
        $('#BeginTime').data("DateTimePicker").maxDate($(this).val());
    });
    $("#ui-datepicker-div").hide();
});