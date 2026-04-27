const CargaDeCupoUI = {
    dom: {
        getElement: (selector) => document.querySelector(selector),
        getValue: (selector) => document.querySelector(selector).value,
        setValue: (selector, value) => document.querySelector(selector).value = value,
        isChecked: (selector) => document.querySelector(selector).checked,
        setCheckedEvent: (selector, checked) => {
            const element = document.querySelector(selector);
            if (element && element.checked !== checked) {
                element.checked = checked;
                element.dispatchEvent(new Event('change', { bubbles: true }));
            }
        },
        setChecked: (selector, checked) => {
            const element = document.querySelector(selector);
            if (element && element.checked !== checked) {
                element.checked = checked;
            }
        },
        addClass: (selector, className) => document.querySelector(selector).classList.add(className),
        removeClass: (selector, className) => document.querySelector(selector).classList.remove(className),
        setReadonly: (selector, readonly) => {
            const element = document.querySelector(selector);
            if (element) {
                if (readonly) {
                    element.setAttribute('readonly', 'readonly');
                } else {
                    element.removeAttribute('readonly');
                }
            }
        },
        setDisabled: (selector, disabled) => {
            const element = document.querySelector(selector);
            if (element) {
                if (disabled) {
                    element.setAttribute('disabled', 'disabled');
                } else {
                    element.removeAttribute('disabled');
                }
            }
        }
    },

    validation: {
        showFieldError: (fieldName, errorMessage) => {
            const fieldMsg = CargaDeCupoUI.dom.getElement(`[data-valmsg-for='${fieldName}']`);
            if (fieldMsg) {
                const errorSpan = document.createElement('span');
                errorSpan.textContent = errorMessage;
                fieldMsg.innerHTML = '';
                fieldMsg.appendChild(errorSpan);
                fieldMsg.classList.add('field-validation-error');
                fieldMsg.classList.remove('field-validation-valid');
            }

            const fieldInput = CargaDeCupoUI.dom.getElement(`#${fieldName}`);
            if (fieldInput) {
                fieldInput.classList.add('input-validation-error');
                fieldInput.focus();
            }
        },

        clearFieldError: (fieldName) => {
            const fieldMsg = CargaDeCupoUI.dom.getElement(`[data-valmsg-for='${fieldName}']`);
            if (fieldMsg) {
                fieldMsg.innerHTML = '';
                fieldMsg.classList.add('field-validation-valid');
                fieldMsg.classList.remove('field-validation-error');
            }

            const fieldInput = CargaDeCupoUI.dom.getElement(`#${fieldName}`);
            if (fieldInput) {
                fieldInput.classList.remove('input-validation-error');
            }
        },
    },

    config: {
        applyNoGranosConfig: () => {
            CargaDeCupoUI.form.clearFieldValues();
            CargaDeCupoUI.form.clearValidation();

            CargaDeCupoUI.dom.setCheckedEvent('#checkSinCupo', true);
            CargaDeCupoUI.dom.setDisabled('#checkSinCupo', true);
            CargaDeCupoUI.dom.setReadonly('#Cupo', true);
            CargaDeCupoUI.dom.setCheckedEvent('#checkvalidarPatente', true);
        },

        applyGranosConfig: () => {
            CargaDeCupoUI.form.clearFieldValues();
            CargaDeCupoUI.form.clearValidation();

            CargaDeCupoUI.dom.setCheckedEvent('#checkSinCupo', false);
            CargaDeCupoUI.dom.setDisabled('#checkSinCupo', false);
            CargaDeCupoUI.dom.setReadonly('#Cupo', false);
            CargaDeCupoUI.dom.setCheckedEvent('#checkvalidarPatente', true);
        },
    },

    combo: {
        clearMateriales: (addDefault = false) => {
            const materialSelect = CargaDeCupoUI.dom.getElement('#MaterialId');
            if (materialSelect) {
                materialSelect.innerHTML = '';
                if (addDefault) {
                    const defaultOption = document.createElement('option');
                    defaultOption.value = '';
                    defaultOption.textContent = '(material)';
                    materialSelect.appendChild(defaultOption);
                }
            }
        },

        setTipoOrdenCargaNoGranos: () => {
            const materialSelect = CargaDeCupoUI.dom.getElement('#MaterialId');
            if (!materialSelect) return;

            const selectedOption = materialSelect.options[materialSelect.selectedIndex];
            CargaDeCupoUI.dom.setValue('#TipoOrdenCargaNoGranos', selectedOption?.dataset.tipoCarga || '');
        },

        populateMaterialesNoGranos: (ordenes) => {
            const materialSelect = CargaDeCupoUI.dom.getElement('#MaterialId');
            if (!materialSelect) return;

            if (ordenes.length > 1) {
                const defaultOption = document.createElement('option');
                defaultOption.value = '';
                defaultOption.textContent = '(material)';
                materialSelect.appendChild(defaultOption);
                CargaDeCupoUI.dom.setValue('#HayVariosMateriales', 'True');
            } else {
                CargaDeCupoUI.dom.setValue('#HayVariosMateriales', 'False');
            }

            if (ordenes.length > 0 && ordenes[0].PatenteAcoplado) {
                CargaDeCupoUI.dom.setValue('#PatenteAcoplado', ordenes[0].PatenteAcoplado);
            }

            ordenes.forEach(orden => {
                const option = document.createElement('option');
                option.value = orden.MaterialId;
                option.textContent = orden.MaterialDescripcion;
                option.dataset.tipoCarga = orden.TipoOrden;
                materialSelect.appendChild(option);
            });

            CargaDeCupoUI.combo.setTipoOrdenCargaNoGranos();
        },

        populateMateriales: (materiales) => {
            const materialSelect = CargaDeCupoUI.dom.getElement('#MaterialId');
            if (!materialSelect) return;

            const currentValue = materialSelect.value;

            materialSelect.innerHTML = '';

            const defaultOption = document.createElement('option');
            defaultOption.value = '';
            defaultOption.textContent = '(material)';
            defaultOption.selected = true;
            materialSelect.appendChild(defaultOption);

            materiales.forEach(material => {
                const option = document.createElement('option');
                option.value = material.Value;
                option.textContent = material.Text;
                option.selected = material.Value === currentValue;
                materialSelect.appendChild(option);
            });

            materialSelect.value = currentValue;
        },
    },

    alerts: {
        showAlert: (message, alertType = 'alert-success') => {
            const messageContainer = CargaDeCupoUI.dom.getElement('#validation-alert-cargaDeCupo-message');
            messageContainer.innerHTML = message;

            const alertContainer = CargaDeCupoUI.dom.getElement('#validation-alert-cargaDeCupo');
            alertContainer.classList.remove('hide', 'alert-error', 'alert-block', 'alert-info', 'alert-success');
            alertContainer.classList.add(alertType);
        },

        hideAlert: () => {
            const messageContainer = CargaDeCupoUI.dom.getElement('#validation-alert-cargaDeCupo-message');
            messageContainer.innerHTML = '';

            const alertContainer = CargaDeCupoUI.dom.getElement('#validation-alert-cargaDeCupo');
            alertContainer.classList.remove('hide', 'alert-error', 'alert-block', 'alert-info', 'alert-success');
            alertContainer.classList.add('hide');
        },

        showAlertPatente: (message) => {
            const dangerContainer = CargaDeCupoUI.dom.getElement('#validation-patente-danger-message');
            dangerContainer.innerHTML = `<strong>${message}</strong>`;

            const alertContainer = CargaDeCupoUI.dom.getElement('#validation-patente-danger');
            alertContainer.classList.remove('hide');
        },

        hideAlertPatente: () => {
            const dangerContainer = CargaDeCupoUI.dom.getElement('#validation-patente-danger-message');
            dangerContainer.innerHTML = '';

            const alertContainer = CargaDeCupoUI.dom.getElement('#validation-patente-danger');
            alertContainer.classList.add('hide');
        },

        showAlertTarjeta: (message) => {
            const container = CargaDeCupoUI.dom.getElement('#validation-tarjeta');
            container.innerHTML = message;

            const alertContainer = CargaDeCupoUI.dom.getElement('#validation-tarjeta-alert');
            alertContainer.classList.remove('hide');
        },

        hideAlertTarjeta: () => {
            const container = CargaDeCupoUI.dom.getElement('#validation-tarjeta');
            container.innerHTML = '';

            const alertContainer = CargaDeCupoUI.dom.getElement('#validation-tarjeta-alert');
            alertContainer.classList.add('hide');
        },
    },

    dialogs: {
        showErrorModal: (message) => {
            const bodyElement = CargaDeCupoUI.dom.getElement('#dialogo-error-body');
            bodyElement.innerHTML = `<strong>${message}</strong>`;

            const dialog = CargaDeCupoUI.dom.getElement('#dialogo-error');
            dialog.showModal();
        },

        showConfirmationModal: (message, onConfirm) => {
            const dialog = CargaDeCupoUI.dom.getElement('#dialogo-confirmar');
            const bodyElement = CargaDeCupoUI.dom.getElement('#dialogo-confirmar-body');
            bodyElement.textContent = message;

            const confirmBtn = CargaDeCupoUI.dom.getElement('#dialogo-confirmar-confirmar');
            const newConfirmBtn = confirmBtn.cloneNode(true);
            confirmBtn.parentNode.replaceChild(newConfirmBtn, confirmBtn);
            newConfirmBtn.addEventListener('click', () => {
                dialog.close();
                onConfirm();
            });
            dialog.showModal();
        },
    },

    form: {
        clearValidation: () => {
            const validationErrors = document.querySelectorAll('.validation-summary-errors');
            validationErrors.forEach(el => el.style.display = 'none');

            const errorElements = document.querySelectorAll('.error');
            errorElements.forEach(el => el.classList.remove('error'));

            const fieldValidationErrors = document.querySelectorAll('.field-validation-error');
            fieldValidationErrors.forEach(el => {
                el.innerHTML = '';
                el.classList.remove('field-validation-error');
                el.classList.add('field-validation-valid');
            });
        },

        clearFieldValues: () => {
            CargaDeCupoUI.dom.setValue('#CTG', '');
            CargaDeCupoUI.dom.setReadonly('#CTG', true);
            CargaDeCupoUI.dom.setValue('#Patente', '');
            CargaDeCupoUI.dom.setReadonly('#Patente', true);
            CargaDeCupoUI.dom.setValue('#Cupo', '');
            CargaDeCupoUI.dom.setValue('#TipoOrdenCargaNoGranos', '');
            CargaDeCupoUI.dom.setValue('#CodEstab', '');
            CargaDeCupoUI.dom.setValue('#RtteComercialCodigoSap', '');
            CargaDeCupoUI.dom.setValue('#TitularCartaPorteCodigoSap', '');
            CargaDeCupoUI.dom.setValue('#CodigoRENSPA', '');
            CargaDeCupoUI.dom.setValue('#Cosecha', '');
            CargaDeCupoUI.dom.setValue('#PesoNetoOrigen', '0');
            CargaDeCupoUI.dom.setValue('#HayVariosMateriales', 'False');
            CargaDeCupoUI.combo.clearMateriales(true);
        },
    }
};

window.CargaDeCupoUI = CargaDeCupoUI;