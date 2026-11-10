using System.Diagnostics;

namespace Microsoft.Extensions.Logging;

/// <summary>
/// Lazy <see cref="ILogger"/> extension methods that defer message creation until the specific log level is enabled.
/// </summary>
/// <remarks>
/// This type intentionally lives in the <c>Microsoft.Extensions.Logging</c> namespace to match the standard logging
/// extensions, but is named <see cref="LazyLoggerExtensions"/> to avoid collisions with the framework-provided
/// <c>LoggerExtensions</c>. No additional <c>using</c> statement is required in consuming code.
/// The message returned by the factory is a message template. It is formatted by the standard logging pipeline,
/// so named placeholders such as <c>{UserId}</c> are supported and captured as structured properties.
/// </remarks>
public static class LazyLoggerExtensions
{
    /// <summary>
    /// Core implementation: evaluates the factory and logs only if the level is enabled.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> or <paramref name="messageFactory"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="messageFactory"/> returns null.
    /// </exception>
    private static void LogLazy(
        ILogger logger,
        LogLevel level,
        EventId eventId,
        Exception? exception,
        Func<string> messageFactory,
        object?[]? args )
    {
        ArgumentNullException.ThrowIfNull( logger );
        ArgumentNullException.ThrowIfNull( messageFactory );

        if ( !logger.IsEnabled( level ) )
        {
            return;
        }

        var message = messageFactory()
            ?? throw new InvalidOperationException( "messageFactory must not return null." );

        // The framework extension treats the message as a template and passes the array as-is (no double wrapping),
        // so named placeholders are captured as structured properties.
        logger.Log( level, eventId, exception, message, args ?? Array.Empty<object?>() );
    }

    /// <summary>
    /// Guards the exception argument of the exception overloads.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="exception"/> is null.</exception>
    private static Exception RequireException( Exception? exception )
    {
        ArgumentNullException.ThrowIfNull( exception );
        return exception;
    }

    // ─── Trace ─────────────────────────────────────────────────────────────

    /// <summary>Logs a trace message (lazy).</summary>
    [StackTraceHidden]
    public static void LogTrace(
        this ILogger logger, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Trace, default, null, messageFactory, null );

    /// <summary>Logs a trace message with formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogTrace(
        this ILogger logger, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Trace, default, null, messageFactory, args );

    /// <summary>Logs a trace message with an event id (lazy).</summary>
    [StackTraceHidden]
    public static void LogTrace(
        this ILogger logger, EventId eventId, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Trace, eventId, null, messageFactory, null );

    /// <summary>Logs a trace message with an event id and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogTrace(
        this ILogger logger, EventId eventId, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Trace, eventId, null, messageFactory, args );

    /// <summary>Logs a trace message with an exception (lazy).</summary>
    [StackTraceHidden]
    public static void LogTrace(
        this ILogger logger, Exception exception, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Trace, default, RequireException( exception ), messageFactory, null );

    /// <summary>Logs a trace message with an exception and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogTrace(
        this ILogger logger, Exception exception, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Trace, default, RequireException( exception ), messageFactory, args );

    /// <summary>Logs a trace message with an event id and an exception (lazy).</summary>
    [StackTraceHidden]
    public static void LogTrace(
        this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Trace, eventId, RequireException( exception ), messageFactory, null );

    /// <summary>Logs a trace message with an event id, an exception, and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogTrace(
        this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Trace, eventId, RequireException( exception ), messageFactory, args );

    // ─── Debug ─────────────────────────────────────────────────────────────

    /// <summary>Logs a debug message (lazy).</summary>
    [StackTraceHidden]
    public static void LogDebug(
        this ILogger logger, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Debug, default, null, messageFactory, null );

    /// <summary>Logs a debug message with formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogDebug(
        this ILogger logger, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Debug, default, null, messageFactory, args );

    /// <summary>Logs a debug message with an event id (lazy).</summary>
    [StackTraceHidden]
    public static void LogDebug(
        this ILogger logger, EventId eventId, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Debug, eventId, null, messageFactory, null );

    /// <summary>Logs a debug message with an event id and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogDebug(
        this ILogger logger, EventId eventId, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Debug, eventId, null, messageFactory, args );

    /// <summary>Logs a debug message with an exception (lazy).</summary>
    [StackTraceHidden]
    public static void LogDebug(
        this ILogger logger, Exception exception, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Debug, default, RequireException( exception ), messageFactory, null );

    /// <summary>Logs a debug message with an exception and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogDebug(
        this ILogger logger, Exception exception, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Debug, default, RequireException( exception ), messageFactory, args );

    /// <summary>Logs a debug message with an event id and an exception (lazy).</summary>
    [StackTraceHidden]
    public static void LogDebug(
        this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Debug, eventId, RequireException( exception ), messageFactory, null );

    /// <summary>Logs a debug message with an event id, an exception, and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogDebug(
        this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Debug, eventId, RequireException( exception ), messageFactory, args );

    // ─── Information ───────────────────────────────────────────────────────

    /// <summary>Logs an information message (lazy).</summary>
    [StackTraceHidden]
    public static void LogInformation(
        this ILogger logger, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Information, default, null, messageFactory, null );

    /// <summary>Logs an information message with formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogInformation(
        this ILogger logger, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Information, default, null, messageFactory, args );

    /// <summary>Logs an information message with an event id (lazy).</summary>
    [StackTraceHidden]
    public static void LogInformation(
        this ILogger logger, EventId eventId, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Information, eventId, null, messageFactory, null );

    /// <summary>Logs an information message with an event id and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogInformation(
        this ILogger logger, EventId eventId, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Information, eventId, null, messageFactory, args );

    /// <summary>Logs an information message with an exception (lazy).</summary>
    [StackTraceHidden]
    public static void LogInformation(
        this ILogger logger, Exception exception, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Information, default, RequireException( exception ), messageFactory, null );

    /// <summary>Logs an information message with an exception and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogInformation(
        this ILogger logger, Exception exception, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Information, default, RequireException( exception ), messageFactory, args );

    /// <summary>Logs an information message with an event id and an exception (lazy).</summary>
    [StackTraceHidden]
    public static void LogInformation(
        this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Information, eventId, RequireException( exception ), messageFactory, null );

    /// <summary>Logs an information message with an event id, an exception, and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogInformation(
        this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Information, eventId, RequireException( exception ), messageFactory, args );

    // ─── Warning ───────────────────────────────────────────────────────────

    /// <summary>Logs a warning message (lazy).</summary>
    [StackTraceHidden]
    public static void LogWarning(
        this ILogger logger, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Warning, default, null, messageFactory, null );

    /// <summary>Logs a warning message with formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogWarning(
        this ILogger logger, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Warning, default, null, messageFactory, args );

    /// <summary>Logs a warning message with an event id (lazy).</summary>
    [StackTraceHidden]
    public static void LogWarning(
        this ILogger logger, EventId eventId, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Warning, eventId, null, messageFactory, null );

    /// <summary>Logs a warning message with an event id and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogWarning(
        this ILogger logger, EventId eventId, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Warning, eventId, null, messageFactory, args );

    /// <summary>Logs a warning message with an exception (lazy).</summary>
    [StackTraceHidden]
    public static void LogWarning(
        this ILogger logger, Exception exception, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Warning, default, RequireException( exception ), messageFactory, null );

    /// <summary>Logs a warning message with an exception and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogWarning(
        this ILogger logger, Exception exception, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Warning, default, RequireException( exception ), messageFactory, args );

    /// <summary>Logs a warning message with an event id and an exception (lazy).</summary>
    [StackTraceHidden]
    public static void LogWarning(
        this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Warning, eventId, RequireException( exception ), messageFactory, null );

    /// <summary>Logs a warning message with an event id, an exception, and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogWarning(
        this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Warning, eventId, RequireException( exception ), messageFactory, args );

    // ─── Error ─────────────────────────────────────────────────────────────

    /// <summary>Logs an error message (lazy).</summary>
    [StackTraceHidden]
    public static void LogError(
        this ILogger logger, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Error, default, null, messageFactory, null );

    /// <summary>Logs an error message with formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogError(
        this ILogger logger, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Error, default, null, messageFactory, args );

    /// <summary>Logs an error message with an event id (lazy).</summary>
    [StackTraceHidden]
    public static void LogError(
        this ILogger logger, EventId eventId, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Error, eventId, null, messageFactory, null );

    /// <summary>Logs an error message with an event id and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogError(
        this ILogger logger, EventId eventId, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Error, eventId, null, messageFactory, args );

    /// <summary>Logs an error message with an exception (lazy).</summary>
    [StackTraceHidden]
    public static void LogError(
        this ILogger logger, Exception exception, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Error, default, RequireException( exception ), messageFactory, null );

    /// <summary>Logs an error message with an exception and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogError(
        this ILogger logger, Exception exception, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Error, default, RequireException( exception ), messageFactory, args );

    /// <summary>Logs an error message with an event id and an exception (lazy).</summary>
    [StackTraceHidden]
    public static void LogError(
        this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Error, eventId, RequireException( exception ), messageFactory, null );

    /// <summary>Logs an error message with an event id, an exception, and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogError(
        this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Error, eventId, RequireException( exception ), messageFactory, args );

    // ─── Critical ──────────────────────────────────────────────────────────

    /// <summary>Logs a critical message (lazy).</summary>
    [StackTraceHidden]
    public static void LogCritical(
        this ILogger logger, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Critical, default, null, messageFactory, null );

    /// <summary>Logs a critical message with formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogCritical(
        this ILogger logger, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Critical, default, null, messageFactory, args );

    /// <summary>Logs a critical message with an event id (lazy).</summary>
    [StackTraceHidden]
    public static void LogCritical(
        this ILogger logger, EventId eventId, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Critical, eventId, null, messageFactory, null );

    /// <summary>Logs a critical message with an event id and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogCritical(
        this ILogger logger, EventId eventId, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Critical, eventId, null, messageFactory, args );

    /// <summary>Logs a critical message with an exception (lazy).</summary>
    [StackTraceHidden]
    public static void LogCritical(
        this ILogger logger, Exception exception, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Critical, default, RequireException( exception ), messageFactory, null );

    /// <summary>Logs a critical message with an exception and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogCritical(
        this ILogger logger, Exception exception, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Critical, default, RequireException( exception ), messageFactory, args );

    /// <summary>Logs a critical message with an event id and an exception (lazy).</summary>
    [StackTraceHidden]
    public static void LogCritical(
        this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory )
        => LogLazy( logger, LogLevel.Critical, eventId, RequireException( exception ), messageFactory, null );

    /// <summary>Logs a critical message with an event id, an exception, and formatting arguments (lazy).</summary>
    [StackTraceHidden]
    public static void LogCritical(
        this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory, params object?[] args )
        => LogLazy( logger, LogLevel.Critical, eventId, RequireException( exception ), messageFactory, args );
}
