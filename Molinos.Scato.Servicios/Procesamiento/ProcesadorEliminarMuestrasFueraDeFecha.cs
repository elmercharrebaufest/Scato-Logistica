using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarMuestrasFueraDeFecha : ProcesadorComando<EliminarMuestrasFueraDeFecha>
    {
        public ProcesadorEliminarMuestrasFueraDeFecha(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public sealed override Resultado Ejecutar(EliminarMuestrasFueraDeFecha comando)
        {
            var resultado = new Resultado();
            DateTime fechaActual = DateTime.Now;
            int year = fechaActual.Year;
            int mes = fechaActual.Month;
            int dia = fechaActual.Day;
            int hora = comando.HrDiaAnterior;

            var fechaAyer = new DateTime(year , mes , dia - 1 , hora , 0 , 0);
            var muestras  = Repositorio.Listar<MuestraEnvioACamara>(c => c.EsPreLote == comando.EsPreLote &&  c.FechaDescarga < fechaAyer && c.Lote == null);
            
            
            Repositorio.RemoverTodos(muestras);
            try
            {
                Repositorio.GuardarCambios();
               
            }
            catch (EntidadReferenciadaException)
            {
                resultado.Error("", Textos.Error_EliminarReferenciado);
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrio al eliminar muestras fuera de fechas con la entidad del tipo {0}", typeof(AjusteDeStock).Name);
                resultado.Error("", Textos.Error_ActualizarGenerico);
            }
            
            return resultado;
        }
    }
}