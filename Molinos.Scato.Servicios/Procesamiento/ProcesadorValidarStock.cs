using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorValidarStock : ProcesadorComando<ValidarStock>
    {
        private readonly IServicioComandos servicioComandos;

        public ProcesadorValidarStock(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(ValidarStock comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Debug($"Valido Stock para: {comando.InstanceId}");
                var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
                var neto = recorrido.PesoBrutoFecha.HasValue && recorrido.PesoTaraFecha.HasValue ?
                    (recorrido.PesoBruto ?? 0) - (recorrido.PesoTara ?? 0) : 
                    30000;

                var stock = Repositorio.Obtener<Stock>(x => x.Recorrido.Id == recorrido.Id);
                if (stock != null)
                {
                    stock.Cantidad = neto * -1;
                    Repositorio.GuardarCambios();
                }
                else if (neto <= 0)
                {
                    Log.Debug($"Valido Stock para: {comando.InstanceId} - {Textos.AsignacionEstablecimientoError_SinNetoOrigen}");
                    resultado.Error("", Textos.AsignacionEstablecimientoError_SinNetoOrigen);
                }
                else if (SaldoStock(comando, recorrido) - neto < 0)
                {
                    var mensaje = string.Format(Textos.LimiteStockSuperado, recorrido.Vehiculo.CartaPorte.Destinatario.Id, recorrido.Material.Id);
                    Log.Debug($"Valido Stock para: {comando.InstanceId} - {mensaje}");
                    resultado.Error("", mensaje);
                }
                else
                {
                    Repositorio.Agregar(new Stock
                    {
                        Cantidad = neto * -1,
                        Fecha = DateTime.Now,
                        Material = recorrido.Material,
                        Proveedor= Repositorio.Obtener<Proveedor>(recorrido.Vehiculo.CartaPorte.Destinatario.Id),
                        Recorrido= recorrido
                    });
                    Log.Debug($"Valido Stock para: {comando.InstanceId} - ok");
                    Repositorio.GuardarCambios();
                }
                return resultado;
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error ValidarStock {comando.InstanceId}");
                resultado.Error("", Textos.Error_ActualizarGenerico);
                return resultado;
            }
        }

        public decimal SaldoStock(ValidarStock comando, Recorrido recorrido)
        {
            try
            {
                servicioComandos.Ejecutar(new ActualizarStock { Fecha = DateTime.Now, MaterialId = recorrido.Material.Id, ProveedorId = recorrido.Vehiculo.CartaPorte.Destinatario.Id });

            }
            catch (Exception e)
            {
                Log.Error(e, $"No se pudo actualizar el stock mat {recorrido.Material.Id}, {DateTime.Now}, proveedor {recorrido.Vehiculo.CartaPorte.Destinatario.Id}");
            }


            var pesosNeto = Repositorio.Sumar<Stock>(x => x.Cantidad, x => x.Proveedor.Id == recorrido.Vehiculo.CartaPorte.Destinatario.Id && x.Material.Id == recorrido.Material.Id && (x.Recorrido == null || !x.Recorrido.Rechazado));
            return pesosNeto;
        }


    }
}