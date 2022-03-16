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
    public class ProcesadorModificarCaracteristicaDeCalidad : ProcesadorModificar<ModificarCaracteristicaDeCalidad>
    {
        public ProcesadorModificarCaracteristicaDeCalidad(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCaracteristicaDeCalidad comando)
        {
            var caracteristicaDeCalidad = Repositorio.Obtener<CaracteristicaDeCalidad>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, caracteristicaDeCalidad);
            if (comando.DescuentosBorrados != null)
            {
                foreach (var idBorrado in comando.DescuentosBorrados)
                {
                    Repositorio.Remover<Descuento>(idBorrado);
                }
            }

            if (comando.Dto.DescuentosDto != null)
            {
                var descuentos = (from descuento in comando.Dto.DescuentosDto
                                  where !descuento._destroy
                                  select Conversor.Convertir<DescuentoDto, Descuento>(descuento)).ToList();
                foreach (var descuento in descuentos)
                {
                    descuento.CaracteristicaDeCalidad = caracteristicaDeCalidad;
                    caracteristicaDeCalidad.Descuentos.Add(descuento);
                }
            }
            if (caracteristicaDeCalidad.MaterialPorCentro.Material.Id != comando.Dto.MaterialId)
            {
                caracteristicaDeCalidad.MaterialPorCentro = Repositorio.Obtener<MaterialPorCentro>(x => x.Material.Id == comando.Dto.MaterialId & x.Centro.Id == comando.Dto.CentroId);
            }

            var existeCaracteristica = Repositorio.Existe<CaracteristicaDeCalidadMaestro>(c => c.Descripcion == comando.Dto.Descripcion);
            if (!existeCaracteristica)
            {
                var caracteristicaDeCalidadMaestro = new CaracteristicaDeCalidadMaestro{ Descripcion = comando.Dto.Descripcion };
                Repositorio.Agregar(caracteristicaDeCalidadMaestro);
                caracteristicaDeCalidad.CaracteristicaDeCalidadMaestro = caracteristicaDeCalidadMaestro;
            }
            else
            {
                caracteristicaDeCalidad.CaracteristicaDeCalidadMaestro = Repositorio.Obtener<CaracteristicaDeCalidadMaestro>(c => c.Descripcion == comando.Dto.Descripcion);
            }
        }

        protected override void Validar(ModificarCaracteristicaDeCalidad comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Material>(e => e.Id == comando.Dto.MaterialId))
            {
                resultado.Error("MaterialId", string.Format(Textos.Error_Requerido, Textos.Material));
            }
            var cantidad = 0;
            if (comando.Dto.EsHumedad)
            {
                cantidad++;
                if (Repositorio.Existe<CaracteristicaDeCalidad>(e => e.Id != comando.Dto.Id && e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsHumedad))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExisteHumedad);
                }
            }
            if (comando.Dto.EsCalidadUva)
            {
                cantidad++;
                if (Repositorio.Existe<CaracteristicaDeCalidad>(e => e.Id != comando.Dto.Id && e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsCalidadUva))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExisteCalidadUva);
                }
            }
            if (comando.Dto.EsEstadoSanitario)
            {
                cantidad++;
                if (Repositorio.Existe<CaracteristicaDeCalidad>(e => e.Id != comando.Dto.Id && e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsEstadoSanitario))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExisteEstadoSanitario);
                }
            }
            if (comando.Dto.EsTenorAzucarino)
            {
                cantidad++;
                if (Repositorio.Existe<CaracteristicaDeCalidad>(e => e.Id != comando.Dto.Id && e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsTenorAzucarino))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExisteTenorAzucarino);
                }
            }
            if (comando.Dto.EsCuerposExtranos)
            {
                cantidad++;
                if (Repositorio.Existe<CaracteristicaDeCalidad>(e => e.Id != comando.Dto.Id && e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsCuerposExtranos))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExisteCuerposExtraños);
                }
            }
            if (comando.Dto.EsGranosVerdes)
            {
                cantidad++;
                if (Repositorio.Existe<CaracteristicaDeCalidad>(e => e.Id != comando.Dto.Id && e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsGranosVerdes))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExisteGranosVerdes);
                }
            }
            if (comando.Dto.EsGranosDañados)
            {
                cantidad++;
                if (Repositorio.Existe<CaracteristicaDeCalidad>(e => e.Id != comando.Dto.Id && e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsGranosDañados))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExisteGranosDañados);
                }
            }

            if (comando.Dto.EsPesoHectolitrico)
            {
                cantidad++;
                if (Repositorio.Existe<CaracteristicaDeCalidad>(e => e.Id != comando.Dto.Id && e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId && e.EsPesoHectolitrico))
                {
                    resultado.Error("TipoCaracteristica", Textos.ExistePesoHectolitrico);
                }
            }
            if (cantidad > 1)
            {
                resultado.Error("SoloUno", Textos.Error_SoloUno);
            }

            if (Repositorio.Existe<CaracteristicaDeCalidad>(e => e.Id != comando.Dto.Id && e.CaracteristicaDeCalidadMaestro.Descripcion == comando.Dto.Descripcion && e.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.MaterialPorCentro.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("Descripcion", Textos.CaracteristicaDeCalidad_ExisteDescripcion);
            }
        }
    }
}
