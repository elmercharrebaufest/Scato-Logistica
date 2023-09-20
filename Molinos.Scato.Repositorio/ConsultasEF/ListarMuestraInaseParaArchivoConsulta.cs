using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarMuestraInaseParaArchivoConsulta : IConsulta<MuestraDeInaseDto>
    {
        private readonly string codigoSapFirma;
        private readonly int? loteId;

        public ListarMuestraInaseParaArchivoConsulta(string codigoSapFirma, int? loteId = null)
        {
            this.codigoSapFirma = codigoSapFirma;
            this.loteId = loteId;
        }

        public List<MuestraDeInaseDto> Ejecutar(DbContext contexto)
        {
            var query = contexto.Set<MuestraDeInase>().AsQueryable();

            query = (loteId != null)
                        ? query.Where(x => x.LoteInase.Id == loteId)
                        : query.Where(x => !x.MuestraEnviada && !x.Recorrido.Rechazado && x.LoteInase == null);

            var listasDeMuestras = query.Select(x => new ControlDto { IdRelacionado = x.Recorrido.Id, Id = x.Id }).ToList();
            var muestras = ListarMuestrasDeInase(contexto, listasDeMuestras);
            var resultado = muestras.ToList();
            CompletarCamposExtras(contexto, resultado, listasDeMuestras);
            return resultado;
        }

        private IQueryable<MuestraDeInaseDto> ListarMuestrasDeInase(DbContext contexto, List<ControlDto> listasDeMuestras)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var empresa = contexto.Set<Proveedor>().FirstOrDefault(x => x.CodigoSap == codigoSapFirma);
            var recorridosIds = listasDeMuestras.Select(x => x.IdRelacionado).ToList();
            var remitos = contexto.Set<Remito>().Where(r => recorridosIds.Contains(r.Id)).ToList();
            var muestras = contexto.Set<Recorrido>()
                                    .Where(r => recorridosIds.Contains(r.Id))
                                    .Select(x => new MuestraDeInaseDto
                                    {
                                        MaterialId = x.Material.Id,
                                        CentroId = x.Centro.Id,
                                        RecorridoId = x.Id,
                                        WorkflowInstanceId = x.InstanciaWorkflow,
                                        CartaPorte = x.NumeroDocumentoIngreso,
                                        Patente = x.Patente,
                                        MaterialDescripcion = x.Material.Descripcion,
                                        DestinatarioCuil = x.Vehiculo.CartaPorte.Destinatario.Cuil
                                                                ?? x.Vehiculo.CartaPorte.DestinatarioCliente.Cuit
                                                                ?? empresa.Cuil,
                                        RtteComercialCuit = x.Vehiculo.CartaPorte.RtteComercial.Cuil,
                                        CorredorCuil = x.Vehiculo.CartaPorte.Corredor.Cuil,
                                        TitularCartaPorte = x.Vehiculo.CartaPorte.TitularCartaPorte.Descripcion
                                                                ?? remitos.Where(re => re.Recorrido.Id == x.Id).Select(re => re.ProveedorOrigen.Descripcion).FirstOrDefault()
                                                                ?? empresa.Descripcion,
                                        CPE = x.Vehiculo.CartaPorte.Cpe ?? false,
                                        Sucursal = x.Vehiculo.CartaPorte.Sucursal,
                                        CTG = x.Vehiculo.CartaPorte.CTG ?? "0",
                                        TitularCartaPorteCuil = x.Vehiculo.CartaPorte.TitularCartaPorte.Cuil
                                                                ?? remitos.Where(re => re.Recorrido.Id == x.Id).Select(re => re.ProveedorOrigen.Cuil).FirstOrDefault()
                                                                ?? empresa.Cuil,
                                        CodEstab = x.Vehiculo != null
                                                    ? (x.Vehiculo.CartaPorte.CodEstab.StartsWith("999")
                                                            ? "" : x.Vehiculo.CartaPorte.CodEstab)
                                                    : (remitos.Any(re => re.Recorrido.Id == x.Id && re.CodEstab.StartsWith("999"))
                                                            ? "" : remitos.Where(re => re.Recorrido.Id == x.Id).Select(re => re.CodEstab).FirstOrDefault()),
                                        Direccion = x.Centro.Direccion,
                                        ProcedenciaCodigoSap = x.Vehiculo.CartaPorte.Procedencia.CodigoAfip ?? remitos.Where(re => re.Recorrido.Id == x.Id).Select(re => re.Procedencia.CodigoAfip).FirstOrDefault(),
                                        Vendedor = x.Vehiculo.CartaPorte.Destinatario.Descripcion,
                                        Localidad = x.Vehiculo.CartaPorte.Procedencia.Descripcion,
                                        LocalidadCodigoSap = x.Centro.Localidad.CodigoAfip,
                                        TipoVehiculo = x.TipoVehiculo,
                                        CantidadDeVagones = x.Vehiculo.CartaPorte.Vehiculos.Count,
                                        CodigoEstablecimiento = x.Centro.CodigoEstablecimiento,
                                        Corredor = x.Vehiculo.CartaPorte.Corredor.RazonSocial,
                                        RtteComercial = x.Vehiculo.CartaPorte.RtteComercial.Descripcion,
                                        Cosecha = x.Vehiculo.CartaPorte.Cosecha ?? remitos.Where(re => re.Recorrido.Id == x.Id).Select(re => re.Cosecha).FirstOrDefault(),
                                        ProcedenciaCodigoPostal = x.Vehiculo.CartaPorte.Procedencia.CodigoPostal
                                                                    ?? remitos.Where(re => re.Recorrido.Id == x.Id).Select(re => re.Procedencia.CodigoPostal).FirstOrDefault()
                                                                    ?? 0,
                                        ProcedenciaSubcodigoPostal = x.Vehiculo.CartaPorte.Procedencia.SubcodigoPostal
                                                                    ?? remitos.Where(re => re.Recorrido.Id == x.Id).Select(re => re.Procedencia.SubcodigoPostal).FirstOrDefault()
                                                                    ?? 0,
                                        PesoNeto = (x.PesoBruto ?? 0) - (x.PesoTara ?? 0),
                                        FechaDescarga = x.PesoTaraFecha ?? x.FechaInicio,
                                        Intermediario = null,
                                        IntermediarioCuit = null,
                                        NumeroVehiculo = x.Vehiculo.NumeroVehiculo
                                    });
            return muestras;
        }

        private void CompletarCamposExtras(DbContext contexto, List<MuestraDeInaseDto> muestras, List<ControlDto> listasDeMuestras)
        {
            var materiales = muestras.Select(x => x.MaterialId).ToList();
            var centros = muestras.Select(x => x.CentroId).ToList();
            var materialesPorCentro = contexto.Set<MaterialPorCentro>()
                                            .Where(mpc => centros.Contains(mpc.Centro.Id) && materiales.Contains(mpc.Material.Id))
                                            .ToList();
            foreach (var muestra in muestras)
            {
                var premuestra = listasDeMuestras.FirstOrDefault(x => x.IdRelacionado == muestra.RecorridoId);
                muestra.Id = premuestra != null ? premuestra.Id : 0;

                var camaraId = materialesPorCentro
                                            .Where(mpc => mpc.Centro.Id == muestra.CentroId && mpc.Material.Id == muestra.MaterialId)
                                            .Select(mpc => mpc.Camara.Id)
                                            .FirstOrDefault();
                muestra.CodigoDeCamara = contexto.Set<ConversionCentro>()
                                            .Where(x => x.Centro.Id == muestra.CentroId && x.Camara.Id == camaraId)
                                            .Select(x => x.CodigoCamara)
                                            .FirstOrDefault();
                muestra.CamaraDesc = materialesPorCentro
                                            .Where(mpc => mpc.Centro.Id == muestra.CentroId && mpc.Material.Id == muestra.MaterialId)
                                            .Select(mpc => mpc.Camara.Descripcion)
                                            .FirstOrDefault();
                muestra.CamaraFormatoDeArchivo = (CamaraFormatoDeArchivo)materialesPorCentro
                                            .Where(mpc => mpc.Centro.Id == muestra.CentroId && mpc.Material.Id == muestra.MaterialId)
                                            .Select(mpc => mpc.Camara.FormatoDeArchivo)
                                            .FirstOrDefault();
                muestra.CodigoCamaraMaterial = contexto.Set<ConversionMaterial>()
                                            .Where(x => x.Material.Id == muestra.MaterialId && x.Camara.Id == camaraId)
                                            .Select(x => x.CodigoCamara)
                                            .FirstOrDefault();
                muestra.CodigoCamaraGrupo = contexto.Set<ConversionGrupo>()
                                            .Where(x => x.Material.Id == muestra.MaterialId && x.Camara.Id == camaraId)
                                            .Select(x => x.CodigoSegunCamara)
                                            .FirstOrDefault();
            }
        }
    }
}