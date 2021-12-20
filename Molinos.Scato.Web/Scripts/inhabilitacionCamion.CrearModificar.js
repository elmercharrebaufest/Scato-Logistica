$(document).ready(function () {
    ko.applyBindings(new AdjuntoListViewModel());
});


function Adjunto(id, archivo, descripcion) {
    if ($.isNumeric(parseInt(id))) {
        this.Id = id;
        this.Archivo = archivo;
        this.Descripcion = descripcion;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.Descripcion = id.Descripcion;
    }
}

function AdjuntoListViewModel() {
    console.log($('#Id').val());
    var self = this;
    self.adjuntos = ko.observableArray([]);
    if ($("#Id").val() != "") {
    $.getJSON("InhabilitacionCamion/ObtenerAdjuntos", { id: $('#Id').val() },
        function (allData) {
            var mappedAdjuntos = $.map(allData, function (item) {
                return new Adjunto(item);
            });
            self.adjuntos(mappedAdjuntos);
            $('#adjuntosFinales').val(ko.toJSON(mappedAdjuntos));
        }
        );
    }


    // Operations
    self.fileUpload = function (data, e) {
        var file = e.target.files[0];
        var reader = new FileReader();

        reader.onloadend = function (onloadend_e) {
            var result = reader.result;
            self.adjuntos.push(new Adjunto(0, result, $('#adjunto').val().split('\\').pop()));
            $('#adjuntosFinales').val(ko.toJSON(self.adjuntos));
        };

        if (file) {
            reader.readAsDataURL(file);
        }
    };

    self.removeAdjunto = function (adjunto) {
        self.adjuntos.remove(adjunto);
        $('#adjuntosFinales').val(ko.toJSON(self.adjuntos));
    };

    self.downloadAdjunto = function (adjunto) {
        Download(adjunto);
    };
}