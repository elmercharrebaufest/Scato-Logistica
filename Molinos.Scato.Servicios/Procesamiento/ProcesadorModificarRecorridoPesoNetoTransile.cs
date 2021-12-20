using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoPesoNetoTransile : ProcesadorComando<ModificarRecorridoPesoNetoTransile>
    {
        public ProcesadorModificarRecorridoPesoNetoTransile(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }
        public override Resultado Ejecutar(ModificarRecorridoPesoNetoTransile comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
                recorridoAEditar.PesoNetoTransile = recorridoAEditar.PesoNetoTransile.HasValue
                                                        ? recorridoAEditar.PesoNetoTransile
                                                        : 0;
                recorridoAEditar.PesoNetoTransile += comando.PesoAgregado;
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al modificar el peso neto transile del recorrido {0}", comando.InstanceId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
