using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAsignacionDeRecorrido : ProcesadorCrear<CrearAsignacionDeRecorrido, AsignacionDeRecorrido>
    {
        public ProcesadorCrearAsignacionDeRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override AsignacionDeRecorrido CrearEntidad(CrearAsignacionDeRecorrido comando)
        {
            return new AsignacionDeRecorrido
                {
                    FechaDesde = comando.Dto.FechaDesde,
                    FechaHasta = comando.Dto.FechaHasta,
                    MaterialPorCentro =
                        Repositorio.Obtener<MaterialPorCentro>(comando.Dto.MaterialPorCentroId),
                    Calidad = Repositorio.Obtener<CalidadMaterial>(comando.Dto.CalidadId),
                    Calle = Repositorio.Obtener<Calle>(comando.Dto.CalleId),
                    BalanzaBruto = Repositorio.Obtener<Balanza>(comando.Dto.BalanzaBrutoId ?? 0),
                    BalanzaTara = Repositorio.Obtener<Balanza>(comando.Dto.BalanzaTaraId ?? 0),
                    AlmacenDestino = Repositorio.Obtener<Almacen>(comando.Dto.AlmacenDestinoId),
                    Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId),
                    Workflow = Repositorio.Obtener<Workflow>(comando.Dto.WorkflowId),
                    PuestosDeCargaDescargas = Repositorio.Listar<PuestosDeCargaDescarga>(x => comando.Dto.HidraulicasId.Contains(x.Id))
                };
        }

        protected override void Validar(CrearAsignacionDeRecorrido comando, Resultado resultado)
        {
            if (comando.Dto.FechaDesde > comando.Dto.FechaHasta)
            {
                resultado.Error("FechaHasta", Textos.Error_FechasDesdeHasta);
            }
            if (Repositorio.Existe<AsignacionDeRecorrido>(x => x.Centro.Id == comando.Dto.CentroId
                && x.MaterialPorCentro.Id == comando.Dto.MaterialPorCentroId
                && x.Calidad.Id == comando.Dto.CalidadId
                && x.Id != comando.Dto.Id
                && ((comando.Dto.FechaDesde > x.FechaDesde && comando.Dto.FechaDesde < x.FechaHasta)
                    || (comando.Dto.FechaHasta > x.FechaDesde && comando.Dto.FechaHasta < x.FechaHasta))))
            {
                resultado.Error("FechaDesde", Textos.Error_PeriodoSolapado);
            }
            if (comando.Dto.MaterialPorCentroId != 0 && !Repositorio.Existe<MaterialPorCentro>(x => x.Id == comando.Dto.MaterialPorCentroId))
            {
                resultado.Error("MaterialPorCentroId", Textos.Error_Invalido);
            }
            if (comando.Dto.CalidadId != 0 && !Repositorio.Existe<CalidadMaterial>(x => x.Id == comando.Dto.CalidadId))
            {
                resultado.Error("CalidadId", Textos.Error_Invalido);
            }
            if (comando.Dto.CalleId != 0 && !Repositorio.Existe<Calle>(x => x.Id == comando.Dto.CalleId))
            {
                resultado.Error("CalleId", Textos.Error_Invalido);
            }
            if (comando.Dto.BalanzaBrutoId.HasValue && !Repositorio.Existe<Balanza>(x => x.Id == comando.Dto.BalanzaBrutoId))
            {
                resultado.Error("BalanzaBrutoId", Textos.Error_Invalido);
            }
            if (comando.Dto.BalanzaTaraId.HasValue && !Repositorio.Existe<Balanza>(x => x.Id == comando.Dto.BalanzaTaraId))
            {
                resultado.Error("BalanzaTaraId", Textos.Error_Invalido);
            }
            if (comando.Dto.AlmacenDestinoId != 0 && !Repositorio.Existe<Almacen>(x => x.Id == comando.Dto.AlmacenDestinoId))
            {
                resultado.Error("AlmacenDestinoId", Textos.Error_Invalido);
            }
            if (comando.Dto.CentroId != 0 && !Repositorio.Existe<Centro>(x => x.Id == comando.Dto.CentroId))
            {
                resultado.Error("CentroId", Textos.Error_Invalido);
            }
        }
    }
}