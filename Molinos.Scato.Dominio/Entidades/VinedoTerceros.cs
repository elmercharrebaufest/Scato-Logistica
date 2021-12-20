namespace Molinos.Scato.Dominio.Entidades
{
    public class VinedoTerceros : Vinedo
    {
        public virtual string CUIT_Titular { get; set; }

        public override string Vinatero()
        {
            return Descripcion;
        }

        public override string VinateroCuit()
        {
            return CUIT_Titular;
        }

        public override string CentroOperativoCodigoSap()
        {
            return null;
        }
    }
}
