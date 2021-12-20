jQuery(document).ready(function ($) {
    //var camId = getUrlVars()["camId"];
    //$('#camarasDropDown').val(camId);
    
    if (!$('#materialesDropDown').val() > 0) {
        InicializarCombos();
    }

    $('#camarasDropDown').change(function () {
        $("#tabs").block({
            overlayCSS: { backgroundColor: 'white' },
            message: $('#Cargando').val(),
            onBlock: function () {
                $(".blockPage").addClass("alert alert-info");
            }
        });
        
        $.getJSON($("#SetearMaterialesPorCamara").val(), { camaraId: $("#camarasDropDown").val() },
            function (response) {
                var options = '';
                options += "<option value='" + "'>"
                            + $("#ValorDefault").val() + "</option>";
                for (var i = 0; i < response.length; i++) {
                    options += "<option value='" + response[i].Value + "'>"
                            + response[i].Text + "</option>";
                }
                $('#materialesDropDown').html(options);
                $('#materialesDropDown').attr("disabled", false);
            });
        $.get($('#ConsultarConversionPorCamara').val(), { camaraId: $("#camarasDropDown").val() }, function (data) {
            $('#gridContainer').html(data);
        }).complete(function () {
            $("#tabs").unblock();

            $("#Materialhref").attr('href', function (i, v) { return v.replace(/camaraId=([^&]+)/, function () { return 'camaraId=' + $("#camarasDropDown").val(); }); });
            $("#Procedenciahref").attr('href', function (i, v) { return v.replace(/camaraId=([^&]+)/, function () { return 'camaraId=' + $("#camarasDropDown").val(); }); });
            $("#Grupohref").attr('href', function (i, v) { return v.replace(/camaraId=([^&]+)/, function () { return 'camaraId=' + $("#camarasDropDown").val(); }); });
            $("#Caracteristicahref").attr('href', function (i, v) { return v.replace(/camaraId=([^&]+)/, function () { return 'camaraId=' + $("#camarasDropDown").val(); }); });
        });
    });

    function getUrlVars() {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    }

    $('#btnFiltrar').click(function () {
        var camId = $('#camarasDropDown').val();
        $(this).attr("href", $(this).attr("href") + "?camaraId=" + camId);
    });
});

function InicializarCombos() {
    $.getJSON($("#SetearMaterialesPorCamara").val(), { camaraId: $("#camarasDropDown").val() },
        function (response) {
            var options = '';
            options += "<option value='" + "'>"
                        + $("#ValorDefault").val() + "</option>";
            for (var i = 0; i < response.length; i++) {
                options += "<option value='" + response[i].Value + "'>"
                        + response[i].Text + "</option>";
            }
            $('#materialesDropDown').html(options);
            $('#materialesDropDown').attr("disabled", false);
        });
}