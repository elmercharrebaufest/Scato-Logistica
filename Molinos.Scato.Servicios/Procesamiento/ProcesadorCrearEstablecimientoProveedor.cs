using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearEstablecimientoProveedor : ProcesadorComando<CrearEstablecimientoProveedor>
    {
        public ProcesadorCrearEstablecimientoProveedor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearEstablecimientoProveedor comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Info("Se está ejecutando ProcesadorCrearEstablecimientoProveedor con EstablecimientoId = {0} y ProveedorId = {1}", comando.Dto.EstablecimientoId, comando.Dto.ProveedorId);
                var establecimiento = Repositorio.Obtener<Establecimiento>(comando.Dto.EstablecimientoId);
                var proveedor = Repositorio.Obtener<Proveedor>(comando.Dto.ProveedorId);

                if (!establecimiento.CorredoresAsociados.Contains(proveedor))
                {
                    establecimiento.CorredoresAsociados.Add(proveedor);
                    Repositorio.GuardarCambios();
                }
                else
                {
                    resultado.Error("", Textos.Error_Generico);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorCrearEstablecimientoProveedor con EstablecimientoId = {0} y ProveedorId = {1}", comando.Dto.EstablecimientoId, comando.Dto.ProveedorId);
                resultado.Error("", e.Message);
            }

            return resultado;
        }
    }
}
