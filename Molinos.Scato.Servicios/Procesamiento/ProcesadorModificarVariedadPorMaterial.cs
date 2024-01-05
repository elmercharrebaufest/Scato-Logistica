using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarVariedadPorMaterial : ProcesadorModificar<ModificarVariedadPorMaterial>
    {
        public ProcesadorModificarVariedadPorMaterial(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarVariedadPorMaterial comando)
        {
            string usuario = comando.Usuario;
            DateTime fecha = DateTime.Now;
            string codigoFondoNegro = "#000000";
            string codigoTextoBlanco = "#FFFFFF";

            List<TipoVariedadDto> variedades = comando.Dto.ToList();
            List<TipoVariedadPorMaterial> materialVariedad = Repositorio.Listar<TipoVariedadPorMaterial>(c => c.MaterialId == comando.IdMaterial).ToList();

            //Obtiene los que van a ser marcados como borrados, estan en la base pero no en comando
            var borrados = materialVariedad.Where(a => !comando.Dto.Any(b => b.Id.Equals(a.TipoVariedadId))).ToList();
            if (borrados.Any())
            {
                Repositorio.RemoverTodos(borrados);
            }


            //Obtiene los nuevos registros que no estan en base
            var nuevos = variedades.Where(a => !materialVariedad.Any(b => b.TipoVariedadId.Equals(a.Id))).ToList();
            if (nuevos.Any())
            {
                nuevos.ForEach(f =>
                {
                    var tipoMaterial = new TipoVariedadPorMaterial
                    {
                        MaterialId = comando.IdMaterial,
                        TipoVariedadId = f.Id,
                        ColorFondo = string.IsNullOrEmpty(f.ColorFondo) ? codigoFondoNegro : f.ColorFondo,
                        ColorTexto = string.IsNullOrEmpty(f.ColorTexto) ? codigoTextoBlanco : f.ColorTexto,
                        CreadoPor = usuario,
                        FechaCreacion = fecha
                    };
                    Repositorio.Agregar(tipoMaterial);
                });
            }

            Repositorio.GuardarCambios();
        }

        protected override void Validar(ModificarVariedadPorMaterial comando, Resultado resultado)
        {
        }
    }
}