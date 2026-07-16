const CargaDeCupoApp = {
    init: {
        all: () => {
            try {
                BlockUI();
                console.log('Iniciando aplicación Carga de Cupo...');

                CargaDeCupoApp.events.setupFormEvents();
                CargaDeCupoApp.events.setupCheckEvents();
                CargaDeCupoApp.events.setupButtonEvents();
                CargaDeCupoApp.events.setupTextFieldEvents();
                CargaDeCupoApp.events.setupSelectEvents();
                CargaDeCupoApp.events.setupCupoMask();

                CargaDeCupoApp.init.signalR();
                CargaDeCupoApp.init.puestoDeTrabajo();
                CargaDeCupoApp.init.ui();

                CargaDeCupoApp.polling.startListarCupos();
                CargaDeCupoApp.polling.startPatenteImageRefresh();

                console.log('Aplicación Carga de Cupo iniciada correctamente');
            } catch (error) {
                console.error('Error inicializando aplicación:', error);
                MostrarAlertaError('Error al inicializar la aplicación');
            } finally {
                $.unblockUI();
            }
        },

        signalR: async () => {
            if (!$.connection) {
                console.warn('SignalR no está disponible');
                return;
            }

            try {
                CargaDeCupoApp.signalR.config.notificaLectura = $.connection.notificaLectura;

                CargaDeCupoApp.signalR.config.notificaLectura.client.informarLectura = CargaDeCupoApp.signalR.handlers.informarLectura;
                CargaDeCupoApp.signalR.config.notificaLectura.client.informarEstadoConexion = CargaDeCupoApp.signalR.handlers.informarEstadoConexion;
                CargaDeCupoApp.signalR.config.notificaLectura.client.informarLecturaCpe = CargaDeCupoApp.signalR.handlers.informarLecturaCpe;

                if (window.hubReady) {
                    await window.hubReady;

                    const puestoDeTrabajo = CargaDeCupoCore.state.puestoDeTrabajo;
                    if (puestoDeTrabajo?.Automatico && CargaDeCupoApp.signalR.config.notificaLectura?.server) {
                        const centroId = CargaDeCupoUI.dom.getValue('#centroId');
                        await CargaDeCupoApp.signalR.config.notificaLectura.server.escucharPuestosDeTrabajo(centroId, puestoDeTrabajo.Id);
                    }
                } else {
                    console.warn('window.hubReady no está disponible - funcionando sin SignalR en tiempo real');
                    window.location.href = window.location.href;
                }
            } catch (error) {
                console.error('Error configurando SignalR:', error);
                window.location.href = window.location.href;
            }
        },

        puestoDeTrabajo: () => {
            try {
                CargaDeCupoCore.state.puestoDeTrabajo = JSON.parse(CargaDeCupoUI.dom.getValue('#puestoDeTrabajo'));

                CargaDeCupoUI.dom.setValue('#ImprimeCartaPorte', CargaDeCupoCore.state.puestoDeTrabajo.ImprimeCartaPorte);
                CargaDeCupoUI.dom.setValue('#ImprimeTarjetaDeAcceso', CargaDeCupoCore.state.puestoDeTrabajo.ImprimeTarjetaDeAcceso);
                CargaDeCupoUI.dom.setValue('#NoAsignaCalleEnGaritaEntrada', CargaDeCupoCore.state.puestoDeTrabajo.NoAsignaCalleEnGaritaEntrada);
                CargaDeCupoUI.dom.setValue('#SinFotoCartaPorte', CargaDeCupoCore.state.puestoDeTrabajo.SinFotoCartaPorte);
                CargaDeCupoUI.dom.setValue('#PuestoDeTrabajoId', CargaDeCupoCore.state.puestoDeTrabajo.Id);

                if (CargaDeCupoCore.state.puestoDeTrabajo.SinCupo) {
                    CargaDeCupoUI.dom.setChecked('#checkSinCupo', true);
                } else {
                    CargaDeCupoUI.dom.setChecked('#checkSinCupo', false);
                }

                if (CargaDeCupoCore.state.puestoDeTrabajo.SinFotoCartaPorte) {
                    CargaDeCupoUI.dom.getElement('.divFotoCP').classList.add('hide');
                }
            } catch (error) {
                console.error('Error parseando datos del puesto de trabajo:', error);
            }
        },

        ui: () => {
            const puesto = CargaDeCupoCore.state.puestoDeTrabajo;
            if (!puesto?.Automatico) {
                const numeroInput = CargaDeCupoUI.dom.getElement('#Numero');
                if (numeroInput) {
                    numeroInput.removeAttribute('readonly');
                    numeroInput.placeholder = '';
                }

                CargaDeCupoUI.dom.addClass('#labelConectado', 'hidden');
                CargaDeCupoUI.dom.addClass('#labelDesconectado', 'hidden');
            }

            CargaDeCupoUI.dom.setChecked('#circuitoNoGranos', false);
            CargaDeCupoApp.events.handlers.circuitoChange();
        },
    },

    events: {
        setupCupoMask: () => {
            const cupoInput = CargaDeCupoUI.dom.getElement('#Cupo');
            $(cupoInput).inputmask("MOL9999/99999999", {
                "placeholder": "MOL____/" + CargaDeCupoCore.date.getCurrentDate(),
                onKeyDown: function (event, buffer, caretPos, opts) {
                    const currentValue = $(this).val();

                    setTimeout(() => {
                        const newValue = $(this).val();
                        if (newValue !== currentValue && newValue.length > 0 && CargaDeCupoCore.validation.isValidCupoFormat(newValue)) {
                            CargaDeCupoUI.dom.setCheckedEvent('#checkSinCupo', false);
                            CargaDeCupoApp.business.validateCupoSap();
                        }
                    }, 100);
                }
            });
        },

        setupFormEvents: () => {
            const btnAceptar = CargaDeCupoUI.dom.getElement('#btnAceptar');
            btnAceptar.addEventListener('click', async (event) => {
                event.preventDefault();
                await CargaDeCupoApp.events.handlers.handleSubmit();
            });

            const form = CargaDeCupoUI.dom.getElement('#formCargaDeCupo');
            form.addEventListener('keydown', async (event) => {
                if (event.key !== 'Enter') {
                    return;
                }

                const target = event.target;
                const tagName = target && target.tagName ? target.tagName.toLowerCase() : '';
                if (tagName === 'textarea') {
                    return;
                }

                event.preventDefault();
                event.stopPropagation();
                await CargaDeCupoApp.events.handlers.handleSubmit();
            }, true);

            form.addEventListener('submit', async (event) => {
                event.preventDefault();
                event.stopPropagation();
                await CargaDeCupoApp.events.handlers.handleSubmit();
            });
        },

        setupCheckEvents: () => {
            const circuitoCheckbox = CargaDeCupoUI.dom.getElement('#circuitoNoGranos');
            circuitoCheckbox.addEventListener('change', CargaDeCupoApp.events.handlers.circuitoChange);

            const checkValidarPatente = CargaDeCupoUI.dom.getElement('#checkvalidarPatente');
            checkValidarPatente.addEventListener('change', CargaDeCupoApp.events.handlers.checkValidarPatente);

            const checkSinCupo = CargaDeCupoUI.dom.getElement('#checkSinCupo');
            checkSinCupo.addEventListener('change', CargaDeCupoApp.events.handlers.checkSinCupo);
        },

        setupTextFieldEvents: () => {
            const ctgField = CargaDeCupoUI.dom.getElement('#CTG');
            ctgField.addEventListener('change', CargaDeCupoApp.events.handlers.ctgChange);

            const patenteField = CargaDeCupoUI.dom.getElement('#Patente');
            patenteField.addEventListener('change', CargaDeCupoApp.events.handlers.patenteChange);
        },

        setupButtonEvents: () => {
            const tomarFotoBtn = CargaDeCupoUI.dom.getElement('.tomarFoto1');
            tomarFotoBtn.addEventListener('click', CargaDeCupoApp.events.handlers.tomarFoto);

            const btnActualizarCpe = CargaDeCupoUI.dom.getElement('#btnActualizarCpe');
            if (btnActualizarCpe) {
                btnActualizarCpe.addEventListener('click', CargaDeCupoApp.events.handlers.actualizarCpe);
            }

            const closeButtons = [
                { id: '#validation-alert-cargaDeCupo-close', target: '#validation-alert-cargaDeCupo' },
                { id: '#validation-error-close', target: '#validation-error-alert' },
                { id: '#validation-advertencia-close', target: '#validation-advertencia-alert' },
                { id: '#validation-informativo-close', target: '#validation-informativo-alert' },
                { id: '#validation-tarjeta-close', target: '#validation-tarjeta-alert' },
                { id: '#validation-danger-close', target: '#validation-patente-danger' },
                { id: '#validation-ctg-close', target: '#validation-ctg-alert' }
            ];
            closeButtons.forEach(({ id, target }) => {
                const button = CargaDeCupoUI.dom.getElement(id);
                const targetElement = CargaDeCupoUI.dom.getElement(target);

                if (button && targetElement) {
                    button.addEventListener('click', () => {
                        targetElement.classList.add('hide');
                    });
                }
            });
        },

        setupSelectEvents: () => {
            const materialField = CargaDeCupoUI.dom.getElement('#MaterialId');
            materialField.addEventListener('change', CargaDeCupoApp.events.handlers.materialChange);
        },

        handlers: {
            handleSubmit: async () => {
                const btnAceptar = CargaDeCupoUI.dom.getElement('#btnAceptar');
                if (btnAceptar.disabled) return;

                const form = CargaDeCupoUI.dom.getElement('#formCargaDeCupo');
                const $form = $(form);
                if (!$form.valid()) return;

                const materialValido = CargaDeCupoApp.validations.validateMaterialSelection();
                if (!materialValido) return;

                if (CargaDeCupoCore.state.errorPatente) {
                    const mensaje = CargaDeCupoUI.dom.isChecked('#circuitoNoGranos')
                        ? 'La patente leída en la imagen no coincide con la indicada en el formulario'
                        : 'La patente leída en la imagen no coincide con la obtenida de AFIP';

                    CargaDeCupoUI.dialogs.showConfirmationModal(
                        `${mensaje}, ¿desea confirmarlo de todas formas?`,
                        () => {
                            CargaDeCupoCore.state.errorPatente = false;
                            CargaDeCupoApp.business.submitForm();
                        }
                    );
                    return;
                }

                await CargaDeCupoApp.business.submitForm();
            },

            circuitoChange: async (event) => {
                if (CargaDeCupoUI.dom.isChecked('#circuitoNoGranos')) {
                    CargaDeCupoUI.dom.setValue('#circuitoNoGranos', true)
                    CargaDeCupoUI.config.applyNoGranosConfig();
                } else {
                    CargaDeCupoUI.dom.setValue('#circuitoNoGranos', false)
                    CargaDeCupoUI.config.applyGranosConfig();
                    await CargaDeCupoApp.business.loadMateriales(true);
                }
            },

            checkValidarPatente: (event) => {
                const validarPatente = CargaDeCupoUI.dom.isChecked('#checkvalidarPatente');
                CargaDeCupoUI.dom.setReadonly('#Patente', validarPatente);
                CargaDeCupoUI.dom.setReadonly('#CTG', validarPatente);
                if (!validarPatente) {
                    CargaDeCupoUI.dom.getElement('#Patente').focus();
                }
            },

            checkSinCupo: (event) => {
                if (CargaDeCupoUI.dom.isChecked('#checkSinCupo')) {
                    CargaDeCupoUI.dom.setValue('#checkSinCupo', true);
                    CargaDeCupoUI.dom.setValue('#Cupo', CargaDeCupoCore.CONSTANTS.CUPO_GENERICO);
                } else {
                    CargaDeCupoUI.dom.setValue('#checkSinCupo', false);
                    CargaDeCupoUI.dom.setValue('#Cupo', '');
                }
            },

            ctgChange: async (event) => {
                const nroCTG = CargaDeCupoUI.dom.getValue('#CTG');
                const tarjeta = CargaDeCupoUI.dom.getValue('#Numero');
                const esEspecial = CargaDeCupoUI.dom.getValue('#Especial');

                if (CargaDeCupoCore.validation.isValidCtgLength(nroCTG)) {
                    await CargaDeCupoApp.business.handleCtg(nroCTG, tarjeta, esEspecial);
                }
            },

            actualizarCpe: async (event) => {
                event.preventDefault();

                const nroCTG = CargaDeCupoUI.dom.getValue('#CTG');
                const tarjeta = CargaDeCupoUI.dom.getValue('#Numero');
                const esEspecial = CargaDeCupoUI.dom.getValue('#Especial');

                if (!CargaDeCupoCore.validation.isValidCtgLength(nroCTG)) {
                    CargaDeCupoUI.alerts.showAlert('Debe ingresar un CTG válido para actualizar la CPE', 'alert-error');
                    return;
                }

                await CargaDeCupoApp.business.handleCtg(nroCTG, tarjeta, esEspecial, true);
            },

            patenteChange: async (event) => {
                const patente = CargaDeCupoUI.dom.getValue('#Patente').trim().toUpperCase();
                const noGranosActivado = CargaDeCupoUI.dom.isChecked('#circuitoNoGranos');
                if (!noGranosActivado && patente) {
                    const tarjeta = CargaDeCupoUI.dom.getValue('#Numero');
                    const esEspecial = CargaDeCupoUI.dom.getValue('#Especial');
                    await CargaDeCupoApp.business.handlePatenteGranos(patente, tarjeta, esEspecial, null);
                } else if (noGranosActivado && patente) {
                    await CargaDeCupoApp.business.handlePatenteNoGranos(patente);
                }
            },

            materialChange: async (event) => {
                const noGranosActivado = CargaDeCupoUI.dom.isChecked('#circuitoNoGranos');
                if (noGranosActivado) {
                    CargaDeCupoUI.combo.setTipoOrdenCargaNoGranos();
                } else {
                    const patente = CargaDeCupoUI.dom.getValue('#Patente').trim().toUpperCase();
                    const materialId = CargaDeCupoUI.dom.getValue('#MaterialId');

                    if (patente && materialId) {
                        const tarjeta = CargaDeCupoUI.dom.getValue('#Numero');
                        const esEspecial = CargaDeCupoUI.dom.getValue('#Especial');
                        await CargaDeCupoApp.business.handlePatenteGranos(patente, tarjeta, esEspecial, materialId);
                    }
                }
            },

            tomarFoto: (event) => {
                event.preventDefault();
                CargaDeCupoApp.business.takeCartaPortePhoto();
            }
        }
    },

    validations: {
        validatePatenteWithImage: () => {
            CargaDeCupoCore.state.errorPatente = false;



            const patenteALPR = CargaDeCupoUI.dom.getElement('#patenteALPR')?.textContent || '';
            const patenteValue = CargaDeCupoUI.dom.getValue('#Patente');
            const isCircuitoNoGranos = CargaDeCupoUI.dom.isChecked('#circuitoNoGranos');

            if (patenteALPR !== CargaDeCupoCore.CONSTANTS.PATENTE_NO_RECONOCIDA &&
                patenteALPR !== '' &&
                patenteValue !== '' &&
                patenteALPR.toUpperCase() !== patenteValue.toUpperCase()) {
                const mensaje = isCircuitoNoGranos
                    ? `La patente reconocida en la imagen (${patenteALPR.toUpperCase()}) no coincide con : (${patenteValue.toUpperCase()})`
                    : `La patente reconocida en la imagen (${patenteALPR.toUpperCase()}) no coincide con la obtenida de AFIP (${patenteValue.toUpperCase()})`;

                CargaDeCupoUI.alerts.showAlertPatente(mensaje);
                CargaDeCupoCore.state.errorPatente = true;
            } else if ((patenteALPR === CargaDeCupoCore.CONSTANTS.PATENTE_NO_RECONOCIDA || patenteALPR === '') && patenteValue !== '') {
                CargaDeCupoUI.alerts.showAlertPatente('No se pudo reconocer la patente del vehículo en la imagen, debe validarla manualmente');
                CargaDeCupoCore.state.errorPatente = true;
            } else {
                CargaDeCupoUI.alerts.hideAlertPatente();
            }
        },

        validateMaterialSelection: () => {
            const materialId = CargaDeCupoUI.dom.getValue('#MaterialId');
            const isNoGranos = CargaDeCupoUI.dom.isChecked('#circuitoNoGranos');
            if (!isNoGranos) {
                if (materialId) {
                    CargaDeCupoUI.validation.clearFieldError('MaterialId');
                    return true;
                } else {
                    CargaDeCupoUI.validation.showFieldError('MaterialId', 'Debe seleccionar un Material');
                    return false;
                }
            }
            else {
                const hayVariosMateriales = CargaDeCupoUI.dom.getValue('#HayVariosMateriales');
                if (hayVariosMateriales === 'True' && materialId === '') {
                    CargaDeCupoUI.validation.showFieldError('MaterialId', 'Selecciona un Material');
                    return false;
                } else {
                    CargaDeCupoUI.validation.clearFieldError('MaterialId');
                    return true;
                }
            }
        }
    },

    processors: {
        responseCupoSap: (data) => {
            if (data.error) {
                return {
                    type: 'error',
                    message: data.error,
                    focusElement: '#Cupo'
                };
            }

            let especial = '';
            if (data.model.Especial && data.model.MaterialId === 4) {
                especial = ' Sustentable';
            } else if (data.model.Especial) {
                especial = ' Especial';
            }

            const mensaje = `<h4><strong>${data.model.RespuestaSap}</strong></h4>  Fecha: <strong>${data.model.FechaSap}</strong>  Material: <strong>${data.model.MaterialDescripcion}${especial}</strong>  Proveedor: <strong>${data.model.ProveedorDescripcion}(${data.model.ProveedorCuit})</strong>`;

            return {
                type: 'success',
                message: mensaje,
                respuestaSap: data.model.RespuestaSap,
                fieldsToUpdate: {
                    '#MaterialId': data.model.MaterialId,
                    '#FechaSap': data.model.FechaSap,
                    '#Especial': data.model.Especial,
                    '#RespuestaSap': data.model.RespuestaSap,
                    '#Camara': data.model.Camara
                },
                imageData: data.model.Especial && data.model.MaterialId === 4 ? {
                    type: 'sustentable',
                    imagen: data.PdfImageSustentableBase64
                } : null
            };
        },

        responseFasonInsumos: (data) => {
            if (data.HayErrores) {
                if (data.Errores["ClienteDuplicado"]) {
                    return {
                        type: 'modal_error',
                        message: data.Errores["ClienteDuplicado"]
                    };
                } else {
                    return {
                        type: 'error',
                        message: data.Errores["Error"]
                    };
                }
            }

            return {
                type: 'success',
                ordenes: data.Ordenes
            };
        }
    },

    images: {
        setPatenteImage: (error, imagen, patente = null) => {
            if (CargaDeCupoUI.dom.isChecked('#checkvalidarPatente')) {
                const imageElement = CargaDeCupoUI.dom.getElement('#imagen-patente');
                const patenteDisplay = CargaDeCupoUI.dom.getElement('#patenteALPR');
                const patenteNormalizada = patente?.toUpperCase();

                if (!error) {
                    if (patenteNormalizada && patenteNormalizada !== 'NULL') {
                        patenteDisplay.textContent = patenteNormalizada;

                        const patenteField = CargaDeCupoUI.dom.getElement('#Patente');
                        const patenteGuardada = CargaDeCupoCore.state.patenteGuardada;
                        const currentPatente = (CargaDeCupoUI.dom.getValue('#Patente') || '').toUpperCase();
                        const patenteSetByCamara = (CargaDeCupoCore.state.patenteSetByCamara || '').toUpperCase();
                        // La cámara solo actualiza #Patente si: el campo está vacío,
                        // o si el valor actual fue el que la cámara misma puso (el operador no lo modificó a mano).
                        const camaraEsFuenteActual = !currentPatente || currentPatente === patenteSetByCamara;
                        if (patenteField && patenteNormalizada !== patenteGuardada && camaraEsFuenteActual && currentPatente !== patenteNormalizada) {
                            CargaDeCupoCore.state.patenteSetByCamara = patenteNormalizada;
                            CargaDeCupoUI.dom.setValue('#Patente', patenteNormalizada);
                            patenteField.dispatchEvent(new Event('change', { bubbles: true }));
                        }
                    } else {
                        patenteDisplay.textContent = CargaDeCupoCore.CONSTANTS.PATENTE_NO_RECONOCIDA;
                        CargaDeCupoUI.dom.setValue('#Patente', '');
                    }
                    imageElement.src = imagen || '';
                    imageElement.alt = 'Imagen de patente';
                } else {
                    patenteDisplay.textContent = '';
                    imageElement.src = '';
                    imageElement.alt = 'Error al obtener la imagen';
                }

                CargaDeCupoApp.validations.validatePatenteWithImage();
            }
        },

        setCartaPorteImage: (error, imagen, directorio, esSustentable = false) => {
            const imageElement = CargaDeCupoUI.dom.getElement('#imagen-cp');
            if (error) {
                imageElement.alt = 'Error al obtener la imagen';
                imageElement.src = '';

                const tomarFotoBtn = CargaDeCupoUI.dom.getElement('.tomarFoto1');
                tomarFotoBtn.style.display = 'block';

                return;
            }

            if (esSustentable) {
                CargaDeCupoUI.dom.setValue('#ImagenCartaPorteSustentable', imagen);
            } else {
                CargaDeCupoUI.dom.setValue('#ImagenCartaPorte', imagen);
            }
            CargaDeCupoUI.dom.setValue('#FotoRutaDestino', directorio);

            imageElement.onload = () => {
                if ($(imageElement).elevateZoom) {
                    $(imageElement).elevateZoom({
                        zoomType: "inner",
                        cursor: "crosshair"
                    });
                }
            };

            imageElement.src = imagen;
            imageElement.alt = 'Cargando...';
        }
    },

    business: {
        validateCupoSap: async () => {
            try {
                BlockUI('Cupo');

                const cupo = CargaDeCupoUI.dom.getValue('#Cupo');
                const imagen = CargaDeCupoUI.dom.getValue('#ImagenCartaPorte');
                const nroCartaPorte = CargaDeCupoUI.dom.getValue('#CTG');

                const apiResponse = await CargaDeCupoAPI.services.validarCupoSap(cupo, imagen, nroCartaPorte);
                const processedData = CargaDeCupoApp.processors.responseCupoSap(apiResponse);

                if (processedData.type === 'error') {
                    CargaDeCupoUI.alerts.showAlert(processedData.message, 'alert-error');
                    if (processedData.focusElement) {
                        CargaDeCupoUI.dom.getElement(processedData.focusElement)?.focus();
                    }
                } else if (processedData.type === 'success') {
                    const alertType = processedData.respuestaSap === 'Cupo del día' ? 'alert-success'
                        : processedData.respuestaSap === 'Cupo vencido' ? 'alert-block'
                            : processedData.respuestaSap === 'Cupo futuro' ? 'alert-info'
                                : 'alert-success';
                    CargaDeCupoUI.alerts.showAlert(processedData.message, alertType);

                    Object.entries(processedData.fieldsToUpdate).forEach(([selector, value]) => {
                        CargaDeCupoUI.dom.setValue(selector, value);
                    });

                    if (processedData.imageData) {
                        const directorio = CargaDeCupoUI.dom.getValue('#CodigoCamaraCPDir');
                        CargaDeCupoApp.images.setCartaPorteImage(
                            processedData.imageData.imagen ? '' : 'error',
                            processedData.imageData.imagen,
                            directorio,
                            processedData.imageData.type === 'sustentable'
                        );
                    }
                }
            } catch (error) {
                CargaDeCupoUI.alerts.showAlert('Error al validar el cupo', 'alert-error');
            } finally {
                $.unblockUI();
            }
        },

        loadMateriales: async (esGrano) => {
            try {
                BlockUI('Cargando Material...');
                const materiales = await CargaDeCupoAPI.services.obtenerMateriales(esGrano);
                CargaDeCupoUI.combo.populateMateriales(materiales);
                return materiales;
            } catch (error) {
                MostrarAlertaError('Error al cargar materiales');
            } finally {
                $.unblockUI();
            }
        },

        handleCtg: async (nroCTG, tarjeta, esEspecial, forzarActualizacion = false) => {
            try {
                BlockUI(forzarActualizacion ? 'Actualizando CPE desde AFIP...' : undefined);

                CargaDeCupoUI.form.clearValidation();
                CargaDeCupoUI.alerts.hideAlert();

                const data = await CargaDeCupoAPI.services.obtenerCPE(nroCTG, tarjeta, esEspecial, forzarActualizacion);

                if (data.CodigoDeError === "1") {
                    CargaDeCupoUI.alerts.showAlert(data.Error, 'alert-info');
                } else if (data.CodigoDeError === "3" || data.CodigoDeError === "4") {
                    if (!data.Cpe) {
                        CargaDeCupoUI.alerts.showAlert('No se recibieron datos del CPE', 'alert-error');
                        return;
                    }

                    CargaDeCupoUI.dom.setValue('#MaterialId', data.Cpe.MaterialId);
                    CargaDeCupoUI.dom.setValue('#RtteComercialCodigoSap', data.Cpe.RtteComercialCodigoSap);
                    CargaDeCupoUI.dom.setValue('#TitularCartaPorteCodigoSap', data.Cpe.TitularCartaPorteCodigoSap);
                    CargaDeCupoUI.dom.setValue('#CodEstab', data.Cpe.CodEstab);
                    CargaDeCupoUI.dom.setValue('#CodigoRENSPA', data.Cpe.CodigoRENSPA);
                    CargaDeCupoUI.dom.setValue('#Cosecha', data.Cpe.Cosecha);
                    if (data.Cpe.Vehiculos?.length > 0) {
                        CargaDeCupoUI.dom.setValue('#PesoNetoOrigen', data.Cpe.Vehiculos[0]["PesoNetoOrigen"]);
                    }
                    CargaDeCupoUI.dom.setValue('#RtteComercialVentaSecundariaCuit', data.Cpe.RtteComercialVentaSecundarioCuil);

                    if (data.Cpe.Vehiculos && data.Cpe.Vehiculos.length > 0) {
                        CargaDeCupoUI.dom.setValue('#Patente', data.Cpe.Vehiculos[0]["Patente"]);

                        if (data.Cpe.Vehiculos[0]["PatenteAcoplado"]) {
                            CargaDeCupoUI.dom.setValue('#PatenteAcoplado', data.Cpe.Vehiculos[0]["PatenteAcoplado"]);
                        } else {
                            CargaDeCupoUI.alerts.showAlert('Vehiculo sin acoplado', 'alert-block');
                        }
                    }

                    if (data.Cpe.Cupo) {
                        const cupoFormateado = CargaDeCupoCore.formatting.truncateCupoLength(data.Cpe.Cupo);
                        CargaDeCupoUI.dom.setCheckedEvent('#checkSinCupo', false);
                        CargaDeCupoUI.dom.setValue('#Cupo', cupoFormateado);
                        await CargaDeCupoApp.business.validateCupoSap();
                    } else {
                        CargaDeCupoUI.dom.setCheckedEvent('#checkSinCupo', true);
                    }

                    const imagenBase64 = esEspecial === "true" ? data.PdfImageSustentableBase64 : data.PdfImageBase64;
                    const directorio = CargaDeCupoUI.dom.getValue('#CodigoCamaraCPDir');
                    CargaDeCupoApp.images.setCartaPorteImage(
                        imagenBase64 ? '' : 'error',
                        imagenBase64,
                        directorio,
                        esEspecial === "true"
                    );

                    if (data.CodigoDeError == 4 || (data.CodigoDeError === "3" && data.Error)) {
                        CargaDeCupoUI.alerts.showAlert(data.Error, 'alert-block');
                    }
                } else {
                    CargaDeCupoUI.alerts.showAlert(data.Error || 'Error al procesar CPE', 'alert-error');
                }
            } catch (error) {
                console.error('Error al obtener datos del CPE:', error);
                CargaDeCupoUI.alerts.showAlert('Error al obtener datos del CPE', 'alert-error');
            } finally {
                $.unblockUI();
            }
        },

        handlePatenteGranos: async (patente, tarjeta, esEspecial, materialId) => {
            try {
                BlockUI('Consultando CPE por patente...');

                CargaDeCupoUI.dom.setValue('#CTG', '');
                CargaDeCupoUI.form.clearValidation();
                CargaDeCupoUI.alerts.hideAlert();

                const data = await CargaDeCupoAPI.services.obtenerCPEPorPatente(patente, tarjeta, esEspecial, materialId);

                if (data.Error && data.CodigoDeError === "NroCtg") {
                    CargaDeCupoUI.dom.setReadonly('#CTG', false);
                    CargaDeCupoUI.validation.showFieldError('CTG', data.Error);
                } else if (data.Error && data.CodigoDeError === "MaterialId") {
                    CargaDeCupoUI.dom.setReadonly('#CTG', false);
                    CargaDeCupoUI.validation.showFieldError('MaterialId', data.Error);
                } else if (data.Cpe) {
                    CargaDeCupoUI.dom.setValue('#CTG', data.Cpe.NroCartaPorte);
                    CargaDeCupoUI.dom.setValue('#MaterialId', data.Cpe.MaterialId);
                    CargaDeCupoUI.dom.setValue('#RtteComercialCodigoSap', data.Cpe.RtteComercialCodigoSap);
                    CargaDeCupoUI.dom.setValue('#TitularCartaPorteCodigoSap', data.Cpe.TitularCartaPorteCodigoSap);
                    CargaDeCupoUI.dom.setValue('#CodEstab', data.Cpe.CodEstab);
                    CargaDeCupoUI.dom.setValue('#CodigoRENSPA', data.Cpe.CodigoRENSPA);
                    CargaDeCupoUI.dom.setValue('#Cosecha', data.Cpe.Cosecha);
                    CargaDeCupoUI.dom.setValue('#PesoNetoOrigen', data.Cpe.Vehiculos[0]["PesoNetoOrigen"]);
                    CargaDeCupoUI.dom.setValue('#RtteComercialVentaSecundariaCuit', data.Cpe.RtteComercialVentaSecundarioCuil);

                    const patenteAcoplado = data.Cpe.Vehiculos?.[0]?.["PatenteAcoplado"];
                    if (patenteAcoplado) {
                        CargaDeCupoUI.dom.setValue('#PatenteAcoplado', patenteAcoplado);
                    } else {
                        CargaDeCupoUI.alerts.showAlert('Vehiculo sin acoplado', 'alert-block');
                    }

                    if (data.Cpe.Cupo) {
                        const cupoFormateado = CargaDeCupoCore.formatting.truncateCupoLength(data.Cpe.Cupo);
                        CargaDeCupoUI.dom.setCheckedEvent('#checkSinCupo', false);
                        CargaDeCupoUI.dom.setValue('#Cupo', cupoFormateado);
                        await CargaDeCupoApp.business.validateCupoSap();
                    } else {
                        CargaDeCupoUI.dom.setCheckedEvent('#checkSinCupo', true);
                    }

                    const imagenBase64 = esEspecial === "true" ? data.PdfImageSustentableBase64 : data.PdfImageBase64;
                    if (imagenBase64) {
                        const directorio = CargaDeCupoUI.dom.getValue('#CodigoCamaraCPDir');
                        CargaDeCupoApp.images.setCartaPorteImage(
                            '',
                            imagenBase64,
                            directorio,
                            esEspecial === "true"
                        );
                    }

                    if (data.CodigoDeError === "3" && data.Error) {
                        CargaDeCupoUI.alerts.showAlert(data.Error, 'alert-block');
                    }
                } else if (data.Error) {
                    CargaDeCupoUI.dom.setReadonly('#CTG', false);
                    CargaDeCupoUI.dom.getElement('#CTG').focus();
                    CargaDeCupoUI.alerts.showAlert(data.Error, 'alert-error');
                }
            } catch (error) {
                console.error('Error al obtener datos por patente:', error);
                CargaDeCupoUI.dom.setReadonly('#CTG', false);
                CargaDeCupoUI.alerts.showAlert('Error al obtener datos por patente', 'alert-error');
            } finally {
                $.unblockUI();
            }
        },

        handlePatenteNoGranos: async (patente) => {
            try {
                BlockUI(CargaDeCupoUI.dom.getValue('#MensajeBuscandoDatos') || 'Cargando...');

                if (CargaDeCupoCore.validation.isValidPatenteFormat(patente)) {
                    const apiResponse = await CargaDeCupoAPI.services.obtenerOrdenesFasonInsumos(patente);
                    const processedData = CargaDeCupoApp.processors.responseFasonInsumos(apiResponse);

                    CargaDeCupoUI.combo.clearMateriales(false);

                    if (processedData.type === 'modal_error') {
                        CargaDeCupoUI.dialogs.showErrorModal(processedData.message);
                    } else if (processedData.type === 'error') {
                        CargaDeCupoUI.alerts.showAlert(processedData.message, 'alert-error');
                    } else if (processedData.type === 'success') {
                        CargaDeCupoUI.combo.populateMaterialesNoGranos(processedData.ordenes);
                    }

                    CargaDeCupoApp.validations.validateMaterialSelection();
                }
            } catch (error) {
                CargaDeCupoUI.alerts.showAlert('Error al cargar datos de insumos', 'alert-error');
            } finally {
                $.unblockUI();
            }
        },

        takeCartaPortePhoto: async () => {
            if (CargaDeCupoUI.dom.getValue('#SinFotoCartaPorte')) {
                return;
            }

            try {
                BlockUI();

                const puestoDeTrabajoId = CargaDeCupoUI.dom.getValue('#PuestoDeTrabajoId');
                const codigoCamara = CargaDeCupoUI.dom.getValue('#CodigoCamaraCP');
                const directorio = CargaDeCupoUI.dom.getValue('#CodigoCamaraCPDir');
                const numero = CargaDeCupoUI.dom.getValue('#Numero');
                const numeroCartaPorte = CargaDeCupoUI.dom.getValue('#NumeroCartaPorte');

                const data = await CargaDeCupoAPI.services.obtenerFotoPatente(
                    puestoDeTrabajoId,
                    codigoCamara,
                    directorio,
                    false,
                    numero,
                    numeroCartaPorte
                );

                CargaDeCupoApp.images.setCartaPorteImage(data.error, data.imagen, data.directorio);
            } catch (error) {
                console.error('Error taking carta porte photo:', error);
                CargaDeCupoApp.images.setCartaPorteImage('error', '', '');
            } finally {
                $.unblockUI();
            }
        },

        submitForm: async () => {
            const btnAceptar = CargaDeCupoUI.dom.getElement('#btnAceptar');
            btnAceptar.disabled = true;
            btnAceptar.classList.add('disabled');
            btnAceptar.blur();

            try {
                BlockUI();
                const form = CargaDeCupoUI.dom.getElement('#formCargaDeCupo');
                const formData = new FormData(form);

                const response = await fetch(form.action, {
                    method: 'POST',
                    body: formData
                });

                const data = await response.json();

                CargaDeCupoUI.form.clearValidation();
                CargaDeCupoUI.alerts.hideAlert();

                if (data.Success) {
                    const patente = CargaDeCupoUI.dom.getValue('#Patente');
                    if (patente) {
                        CargaDeCupoCore.state.patenteGuardada = patente.trim().toUpperCase();
                    }
                    const imageElement = CargaDeCupoUI.dom.getElement('#imagen-cp');
                    imageElement.src = '';
                    form.reset();

                    MostrarAlertaExitosa(data.Message || 'Carga de cupo realizada exitosamente', 10000);

                    if (data.Data.MensajeTasaMunicipal) {
                        if (data.Data.TipoAlertaTasaMunicipal === 0) {
                            MostrarAlertaExitosaTasaMunicipal(data.Data.MensajeTasaMunicipal, 10000);
                        } else {
                            MostrarAlertaAdvertenciaTasaMunicipal(data.Data.MensajeTasaMunicipal, 10000);
                        }
                    }

                    if (data.ValidationErrors?.["warning"]) {
                        CargaDeCupoUI.alerts.showAlert(data.ValidationErrors["warning"], 'alert-block');
                    }

                    await CargaDeCupoApp.events.handlers.circuitoChange();
                } else {
                    if (data.ValidationErrors) {
                        const globalKeys = new Set(['', 'error', 'warning', 'avanceCpe']);
                        const hasFieldErrors = Object.keys(data.ValidationErrors).some(k => !globalKeys.has(k));
                        if (!hasFieldErrors) {
                            const imageElement = CargaDeCupoUI.dom.getElement('#imagen-cp');
                            if (imageElement) imageElement.src = '';
                            form.reset();
                            await CargaDeCupoApp.events.handlers.circuitoChange();
                        }

                        Object.keys(data.ValidationErrors).forEach(key => {
                            const message = data.ValidationErrors[key];

                            if (key === '' || key === 'error' || key === 'avanceCpe') {
                                CargaDeCupoUI.alerts.showAlert(message, 'alert-error');
                            } else if (key === 'warning') {
                                CargaDeCupoUI.alerts.showAlert(message, 'alert-block');
                            } else {
                                CargaDeCupoUI.validation.showFieldError(key, message);
                            }
                        });
                    }
                }
            } catch (error) {
                console.error('Error submitting form:', error);
                MostrarAlertaError('Ocurrió un error al ingresar camión');
            } finally {
                btnAceptar.disabled = false;
                btnAceptar.classList.remove('disabled');
                $.unblockUI();
            }
        }
    },

    polling: {
        startListarCupos: () => {
            const updateCupos = async () => {
                if (CargaDeCupoUI.dom.getElement('#dialogo-confirmar').open) {
                    setTimeout(updateCupos, CargaDeCupoCore.CONSTANTS.LISTAR_CAMIONES_REFRESH_INTERVAL);
                    return;
                }

                if (CargaDeCupoUI.dom.getElement('#dialogo-error').open) {
                    setTimeout(updateCupos, CargaDeCupoCore.CONSTANTS.LISTAR_CAMIONES_REFRESH_INTERVAL);
                    return;
                }

                try {
                    const data = await CargaDeCupoAPI.services.listarCupos();
                    const listaCuposContainer = CargaDeCupoUI.dom.getElement('#listaCupos');
                    if (listaCuposContainer && data) {
                        listaCuposContainer.innerHTML = data;
                    }
                } catch (error) {
                    console.error('Error actualizando lista de cupos:', error);
                } finally {
                    setTimeout(updateCupos, CargaDeCupoCore.CONSTANTS.LISTAR_CAMIONES_REFRESH_INTERVAL);
                }
            };

            updateCupos();
        },

        startPatenteImageRefresh: () => {
            if (CargaDeCupoCore.state.iniciarLoopFotoPatenteActivo) {
                return;
            }

            const refreshPatenteLoop = async () => {
                if (CargaDeCupoUI.dom.getElement('#dialogo-confirmar').open) {
                    setTimeout(refreshPatenteLoop, CargaDeCupoCore.CONSTANTS.CONSULTAR_FOTO_REFRESH_INTERVAL);
                    return;
                }

                if (CargaDeCupoUI.dom.getElement('#dialogo-error').open) {
                    setTimeout(refreshPatenteLoop, CargaDeCupoCore.CONSTANTS.CONSULTAR_FOTO_REFRESH_INTERVAL);
                    return;
                }

                try {
                    const puestoDeTrabajoId = CargaDeCupoUI.dom.getValue('#PuestoDeTrabajoId');
                    const codigoCamara = CargaDeCupoUI.dom.getValue('#CodigoCamaraPatente');
                    const directorio = CargaDeCupoUI.dom.getValue('#CodigoCamaraPatenteDir');

                    const data = await CargaDeCupoAPI.services.obtenerFotoPatente(
                        puestoDeTrabajoId,
                        codigoCamara,
                        directorio
                    );
                    CargaDeCupoApp.images.setPatenteImage(data.error, data.imagen, data.patente);
                } catch (error) {
                    console.error('Error refreshing patente image:', error);
                    CargaDeCupoApp.images.setPatenteImage('error', '', null);
                } finally {
                    setTimeout(refreshPatenteLoop, CargaDeCupoCore.CONSTANTS.CONSULTAR_FOTO_REFRESH_INTERVAL);
                }
            };

            CargaDeCupoCore.state.iniciarLoopFotoPatenteActivo = true;
            refreshPatenteLoop();
        }
    },

    signalR: {
        config: {
            notificaLectura: null
        },

        handlers: {
            informarLectura: (notificacion) => {
                CargaDeCupoUI.alerts.hideAlertTarjeta();
                CargaDeCupoUI.alerts.hideAlertPatente();

                if (!notificacion.TarjetaValida && !notificacion.EsTarjetaSupervisor) {
                    CargaDeCupoUI.alerts.showAlertTarjeta(notificacion.MensajeError);
                    CargaDeCupoUI.dom.setValue('#Numero', '');
                } else if (notificacion.EsTarjetaSupervisor) {
                    CargaDeCupoUI.alerts.showAlertPatente(notificacion.MensajeError);
                    CargaDeCupoUI.dom.setValue('#Numero', '');
                } else if (notificacion.NumeroDeTarjeta) {
                    CargaDeCupoUI.dom.setValue('#Numero', notificacion.NumeroDeTarjeta);
                }
            },

            informarEstadoConexion: (notificacion) => {
                if (notificacion.Estado) {
                    CargaDeCupoUI.dom.addClass('#labelConectado', 'hidden');
                    CargaDeCupoUI.dom.removeClass('#labelDesconectado', 'hidden');
                } else {
                    CargaDeCupoUI.dom.removeClass('#labelConectado', 'hidden');
                    CargaDeCupoUI.dom.addClass('#labelDesconectado', 'hidden');
                }
            },

            informarLecturaCpe: (notificacion) => {
                if (notificacion.NroCtg) {
                    if (!CargaDeCupoUI.dom.getValue('#CTG')) {
                        const ctgField = CargaDeCupoUI.dom.getElement('#CTG');
                        ctgField.focus();
                        CargaDeCupoUI.dom.setValue('#CTG', notificacion.NroCtg);

                        if (CargaDeCupoCore.validation.isValidCtgLength(notificacion.NroCtg)) {
                            ctgField.dispatchEvent(new Event('change'));
                            const cupoField = CargaDeCupoUI.dom.getElement('#Cupo');
                            cupoField.focus();
                        }
                    }
                }
            }
        }
    }
};

window.CargaDeCupoApp = CargaDeCupoApp;
document.addEventListener('DOMContentLoaded', CargaDeCupoApp.init.all);