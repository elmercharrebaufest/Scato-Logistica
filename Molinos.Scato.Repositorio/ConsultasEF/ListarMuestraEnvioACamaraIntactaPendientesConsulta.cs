using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarMuestraEnvioACamaraIntactaPendientesConsulta : IConsulta<MuestraEnvioACamaraBiotecnoligiaDto>
    {
        private readonly int materialId;
        private readonly int camaraId;
        private readonly int centroId;

        public ListarMuestraEnvioACamaraIntactaPendientesConsulta(int materialId, int camaraId, int centroId)
        {
            this.materialId = materialId;
            this.camaraId = camaraId;
            this.centroId = centroId;
        }

        public List<MuestraEnvioACamaraBiotecnoligiaDto> Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            return (from recorrido in contexto.Set<Recorrido>()

                    join remito in contexto.Set<Remito>() on recorrido.Id equals remito.Recorrido.Id into remitoJoined
                    from remito in remitoJoined.DefaultIfEmpty()

                    join conversionCentro in contexto.Set<ConversionCentro>().DefaultIfEmpty() on new { Key1 = centroId, Key2 = camaraId } equals new { Key1 = conversionCentro.Centro.Id, Key2 = conversionCentro.Camara.Id } into conversionCentroJoined
                    from conversionCentro in conversionCentroJoined.DefaultIfEmpty()

                    join camara in contexto.Set<Camara>() on camaraId equals camara.Id into camaraJoined
                    from camara in camaraJoined.DefaultIfEmpty()

                    where recorrido.Centro.Id == centroId && recorrido.Material.Id == materialId && recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso && (!recorrido.Rechazado || recorrido.Vehiculo.TipoVehiculo == TipoVehiculo.Tren) &&
                    recorrido.Terminado && (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte || recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.Remito) &&
                    !contexto.Set<ProveedorExcluidoIntacta>().Any(x => (remito != null && x.Proveedor.Id == remito.ProveedorOrigen.Id) || (recorrido.Vehiculo != null && x.Proveedor.Id == recorrido.Vehiculo.CartaPorte.Destinatario.Id)) &&
                    !contexto.Set<MuestraEnvioACamaraBiotecnologia>().Any(x => x.Recorrido.Id == recorrido.Id)
                    select new MuestraEnvioACamaraBiotecnoligiaDto
                    {
                        CentroId = recorrido.Centro.Id,
                        CamaraId = camaraId,
                        Material = recorrido.Material.Descripcion,
                        FechaDescarga = recorrido.PesoTaraFecha.HasValue ? recorrido.PesoTaraFecha.Value : recorrido.FechaInicio,
                        PesoNeto = (recorrido.PesoBruto ?? 0) - (recorrido.PesoTara ?? 0),
                        NroCartaPorte = remito != null ? remito.OrdenRemito : recorrido.NumeroDocumentoIngreso,
                        FechaCartaPorte = recorrido.Vehiculo != null ? recorrido.Vehiculo.CartaPorte.FechaCP : (remito != null ? remito.FechaOD : DateTime.Now),
                        Proveedor = recorrido.Vehiculo != null ? recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Descripcion : (remito != null ? remito.ProveedorOrigen.Descripcion : ""),
                        ProveedorCodigoSap = recorrido.Vehiculo != null ? recorrido.Vehiculo.CartaPorte.TitularCartaPorte.CodigoSap : (remito != null ? remito.ProveedorOrigen.CodigoSap : ""),
                        Vendedor = recorrido.Vehiculo != null ? recorrido.Vehiculo.CartaPorte.Destinatario.Descripcion : "",
                        Corredor = recorrido.Vehiculo != null ? recorrido.Vehiculo.CartaPorte.Corredor.Descripcion : "",
                        Localidad = recorrido.Vehiculo != null ? recorrido.Vehiculo.CartaPorte.Procedencia.Descripcion : (remito != null ? remito.Procedencia.Descripcion : ""),

                        Patente = recorrido.Patente,
                        WorkflowInstanceId = recorrido.InstanciaWorkflow,
                        EstadoMuestra = EstadoMuestra.Pendiente,
                        TieneAnalisisInterno = recorrido.AnalisisDeCalidad != null,
                        CamaraDesc = camara.Descripcion,
                        CodigoDeCamara = conversionCentro.CodigoCamara,
                        NumeroVehiculo = recorrido.Vehiculo != null ? recorrido.Vehiculo.NumeroVehiculo : 1,
                        CamaraFormatoDeArchivo = camara.FormatoDeArchivo != null ? camara.FormatoDeArchivo.Value : CamaraFormatoDeArchivo.NoEspecificado
                    }).Take(5000).ToList();

        }
    }
}
