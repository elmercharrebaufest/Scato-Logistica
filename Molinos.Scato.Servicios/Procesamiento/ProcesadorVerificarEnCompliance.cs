using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.ComplianceWebServiceV2;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using NPOI.POIFS.Properties;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorVerificarEnCompliance : ProcesadorComando<VerificarEnCompliance>
    {

        private readonly DatosPort servicioCompliance;
        public ProcesadorVerificarEnCompliance(IRepositorio repositorio, IConversor conversor, ILogger log , DatosPort servicioCompliance)
            : base(repositorio, conversor, log )
        {
            this.servicioCompliance = servicioCompliance;
        }

        public override Resultado Ejecutar(VerificarEnCompliance comando)
        {
            ResultadoValidarCompliance resultado = new ResultadoValidarCompliance();
            try
            {
                Log.Debug($"ProcesadorVerificarSalidaFlete: Patente:{comando.Patente},  Cuit: {comando.Cuit}, Planta: {comando.Planta}, Dni Chofer: {comando.Dni}");

                if (comando.Dni.Length == 7)
                {
                    comando.Dni = "0" + comando.Dni;
                }
                ComplianceV2(comando.Planta, comando.Cuit, comando.Dni, comando.Patente, ref resultado);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error al procesar la validacion de Compliance: {e.Message}");
                resultado.Error("", e.Message);
            }
            return resultado;
        }

        private void ComplianceV2(string planta, string cuit, string dniChofer, string patente , ref ResultadoValidarCompliance resultado)
        {
            

            controlarDatosAgroacopiosRequest datosGranelesRequest = new controlarDatosAgroacopiosRequest()
            {
                datos = new Datos()
                {
                    cuit = cuit,
                    dni = dniChofer,
                    patente1 = patente,
                    planta = planta
                }
            };

            try
            {
                var request = datosGranelesRequest.ToXml();
                Log.Debug($"Request Compliance: {request}");
                var respuesta = servicioCompliance.controlarDatosAgroacopios(datosGranelesRequest);
                if(respuesta?.controlarDatosReturn is null)
                {
                    throw new Exception("Respuesta nula");
                }
                var response = respuesta.ToXml();
                Log.Debug($"Response Compliance: {response}");

                if (respuesta.controlarDatosReturn.codigoError != 0 || respuesta.controlarDatosReturn.colorEmpresa == 1 ||
                respuesta.controlarDatosReturn.colorChofer == 1 ||
                respuesta.controlarDatosReturn.colorVehiculo1 == 1 || respuesta.controlarDatosReturn.colorVehiculo2 == 1)
                {
                   resultado.Error("", ObtenerDescripcionMensaje(respuesta.controlarDatosReturn));
                }
                else
                {
                   resultado.Valido();
                }

            }
            catch (Exception e)
            {
                resultado.Error("", e.Message);
            }
           
        }

        private string ObtenerDescripcionMensaje(Response respuesta)
        {
            string mensaje = string.Empty;
            if (respuesta.codigoError != 0)
            {
                switch (respuesta.codigoError)
                {
                    case 1:
                        mensaje = "No existe el vehículo T";
                        break;
                    case 2:
                        mensaje = "No existe el vehículo A";
                        break;
                    case 3:
                        mensaje = "El Vehiculo T no pertenece a la empresa";
                        break;
                    case 4:
                        mensaje = "El Vehiculo A no pertenece a la empresa";
                        break;
                    case 5:
                        mensaje = "El vehículo T es un A";
                        break;
                    case 6:
                        mensaje = "El vehículo A es un T";
                        break;
                    case 7:
                        mensaje = "El chofer no existe";
                        break;
                    case 8:
                        mensaje = "El chofer no pertenece a la empresa";
                        break;
                    case 9:
                        mensaje = "La empresa no existe";
                        break;
                    case 10:
                        mensaje = "10";
                        break;
                    case 11:
                        mensaje = "11";
                        break;
                    case 12:
                        mensaje = "12";
                        break;
                    case 13:
                        mensaje = "Chofer bloquedo por AVL";
                        break;
                    case 14:
                        mensaje = "Unidad Tractor bloqueada por AVL";
                        break;
                    case 15:
                        mensaje = "Unidad Acoplado bloqueada por AVL";
                        break;
                    case 16:
                        mensaje = "Chofer bloqueado por empresa subcontratista";
                        break;
                    case 17:
                        mensaje = "Unidad Tractor bloqueada por empresa subcontratista";
                        break;
                    case 18:
                        mensaje = "Unidad Acoplado bloqueada por empresa subcontratista";
                        break;
                    case 20:
                        mensaje = "Error de conexión";
                        break;
                }
                mensaje += " ";
            }

            if (respuesta.colorChofer == 1)
            {
                mensaje += "El chofer se encuentra inhabilitado en Web Compliance";
                mensaje += " ";
            }
            if (respuesta.colorEmpresa == 1)
            {
                mensaje += "La empresa se encuentra inhabilitada en Web Compliance";
                mensaje += " ";
            }
            if (respuesta.colorVehiculo1 == 1 || respuesta.colorVehiculo2 == 1)
            {
                mensaje += "El vehículo se encuentra inhabilitado en Web Compliance";
            }
            return mensaje;
        }
    }
}
