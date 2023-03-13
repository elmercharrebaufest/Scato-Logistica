using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearExcepcionAlControlProveedor : ProcesadorCrear<CrearExcepcionAlControlProveedor, ExcepcionAlControlProveedor>
    {
        public ProcesadorCrearExcepcionAlControlProveedor(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override ExcepcionAlControlProveedor CrearEntidad(CrearExcepcionAlControlProveedor comando)
        {
            return new ExcepcionAlControlProveedor
            {
                Proveedor = Repositorio.Obtener<Proveedor>(x => x.Id == comando.Dto.ProveedorId),
                Material = Repositorio.Obtener<Material>(x => x.Id == comando.Dto.MaterialId),
                FechaDesde = comando.Dto.FechaDesde,
                FechaHasta = comando.Dto.FechaHasta,
                Centro = Repositorio.Obtener<Centro>(x => x.Id == comando.Dto.CentroId),
                FechaDeCarga = DateTime.Now,
                Usuario = comando.Dto.Usuario,
                TipoDestino = comando.Dto.TipoDestino,
                CentroDestino = Repositorio.Obtener<Centro>(x => x.Id == comando.Dto.CentroDestinoId),
                ClienteDestino = Repositorio.Obtener<Cliente>(x => x.Id == comando.Dto.ClienteDestinoId),
                Motivo = MotivoExcepcionAlControl.A
            };
        }

        protected override void Validar(CrearExcepcionAlControlProveedor comando, Resultado resultado)
        {

        }
    }
}
