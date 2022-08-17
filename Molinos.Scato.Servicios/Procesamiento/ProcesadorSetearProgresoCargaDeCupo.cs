using System;
using System.Collections.ObjectModel;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorSetearProgresoCargaDeCupo : ProcesadorComando<SetearProgresoCargaDeCupo>
    {
        private readonly IConfiguracionProvider configuracion;

        public ProcesadorSetearProgresoCargaDeCupo(IRepositorio repositorio, IConversor conversor, ILogger log, IConfiguracionProvider configuracion)
            : base(repositorio, conversor, log)
        {
            this.configuracion = configuracion;
        }

        public override Resultado Ejecutar(SetearProgresoCargaDeCupo comando)
        {
            var resultado = new Resultado();
            var cargaCupo = Repositorio.Obtener<CargaDeCupo>(x => x.Id == comando.Id);
            if(cargaCupo == null)
            {
                resultado.Error("0", $"No exite el registro (${comando.Id}) en la tabla CargaDeCupo");
                return resultado;
            }

            cargaCupo.EnProgresoAutomatico = comando.EnProgresoAutomatico;

            Repositorio.GuardarCambios();

            return resultado;
        }
    }


}
