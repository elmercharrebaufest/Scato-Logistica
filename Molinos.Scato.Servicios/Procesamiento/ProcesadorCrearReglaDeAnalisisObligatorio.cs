using System;
using System.Linq;
using System.Transactions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearReglaDeAnalisisObligatorio : ProcesadorCrear<CrearReglaDeAnalisisObligatorio, ReglaDeAnalisisObligatorio>
    {
        public ProcesadorCrearReglaDeAnalisisObligatorio(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ReglaDeAnalisisObligatorio CrearEntidad(CrearReglaDeAnalisisObligatorio comando)
        {
            var entidad = Conversor.Convertir<ReglaDeAnalisisObligatorioDto, ReglaDeAnalisisObligatorio>(comando.Dto);
            entidad.FechaDeVigenciaHasta = DateTime.Now.AddHours(24);
            entidad.Localidad = Repositorio.Obtener<Localidad>(comando.Dto.LocalidadId);
            entidad.Provincia = Repositorio.Obtener<Provincia>(comando.Dto.ProvinciaId);
            entidad.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            entidad.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            return entidad;
        }

        protected override void Validar(CrearReglaDeAnalisisObligatorio comando, Resultado resultado)
        {
            if (comando.Dto.LocalidadId == 0 && comando.Dto.ProvinciaId == 0)
            {
                resultado.Error("LocalidadProvincia", "Debe ingresar localidad y/o provincia");
            }
            if (comando.Dto.MaterialId == 0)
            {
                resultado.Error("MaterialId", "El material es obligatorio");
            }
        }
    }
}