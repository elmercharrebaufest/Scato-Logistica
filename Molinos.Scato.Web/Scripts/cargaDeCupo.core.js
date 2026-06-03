const CargaDeCupoCore = {
    CONSTANTS: {
        PATENTE_NO_RECONOCIDA: 'Patente no reconocida',
        CUPO_GENERICO: 'MOL1111/11111111',
        CONSULTAR_FOTO_REFRESH_INTERVAL: 3000,
        LISTAR_CAMIONES_REFRESH_INTERVAL: 4000,
        REGEX: {
            PATENTE: /^[A-Z]{3}[0-9]{3}$|^[A-Z]{2}[0-9]{3}[A-Z]{2}$/,
            CUPO: /^MOL[0-9]{4}\/[0-9]{8}$/
        },
        CTG_LENGTH: {
            MIN: 11,
            MAX: 12
        }
    },

    state: {
        errorPatente: false,
        iniciarLoopFotoPatenteActivo: false,
        puestoDeTrabajo: null,
        patenteGuardada: null,
        patenteSetByCamara: ''
    },

    validation: {
        isValidPatenteFormat: (patente) => {
            return CargaDeCupoCore.CONSTANTS.REGEX.PATENTE.test(patente);
        },

        isValidCupoFormat: (cupo) => {
            return cupo !== CargaDeCupoCore.CONSTANTS.CUPO_GENERICO && CargaDeCupoCore.CONSTANTS.REGEX.CUPO.test(cupo);
        },

        isValidCtgLength: (ctg) => {
            return ctg.length >= CargaDeCupoCore.CONSTANTS.CTG_LENGTH.MIN && ctg.length <= CargaDeCupoCore.CONSTANTS.CTG_LENGTH.MAX;
        }
    },

    formatting: {
        truncateCupoLength: (cupo) => {
            try {
                if (cupo.length <= 16) return cupo;

                const parts = cupo.split("/");
                if (parts.length === 0) return cupo;

                let prefix = parts[0] || '';
                let date = parts[1] || '';

                if (prefix.length > 7) {
                    prefix = prefix.substring(0, 7);
                }

                if (date.length > 8) {
                    date = date.substring(0, 8);
                }

                const result = `${prefix}/${date}`;
                return result.length > 16 ? result.substring(0, 16) : result;
            } catch (error) {
                console.error('Error validating cupo length:', error);
                return '';
            }
        },
    },

    date: {
        /**
         * Obtiene fecha actual en formato DDMMYYYY
         */
        getCurrentDate: () => {
            const today = new Date();
            const day = String(today.getDate()).padStart(2, '0');
            const month = String(today.getMonth() + 1).padStart(2, '0');
            const year = today.getFullYear();
            return `${day}${month}${year}`;
        },
    },
};

window.CargaDeCupoCore = CargaDeCupoCore;
