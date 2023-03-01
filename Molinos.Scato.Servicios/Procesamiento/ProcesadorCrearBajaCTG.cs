using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearBajaCTG : ProcesadorComando<CrearBajaCTG>
    {
        public ProcesadorCrearBajaCTG(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearBajaCTG comando)
        {
            var resultado = new Resultado();
            try
            {
                var baja = Repositorio.Obtener<BajaCTG>(x => x.WorkflowId == comando.Dto.WorkflowId);
                if (baja == null)
                {
                    baja = Conversor.Convertir<BajaCTGDto, BajaCTG>(comando.Dto);
                    baja.CartaPorte = Repositorio.Obtener<CartaPorte>(x => x.Id == comando.Dto.CartaPorteId);
                    baja.OrdenDeDescargaFason = Repositorio.Obtener<OrdenDeDescargaFason>(x => x.Id == comando.Dto.OrdenDeDescargaFasonId);
                    Repositorio.Agregar(baja);
                }
                else
                {
                    baja.CodigoDeBaja = comando.Dto.CodigoDeBaja;
                    baja.Fecha = comando.Dto.Fecha;
                }
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al crear la baja CTG para el workflow {0}", comando.Dto.WorkflowId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}