using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearEnvioACamara : ProcesadorCrear<CrearEnvioACamara, MuestraEnvioACamara>
    {
        public ProcesadorCrearEnvioACamara(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        private void SumarMuestraAutomatica(int caladoId, List<int> ids)
        {
            var muestras = Repositorio.Listar<MuestraEnvioACamara>(x => x.Calado.Id == caladoId);
            if(muestras != null && muestras.Any())
            {
                foreach(var muestra in muestras)
                {
                    foreach (var i in muestra.CaracteristicasDeCalidad)
                    {
                        ids.Add(i.Id);
                    }
                }
            }
        }

        protected override void Finally(CrearEnvioACamara comando, int id)
        {
            var muestras = Repositorio.Listar<MuestraEnvioACamara>(x => x.Calado.Id == comando.Dto.CaladoId && x.Id != id);

            if (muestras != null && muestras.Any())
            {
                foreach(var muestra in muestras)
                {
                    Repositorio.Remover(muestra);
                }
                Repositorio.GuardarCambios();
            }
        }

        protected override MuestraEnvioACamara CrearEntidad(CrearEnvioACamara comando)
        {
            List<int> ids = new List<int>();
            if (comando.Dto.CaracteristicasDeCalidad != null)
            {
                ids = comando.Dto.CaracteristicasDeCalidad.Where(y => y.SeEnviaACamara).Select(x => x.Id).ToList();
            }
            SumarMuestraAutomatica(comando.Dto.CaladoId, ids);
            Expression<Func<CaracteristicaDeCalidad, bool>> expresionFiltro = x => ids.Contains(x.Id);
            var caracteristicasDeCalidad = Repositorio.Listar(expresionFiltro);
            //Reseteo descuento a cero de las características para análizar en cámara
            foreach (var caracteristicaDeCalidad in caracteristicasDeCalidad)
            {
                var caracteristica = caracteristicaDeCalidad;
                var analisis = Repositorio.Obtener<AnalisisPorCaracteristica>(f => f.CaracteristicaDeCalidad.Id == caracteristica.Id && f.AnalisisDeCalidad.Calado.Id == comando.Dto.CaladoId);
                if (analisis != null)
                {
                    analisis.DescuentoEnKg = 0;
                    analisis.DescuentoEnPorcentaje = 0;
                }
                var calado = Repositorio.Obtener<CaladoPorCaracteristica>(f => f.CaracteristicaDeCalidad.Id == caracteristica.Id && f.Calado.Id == comando.Dto.CaladoId);
                if (calado != null)
                {
                    calado.DescuentoEnKg = 0;
                    calado.DescuentoEnPorcentaje = 0;
                }
            }

            var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.Dto.WorkflowInstanceId);

            var formatoDeArchivo = Repositorio.ObtenerProyeccion<Camara, CamaraFormatoDeArchivo?>(x => x.Id == comando.Dto.CamaraId, x => x.FormatoDeArchivo);
            var convCentro = Repositorio.Obtener<ConversionCentro>(x => x.Camara.Id == comando.Dto.CamaraId && x.Centro.Id == comando.Dto.CentroId);
            var codigoDeCamara = convCentro != null ? convCentro.CodigoCamara : "";
            var nroDocumento = recorrido.NumeroDocumentoIngreso.Replace("-", "").Substring(recorrido.NumeroDocumentoIngreso.Length - 10);
            switch (formatoDeArchivo)
            {
                case CamaraFormatoDeArchivo.BahiaBlanca:
                    comando.Dto.NroMuestra = nroDocumento;
                    break;
                case CamaraFormatoDeArchivo.Rosario:
                    comando.Dto.NroMuestra = codigoDeCamara.Substring(0, codigoDeCamara.Length > 3 ? 3 : codigoDeCamara.Length) + (recorrido.Vehiculo == null ? 1.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') : recorrido.Vehiculo.NumeroVehiculo.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'))
                        + nroDocumento;
                    break;
                default: //BSAS
                    comando.Dto.NroMuestra = codigoDeCamara.Substring(0, codigoDeCamara.Length > 2 ? 2 : codigoDeCamara.Length) + (recorrido.Vehiculo == null ? 1.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') : recorrido.Vehiculo.NumeroVehiculo.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'))
                        + nroDocumento;
                    break;
            }


            return new MuestraEnvioACamara
                {
                    Calado = Repositorio.Obtener<Calado>(comando.Dto.CaladoId),
                    NombreUsuario = comando.Dto.NombreUsuario,
                    CaracteristicasDeCalidad = caracteristicasDeCalidad,
                    Camara = Repositorio.Obtener<Camara>(comando.Dto.CamaraId),
                    CartaPorte = recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && recorrido.Vehiculo != null ? recorrido.Vehiculo.CartaPorte : null,
                    FechaDescarga = comando.Dto.FechaDescarga,
                    NroMuestra = comando.Dto.NroMuestra,
                    Patente = recorrido.Patente,
                    TipoDocumento = recorrido.TipoDocumentoIngreso,
                    NroDocumento = recorrido.NumeroDocumentoIngreso,
                    Centro = Repositorio.Obtener<Centro>(recorrido.Centro.Id),
                    HuboExcepcion = comando.Dto.HuboExcepcion,
                    EsPreLote = comando.Dto.EsPreLote
                };
        }
        protected override void Validar(CrearEnvioACamara comando, Resultado resultado)
        {
        }
    }
}
