---
name: frontend-ko-signalr
description: "Patrones de frontend para este proyecto: Web (jQuery 1.11, Knockout 2.2.1, SignalR 2.4.1, Bootstrap 2.3.2) y WebMobile (jQuery 3.7.1, Knockout 3.5.1, Bootstrap 5). Usar al escribir o revisar JS en Molinos.Scato.Web/Scripts o Molinos.Scato.WebMobile/Scripts."
---

# Frontend — Scato Logistica

## ⚠️ Dos portales — stacks distintos

| | `Molinos.Scato.Web/Scripts/` | `Molinos.Scato.WebMobile/Scripts/` |
|---|---|---|
| jQuery | **1.11** | **3.7.1** |
| Knockout | **2.2.1** | **3.5.1** |
| Bootstrap | **2.3.2** | **5.x** |
| SignalR | ✅ 2.4.1 | ❌ (mayoría de pantallas) |
| Globalize | ✅ (`es-AR`) | ❌ |
| moment.js | ❌ | ✅ |
| Choices.js | ❌ | ✅ |
| PWA | ❌ | ✅ (`service-worker.js`) |

**Regla**: nunca aplicar patrones de Web a WebMobile ni viceversa.

---

## Web — Estructura de módulo JS (patrón del proyecto)

```js
const MiPantallaApp = {
    init: {
        all: () => {
            try {
                BlockUI();
                MiPantallaApp.events.setup();
                MiPantallaApp.init.signalR();
                MiPantallaApp.init.ui();
            } catch (error) {
                MostrarAlertaError('Error al inicializar');
            } finally {
                $.unblockUI();
            }
        },
        signalR: async () => {
            if (!$.connection) return;
            MiPantallaApp.state.hub = $.connection.miHub;
            MiPantallaApp.state.hub.client.onEvento = MiPantallaApp.handlers.onEvento;
            if (window.hubReady) {
                await window.hubReady;
                await MiPantallaApp.state.hub.server.suscribir(centroId);
            }
        }
    },
    state:    { hub: null },
    events:   { setup: () => { /* bindear eventos jQuery */ } },
    handlers: { onEvento: (data) => { /* actualizar UI */ } }
};
$(document).ready(MiPantallaApp.init.all);
```

## Knockout 2.2.1 — patrones disponibles

```js
function MiViewModel() {
    var self = this;
    self.items     = ko.observableArray([]);
    self.seleccion = ko.observable(null);
    self.total     = ko.computed(function () { return self.items().length; });

    self.agregar = function (item) { self.items.push(item); };
    self.quitar  = function (item) { self.items.remove(item); };
}
ko.applyBindings(new MiViewModel(), document.getElementById('contenedor'));
```

Bindings en Razor: `data-bind="text: nombre, visible: activo, click: accion"`.

**No usar** en v2.2.1: `ko.components`, `ko.mapping` plugin, `ko.bindingHandlers` avanzados.

## AJAX — respuesta estándar del proyecto

El servidor retorna `{ EsValido: bool, Mensajes: [{ Mensaje: string }], Datos: object }`.

```js
// GET con JSON
$.getJSON(url, { param: valor }, function (response) {
    if (!response.EsValido) {
        MostrarAlertaError(response.Mensajes.map(function(m) { return m.Mensaje; }).join('<br>'));
    } else {
        // usar response.Datos
    }
});

// POST de formulario
$.post(url, $('form').serialize(), function (response) {
    if (response.EsValido) window.location.href = response.Redirect || window.location.href;
    else MostrarAlertaError(response.Mensajes[0].Mensaje);
});
```

## Helpers globales disponibles

| Función | Propósito |
|---|---|
| `BlockUI()` / `$.unblockUI()` | Bloquear/desbloquear UI durante operaciones |
| `MostrarAlertaError(html)` | Modal de error al usuario |
| `MostrarAlertaExito(html)` | Modal de confirmación |
| `Globalize.parseFloat(str)` | Parsear número con cultura `es-AR` (`"1.234,56"`) |
| `Globalize.format(num, "n2")` | Formatear número con cultura `es-AR` |

## SignalR 2.4.1 — reglas

- Siempre esperar `window.hubReady` antes de llamar a `.server.*`.
- Definir `.client.*` antes de iniciar la conexión (`hubReady`).
- Si `window.hubReady` no existe, hacer `window.location.reload()` como fallback.
- Hub proxy: `$.connection.{nombreHub}` — nombre del hub en camelCase.

## Pasar datos server → JS

```html
<!-- En Razor: pasar datos de modelo a JS vía hidden inputs o atributos data- -->
@Html.Hidden("centroId", Model.CentroId)
<div id="links" data-validar-url="@Url.Action("Validar", "MiController")"></div>
```

```js
var centroId = $('#centroId').val();
var url = $('#links').data().validarUrl;
```

## Reglas

- No `async/await` en callbacks jQuery — solo en funciones `async` declaradas explícitamente.
- No acceder a `@Model` desde JS externo — pasar via hidden inputs o `data-*`.
- No usar `var` para variables de módulo — usar `const` / `let` en nuevos módulos.
- Validaciones de número: siempre con `$.isNumeric(Globalize.parseFloat(valor))`.

---

## WebMobile — diferencias críticas

### Modal Bootstrap 5

```js
// ✅ Bootstrap 5 (WebMobile)
const modal = new bootstrap.Modal(document.getElementById('miModal'));
modal.show();
modal.hide();

// ❌ No usar en WebMobile — API Bootstrap 2/3
$('#miModal').modal('show');
```

### Knockout 3.5.1 — disponible en WebMobile

```js
// ko.pureComputed disponible en KO 3.5.1
self.totalActivo = ko.pureComputed(function () {
    return self.items().filter(function (i) { return i.activo(); }).length;
});

// ko.components disponible en KO 3.5.1 (NO en Web)
ko.components.register('mi-componente', { ... });
```

### Fechas en WebMobile — usar moment.js

```js
// moment.js disponible solo en WebMobile
const fecha = moment(item.Fecha).format('DD/MM/YYYY');
const hoy   = moment().startOf('day');
```

### Service Worker — no cachear URLs de API

El `service-worker.js` en WebMobile puede cachear assets estáticos.
No registrar URLs dinámicas (`/api/*`, `/MiController/*`) en la cache del SW.

### Reglas exclusivas de WebMobile

- `Globalize` **no está disponible** — formatear fechas con `moment.js`.
- `$.connection` (SignalR) **no está disponible** en la mayoría de pantallas.
- Bootstrap 5 usa `data-bs-*` en lugar de `data-*` para atributos de componentes.
- `choices.js` disponible para selects con búsqueda y multi-select.
