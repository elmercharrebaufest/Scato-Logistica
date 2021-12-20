var i = 0;
function FormatoDeCampo(id, campoId, campoDescripcion) {
    if (id == 0) {
        this.Id = id;
        this.CampoId = campoId;
        this.CampoDescripcion = campoDescripcion;
        this.LetraId = ko.observable(1);
        this.Alineacion = ko.observable(0);
        this.Fila = ko.observable("0");
        this.Columna = ko.observable("0");
        this.Tamaño = ko.observable("10");
        this.Negrita = ko.observable(false);
        this.Cursiva = ko.observable(false);
        this.Subrayado = ko.observable(false);
        this.EsColumna = ko.observable(false);
        this.TipoDeCampo = ko.observable(0);
        this.Texto = ko.observable("");
        this.EsNuevo = true;
        this.Indice1 = ko.observable(i++);
        this.Indice2 = ko.observable(i++);
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.CampoId = id.CampoId;
        this.CampoDescripcion = id.CampoDescripcion;
        this.LetraId = ko.observable(id.LetraId);
        this.Alineacion = ko.observable(id.Alineacion);
        this.Fila = ko.observable(id.Fila);
        this.Columna = ko.observable(id.Columna);
        this.Tamaño = ko.observable(id.Tamaño);
        this.Negrita = ko.observable(id.Negrita);
        this.Cursiva = ko.observable(id.Cursiva);
        this.Subrayado = ko.observable(id.Subrayado);
        this.EsNuevo = id.EsNuevo;
        this.EsColumna = ko.observable(id.EsColumna);
        this.TipoDeCampo = ko.observable(id.TipoDeCampo);
        this.Texto = ko.observable(id.Texto);
        this.Indice1 = ko.observable(i++);
        this.Indice2 = ko.observable(i++);
        this._destroy = id._destroy;
    }
}

function FormatoDeCampoModel() {
    // Inicializo observers
    var self = this;
    self.formatosDeCampos = ko.observableArray([]);

    // Obtengo objetos Descuento del dominio
    if ($("#camposPostBack").val() != "") {
        //Si es un postback del controller: (modelo invalido)
        var mappedcampos = $.map(JSON.parse($("#camposPostBack").val()), function (item) { return new FormatoDeCampo(item); });
        self.formatosDeCampos(mappedcampos);

    } else {
        //Si es la primera vez que entramos:
        $.getJSON($("#botonAgregarCampo").data().obtenerCampos, { formatoDeImpresionId: $('#formatoDeImpresionId').val() },
            function (allData) {
                var mappedcamposs = $.map(allData, function (item) { return new FormatoDeCampo(item); });
                self.formatosDeCampos(mappedcamposs);
            }
        );
    }
    
    // Operations
    self.botonAgregarCampo = function () {
        self.formatosDeCampos.push(new FormatoDeCampo(0, $('#campo').val(), $("#campo :selected").text()));
    };
    
    self.removeCampo = function (campo) {
        self.formatosDeCampos.destroy(campo);
    };
}

$(document).ready(function () {
    ko.applyBindings(new FormatoDeCampoModel(), document.getElementById('grilla'));
    
    $.validator.addMethod("filaValida", function (value, element) {
        return value < parseInt($("#Filas").val());
    }, $('#mensajes').data().errorFila);
    $.validator.addMethod("columnaValida", function (value, element) {
        return value < parseInt($("#Columnas").val());
    }, $('#mensajes').data().errorColumna);
    
    $.validator.addMethod("mayorQueCero", function (value, element) {
        return value >= 0;
    }, $('#mensajes').data().cero);
});