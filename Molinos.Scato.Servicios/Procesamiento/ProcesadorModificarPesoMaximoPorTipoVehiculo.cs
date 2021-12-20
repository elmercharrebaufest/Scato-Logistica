using System;
using System.Linq;
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
    public class ProcesadorModificarPesoMaximoPorTipoVehiculo : ProcesadorModificar<ModificarPesoMaximoPorTipoVehiculo>
    {
        public ProcesadorModificarPesoMaximoPorTipoVehiculo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarPesoMaximoPorTipoVehiculo comando)
        {
            var tipo = Repositorio.Obtener<PesoMaximoPorTipoVehiculo>(comando.Dto.Id);

            tipo.TipoVehiculo = comando.Dto.TipoVehiculo;
            tipo.PesoMaxIngreso = comando.Dto.PesoMaxIngreso;
            tipo.PesoMaxEgreso = comando.Dto.PesoMaxEgreso;
            tipo.PesoNetoMaxPlanta = comando.Dto.PesoNetoMaxPlanta;
            tipo.PesoNetoMinimo = comando.Dto.PesoNetoMinimo;
            tipo.Activo = comando.Dto.Activo;
            
        }

        protected override void Validar(ModificarPesoMaximoPorTipoVehiculo comando, Resultado resultado)
        {
            if (Repositorio.Existe<PesoMaximoPorTipoVehiculo>(e => e.TipoVehiculo == comando.Dto.TipoVehiculo && (e.Id != comando.Dto.Id) && e.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("", Textos.PesoMaximoPorTipoVehiculo_Existente);
            }
        }
    }
}
