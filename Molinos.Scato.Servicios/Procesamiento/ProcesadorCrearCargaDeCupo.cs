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
    public class ProcesadorCrearCargaDeCupo : ProcesadorCrear<CrearCargaDeCupo, CargaDeCupo>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador servicioOrquestador;

        public ProcesadorCrearCargaDeCupo(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioOrquestador servicioOrquestador)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
        }

        protected override CargaDeCupo CrearEntidad(CrearCargaDeCupo comando)
        {
            EliminarDuplicadas(comando);
            var cupo = Conversor.Convertir<CargaDeCupoDto, CargaDeCupo>(comando.Dto);
            cupo.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            cupo.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            cupo.PuestoDeTrabajo = Repositorio.Obtener<PuestoDeTrabajo>(comando.Dto.PuestoDeTrabajoId);
            cupo.Reingresado = Repositorio.Existe<CargaDeCupo>(x => (( x.NumeroCartaPorte != null && x.NumeroCartaPorte == comando.Dto.NumeroCartaPorte && x.Patente== comando.Dto.Patente) || (x.CTG != null && x.CTG == comando.Dto.CTG && x.Patente == comando.Dto.Patente)) &&
                                                                
                                                                x.Centro.Id == comando.Dto.CentroId &&
                                                                x.Recorrido != null &&
                                                                x.Recorrido.Rechazado);//Tarea - (ANS-128) - Número de CP para camión reingresado - SR58195
            return cupo;
        }

        protected void EliminarDuplicadas(CrearCargaDeCupo comando)
        {
            var entidadDuplicada = Repositorio.Listar<CargaDeCupo>(x => x.Numero == comando.Dto.Numero && comando.Dto.Numero != "" && comando.Dto.Numero != null && x.Recorrido == null && x.Centro.Id == comando.Dto.CentroId);

            if (entidadDuplicada != null)
            {
                Log.Debug($"Camion {comando.Dto.Patente} eliminado por duplicidad de Tarjeta {comando.Dto.Numero}");
                foreach(var i in entidadDuplicada)
                {
                    Repositorio.Remover(i);
                }                
            }

            var entidadDuplicada2 = Repositorio.Listar<CargaDeCupo>(x => x.NumeroCartaPorte == comando.Dto.NumeroCartaPorte && comando.Dto.NumeroCartaPorte != "" && comando.Dto.NumeroCartaPorte != null && x.Recorrido == null && x.Centro.Id == comando.Dto.CentroId);

            if (entidadDuplicada2 != null)
            {
                Log.Debug($"Camion {comando.Dto.Patente} eliminado por duplicidad de NumeroCartaPorte {comando.Dto.NumeroCartaPorte}");
                foreach (var i in entidadDuplicada2)
                {
                    Repositorio.Remover(i);
                }
            }

            var entidadDuplicada3 = Repositorio.Listar<CargaDeCupo>(x => x.Patente == comando.Dto.Patente && comando.Dto.Patente != "" && comando.Dto.Patente != null && x.Recorrido == null && x.Centro.Id == comando.Dto.CentroId);

            if (entidadDuplicada3 != null)
            {
                Log.Debug($"Camion {comando.Dto.Patente} eliminado por duplicidad de Patente {comando.Dto.Patente}");
                foreach (var i in entidadDuplicada3)
                {
                    Repositorio.Remover(i);
                }
            }

            var entidadDuplicada4 = Repositorio.Listar<CargaDeCupo>(x => comando.Dto.CPE && x.CTG == comando.Dto.CTG && comando.Dto.CTG != "" && comando.Dto.CTG != null && x.Recorrido == null && x.Centro.Id == comando.Dto.CentroId);

            if (entidadDuplicada4 != null)
            {
                Log.Debug($"Camion {comando.Dto.Patente} eliminado por duplicidad de CTG {comando.Dto.CTG}");
                foreach (var i in entidadDuplicada4)
                {
                    Repositorio.Remover(i);
                }
            }
        }

        protected override void Validar(CrearCargaDeCupo comando, Resultado resultado)
        {
            if (Repositorio.Existe<CargaDeCupo>(
                e => e.Cupo == comando.Dto.Cupo
                && e.Centro.Id == comando.Dto.CentroId
                && ((e.Recorrido != null && (!e.Recorrido.Rechazado || (e.Recorrido.Rechazado && !e.Recorrido.Terminado))) || (e.Numero != comando.Dto.Numero && e.Recorrido == null))
                && !e.SinCupo && !comando.Dto.SinCupo))
            {
                resultado.Error("Cupo2", "El cupo ya fue utilizado");
            }
            if (comando.Dto.NumeroCartaPorte != "" && 
                comando.Dto.NumeroCartaPorte != null && 
                Repositorio.Existe<CargaDeCupo>(
                e => e.NumeroCartaPorte == comando.Dto.NumeroCartaPorte
                && e.Centro.Id == comando.Dto.CentroId
                && (e.Recorrido != null && (!e.Recorrido.Rechazado || (e.Recorrido.Rechazado && !e.Recorrido.Terminado)))))
            {
                resultado.Error("Cupo3", $"La CP {comando.Dto.NumeroCartaPorte} ya se encuentra en circuito");
            }

            if (comando.Dto.CPE &&
                comando.Dto.CTG != "" &&
                comando.Dto.CTG != null &&
                Repositorio.Existe<CargaDeCupo>(
                e => e.CTG == comando.Dto.CTG
                && e.Centro.Id == comando.Dto.CentroId
                && (e.Recorrido != null && (!e.Recorrido.Rechazado || (e.Recorrido.Rechazado && !e.Recorrido.Terminado)))))
            {
                resultado.Error("Cupo4", $"La CPE {comando.Dto.CTG} ya se encuentra en circuito");
            }

            var date = DateTime.Now;
            if (!resultado.HayErrores && comando.Dto.PuestoDeTrabajoId != 0 && comando.EsGarita)
            {
                var sinFotoCartaPorte = Repositorio.ObtenerProyeccion<PuestoDeTrabajo, bool>(x => x.Id == comando.Dto.PuestoDeTrabajoId, x => x.SinFotoCartaPorte);

                if (!sinFotoCartaPorte)
                {
                    if((comando.Dto.CPE && !string.IsNullOrEmpty(comando.Dto.ImagenCartaPorte)) || !comando.Dto.CPE)
                    {
                        var path = servicioComandos.Ejecutar(
                        new GuardarfotoMesaDigitalizacion
                        {
                            EsTemporal = true,
                            NumeroDeTarjeta = comando.Dto.Numero + "-" + comando.Dto.NumeroCartaPorte,
                            Fecha = date,
                            FotoMesaDigitalizacion = comando.Dto.ImagenCartaPorte,
                            Directorio = comando.Dto.FotoRutaDestino
                        }) as ResultadoGuardarFoto;

                        if (path != null && !string.IsNullOrEmpty(path.Path))
                        {
                            comando.Dto.FotoRutaDestino = path.Path;
                            ((ResultadoCrear)resultado).Mensaje = path.Path;
                            if(comando.Dto.Especial)
                            {
                                var resultadoSustentable = servicioComandos.Ejecutar(new AgregarMarcaSustentable
                                {
                                    RutaFotoCP = comando.Dto.FotoRutaDestino,
                                    CodigoCentroSap = comando.Dto.CentroCodigoSap,
                                    NroDocumento = comando.Dto.CTG,
                                    Patente = comando.Dto.Patente,
                                    SoloDibujar = false
                                }) as ResultadoGuardarFoto;
                                if (resultadoSustentable != null && !string.IsNullOrEmpty(resultadoSustentable.Path))
                                {
                                    comando.Dto.FotoRutaSustentable = resultadoSustentable.Path;
                                    ((ResultadoCrear)resultado).PathSustentable = resultadoSustentable.Path;
                                }
                                else
                                {
                                    resultado.Error("ResultadoSustentable", "No se pudo guardar la foto CP con sello sustentable");
                                }
                            }
                        }
                        else
                        {
                            resultado.Error("Imp", "No se pudo guardar la foto de la carta de porte");
                        }
                    }
                }

            }

            if (!resultado.HayErrores && comando.EsGarita)
            {
                var path = TomarFotoCamion(comando.Dto, date);
                if (!string.IsNullOrEmpty(path))
                {
                    comando.Dto.FotoCamionRutaDestino = path;
                }
                else
                {
                    resultado.Error("Imp", "No se pudo guardar la foto del camión");
                }
            }
        }

        private string TomarFotoCamion(CargaDeCupoDto model, DateTime date)
        {
            var puestoDeTrabajo = Repositorio.Obtener<PuestoDeTrabajo>(model.PuestoDeTrabajoId);

            if (puestoDeTrabajo != null && puestoDeTrabajo.VideoCamaras != null && puestoDeTrabajo.VideoCamaras.Any())
            {
                date = date.AddSeconds(1);
                var camaras = puestoDeTrabajo.VideoCamaras.OrderBy(x => x.Id).ToList();
                if (camaras.Count > 1) //remuevo la última cámara ya que se uso para scanear la CP
                {
                    camaras.RemoveAt(camaras.Count - 1);
                }
                foreach (var videoCamara in camaras)
                {
                    try
                    {
                        Log.Debug("Ejecutando Camara para el puesto: {0}", model.PuestoDeTrabajoId);
                        var comando = new EjecutarTomarFoto
                        {
                            CodigoDispositivo = videoCamara.Codigo,
                            FilePath = videoCamara.Directorio,
                            SubPath = "temp",
                            FileName = FotoCamionHelper.GenerarNombreTemporal(model.Numero + "-" + model.NumeroCartaPorte , date)
                        };
                        var resultadoFoto = servicioOrquestador.Ejecutar(comando);

                        if (resultadoFoto.Mensaje.Codigo != 0)
                        {
                            Log.Error("Fallo la Apertura del dispositivo: {0}", resultadoFoto.Mensaje.Descripcion);
                        }
                        else
                        {
                            return comando.FilePath + "\\" + comando.SubPath + "\\" + comando.FileName + ".jpeg";
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Fallo la foto del dispositivo: {0}", puestoDeTrabajo.VideoCamaras);
                    }
                }
            }
            return string.Empty;
        }
    }
}
