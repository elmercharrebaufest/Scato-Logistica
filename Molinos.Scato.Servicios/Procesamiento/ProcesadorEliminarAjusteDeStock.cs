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
    public class ProcesadorEliminarAjusteDeStock : ProcesadorComando<EliminarAjusteDeStock>
    {
        public ProcesadorEliminarAjusteDeStock(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public sealed override Resultado Ejecutar(EliminarAjusteDeStock comando)
        {
            var resultado = new Resultado();

            var ajuste  = Repositorio.Obtener<AjusteDeStock>(comando.Id);
            Repositorio.Agregar(new LogAjusteDeStock
            {
                Centro = ajuste.Centro,
                Fecha = DateTime.Now,
                FechaAjuste = ajuste.Fecha,
                Material = ajuste.Material,
                NumeroDocumentoIngreso = ajuste.NumeroDocumentoIngreso,
                Observaciones = ajuste.Observaciones,
                PesoBrutoIngreso = ajuste.PesoBrutoIngreso,
                PesoNetoEgreso = ajuste.PesoNetoEgreso,
                PesoNetoIngreso = ajuste.PesoNetoIngreso,
                Tipo = "Baja",
                TipoComprobanteOncca = ajuste.TipoComprobanteOncca,
                NombreUsuario = comando.NombreUsuario
            });
            Repositorio.Remover(ajuste);
            try
            {
                Repositorio.GuardarCambios();
                var logueaEntidad = comando.GetType().GetCustomAttributes(true).Any(s => s.GetType() == typeof(LoguearEntidad));
                if (logueaEntidad)
                {
                    try
                    {
                        var logAbm = new LogABM
                        {
                            Pantalla = comando.GetType().Name,
                            Usuario = comando.NombreUsuario,
                            Fecha = DateTime.Now,
                            Evento = EventoABM.Baja,
                            Entidad = comando.ToXml()
                        };
                        Repositorio.Agregar(logAbm);
                        Repositorio.GuardarCambios();
                    }
                    catch (Exception e)
                    {
                        Log.Warn(e, "Ocurrio un error al crear el log AMB Eliminar");
                    }
                }
            }
            catch (EntidadReferenciadaException)
            {
                resultado.Error("", Textos.Error_EliminarReferenciado);
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrio al eliminar la entidad del tipo {0} - Id {1}", typeof(AjusteDeStock).Name, comando.Id);
                resultado.Error("", Textos.Error_ActualizarGenerico);
            }
            
            return resultado;
        }
    }
}