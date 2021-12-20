using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarStock : ProcesadorComando<ActualizarStock>
    {
        private readonly ZSDWS_SCATO servicioSap;

        public ProcesadorActualizarStock(IRepositorio repositorio, IConversor conversor, ILogger log, ZSDWS_SCATO servicioSap)
            : base(repositorio, conversor, log)
        {
            this.servicioSap = servicioSap;
        }

        public override Resultado Ejecutar(ActualizarStock comando)
        {
            var resultado = new Resultado();
            var stocks = Repositorio.Listar<Stock>(x => x.Fecha == comando.Fecha && x.Recorrido == null &&
                (comando.ProveedorId.HasValue && x.Proveedor.Id == comando.ProveedorId.Value || !comando.ProveedorId.HasValue) &&
                (comando.MaterialId.HasValue && x.Material.Id == comando.MaterialId.Value || !comando.MaterialId.HasValue)).ToList();
            string materialCodigoSap = null;
            string cuit = null;
            if (comando.ProveedorId.HasValue)
            {
                cuit = Repositorio.ObtenerProyeccion<Proveedor, string>(x => x.Id == comando.ProveedorId, x => x.Cuil);
                cuit = !string.IsNullOrEmpty(cuit) ? cuit.Replace("-", "") : cuit;
            }
            if (comando.MaterialId.HasValue)
            {
                materialCodigoSap = Repositorio.ObtenerProyeccion<Material, string>(x => x.Id == comando.MaterialId, x => x.CodigoSAP);
            }
            Log.Debug($"Consulto Z_SDMF_RFC_VENTA_TRIGO_MAIZRequest {cuit} , {comando.Fecha.ToString("yyyyMMdd", CultureInfo.InvariantCulture)} , {materialCodigoSap}");
            var stockSap = servicioSap.Z_SDMF_RFC_VENTA_TRIGO_MAIZ(new Z_SDMF_RFC_VENTA_TRIGO_MAIZRequest
            {
                Z_SDMF_RFC_VENTA_TRIGO_MAIZ = new Z_SDMF_RFC_VENTA_TRIGO_MAIZ
                {
                    IM_CUIT = cuit,
                    IM_FECHA= comando.Fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    IM_MATERIAL = materialCodigoSap,
                }
            });

            if (stockSap != null && stockSap.Z_SDMF_RFC_VENTA_TRIGO_MAIZResponse != null && stockSap.Z_SDMF_RFC_VENTA_TRIGO_MAIZResponse.EX_CANTIDAD != null)
            {
                Log.Debug($"Consulto Z_SDMF_RFC_VENTA_TRIGO_MAIZRequest, Resultados: {stockSap.Z_SDMF_RFC_VENTA_TRIGO_MAIZResponse.EX_CANTIDAD.Count() }");

                foreach (var stock in stockSap.Z_SDMF_RFC_VENTA_TRIGO_MAIZResponse.EX_CANTIDAD)
                {
                    var materialStock = stock.MATERIAL.TrimStart('0');
                    var stockAActualizar = stocks.FirstOrDefault(x => x.Material.CodigoSAP == materialStock && x.Proveedor.Cuil.Replace("-", "") == stock.CUIT);

                    if(stockAActualizar == null)
                    {
                        Log.Debug($"Creo Stock para: {stock.CUIT} - {materialStock} - {stock.CANTIDAD}");

                        var proveedor = Repositorio.Obtener<Proveedor>(x => x.Cuil.Replace("-", "") == stock.CUIT && !x.CodigoSap.Contains("C"));
                        if(proveedor == null)
                        {
                            Log.Error($"Proveedor no encontrado: {stock.CUIT} - {materialStock} - {stock.CANTIDAD}");
                            continue;
                        }
                        var material = Repositorio.Obtener<Material>(x => x.CodigoSAP == materialStock);
                        if (material == null)
                        {
                            Log.Error($"Material no encontrado: {stock.CUIT} - {materialStock} - {stock.CANTIDAD}");
                            continue;
                        }
                        stockAActualizar = new Stock
                        {
                            Fecha = comando.Fecha.Date,
                            Proveedor = proveedor,
                            Material = material
                        };
                        Repositorio.Agregar(stockAActualizar);
                    }
                    else
                    {
                        Log.Debug($"Actualizo Stock para: {stock.CUIT} - {materialStock} - {stock.CANTIDAD}");
                    }
                    stockAActualizar.Cantidad = stock.CANTIDAD;
                }
            }
            

            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }

            return new Resultado();
        }


    }
}