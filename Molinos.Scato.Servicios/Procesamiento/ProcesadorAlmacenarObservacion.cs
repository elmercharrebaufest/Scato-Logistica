using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorAlmacenarObservacion : ProcesadorComando<AlmacenarObservacion>
    {
        public ProcesadorAlmacenarObservacion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(AlmacenarObservacion comando)
        {
            var resultado = new Resultado();
            try
            {
                if (comando.Dto.Id != 0)
                {
                    ModificarEntidad(comando);
                }
                else
                {
                    var entidad = CrearEntidad(comando);
                    Repositorio.Agregar(entidad);
                }
            }
            catch (Exception)
            {
                resultado.Error("", Textos.Observacion_ErrorEnLaCarga);
            }
            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }
            return resultado;
        }

        protected void ModificarEntidad(AlmacenarObservacion comando)
        {
            var observacionEditada = Repositorio.Obtener<Observacion>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, observacionEditada);
        }

        protected Observacion CrearEntidad(AlmacenarObservacion comando)
        {
            return Conversor.Convertir<ObservacionDto, Observacion>(comando.Dto);
        }
    }
}
