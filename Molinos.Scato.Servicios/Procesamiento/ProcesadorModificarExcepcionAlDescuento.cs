using System;
using System.Security.Cryptography.X509Certificates;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarExcepcionAlDescuento : ProcesadorModificar<ModificarExcepcionAlDescuento>
    {
        public ProcesadorModificarExcepcionAlDescuento(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarExcepcionAlDescuento comando)
        {
            var excepcionAlDescuento = Repositorio.Obtener<ExcepcionAlDescuento>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, excepcionAlDescuento);
            if ((excepcionAlDescuento.Proveedor.Id != comando.Dto.ProveedorId) || (excepcionAlDescuento.Proveedor.Descripcion != comando.Dto.ProveedorDescripcion))
            {
                excepcionAlDescuento.Proveedor =
                    Repositorio.Obtener<Proveedor>(comando.Dto.ProveedorId);
            }
            if (excepcionAlDescuento.FechaDesde != comando.Dto.FechaDesde)
            {
                excepcionAlDescuento.FechaDesde = comando.Dto.FechaDesde;
            }
            if (excepcionAlDescuento.FechaHasta != comando.Dto.FechaHasta)
            {
                excepcionAlDescuento.FechaHasta = comando.Dto.FechaHasta;
            }
            if (excepcionAlDescuento.CaracteristicaDeCalidad.Id != comando.Dto.CaracteristicaDeCalidadId)
            {
                excepcionAlDescuento.CaracteristicaDeCalidad = Repositorio.Obtener<CaracteristicaDeCalidad>(comando.Dto.CaracteristicaDeCalidadId);
            }
            if (excepcionAlDescuento.Camara.Id != comando.Dto.CamaraId)
            {
                excepcionAlDescuento.Camara = Repositorio.Obtener<Camara>(comando.Dto.CamaraId);
            }
            if (excepcionAlDescuento.Motivo != comando.Dto.Motivo)
            {
                excepcionAlDescuento.Motivo = comando.Dto.Motivo;
            }
            if (excepcionAlDescuento.Usuario != comando.Dto.Usuario)
            {
                excepcionAlDescuento.Usuario = comando.Dto.Usuario;
            }
        }

        protected override void Validar(ModificarExcepcionAlDescuento comando, Resultado resultado)
        {
        }
    }
}
