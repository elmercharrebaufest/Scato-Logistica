using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCaladoEnPlanta : ProcesadorCrear<CrearCaladoEnPlanta, CaladoEnPlanta>
    {
        private ICalculadoraDescuento calculadora;
        public ProcesadorCrearCaladoEnPlanta(IRepositorio repositorio, IConversor conversor, ILogger log, ICalculadoraDescuento calculadora)
            : base(repositorio, conversor, log)
        {
            this.calculadora = calculadora;
        }

        protected override CaladoEnPlanta CrearEntidad(CrearCaladoEnPlanta comando)
        {
            var resultado = new ResultadoCrear();
            Log.Info("Se procederá a ejecutar ProcesadorCrearCaladoEnPlanta para la instancia de workflow = {0}", comando.CaladoEnPlanta.WorkflowInstanceId);
            var entidad = Conversor.Convertir<CaladoEnPlantaDto, CaladoEnPlanta>(comando.CaladoEnPlanta);
            var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.CaladoEnPlanta.WorkflowInstanceId);
            recorrido.CaladoEnPlanta = entidad;


            entidad.CaladosEnPlantaPorCaracteristica = new List<CaladoEnPlantaPorCaracteristica>();
            comando.CaladoEnPlanta.CaladosPorCaracteristica.ToList().ForEach(f => entidad.CaladosEnPlantaPorCaracteristica.Add(new CaladoEnPlantaPorCaracteristica
            {
                CaladoEnPlanta = entidad,
                CaracteristicaDeCalidad = Repositorio.Obtener<CaracteristicaDeCalidad>(f.CaracteristicaId),
                Rango = f.Rango,
                Unidad = f.Unidad,
                ValorCaladoEnPlanta = f.ValorCaladoEnPlanta,
                ValorCalado = f.ValorCalado
            }));
            Log.Info("Se creó exitosamente el CaladoEnPlanta para la instancia de workflow {0}", comando.CaladoEnPlanta.WorkflowInstanceId);
            return entidad;
        }

        protected override void Validar(CrearCaladoEnPlanta comando, Resultado resultado)
        {
        }
    }
}
