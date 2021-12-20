using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarRomaneo : ProcesadorComando<ActualizarRomaneo>
    {
        private ZSDWS_SCATO servicioSap;
        public ProcesadorActualizarRomaneo(IRepositorio repositorio, IConversor conversor, ILogger log, ZSDWS_SCATO servicioSap)
            : base(repositorio, conversor, log)
        {
            this.servicioSap = servicioSap;
        }

        public override Resultado Ejecutar(ActualizarRomaneo comando)
        {
            var resultado = new ResultadoCrear();
            var romaneo = Repositorio.Obtener<Romaneo>(x => (x.Id == comando.Dto.Numero));
            try
            {

                if (romaneo == null)
                {
                    ZMMBALANZA2[] materialesPedidos;
                    List<string> materialsNr;
                    string codigoProveedor;
                    ConsultaPedidoResponse(comando, resultado, out materialesPedidos, out materialsNr, out codigoProveedor);

                    IList<Material> materiales = new List<Material>();
                    Proveedor proveedor = null;
                    if (!resultado.HayErrores)
                    {
                        materiales = Repositorio.Listar<Material>(x => materialsNr.Contains(x.CodigoSAP));
                        var materialesCodigosSap = materiales.Select(x => x.CodigoSAP);
                        var materialesNoEncontrados = materialsNr.Where(x => !materialesCodigosSap.Contains(x));
                        if (materialesNoEncontrados.Count() != 0)
                        {
                            resultado.Errores.Add("", String.Format(Textos.Error_PedidoMaterialesNoEncontrados, comando.Dto.NroPedido));
                        }
                    }
                    if (!resultado.HayErrores)
                    {
                        proveedor = Repositorio.Obtener<Proveedor>(x => x.CodigoSap == codigoProveedor);
                        if (proveedor == null)
                        {
                            resultado.Errores.Add("", String.Format(Textos.Error_PedidoProveedorNoEncontrado, comando.Dto.NroPedido));
                        }
                    }
                    if (!resultado.HayErrores)
                    {
                        romaneo = Conversor.Convertir<RomaneoDto, Romaneo>(comando.Dto);
                        romaneo.FechaCierre = null;
                        romaneo.Proveedor = proveedor;
                        romaneo.RomaneoItemsPedidos = materialesPedidos.Select(x => new RomaneoItemPedido
                        {
                            CantPedido = x.CANT_PEDIDO,
                            Ebelp = x.EBELP,
                            FecEntrega = x.FEC_ENTREGA,
                            PorcentajeExc = x.PORCENTAJE_EXC,
                            Werks = x.WERKS,
                            Romaneo = romaneo,
                            Material = materiales.FirstOrDefault(y => y.CodigoSAP == x.MATNR)
                        }).ToList();
                        Repositorio.Agregar(romaneo);
                    }
                    
                }
                else
                {
                    romaneo.Estado = comando.Dto.Estado;
                    romaneo.Observaciones = comando.Dto.Observaciones;
                    romaneo.FechaCierre = DateTime.Now;
                }
                if (!resultado.HayErrores)
                {
                    Repositorio.GuardarCambios();
                    resultado.Id = romaneo.Id;
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("",e.Message);
            }
            return resultado;
        }

        private void ConsultaPedidoResponse(ActualizarRomaneo comando, ResultadoCrear resultado, out ZMMBALANZA2[] materialesPedidos, out List<string> materialsNr, out string codigoProveedor)
        {
            var respuesta =
                servicioSap.ConsultaPedido(
                    new ConsultaPedidoRequest(new ConsultaPedido {Pedido = comando.Dto.NroPedido}));

            materialesPedidos = respuesta.ConsultaPedidoResponse.Detalles;
            materialsNr = respuesta.ConsultaPedidoResponse.Detalles.Select(x => x.MATNR).ToList();
            for (var i = 0; materialsNr.Count > i; i++)
            {
                materialsNr[i] = materialsNr[i].TrimStart(new char[] {'0'});
            }
            codigoProveedor = respuesta.ConsultaPedidoResponse.Proveedor;
            if (string.IsNullOrEmpty(respuesta.ConsultaPedidoResponse.Proveedor))
            {
                resultado.Errores.Add("", String.Format(Textos.Error_PedidoNoEncontrado, comando.Dto.NroPedido));
                return;
            }
            if (materialsNr.Count == 0)
            {
                resultado.Errores.Add("", String.Format(Textos.Error_PedidoSinMateriales, comando.Dto.NroPedido));
            }
        }
    }


}