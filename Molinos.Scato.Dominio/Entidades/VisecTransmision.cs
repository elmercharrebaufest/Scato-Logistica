using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class VisecTransmision : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual string CUITEmpresa { get; set; }
        public virtual DateTime FechaHoraMovimiento { get; set; }
        public virtual DateTime FechaCPE { get; set; }
        public virtual string NumeroCPE { get; set; }
        public virtual string NumeroCTG { get; set; }
        public virtual string CUITTitular { get; set; }
        public virtual int? NumeroRUCAOrigen { get; set; }
        public virtual string CUITDestinatario { get; set; }
        public virtual string CUITDestino { get; set; }
        public virtual int NumeroRUCADestino { get; set; }
        public virtual int Producto { get; set; }
        public virtual string Campania { get; set; }
        public virtual int PesoNetoCargaKg { get; set; }

        public virtual string NumeroProceso { get; set; }
        public virtual DateTime? FechaTransaccion { get; set; }
        public virtual string DetalleTransaccion { get; set; }
        public virtual string HistorialProcesos { get; set; }
        public virtual int Estado { get; set; }
        public virtual int? StockKg { get; set; }

        public List<VisecTransmisionMovimiento> VisecTransmisionMovimientos { get; set; }
    }
}