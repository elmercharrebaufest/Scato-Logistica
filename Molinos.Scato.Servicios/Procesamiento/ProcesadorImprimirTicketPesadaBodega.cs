using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServicioImpresion;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorImprimirTicketPesadaBodega : ProcesadorImpresionAsync<ImprimirTicketPesadaBodega>
    {
        private readonly IFirmaProvider firmaProvider;

        public ProcesadorImprimirTicketPesadaBodega(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresorFactory servicioImpresion)
            : base(repositorio, conversor, log, servicioImpresion)
        {
            this.firmaProvider = firmaProvider;
        }

        protected override void EjecutarAsync(ImprimirTicketPesadaBodega comando, IServicioImpresion servicioImpresor)
        {
            try
            {
                var firma = firmaProvider.ObtenerFirmaSinLogo();
                comando.Firma = firma;
                servicioImpresor.Ejecutar(comando);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Dto.Impresora);
                throw;
            }
        }

        protected override int EjecutarSync(ImprimirTicketPesadaBodega comando)
        {
            try
                {
                    var entidad = Conversor.Convertir<ImpTicketPesadaBodegaDto, ImpTicketPesadaBodega>(comando.Dto);
                    var caracteristicasCalado = entidad.RubrosCalados.Select(x => x.Id);
                    if (entidad.RubrosCalados.Count > 0)
                    {
                        entidad.RubrosCalados = Repositorio.Listar<CaladoPorCaracteristica>(x => caracteristicasCalado.Count(y => x.Id == y) > 0);
                    }
                entidad.FechaImpresion = DateTime.Now;
                entidad.TipoImpresion = TipoImpresion.TicketPesadaBodega;
                entidad.Codigo = comando.Dto.Codigo;
                Repositorio.Agregar(entidad);
                Repositorio.GuardarCambios();
                return entidad.Id;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al guardar ImpTicketPesadaBodega ");
                throw;
            }
        }
    }
}
