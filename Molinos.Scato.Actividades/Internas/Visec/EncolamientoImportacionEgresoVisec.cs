using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Linq;

namespace Molinos.Scato.Actividades.Internas
{
    public class EncolamientoImportacionEgresoVisec : CodeActivity<Resultado>
    {
        public InArgument<Guid> InstanceId { get; set; }
        
        [RequiredArgument]
        public InArgument<CartaPorteDto> CartaPorte { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
            var servicioComandos = context.GetExtension<IServicioComandos>();
            var servicioHangfire = context.GetExtension<IServicioHangfireQueue>();
            
            var instanceId = InstanceId.Get<Guid>(context);
            var cartaPorte = CartaPorte.Get<CartaPorteDto>(context);

            var recorrido = servicioRepositorio.ObtenerRecorridoPorGuid(instanceId);
            var campania = cartaPorte.Cosecha?.Replace("-", "/");
            if (campania.Length == 4)
                campania = campania.Insert(2, "/");

            var peso = recorrido.PesoNeto ?? 0;
            var producto = int.Parse(recorrido.Material.CodigoONCCA ?? "0");
            var rucaOrigen = !string.IsNullOrEmpty(recorrido.Centro.CodigoEstablecimiento) ? int.Parse(recorrido.Centro.CodigoEstablecimiento) : 0;

            var destino = servicioRepositorio.ObtenerCentro(cartaPorte.DestinoId);
            var rucaDestino = destino != null && !string.IsNullOrEmpty(destino.CodigoEstablecimiento) ? int.Parse(destino.CodigoEstablecimiento) : 0;

            var transmisionAVisec = new VisecTransmisionDto
            {
                Id = -1,
                FechaHoraMovimiento = cartaPorte.FechaEmision,
                FechaCPE = cartaPorte.FechaEmision,
                NumeroCPE = cartaPorte.Sucursal.ToString().PadLeft(5, '0') + "-" + cartaPorte.CTG,
                NumeroCTG = cartaPorte.NroCartaPorte?.PadLeft(12, '0'),
                NumeroRUCAOrigen = rucaOrigen,
                CUITTitular = cartaPorte.TitularCartaPorteCuil.Replace("-", ""),
                CUITDestinatario = cartaPorte.DestinatarioCuil.Replace("-", ""),
                CUITDestino = cartaPorte.DestinoCuit.Replace("-", ""),
                NumeroRUCADestino = rucaDestino,
                Producto = producto,
                Campania = campania,
                PesoNetoCargaKg = peso,
                StockKg = peso
            };

            var resultadoStocksModificados = servicioComandos.Ejecutar(new VisecDistribuirStock
            {
                Producto = producto,
                RUCAOrigenEgreso = rucaOrigen,
                StockSolicitado = peso
            }) as ResultadoVisecDistribuirStock;

            if (resultadoStocksModificados.HayErrores)
                return resultadoStocksModificados;

            transmisionAVisec.VisecTransmisionMovimientos = resultadoStocksModificados.Data.Select(x => new VisecTransmisionMovimientoDto
            {
                NumeroCTGAsignado = x.Key,
                PesoNetoCargaKgPorUP = x.Value,
                UltimoAlmacenamiento = recorrido.Almacen?.DescripcionCorta,
                TipoMovimiento = 2,
            }).ToList();

            var resultado = servicioComandos.Ejecutar(new CrearActualizarVisecTransmision { Dto = transmisionAVisec }) as ResultadoCrear;
            servicioHangfire.EncolarImportarCartaPorteVisec(resultado.Id);
            return resultado;
        }
    }
}