using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Lombiq.HelpfulLibraries.OrchardCore.Caching;

/// <summary>
/// An <see cref="IFileProvider"/> implementation to be used for the webroot of the app that caches all files in memory.
/// This is to overcome storage IO performance issues on platforms like Azure App Services, see <see
/// cref="https://github.com/OrchardCMS/OrchardCore/issues/14859"/>.
/// </summary>
/// <remarks>
/// <para>
/// In the end, unresized Media files and resized images by ImageSharp are all served from the webroot, so this is the
/// lowest common denominator. Sitemaps aren't covered since those go through the module's own controller but that's
/// less of an issue.
/// </para>
/// <para>
/// File watchers take care of cache invalidation. This also works if the drive for the webroot folder is a shared one,
/// like it is with the storage of Azure App Services. So, multi-node hosting is also supported.
/// </para>
/// <para>
/// The implementation is based on the <see
/// href="https://github.com/aspnet/live.asp.net/blob/dev/src/live.asp.net/Services/CachedWebRootFileProvider.cs"><c>
/// CachedWebRootFileProvider</c> of the archived live.asp.net repo.</see>. Its license is included.
/// </para>
/// </remarks>
internal class CachedWebRootFileProvider : IFileProvider
{
    // The performance issue is pronounced if you load a lot of files at once, like a catalog page with thumbnail
    // images. These tend to be relatively small. The bottleneck is the number of storage transactions, not bandwidth,
    // so less to worry about large files being downloaded.
    // This needs to remain an int, so the cached file contents can fit into a MemoryStream.
    private const int FileSizeLimitBytes = 256 * 1024;

    private readonly ILogger<CachedWebRootFileProvider> _logger;
    private readonly IFileProvider _fileProvider;
    private readonly IMemoryCache _cache;

    public CachedWebRootFileProvider(
        ILogger<CachedWebRootFileProvider> logger,
        IWebHostEnvironment hostEnvironment,
        IMemoryCache memoryCache)
    {
        _logger = logger;
        _fileProvider = hostEnvironment.WebRootFileProvider;
        _cache = memoryCache;
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        var cacheKey = GetCacheKey(nameof(GetDirectoryContents) + "_" + subpath);

        if (_cache.TryGetValue(cacheKey, out IDirectoryContents cachedResult)) return cachedResult;

        var directoryContents = _fileProvider.GetDirectoryContents(subpath);

        if (!directoryContents.Exists) return directoryContents;

        //var cachedDirectoryContents = new CachedDirectoryContents(_logger, directoryContents, subpath);

        var cacheEntry = _cache.CreateEntry(cacheKey);
        //cacheEntry.Value = cachedDirectoryContents;
        cacheEntry.Value = directoryContents;
        cacheEntry.RegisterPostEvictionCallback((key, _, reason, _) =>
            _logger.LogDebug("Cache entry {Key} was evicted due to {Reason}.", key, reason));

        return directoryContents;
        //return cachedDirectoryContents;
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        var cacheKey = GetCacheKey(nameof(GetFileInfo) + "_" + subpath);

        if (_cache.TryGetValue(cacheKey, out IFileInfo cachedResult)) return cachedResult;

        var fileInfo = _fileProvider.GetFileInfo(subpath);

        if (!fileInfo.Exists) return fileInfo;

        if (fileInfo.Length > FileSizeLimitBytes)
        {
            _logger.LogDebug(
                "File contents for {Subpath} will not be cached as it's over the file size limit of {FileSizeLimitBytes} bytes.",
                subpath,
                FileSizeLimitBytes);

            return fileInfo;
        }

        // Create the cache entry and return
        var cachedFileInfo = new CachedFileInfo(_logger, fileInfo, subpath);

        var fileChangedToken = Watch(subpath);

        fileChangedToken.RegisterChangeCallback(
            _ => _logger.LogDebug("Change detected for {Subpath} located at {Filepath}", subpath, fileInfo.PhysicalPath),
            state: null);

        var cacheEntry = _cache.CreateEntry(cacheKey)
            .RegisterPostEvictionCallback((key, _, reason, _) =>
                _logger.LogDebug("Cache entry {Key} was evicted due to {Reason}", key, reason))
            .AddExpirationToken(fileChangedToken)
            .SetValue(cachedFileInfo);

        // You have to call Dispose() to actually add the item to the underlying cache. Yeah, I know.
        cacheEntry.Dispose();

        return cachedFileInfo;
    }

    public IChangeToken Watch(string filter) => _fileProvider.Watch(filter);

    private static string GetCacheKey(string key) => "CachedWebRootFileProvider_" + key;

    private sealed class CachedDirectoryContents : IDirectoryContents
    {
        public bool Exists => _directoryContents.Exists;

        private readonly ILogger _logger;
        private readonly IDirectoryContents _directoryContents;
        private readonly string _subpath;

        public CachedDirectoryContents(ILogger logger, IDirectoryContents directoryContents, string subpath)
        {
            _logger = logger;
            _directoryContents = directoryContents;
            _subpath = subpath;
        }

        public IEnumerator<IFileInfo> GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

        //private sealed class CachedFileInfoEnumerator : IEnumerator<IFileInfo>
        //{
        //    private readonly IEnumerator<IFileInfo> _wrappedEnumerator;

        //    public CachedFileInfoEnumerator(IEnumerator<IFileInfo> wrappedEnumerator) =>
        //        _wrappedEnumerator = wrappedEnumerator;

        //    public IFileInfo Current => new CachedFileInfo(_wrappedEnumerator.Current.PhysicalPath);

        //    object IEnumerator.Current => Current;

        //    public bool MoveNext() => _wrappedEnumerator.MoveNext();

        //    public void Reset() => _wrappedEnumerator.Reset();

        //    public void Dispose() => _wrappedEnumerator.Dispose();
        //}
    }

    private sealed class CachedFileInfo : IFileInfo
    {
        private readonly ILogger _logger;
        private readonly IFileInfo _fileInfo;
        private readonly string _subpath;
        private byte[] _content;

        public bool Exists => _fileInfo.Exists;
        public bool IsDirectory => _fileInfo.IsDirectory;
        public DateTimeOffset LastModified => _fileInfo.LastModified;
        public long Length => _fileInfo.Length;
        public string Name => _fileInfo.Name;
        public string PhysicalPath => _fileInfo.PhysicalPath;

        public CachedFileInfo(ILogger logger, IFileInfo fileInfo, string subpath)
        {
            _logger = logger;
            _fileInfo = fileInfo;
            _subpath = subpath;
        }

        public Stream CreateReadStream()
        {
            var content = _content;
            if (content != null)
            {
                _logger.LogDebug("Returning cached file contents for {subpath} located at {filepath}", _subpath, _fileInfo.PhysicalPath);
                return new MemoryStream(content);
            }
            else
            {
                _logger.LogDebug("Loading file contents for {subpath} located at {filepath}", _subpath, _fileInfo.PhysicalPath);
                MemoryStream memoryStream;
                using (var fileStream = _fileInfo.CreateReadStream())
                {
                    memoryStream = new MemoryStream((int)fileStream.Length);
                    fileStream.CopyTo(memoryStream);
                    content = memoryStream.ToArray();
                    memoryStream.Position = 0;
                }

                if (Interlocked.CompareExchange(ref _content, content, null) == null)
                {
                    _logger.LogDebug("Cached file contents for {subpath} located at {filepath}", _subpath, _fileInfo.PhysicalPath);
                }

                return memoryStream;
            }
        }
    }
}
