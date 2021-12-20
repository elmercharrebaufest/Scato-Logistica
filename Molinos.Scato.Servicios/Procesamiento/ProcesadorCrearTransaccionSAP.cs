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
    public class ProcesadorCrearTransaccionSAP : ProcesadorCrear<CrearTransaccionSAP, TransaccionSAP>
    {
        public ProcesadorCrearTransaccionSAP(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override TransaccionSAP CrearEntidad(CrearTransaccionSAP comando)
        {
            var transaccionSAPEditada = Conversor.Convertir<TransaccionSAPDto, TransaccionSAP>(comando.Dto);
            transaccionSAPEditada.CentroOrigen = Repositorio.Obtener<Centro>(Convert.ToInt32(comando.Dto.CentroOrigenId));
            transaccionSAPEditada.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            transaccionSAPEditada.TipoComercial = Repositorio.Obtener<TipoComercial>(comando.Dto.TipoComercialId);
            return transaccionSAPEditada;
        }

        protected override void Validar(CrearTransaccionSAP comando, Resultado resultado)
        {
            var centroOrigen = Convert.ToInt32(comando.Dto.CentroOrigenId);
            
            if (Repositorio.Existe<TransaccionSAP>(e => e.Material.Id == comando.Dto.MaterialId &&
                e.TipoComercial.Id == comando.Dto.TipoComercialId && e.CentroOrigen.Id == centroOrigen &&
                e.FuncionSAP == comando.Dto.FuncionSAP && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("", Textos.TransaccionSAP_TransaccionExistente);
            }
        }
    }
}
