using System.Collections.ObjectModel;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarConfiguracionDeTabla : ProcesadorComando<ActualizarConfiguracionDeTabla>
    {
        public ProcesadorActualizarConfiguracionDeTabla(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarConfiguracionDeTabla comando)
        {
            var resultado = new Resultado();
            if (!resultado.HayErrores)
            {
                ModificarEntidad(comando);
                Repositorio.GuardarCambios();
            }

            return resultado;
        }

        private void ModificarEntidad(ActualizarConfiguracionDeTabla comando)
        {
            var configuracion = Repositorio.Obtener<ConfiguracionDeTabla>(x => x.Centro.Id == comando.Dto.CentroId && x.Material.Id == comando.Dto.MaterialIdd && x.Usuario.NombreUsuario == comando.Usuario) ??
                                new ConfiguracionDeTabla
                                    {
                                        Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId),
                                        CaracteristicasDeCalidad = new Collection<CaracteristicaDeCalidad>(),
                                        Material = Repositorio.Obtener<Material>(comando.Dto.MaterialIdd),
                                        Usuario = Repositorio.Obtener<Usuario>(x => x.NombreUsuario == comando.Usuario)
                                };
            configuracion.CaracteristicasDeCalidad.Clear();
            if (comando.Dto.CaracteristicasDeCalidadId != null)
            {
                foreach (var caracteristica in comando.Dto.CaracteristicasDeCalidadId.Select(x => Repositorio.Obtener<CaracteristicaDeCalidad>(x)))
                {
                    configuracion.CaracteristicasDeCalidad.Add(caracteristica);
                }
            }
            if (configuracion.Id == 0)
            {
                Repositorio.Agregar(configuracion);
            }
        }
    }


}