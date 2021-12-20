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
    public class ProcesadorCrearRomaneoItem : ProcesadorComando<CrearRomaneoItem>
    {
        private ZSDWS_SCATO servicioSap;
        public ProcesadorCrearRomaneoItem(IRepositorio repositorio, IConversor conversor, ILogger log, ZSDWS_SCATO servicioSap)
            : base(repositorio, conversor, log)
        {
            this.servicioSap = servicioSap;
        }
        public override Resultado Ejecutar(CrearRomaneoItem comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorCrearRomaneoItem para el romaneo #{0}", comando.Dto.RomaneoId);
                var romaneo = Repositorio.Obtener<Romaneo>(comando.Dto.RomaneoId);
                var itemPedido = romaneo.RomaneoItemsPedidos.FirstOrDefault(x => x.Material.Id == comando.Dto.MaterialId);
                
                var respuesta = servicioSap.ContabilizarIngresos(new ContabilizarIngresosRequest(new ContabilizarIngresos
                    {
                        Cantidad = comando.Dto.PesoNeto.HasValue ? comando.Dto.PesoNeto.Value : 0,
                        Centro = itemPedido.Werks,
                        DocCompras = romaneo.NroPedido,
                        FechaDoc = romaneo.FechaInicio.ToString("yyyy-MM-dd"),
                        FechaFabricacion = comando.Dto.FechaFabricacion.HasValue ? comando.Dto.FechaFabricacion.Value.ToString("yyyy-MM-dd") : "",
                        LoteProveedor = comando.Dto.LoteProveedor,
                        Material = itemPedido.Material.CodigoSAP,
                        NroNotaEntregaExt = comando.Dto.RemitoNro,
                        PosDocCompras = itemPedido.Ebelp,
                        Usuario = ConfigurationManager.AppSettings.Get("SapServiceUsername")
                    }));

                if (respuesta.ContabilizarIngresosResponse.Return == null || !respuesta.ContabilizarIngresosResponse.Return.Any())
                {
                    var item = Conversor.Convertir<RomaneoItemDto, RomaneoItem>(comando.Dto);
                    item.Almacen = Repositorio.Obtener<Almacen>(comando.Dto.AlmacenId);
                    item.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
                    item.Balanza = Repositorio.Obtener<Balanza>(comando.Dto.BalanzaId);
                    item.TaraRomaneo = Repositorio.Obtener<TaraRomaneo>(comando.Dto.TaraRomaneoId);
                    item.Fecha = DateTime.Now;
                    item.PesoBruto = comando.Dto.PesoBruto ?? 0;
                    item.PesoTara = comando.Dto.PesoTara ?? 0;
                    item.DocMaterial = respuesta.ContabilizarIngresosResponse.DocMaterial;
                    item.EjercicioDocMaterial = respuesta.ContabilizarIngresosResponse.EjercicioDocMaterial;
                    item.Romaneo = romaneo;
                    Repositorio.Agregar(item);
                    romaneo.Estado = EstadoRomaneo.EnProceso;
                    resultado.Id = item.Id;
                }
                else
                {
                    throw new Exception(respuesta.ContabilizarIngresosResponse.Return[0].MESSAGE);
                }

                if (!resultado.HayErrores)
                {
                    Repositorio.GuardarCambios();
                    Log.Info("Se creó exitosamente el item para el romaneo #{0}", comando.Dto.RomaneoId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ocurrió un error al intentar crear el item para el romaneo #{0}", comando.Dto.RomaneoId);
                resultado.Error("", Textos.RomaneoDescargar_Error + ":" + ex.Message);
            }
            return resultado;
        }
    }

}