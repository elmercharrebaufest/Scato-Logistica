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
    public class ProcesadorGuardarEtiquetasPuerto : ProcesadorCrear<GuardarEtiquetaPuerto, ImpEtiquetaPuerto>
    {
        public ProcesadorGuardarEtiquetasPuerto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }
        protected override ImpEtiquetaPuerto CrearEntidad(GuardarEtiquetaPuerto comando)
        {
            var entidad = Conversor.Convertir<ImpEtiquetaPuertoDto, ImpEtiquetaPuerto>(comando.Etiqueta);

            entidad.FechaCreacion = DateTime.Now;

            Repositorio.Agregar(entidad);

            return entidad;
        }

        protected override void Validar(GuardarEtiquetaPuerto comando, Resultado resultado)
        {
            //if(Repositorio.Existe<ImpEtiquetaPuerto>(x => x.Vapor == comando.Etiqueta.Vapor))
            //{
            //    resultado.Error("", "Ya existe una etiqueta para el vapor " + comando.Etiqueta.Vapor);
            //}
        }
    }
}
