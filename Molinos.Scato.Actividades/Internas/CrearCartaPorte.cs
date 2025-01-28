using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class CrearCartaPorte : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<CartaPorteDto> Orden { get; set; }

        [RequiredArgument]
        public InArgument<string> NombreWorkflow { get; set; }

        [RequiredArgument]
        public InArgument<int> WorkflowDefinicionId { get; set; }

        [RequiredArgument]
        public InArgument<Guid> InstanciaWorkflowId { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }

        public OutArgument<CartaPorteDto> CartaPorte { get; set; }

        public OutArgument<string> NumeroCartaPorte { get; set; }

        public OutArgument<TipoDocumentoIngreso> TipoDocumentoIngreso { get; set; }

        public OutArgument<DateTime> FechaInicio { get; set; }

        public InArgument<string> Usuario { get; set; }

        public InArgument<VehiculoDto> Vehiculo { get; set; }
        public OutArgument<bool> VehiculoDemorado { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var orden = Orden.Get<CartaPorteDto>(context);
            var vehiculo = Vehiculo.Get<VehiculoDto>(context);
            var nombreWorkflow = NombreWorkflow.Get<string>(context);
            var instanciaWorkflow = InstanciaWorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var usuario = Usuario.Get<string>(context);
            var workflowDefinicionId = WorkflowDefinicionId.Get<int>(context);
            var resultado = new ResultadoCrearWorkflow();
            resultado.InstanciaWorkflowId = instanciaWorkflow;

            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var titularCartaPorteCodigoSap = srvRepositorio.ObtenerProveedor(orden.TitularCartaPorteId)?.CodigoSap;
                var tipoMaterialPorVariedad = srvRepositorio.ObtenerVariedadIdPorMaterial(orden.MaterialId, titularCartaPorteCodigoSap, orden.CodEstab);
                var resultadoCartaPorte = servicioComandos.Ejecutar(new Dominio.Comandos.CrearCartaPorte
                {
                    Orden = orden,
                    NombreWorkflow = nombreWorkflow,
                    InstanciaWorkflowId = instanciaWorkflow,
                    CentroId = centroId,
                    Usuario = usuario,
                    Vehiculo = vehiculo,
                    WorkflowDefinicionId = workflowDefinicionId,
                    TipoVariedadId = tipoMaterialPorVariedad,
                }) as ResultadoCrear;
                resultado.Id = resultadoCartaPorte.Id;

                var ordenDto = srvRepositorio.ObtenerCartaPorte(resultado.Id);
                if (ordenDto != null)
                {
                    ordenDto.VehiculoDemorado = orden.VehiculoDemorado;
                    CartaPorte.Set(context, ordenDto);
                    FechaInicio.Set(context, DateTime.Now);
                    NumeroCartaPorte.Set(context, ordenDto.NroCartaPorte);
                    TipoDocumentoIngreso.Set(context, Dominio.Enums.TipoDocumentoIngreso.CartaPorte);
                    VehiculoDemorado.Set(context, ordenDto.VehiculoDemorado);
                }

                if (resultadoCartaPorte.HayErrores)
                {
                    resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
                }
            }
            catch (Exception)
            {
                resultado.Errores.Add("1", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}