using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearActualizarVisecTransmision : ProcesadorComando<CrearActualizarVisecTransmision>
    {
        public ProcesadorCrearActualizarVisecTransmision(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearActualizarVisecTransmision comando)
        {
            var resultado = new ResultadoCrear();
            var id = 0;
            try
            {
                var entidad = Repositorio.Obtener<VisecTransmision>(comando.Dto.Id);
                if (entidad == null)
                {
                    Log.Debug("Entidad VisecTransmision no encontrada, creando una nueva.");
                    entidad = new VisecTransmision
                    {
                        CUITEmpresa = comando.Dto.CUITEmpresa,
                        Estado = (int)EstadoTransmisionAVisec.Pendiente,
                        FechaTransaccion = DateTime.Now,
                        VisecTransmisionMovimientos = comando.Dto.VisecTransmisionMovimientos
                                        .Select(x => ObtenerVisecTransmisionMovimiento(x, id))
                                        .ToList()
                    };
                    Repositorio.Agregar(entidad);
                }
                else 
                {
                    entidad.Estado = (int)comando.Dto.Estado;
                    entidad.DetalleTransaccion = comando.Dto.DetalleTransaccion;
                }      
                entidad.FechaHoraMovimiento = comando.Dto.FechaHoraMovimiento;
                entidad.FechaCPE = comando.Dto.FechaCPE;
                entidad.NumeroCPE = comando.Dto.NumeroCPE;
                entidad.NumeroCTG = comando.Dto.NumeroCTG;
                entidad.CUITTitular = comando.Dto.CUITTitular;
                entidad.NumeroRUCAOrigen = comando.Dto.NumeroRUCAOrigen;
                entidad.CUITDestinatario = comando.Dto.CUITDestinatario;
                entidad.CUITDestino = comando.Dto.CUITDestino;
                entidad.NumeroRUCADestino = comando.Dto.NumeroRUCADestino;
                entidad.Producto = comando.Dto.Producto;
                entidad.Campania = comando.Dto.Campania;
                entidad.PesoNetoCargaKg = comando.Dto.PesoNetoCargaKg;
                entidad.StockKg = comando.Dto.StockKg;
                Repositorio.GuardarCambios();
                resultado.Id = entidad.Id;
                Log.Debug("Entidad VisecTransmision actualizada con Id: {Id}", resultado.Id);
                return resultado;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al procesar la actualización de TransmisionAVisec.");
                resultado.Errores.Add("Error", ex.Message);
                return resultado;
            }
        }
        
        public VisecTransmisionMovimiento ObtenerVisecTransmisionMovimiento(VisecTransmisionMovimientoDto dto, int id)
        {
            return new VisecTransmisionMovimiento
            {
                Id = id,
                NumeroRENSPA = dto.NumeroRENSPA,
                NumeroCTGAsignado = dto.NumeroCTGAsignado,
                PesoNetoCargaKgPorUP = dto.PesoNetoCargaKgPorUP,
                PesoNetoDescargaKgPorUP = dto.PesoNetoDescargaKgPorUP,
                PesoIngresoStockKg = dto.PesoIngresoStockKg,
                UltimoAlmacenamiento = dto.UltimoAlmacenamiento,
                TipoMovimiento = dto.TipoMovimiento
            };
        }
    }
}