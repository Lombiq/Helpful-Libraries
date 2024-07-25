namespace Lombiq.HelpfulLibraries.AspNetCore.Security;

/// <summary>
/// The <c>Content-Security-Policy</c> directives defined in the <see href="https://www.w3.org/TR/CSP2/#directives">W3C
/// Recommendation</see> (also see the <see
/// href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy#directives">MDN page</see>).
/// </summary>
public static class ContentSecurityPolicyDirectives
{
    public const string BaseUri = "base-uri";
    public const string ChildSrc = "child-src";
    public const string ConnectSrc = "connect-src";
    public const string DefaultSrc = "default-src";
    public const string FontSrc = "font-src";
    public const string FormAction = "form-action";
    public const string FrameAncestors = "frame-ancestors";
    public const string FrameSrc = "frame-src";
    public const string ImgSrc = "img-src";
    public const string ManifestSrc = "manifest-src";
    public const string MediaSrc = "media-src";
    public const string ObjectSrc = "object-src";
    public const string PluginTypes = "plugin-types";
    public const string ReportTo = "report-to";
    public const string ReportUri = "report-uri";
    public const string Sandbox = "sandbox";
    public const string ScriptSrc = "script-src";
    public const string ScriptSrcAttr = "script-src-attr";
    public const string ScriptSrcElem = "script-src-elem";
    public const string StyleSrc = "style-src";
    public const string StyleSrcAttr = "style-src-attr";
    public const string UpgradeInsecureRequests = "upgrade-insecure-requests";
    public const string StyleSrcElem = "style-src-elem";
    public const string WorkerSrc = "worker-src";

    public static class CommonValues
    {
        // These values represent special words so they must be surrounded with apostrophes.
        public const string Self = "'self'";
        public const string None = "'none'";
        public const string UnsafeInline = "'unsafe-inline'";
        public const string UnsafeEval = "'unsafe-eval'";

        // These values represent allowed protocol schemes.
        public const string Https = "https:";
        public const string Data = "data:";
        public const string Blob = "blob:";
    }
}
