var obtenerProveedorPorId = $('#links').data().urlObtenerProveedorPorId;
var obtenerCorredoresPorEstablecimiento = $('#links').data().urlObtenerCorredoresPorEstablecimientoId;

$(document).ready(function () {
    ko.applyBindings(new CorredoresListViewModel(), document.getElementById('corredoresViewModel'));
    $("#CodigoRENSPA").inputmask("99.999.9.99999/99");
});

function Corredor(id, descripcion, cuil, sap, razonsocial) {
    this.Id = id;
    this.Descripcion = descripcion;
    this.Cuil = cuil;
    this.CodigoSap = sap;
    this.RazonSocial = razonsocial;
}

function CorredoresListViewModel() {
    var self = this;
    self.corredores = ko.observableArray([]);
    let listado = new Array();
    //Validacion es Ventana Crear?
    if ($('#Id').val() > 0) {
        //Seteo Inicial de Corredores Asociados Actuales
        $.getJSON(obtenerCorredoresPorEstablecimiento, { Id: $('#Id').val() }, function (data, status) {
            //Recuperar Lista de Corredores
            data.forEach(function (item) {
                let viejoCorredor = new Corredor(item.Id, item.Descripcion, item.Cuil, item.CodigoSap, item.RazonSocial);
                self.corredores.push(viejoCorredor);
                listado.push(item.Id);
            });
            //Persistencia
            let jsonData = ko.toJSON(self.corredores);
            $('#corredoresFinales').val(jsonData);
        });
    }
    // Operaciones
    self.addCorredor = function () {
        
        if ($('#CorredorId').val() > 0) {
            $.getJSON(obtenerProveedorPorId, { Id: $('#CorredorId').val() }, function (data, status) {
                //recuoera Proveedor segun Id de Corredor
                let nuevoCorredor = new Corredor(data.Id, data.Descripcion, data.Cuil, data.CodigoSap, data.RazonSocial);
                //Validacion Existe?
                let corredorEncontrado=listado.includes(data.Id);
                if(!corredorEncontrado) {
                    self.corredores.push(nuevoCorredor);
                    listado.push(data.Id);
                }
                //Persistencia
                var jsonData = ko.toJSON(self.corredores);
                $('#corredoresFinales').val(jsonData);
            });
        }
        resetCorredorSeleccionado();
    };

    self.removeCorredor = function (corredor) {
        let index = listado.indexOf(corredor.Id);
        listado.splice(index, 1);
        self.corredores.remove(corredor);
        $('#corredoresFinales').val(ko.toJSON(self.corredores));
    };

    function resetCorredorSeleccionado() {
        $('#Corredor').val("");
        $('#CorredorId').val("");
    }
}