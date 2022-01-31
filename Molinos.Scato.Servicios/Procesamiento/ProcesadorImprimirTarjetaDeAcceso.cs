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
    public class ProcesadorImprimirTarjetaDeAcceso : ProcesadorComando<ImprimirTarjetaDeAcceso>
    {
        public ProcesadorImprimirTarjetaDeAcceso(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioImpresion servicioImpresion)
            : base(repositorio, conversor, log)
        {
            ServicioImpresion = servicioImpresion;
        }

        public IServicioImpresion ServicioImpresion { get; }

        public override Resultado Ejecutar(ImprimirTarjetaDeAcceso comando)
        {
            var resultado = new Resultado();
            var impresion = ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(comando.Dto.Codigo, comando.Dto.CentroId, comando.Dto.PuestoDeTrabajoId);
            try
            {
                if (impresion == null)
                {
                    Log.Error("Error al imprimir en la impresora: codigo de impresion no encontrado");
                    resultado.Errores.Add("", String.Format(Textos.Error_CodigoDeImpresion, comando.Dto.Codigo));
                    return resultado;
                }

                Log.Debug("Iniciando impresión de ResumenDeRecepcion en la impresora: " + impresion.Impresora.Direccion + " OrigenImpresion: "+ comando?.OrigenImpresion + " NumeroTarjeta: " + comando?.Dto?.Numero + " PuestroTrabajoId: " + comando?.Dto?.PuestoDeTrabajoId);
                comando.Dto.Impresora = impresion.Impresora.Direccion;
                ServicioImpresion.Ejecutar(comando);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Dto.Impresora);
                resultado.Errores.Add("", String.Format(Textos.ImpresoraNoConecta, impresion.Impresora.Descripcion));
            }
            return resultado;
        }

        public DocumentoDeImpresionPorCentro ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(string codigo, int centroId, int puestoDeTrabajoId)
        {
            DocumentoDeImpresionPorCentro documento = null;
            if (puestoDeTrabajoId != 0)
            {
                documento = Repositorio.Obtener<DocumentoDeImpresionPorCentro>(x => x.DocumentoDeImpresion.Codigo == codigo && x.Centro.Id == centroId && x.PuestoDeTrabajo.Id == puestoDeTrabajoId);
            }
            return documento ?? Repositorio.Obtener<DocumentoDeImpresionPorCentro>(x => x.DocumentoDeImpresion.Codigo == codigo && x.Centro.Id == centroId && x.PuestoDeTrabajo == null);
        }
    }
}
