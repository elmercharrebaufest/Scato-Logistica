$(document).ready(function () {
    $("#zonasDropDown").change(function () {
    $.getJSON($("#CargarSubZonas").val(), { zonaId: $("#zonasDropDown").val() },
            function (response) {
                var options = '';
                options += "<option value='" + "'>"
                            + $("#ValorDefaultSubZona").val() + "</option>";
                for (var i = 0; i < response.length; i++) {
                    options += "<option value='" + response[i].Value + "'>"
                            + response[i].Text + "</option>";
                }   
                $('#subzonasDropDown').html(options);
                $('#subzonasDropDown').attr("disabled", false);
            });
    });
});