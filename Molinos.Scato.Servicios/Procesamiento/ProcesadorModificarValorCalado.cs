using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarValorCalado : ProcesadorComando<ModificarValorCalado>
    {
        public ProcesadorModificarValorCalado(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarValorCalado comando)
        {
            var resultado = new Resultado();
            try
            {
                if (comando.Dto.Id != 0)
                {
                    if (!resultado.HayErrores)
                    {
                        CrearEntidadAjusteDeCalidad(comando);
                        ModificarEntidad(comando.Dto);                            
                    }
                }
            }
            catch (Exception e)
            {
                Log.Info(e, "Error en la modificacion del valor de calado");
                resultado.Error("", /*Textos.Calado_ValorNuevoError*/e.Message);
            }
            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }
            return resultado;
        }

        protected void ModificarEntidad(AjusteDeCalidadDto dto)
        {
            if (!dto.EsAnalisis)
            {
                var caladoPorCaracteristica = Repositorio.Obtener<CaladoPorCaracteristica>(dto.Id);
                caladoPorCaracteristica.ValorCalado = dto.ValorNuevo;
            }
            else
            {
                var analisisPorCaracteristica = Repositorio.Obtener<AnalisisPorCaracteristica>(dto.Id);
                analisisPorCaracteristica.ValorAnalisis = dto.ValorNuevo;
            }
        }

        protected void CrearEntidadAjusteDeCalidad(ModificarValorCalado comando)
        {
            var ajuste = new AjusteDeCalidadDto
                {
                    Fecha = DateTime.Now,
                    NumeroDocumentoIngreso = comando.NumeroDoc,
                    TipoDocumentoIngreso = comando.TipoDoc,
                    Usuario = comando.Usuario,
                    ValorOriginal = comando.Dto.EsAnalisis ? Repositorio.Obtener<AnalisisPorCaracteristica>(comando.Dto.Id).ValorAnalisis ?? Repositorio.Obtener<AnalisisPorCaracteristica>(comando.Dto.Id).ValorCalado : Repositorio.Obtener<CaladoPorCaracteristica>(comando.Dto.Id).ValorCalado,
                    ValorNuevo = comando.Dto.ValorNuevo,
                };
            var ajusteDeCalidad = Conversor.Convertir<AjusteDeCalidadDto, AjusteDeCalidad>(ajuste);
            ajusteDeCalidad.CaracteristicaDeCalidad = Repositorio.Obtener<CaracteristicaDeCalidad>(comando.Dto.CaracteristicaId);
            Repositorio.Agregar(ajusteDeCalidad);
        }

        protected void Validar(CaladoPorCaracteristicaDto caladoPorCaracteristicaDto, Resultado resultado)
        {
            /*var caracteristica = Repositorio.Obtener<CaracteristicaDeCalidadDto>(caladoPorCaracteristicaDto.CaracteristicaId);
            if (caladoPorCaracteristicaDto.ValorCalado > caracteristica.CaladoMaximo || caladoPorCaracteristicaDto.ValorCalado < caracteristica.CaladoMinimo)
            {
                resultado.Error("", Textos.Calado_FueraDeRango);
            }*/
        }
    }
}
