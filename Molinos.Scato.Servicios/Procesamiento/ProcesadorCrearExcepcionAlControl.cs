using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearExcepcionAlControl : ProcesadorCrear<CrearExcepcionAlControl, ExcepcionAlControl>
    {
        public ProcesadorCrearExcepcionAlControl(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override ExcepcionAlControl CrearEntidad(CrearExcepcionAlControl comando)
        {
            return new ExcepcionAlControl
            {
                Transportista = Repositorio.Obtener<Transportista>(x => x.Id == comando.Dto.TransportistaId),
                Material = Repositorio.Obtener<Material>(x => x.Id == comando.Dto.MaterialId),
                FechaDesde = comando.Dto.FechaDesde,
                FechaHasta = comando.Dto.FechaHasta,
                Centro = Repositorio.Obtener<Centro>(x => x.Id == comando.Dto.CentroId),
                FechaDeCarga = DateTime.Now,
                Usuario = comando.Dto.Usuario,
                TipoDestino = comando.Dto.TipoDestino,
                CentroDestino = Repositorio.Obtener<Centro>(x => x.Id == comando.Dto.CentroDestinoId),
                ClienteDestino = Repositorio.Obtener<Cliente>(x => x.Id == comando.Dto.ClienteDestinoId),
                Motivo = MotivoExcepcionAlControl.A,
                ProveedorDestino = Repositorio.Obtener<Proveedor>(x => x.Id == comando.Dto.ProveedorDestinoId)
            };
        }

        protected override void Validar(CrearExcepcionAlControl comando, Resultado resultado)
        {

        }
    }
}
