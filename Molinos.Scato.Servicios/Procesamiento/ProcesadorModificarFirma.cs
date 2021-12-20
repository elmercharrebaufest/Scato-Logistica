using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarFirma : ProcesadorComando<ModificarFirma>
    {
        private readonly IFirmaProvider firmaProvider;

        public ProcesadorModificarFirma(IRepositorio repositorio, IFirmaProvider firmaProvider, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
            this.firmaProvider = firmaProvider;
        }

        public override Resultado Ejecutar(ModificarFirma comando)
        {
            var resultado = new ResultadoCrear();
                try
                {
                    var firma = Repositorio.Obtener<Firma>(comando.Dto.Id);
                    if (firma == null)
                    {
                        if (comando.Dto.Logo == null || comando.Dto.Favicon == null)
                        {
                            throw new ArgumentException();
                        }
                        Log.Info("Creando Firma Nueva");
                        var firmaNueva = Conversor.Convertir<FirmaDto, Firma>(comando.Dto);
                        Repositorio.Agregar(firmaNueva);
                        firma = firmaNueva;
                    }
                    else
                    {
                        Log.Info("Modificando Firma");
                        Conversor.Convertir(comando.Dto, firma);
                    }
                    Log.Info("Se van a guardar los cambios en Firma");
                    Repositorio.GuardarCambios();
                    resultado.Id = firma.Id;
                    Log.Info("Cambios guardados");
                    firmaProvider.RefrescarFirma();
                }
                catch (ArgumentException)
                {
                    if (comando.Dto.Logo == null)
                    {
                        resultado.Error("LogoFile", Textos.Logo_Requerido);
                    }
                    if (comando.Dto.Favicon == null)
                    {
                        resultado.Error("Favicon", Textos.Icono_Requerido);
                    }
                }
                catch (Exception e)
                {
                    resultado.Error("", Textos.Error_ActualizarGenerico);
                    Log.Error(e, Textos.Error_ActualizarGenerico);
                }                
    return resultado;
        }      
    }
}
