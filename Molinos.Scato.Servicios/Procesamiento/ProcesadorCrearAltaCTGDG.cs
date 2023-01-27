using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAltaCTGDG : ProcesadorCrear<CrearAltaCTGDG, AltaCTG>
    {
        public ProcesadorCrearAltaCTGDG(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override AltaCTG CrearEntidad(CrearAltaCTGDG comando)
        {
            var entidad = Conversor.Convertir<AltaCTGDto, AltaCTG>(comando.Dto);
            var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.Dto.WorkflowId);
            var cartaPorteDerivadoGranario = new CartaPorteDerivadoGranario()
            {
                    NroCTG = comando.Dto.CodigoCTG,
                    NroOrden = comando.Dto.NroOrden.PadLeft(8, '0'),
                    Sucursal = comando.Dto.Sucursal.PadLeft(5, '0'),
        };
            cartaPorteDerivadoGranario.Recorrido = recorrido;
            entidad.CartaPorteDerivadoGranario = cartaPorteDerivadoGranario;
            return entidad;
        }

        protected override void Validar(CrearAltaCTGDG comando, Resultado resultado)
        {
            if (string.IsNullOrEmpty(comando.Dto.CodigoCTG))
            {
                resultado.Error("CodigoCTG", string.Format(Textos.Error_Requerido, "CodigoCTG"));
            }

            if (string.IsNullOrEmpty(comando.Dto.Sucursal))
            {
                resultado.Error("Sucursal", string.Format(Textos.Error_Requerido, "Sucursal"));
            }

            if (string.IsNullOrEmpty(comando.Dto.NroOrden))
            {
                resultado.Error("NroOrden", string.Format(Textos.Error_Requerido, "NroOrden"));
            }

            if (!Repositorio.Existe<Recorrido>(x => x.InstanciaWorkflow == comando.Dto.WorkflowId))
            {
                resultado.Error("Recorrido", string.Format("No existe un recorrido"));
            }
        }
    }
}
