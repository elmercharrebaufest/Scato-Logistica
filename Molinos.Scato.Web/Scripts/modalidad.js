$('#motivo').on("change paste keyup", function () {
    var length = $(this).val().length;
    var length = 255 - length;
    $('#contador').text(length);
    

});