using System;
using System.Activities;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionConstanciaDeEntregaLaser : CodeActivity<Resultado>
    {
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<CartaPorteDto> Orden { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroIngreso { get; set; }
        public InArgument<string> Observaciones { get; set; }
        public InArgument<string> ObservacionesCalado { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        public InArgument<string> PesoBruto { get; set; }
        public InArgument<string> PesoNeto { get; set; }
        public InArgument<string> PesoTara { get; set; }
        [RequiredArgument]
        public InArgument<VehiculoDto> Vehiculo { get; set; }
        public InArgument<int?> CantCopias { get; set; }

        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var firmaProvider = context.GetExtension<IFirmaProvider>();
            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);
            var orden = Orden.Get<CartaPorteDto>(context);
            var vehiculo = Vehiculo.Get<VehiculoDto>(context);
            var centroId = CentroId.Get<int>(context);
            var numeroIngreso = NumeroIngreso.Get<string>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var observaciones = Observaciones.Get<string>(context);
            var observacionesCalado = ObservacionesCalado.Get<string>(context);
            var centro = repositorio.ObtenerCentro(centroId);
            var pesoBruto = PesoBruto.Get<string>(context);
            var pesoNeto = PesoNeto.Get<string>(context); ;
            var pesoTara = PesoTara.Get<string>(context); ;
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;

            var recorrido = repositorio.ObtenerRecorridoPorGuid(workflowId);
            var ultimaPesada = orden.TipoDeWorkflow == TipoDeWorkflow.Ingreso ? recorrido.PesoTaraFecha : recorrido.PesoBrutoFecha;
            var logActividad = new LogActividadDto
            {
                Actividad = "Impresion Constancia De Entrega Laser",
                ActividadXaml = "ImpresionConstanciaDeEntregaLaser",
                WorkflowInstanceId = workflowId,
                Fecha = DateTime.Now
            };
            
            try
            {
                resultado = servicio.Ejecutar(new CrearLogActividad { Dto = logActividad });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.LogActividad_ErrorEnLaCarga);
            }

            try
            {
                var balanzas = repositorio.ObtenerBalanzasPorGuid(workflowId);

                var balanzaBruto = balanzas.BalanzaBruto;
                var balanzaTara = balanzas.BalanzaTara;

                string vendedor;
                if (orden.DestinatarioCodigoSap != firmaProvider.ObtenerFirmaSinLogo().CodigoSAP)
                    vendedor = orden.Destinatario;
                else
                    vendedor = orden.RtteComercial ?? orden.TitularCartaPorte;

                var muestraEnvioCamara = repositorio.ObtenerMuestraEnvioACamaraPorCalado(recorrido.Calado.Id);
                var caracteristicas = repositorio.ListarAnalisisYCaladoPorCaracteristica(workflowId);
                var calidadValor = caracteristicas.Select(c => new CaracteristicasCalidadValorDto { Key = c.Caracteristica, Data = (c.ValorAnalisis ?? c.ValorCalado).ToString(), Descuento = c.DescuentoEnKg.ToString(), EnvioCamara = muestraEnvioCamara != null && muestraEnvioCamara.CaracteristicasDeCalidad.Select(x => x.Descripcion).Contains(c.Caracteristica) ? "Si" : "No"});

                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null) 
                    throw new Exception(string.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo));

                var dto = new ImpConstanciaDeEntregaLaserDto
                {
                    Impresora = documento.ImpresoraDireccion ?? string.Empty,
                    Centro = centro.Descripcion,
                    CentroDireccion = centro.Direccion,
                    CentroLocalidad = centro.LocalidadDesc,
                    CentroProvincia = centro.ProvinciaDesc,
                    Fecha = DateTime.Now,
                    FechaCartaPorte = ultimaPesada ?? orden.FechaEmision,
                    Corredor = orden.Corredor ?? string.Empty,
                    Codigo = codigo,
                    Observaciones = observaciones ?? string.Empty,
                    Patente = vehiculo.Patente,
                    PatenteAcoplado = vehiculo.PatenteAcoplado ?? string.Empty,
                    Vendedor = vendedor ?? string.Empty,
                    Material = orden.Material ?? string.Empty,
                    NumeroCartaPorte = orden.NroCartaPorte,
                    CaracteristicasCalidadValor = calidadValor.ToList(),
                    NumeroCertificacion = orden.Id.ToString(CultureInfo.InvariantCulture),
                    NumeroDeOrden = numeroIngreso,
                    ObservacionesCalado = observacionesCalado,
                    PesoBruto = pesoBruto ?? string.Empty,
                    PesoNeto = pesoNeto ?? string.Empty,
                    PesoTara = pesoTara ?? string.Empty,
                    Procedencia = orden.Procedencia,
                    WorkflowId = workflowId,
                    BalanzaBruto = balanzaBruto.Nombre ?? string.Empty,
                    BalanzaTara = balanzaBruto.Nombre ?? string.Empty,
                    ModeloBalanzaBruto = balanzaBruto != null ? balanzaBruto.Modelo : string.Empty,
                    ModeloBalanzaTara = balanzaTara != null ? balanzaTara.Modelo : string.Empty,
                    NroSerieBalanzaBruto = balanzaBruto != null ? balanzaBruto.NroSerie : string.Empty,
                    NroSerieBalanzaTara = balanzaTara != null ? balanzaTara.NroSerie : string.Empty,
                    Entregador = orden.Entregador ?? string.Empty, 
                    MaterialId = orden.MaterialId,
                    CentroId = centroId
                };

                resultado = servicio.Ejecutar(new ImprimirConstanciaDeEntregaLaser { Dto = dto, CantidadCopias = cantCopias });
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionConstanciaDeEntregaLaser", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}