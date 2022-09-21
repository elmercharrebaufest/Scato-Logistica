$(document).ready(function () {
    $('#documentos').val(null);

    var model = new UploadCpsViewModel('DocumentoExterno/GuardarCartasDePorte', 'DocumentoExterno/ObtenerNrosCP');
    ko.applyBindings(model, $('#cpsContainer')[0]);

    const fileInput = document.querySelector('#file-js-example input[type=file]');
    $("#file-js-example").change(function () {
        $("#cpsContainer").hide();

        if (parseInt(fileInput.files.length) > 200) {
            MostrarAlertaError("Solo puede subir un máximo de 200 archivos");
            return false;
        } 

        if (fileInput.files.length > 0) {
            const fileName = document.querySelector('#file-js-example .file-name');
            fileName.textContent = fileInput.files.length + " archivos seleccionados";
            model.obtenerNrosCps(fileInput.files);
        }
    });

    $("#file-js-example").click(function () {
        $("#form-documento-externo").trigger("reset");
    });

    $('#dialogo-cp').on('hidden', function () {
        ko.cleanNode($('#cp-nro')[0]);
    });

    $('#dialogo-cps-errores').on('hidden', function () {
        $("#cpsContainer").hide();
        document.querySelector('#file-js-example .file-name').textContent = "Ningún Archivo Seleccionado";
    });
    

    $("#cpsContainer").hide();
});

function UploadCpsViewModel(urlUpload, urlGetNrosCp) {
    var self = this;
    self.urlUpload = urlUpload;
    self.urlGetNrosCp = urlGetNrosCp;
    self.cps = ko.observableArray([]); //todos los cargados
    self.showFallidos = ko.observable(false);
    self.showCorrectos = ko.observable(false);
    self.logErrores = ko.observableArray([]);
    self.useFilename = ko.observable(false);
    self.useFilename.subscribe(function () {
        self.loadCpsFromFilename();
    });
    self.imagesLoaded = ko.observable(0);
    self.imagesLoaded.subscribe(function () {
        if (self.imagesLoaded() == self.cps().length) {
            self.showFallidos(true);
            self.showCorrectos(false);
            $("#cpsContainer").show();
        }
    });
    self.cantFallidos = ko.computed(function () {
        var count = 0;
        $.each(self.cps(), function (i, cp) {
            count += cp().nroCp().length == 0 ? 1 : 0;
        });

        return count;
    }, self);

    self.cantCorrectos = ko.computed(function () {
        var count = 0;
        $.each(self.cps(), function (i, cp) {
            count += cp().nroCp().length > 0 ? 1 : 0;
        });

        return count;
    }, self);

    self.cpsShow = ko.computed(function () {
        var cps = [];

        $.each(self.cps(), function (i, cp) {
            if ((self.showFallidos() && cp().nroCp().length == 0) || (self.showCorrectos() && cp().nroCp().length > 0))
                cps.push(cp());
        });

        return cps;
    }, self);

    self.obtenerNrosCps = function(imagesToUpload){
        self.cps([]);
        self.logErrores([]);
        self.imagesLoaded(0);
        self.showFallidos(false);
        self.showCorrectos(false);
        self.useFilename(false);

        var cp11 = /^\d{11}$/;
        var cp12 = /^\d{12}$/;

        $.each(imagesToUpload, function (index, image) {
           var nombre = image.name.split('.')[0];
           var nroCp = cp11.test(nombre) || cp12.test(nombre) ? nombre : "";
           self.cps.push(ko.observable({ filename: image.name, nroCp: ko.observable(""), imageRaw: image, imageLoaded: ko.observable(false), nroCpArchivo: nroCp, image: null }));
        });

        self.uploadImages();
    }

    self.uploadImages = function () {
        var formData = new FormData();
        $.each(self.cps(), function (i, cp) {
            formData.append(cp().filename, cp().imageRaw);
        });

        BlockUI();
        $.ajax({
            url: self.urlGetNrosCp,
            type: "POST",
            contentType: false,
            processData: false,
            data: formData,
            success: function (result) {
                if (result && result.CartasDePorte && result.CartasDePorte.length > 0) {
                    $.each(result.CartasDePorte, function (i, c) {
                        var match = ko.utils.arrayFirst(self.cps(), function (item) {
                            return c.nombreArchivo === item().filename;
                        });
                        var cp = match();

                        var loadImage = function () {
                            var reader = new FileReader();
                            reader.onload = function (e) {
                                if (cp.imageRaw.type == 'image/jpeg' || cp.imageRaw.type == 'image/png') {
                                    cp.image = e.target.result;
                                } else if (cp.imageRaw.type == 'image/tiff') {
                                    cp.image = null;
                                    var tiff = new Tiff({ buffer: e.target.result });
                                    cp.image = tiff.toDataURL();
                                }

                                if (!self.useFilename() && !cp.nroCp())
                                    cp.nroCp(c.nroCp || "");

                                cp.imageLoaded(true);
                                self.imagesLoaded(self.imagesLoaded() + 1);
                            }

                            if (cp.imageRaw.type == 'image/jpeg' || cp.imageRaw.type == 'image/png') {
                                reader.readAsDataURL(cp.imageRaw);
                            } else if (cp.imageRaw.type == 'image/tiff') {
                                reader.readAsArrayBuffer(cp.imageRaw);
                            }
                        }

                        loadImage();
                    });
                    self.loadCpsFromFilename();
                }

                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
                MostrarAlertaError("Hubo un error al obtener los números de cp. Por favor intentelo nuevamente.");
            }
        });
    }

    self.showPopup = function (item) {
        ko.applyBindings(item, $('#dialogo-cp')[0]);
        $('#dialogo-cp').modal('show');
    }

    self.quitarCp = function (item) {
        self.cps.remove(function (cp) {
            return cp().filename == item.filename;
        });
    }

    self.loadCpsFromFilename = function () {
        if (self.useFilename()) {
            $.each(self.cps(), function (i, c) {
                var cp = c();

                if (!cp.nroCp())
                    cp.nroCp(cp.nroCpArchivo);
            });
        } else {
            $.each(self.cps(), function (i, c) {
                var cp = c();

                if (cp.nroCp() && cp.nroCp() == cp.nroCpArchivo)
                    cp.nroCp('');
            });
        }
    }

    self.guardarCps = function () {
        var formData = new FormData();
        var cps = [];

        $.each(self.cps(), function (i, c) {
            var cp = c();

            if (cp.nroCp()) {
                cps.push(
                    {
                        NombreArchivoOriginal: cp.filename,
                        NumeroDeDocumento: cp.nroCp()
                    });

                formData.append(cp.filename, cp.imageRaw);
            }
        });

        if (cps.length == 0) {
            MostrarAlertaError("No hay cartas de porte correctas para guardar.");
        } else {
            formData.append("cartasDePorte", JSON.stringify(cps));

            BlockUI();
            $.ajax({
                url: self.urlUpload,
                type: "POST",
                contentType: false,
                processData: false,
                data: formData,
                success: function (result) {
                    if (result) {
                        if (result.Errores && result.Errores.length > 0) {
                            $.each(result.Errores, function (i, v) {
                                self.logErrores.push(v);
                            });

                            var item = { logErrores: self.logErrores };
                            ko.applyBindings(item, $('#dialogo-cps-errores')[0]);
                            $('#dialogo-cps-errores').modal('show');
                        } else {
                            $("#cpsContainer").hide();
                            document.querySelector('#file-js-example .file-name').textContent = "Ningún Archivo Seleccionado";
                            MostrarAlertaExitosa("Se cargaron correctamente las cartas de porte");
                        }
                    }

                    $.unblockUI();
                    $("#search-form").submit();

                },
                error: function (err) {
                    $.unblockUI();
                    MostrarAlertaError("Hubo un error al obtener los números de cp. Por favor intentelo nuevamente.");
                }
            });
        }
    }
}


