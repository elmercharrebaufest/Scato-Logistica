const gridContainer = document.getElementById('gridContainer');
const tfoot = gridContainer.querySelector('table#grid tfoot');
const historicoChk = document.getElementById("historico");
const filtroInput = document.querySelector('input[name="filtro"]');


function updateQueryStringParameter(url, key, value) {
    const urlObj = new URL(url, window.location.origin);
    urlObj.searchParams.set(key, value);
    return urlObj.toString();
}
function handlePaginadoClick(event, link) {
    event.preventDefault();

    const filtroInputValue = document.querySelector('input[name="filtro"]').value;
    const historicoChecked = localStorage.getItem("historicoChecked") || "false";

    const currentHref = link.getAttribute('href');
    const urlObj = new URL(currentHref, window.location.origin);

    urlObj.searchParams.set('filtro', filtroInputValue);
    urlObj.searchParams.set('historico', historicoChecked);

    urlObj.searchParams.delete('X-Requested-With');

    const updatedUrl = urlObj.toString();
    link.setAttribute('href', updatedUrl);

    window.location.href = updatedUrl;
}


document.addEventListener('DOMContentLoaded', () => {
    const gridContainer = document.getElementById('gridContainer');
    if (gridContainer) {
        const tfoot = gridContainer.querySelector('table#grid tfoot');
        if (tfoot) {
            const links = tfoot.querySelectorAll('a');
            links.forEach((link) => {
                link.addEventListener('click', (event) => handlePaginadoClick(event, link));
            });
        }
    } else {
        console.error('El contenedor gridContainer no se encontró.');
    }
});
