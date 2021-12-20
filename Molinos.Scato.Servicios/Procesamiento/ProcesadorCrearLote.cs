using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLote : ProcesadorComando<CrearLote>
    {
        public ProcesadorCrearLote(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }
        public override Resultado Ejecutar(CrearLote comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se está ejecutando ProcesadorCrearLote con Numero de Lote: {0}", comando.Dto.NumeroDeLote);

                if (Repositorio.Existe<Lote>(e => e.NumeroDeLote == comando.Dto.NumeroDeLote))
                {
                    resultado.Error("NumeroDeLote", Textos.Lote_NumeroExistente);
                    return resultado;
                }
                var lote = new Lote
                    {
                        Fecha = comando.Dto.Fecha,
                        Camara = Repositorio.Obtener<Camara>(comando.Dto.CamaraId),
                        NumeroDeLote = comando.Dto.NumeroDeLote,
                        Muestras = new List<MuestraEnvioACamara>()
                    };
                foreach (var muestraDto in comando.Dto.Muestras)
                {
                    var muestra = Repositorio.Obtener<MuestraEnvioACamara>(muestraDto.Id);
                    muestra.NroMuestraTerceros = muestraDto.NroMuestraTerceros;
                    muestra.EstadoMuestra = EstadoMuestra.Enviada;
                    lote.Muestras.Add(muestra);
                }
                Repositorio.Agregar(lote);
                Repositorio.GuardarCambios();
                resultado.Id = lote.Id;
                return resultado;
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorCrearLote con Numero de Lote: {0}", comando.Dto.NumeroDeLote);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
