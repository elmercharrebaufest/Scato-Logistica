using System;
using System.Collections.Generic;
using System.Text;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLoteInase : ProcesadorComando<CrearLoteInase>
    {
        public ProcesadorCrearLoteInase(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }
        public override Resultado Ejecutar(CrearLoteInase comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se está ejecutando ProcesadorCrearLoteInase");
                var lote = new LoteInase
                    {
                        Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId),
                        Fecha = comando.Dto.Fecha,
                        NombreUsuario = comando.Dto.NombreUsuario,
                        NumeroDeLote = ""
                };
                Repositorio.Agregar(lote);
                Repositorio.GuardarCambios();
                foreach (var muestraDto in comando.Dto.Muestras)
                {
                    var muestra = Repositorio.Obtener<MuestraDeInase>(muestraDto.Id);
                    muestra.MuestraEnviada = true;
                    muestra.LoteInase = lote;
                }
                lote.NumeroDeLote = GenerarNumeroDeLote(lote);
                Log.Info("Se ejecutó ProcesadorCrearLoteInase con Numero de Lote: {0}", lote.NumeroDeLote);
                Repositorio.GuardarCambios();
                return resultado;
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorCrearLoteInase");
                resultado.Error("", e.Message);
            }
            return resultado;
        }

        private string GenerarNumeroDeLote(LoteInase lote)
        {
            var loteId = lote.Id.ToString();
            var numeroLote = new StringBuilder();
            numeroLote.Append(lote.Centro.Descripcion.Replace(" ", "").Substring(0, 3).ToUpper());
            numeroLote.Append("INA");
            numeroLote.Append(loteId.PadLeft(6, '0'));
            return  numeroLote.ToString();
        }
    }
}
