using System;
using System.Configuration;
using System.Threading.Tasks;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServicioImpresion;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorImprimirCartaPorteMesa : ProcesadorComando<ImprimirCartaPorteMesa>
    {
        public ProcesadorImprimirCartaPorteMesa(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioImpresion servicioImpresion)
            : base(repositorio, conversor, log)
        {
            ServicioImpresion = servicioImpresion;
        }

        public IServicioImpresion ServicioImpresion { get; }

        public override Resultado Ejecutar(ImprimirCartaPorteMesa comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Debug("Iniciando impresión de ImprimirCartaPorteMesa en la impresora: " + comando.Dto.Impresora);
                comando.Dto.Impresora = comando.Dto.Impresora;
                comando.CantidadCopias = 1;
                ServicioImpresion.Ejecutar(comando);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Dto.Impresora);
                resultado.Errores.Add("", String.Format(Textos.ImpresoraNoConecta, comando.Dto.Impresora));
            }
            return resultado;
        }
    }
}
