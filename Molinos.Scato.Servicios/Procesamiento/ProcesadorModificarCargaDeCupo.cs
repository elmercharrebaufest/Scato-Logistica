using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCargaDeCupo : ProcesadorComando<ModificarCargaDeCupo>
    {

        public ProcesadorModificarCargaDeCupo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarCargaDeCupo comando)
        {
            var cupo = Repositorio.Obtener<CargaDeCupo>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, cupo);
            Repositorio.GuardarCambios();
            return new Resultado();
        }
    }
}
