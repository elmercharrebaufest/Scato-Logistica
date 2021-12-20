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
    public class ProcesadorCrearRegistroStockEpa : ProcesadorComando<CrearRegistroStockEpa>
    {
        public ProcesadorCrearRegistroStockEpa(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearRegistroStockEpa comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                var codigoEstablecimientoYCosecha = Repositorio.ObtenerProyeccion<Recorrido, EstablecimientoYCosechaDto>(x => x.InstanciaWorkflow == comando.Dto.InstanceId, x => new EstablecimientoYCosechaDto() {CodigoEstablecimiento = x.Establecimiento.CodigoDeEstablecimiento, Cosecha = x.Vehiculo.CartaPorte.Cosecha});
                if (codigoEstablecimientoYCosecha.CodigoEstablecimiento == null || codigoEstablecimientoYCosecha.Cosecha == null)
                {
                    resultado.Errores.Add("Error", "Falta Establecimiento o Cosecha");
                    Log.Debug("Falta Establecimiento o Cosecha , id = {0}", comando.Dto.InstanceId);
                    return resultado;
                }
                comando.Dto.CodigoEstablecimiento = codigoEstablecimientoYCosecha.CodigoEstablecimiento;
                comando.Dto.Cosecha = codigoEstablecimientoYCosecha.Cosecha;
                
                var registroStockEpa = Conversor.Convertir<RegistroStockEPADto, RegistroStockEPA>(comando.Dto);
                var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.Dto.InstanceId);
                registroStockEpa.Recorrido = recorrido;
                if (!resultado.HayErrores)
                {
                    Repositorio.Agregar(registroStockEpa);
                    Repositorio.GuardarCambios();
                    Log.Debug("Se creó el registro stock EPA para recorrido {0}", comando.Dto.InstanceId);
                }
            }
            catch (Exception e)
            {
                resultado.Error("", Textos.Error_ActualizarGenerico);
                Log.Error("Error al crear Registro stock EPA - {0} - {1}", comando.Dto.InstanceId, e.StackTrace);
            }
            return resultado;
        }
    }
}
