using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarModalidadPuestoDeTrabajo : ProcesadorComando<ModificarModalidadPuestoDeTrabajo>
    {
        public ProcesadorModificarModalidadPuestoDeTrabajo(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarModalidadPuestoDeTrabajo comando)
        {
            var resultado = new Resultado();

            var puesto = Repositorio.Obtener<PuestoDeTrabajo>(comando.IdPuesto);
            var centro = Repositorio.Obtener<Centro>(comando.CentroId);
            puesto.Automatico = comando.Automatico;

            var cambioModalidad = new PuestoDeTrabajoModificacionModalidad
                {
                    Automatico = comando.Automatico,
                    Motivo = comando.Motivo,
                    PuestoDeTrabajo = puesto,
                    NombreUsuarioResponsable = comando.Usuario,
                    Fecha = DateTime.Now,
                    Centro = centro
                };

            Repositorio.Agregar(cambioModalidad);
            Repositorio.GuardarCambios();

            return resultado;
        }
    }
}
