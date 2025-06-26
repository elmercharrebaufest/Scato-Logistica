using System;
using System.IO;
using System.ServiceModel;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Urenport;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEnviarCartaPorteUnreport : ProcesadorComando<EnviarCartaPorteUnreport>
    {
        private readonly calpesSoap urenport;
        private readonly IConfiguracionProvider configuracion;

        public ProcesadorEnviarCartaPorteUnreport(IRepositorio repositorio, IConversor conversor, ILogger log, Urenport.calpesSoap urenport, IConfiguracionProvider configuracion)
            : base(repositorio, conversor, log)
        {
            this.urenport = urenport;
            this.configuracion = configuracion;
        }

        public override Resultado Ejecutar(EnviarCartaPorteUnreport comando)
        {
            var resultado = new Resultado();
            EnvioUrenport estadoUrenport = null;
            if (comando.EnvioUrenport > 0)
            {
                estadoUrenport = Repositorio.Obtener<EnvioUrenport>(x => x.Id == comando.EnvioUrenport);
            }
            else
            {
                var cp = Repositorio.ObtenerProyeccion((CartaPorte x) => x.Id == comando.CartaPorteId, x => new { Ruta = x.FotoRutaDestino, TieneEntregador = x.Entregador != null, Numero = x.NroCartaPorte});

                if (!cp.TieneEntregador)
                {
                    Log.Debug("La carta de porte no tiene entregador, id: " + comando.CartaPorteId + " , circuito " + comando.WorkflowId);
                    return resultado;
                }
                string rutaReIngreso = null;
                if (string.IsNullOrEmpty(cp.Ruta))
                {
                    rutaReIngreso = Repositorio.ObtenerProyeccion<CartaPorte, string>(x => x.NroCartaPorte == cp.Numero && x.FotoRutaDestino != null, x => x.FotoRutaDestino);
                }
                var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.WorkflowId);
                estadoUrenport = Repositorio.Obtener<EnvioUrenport>(x => x.Recorrido.Id == recorrido.Id && x.TipoDoc == Constantes.TipoDocEnvioUrenport.CartaPorteUrenport);
                if (estadoUrenport == null)
                {
                    estadoUrenport = Repositorio.Agregar(new EnvioUrenport
                    {
                        NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso.TrimStart(new Char[] { '0' }),
                        TipoDoc = Constantes.TipoDocEnvioUrenport.CartaPorteUrenport,
                        Ruta = cp.Ruta ?? rutaReIngreso,
                        Recorrido = recorrido,
                        Estado = EstadoTransmisionASap.Pendiente,
                        Fecha = DateTime.Now
                    });
                }
            }

            if (string.IsNullOrEmpty(estadoUrenport.Ruta))
            {
                var error = "No se encontró la ruta de la imagen asociada a la carta de porte id " + comando.CartaPorteId + " para el circuito" + comando.WorkflowId;
                Log.Error(error);
                estadoUrenport.Estado = EstadoTransmisionASap.Error;
                estadoUrenport.Error = error;
                resultado.Errores.Add("CodigoDeBaja", error);
            }
            else
            {
                try
                {
                    Log.Debug("ProcesadorEnviarCartaPorteUnreport - Inicio la consulta");
                    var response = urenport.Imagen(new ImagenRequest
                    {
                        Body = new ImagenRequestBody
                        {
                            Au = configuracion.AppSettings["UnreportAu"],
                            numCP = estadoUrenport.NumeroDocumentoIngreso,
                            tipo_doc = estadoUrenport.TipoDoc,
                            pic = ObtenerImagen(estadoUrenport.Ruta)
                        }
                    });
                    Log.Debug("ProcesadorEnviarCartaPorteUnreport - Realizo la consulta ");


                    if (!response.Body.ImagenResult.Contains("OK"))
                    {
                        resultado.Errores.Add("CodigoDeBaja", response.Body.ImagenResult);
                        Log.Error("ProcesadorEnviarCartaPorteUnreport -" + response.Body.ImagenResult);
                        estadoUrenport.Error = response.Body.ImagenResult;
                        estadoUrenport.Estado = EstadoTransmisionASap.Error;
                    }
                    else
                    {
                        estadoUrenport.Estado = EstadoTransmisionASap.Correcto;
                        Log.Debug("ProcesadorEnviarCartaPorteUnreport {0} procesada correctamente", estadoUrenport.NumeroDocumentoIngreso);
                    }
                }
                catch (FaultException e)
                {
                    Log.Error(e, "No se pudo registrar en urenport la CP {0}", estadoUrenport.NumeroDocumentoIngreso);
                    resultado.Errores.Add("CodigoDeBaja", e.Message);
                    estadoUrenport.Error = e.Message;
                    estadoUrenport.Estado = EstadoTransmisionASap.Error;
                }
                catch (Exception e)
                {
                    Log.Error(e, "No se pudo registrar en urenport la CP {0}", estadoUrenport.NumeroDocumentoIngreso);
                    resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
                    estadoUrenport.Error = Textos.Error_Generico;
                    estadoUrenport.Estado = EstadoTransmisionASap.Error;
                }
            }

            Repositorio.GuardarCambios();
            return resultado;
        }

        private byte[] ObtenerImagen(string path)
        {
            byte[] buffer = null;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
            }
            return buffer;
        }
    }
}