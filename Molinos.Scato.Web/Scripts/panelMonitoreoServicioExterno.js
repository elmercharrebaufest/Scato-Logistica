function setCardBusy(card, busy) {
    const btn = card.querySelector('.btn--refresh-servicio-externo, .btn-refresh');
    const status = card.querySelector('.healthCheckEstado');
    if (busy) {
        if (btn) btn.disabled = true;
        if (status) {
            status.dataset.prevStatus = status.textContent || '';
            status.textContent = 'Consultando...';
        }
        card.classList.add('loading');
    } else {
        if (btn) btn.disabled = false;
        if (status && status.dataset && status.dataset.prevStatus !== undefined) {
            status.textContent = status.dataset.prevStatus;
            delete status.dataset.prevStatus;
        }
        card.classList.remove('loading');
    }
}

function refreshAll(targetContainerId) {
    const container = document.getElementById(targetContainerId);
    if (!container) return;
    const cards = container.querySelectorAll('.card__container');
    cards.forEach(function (card) { setCardBusy(card, true); });

    fetch($('#linksMonitoreoServicioExterno').data().urlActualizarTodosServiciosExternos, { method: 'POST', headers: { 'Content-Type': 'application/json' } })
        .then(function (r) { return r.json ? r.json() : r.text(); })
        .then(function (result) {
            if (result && result.success === false) {
                cards.forEach(function (card) { setCardBusy(card, false); });
            }
        }).catch(function (err) {
            cards.forEach(function (card) { setCardBusy(card, false); });
            console.error('Error iniciando healthchecks:', err);
        });
}

function attachRefreshButtons() {
    const buttons = document.querySelectorAll('.btn--refresh-servicio-externo, .btn-refresh');
    buttons.forEach(function (btn) {
        if (btn._refreshAttached) return;
        btn._refreshAttached = true;
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            let key = btn.getAttribute('data-key');
            if (!key) return;
            const card = btn.closest('.card__container');
            if (card) setCardBusy(card, true);

            fetch($('#linksMonitoreoServicioExterno').data().urlActualizarUnicoServicioExterno, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ key: key }) })
                .then(function (resp) {
                    return resp.json();
                })
                .then(function (result) {
                    if (result && result.success === false) {
                        if (card) setCardBusy(card, false);
                    }
                })
                .catch(function (err) {
                    if (card) setCardBusy(card, false);
                    console.error('Error calling healthcheck endpoint', err);
                });
        });
    });
}

function computeHealthcheckSemaforo() {
    try {
        const elIcon = document.getElementById('semaforoGeneralHealthCheckServiciosExternos');
        if (!elIcon) return;

        const els = document.querySelectorAll('.card__container .healthCheckEstado');
        let statuses = [];
        els.forEach(function (el) {
            var txt = (el.textContent || '');
            if (txt) statuses.push(txt);
        });

        const desconectado = statuses.some(function (s) { return s === 'Desconectado'; });
        const conectado = statuses.every(function (s) { return s === 'Conectado'; });
        
        if (desconectado) {
            elIcon.style.color = 'red';
        } else if (conectado) {
            elIcon.style.color = '#36634b';
        } else {
            elIcon.style.color = 'gray';
        }
    } catch (e) {
        console.error('computeHealthcheckSemaforo error', e);
    }
};

document.addEventListener('DOMContentLoaded', function () {
    const btnAll = document.getElementById('btnActualizarServiciosExternos');
    if (btnAll) {
        btnAll.addEventListener('click', function (e) {
            e.preventDefault();
            refreshAll('panel-servicios__list');
        });
    }

    attachRefreshButtons();
    computeHealthcheckSemaforo();
});