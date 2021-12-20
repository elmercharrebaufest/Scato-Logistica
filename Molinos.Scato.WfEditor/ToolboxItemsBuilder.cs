using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Toolbox;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Molinos.Scato.WfEditor
{
    public class ToolboxItemsBuilder
    {
        private readonly ToolboxCategoryItems toolboxItems;

        private readonly IDictionary<ToolboxCategory, IList<string>> actividadesCargadas;
        private readonly IDictionary<string, ToolboxCategory> categoriasCargadas;

        private ToolboxItemsBuilder()
        {
            actividadesCargadas = new Dictionary<ToolboxCategory, IList<string>>();
            categoriasCargadas = new Dictionary<string, ToolboxCategory>();
            toolboxItems = new ToolboxCategoryItems();
        }

        public static ToolboxItemsBuilder Items()
        {
            return new ToolboxItemsBuilder();
        }

        public ToolboxCategoryItems Build()
        {
            return toolboxItems;
        }

        public ToolboxItemsBuilder AgregarCategoria(string nombre, IEnumerable<Type> actividades)
        {
            actividades = actividades.OrderBy(act => act.Name);
            foreach (var tipoActividad in actividades)
            {
                if (EsActividadDeToolboxValida(tipoActividad))
                {
                    var categoria = ObtenerCategoria(nombre);

                    if (!actividadesCargadas[categoria].Contains(tipoActividad.FullName))
                    {
                        var splitName = tipoActividad.Name.Split('`');
                        var displayName = splitName.Length == 1 ? tipoActividad.Name : string.Format("{0}<>", splitName[0]);

                        actividadesCargadas[categoria].Add(tipoActividad.FullName);
                        categoria.Add(new ToolboxItemWrapper(tipoActividad.FullName, tipoActividad.Assembly.FullName, null, displayName));
                    }
                }
            }
            return this;
        }

        public ToolboxItemsBuilder AgregarCategoria(string nombre, Assembly assembly, string includeNamespace = null)
        {
            return AgregarCategoria(nombre, assembly.GetTypes().Where(t => t.Namespace == includeNamespace));
        }

        private bool EsActividadDeToolboxValida(Type tipoActividad)
        {
            return tipoActividad.IsPublic && !tipoActividad.IsNested && !tipoActividad.IsAbstract
                && (typeof(Activity).IsAssignableFrom(tipoActividad)
                    || typeof(IActivityTemplateFactory).IsAssignableFrom(tipoActividad)
                    || typeof(FlowNode).IsAssignableFrom(tipoActividad));
        }

        private ToolboxCategory ObtenerCategoria(string nombre)
        {
            ToolboxCategory categoria;
            categoriasCargadas.TryGetValue(nombre, out categoria);
            if (categoria == null)
            {
                categoria = new ToolboxCategory(nombre);
                categoriasCargadas[nombre] = categoria;
                actividadesCargadas.Add(categoria, new List<string>());
                toolboxItems.Add(categoria);
            }
            return categoria;
        }

        
    }
}
