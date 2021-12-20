using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCambiarPinchazos : ProcesadorComando<CambiarPinchazosComando>
    {
        public ProcesadorCambiarPinchazos(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CambiarPinchazosComando comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se está ejecutando ProcesadorCambiarPinchazos para el centro: {0}", comando.CentroId);
                var pinchazo = Conversor.Convertir<PinchazosPorCaladaDto, PinchazosPorCalada>(comando.Dto);
                pinchazo.Id = -1;
                pinchazo.Centro_Id = comando.CentroId;

                var usuario = Repositorio.Obtener<Usuario>(x=>x.NombreUsuario == comando.Dto.Usuario.NombreUsuario);
                pinchazo.Usuario = null;
                pinchazo.Usuario_Id = usuario.Id;

                Repositorio.Agregar(pinchazo);
                Repositorio.GuardarCambios();

                //enviar email
                
                resultado.Id = pinchazo.Id;
                return resultado;
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorCambiarPinchazos para el centro: {0}", comando.CentroId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
