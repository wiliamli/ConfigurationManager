using System;
using System.Net;

namespace Jwell.ConfigurationManager.Exceptions
{
    public class JwellConfigStatusCodeException : Exception
    {
        public JwellConfigStatusCodeException(HttpStatusCode statusCode, string message)
            : base($"[status code: {statusCode:D}] {message}")
        {
            StatusCode = statusCode;
        }

        public virtual HttpStatusCode StatusCode { get; }
    }
}
