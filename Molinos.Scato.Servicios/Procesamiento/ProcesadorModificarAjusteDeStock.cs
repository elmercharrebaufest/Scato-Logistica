using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarAjusteDeStock : ProcesadorModificar<ModificarAjusteDeStock>
    {
        public ProcesadorModificarAjusteDeStock(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarAjusteDeStock comando)
        {
            var ajuste = Repositorio.Obtener<AjusteDeStock>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, ajuste);
            if (ajuste.Material == null || ajuste.Material.Id != comando.Dto.MaterialId)
            {
                ajuste.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            }
            if (ajuste.Observaciones == null || ajuste.Observaciones != comando.Dto.Observaciones)
            {
                ajuste.Observaciones = comando.Dto.Observaciones;
            }
            if (ajuste.PesoBrutoIngreso == null || ajuste.PesoBrutoIngreso != comando.Dto.PesoBrutoIngreso)
            {
                ajuste.PesoBrutoIngreso = comando.Dto.PesoBrutoIngreso;
            }
            if (ajuste.PesoNetoEgreso == null || ajuste.PesoNetoEgreso != comando.Dto.PesoNetoEgreso)
            {
                ajuste.PesoNetoEgreso = comando.Dto.PesoNetoEgreso;
            }
            if (ajuste.PesoNetoIngreso == null || ajuste.PesoNetoIngreso != comando.Dto.PesoNetoIngreso)
            {
                ajuste.PesoNetoIngreso = comando.Dto.PesoNetoIngreso;
            }
            if (ajuste.TipoComprobanteOncca == null ||
                ajuste.TipoComprobanteOncca.Id != comando.Dto.TipoComprobanteOnccaId)
            {
                ajuste.TipoComprobanteOncca = Repositorio.Obtener<TipoComprobanteOncca>(comando.Dto.TipoComprobanteOnccaId);
            }
            if (ajuste.NumeroDocumentoIngreso == null || ajuste.NumeroDocumentoIngreso != comando.Dto.NumeroDocumentoIngreso)
            {
                ajuste.NumeroDocumentoIngreso = comando.Dto.NumeroDocumentoIngreso;
            }
            if (ajuste.NumeroCTG == null || ajuste.NumeroCTG != comando.Dto.NumeroCTG)
            {
                ajuste.NumeroCTG = comando.Dto.NumeroCTG;
            }
            Repositorio.Agregar(new LogAjusteDeStock
                {
                    Centro = ajuste.Centro,
                    Fecha = DateTime.Now,
                    FechaAjuste = ajuste.Fecha,
                    Material = ajuste.Material,
                    NumeroDocumentoIngreso = ajuste.NumeroDocumentoIngreso,
                    NumeroCTG = ajuste.NumeroCTG,
                    Observaciones = ajuste.Observaciones,
                    PesoBrutoIngreso = ajuste.PesoBrutoIngreso,
                    PesoNetoEgreso = ajuste.PesoNetoEgreso,
                    PesoNetoIngreso = ajuste.PesoNetoIngreso,
                    Tipo = "Modificación",
                    TipoComprobanteOncca = ajuste.TipoComprobanteOncca,
                    NombreUsuario = comando.Dto.NombreUsuario
                });
        }
    
        protected override void Validar(ModificarAjusteDeStock comando, Resultado resultado)
        {
            if (comando.Dto.MaterialId != 0 && !Repositorio.Existe<Material>(x => x.Id != comando.Dto.MaterialId))
            {
                resultado.Error("MaterialId", Textos.Error_Invalido);
            }
        }
    }
}
