using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearExcepcionPagoTasaMunicipal : ProcesadorCrear<CrearExcepcionPagoTasaMunicipal, ExceptuadosTicketMunicipal>
    {
        public ProcesadorCrearExcepcionPagoTasaMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log) 
            : base(repositorio, conversor, log)
        {
        }

        protected override ExceptuadosTicketMunicipal CrearEntidad(CrearExcepcionPagoTasaMunicipal comando)
        {
            return new ExceptuadosTicketMunicipal
            {
                Patente = comando.ExceptuadosTicketMunicipalDto.Patente,
                NombreUsuario = comando.ExceptuadosTicketMunicipalDto.NombreUsuario,
                FechaCreacionExcepcion = DateTime.Now,
                WorkflowInstanceId = null
            };
        }

        protected override void Validar(CrearExcepcionPagoTasaMunicipal comando, Resultado resultado)
        {
            // Ver que al momento de crear una nueva excepción no exista ya otra excepción activa para la patente.
            bool existe =
                this.Repositorio.Existe<ExceptuadosTicketMunicipal>(e =>
                    e.Patente == comando.ExceptuadosTicketMunicipalDto.Patente &&
                    !e.WorkflowInstanceId.HasValue);

            if (existe)
                throw new CrearException("Ya existe una excepción de pago de tasa municipal activa para la patente indicada.");
        }
    }
}
