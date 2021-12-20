using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAlmacenPorMaterial : ProcesadorComando<CrearAlmacenPorMaterial>
    {
        public ProcesadorCrearAlmacenPorMaterial(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearAlmacenPorMaterial comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Info("Se está ejecutando ProcesadorCrearAlmacenPorMaterial con MaterialId = {0} y AlmacenId = {1}", comando.Dto.MaterialId, comando.Dto.AlmacenId);
                var material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
                var almacen = Repositorio.Obtener<Almacen>(comando.Dto.AlmacenId);

                if (!material.Almacenes.Contains(almacen))
                {
                    material.Almacenes.Add(almacen);
                    Repositorio.GuardarCambios();
                }
                else
                {
                    resultado.Error("", Textos.Error_Generico);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorCrearAlmacenPorMaterial con MaterialId = {0} y AlmacenId = {1}", comando.Dto.MaterialId, comando.Dto.AlmacenId);
                resultado.Error("", e.Message);
            }

            return resultado;
        }
    }
}
