using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoBalanzaBruto : ProcesadorComando<ModificarRecorridoBalanzaBruto>
    {
        public ProcesadorModificarRecorridoBalanzaBruto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarRecorridoBalanzaBruto comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
                recorridoAEditar.BalanzaBruto = null;
                
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Ha ocurrido un error en ProcesadorModificarRecorridoBalanzaBruto con RecorridoId = {0}", comando.InstanceId);
                resultado.Error("", Textos.Error_Generico);
            }
            return resultado;
        }
    }
}
