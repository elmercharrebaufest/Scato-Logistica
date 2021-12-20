using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearExcepcionAlDescuento : ProcesadorCrear<CrearExcepcionAlDescuento, ExcepcionAlDescuento>
    {
        public ProcesadorCrearExcepcionAlDescuento(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ExcepcionAlDescuento CrearEntidad(CrearExcepcionAlDescuento comando)
        {
            return new ExcepcionAlDescuento
            {
                Proveedor = Repositorio.Obtener<Proveedor>(x => x.Id == comando.Dto.ProveedorId),
                FechaDesde = comando.Dto.FechaDesde,
                FechaHasta = comando.Dto.FechaHasta,
                CaracteristicaDeCalidad = Repositorio.Obtener<CaracteristicaDeCalidad>(x => x.Id == comando.Dto.CaracteristicaDeCalidadId),
                Camara = Repositorio.Obtener<Camara>(x => x.Id == comando.Dto.CamaraId),
                Motivo = comando.Dto.Motivo,
                Usuario = comando.Dto.Usuario
            };
        }

        protected override void Validar(CrearExcepcionAlDescuento comando, Resultado resultado)
        {

        }
    }
}
