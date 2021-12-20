jQuery(document).ready(function ($) {
    $('#centrosLista').focus();
    
    var formatoFecha = Globalize.culture().calendars.standard.patterns.d.replace(/[a-z]/g, '9');
    formatoFecha = formatoFecha.replace(/[A-Z]/g, '9');
    
    var formatoHora = Globalize.culture().calendar.patterns.t.replace(/[a-su-zA-SU-Z]/g, '9');
    formatoHora = formatoHora.replace(/[t]/g, '');
    formatoHora = formatoHora.replace(formatoHora, '?' + formatoHora);
    
    $('#FechaDesde').mask(formatoFecha + " " + formatoHora);
    $('#FechaHasta').mask(formatoFecha + " " + formatoHora);
    
    $('#FechaDesde').on('change', function() {
        var fDesde = $('#FechaDesde').val();
        if ((fDesde.length - 1) == formatoFecha.length) {
            $('#FechaDesde').val($('#FechaDesde').val() + '00:00');
        }
    });
    
    $('#FechaHasta').on('change', function () {
        var fDesde = $('#FechaHasta').val();
        if ((fDesde.length - 1) == formatoFecha.length) {
            $('#FechaHasta').val($('#FechaHasta').val() + '00:00');
        }
    });


    $('#Titulo').css("margin-bottom","35px");
        
    if ($(".datetimeFormat").length > 0) {
        $(".datetimeFormat").html(Globalize.culture().calendar.patterns.d + " " + Globalize.culture().calendar.patterns.t);
    }

    $.validator.addMethod("date", function() {
        return Globalize.parseDate($('#FechaDesde').val()) <= Globalize.parseDate($('#FechaHasta').val()) || $('#FechaHasta').val() == "" || $('#FechaDesde').val() == "";
    }, $('#FechaHasta').data().secondDateValidation);

    $('#centrosLista').val('option: first').val(0);

    if ($('#centrosLista').val() == 0) {
            inhabilitarHumedimetro();
        }

    var cargoHumedimetros = false;

    $("#centrosLista").change(function () {
        cargoHumedimetros = false;
    });

    $("#centrosLista").blur(function () {
        cargarHumedimetros(cargoHumedimetros);
        cargoHumedimetros = true;
    });

    $("#centrosLista option").click(function () {
        cargarHumedimetros(cargoHumedimetros);
        cargoHumedimetros = true;
    });

});

function inhabilitarHumedimetro() {
    $('#humedimetrosLista').val('option: first').val(0);
    $('#humedimetrosLista').attr("disabled", "disabled");
}

function habilitarHumedimetro() {

    $('#humedimetrosLista').removeAttr("disabled");
    $('#humedimetrosLista').focus();
}

function cargarHumedimetros(cargoHumedimetros) {
    if (!cargoHumedimetros) {
        inhabilitarHumedimetro();
        if ($('#centrosLista').val() != 0) {
            setearDropDownHumedimetros($('#centrosLista').val());
        }
    }
}

function setearDropDownHumedimetros(centroId) {
    var url =$(links).data().urlObtenerHumedimetros;
    $.ajax({
        url: url,
        data: {centroId : centroId},
    dataType: "json",
    type: "GET",
    error: function () {
        alert("Ocurrió un error al actualizar la lista de humedimetros");
    },
    success: function (data) {
       $("#humedimetrosLista").empty();
       $.each(data, function (i) {
           var opcion = '<option value="' + data[i].Value + '">' + data[i].Text + '</option>';
           $("#humedimetrosLista").append(opcion);
       });
       habilitarHumedimetro();
   }
});

}