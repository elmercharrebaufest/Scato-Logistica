using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Molinos.Scato.Dominio.Comandos;

namespace Molinos.Scato.Dominio.Helpers
{
    public static class ExtensionesReflection
    {

        public static IDictionary<string, string> ObtenerPropiedades(this ITipeable objeto)
        {
            var campos = new Dictionary<string, string>();
            var tipoBase = objeto.ObtenerTipoObjeto();
            var propiedades = tipoBase == null ? new List<PropertyInfo>() : tipoBase.GetProperties().ToList();
            propiedades = propiedades.Where(p => p.DeclaringType == tipoBase).ToList();

            foreach (var campo in propiedades)
            {

                var valor = campo.GetValue(objeto, null);
                var tipo = campo.PropertyType.Name;
                if (tipo == typeof(ICollection<>).Name)
                {
                    foreach (ITipeable obj in (IEnumerable)valor)
                    {
                        var tipoB = obj.ObtenerTipoObjeto();
                        var prop = tipoB == null
                                       ? new List<PropertyInfo>()
                                       : tipoB.GetProperties().Where(x => x.PropertyType == typeof (string)).ToList();
                        foreach (var propertyInfo in prop)
                        {
                            var val = propertyInfo.GetValue(obj, null);
                            campos.Add(campo.Name + "-" + obj.Id + "-"  + propertyInfo.Name, val == null ? string.Empty : val.ToString());
                        }
                    }
                }
                else
                {
                    campos.Add(campo.Name, valor == null ? string.Empty : valor.ToString());
                }               
            }
            return campos;
        }

        public static Resultado SetearPropiedades(this ITipeable objeto, IDictionary<string, string> campos)
        {
            var resultado = new Resultado();
            var type = objeto.ObtenerTipoObjeto();

            foreach (var campo in campos.Keys)
            {
                try
                {
                    if (campo.Contains("-"))
                    {
                        var propiedadObjeto = campo.Split('-')[0];
                        var entidadId = campo.Split('-')[1];
                        var nombrePropiedadEntidad = campo.Split('-')[2];

                        var propiedad = type.GetProperty(propiedadObjeto);
                        var lista = (IEnumerable<ITipeable>)propiedad.GetValue(objeto, null);
                        var entidad = lista.First(x => x.Id == Convert.ToInt32(entidadId));

                        var propiedadEntidad = entidad.ObtenerTipoObjeto().GetProperty(nombrePropiedadEntidad);
                        var tipoPropiedad = Nullable.GetUnderlyingType(propiedadEntidad.PropertyType) ?? propiedadEntidad.PropertyType;
                        propiedadEntidad.SetValue(entidad, Convert.ChangeType(campos[campo], tipoPropiedad)); 
                    }
                    else
                    {
                        var propiedad = type.GetProperty(campo);
                        var tipoPropiedad = Nullable.GetUnderlyingType(propiedad.PropertyType) ?? propiedad.PropertyType;
                        if (propiedad.PropertyType.IsGenericType && propiedad.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>) && string.IsNullOrEmpty(campos[campo]))
                        {
                            propiedad.SetValue(objeto, null);
                        }
                        else
                        {
                            propiedad.SetValue(objeto, Convert.ChangeType(campos[campo], tipoPropiedad));
                        }                        
                    }                  
                }
                catch (Exception e)
                {
                    resultado.Error(campo,e.Message);
                }               
            }
            return resultado;
        }
    }
}
