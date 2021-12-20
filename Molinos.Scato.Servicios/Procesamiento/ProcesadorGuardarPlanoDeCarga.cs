using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorGuardarPlanoDeCarga : ProcesadorModificar<GuardarPlanoDeCarga>
    {
        public ProcesadorGuardarPlanoDeCarga(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(GuardarPlanoDeCarga comando)
        {
            var planoDeCarga = Repositorio.Obtener<PlanoDeCarga>(comando.Dto.Id);

            //PROCESO PARA EL HISTORICO
            if (planoDeCarga.FechaDeCreacion == null)
                planoDeCarga.FechaDeCreacion = DateTime.Now;
            else
            {
                planoDeCarga.FechaDeModificacion = DateTime.Now;

                var planodecargahistorico = Repositorio.Agregar(new PlanoDeCargaHistorico
                {
                    PlanoDeCarga = planoDeCarga,
                    Observaciones = planoDeCarga.Observaciones,
                    CaladoSalida = planoDeCarga.CaladoSalida,
                    Estiba = planoDeCarga.Estiba,
                    AgenciaControlPrivado = planoDeCarga.AgenciaControlPrivado,
                    Cargado = planoDeCarga.Cargado,
                    Enviado = planoDeCarga.Enviado,
                    DefensasMoviles = planoDeCarga.DefensasMoviles,
                    FilePathPlano = planoDeCarga.FilePathPlano,
                    FilePathSecuencia = planoDeCarga.FilePathSecuencia,
                    Fumigacion = planoDeCarga.Fumigacion,
                    EmpresaFumigadora = planoDeCarga.EmpresaFumigadora,
                    FechaDeCreacion = planoDeCarga.FechaDeCreacion.Value,
                    FechaDeModificacion = planoDeCarga.FechaDeModificacion.Value,
                    Usuario = planoDeCarga.Usuario,
                    FechaDeFinalizacion = planoDeCarga.FechaDeFinalizacion,
                    UsuarioFinalizacion = planoDeCarga.UsuarioFinalizacion
                });

                Repositorio.GuardarCambios();

                if (planoDeCarga.PlanoDeCargaBodega != null)
                {
                    foreach (var planoBodega in planoDeCarga.PlanoDeCargaBodega.Where(x => x.Cantidad > 0))
                    {
                        Repositorio.Agregar(new PlanoDeCargaBodegaHistorico
                        {
                            BodegaParcel = planoBodega.BodegaParcel,
                            Cantidad = planoBodega.Cantidad,
                            MaterialPuerto = planoBodega.MaterialPuerto,
                            Condicion = planoBodega.Condicion,
                            SfFull = planoBodega.SfFull,
                            Destino = planoBodega.Destino,
                            PlanoDeCargaHistorico = planodecargahistorico,
                            TanqueDeAbordo = planoBodega.TanqueDeAbordo
                        });
                    }
                }
                //Repositorio.GuardarCambios();

                if (planoDeCarga.CargaComercial != null)
                {
                    foreach (var car in planoDeCarga.CargaComercial.Where(x => x.Cantidad > 0))
                    {
                        Repositorio.Agregar(new CargaComercialHistorico
                        {
                            Cantidad = car.Cantidad,
                            Exportador = car.Exportador,
                            MaterialPuerto = car.MaterialPuerto,
                            PlanoDeCargaHistorico = planodecargahistorico
                        });
                    }
                }

                if (planoDeCarga.AgentesControlPrivado != null)
                {
                    foreach (var agente in planoDeCarga.AgentesControlPrivado)
                    {
                        Repositorio.Agregar(new PlanoDeCargaAgenteControlPrivadoHistorico
                        {
                            AgenteControlPrivado = agente,
                            PlanoDeCargaHistorico = planodecargahistorico
                        });
                    }
                }

                Repositorio.GuardarCambios();
            }
            //PROCESO PARA EL HISTORICO

            if (comando.Dto.Estiba != null)
                planoDeCarga.Estiba = Repositorio.Obtener<Estiba>(comando.Dto.Estiba.Id);
            else
                planoDeCarga.Estiba = null;

            if (comando.Dto.AgenciaControlPrivado != null)
                planoDeCarga.AgenciaControlPrivado = Repositorio.Obtener<AgenciaControlPrivado>(comando.Dto.AgenciaControlPrivado.Id);
            else
                planoDeCarga.AgenciaControlPrivado = null;

            planoDeCarga.Observaciones = comando.Dto.Observaciones;
            planoDeCarga.DefensasMoviles = comando.Dto.DefensasMoviles;
            planoDeCarga.Cargado = true;
            planoDeCarga.Enviado = comando.Dto.Enviado;
            planoDeCarga.CaladoSalida = comando.Dto.CaladoSalida;
            planoDeCarga.Fumigacion = comando.Dto.Fumigacion;
            planoDeCarga.EmpresaFumigadora = comando.Dto.EmpresaFumigadora;
            planoDeCarga.Usuario = comando.Dto.Usuario;

            if (comando.Dto.UsuarioFinalizacion != null)
            {
                planoDeCarga.FechaDeFinalizacion = DateTime.Now;
                planoDeCarga.UsuarioFinalizacion = comando.Dto.UsuarioFinalizacion;
            }

            planoDeCarga.AgentesControlPrivado.Clear();
            if (comando.Dto.AgentesControlPrivado != null)
            {
                foreach (var agente in comando.Dto.AgentesControlPrivado)
                {
                    planoDeCarga.AgentesControlPrivado.Add(Repositorio.Obtener<AgenteControlPrivado>(agente.Id));
                }
            }

            Repositorio.RemoverTodos(planoDeCarga.PlanoDeCargaBodega.ToList());
            if (comando.Dto.PlanoDeCargaBodegas != null)
            {
                foreach (var pla in comando.Dto.PlanoDeCargaBodegas.Where(x => x.Cantidad > 0))
                {
                    var destino = pla.Destino != null ? Repositorio.Obtener<Destino>(pla.Destino.Id) : null;
                    var materialPuerto = pla.MaterialPuerto != null ? Repositorio.Obtener<MaterialPuerto>(pla.MaterialPuerto.Id) : null;
                    planoDeCarga.PlanoDeCargaBodega.Add(new PlanoDeCargaBodega
                    {
                        BodegaParcel = pla.BodegaParcel,
                        Cantidad = pla.Cantidad,
                        Condicion = pla.Condicion,
                        Destino = destino,
                        PlanoDeCarga = planoDeCarga,
                        MaterialPuerto = materialPuerto,
                        SfFull = pla.SfFull,
                        TanqueDeAbordo = pla.TanqueDeAbordo
                    });
                }                
            }
                        
            Repositorio.RemoverTodos(planoDeCarga.CargaComercial.ToList());
            if (comando.Dto.CargasComerciales != null)
            {
                foreach (var car in comando.Dto.CargasComerciales.Where(x => x.Cantidad > 0))
                {
                    planoDeCarga.CargaComercial.Add(new CargaComercial
                    {
                        Cantidad = car.Cantidad,
                        Exportador = Repositorio.Obtener<Exportador>(car.Exportador.Id),
                        MaterialPuerto = Repositorio.Obtener<MaterialPuerto>(car.MaterialPuerto.Id),
                        PlanoDeCarga = planoDeCarga
                    });
                }
            }

            LimpiarCarpetaDeArchivos(comando.Dto.Id);
            if (comando.Dto.FilePathPlano != null)
                planoDeCarga.FilePathPlano = GuardarArchivo(comando.Dto.FilePathPlano, comando.Dto.PlanoDeCargaArchivoPlanoNombre, comando.Dto.Id);
            else
                planoDeCarga.FilePathPlano = null;

            if (comando.Dto.FilePathSecuencia != null)
                planoDeCarga.FilePathSecuencia = GuardarArchivo(comando.Dto.FilePathSecuencia, comando.Dto.PlanoDeCargaArchivoSecuenciaNombre, comando.Dto.Id);
            else
                planoDeCarga.FilePathSecuencia = null;
        }

        private void LimpiarCarpetaDeArchivos(int planoDeCargaId)
        {
            var path = ConfigurationManager.AppSettings["ArchivosPath"];
            var directorio = "PlanosDeCarga";
            DirectoryInfo di = new DirectoryInfo(path + (path.EndsWith("\\") ? "" : "\\") + directorio + "\\" + planoDeCargaId);

            if (di.Exists)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            
        }

        public string GuardarArchivo(string archivoBase64, string nombreArchivo, int planoDeCargaId)
        {
            try
            {
                var path = ConfigurationManager.AppSettings["ArchivosPath"];
                var directorio = "PlanosDeCarga";
                var archivoBase64Split = archivoBase64.Contains(',') ?  archivoBase64.Split(',')[1] : archivoBase64;
                var archivo = Convert.FromBase64String(archivoBase64Split);

                var rutaDestino = path + (path.EndsWith("\\") ? "" : "\\") + directorio + "\\" + planoDeCargaId + "\\" + nombreArchivo;

                if (!Directory.Exists(Path.GetDirectoryName(rutaDestino)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(rutaDestino));
                }
                using (var outputStream = File.OpenWrite(rutaDestino))
                {
                    outputStream.Write(archivo, 0, archivo.Length);
                }
                return rutaDestino;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
            }
            return null;
        }

        protected override void Validar(GuardarPlanoDeCarga comando, Resultado resultado)
        {

        }
    }
}
