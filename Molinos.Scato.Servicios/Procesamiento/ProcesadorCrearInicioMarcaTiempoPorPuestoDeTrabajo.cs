using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearInicioMarcaTiempoPorPuestoDeTrabajo : ProcesadorCrear<CrearInicioMarcaTiempoPorPuestoDeTrabajo, MarcaTiempoPorPuestoDeTrabajo>
    {
        public ProcesadorCrearInicioMarcaTiempoPorPuestoDeTrabajo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log) { }

        protected override MarcaTiempoPorPuestoDeTrabajo CrearEntidad(CrearInicioMarcaTiempoPorPuestoDeTrabajo comando)
        {
            var recorrido = BuscarRecorrido(comando);

            return new MarcaTiempoPorPuestoDeTrabajo
            {
                NumeroDocumento    = recorrido.NumeroDocumentoIngreso,
                PuestoDeTrabajoId  = comando.PuestoDeTrabajoId,
                FechaInicio        = DateTime.Now,
                Centro_Id          = comando.CentroId,
                TipoIngreso        = comando.TipoIngreso
            };
        }

        protected override void Validar(CrearInicioMarcaTiempoPorPuestoDeTrabajo comando, Resultado resultado)
        {
            var recorrido = BuscarRecorrido(comando);
            if (recorrido == null)
            {
                resultado.Error(nameof(MarcaTiempoPorPuestoDeTrabajo.NumeroDocumento), "No hay recorrido activo");
                return;
            }

            var yaExiste = Repositorio.Existe<MarcaTiempoPorPuestoDeTrabajo>(
                x => x.NumeroDocumento == recorrido.NumeroDocumentoIngreso
                  && x.PuestoDeTrabajoId == comando.PuestoDeTrabajoId);

            if (yaExiste)
                resultado.Error(nameof(MarcaTiempoPorPuestoDeTrabajo.NumeroDocumento), "Ya existe un registro pendiente para este documento y puesto");
        }

        private Recorrido BuscarRecorrido(CrearInicioMarcaTiempoPorPuestoDeTrabajo comando)
        {
            if (!string.IsNullOrEmpty(comando.NumeroDeTarjeta))
                return Repositorio.Obtener<Recorrido>(r => r.TarjetaDeAcceso == comando.NumeroDeTarjeta && !r.Terminado);

            if (!string.IsNullOrEmpty(comando.Patente))
                return Repositorio.Obtener<Recorrido>(r => r.Patente == comando.Patente && !r.Terminado);

            return null;
        }
    }
}
