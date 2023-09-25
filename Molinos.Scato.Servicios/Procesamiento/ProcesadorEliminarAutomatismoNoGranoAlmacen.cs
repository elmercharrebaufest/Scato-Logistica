using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarAutomatismoNoGranoAlmacen : ProcesadorComando<EliminarAutomatismoNoGranoAlmacen>
    {
        public ProcesadorEliminarAutomatismoNoGranoAlmacen(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(EliminarAutomatismoNoGranoAlmacen comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Info("Se está ejecutando ProcesadorEliminarAutomatismoNoGranoAlmacen con AutomatismoNoGranoId = {0} y AlmacenId = {1}", comando.Dto.AutomatismoNoGranoId, comando.Dto.AlmacenId);
                var automatismo = Repositorio.Obtener<AutomatismoNoGrano>(comando.Dto.AutomatismoNoGranoId);
                var almacen = Repositorio.Obtener<Almacen>(comando.Dto.AlmacenId);

                if (automatismo.AlmacenesAsociados.Contains(almacen))
                {
                    automatismo.AlmacenesAsociados.Remove(almacen);
                    Repositorio.GuardarCambios();
                }
                else
                {
                    resultado.Error("", Textos.Error_Generico);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorEliminarAutomatismoNoGranoAlmacen con AutomatismoNoGranoId = {0} y AlmacenId = {1}", comando.Dto.AutomatismoNoGranoId, comando.Dto.AlmacenId);
                resultado.Error("", e.Message);
            }

            return resultado;
        }
    }
}

