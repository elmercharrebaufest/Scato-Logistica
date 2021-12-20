using System;
using System.Collections.ObjectModel;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCorrespondeAnalisisObligatorio : ProcesadorComando<CorrespondeAnalisisObligatorio>
    {
        public ProcesadorCorrespondeAnalisisObligatorio(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CorrespondeAnalisisObligatorio comando)
        {
            var resultado = new ResultadoCorrespondeAnalisisObligatorio();
            var cartaPorte = Repositorio.ObtenerProyeccion<Recorrido, CartaPorte>(x => x.Id == comando.RecorridoId && x.Vehiculo != null, x => x.Vehiculo.CartaPorte);
            if (cartaPorte != null)
            {
                var analisisObligatorio = Repositorio.Obtener<AnalisisObligatorio>(x => x.Material.Id == comando.MaterialId && x.Centro.Id == comando.CentroId && x.PuestosDeTrabajoAsociados.Any(y => y.Id == comando.PuestoDeTrabajoId));
                if (Repositorio.Existe<ReglaDeAnalisisObligatorio>(x => x.Centro.Id == comando.CentroId && x.Material.Id == comando.MaterialId && x.FechaDeVigenciaHasta > DateTime.Now && x.CantidadAnalisis > 0))
                {
                    var reglaDeAnalisisObligatorio = Repositorio.ObtenerMayor<ReglaDeAnalisisObligatorio, int>(x => x.Material.Id == comando.MaterialId && x.Centro.Id == comando.CentroId && x.FechaDeVigenciaHasta > DateTime.Now && x.CantidadAnalisis > 0 && x.Cosecha == cartaPorte.Cosecha && ((x.Localidad == null && x.Provincia.Id == cartaPorte.Procedencia.Provincia.Id) || (x.Localidad.Id == cartaPorte.Procedencia.Id)), x => x.Id);
                    if (reglaDeAnalisisObligatorio != null && analisisObligatorio != null)
                    {
                        reglaDeAnalisisObligatorio.CantidadAnalisis += comando.Cancelar ? 1 : -1;
                        resultado.CorrespondeAnalisisObligatorio = true;
                        Repositorio.GuardarCambios();
                        return resultado;
                    }
                }
                else
                {
                    resultado.CorrespondeAnalisisObligatorio = analisisObligatorio != null && DateTime.Now.Subtract(analisisObligatorio.UltimoAnalisis ?? new DateTime()).TotalMinutes > analisisObligatorio.IntervaloDeAnalisis;
                    if (resultado.CorrespondeAnalisisObligatorio)
                    {
                        analisisObligatorio.UltimoAnalisis = comando.Cancelar && analisisObligatorio.UltimoAnalisis.HasValue ? analisisObligatorio.UltimoAnalisis.Value.AddMinutes(analisisObligatorio.IntervaloDeAnalisis * -1) : DateTime.Now;
                        Repositorio.GuardarCambios();
                    }
                }
            }
            return resultado;
        }
    }


}