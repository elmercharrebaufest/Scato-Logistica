using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearExcepcionPagoTasaMunicipal : ProcesadorComando<CrearExcepcionPagoTasaMunicipal>
    {
        public ProcesadorCrearExcepcionPagoTasaMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearExcepcionPagoTasaMunicipal comando)
        {
            var resultado = new ResultadoCrear();

            try
            {
                var esValido = Validar(comando, resultado);
                if (esValido)
                {
                    Repositorio.Agregar(new ExceptuadosTicketMunicipal
                    {
                        Patente = comando.ExceptuadosTicketMunicipalDto.Patente,
                        NumeroDocumentoIngreso = comando.ExceptuadosTicketMunicipalDto.NumeroDocumentoIngreso,
                        FechaCreacionExcepcion = DateTime.Now,
                        WorkflowCodigo = comando.ExceptuadosTicketMunicipalDto.WorkflowCodigo,
                        WorkflowDescripcion = comando.ExceptuadosTicketMunicipalDto.WorkflowDescripcion,
                        Activo = true,
                        PermiteAcciones = true
                    });
                    Repositorio.GuardarCambios();
                }

            }
            catch (Exception e)
            {
                Log.Error(e, "Error al agregar excepcion de pago municipal");
                resultado.Error("", e.Message);
            }
            return resultado;
        }

        private bool Validar(CrearExcepcionPagoTasaMunicipal comando, ResultadoCrear resultado)
        {
            bool esValido = true;
            long numero = 0;
            if (long.TryParse(comando.ExceptuadosTicketMunicipalDto.NumeroDocumentoIngreso, out numero))
            {
                if (!Repositorio.Existe<CartaPorteElectronica>(x => x.NroCTG == numero && x.Dominio == comando.ExceptuadosTicketMunicipalDto.Patente))
                {
                    esValido = false;
                    resultado.Error("NumeroDocumentoIngresoActual", "No existe una carta porte para el número de documento de ingreso y la patente ingresada.");
                }
            }
            return esValido;
        }
    }
}
