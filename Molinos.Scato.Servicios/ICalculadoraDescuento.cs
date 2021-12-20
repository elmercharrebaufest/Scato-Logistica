using System;
using System.Collections.Generic;
using System.ServiceModel;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios
{
    [ServiceContract]
    public interface ICalculadoraDescuento
    {
        [OperationContract]
        decimal CalcularPorcentajeMuestraAuditoria(IList<AnalisisPorCaracteristica> analisis, Guid instanceId);
        [OperationContract]
        decimal CalcularDescuentoEnPorcentaje(CaracteristicaDeCalidad caracteristica, decimal valorMedicion, Guid instanceId);
        [OperationContract]
        decimal CalcularDescuentoEnKg(CaracteristicaDeCalidad caracteristica, decimal valorMedicion, int pesoNetoOrigen, Guid instanceId);
        [OperationContract]
        bool ExisteExcepcionAlDescuento(Guid instanceId, int caracteristicaDeCalidadId);
        [OperationContract]
        decimal TotalKilosDescuento(CaladoDto calado, AnalisisDeCalidadDto analisis, int pesoNeto);
        [OperationContract]
        decimal ActualizarMermaVolatil(Calado calado, AnalisisDeCalidad analisis, int pesoNeto);
        [OperationContract]
        void ActualizarEstado(Calado calado, AnalisisDeCalidad analisis, CaracteristicasAnalizadas estado, bool tieneEntregador = false);
        [OperationContract]
        void ActualizarEstadoEspecial(Calado calado, AnalisisDeCalidad analisis, CaracteristicasAnalizadas estado);
        [OperationContract]
        bool EnviaACamara(CaladoPorCaracteristica dto, Guid instanceId);
        [OperationContract]
        bool EnviaACamara(AnalisisPorCaracteristica obj, Guid instanceId);
    }
}
