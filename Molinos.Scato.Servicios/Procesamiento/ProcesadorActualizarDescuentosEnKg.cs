using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarDescuentosEnKg : ProcesadorComando<ActualizarDescuentosEnKg>
    {
        private readonly ICalculadoraDescuento calculadora;

        public ProcesadorActualizarDescuentosEnKg(IRepositorio repositorio, IConversor conversor, ILogger log, ICalculadoraDescuento calculadora)
            : base(repositorio, conversor, log)
        {
            this.calculadora = calculadora;
        }

        public override Resultado Ejecutar(ActualizarDescuentosEnKg comando)
        {
            var resultado = new Resultado();
            var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);

            if (recorrido.AnalisisDeCalidad != null)
            {
                foreach (var caracteristica in recorrido.AnalisisDeCalidad.CaracteristicasAnalizadas)
                {
                    if (caracteristica.DescuentoEnPorcentaje.HasValue)
                    {
                        caracteristica.DescuentoEnKg =
                            (caracteristica.DescuentoEnPorcentaje.Value * comando.PesoNeto) / 100;
                    }
                }
            }
            if (recorrido.Calado != null)
            {
                foreach (var caracteristica in recorrido.Calado.CaladosPorCaracteristica)
                {
                    if (caracteristica.DescuentoEnPorcentaje.HasValue)
                    {
                        caracteristica.DescuentoEnKg =
                            (caracteristica.DescuentoEnPorcentaje.Value * comando.PesoNeto) / 100;
                    }
                }
            }

            var descuentoPorMerma = calculadora.ActualizarMermaVolatil(recorrido.Calado,recorrido.AnalisisDeCalidad,comando.PesoNeto);
            recorrido.DescuentoEnKgOncca = descuentoPorMerma;

            Repositorio.GuardarCambios();
            return resultado;
        }
    }
}
