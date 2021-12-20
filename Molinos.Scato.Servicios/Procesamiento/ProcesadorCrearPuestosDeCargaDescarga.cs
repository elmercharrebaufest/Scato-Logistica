using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearPuestosDeCargaDescarga : ProcesadorCrear<CrearPuestosDeCargaDescarga, PuestosDeCargaDescarga>
    {
        public ProcesadorCrearPuestosDeCargaDescarga(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override PuestosDeCargaDescarga CrearEntidad(CrearPuestosDeCargaDescarga comando)
        {
            return new PuestosDeCargaDescarga
            {
                Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId),
                Codigo = comando.Dto.Codigo,
                Nombre = comando.Dto.Nombre,
                PuestoDeTrabajo = Repositorio.Obtener<PuestoDeTrabajo>(comando.Dto.PuestoDeTrabajoId),
                EsSojaSustentable = comando.Dto.EsSojaSustentable
            };
        }

        protected override void Validar(CrearPuestosDeCargaDescarga comando, Resultado resultado)
        {
            if (Repositorio.Existe<PuestosDeCargaDescarga>(e => e.Codigo == comando.Dto.Codigo && e.Centro.Id == comando.Dto.CentroId && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Codigo", Textos.Hidraulica_CodigoExistente);
            }
        }
    }
}
