jQuery(document).ready(function ($) {
    //var camId = getUrlVars()["camId"];
    //$('#camarasDropDown').val(camId);
    
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

    $('#camarasDropDown').change(function () {
        $("#tabs").block({
            overlayCSS: { backgroundColor: 'white' },
            message: $('#Cargando').val(),
            onBlock: function () {
                $(".blockPage").addClass("alert alert-info");
            }
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

    $('#btnFiltrar').click(function () {
        var camId = $('#camarasDropDown').val();
        $(this).attr("href", $(this).attr("href") + "?camaraId=" + camId);
    });
});