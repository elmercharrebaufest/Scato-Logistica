using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorLiberarStock : ProcesadorComando<LiberarStock>
    {
        public ProcesadorLiberarStock(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(LiberarStock comando)
        {
            var resultado = new Resultado();

            var stock = Repositorio.Obtener<Stock>(x => x.Recorrido.InstanciaWorkflow == comando.InstanceId);
            Repositorio.Remover(stock);
            return resultado;
        }
    }
}