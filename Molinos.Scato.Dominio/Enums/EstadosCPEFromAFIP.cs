using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Molinos.Scato.Dominio.Enums
{
    public static class EstadosCPEdeAFIP
    {
        //AC: Activa
        //CF: Activa con confirmacion de arribo
        //AN: Anulada
        //RE: Rechazado
        //CO: Activa con contingencia
        //DE: Desactivada
        //CN: Confirmada
        //BR: Borrador
        //PA: Pendiente de Aceptacion por el Productor
        //AP: Anulacion por el Productor
        //DD: Descargado en destino

        public static readonly IDictionary<string, string> Descripciones = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            {"AC", "ACTIVO" },
            {"CF", "ACTIVO CON CONFIRMACION DE ARRIBO" },
            {"AN", "ANULADO" },
            {"RE", "RECHAZADO" },
            {"CO", "ACTIVO CON CONTINGENCIA" },
            {"DE", "DESACTIVADO" },
            {"CN", "CONFIRMADO" },
            {"BR", "BORRADOR" },
            {"PA", "PENDIENTE DE ACEPTACION POR EL PRODUCTOR" },
            {"AP", "ANULACION POR EL PRODUCTOR" },
            {"DD", "DESCARGADO EN DESTINO" },
        });

        public static IReadOnlyList<string> Validos = new List<string> { "AC", "CF" };
        public static IReadOnlyList<string> Bloqueantes = new List<string> { "AN", "RE", "CO", "DE", "CN", "BR", "PA", "AP", "DD" };
        public static IReadOnlyList<string> ValidosParaConfirmacionArribo = new List<string> { "CF", "CN" };

        // Estados que NO requieren re-consulta a ARCA: si el CTG tiene uno de estos estados en caché, se confía en el dato local.
        // AC=Activa, CO=Con contingencia, CF=Con confirmación de arribo, AN=Anulada.
        // RE (Rechazada) NO está incluido — debe verificarse en ARCA porque puede haber sido corregida y re-autorizada.
        // NO modifica Validos ni Bloqueantes — son listas de validación de negocio independientes.
        public static IReadOnlyList<string> EstadosSinReConsulta = new List<string> { "AC", "CO", "CF", "AN" };
    }
}