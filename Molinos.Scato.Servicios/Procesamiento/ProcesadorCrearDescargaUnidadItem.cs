using System;
using System.Configuration;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearDescargaUnidadItem : ProcesadorComando<CrearDescargaUnidadItem>
    {
        private ZSDWS_SCATO servicioSap;
        public ProcesadorCrearDescargaUnidadItem(IRepositorio repositorio, IConversor conversor, ILogger log, ZSDWS_SCATO servicioSap)
            : base(repositorio, conversor, log)
        {
            this.servicioSap = servicioSap;
        }
        public override Resultado Ejecutar(CrearDescargaUnidadItem comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorCrearDescargaUnidadItem para la descargaUnidad #{0}", comando.Dto.DescargaUnidadId);
                var descarga = Repositorio.Obtener<DescargaUnidad>(comando.Dto.DescargaUnidadId);
                var itemPedido = descarga.DescargaUnidadItemPedidos.FirstOrDefault(x => x.Material.Id == comando.Dto.MaterialId);
                
                var respuesta = servicioSap.ContabilizarIngresos(new ContabilizarIngresosRequest(new ContabilizarIngresos
                    {
                        Cantidad = comando.Dto.Unidades,
                        Centro = itemPedido.Werks,
                        DocCompras = descarga.NroPedido,
                        FechaDoc = descarga.FechaInicio.ToString("yyyy-MM-dd"),
                        FechaFabricacion = comando.Dto.FechaFabricacion.HasValue ? comando.Dto.FechaFabricacion.Value.ToString("yyyy-MM-dd") : "",
                        LoteProveedor = comando.Dto.LoteProveedor,
                        Material = itemPedido.Material.CodigoSAP,
                        NroNotaEntregaExt = comando.Dto.RemitoNro,
                        PosDocCompras = itemPedido.Ebelp,
                        Usuario = ConfigurationManager.AppSettings.Get("SapServiceUsername")
                    }));

                if (respuesta.ContabilizarIngresosResponse.Return == null || !respuesta.ContabilizarIngresosResponse.Return.Any())
                {
                    var item = Conversor.Convertir<DescargaUnidadItemDto, DescargaUnidadItem>(comando.Dto);
                    item.Almacen = Repositorio.Obtener<Almacen>(comando.Dto.AlmacenId);
                    item.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
                    item.TaraRomaneo = Repositorio.Obtener<TaraRomaneo>(comando.Dto.TaraRomaneoId);
                    item.Fecha = DateTime.Now;
                    item.PesoBruto = comando.Dto.PesoBruto ?? 0;
                    item.PesoTara = comando.Dto.PesoTara ?? 0;
                    item.DocMaterial = respuesta.ContabilizarIngresosResponse.DocMaterial;
                    item.EjercicioDocMaterial = respuesta.ContabilizarIngresosResponse.EjercicioDocMaterial;
                    item.DescargaUnidad = descarga;
                    Repositorio.Agregar(item);
                    descarga.Estado = EstadoDescargaUnidad.EnProceso;
                    resultado.Id = item.Id;
                }
                else
                {
                    throw new Exception(respuesta.ContabilizarIngresosResponse.Return[0].MESSAGE);
                }

                if (!resultado.HayErrores)
                {
                    Repositorio.GuardarCambios();
                    Log.Info("Se creó exitosamente el item para la descargaUnidad #{0}", comando.Dto.DescargaUnidadId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ocurrió un error al intentar crear el item para el descargaUnidad #{0}", comando.Dto.DescargaUnidadId);
                resultado.Error("", Textos.RomaneoDescargar_Error + ":" + ex.Message);
            }
            return resultado;
        }
    }

}