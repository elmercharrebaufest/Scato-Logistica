const CargaDeCupoAPI = {
    urls: {
        get: () => {
            const linksElement = document.querySelector('#links');
            if (!linksElement) return {};

            return {
                listar: linksElement.dataset.urlListar,
                obtenerCartaPorteCtg: linksElement.dataset.urlObtenerCartaPorteCtg, // No utilizado actualmente
                obtenerPatente: linksElement.dataset.urlObtenerPatente,
                obtenerMaterial: linksElement.dataset.urlObtenerMaterial,
                obtenerCpe: linksElement.dataset.urlObtenerCpe,
                obtenerCpePorPatente: linksElement.dataset.urlObtenerCpePorPatente,
                obtenerOrdenesInsumos: linksElement.dataset.urlObtenerOrdenesInsumos,
                validarCupoSap: linksElement.dataset.urlValidarCupoSap
            };
        }
    },

    config: {
        defaultHeaders: {
            'Content-Type': 'application/json'
        },

        handleResponse: async (response) => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const contentType = response.headers.get('content-type');
            if (contentType && contentType.includes('application/json')) {
                return await response.json();
            }

            return await response.text();
        },

        handleError: (error) => {
            console.error('API Error:', error);
            throw error;
        }
    },

    services: {
        listarCupos: async () => {
            try {
                const urls = CargaDeCupoAPI.urls.get();
                if (!urls.listar) throw new Error('URL listar no configurada');

                const response = await fetch(urls.listar, {
                    method: 'GET',
                    headers: CargaDeCupoAPI.config.defaultHeaders
                });

                return await CargaDeCupoAPI.config.handleResponse(response);
            } catch (error) {
                return CargaDeCupoAPI.config.handleError(error);
            }
        },

        obtenerFotoPatente: async (puestoDeTrabajoId, codigoCamara, directorio, fotoPatente = true, numero = '', numeroCartaPorte = '') => {
            try {
                const urls = CargaDeCupoAPI.urls.get();
                if (!urls.obtenerPatente) throw new Error('URL obtener patente no configurada');

                const params = new URLSearchParams({
                    puestodetrabajoid: puestoDeTrabajoId,
                    codigoCamara: codigoCamara,
                    directorio: directorio,
                    fotoPatente: fotoPatente,
                    numero: numero,
                    numeroCartaPorte: numeroCartaPorte
                });

                const response = await fetch(`${urls.obtenerPatente}?${params}`, {
                    method: 'GET',
                    headers: CargaDeCupoAPI.config.defaultHeaders
                });

                return await CargaDeCupoAPI.config.handleResponse(response);
            } catch (error) {
                return CargaDeCupoAPI.config.handleError(error);
            }
        },

        obtenerMateriales: async (esGrano) => {
            try {
                const urls = CargaDeCupoAPI.urls.get();
                if (!urls.obtenerMaterial) throw new Error('URL obtener material no configurada');

                const params = new URLSearchParams({ esGrano: esGrano });

                const response = await fetch(`${urls.obtenerMaterial}?${params}`, {
                    method: 'GET',
                    headers: CargaDeCupoAPI.config.defaultHeaders
                });

                return await CargaDeCupoAPI.config.handleResponse(response);
            } catch (error) {
                return CargaDeCupoAPI.config.handleError(error);
            }
        },

        obtenerOrdenesFasonInsumos: async (patente) => {
            try {
                const urls = CargaDeCupoAPI.urls.get();
                if (!urls.obtenerOrdenesInsumos) throw new Error('URL obtener órdenes insumos no configurada');

                const params = new URLSearchParams({ patente: patente });

                const response = await fetch(`${urls.obtenerOrdenesInsumos}?${params}`, {
                    method: 'GET',
                    headers: CargaDeCupoAPI.config.defaultHeaders
                });

                return await CargaDeCupoAPI.config.handleResponse(response);
            } catch (error) {
                return CargaDeCupoAPI.config.handleError(error);
            }
        },

        obtenerCPE: async (numeroCtg, tarjeta, esEspecial) => {
            try {
                const urls = CargaDeCupoAPI.urls.get();
                if (!urls.obtenerCpe) throw new Error('URL obtener CPE no configurada');

                const params = new URLSearchParams({
                    numeroCtg: numeroCtg,
                    tarjeta: tarjeta,
                    esEpecial: esEspecial
                });

                const response = await fetch(`${urls.obtenerCpe}?${params}`, {
                    method: 'GET',
                    headers: CargaDeCupoAPI.config.defaultHeaders
                });

                return await CargaDeCupoAPI.config.handleResponse(response);
            } catch (error) {
                return CargaDeCupoAPI.config.handleError(error);
            }
        },

        obtenerCPEPorPatente: async (patente, tarjeta, esEspecial, materialId) => {
            try {
                const urls = CargaDeCupoAPI.urls.get();
                if (!urls.obtenerCpePorPatente) throw new Error('URL obtener CPE por patente no configurada');

                const params = new URLSearchParams({
                    patente: patente,
                    tarjeta: tarjeta,
                    esEpecial: esEspecial,
                    materialId: materialId
                });

                const response = await fetch(`${urls.obtenerCpePorPatente}?${params}`, {
                    method: 'GET',
                    headers: CargaDeCupoAPI.config.defaultHeaders
                });

                return await CargaDeCupoAPI.config.handleResponse(response);
            } catch (error) {
                return CargaDeCupoAPI.config.handleError(error);
            }
        },

        validarCupoSap: async (cupo, imagen, nroCartaPorte) => {
            try {
                const urls = CargaDeCupoAPI.urls.get();
                if (!urls.validarCupoSap) throw new Error('URL validar cupo SAP no configurada');

                const requestData = {
                    cupo: cupo,
                    imagen: imagen,
                    nroCartaPorte: nroCartaPorte
                };

                const response = await fetch(urls.validarCupoSap, {
                    method: 'POST',
                    headers: CargaDeCupoAPI.config.defaultHeaders,
                    body: JSON.stringify(requestData)
                });

                return await CargaDeCupoAPI.config.handleResponse(response);
            } catch (error) {
                return CargaDeCupoAPI.config.handleError(error);
            }
        }
    }
};

window.CargaDeCupoAPI = CargaDeCupoAPI;