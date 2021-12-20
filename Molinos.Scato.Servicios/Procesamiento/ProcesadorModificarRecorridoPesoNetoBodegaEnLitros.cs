using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoPesoNetoBodegaEnLitros : ProcesadorComando<ModificarRecorridoPesoNetoBodegaEnLitros>
    {
        public ProcesadorModificarRecorridoPesoNetoBodegaEnLitros(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarRecorridoPesoNetoBodegaEnLitros comando)
        {
            var resultado = new ResultadoModificarRecorridoPesoNetoBodegaEnLitros();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorModificarRecorridoPesoNetoBodegaEnLitros con WorkflowId = {0}, PesoNetoBodega = {1} y MaterialId = {2}", comando.WorkflowId, comando.PesoNetoBodega, comando.MaterialId);
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.WorkflowId);

                if (!recorridoAEditar.Material.FactorConversion.HasValue)
                {
                    Log.Error("El material MaterialId = {0} no posee factor de conversion",comando.MaterialId);
                    resultado.Error("", String.Format(Textos.Error_Requerido, "FactorDeConversion"));
                    return resultado;
                }

                recorridoAEditar.PesoNetoBodegaEnLitros = Convert.ToInt32(recorridoAEditar.Material.FactorConversion.Value * comando.PesoNetoBodega);
                Repositorio.GuardarCambios();
                resultado.PesoNetoBodegaEnLitros = recorridoAEditar.PesoNetoBodegaEnLitros.Value;
            }
            catch (Exception e)
            {
                Log.Error(e, "Ha ocurrido un error en ProcesadorModificarRecorridoPesoNetoBodegaEnLitros con WorkflowId = {0}, PesoNetoBodega = {1} y MaterialId = {2}", comando.WorkflowId, comando.PesoNetoBodega, comando.MaterialId);
                resultado.Error("", Textos.Error_Generico);
            }
            return resultado;
        }
    }
}
