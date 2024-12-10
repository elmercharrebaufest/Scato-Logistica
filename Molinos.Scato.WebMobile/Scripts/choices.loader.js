/*

choices.loader v1.0.0  @Hernan Lavrencic

Dependency: 
- choices.js v11.0.2

Description:
Este script se encarga de inicializar los elementos <select> con el atributo multiple

Usage:
1. Agregar el atributo multiple a los elementos <select> que se deseen inicializar con Choices
2. Agregar el script en el layout o en la vista que contenga los elementos <select> con el atributo multiple

*/

// Funcion anonima para generar un scope aislado.
(function () {

    // funcion que actualiza el combo de multi seleccion cada vez que se modifica el select o su contenido.
    const bindOptionsUpdates = (targetNode, choicesElement) => {
        // Opciones de configuración para el observer
        const config = {
            childList: true, // Observa adiciones y eliminaciones de hijos
            //subtree: true,   // Observa cambios en todos los descendientes
        };

        // Callback que se ejecuta cuando se detectan cambios
        const callback = (mutationsList) => {
            for (let mutation of mutationsList) {
                if (mutation.type === 'childList') {
                    choicesElement.refresh();
                }
            }
        };

        // Crea una instancia de MutationObserver
        const observer = new MutationObserver(callback);

        // Comienza a observar el nodo objetivo con las opciones configuradas
        observer.observe(targetNode, config);
    };

    const bindChoices = (elem) => {
        // Selecciona todos los elementos <select> con el atributo multiselect
        const elements = elem.querySelectorAll('select[multiple]');

        // Itera sobre cada elemento seleccionado y crea un nuevo objeto Choices
        elements.forEach(element => {
            // Para evitar inicializar un elemento que ya fue iniciacilizado con Choices, verifico el attribute
            if (!element.hasAttribute('data-choice')) {
                const placeholder = element.getAttribute('placeholder');
                const choices = new Choices(element, {
                    removeItemButton: true,
                    noChoicesText: 'Sin opciones',
                    placeholderValue: placeholder || '(seleccione)',
                    itemSelectText: '<',
                });

                bindOptionsUpdates(element, choices);
            }
        });
    };

    document.addEventListener('DOMContentLoaded', () => {
        bindChoices(document);

        // Selecciona el nodo que deseas observar (puede ser document.body o cualquier otro nodo)
        const targetNode = document.body;

        // Opciones de configuración para el observer
        const config = {
            childList: true, // Observa adiciones y eliminaciones de hijos
            subtree: true,   // Observa cambios en todos los descendientes
        };

        // Callback que se ejecuta cuando se detectan cambios
        const callback = (mutationsList) => {
            for (let mutation of mutationsList) {
                if (mutation.type === 'childList') {
                    bindChoices(mutation.target);
                }
            }
        };

        // Crea una instancia de MutationObserver
        const observer = new MutationObserver(callback);

        // Comienza a observar el nodo objetivo con las opciones configuradas
        observer.observe(targetNode, config);

        // Para dejar de observar, puedes usar:
        // observer.disconnect();
    });
})(); 