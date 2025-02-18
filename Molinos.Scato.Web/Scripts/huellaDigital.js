const choferInput = document.getElementById('Chofer');
const idChoferInput = document.getElementById('IdChofer');
const transportistaInput = document.getElementById('Transportista');
const idTransportistaInput = document.getElementById('IdTransportista');
const observacionesInput = document.getElementById('Observaciones');
const patenteInput = document.getElementById("Patente");
const acopladoInput = document.getElementById("Acoplado");
const filtroInput = document.querySelector('input[name="filtro"]');
const historicoChk = document.getElementById("Historico");
const centrosDropDown = document.getElementById("centrosDropDown");
const balanzaDropDown = document.getElementById(" balanzaDropDown");

const linkChoferUnico = $('#links').data().urlBuscarChoferUnico;
const linkTransportistas = $('#links').data().urlBuscarTransportistas;
const linkBalanzas = $('#links').data().urlBuscarBalanza;


choferInput.addEventListener('input', function () {

    AutocompletarChofer(choferInput, idChoferInput, linkChoferUnico);
});


transportistaInput.addEventListener('input', function () {

    AutocompletarTransportista(transportistaInput, idTransportistaInput, linkTransportistas);

});

centrosDropDown.addEventListener('change', function () {

    ActualizarBalanzasPorCentro(this.value);

});

function BordeColorChofer(elemento, elementoId) {
    if ($(elementoId).val() != 0) {
        $(elemento).css('border', 'solid 1px green');
        $(elemento).css('border-right', 'solid 5px green');
        $(elemento).addClass("italic");
    }
    if ($(elemento).val() == "") {
        $(elemento).css('border', 'solid 1px #CCCCCC');
        $(elemento).removeClass("italic");
    }
}

function AutocompletarChofer(elemento, elementoId, linkListar) {

    $(elemento).autocomplete({
        delay: 100,
        source: function (request, response) {

            if (typeof linkListar === "string") {
                $.ajax({
                    url: linkListar,
                    dataType: "json",
                    data: { term: request.term },
                    success: function (data) {
                        response(data);
                    },
                    error: function () {
                        console.error("Error al cargar datos de autocompletado");
                        response([]);
                    }
                });
            } else if (Array.isArray(linkListar)) {
                // Si es un array, filtra los resultados localmente
                response(
                    $.grep(linkListar, function (item) {
                        return item.label.toLowerCase().includes(request.term.toLowerCase());
                    })
                );
            }
        },

        max: 15,
        minLength: 5,
        appendTo: "#autocompleteChofer", // Contenedor donde se mostrará la lista
        response: function (event, ui) {

        },
        open: function () {
            $(elemento).data("is_open", true);

        },
        close: function () {
            $(elemento).data("is_open", false);

        },
        select: function (event, ui) {

            idChoferInput.value = ui.item.value
            choferInput.value = ui.item.label
            BordeColorChofer(elemento, elementoId);
            return false; // Evitar que autocomplete sobrescriba el valor del input
        }
    });

}

function AutocompletarTransportista(elemento, elementoId, linkListar) {
    $(elemento).autocomplete({
        delay: 100,
        source: function (request, response) {

            if (typeof linkListar === "string") {
                $.ajax({
                    url: linkListar,
                    dataType: "json",
                    data: { term: request.term },
                    success: function (data) {
                        response(data);
                    },
                    error: function () {
                        console.error("Error al cargar datos de autocompletado");
                        response([]);
                    }
                });
            } else if (Array.isArray(linkListar)) {
                response(
                    $.grep(linkListar, function (item) {
                        return item.label.toLowerCase().includes(request.term.toLowerCase());
                    })
                );
            }
        },

        max: 15,
        minLength: 5,
        appendTo: "#autocompleteTran", // Contenedor donde se mostrará la lista
        response: function (event, ui) {
            console.log(ui)

        },
        open: function () {
            $(elemento).data("is_open", true);


        },
        close: function () {
            $(elemento).data("is_open", false);

        },
        select: function (event, ui) {
            console.log(ui)
            idTransportistaInput.value = ui.item.value
            transportistaInput.value = ui.item.label
            BordeColorChofer(elemento, elementoId);
            return false; // Evitar que autocomplete sobrescriba el valor del input
        }
    });

}

function ActualizarBalanzasPorCentro(centroId) {
    balanzaDropDown.innerHTML = '<option value="">(balanza tara)</option>';
    if (!centroId) return; 

    fetch(`${linkBalanzas}?idCentro=${centroId}`)
        .then(response => response.json())
        .then(data => {
            // Agregar las opciones al dropdown de balanzas
            data.forEach(balanza => {
                const option = document.createElement('option');
                option.value = balanza.Value;
                option.textContent = balanza.Text;
                balanzaDropDown.appendChild(option);
            });
        })
        .catch(error => console.error('Error:', error));
}


