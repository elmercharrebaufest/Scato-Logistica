$(document).ready(function () {
    $("#patente").keydown(function () {
        $(this).val($(this).val().toUpperCase());
    });
    $('.campoNumerico').keydown(function (event) {
        if (event.keyCode === 8 || event.keyCode === 9 || event.keyCode === 16 || event.keyCode === 39 || event.keyCode === 37 || event.keyCode === 46 || event.keyCode === 13) {
            //aca no hay nada
        } else {
            if (event.keyCode === 190 || event.keyCode === 188 || event.keyCode === 110) {
                $(this).val($(this).val() + Globalize.culture().numberFormat["."]);
                event.preventDefault();
            }
            else if (!((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105))) {
                event.preventDefault();
            }
        }
    });
    var listarProveedores = $('#links').data().urlBuscarProveedores;
    var obtenerProveedor = $('#links').data().urlBuscarProveedor;
    var obtenerProveedorSap = $('#links').data().urlObtenerProveedoresSap;
    DefinirAutocompletarConSAP('#TitularCartaPorte', '#TitularCartaPorteId', '#autocompleteTCP', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    DefinirAutocompletarConSAP('#Corredor', '#CorredorId', '#autocompleteRtte', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    DefinirAutocompletar('#MaterialDesc', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);

    $("#TitularCartaPorte").change();

    $("#numDocumento").bind("paste", function (e) {
        e.preventDefault();
        if (e.originalEvent.clipboardData !== undefined) {
            clipText = e.originalEvent.clipboardData.getData('text/plain');
        } else {
            clipText = window.clipboardData.getData('text');
        }
        $("#numDocumento").val(clipText.replace(/(\r\n|\n|\r)/gm, ";"));
    });

});

function BajarTodasLasFotos() {

    GenerarUrl("urlVer");
    GenerarUrl("urlDescargar");

}

function GenerarUrl(i) {
    var todosCPs = "";
    $("#grid tbody tr td:nth-child(2) a.ajax-popup-link").each(function (index, accion) {
        var id = accion.getAttribute("href").split('/').pop();
        todosCPs += id.split('?')[0] + "-";
    });
    $("#" + i).attr('disabled', true);
    if (todosCPs.length > 0) {
        $("#"+i).attr('disabled', false);
        todosCPs = todosCPs.slice(0, -1);
    }
    var path = $("#" + i).data().url;
    if (path.indexOf('?') !== -1) {
        var split = path.split("?");
        path = split.slice(0, split.length - 1).join("");
    }

    $("#" + i).data().url = path + "?id=" + todosCPs;
}

