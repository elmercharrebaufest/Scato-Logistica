using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarAlmacenPorMaterial : ProcesadorComando<EliminarAlmacenPorMaterial>
    {
        public ProcesadorEliminarAlmacenPorMaterial(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(EliminarAlmacenPorMaterial comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Info("Se está ejecutando ProcesadorEliminarAlmacenPorMaterial con MaterialId = {0} y AlmacenId = {1}", comando.Dto.MaterialId, comando.Dto.AlmacenId);
                var material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
                var almacen = Repositorio.Obtener<Almacen>(comando.Dto.AlmacenId);

                if (material.Almacenes.Contains(almacen))
                {
                    material.Almacenes.Remove(almacen);
                    Repositorio.GuardarCambios();
                }
                else
                {
                    resultado.Error("", Textos.Error_Generico);
                }               
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorEliminarAlmacenPorMaterial con MaterialId = {0} y AlmacenId = {1}", comando.Dto.MaterialId, comando.Dto.AlmacenId);
                resultado.Error("", e.Message);
            }

            return resultado;
        }
    }
}
