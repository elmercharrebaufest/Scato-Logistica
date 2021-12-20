using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCalidadMaterial : ProcesadorCrear<CrearCalidadMaterial, CalidadMaterial>
    {
        public ProcesadorCrearCalidadMaterial(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override CalidadMaterial CrearEntidad(CrearCalidadMaterial comando)
        {
            return new CalidadMaterial
            {
                MaterialPorCentro = Repositorio.Obtener<MaterialPorCentro>(x => x.Material.Id == comando.Dto.MaterialId && x.Centro.Id == comando.Dto.CentroId),
                TieneAnalisis = comando.Dto.TieneAnalisis,
                Descripcion = comando.Dto.Descripcion,
                ValorDesde = comando.Dto.ValorDesde ?? 0,
                ValorHasta = comando.Dto.ValorHasta ?? 0
            };
        }

        protected override void Validar(CrearCalidadMaterial comando, Resultado resultado)
        {
            if (comando.Dto.MaterialId == 0)
            {
                resultado.Error("Material", String.Format(Textos.Error_Requerido, Textos.Material));
            }
            if (comando.Dto.MaterialId != 0 && !Repositorio.Existe<MaterialPorCentro>(x => x.Material.Id == comando.Dto.MaterialId && x.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("Material", Textos.Error_Invalido);
            }
            if (comando.Dto.ValorDesde > comando.Dto.ValorHasta)
            {
                resultado.Error("ValorDesde", Textos.CalidadMaterial_ErrorRango);
            }
            else if (Repositorio.Existe<CalidadMaterial>(x => x.MaterialPorCentro.Id == comando.Dto.MaterialId && x.TieneAnalisis == comando.Dto.TieneAnalisis && (comando.Dto.ValorDesde <= x.ValorHasta && x.ValorDesde < comando.Dto.ValorHasta)))
            {
                resultado.Error("", Textos.CalidadMaterial_RangoExistente);
            }
            else if (Repositorio.Existe<CalidadMaterial>(x => x.MaterialPorCentro.Id == comando.Dto.MaterialId && x.TieneAnalisis == comando.Dto.TieneAnalisis && (comando.Dto.ValorHasta == x.ValorHasta && x.ValorDesde == comando.Dto.ValorDesde)))
            {
                resultado.Error("", Textos.CalidadMaterial_RangoExistente);
            }
        }
    }
}
