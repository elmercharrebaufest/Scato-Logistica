using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Services;
using System.Text;
using System.Web;

namespace Molinos.Scato.WebMobile.Seguridad
{

    public sealed class ChunkedSecurityCookieHandler : CookieHandler
    {
        public const int DefaultChunkSize = 2000;

        public const int MinimumChunkSize = 1000;

        private int _chunkSize;

        public int ChunkSize => _chunkSize;

        public ChunkedSecurityCookieHandler()
            : this(2000)
        {
        }

        public ChunkedSecurityCookieHandler(int chunkSize)
        {
            if (chunkSize < 1000)
            {
                throw new ArgumentOutOfRangeException("chunkSize");
            }

            _chunkSize = chunkSize;
        }

        protected override void DeleteCore(string name, string path, string domain, HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            DeleteInternal(name, path, domain, context.Request.Cookies, context.Response.Cookies);
        }

        internal void DeleteInternal(string name, string path, string domain, HttpCookieCollection requestCookies, HttpCookieCollection responseCookies)
        {
            foreach (HttpCookie cookieChunk in GetCookieChunks(name, requestCookies))
            {
                HttpCookie httpCookie = new HttpCookie(cookieChunk.Name, null);
                httpCookie.Path = path;
                httpCookie.Expires = DateTime.UtcNow.AddDays(-1.0);
                if (!string.IsNullOrEmpty(domain))
                {
                    httpCookie.Domain = domain;
                }

                responseCookies.Set(httpCookie);
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetCookieChunks(string baseName, string cookieValue)
        {
            int chunksRequired = CeilingDivide(cookieValue.Length, _chunkSize);
            for (int i = 0; i < chunksRequired; i++)
            {
                yield return new KeyValuePair<string, string>(GetChunkName(baseName, i), cookieValue.Substring(i * _chunkSize, Math.Min(cookieValue.Length - i * _chunkSize, _chunkSize)));
            }
        }

        private int CeilingDivide(int value, int divisor)
        {
            return (value + divisor - 1) / divisor;
        }

        private IEnumerable<HttpCookie> GetCookieChunks(string baseName, HttpCookieCollection cookies)
        {
            int chunkIndex = 0;
            string chunkName = GetChunkName(baseName, chunkIndex);
            HttpCookie httpCookie;
            while ((httpCookie = cookies[chunkName]) != null)
            {
                yield return httpCookie;
                int num = chunkIndex + 1;
                chunkIndex = num;
                chunkName = GetChunkName(baseName, num);
            }
        }

        protected override byte[] ReadCore(string name, HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return ReadInternal(name, context.Request.Cookies);
        }

        internal byte[] ReadInternal(string name, HttpCookieCollection requestCookies)
        {
            StringBuilder stringBuilder = null;
            foreach (HttpCookie cookieChunk in GetCookieChunks(name, requestCookies))
            {
                if (stringBuilder == null)
                {
                    stringBuilder = new StringBuilder();
                }

                stringBuilder.Append(cookieChunk.Value);

            }

            if (stringBuilder != null)
            {
                return Convert.FromBase64String(stringBuilder.ToString());
            }

            return null;
        }

        protected override void WriteCore(byte[] value, string name, string path, string domain, DateTime expirationTime, bool secure, bool httpOnly, HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            WriteInternal(value, name, path, domain, expirationTime, secure, httpOnly, context.Request.Cookies, context.Response.Cookies);
        }

        internal void WriteInternal(byte[] value, string name, string path, string domain, DateTime expirationTime, bool secure, bool httpOnly, HttpCookieCollection requestCookies, HttpCookieCollection responseCookies)
        {
            string cookieValue = Convert.ToBase64String(value);
            DeleteInternal(name, path, domain, requestCookies, responseCookies);
            foreach (KeyValuePair<string, string> cookieChunk in GetCookieChunks(name, cookieValue))
            {
                HttpCookie httpCookie = new HttpCookie(cookieChunk.Key, cookieChunk.Value);
                httpCookie.Secure = secure;
                httpCookie.HttpOnly = httpOnly;
                httpCookie.Path = path;
                httpCookie.SameSite = SameSiteMode.Lax;
                if (!string.IsNullOrEmpty(domain))
                {
                    httpCookie.Domain = domain;
                }

                if (expirationTime != DateTime.MinValue)
                {
                    httpCookie.Expires = expirationTime;
                }

                responseCookies.Set(httpCookie);
            }
        }

        private static string GetChunkName(string baseName, int chunkIndex)
        {
            if (chunkIndex != 0)
            {
                return baseName + chunkIndex.ToString(CultureInfo.InvariantCulture);
            }

            return baseName;
        }
    }

}