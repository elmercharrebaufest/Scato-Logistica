function Centro(id, descripcion) {
    if ($.isNumeric(parseInt(id))) {
        this.Id = id;
        this.Descripcion = descripcion;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.Descripcion = id.Descripcion;
    }
}

function Material(id, descripcion) {
    if ($.isNumeric(parseInt(id))) {
        this.Id = id;
        this.Descripcion = descripcion;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.Descripcion = id.Descripcion;
    }
}

function TiposComercial(id, descripcion) {
    if ($.isNumeric(parseInt(id))) {
        this.Id = id;
        this.Descripcion = descripcion;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.Descripcion = id.Descripcion;
    }
}

function ExportacionDeArchivosListViewModel() {
    var self = this;
    self.centros = ko.observableArray([]);
    self.centrosSeleccionados = ko.observableArray([]);
    
    self.materiales = ko.observableArray([]);
    self.materialesExcluidos = ko.observableArray([]);
  
    self.tiposComerciales = ko.observableArray([]);
    self.tiposComercialesSeleccionados = ko.observableArray([]);
    
    if ($("#CentrosJson").val() != "") {
        var mappedCentrosJson = $.map(JSON.parse($("#CentrosJson").val()), function (item) { return new Centro(item); });
        self.centros(mappedCentrosJson);
    }
    
    if ($("#TiposComercialesJson").val() != "") {
        var mappedTiposComercialesJson = $.map(JSON.parse($("#TiposComercialesJson").val()), function (item) { return new Material(item); });
        self.tiposComerciales(mappedTiposComercialesJson);
    }
    
    if ($("#MaterialesJson").val() != "") {
        var mappedMaterialesJson = $.map(JSON.parse($("#MaterialesJson").val()), function (item) { return new TiposComercial(item); });
        self.materiales(mappedMaterialesJson);
    }
    
    self.CentrosSeleccionados = function (data) {
        self.centrosSeleccionados.removeAll();
        
        $(data).each(function () {
            var seleccionado = this;
            var matchSinSeleccionar = ko.utils.arrayFirst(self.centros(), function (i) { return seleccionado.value == i.Id; });
            self.centrosSeleccionados.push(matchSinSeleccionar);
        });
    };
    
    self.MaterialesSeleccionados = function (data) {
        self.materialesExcluidos.removeAll();

        $(data).each(function () {
            var seleccionado = this;
            var matchSinSeleccionar = ko.utils.arrayFirst(self.materiales(), function (i) { return seleccionado.value == i.Id; });
            self.materialesExcluidos.push(matchSinSeleccionar);
        });
    };
    
    self.TiposComercialesSeleccionados = function (data) {
        self.tiposComercialesSeleccionados.removeAll();

        $(data).each(function () {
            var seleccionado = this;
            var matchSinSeleccionar = ko.utils.arrayFirst(self.tiposComerciales(), function (i) { return seleccionado.value == i.Id; });
            self.tiposComercialesSeleccionados.push(matchSinSeleccionar);
        });
    };
}

$(document).ready(function () {
    var modelo = new ExportacionDeArchivosListViewModel();
    ko.applyBindings(modelo);

    var listcentros = $('.centros').bootstrapDualListbox({
        bootstrap2Compatible: true,
        nonSelectedListLabel: 'Centros',
        selectedListLabel: 'Seleccionados',
        preserveSelectionOnMove: 'moved',
        moveOnSelect: false,
        showFilterInputs: false,
    });
    
    var listmateriales = $('.materiales').bootstrapDualListbox({
        bootstrap2Compatible: true,
        nonSelectedListLabel: 'Materiales',
        selectedListLabel: 'Excluidos',
        preserveSelectionOnMove: 'moved',
        moveOnSelect: false,
        showFilterInputs: false,
    });
    
    var listtiposComerciales = $('.tiposComerciales').bootstrapDualListbox({
        bootstrap2Compatible: true,
        nonSelectedListLabel: 'Tipos Comerciales',
        selectedListLabel: 'Seleccionados',
        preserveSelectionOnMove: 'moved',
        moveOnSelect: false,
        showFilterInputs: false,
    });

    if ($('#MaterialId').size() > 0) {
        DefinirAutocompletar('#Material', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    }

    $('.centros').on('change', function () {
        modelo.CentrosSeleccionados($(".centros option:selected"));
    });
    
    $('.materiales').on('change', function () {
        modelo.MaterialesSeleccionados($(".materiales option:selected"));
    });
    
    $('.tiposComerciales').on('change', function () {
        modelo.TiposComercialesSeleccionados($(".tiposComerciales option:selected"));
    });
    
    $('.info-container').addClass('hide');
    
    if ($("#CentrosSeleccionados").val() != "") {
        var mappedCentrosSeleccionadosJson = $.map(JSON.parse($("#CentrosSeleccionados").val()), function (item) { return new Centro(item); });
        mappedCentrosSeleccionadosJson.forEach(function (a) {
            $(".centros option[value='" + a.Id + "']").attr("selected", "selected");
        });
    }

    if ($("#TiposComercialesSeleccionados").val() != "") {
        var mappedTiposComercialesSeleccionadosJson = $.map(JSON.parse($("#TiposComercialesSeleccionados").val()), function (item) { return new Material(item); });
        mappedTiposComercialesSeleccionadosJson.forEach(function (a) {
            $(".tiposComerciales option[value='" + a.Id + "']").attr("selected", "selected");
        });
    }

    if ($("#MaterialesSeleccionados").val() != "") {
        var mappedMaterialesSeleccionadosJson = $.map(JSON.parse($("#MaterialesSeleccionados").val()), function (item) { return new TiposComercial(item); });
        mappedMaterialesSeleccionadosJson.forEach(function (a) {
            $(".materiales option[value='" + a.Id + "']").attr("selected", "selected");
        });
    }
    
    listcentros.bootstrapDualListbox('refresh');
    listmateriales.bootstrapDualListbox('refresh');
    listtiposComerciales.bootstrapDualListbox('refresh');
    modelo.CentrosSeleccionados($(".centros option:selected"));
    modelo.MaterialesSeleccionados($(".materiales option:selected"));
    modelo.TiposComercialesSeleccionados($(".tiposComerciales option:selected"));
    
    $('#bootstrap-duallistbox-nonselected-list_duallistbox_centros').bind('scroll', function () {
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            $.getJSON($(links).data().urlObtenerCentros, { pagina: $("#PaginaCentros").val() }, function(data) {
                if (data.lista != "") {
                    var mappedJson = $.map(data.lista, function(item) { return new Centro(item); });
                    mappedJson.forEach(function(a) {
                        modelo.centros.push(a);
                    });
                    listcentros.bootstrapDualListbox('refresh');
                    $("#PaginaCentros").val(parseInt($("#PaginaCentros").val()) + 1);
                }
            });
        }
    });
    $('#bootstrap-duallistbox-nonselected-list_duallistbox_materiales').bind('scroll', function () {
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            $.getJSON($(links).data().urlObtenerMateriales, { pagina: $("#PaginaMateriales").val() }, function (data) {
                if (data.lista != "") {
                    var mappedJson = $.map(data.lista, function (item) { return new Material(item); });
                    mappedJson.forEach(function (a) {
                        modelo.materiales.push(a);
                    });
                    listmateriales.bootstrapDualListbox('refresh');
                    $("#PaginaMateriales").val(parseInt($("#PaginaMateriales").val()) + 1);
                }
            });
        }
    });
    $('#bootstrap-duallistbox-nonselected-list_duallistbox_tiposComerciales').bind('scroll', function () {
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            $.getJSON($(links).data().urlObtenerTiposcomerciales, { pagina: $("#PaginaTiposComerciales").val() }, function (data) {
                if (data.lista != "") {
                    var mappedJson = $.map(data.lista, function (item) { return new TiposComercial(item); });
                    mappedJson.forEach(function (a) {
                        modelo.tiposComerciales.push(a);
                    });
                    listtiposComerciales.bootstrapDualListbox('refresh');
                    $("#PaginaTiposComerciales").val(parseInt($("#PaginaTiposComerciales").val()) + 1);
                }
            });
        }
    });
    
    $("#botonaceptar").on('click', function () {
        $("#botonaceptar").attr("disabled", "disabled");
    });

    DesbloquearBoton();
});

function DesbloquearBoton() {
    setInterval(function () {
        if ($.cookie('RetornoExportacion') != null) {
            $("#botonaceptar").removeAttr("disabled");
            $.removeCookie('RetornoExportacion', { path: '/' });
        }
    }, 1000);
}
