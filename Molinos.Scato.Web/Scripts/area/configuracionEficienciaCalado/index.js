function ConfiguracionEficienciaCaladoVM(config) {
    this.containerId = config.containerId;
    this.generalIds = config.generalIds;
    this.vmData = config.vmData;
    this.labels = config.labels;
    this.messages = config.messages;
    this.constants = config.constants;
    this.models = config.models;
    this.generalUrls = config.generalUrls;
}
ConfiguracionEficienciaCaladoVM.prototype = {
    onReady: function () {
        let self = this;
        self.vm = {
            mainModule: {
                selectors: {
                    horaDesde: $('#' + self.generalIds.horarioEntrada),
                    horaHasta: $('#' + self.generalIds.horarioSalida),
                },
                states: {},
                actions: {
                    actualizarHorarioTurno: function () {
                        event.preventDefault();
                        var request = {
                            desde: self.vm.mainModule.selectors.horaDesde.val(),
                            hasta: self.vm.mainModule.selectors.horaHasta.val()
                        }

                        $.ajax({
                            type: 'POST',
                            dataType: "json",
                            url: self.generalUrls.actualizarHorario,
                            data: request,
                            success: function (data) {
                                if (!data.HayErrores) {
                                } else {
                                }
                            },
                            error: function (error) {
                            },
                        }).always(function () {
                        });
                    },
                }, //Eventos que hacen feedback
                methods: {}, //Metodos internos
                models: {}, //Crear modelo
                events: {
                    setearIdCallesDropdown: function () {
                        let nombreCalle = $('#' + self.generalIds.listaCalles + ' option:selected').text();
                        $('#' + self.generalIds.nombreInputCalleEficiencia).val(nombreCalle);
                    }
                }, //Eventos que no hacen feedback
                validations: {}, //Validaciones
            },
        };

        self.init();
    },
    init: function () {
        let self = this;
        $("body").on("change", $('#' + self.generalIds.listaCalles), function () {
            self.vm.mainModule.events.setearIdCallesDropdown();
        });
    }
}