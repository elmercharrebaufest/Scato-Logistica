using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;

namespace Molinos.Scato.Repositorio
{
    public class Cache : ICache
    {
        private MemoryCache cache;

        public Cache()
        {
            cache = MemoryCache.Default;
        }

        public TEntidad Agregar<TEntidad>(string clave, TEntidad entidad, DateTimeOffset? tiempoDeExpiracion = null) where TEntidad : class
        {
            if (tiempoDeExpiracion.HasValue)
            {
                cache.Set(clave, entidad, tiempoDeExpiracion.Value);
            }
            else
            {
                var policy = new CacheItemPolicy();
                cache.Set(clave, entidad, policy);

            }

            return entidad;
        }

        public void Dispose()
        {
            cache.Dispose();
        }

        public bool Existe(string clave)
        {
            return cache.Contains(clave);
        }

        public TEntidad Obtener<TEntidad>(string clave) where TEntidad : class
        {
            return (TEntidad)cache.Get(clave);
        }

        public void Remover(string clave)
        {
            if (Existe(clave)) cache.Remove(clave);
        }

        public List<TEntidad> ObtenerPorGrupo<TEntidad>(string group) where TEntidad : class
        {
            var claves = cache.Where(x => x.Key.StartsWith(group)).Select(kvp => kvp.Key).ToList();
            var items = new List<TEntidad>();
            if (claves != null)
            {
                foreach (var clave in claves)
                {
                    items.Add((TEntidad)cache.Get(clave));
                }
            }
            return items;
        }

        public void RemoverTodos()
        {
            var claves = cache.Select(kvp => kvp.Key).ToList();
            foreach (var clave in claves)
            {
                Remover(clave);
            }
        }

        public void RemoverPorGrupo(string group)
        {
            var claves = cache.Where(x => x.Key.StartsWith(group)).Select(kvp => kvp.Key).ToList();
            foreach (var clave in claves)
            {
                Remover(clave);
            }
        }
    }
}