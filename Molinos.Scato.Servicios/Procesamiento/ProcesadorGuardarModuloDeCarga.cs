using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorGuardarModuloDeCarga : ProcesadorModificar<GuardarModuloDeCarga>
    {
        public ProcesadorGuardarModuloDeCarga(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(GuardarModuloDeCarga comando)
        {
            var moduloDeCarga = Repositorio.Obtener<ModuloDeCarga>(comando.Dto.Id);

            ///////////////////////////
            ///// PROCESO PARA EL HISTORICO /////
            if (moduloDeCarga.FechaDeCreacion == null)
                moduloDeCarga.FechaDeCreacion = DateTime.Now;
            else
            {
                ///// OPERACIONES/TABLERISTAS /////
                moduloDeCarga.FechaDeModificacion = DateTime.Now;

                var modulodecargahistorico = Repositorio.Agregar(new ModuloDeCargaHistorico
                {
                    ModuloDeCarga = moduloDeCarga,
                    Cargado = moduloDeCarga.Cargado,
                    FechaDeCreacion = moduloDeCarga.FechaDeCreacion.Value,
                    FechaDeModificacion = moduloDeCarga.FechaDeModificacion.Value,
                    Usuario = moduloDeCarga.Usuario,
                    FechaDeFinalizacion = moduloDeCarga.FechaDeFinalizacion,
                    UsuarioFinalizacion = moduloDeCarga.UsuarioFinalizacion,
                    Enviado = moduloDeCarga.Enviado
                });

                Repositorio.GuardarCambios();
                ///// OPERACIONES/TABLERISTAS /////

                ///// OPERACIONES /////
                /////////////////////////
                // LÍQUIDOS
                if (moduloDeCarga.ModuloDeCargaHabilitacionDeTanques != null)
                {
                    foreach (var tanques in moduloDeCarga.ModuloDeCargaHabilitacionDeTanques)
                    {
                        Repositorio.Agregar(new ModuloDeCargaHabilitacionDeTanquesHistorico
                        {
                            ModuloDeCargaHistorico = modulodecargahistorico,
                            Tanque1 = tanques.Tanque1,
                            Tanque2 = tanques.Tanque2,
                            Tanque7 = tanques.Tanque7,
                            Tanque8 = tanques.Tanque8,
                            Tanque9 = tanques.Tanque9,
                            Tanque20 = tanques.Tanque20,
                            Tanque30 = tanques.Tanque30,
                            Tanque31 = tanques.Tanque31,
                            Tanque32 = tanques.Tanque32,
                            Tanque33 = tanques.Tanque33,
                            Tanque34 = tanques.Tanque34,
                            Tanque35 = tanques.Tanque35,
                            Tanque36 = tanques.Tanque36,
                            Tanque37 = tanques.Tanque37,
                            Tanque38 = tanques.Tanque38,
                            Tanque40 = tanques.Tanque40
                        });
                    }
                }

                if (moduloDeCarga.ModuloDeCargaLineasDeEmbarque != null)
                {
                    foreach (var lineas in moduloDeCarga.ModuloDeCargaLineasDeEmbarque)
                    {
                        Repositorio.Agregar(new ModuloDeCargaLineasDeEmbarqueHistorico
                        {
                            ModuloDeCargaHistorico = modulodecargahistorico,
                            Linea = lineas.Linea,
                            MaterialPuerto = lineas.MaterialPuerto,
                            TkInicial = lineas.TkInicial,
                            TemperaturaInicial = lineas.TemperaturaInicial,
                            AlturaInicialCM = lineas.AlturaInicialCM,
                            AlturaInicialMM = lineas.AlturaInicialMM,
                            DensidadInicial = lineas.DensidadInicial,
                            TemperaturaFinal = lineas.TemperaturaFinal,
                            Litros = lineas.Litros,
                            DensidadFinal = lineas.DensidadFinal,
                            AlturaFinalCM = lineas.AlturaFinalCM,
                            AlturaFinalMM = lineas.AlturaFinalMM,
                            Kilos = lineas.Kilos,
                            TkFinal = lineas.TkFinal
                        });
                    }
                }
                // LÍQUIDOS
                /////////////////////////


                /////////////////////////
                // SÓLIDOS
                if (moduloDeCarga.ModuloDeCargaElementoGrafico != null)
                {
                    foreach (var elementos in moduloDeCarga.ModuloDeCargaElementoGrafico)
                    {
                        Repositorio.Agregar(new ModuloDeCargaElementoGraficoHistorico
                        {
                            ModuloDeCargaHistorico = modulodecargahistorico,
                            CeldaManoDeEmbarque = elementos.CeldaManoDeEmbarque,
                            Tipo = elementos.Tipo,
                            X = elementos.X,
                            Y = elementos.Y,
                            Forma = elementos.Forma,
                            Width = elementos.Width,
                            Height = elementos.Height,
                            RadioX = elementos.RadioX,
                            RadioY = elementos.RadioY,
                            MaterialPuerto = elementos.MaterialPuerto,
                            Rotacion = elementos.Rotacion
                        });
                    }
                }

                if (moduloDeCarga.ModuloDeCargaManosDeEmbarque != null)
                {
                    foreach (var manos in moduloDeCarga.ModuloDeCargaManosDeEmbarque)
                    {
                        Repositorio.Agregar(new ModuloDeCargaManosDeEmbarqueHistorico
                        {
                            ModuloDeCargaHistorico = modulodecargahistorico,
                            Mano = manos.Mano,
                            Observaciones = manos.Observaciones
                        });
                    }
                }

                //FALTA HISTORICO DE DETALLES

                if (moduloDeCarga.ModuloDeCargaTabiquesDeEmbarque != null)
                {
                    foreach (var tabiques in moduloDeCarga.ModuloDeCargaTabiquesDeEmbarque)
                    {
                        Repositorio.Agregar(new ModuloDeCargaTabiquesDeEmbarqueHistorico
                        {
                            ModuloDeCargaHistorico = modulodecargahistorico,
                            Tabique = tabiques.Tabique,
                            EntreColumna = tabiques.EntreColumna,
                            YColumna = tabiques.YColumna
                        });
                    }
                }
                // SÓLIDOS
                /////////////////////////

                Repositorio.GuardarCambios();
                ///// OPERACIONES /////
            }
            ///// PROCESO PARA EL HISTORICO /////
            ///////////////////////////


            ///////////////////////////
            ///// PROCESO TABLAS ACTUALES /////
            ///// OPERACIONES/TABLERISTAS /////
            moduloDeCarga.Usuario = comando.Dto.Usuario;
            moduloDeCarga.Cargado = true;
            moduloDeCarga.Enviado = comando.Dto.Enviado;

            if (comando.Dto.UsuarioFinalizacion != null)
            {
                moduloDeCarga.FechaDeFinalizacion = DateTime.Now;
                moduloDeCarga.UsuarioFinalizacion = comando.Dto.UsuarioFinalizacion;
            }
            ///// OPERACIONES/TABLERISTAS /////

            ///// OPERACIONES /////
            /////////////////////////
            // LÍQUIDOS
            Repositorio.RemoverTodos(moduloDeCarga.ModuloDeCargaHabilitacionDeTanques.ToList());
            if (comando.Dto.ModuloDeCargaHabilitacionDeTanques != null)
            {
                foreach (var tanques in comando.Dto.ModuloDeCargaHabilitacionDeTanques)
                {
                    moduloDeCarga.ModuloDeCargaHabilitacionDeTanques.Add(new ModuloDeCargaHabilitacionDeTanques
                    {
                        ModuloDeCarga = moduloDeCarga,
                        Tanque1 = tanques.Tanque1,
                        Tanque2 = tanques.Tanque2,
                        Tanque7 = tanques.Tanque7,
                        Tanque8 = tanques.Tanque8,
                        Tanque9 = tanques.Tanque9,
                        Tanque20 = tanques.Tanque20,
                        Tanque30 = tanques.Tanque30,
                        Tanque31 = tanques.Tanque31,
                        Tanque32 = tanques.Tanque32,
                        Tanque33 = tanques.Tanque33,
                        Tanque34 = tanques.Tanque34,
                        Tanque35 = tanques.Tanque35,
                        Tanque36 = tanques.Tanque36,
                        Tanque37 = tanques.Tanque37,
                        Tanque38 = tanques.Tanque38,
                        Tanque40 = tanques.Tanque40
                    });
                }
            }
            //OPERACIONES Y TABLERISTAS
            Repositorio.RemoverTodos(moduloDeCarga.ModuloDeCargaLineasDeEmbarque.ToList());
            if (comando.Dto.ModuloDeCargaLineasDeEmbarque != null)
            {
                foreach (var lineas in comando.Dto.ModuloDeCargaLineasDeEmbarque)
                {
                    var materialPuerto = lineas.MaterialPuerto != null ? Repositorio.Obtener<MaterialPuerto>(lineas.MaterialPuerto.Id) : null;
                    moduloDeCarga.ModuloDeCargaLineasDeEmbarque.Add(new ModuloDeCargaLineasDeEmbarque
                    {
                        ModuloDeCarga = moduloDeCarga,
                        Linea = lineas.Linea,
                        MaterialPuerto = materialPuerto,
                        TkInicial = lineas.TkInicial,
                        TemperaturaInicial = lineas.TemperaturaInicial,
                        AlturaInicialCM = lineas.AlturaInicialCM,
                        AlturaInicialMM = lineas.AlturaInicialMM,
                        DensidadInicial = lineas.DensidadInicial,
                        TemperaturaFinal = lineas.TemperaturaFinal,
                        Litros = lineas.Litros,
                        DensidadFinal = lineas.DensidadFinal,
                        AlturaFinalCM = lineas.AlturaFinalCM,
                        AlturaFinalMM = lineas.AlturaFinalMM,
                        Kilos = lineas.Kilos,
                        TkFinal = lineas.TkFinal
                    });
                }
            }
            //OPERACIONES Y TABLERISTAS
            // LÍQUIDOS
            /////////////////////////


            /////////////////////////
            //MODULO DE CARGA SÓLIDOS
            Repositorio.RemoverTodos(moduloDeCarga.ModuloDeCargaElementoGrafico.ToList());
            if (comando.Dto.ModuloDeCargaElementoGrafico != null)
            {
                foreach (var elementos in comando.Dto.ModuloDeCargaElementoGrafico)
                {
                    var celdasManoDeEmbarque = elementos.CeldaManoDeEmbarque != null ? Repositorio.Obtener<CeldaManoDeEmbarque>(elementos.CeldaManoDeEmbarque.Id) : null;
                    var materialPuerto = elementos.MaterialPuerto != null ? Repositorio.Obtener<MaterialPuerto>(elementos.MaterialPuerto.Id) : null;
                    moduloDeCarga.ModuloDeCargaElementoGrafico.Add(new ModuloDeCargaElementoGrafico
                    {
                        ModuloDeCarga = moduloDeCarga,
                        CeldaManoDeEmbarque = celdasManoDeEmbarque,
                        Tipo = elementos.Tipo,
                        X = elementos.X,
                        Y = elementos.Y,
                        Forma = elementos.Forma,
                        Width = elementos.Width,
                        Height = elementos.Height,
                        RadioX = elementos.RadioX,
                        RadioY = elementos.RadioY,
                        MaterialPuerto = materialPuerto,
                        Rotacion = elementos.Rotacion
                    });
                }
            }

            Repositorio.RemoverTodos(moduloDeCarga.ModuloDeCargaManosDeEmbarque.ToList());
            if (comando.Dto.ModuloDeCargaManosDeEmbarque != null)
            {
                foreach (var manos in comando.Dto.ModuloDeCargaManosDeEmbarque)
                {
                    var moduloDeCargaManosDeEmbarque = new ModuloDeCargaManosDeEmbarque
                    {
                        ModuloDeCarga = moduloDeCarga,
                        Mano = manos.Mano,
                        Observaciones = manos.Observaciones
                    };

                    moduloDeCargaManosDeEmbarque.ModuloDeCargaManosDeEmbarqueDetalle = new List<ModuloDeCargaManosDeEmbarqueDetalle>();
                    manos.ModuloDeCargaManosDeEmbarqueDetalle.ToList()
                        .ForEach(detalle => moduloDeCargaManosDeEmbarque.ModuloDeCargaManosDeEmbarqueDetalle.Add(new ModuloDeCargaManosDeEmbarqueDetalle
                        {
                            CeldaManoDeEmbarque = detalle.CeldaManoDeEmbarque != null ? Repositorio.Obtener<CeldaManoDeEmbarque>(detalle.CeldaManoDeEmbarque.Id) : null,
                            SentidoManoDeEmbarque = detalle.SentidoManoDeEmbarque != null ? Repositorio.Obtener<SentidoManoDeEmbarque>(detalle.SentidoManoDeEmbarque.Id) : null,
                            PorcentajePorMano = detalle.PorcentajePorMano,
                            AperturaPorton = detalle.AperturaPorton,
                            MasProduccion = detalle.MasProduccion,
                            MaterialPuerto = detalle.MaterialPuerto != null ? Repositorio.Obtener<MaterialPuerto>(detalle.MaterialPuerto.Id) : null
                        }));

                    moduloDeCarga.ModuloDeCargaManosDeEmbarque.Add(new ModuloDeCargaManosDeEmbarque
                    {
                        ModuloDeCarga = moduloDeCarga,
                        Mano = moduloDeCargaManosDeEmbarque.Mano,
                        Observaciones = moduloDeCargaManosDeEmbarque.Observaciones,
                        ModuloDeCargaManosDeEmbarqueDetalle = moduloDeCargaManosDeEmbarque.ModuloDeCargaManosDeEmbarqueDetalle
                    });
                }
            }

            Repositorio.RemoverTodos(moduloDeCarga.ModuloDeCargaTabiquesDeEmbarque.ToList());
            if (comando.Dto.ModuloDeCargaTabiquesDeEmbarque != null)
            {
                foreach (var tabiques in comando.Dto.ModuloDeCargaTabiquesDeEmbarque)
                {
                    moduloDeCarga.ModuloDeCargaTabiquesDeEmbarque.Add(new ModuloDeCargaTabiquesDeEmbarque
                    {
                        ModuloDeCarga = moduloDeCarga,
                        Tabique = tabiques.Tabique,
                        EntreColumna = tabiques.EntreColumna,
                        YColumna = tabiques.YColumna
                    });
                }
            }
            // SÓLIDOS
            /////////////////////////
            ///// OPERACIONES /////



            ///// TABLERISTAS /////
            moduloDeCarga.IniciarCarga = comando.Dto.IniciarCarga;
            /////////////////////////
            // LÍQUIDOS
            Repositorio.RemoverTodos(moduloDeCarga.ModuloDeCargaMangueraCarga.ToList()); 
            if (comando.Dto.ModuloDeCargaMangueraCarga != null)
            {
                foreach (var magueras in comando.Dto.ModuloDeCargaMangueraCarga)
                {
                    moduloDeCarga.ModuloDeCargaMangueraCarga.Add(new ModuloDeCargaMangueraCarga
                    {
                        ModuloDeCarga = moduloDeCarga,
                        FechaConexionMangueras = magueras.FechaConexionMangueras,
                        HoraConexionMangueras = magueras.HoraConexionMangueras,
                        FechaDesconexionMangueras = magueras.FechaDesconexionMangueras,
                        HoraDesconexionMangueras = magueras.HoraDesconexionMangueras,
                        FechaComienzoCarga = magueras.FechaComienzoCarga,
                        HoraComienzoCarga = magueras.HoraComienzoCarga,
                        FechaFinalizacionCarga = magueras.FechaFinalizacionCarga,
                        HoraFinalizacionCarga = magueras.HoraFinalizacionCarga
                    });
                }
            }

            Repositorio.RemoverTodos(moduloDeCarga.ModuloDeCargaPlanillaDeEmbarque.ToList());
            if (comando.Dto.ModuloDeCargaPlanillaDeEmbarque != null)
            {
                foreach (var planilla in comando.Dto.ModuloDeCargaPlanillaDeEmbarque)
                {
                    var exportador = planilla.Exportador != null ? Repositorio.Obtener<Exportador>(planilla.Exportador.Id) : null;
                    var destino = planilla.Destino != null ? Repositorio.Obtener<Destino>(planilla.Destino.Id) : null;
                    var materialPuerto = planilla.MaterialPuerto != null ? Repositorio.Obtener<MaterialPuerto>(planilla.MaterialPuerto.Id) : null;

                    moduloDeCarga.ModuloDeCargaPlanillaDeEmbarque.Add(new ModuloDeCargaPlanillaDeEmbarque
                    {
                        ModuloDeCarga = moduloDeCarga,
                        Exportador = exportador,
                        BodegaParcel = planilla.BodegaParcel,
                        TanqueDeAbordo = planilla.TanqueDeAbordo,
                        Destino = destino,
                        Tk = planilla.Tk,
                        Cantidad = planilla.Cantidad,
                        MaterialPuerto = materialPuerto,
                        FechaComienzoCarga = planilla.FechaComienzoCarga,
                        HoraComienzoCarga = planilla.HoraComienzoCarga,
                        FechaFinalizacionCarga = planilla.FechaFinalizacionCarga,
                        HoraFinalizacionCarga = planilla.HoraFinalizacionCarga
                    });
                }
            }

            Repositorio.RemoverTodos(moduloDeCarga.ModuloDeCargaPlanillaDeTurnos.ToList());
            if (comando.Dto.ModuloDeCargaPlanillaDeTurnos != null)
            {
                foreach (var planilla in comando.Dto.ModuloDeCargaPlanillaDeTurnos)
                {
                    var moduloDeCargaPlanillaDeTurnos = new ModuloDeCargaPlanillaDeTurnos
                    {
                        ModuloDeCarga = moduloDeCarga,
                        Fecha = planilla.Fecha
                    };

                    moduloDeCargaPlanillaDeTurnos.ModuloDeCargaPlanillaDeTurnosTurnos = new List<ModuloDeCargaPlanillaDeTurnosTurnos>();
                    planilla.ModuloDeCargaPlanillaDeTurnosTurnos.ToList()
                    .ForEach(turnos => moduloDeCargaPlanillaDeTurnos.ModuloDeCargaPlanillaDeTurnosTurnos.Add(new ModuloDeCargaPlanillaDeTurnosTurnos
                    {
                        TurnoPuerto = turnos.TurnoPuerto != null ? Repositorio.Obtener<TurnoPuerto>(turnos.TurnoPuerto.Id) : null,
                        Cerrado = turnos.Cerrado,
                        Enviado = turnos.Enviado
                        //public virtual ICollection<ModuloDeCargaPlanillaDeTurnosTurnosDetalles> ModuloDeCargaPlanillaDeTurnosTurnosDetalles { get; set; }
                        //public virtual ICollection<ModuloDeCargaPlanillaDeTurnosTurnosCortes> ModuloDeCargaPlanillaDeTurnosTurnosCortes { get; set; }
                    }));

                    moduloDeCarga.ModuloDeCargaPlanillaDeTurnos.Add(new ModuloDeCargaPlanillaDeTurnos
                    {
                        ModuloDeCarga = moduloDeCarga,
                        Fecha = planilla.Fecha,
                        ModuloDeCargaPlanillaDeTurnosTurnos = moduloDeCargaPlanillaDeTurnos.ModuloDeCargaPlanillaDeTurnosTurnos
                    });
                }
            }
            // LÍQUIDOS
            /////////////////////////


            /////////////////////////
            // LÍQUIDOS / SÓLIDOS
            Repositorio.RemoverTodos(moduloDeCarga.ModuloDeCargaPeriodoDeCarga.ToList());
            if (comando.Dto.ModuloDeCargaPeriodoDeCarga != null)
            {
                foreach (var periodo in comando.Dto.ModuloDeCargaPeriodoDeCarga)
                {
                    moduloDeCarga.ModuloDeCargaPeriodoDeCarga.Add(new ModuloDeCargaPeriodoDeCarga
                    {
                        ModuloDeCarga = moduloDeCarga,
                        FechaAmarro = periodo.FechaAmarro,
                        HoraAmarro = periodo.HoraAmarro,
                        VientoAmarro = periodo.VientoAmarro,
                        DireccionAmarro = periodo.DireccionAmarro,
                        FechaDesamarro = periodo.FechaDesamarro,
                        HoraDesamarro = periodo.HoraDesamarro,
                        VientoDesamarro = periodo.VientoDesamarro,
                        DireccionDesamarro = periodo.DireccionDesamarro,
                        FechaHabilitacion = periodo.FechaHabilitacion,
                        HoraHabilitacion = periodo.HoraHabilitacion
                    });
                }
            }
            // LÍQUIDOS / SÓLIDOS
            /////////////////////////


            /////////////////////////
            // SÓLIDOS
            Repositorio.RemoverTodos(moduloDeCarga.ModuloDeCargaUmap.ToList());
            if (comando.Dto.ModuloDeCargaUmap != null)
            {
                foreach (var umap in comando.Dto.ModuloDeCargaUmap)
                {
                    moduloDeCarga.ModuloDeCargaUmap.Add(new ModuloDeCargaUmap
                    {
                        ModuloDeCarga = moduloDeCarga,
                        FechaEncendido = umap.FechaEncendido,
                        HoraEncendido = umap.HoraEncendido,
                        FechaApagado = umap.FechaApagado,
                        HoraApagado = umap.HoraApagado
                    });
                }
            }

            Repositorio.RemoverTodos(moduloDeCarga.ModuloDeCargaBalanzas.ToList());
            if (comando.Dto.ModuloDeCargaBalanzas != null)
            {
                foreach (var balanzas in comando.Dto.ModuloDeCargaBalanzas)
                {
                    var motivosFallasBalanza = balanzas.MotivosFallasBalanza != null ? Repositorio.Obtener<MotivosFallasBalanza>(balanzas.MotivosFallasBalanza.Id) : null;
                    
                    moduloDeCarga.ModuloDeCargaBalanzas.Add(new ModuloDeCargaBalanzas
                    {
                        
                       // ModuloDeCarga = moduloDeCarga,
                        MotivosFallasBalanza = motivosFallasBalanza,
                        Observaciones = balanzas.Observaciones,
                    
                    });
                }
            }
            // SÓLIDOS
            /////////////////////////
            ///// TABLERISTAS /////

            ///// PROCESO TABLAS ACTUALES /////
            ///////////////////////////
        }

        protected override void Validar(GuardarModuloDeCarga comando, Resultado resultado)
        {

        }
    }
}