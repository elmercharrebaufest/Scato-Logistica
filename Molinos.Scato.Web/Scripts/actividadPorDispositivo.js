function Salida(codigo, descripcion) {
    this.Codigo = codigo;
    this.Descripcion = descripcion;
}

function Entrada(codigo, descripcion) {
    this.Codigo = codigo;
    this.Descripcion = descripcion;
}

function VideoCamara(id ,codigo, descripcion, directorio) {
    this.Id = id;
    this.Codigo = codigo;
    this.Descripcion = descripcion;
    this.Directorio = directorio;
}

function DispositivoListViewModel() {
    var self = this;
    self.salidas = ko.observableArray([]);
    self.videoCamaras = ko.observableArray([]);
    self.entradas = ko.observableArray([]);

    $.getJSON("ActividadPorDispositivo/ObtenerDispositivos", { id: $('#Id').val() },
        function (allData) {
            var mappedSalidas = $.map(allData, function (item) {
                //inhabilito la opcion
                $("#salida option[value=" + item.Codigo + "]").attr('disabled', 'disabled');
                setearDropDownSelected("salida");
                return new Salida(item.Codigo, item.Descripcion);
            });
            self.salidas(mappedSalidas);
            $('#salidasFinales').val(ko.toJSON(mappedSalidas));
        }
    );

    $.getJSON("ActividadPorDispositivo/ObtenerDispositivosEntrada", { id: $('#Id').val() },
        function (allData) {
            var mappedEntradas = $.map(allData, function (item) {
                //inhabilito la opcion
                $("#entrada option[value=" + item.Codigo + "]").attr('disabled', 'disabled');
                setearDropDownSelected("entrada");
                return new Entrada(item.Codigo, item.Descripcion);
            });
            self.entradas(mappedEntradas);
            $('#Finales').val(ko.toJSON(mappedEntradas));
        }
    );

    if ($('#videoCamarasFinales').val() !== undefined) {
        var camaras = jQuery.parseJSON($('#videoCamarasFinales').val());
        var mappedVideoCamaras = $.map(camaras, function (item) {
            //inhabilito la opcion
            $("#videoCamara option[value=" + item.Codigo + "]").attr('disabled', 'disabled');
            setearDropDownSelected("videoCamara");
            return new VideoCamara(item.Id, item.Codigo, item.Descripcion, item.Directorio);
        });
        self.videoCamaras(mappedVideoCamaras);
        $('#videoCamarasFinales').val(ko.toJSON(mappedVideoCamaras));
    }

    // Operations
    self.addSalida = function () {
        if ($('#salida option:selected').text() > "") {
            self.salidas.push(new Salida($('#salida option:selected').val(), $('#salida option:selected').text()));
            //inhabilito la opcion
            $("#salida option:selected").attr('disabled', 'disabled');
            setearDropDownSelected("salida");
        }
        $('#salidasFinales').val(ko.toJSON(self.salidas));
    };

    self.addEntrada = function () {
        if ($('#entrada option:selected').text() > "") {
            self.entradas.push(new Entrada($('#entrada option:selected').val(), $('#entrada option:selected').text()));
            //inhabilito la opcion
            $("#entrada option:selected").attr('disabled', 'disabled');
            setearDropDownSelected("entrada");
        }
        $('#entradasFinales').val(ko.toJSON(self.entradas));
    };

    self.addVideoCamara = function () {
        if ($('#videoCamara option:selected').text() > "" && $("#videoCamaraDirectorio").val() > "") {
            self.videoCamaras.push(new VideoCamara(0, $('#videoCamara option:selected').val(), $('#videoCamara option:selected').text(), $('#videoCamaraDirectorio').val()));
            //inhabilito la opcion
            $("#videoCamara option:selected").attr('disabled', 'disabled');
            setearDropDownSelected("videoCamara");
            $('#videoCamaraDirectorio').val("");
        }
        $('#videoCamarasFinales').val(ko.toJSON(self.videoCamaras));
    };
    
    self.removeSalida = function (salida) {
        //habilito la opcion
        $("#salida option[value=" + salida.Codigo + "]").removeAttr('disabled');
        self.salidas.remove(salida);
        $('#salidasFinales').val(ko.toJSON(self.salidas));
    };

    self.removeEntrada = function (entrada) {
        //habilito la opcion
        $("#entrada option[value=" + entrada.Codigo + "]").removeAttr('disabled');
        self.entradas.remove(entrada);
        $('#entradaFinales').val(ko.toJSON(self.entradas));
    };

    self.removeVideoCamara = function (videoCamara) {
        //habilito la opcion
        $("#videoCamara option[value=" + videoCamara.Codigo + "]").removeAttr('disabled');
        self.videoCamaras.remove(videoCamara);
        $('#videoCamarasFinales').val(ko.toJSON(self.videoCamaras));
    };
}

function setearDropDownSelected(combo) {
    var seleccionado = false;
    $.each($("#" + combo + " option"), function (index, value) {
        if (value.disabled) {
            value.selected = false;
        } else {
            if (!seleccionado) {
                value.selected = true;
                seleccionado = true;
            }
        }
    });
}


$(document).ready(function () {
    ko.applyBindings(new DispositivoListViewModel());

    //Inicializo el id que es obligatorio
    if ($('#Id').val() == '') {
        $('#Id').val(0);
    }
    //CargarActividades($('#WorkflowId').val());
    $('#WorkflowId').change(function () {
        CargarActividades($(this).val());
    });
    if ($("#WorkflowId").val() == "") {
        $('#Actividad').prop('disabled', true);
    }
});

/*Carga de segundo combo dependiendo del workflow elegido*/
function CargarActividades(selectedWf) {
    $('#Actividad').prop('disabled', true);
    if (selectedWf != null && selectedWf != '') {
        BlockUI($("#BuscandoActividadesMensaje").val());
        $.getJSON($("#ObtenerActividadesUrl").val(), { workflowId: selectedWf }, function (report) {
            $('#Actividad').prop('disabled', false);
            $('#Actividad').each(function () {
                var value = $(this).val();
                var combo = $(this);
                combo.empty();
                combo.append($('<option/>', {
                    value: "",
                    text: ""
                }));
                $.each(report, function (index, data) {
                    combo.append($('<option/>', {
                        value: data.value,
                        text: data.text,
                        selected: data.value == value
                    }));
                });
                combo.val(value);
            });
        }).complete(function () {
            $.unblockUI();
        });
    }
}