using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarEtiquetaPuerto : ProcesadorComando<EliminarEtiquetaPuerto>
    {
        public ProcesadorEliminarEtiquetaPuerto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(EliminarEtiquetaPuerto comando)
        {
            var resultado = new Resultado();
            var id = comando.UsuarioId;
            try
            {
                var etiquetas = Repositorio.Listar<Dominio.Entidades.ImpEtiquetaPuerto>(x => x.Usuario_Id == comando.UsuarioId);

                if(etiquetas != null)
                {
                    foreach (var etiqueta in etiquetas)
                    {
                        Repositorio.Remover<Dominio.Entidades.ImpEtiquetaPuerto>(etiqueta);
                    }

                    Repositorio.GuardarCambios();
                }
            }
            catch (EntidadReferenciadaException)
            {
                resultado.Error("", Textos.Error_EliminarReferenciado);
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrio al eliminar la entidad del tipo {0} - IdUsuario {1}", typeof(EliminarEtiquetaPuerto).Name, id );
                resultado.Error("", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}
