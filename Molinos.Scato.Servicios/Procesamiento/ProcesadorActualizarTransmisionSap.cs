using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarTransmisionSap : ProcesadorComando<ActualizarTransmision>
    {
        public ProcesadorActualizarTransmisionSap(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarTransmision comando)
        {
            var resultado = new Resultado();
            try
            {
                var transmisionAModificar = Repositorio.ObtenerConsultaEscalar(new ObtenerTransmisionASap(comando.Dto.Id));
                resultado = transmisionAModificar.SetearPropiedades(comando.Dto.Campos);
                Repositorio.GuardarCambios();
                return resultado;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al guardar la transmision editada {0}", comando.Dto.Id);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
