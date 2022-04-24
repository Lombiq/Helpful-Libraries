using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Libraries.AspNetCore;

/// <summary>
/// An  <see cref="IAsyncDisposable"/> that replaces the <see cref="HttpContext"/>'s response stream at creation
/// with a <see cref="MemoryStream"/> and copies its content back into the real response stream during disposal. It
/// doesn't dispose anything in the <see cref="HttpContext"/>.
/// </summary>
public sealed class TemporaryResponseWrapper : IAsyncDisposable
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

    public void Reset()
    {
        _temporaryStream.Seek(0, SeekOrigin.Begin);
        _temporaryStream.SetLength(0);
    }

    public async ValueTask DisposeAsync()
    {
        _temporaryStream.Seek(0, SeekOrigin.Begin);
        await _temporaryStream.CopyToAsync(_originalBody, _httpContext.RequestAborted);
        _httpContext.Response.Body = _originalBody;

        await _temporaryStream.DisposeAsync();
    }
}
