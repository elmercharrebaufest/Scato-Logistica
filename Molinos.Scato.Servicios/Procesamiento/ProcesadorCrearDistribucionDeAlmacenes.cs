using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearDistribucionDeAlmacenes : ProcesadorCrear<CrearDistribucionDeAlmacenes, DistribucionDeAlmacenes>
    {
        public ProcesadorCrearDistribucionDeAlmacenes(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override DistribucionDeAlmacenes CrearEntidad(CrearDistribucionDeAlmacenes comando)
        {
            var entidad = Conversor.Convertir<DistribucionDeAlmacenesDto, DistribucionDeAlmacenes>(comando.Dto);
            entidad.Recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.Dto.InstanceId);
            entidad.DistribucionesDeAlmacenes.Clear();
            foreach (var distribucionDeAlmacen in comando.Dto.DistribucionesDeAlmacenes)
            {
                entidad.DistribucionesDeAlmacenes.Add(new DistribucionDeAlmacen
                    {
                        Almacen = Repositorio.Obtener<Almacen>(distribucionDeAlmacen.AlmacenId),
                        DistribucionDeAlmacenes = entidad,
                        Litros = distribucionDeAlmacen.Litros
                    });
            }
            return entidad;
        }

        protected override void Validar(CrearDistribucionDeAlmacenes comando, Resultado resultado)
        {
            if (comando.Dto.PesoNetoBodegaEnLitros != comando.Dto.DistribucionesDeAlmacenes.Sum(x => x.Litros))
            {
                resultado.Error("DistribucionesDeAlmacenes", Textos.Error_LitrosInvalidos);
            }
            if (comando.Dto.DistribucionesDeAlmacenes.Any(y => !Repositorio.Existe<Almacen>(x => x.Id == y.AlmacenId)))
            {
                resultado.Error("", Textos.Error_Generico);
            }
            else if (!Repositorio.Existe<Recorrido>(x => x.InstanciaWorkflow == comando.Dto.InstanceId))
            {
                resultado.Error("", Textos.Error_Generico);
            }
        }
    }
}
