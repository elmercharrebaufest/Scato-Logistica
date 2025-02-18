using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCaracteristicasAnalizadas : ProcesadorCrear<CrearCaracteristicasAnalizadas, CaracteristicasAnalizadas>
    {
        public ProcesadorCrearCaracteristicasAnalizadas(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override CaracteristicasAnalizadas CrearEntidad(CrearCaracteristicasAnalizadas comando)
        {
            var caracteristica = Repositorio.Obtener<CaracteristicasAnalizadas>(o => o.Recorrido.Id == comando.IdRecorridoIngreso);
            caracteristica.Recorrido = Repositorio.Obtener<Recorrido>(comando.IdRecorridoEgreso);
       
            return caracteristica;
        }

        protected override void Validar(CrearCaracteristicasAnalizadas comando, Resultado resultado)
        {
            if (Repositorio.Existe<CaracteristicasAnalizadas>(x => x.Recorrido.Id == comando.IdRecorridoEgreso))
            {
                resultado.Error("IdRecorrido", "Caracteristicas analizadas con el mismo recorrido");
            }
        }
    }
}