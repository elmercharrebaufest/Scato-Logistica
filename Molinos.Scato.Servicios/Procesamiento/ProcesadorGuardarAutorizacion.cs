using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorGuardarAutorizacion : ProcesadorComando<GuardarAutorizacionComando>
    {
        public ProcesadorGuardarAutorizacion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(GuardarAutorizacionComando comando)
        {
            var resultado = new Resultado();
            try
            {
                var choferes = Repositorio.Listar<InhabilitacionChofer>(x => x.Chofer.Id == comando.ChoferId && x.Centro.Id == comando.Centro).ToList();
                var camiones = Repositorio.Listar<InhabilitacionCamion>(x => x.Patente == comando.Patente && x.Centro.Id == comando.Centro).ToList();

                foreach (var i in choferes)
                {
                    Repositorio.Agregar(new AutorizacionChofer
                    {
                        Fecha = DateTime.Now,
                        NombreUsuarioResponsable = comando.NombreUsuario,
                        Comentario = comando.Mensaje,
                        InhabilitacionChofer = i

                    });

                }
                foreach (var i in camiones)
                {
                    Repositorio.Agregar(new AutorizacionCamion
                    {
                        Fecha = DateTime.Now,
                        NombreUsuarioResponsable = comando.NombreUsuario,
                        Comentario = comando.Mensaje,
                        InhabilitacionCamion = i

                    });
                }

                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                //Log.Error(e, "Erro al marcar NO rechazado en el recorrido {0}", comando.WorkflowId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
