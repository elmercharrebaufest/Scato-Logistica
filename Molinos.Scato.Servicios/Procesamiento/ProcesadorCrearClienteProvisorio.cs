using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearClienteProvisorio : ProcesadorCrear<CrearClienteProvisorio, Cliente>
    {
        public ProcesadorCrearClienteProvisorio(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Cliente CrearEntidad(CrearClienteProvisorio comando)
        {
            var date = DateTime.Now;
            var rnd = new Random();
            var data = 'D' + rnd.Next(10, 100).ToString() + date.Minute.ToString() + date.Second.ToString() + date.Millisecond.ToString();
            comando.Dto.CodigoSap = data;
            comando.Dto.Activo = true;
            comando.Dto.EsClienteProvisorio = true;
            var usarioCreado = Conversor.Convertir<ClienteDto, Cliente>(comando.Dto);
            return usarioCreado;
        }

        protected override void Validar(CrearClienteProvisorio comando, Resultado resultado)
        {
            if (Repositorio.Existe<Cliente>(e => e.Cuit == comando.Dto.Cuit ))
            {
                resultado.Error("Cuit", Textos.Cliente_CuitExistente);
            }
        }
    }
}
