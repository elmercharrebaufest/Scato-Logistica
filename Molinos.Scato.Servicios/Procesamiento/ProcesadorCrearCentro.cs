using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCentro : ProcesadorCrear<CrearCentro, Centro>
    {
        public ProcesadorCrearCentro(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override Centro CrearEntidad(CrearCentro comando)
        {
            var centro = Conversor.Convertir<CentroDto, Centro>(comando.Dto);
            centro.CamaraDefault = Repositorio.Obtener<Camara>(comando.Dto.CamaraId);
            centro.Provincia = Repositorio.Obtener<Provincia>(comando.Dto.ProvinciaId);
            centro.Localidad = Repositorio.Obtener<Localidad>(comando.Dto.LocalidadId);
            centro.UsuariosAsociados = Repositorio.Listar<Usuario>(x => x.NombreUsuario == comando.NombreUsuario);
            return centro;
        }

        protected override void Validar(CrearCentro comando, Resultado resultado)
        {
            if (Repositorio.Existe<Centro>(x => x.CodigoSAP == comando.Dto.CodigoSAP))
            {
                resultado.Error("CodigoSAP", Textos.Centro_CodigoSAPExistente);
            }
        }
    }
}