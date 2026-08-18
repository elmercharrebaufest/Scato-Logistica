using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarLecturaDeTarjeta : ProcesadorComando<ActualizarLecturaDeTarjeta>
    {
        public ProcesadorActualizarLecturaDeTarjeta(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarLecturaDeTarjeta comando)
        {
            var resultado = new ResultadoActualizarLecturaDeTarjeta();
            Validar(comando, resultado);
            if (!resultado.HayErrores)
            {
                ModificarEntidad(comando, resultado);
                Repositorio.GuardarCambios();

                foreach(var puesto in resultado.LecturaPuestosDeTrabajo)
                {
                    var primeraLectura = Repositorio.ObtenerMenor<LecturaDeTarjeta, int>(x => x.PuestoDeTrabajo.Id == puesto.PuestoDeTrabajoId, x => x.Id);
                    puesto.PrimerNumeroDeTarjeta = primeraLectura == null ? "" : primeraLectura.Lectura;
                }
            }

            return resultado;
        }

        private void ModificarEntidad(ActualizarLecturaDeTarjeta comando, ResultadoActualizarLecturaDeTarjeta resultado)
        {
            var puestosDeTrabajo = Repositorio.Listar<PuestoDeTrabajo>(x => x.Lector == comando.Dto.CodigoDispositivo);
            if (!puestosDeTrabajo.Any())
            {
                return;
            }
            foreach(var puestoDeTrabajo in puestosDeTrabajo)
            {
                var lecturaPuestoDeTrabajo = new LecturaPuestoDeTrabajoDto
                {
                    PuestoDeTrabajoId = puestoDeTrabajo.Id,
                    CentroId = puestoDeTrabajo.Centro.Id,
                    NumeroDeTarjeta = comando.Dto.Lectura,
                    PuestoDeTrabajoPidePantente = puestoDeTrabajo.PidePatente,
                    TarjetaValida = true,
                    PuestoDeTrabajoImprimeTarjetaDeAcceso = puestoDeTrabajo.ImprimeTarjetaDeAcceso,
                    Entrada = puestoDeTrabajo.Entradas(),
                    Salida = puestoDeTrabajo.CierresEntrada(),
                    Automatizado = puestoDeTrabajo.AutomatizadoFull && !puestoDeTrabajo.PausaAutoFull,
                    VideoCamaras = !puestoDeTrabajo.PidePatente || puestoDeTrabajo.FotoAlMarcarTarjeta ? Conversor.ConvertirList<VideoCamara, VideoCamaraDto>(puestoDeTrabajo.VideoCamaras.ToList()) : new List<VideoCamaraDto>(),
                    Patente = Repositorio.ObtenerProyeccion<Recorrido, string>(r => r.TarjetaDeAcceso == comando.Dto.Lectura && !r.Terminado, x => x.Patente),
                    CodigoDispositivo = comando.Dto.CodigoDispositivo,
                    Firmware = puestoDeTrabajo.Firmware,
                    TipoIdentificacion = TipoIdentificacionPorPuesto.IngresoPorLectura,
                };

                var codigo = comando.Dto.Lectura.Substring(0, 5);
                var rango = comando.Dto.Lectura.Substring(comando.Dto.Lectura.Length - Math.Min(5, comando.Dto.Lectura.Length));
                if (Repositorio.Existe<TarjetaBloqueada>(x => x.Centro.Id == puestoDeTrabajo.Centro.Id && x.Numero == comando.Dto.Lectura))
                {
                    lecturaPuestoDeTrabajo.MensajeError = string.Format(Textos.Error_TarjetaBloqueada, comando.Dto.Lectura, puestoDeTrabajo.NombrePuesto, comando.Dto.CodigoDispositivo);
                    lecturaPuestoDeTrabajo.TarjetaValida = false;
                }
                else if (!Repositorio.Existe<TarjetaRango>(x => x.Centro.Id == puestoDeTrabajo.Centro.Id && x.ValidoDesde <= DateTime.Today && x.ValidoHasta >= DateTime.Today && x.Codigo == codigo && String.Compare(rango, x.RangoDesde, StringComparison.OrdinalIgnoreCase) >= 0 && String.Compare(rango, x.RangoHasta, StringComparison.OrdinalIgnoreCase) <= 0))
                {
                    lecturaPuestoDeTrabajo.MensajeError = string.Format(Textos.Error_TarjetaFueraDeRango, comando.Dto.Lectura, puestoDeTrabajo.NombrePuesto, comando.Dto.CodigoDispositivo);
                    lecturaPuestoDeTrabajo.TarjetaValida = false;
                }
                else if (Repositorio.Existe<TarjetaSupervisor>(x => x.Centro.Id == puestoDeTrabajo.Centro.Id && x.Numero == comando.Dto.Lectura && x.PuestosDeTrabajoAsociados.Any(y=> y.Id == puestoDeTrabajo.Id)))
                {
                    lecturaPuestoDeTrabajo.EsTarjetaSupervisor = true;
                    lecturaPuestoDeTrabajo.TarjetaValida = false;
                    lecturaPuestoDeTrabajo.DispositivosSupervisor = puestoDeTrabajo.EntradasSupervisor();
                    lecturaPuestoDeTrabajo.MensajeError = string.Format(Textos.Error_TarjetaSupervisor, comando.Dto.Lectura, puestoDeTrabajo.NombrePuesto, comando.Dto.CodigoDispositivo);
                }
                else if (puestoDeTrabajo.ImprimeTarjetaDeAcceso &&
                         !string.IsNullOrEmpty(lecturaPuestoDeTrabajo.Patente))
                {
                    lecturaPuestoDeTrabajo.MensajeError = string.Format(Textos.Error_TarjetaEnUso, comando.Dto.Lectura, puestoDeTrabajo.NombrePuesto, comando.Dto.CodigoDispositivo);
                    lecturaPuestoDeTrabajo.TarjetaValida = false;
                }

                if(lecturaPuestoDeTrabajo.TarjetaValida)
                {
                    if (puestoDeTrabajo.PidePatente && puestoDeTrabajo.EncolaLecturas)
                    {
                        var lecturaAnterior = Repositorio.ObtenerMayor<LecturaDeTarjeta, int>(x => x.PuestoDeTrabajo.Id == puestoDeTrabajo.Id, x => x.Id);
                        if (lecturaAnterior == null || lecturaAnterior.Lectura != lecturaPuestoDeTrabajo.NumeroDeTarjeta)
                        {
                            Repositorio.Agregar(new LecturaDeTarjeta
                            {
                                PuestoDeTrabajo = puestoDeTrabajo,
                                Lectura = lecturaPuestoDeTrabajo.NumeroDeTarjeta,
                                Patente = lecturaPuestoDeTrabajo.Patente
                            });
                        }
                    }
                    else
                    {
                        var lectura = puestoDeTrabajo.Lecturas.LastOrDefault();
                        if (lectura == null)
                        {
                            lectura = new LecturaDeTarjeta
                            {
                                PuestoDeTrabajo = puestoDeTrabajo
                            };
                            Repositorio.Agregar(lectura);
                        }
                        lectura.Lectura = lecturaPuestoDeTrabajo.NumeroDeTarjeta;
                        lectura.Patente = lecturaPuestoDeTrabajo.Patente;
                    }
                }
                resultado.LecturaPuestosDeTrabajo.Add(lecturaPuestoDeTrabajo);
            }
        }

        protected void Validar(ActualizarLecturaDeTarjeta comando, ResultadoActualizarLecturaDeTarjeta resultado)
        {

        }
    }
}