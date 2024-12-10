using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.Helpers
{
    /// <summary>
    /// Clase auxiliar que contiene métodos para manipular el ModelState en ASP.NET.
    /// </summary>
    public static class ModelStateHelper
    {
        /// <summary>
        /// Elimina la validación de una propiedad específica del ModelState basada en una expresión lambda.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo que contiene la propiedad.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad que se desea eliminar del ModelState.</typeparam>
        /// <param name="ModelState">Instancia del diccionario ModelState que contiene el estado de la validación.</param>
        /// <param name="expression">Expresión lambda que hace referencia a la propiedad que se desea remover del ModelState.</param>
        /// <remarks>
        /// Este método elimina la propiedad especificada de las futuras validaciones del ModelState,
        /// es útil cuando se necesita excluir una propiedad de la validación en un escenario específico.
        /// </remarks>
        public static void RemoveValidation<TModel, TProperty>(this ModelStateDictionary ModelState, Expression<Func<TModel, TProperty>> expression)
        {
            // Obtiene el nombre completo de la propiedad anidada usando la expresión lambda proporcionada
            var propertyName = GetNestedPropertyName(expression);

            // Elimina la propiedad especificada del ModelState para que no se valide en futuras validaciones
            ModelState.Remove(propertyName);
        }

        /// <summary>
        /// Obtiene el nombre completo de una propiedad anidada a partir de una expresión lambda.
        /// </summary>
        /// <param name="expression">Expresión lambda que hace referencia a la propiedad anidada.</param>
        /// <returns>
        /// El nombre completo de la propiedad, incluyendo todas las propiedades intermedias, 
        /// por ejemplo, "Propiedad1.Propiedad2".
        /// </returns>
        /// <remarks>
        /// Este método recorre la expresión lambda para extraer el nombre completo de la propiedad 
        /// desde la raíz hasta la propiedad más interna.
        /// </remarks>
        private static string GetNestedPropertyName(LambdaExpression expression)
        {
            // Extraer el cuerpo de la expresión (que es un MemberExpression)
            var memberExpression = expression.Body as MemberExpression;

            // Lista para almacenar los nombres de las propiedades en la jerarquía
            var propertyNames = new List<string>();

            // Mientras haya una MemberExpression (es decir, mientras haya propiedades anidadas)
            while (memberExpression != null)
            {
                // Insertar el nombre de la propiedad al principio de la lista para mantener el orden correcto
                // de las propiedades desde la raíz hasta la propiedad más interna
                propertyNames.Insert(0, memberExpression.Member.Name);

                // Accede a la expresión que precede a la propiedad para continuar recorriendo la jerarquía
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            // Unir todos los nombres de las propiedades con un punto (.) para formar el nombre completo
            // de la propiedad anidada (por ejemplo, "Propiedad1.Propiedad2")
            return string.Join(".", propertyNames);
        }
    }
}