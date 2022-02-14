function MonitorCPECacheadaVM(config) {
    this.containerId = config.containerId;
    this.generalIds = config.generalIds;
    this.vmData = config.vmData;
    this.labels = config.labels;
    this.messages = config.messages;
    this.constants = config.constants;
    this.models = config.models;
    this.generalUrls = config.generalUrls;
    this.aucDataSourceUrls = config.aucDataSourceUrls;
}
MonitorCPECacheadaVM.prototype = {
    onReady: function () {
        let self = this;
        self.vm = {
            mainModule: {
                selectors: {
                    gridInterval: null,
                },
                states: {},
                actions: {
                    procesarCPEJob: function () {
                        let confirmAction = confirm(self.messages.confirmarEjecucion);
                        if (confirmAction) {
                            self.vm.mainModule.methods.procesarCPEJob();
                        }
                    }
                },
                methods: {
                    refrescarGrilla: function () {
                        var container = $("#" + self.generalIds.gridContainer);
                        var url = container.data().gridUrl;

                        url = UpdateQueryString("FechaCPEDesde", $("[name='FechaCPEDesde']").val(), url);
                        url = UpdateQueryString("FechaCPEHasta", $("[name='FechaCPEHasta']").val(), url);
                        url = UpdateQueryString("CTG", $("[name='CTG']").val(), url);
                        url = UpdateQueryString("MaterialId", $("[name='MaterialId']").val(), url);
                        url = UpdateQueryString("Patente", $("[name='Patente']").val(), url);
                        url = UpdateQueryString("VerCamionesPorLlegar", $("[name='VerCamionesPorLlegar']").val(), url);

                        $.get(url, function (data) {
                            container.html(data);
                        });
                    },
                    procesarCPESeleccionado: function (id) {
                        var request = {
                            id: id
                        }

                        $.ajax({
                            type: 'POST',
                            dataType: "json",
                            url: self.generalUrls.procesarCacheadoUrl,
                            data: request,
                            success: function (data) {
                                self.vm.mainModule.methods.refrescarGrilla();
                            },
                            error: function (error) {
                            },
                        }).always(function () {
                        });
                    },
                    procesarCPEJob: function () {

                        $.ajax({
                            type: 'POST',
                            dataType: "json",
                            url: self.generalUrls.procesarCacheadoJobUrl,
                            success: function (data) {
                                self.vm.mainModule.methods.refrescarGrilla();
                            },
                            error: function (error) {
                            },
                        }).always(function () {
                        });

                    }
                },
                models: {},
                events: {
                    activarRecargaAutomatica: function (e) {
                        var currentModule = self.vm.mainModule;
                        let checked = $(e).is(':checked');
                        if (checked) {
                            currentModule.selectors.gridInterval = setInterval(function () {
                                currentModule.methods.refrescarGrilla();
                            }, 30000);
                        }
                        else {
                            clearInterval(currentModule.selectors.gridInterval);
                        }
                    },
                    checkVerCamionesPorLlegar: function (e) {
                        let checked = $(e).is(':checked');
                        if (checked) {
                            $(e).val(true);
                        }
                        else {
                            $(e).val(false);
                        }
                    }
                },
                validations: {},
            },
        };

        self.init();
    },
    init: function () {
        let self = this;
        $(document).on('click', "." + self.generalIds.procesarBtnGridClass, (function () {
            let confirmAction = confirm(self.messages.confirmarEjecucion);
            if (confirmAction) {
                let id = $(this).data("botonCerearId");
                self.vm.mainModule.methods.procesarCPESeleccionado(id);
            }
        }));
    }
}