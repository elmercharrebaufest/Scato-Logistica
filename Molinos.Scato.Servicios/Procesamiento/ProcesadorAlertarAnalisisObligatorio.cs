using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorAlertarAnalisisObligatorio : ProcesadorComando<AlertarAnalisisObligatorio>
    {
        public ProcesadorAlertarAnalisisObligatorio(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(AlertarAnalisisObligatorio comando)
        {
            var resultado = new ResultadoAlertarAnalisisObligatorio();
            if (comando.ListaPuestoDeTrabajoId != null)
            {
                var analisisObligatorio = Repositorio.Listar<AnalisisObligatorio>(x => x.Centro.Id == comando.CentroId && x.PuestosDeTrabajoAsociados.Any(y => comando.ListaPuestoDeTrabajoId.Contains(y.Id)));
                foreach(var analisis in analisisObligatorio)
                {
                    var reglaDeAnalisisObligatorio = Repositorio.ObtenerMayor<ReglaDeAnalisisObligatorio, int>(x => x.Material.Id == analisis.Material.Id && x.Centro.Id == comando.CentroId && x.FechaDeVigenciaHasta > DateTime.Now && x.CantidadAnalisis > 0, x => x.Id);
                    if (reglaDeAnalisisObligatorio != null)
                    {
                        var material = reglaDeAnalisisObligatorio.Material.Id;
                        var cosecha = reglaDeAnalisisObligatorio.Cosecha;
                        var centro = reglaDeAnalisisObligatorio.Centro.Id;
                        var provincia = reglaDeAnalisisObligatorio.Provincia != null ? (int?)reglaDeAnalisisObligatorio.Provincia.Id : null;
                        var localidad = reglaDeAnalisisObligatorio.Localidad != null ? (int?)reglaDeAnalisisObligatorio.Localidad.Id : null;
                        if (Repositorio.Existe<Recorrido>(
                            x => x.Material.Id == material &&
                            x.Calado.FechaCreacion == null && 
                            x.Centro.Id == centro &&
                            x.Vehiculo.CartaPorte.Cosecha == cosecha &&
                            ((localidad == null && provincia == x.Vehiculo.CartaPorte.Procedencia.Provincia.Id) || (localidad != null && localidad == x.Vehiculo.CartaPorte.Procedencia.Id))))
                        {
                            resultado.AlertarAnalisisObligatorio = true;
                            resultado.Material.Add(analisis.Material.Descripcion);
                        }
                    }
                    else if (analisis.IntervaloDeAnalisis - DateTime.Now.Subtract(analisis.UltimoAnalisis ?? new DateTime()).TotalMinutes < analisis.AlertarAnalisisIntervalo)
                    {   
                        resultado.AlertarAnalisisObligatorio = true;
                        var difDeTiempo = DateTime.Now.Subtract(analisis.UltimoAnalisis ?? new DateTime());
                        var tiempo = "";
                        if(analisis.IntervaloDeAnalisis - difDeTiempo.TotalMinutes > 0)
                        {
                            if (analisis.IntervaloDeAnalisis - difDeTiempo.TotalMinutes >= 1)
                            {
                                tiempo = "("+((int)(analisis.IntervaloDeAnalisis - difDeTiempo.TotalMinutes)).ToString() + "min)";
                            }
                            else
                            {
                                tiempo = "(" + ((int)((analisis.IntervaloDeAnalisis * 60) - difDeTiempo.TotalSeconds)).ToString() + "seg)";
                            }
                        }
                        resultado.Material.Add($"{analisis.Material.Descripcion} {tiempo}");
                    }
                }
            }
            resultado.Material = resultado.Material.Distinct().ToList();
            return resultado;
        }
    }


}