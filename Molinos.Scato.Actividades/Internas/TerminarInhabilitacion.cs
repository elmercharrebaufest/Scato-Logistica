using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    /// <summary>
    ///     Retorna un IEnumerable de strings con los motivos por los que esta inhabilitado el chofer o camion
    /// </summary>
    public class TerminarInhabilitacion : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }

        [RequiredArgument]
        public InArgument<int> ChoferId { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }

        [RequiredArgument]
        public InArgument<string> NombreUsuario { get; set; }
        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }


           

        protected override void Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();

            var patente = Patente.Get<string>(context);
            var choferId = ChoferId.Get<int>(context);
            var centroId = CentroId.Get<int>(context);
            var usuario = NombreUsuario.Get<string>(context);
            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);
            try
            {
              var inhabilitacionesCamion = repositorio.ListarInhabilitacionCamion(patente, centroId);
              var inhabilitacionesChofer = repositorio.ListarInhabilitacionChofer(choferId, centroId);

                foreach (var i in inhabilitacionesCamion)
                {
                    i.FechaHasta = DateTime.Today.AddDays(-1);
                    i.Comentario = controlRecorrido.Comentario;
                    servicio.Ejecutar(new ModificarInhabilitacionCamion (){Dto = i, Usuario = usuario});
                }
                foreach (var i in inhabilitacionesChofer)
                {
                    i.FechaHasta = DateTime.Today.AddDays(-1);
                    i.Comentario = controlRecorrido.Comentario;
                    servicio.Ejecutar(new ModificarInhabilitacionChofer (){Dto = i, Usuario = usuario});
                }
            }
            catch
            {
                
            }
        }
    }
}
