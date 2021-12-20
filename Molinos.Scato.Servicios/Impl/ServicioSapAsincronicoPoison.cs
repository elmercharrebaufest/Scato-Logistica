using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Impl
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class ServicioSapAsincronicoPoison : IServicioSapAsincronico
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IConversor conversor;
        private readonly ILogger log;

        public ServicioSapAsincronicoPoison(IServicioComandos servicioComandos, IConversor conversor, ILogger log)
        {
            this.servicioComandos = servicioComandos;
            this.conversor = conversor;
            this.log = log;
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void IngresoPorCompraDeGranos(Guid idInstancia, Fill_Z1000 registro)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.IngresosPorCompraDeGranos);
            try
            {
                var transmision = new TransmisionASapDto
                {
                    Estado = EstadoTransmisionASap.Error,
                    FuncionSap = FuncionSAP.IngresosPorCompraDeGranos,
                    InstanciaWorkflow = idInstancia
                };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void ZE7550(Guid idInstancia, Z_SDMF_RFC_ZE7550 registro)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.IngresosPorCompraDeGranos);
            try
            {
                var transmision = new TransmisionASapDto
                {
                    Estado = EstadoTransmisionASap.Error,
                    FuncionSap = FuncionSAP.ZE7550,
                    InstanciaWorkflow = idInstancia
                };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void SalidaDeOrigenEnRedespachos(Guid idInstancia, Mov975 mov975)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.SalidaDeOrigenEnRedespachos);
            try
            {
                var transmision = new TransmisionASapDto
                {
                    Estado = EstadoTransmisionASap.Error,
                    FuncionSap = FuncionSAP.SalidaDeOrigenEnRedespachos,
                    InstanciaWorkflow = idInstancia
                };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void LlegadaADestinoEnRedespachos(Guid idInstancia, Mov305 mov305)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.LlegadaADestinosEnRedespachos);
            try
            {
                var transmision = new TransmisionASapDto
                {
                    Estado = EstadoTransmisionASap.Error,
                    FuncionSap = FuncionSAP.LlegadaADestinosEnRedespachos,
                    InstanciaWorkflow = idInstancia
                };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void EgresosMaterialNoProductivo(Guid idInstancia, EgresosNoProductivos egresosNoProductivos)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.EgresosMaterialNoProductivo);
            try
            {
                var transmision = new TransmisionASapDto
                    {
                        Estado = EstadoTransmisionASap.Error,
                        FuncionSap = FuncionSAP.EgresosMaterialNoProductivo,
                        InstanciaWorkflow = idInstancia
                    };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap {Dto = transmision});
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
            
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void IngresosEgresosFazones(Guid idInstancia, IngresosEgresosFazones ingresosEgresosFazones)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.IngresosEgresosFazones);
            try
            {
                var transmision = new TransmisionASapDto
                {
                    Estado = EstadoTransmisionASap.Error,
                    FuncionSap = FuncionSAP.IngresosEgresosFazones,
                    InstanciaWorkflow = idInstancia
                };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void PesaNeto(Guid idInstancia, PesaNeto pesaNeto)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.PesaNeto);
            try
            {
                var transmision = new TransmisionASapDto
                {
                    Estado = EstadoTransmisionASap.Error,
                    FuncionSap = FuncionSAP.PesaNeto,
                    InstanciaWorkflow = idInstancia
                };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AjusteDeDiferenciasDePesoEnRedespachos(Guid idInstancia, MovAjuste movAjuste) 
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.AjusteDeDiferencias);
            try
            {
                var transmision = new TransmisionASapDto
                {
                    Estado = EstadoTransmisionASap.Error,
                    FuncionSap = FuncionSAP.AjusteDeDiferencias,
                    InstanciaWorkflow = idInstancia
                };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void FletesDobleTramo(Guid idInstancia, FletesDobleTramo fletesDobleTramo)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.FletesDobleTramo);
            try
            {
                var transmision = new TransmisionASapDto
                {
                    Estado = EstadoTransmisionASap.Error,
                    FuncionSap = FuncionSAP.FletesDobleTramo,
                    InstanciaWorkflow = idInstancia
                };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void EgresoSinFleteFazones(Guid idInstancia, EgresoSinFleteFazones mov291)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.EgresoSinFleteFazones);
            try
            {
                var transmision = new TransmisionASapDto
                {
                    Estado = EstadoTransmisionASap.Error,
                    FuncionSap = FuncionSAP.EgresoSinFleteFazones,
                    InstanciaWorkflow = idInstancia
                };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void RegistrarCartaDePorteTransporteAutomotor(Guid idInstancia, CartaPorteTransporteAutomotorRegistro cartaPorteRegistro)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.CartaPorteTransporteAutomotorRegistro);
            try
            {
                var transmision = new TransmisionASapDto
                {
                    Estado = EstadoTransmisionASap.Error,
                    FuncionSap = FuncionSAP.CartaPorteTransporteAutomotorRegistro,
                    InstanciaWorkflow = idInstancia
                };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void RegistrarCartaDePorteVagonFerroviario(Guid idInstancia, CartaPorteVagonFerroviarioRegistro cartaPorteRegistro)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.CartaPorteVagonFerroviarioRegistro);
            try
            {
                var transmision = new TransmisionASapDto
                {
                    Estado = EstadoTransmisionASap.Error,
                    FuncionSap = FuncionSAP.CartaPorteVagonFerroviarioRegistro,
                    InstanciaWorkflow = idInstancia
                };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void RegistrarMuestreoYPesajeTransporteAutomotor(Guid idInstancia, MuestreoPesajeTransporteAutomotor muestreoRegistro)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.MuestreoPesajeTransporteAutomotorRegistro);
            try
            {
                var transmision = new TransmisionASapDto
                {
                    Estado = EstadoTransmisionASap.Error,
                    FuncionSap = FuncionSAP.MuestreoPesajeTransporteAutomotorRegistro,
                    InstanciaWorkflow = idInstancia
                };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void RegistrarMuestreoYPesajeVagonFerroviario(Guid idInstancia, MuestreoPesajeVagonFerroviario muestreoRegistro)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.MuestreoPesajeVagonFerroviarioRegistro);
            try
            {
                var transmision = new TransmisionASapDto
                {
                    Estado = EstadoTransmisionASap.Error,
                    FuncionSap = FuncionSAP.MuestreoPesajeVagonFerroviarioRegistro,
                    InstanciaWorkflow = idInstancia
                };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void InformarCupo(Guid idInstancia, Z_SDMF_Z2200N informarCupo)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.InformarCupo);
            try
            {
                var transmision = new TransmisionASapDto
                {
                    Estado = EstadoTransmisionASap.Error,
                    FuncionSap = FuncionSAP.InformarCupo,
                    InstanciaWorkflow = idInstancia
                };
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void IngresosBodega(Guid idInstancia, IngresosBodegaAsincronicoDto ingresosBodega)
        {
            log.Info("Iniciando Poison para: " + FuncionSAP.IngresosBodega);
            try
            {
                var transmision = conversor.Convertir<IngresosBodega, IngresosBodegaTransmisionASap>(ingresosBodega.IngresosBodega.IngresosBodega);
                transmision.FuncionSap = FuncionSAP.IngresosBodega;
                transmision.InstanciaWorkflow = idInstancia;
                transmision.Estado = EstadoTransmisionASap.Error;

                servicioComandos.Ejecutar(new ActualizarIngresosBodegaTransmisionASap { Dto = transmision, TipoBinId = ingresosBodega.TipoBinId });
            }
            catch (Exception e)
            {
                log.Error(e, "ERROR en Poison para: " + idInstancia);
                throw;
            }
            
        }
    }
}
