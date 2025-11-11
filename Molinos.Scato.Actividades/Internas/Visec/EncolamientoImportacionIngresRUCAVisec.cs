using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using System.Activities;
using System.Collections.Generic;

namespace Molinos.Scato.Actividades.Internas
{
    public class EncolamientoImportacionIngresRUCAVisec : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<CartaPorteDto> CartaPorte { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
            var servicioComandos = context.GetExtension<IServicioComandos>();
            var servicioHangfire = context.GetExtension<IServicioHangfireQueue>();
            var recorrido = servicioRepositorio.ObtenerRecorridoPorGuid(context.WorkflowInstanceId);
            var cartaPorte = CartaPorte.Get<CartaPorteDto>(context);

            var campania = cartaPorte.Cosecha?.Replace("-", "/");
            if (campania.Length == 4)
                campania = campania.Insert(2, "/");

            var numeroCartaPorte = cartaPorte.NroCartaPorte?.PadLeft(12, '0');
            var peso = recorrido.PesoNeto ?? 0;
            var rucaDestino = !string.IsNullOrEmpty(recorrido.Centro.CodigoEstablecimiento) ? int.Parse(recorrido.Centro.CodigoEstablecimiento) : 0;

            var movimientos = new List<VisecTransmisionMovimientoDto>();
            var transmisionAVisec = new VisecTransmisionDto
            {
                Id = -1,
                FechaHoraMovimiento = cartaPorte.FechaEmision,
                FechaCPE = cartaPorte.FechaEmision,
                NumeroCPE = cartaPorte.Sucursal.ToString().PadLeft(5, '0') + "-" + cartaPorte.CTG,
                NumeroCTG = numeroCartaPorte,
                NumeroRUCAOrigen = !string.IsNullOrEmpty(cartaPorte.CodEstab) ? int.Parse(cartaPorte.CodEstab) : 0,
                CUITTitular = cartaPorte.TitularCartaPorteCuil.Replace("-", ""),
                CUITDestinatario = cartaPorte.DestinatarioCuil.Replace("-", ""),
                CUITDestino = cartaPorte.DestinoCuit.Replace("-", ""),
                NumeroRUCADestino = rucaDestino,
                Producto = int.Parse(recorrido.Material.CodigoONCCA ?? "0"),
                Campania = campania,
                PesoNetoCargaKg = peso,
                StockKg = peso,
                VisecTransmisionMovimientos = movimientos
            };

            var movimiento = new VisecTransmisionMovimientoDto
            {
                PesoNetoDescargaKgPorUP = peso,
                PesoIngresoStockKg = peso,
                TipoMovimiento = 1,
            };
            movimientos.Add(movimiento);

            var resultado = servicioComandos.Ejecutar(new CrearActualizarVisecTransmision { Dto = transmisionAVisec }) as ResultadoCrear;
            servicioHangfire.EncolarImportarCartaPorteVisec(resultado.Id);
            return resultado;
        }
    }
}