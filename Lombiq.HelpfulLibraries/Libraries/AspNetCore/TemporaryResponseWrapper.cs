using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Lombiq.HelpfulLibraries.Libraries.AspNetCore
{
    /// <summary>
    /// An  <see cref="IDisposable"/> that replaces the <see cref="HttpContext"/>'s response stream at creation with a
    /// <see cref="MemoryStream"/> and copies its content back into the real response stream during disposal. It doesn't
    /// dispose anything in the <see cref="HttpContext"/>.
    /// </summary>
    public sealed class TemporaryResponseWrapper : IDisposable
    {
        private readonly HttpContext _httpContext;
        private readonly MemoryStream _temporaryStream;
        private readonly Stream _originalBody;

        public TemporaryResponseWrapper(HttpContext httpContext)
        {
            _httpContext = httpContext;

            _temporaryStream = new MemoryStream();
            _originalBody = httpContext.Response.Body;
            httpContext.Response.Body = _temporaryStream;
        }

        public void Dispose()
        {
            _temporaryStream.Seek(0, SeekOrigin.Begin);
            _temporaryStream.CopyTo(_originalBody);
            _httpContext.Response.Body = _originalBody;

            _temporaryStream.Dispose();
        }

        public void Reset()
        {
            _temporaryStream.Seek(0, SeekOrigin.Begin);
            _temporaryStream.SetLength(0);
        }
    }
}
