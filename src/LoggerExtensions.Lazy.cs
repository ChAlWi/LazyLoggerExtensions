using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Logging
{
	/// <summary>
	/// Lazy ILogger extension methods that defer message creation until the specific log level is enabled.
	/// </summary>
	/// <remarks>
	/// This type intentionally lives in the <c>Microsoft.Extensions.Logging</c> namespace to match the standard logging extensions,
	/// but is named <see cref="LazyLoggerExtensions"/> to avoid collisions with framework-provided <c>LoggerExtensions</c>.
	/// No additional <c>using</c> statement is required in consuming code.
	/// </remarks>
	public static class LazyLoggerExtensions
	{
		/// <summary>
		/// Core implementation: evaluates the factory and logs only if the level is enabled.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> or <paramref name="messageFactory"/> is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown when <paramref name="messageFactory"/> returns null.</exception>
		private static void LogLazy(
			ILogger logger,
			LogLevel level,
			EventId eventId,
			Exception? exception,
			Func<string> messageFactory,
			object?[]? args )
		{
			if ( logger is null )
				throw new ArgumentNullException( nameof( logger ) );

			if ( messageFactory is null )
				throw new ArgumentNullException( nameof( messageFactory ) );

			if ( !logger.IsEnabled( level ) )
				return;

			string message = messageFactory()
				?? throw new InvalidOperationException( "messageFactory must not return null." );

			// Call ILogger.Log<TState> directly to avoid the "params object?[] args" double-wrapping
			// bug that can occur when forwarding args into other extension methods.
			if ( args is null || args.Length == 0 )
			{
				logger.Log<string>( level, eventId, message, exception, static ( state, _ ) => state );
				return;
			}

			logger.Log<object?[]>(
				level,
				eventId,
				args,
				exception,
				( state, _ ) => string.Format( message, state ) );
		}

		// ─── Trace ───────────────────────────────────────────────────────────────

		/// <summary>Logs a trace message (lazy).</summary>
		[StackTraceHidden]
		public static void LogTrace( this ILogger logger, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Trace, default, null, messageFactory, null );

		/// <summary>Logs a trace message with formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogTrace( this ILogger logger, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Trace, default, null, messageFactory, args );

		/// <summary>Logs a trace message with an event id (lazy).</summary>
		[StackTraceHidden]
		public static void LogTrace( this ILogger logger, EventId eventId, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Trace, eventId, null, messageFactory, null );

		/// <summary>Logs a trace message with an event id and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogTrace( this ILogger logger, EventId eventId, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Trace, eventId, null, messageFactory, args );

		/// <summary>Logs a trace message with an exception (lazy).</summary>
		[StackTraceHidden]
		public static void LogTrace( this ILogger logger, Exception exception, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Trace, default, exception, messageFactory, null );

		/// <summary>Logs a trace message with an exception and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogTrace( this ILogger logger, Exception exception, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Trace, default, exception, messageFactory, args );

		/// <summary>Logs a trace message with an event id and an exception (lazy).</summary>
		[StackTraceHidden]
		public static void LogTrace( this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Trace, eventId, exception, messageFactory, null );

		/// <summary>Logs a trace message with an event id, an exception, and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogTrace( this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Trace, eventId, exception, messageFactory, args );

		// ─── Debug ───────────────────────────────────────────────────────────────

		/// <summary>Logs a debug message (lazy).</summary>
		[StackTraceHidden]
		public static void LogDebug( this ILogger logger, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Debug, default, null, messageFactory, null );

		/// <summary>Logs a debug message with formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogDebug( this ILogger logger, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Debug, default, null, messageFactory, args );

		/// <summary>Logs a debug message with an event id (lazy).</summary>
		[StackTraceHidden]
		public static void LogDebug( this ILogger logger, EventId eventId, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Debug, eventId, null, messageFactory, null );

		/// <summary>Logs a debug message with an event id and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogDebug( this ILogger logger, EventId eventId, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Debug, eventId, null, messageFactory, args );

		/// <summary>Logs a debug message with an exception (lazy).</summary>
		[StackTraceHidden]
		public static void LogDebug( this ILogger logger, Exception exception, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Debug, default, exception, messageFactory, null );

		/// <summary>Logs a debug message with an exception and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogDebug( this ILogger logger, Exception exception, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Debug, default, exception, messageFactory, args );

		/// <summary>Logs a debug message with an event id and an exception (lazy).</summary>
		[StackTraceHidden]
		public static void LogDebug( this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Debug, eventId, exception, messageFactory, null );

		/// <summary>Logs a debug message with an event id, an exception, and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogDebug( this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Debug, eventId, exception, messageFactory, args );

		// ─── Information ─────────────────────────────────────────────────────────

		/// <summary>Logs an information message (lazy).</summary>
		[StackTraceHidden]
		public static void LogInformation( this ILogger logger, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Information, default, null, messageFactory, null );

		/// <summary>Logs an information message with formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogInformation( this ILogger logger, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Information, default, null, messageFactory, args );

		/// <summary>Logs an information message with an event id (lazy).</summary>
		[StackTraceHidden]
		public static void LogInformation( this ILogger logger, EventId eventId, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Information, eventId, null, messageFactory, null );

		/// <summary>Logs an information message with an event id and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogInformation( this ILogger logger, EventId eventId, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Information, eventId, null, messageFactory, args );

		/// <summary>Logs an information message with an exception (lazy).</summary>
		[StackTraceHidden]
		public static void LogInformation( this ILogger logger, Exception exception, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Information, default, exception, messageFactory, null );

		/// <summary>Logs an information message with an exception and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogInformation( this ILogger logger, Exception exception, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Information, default, exception, messageFactory, args );

		/// <summary>Logs an information message with an event id and an exception (lazy).</summary>
		[StackTraceHidden]
		public static void LogInformation( this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Information, eventId, exception, messageFactory, null );

		/// <summary>Logs an information message with an event id, an exception, and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogInformation( this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Information, eventId, exception, messageFactory, args );

		// ─── Warning ─────────────────────────────────────────────────────────────

		/// <summary>Logs a warning message (lazy).</summary>
		[StackTraceHidden]
		public static void LogWarning( this ILogger logger, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Warning, default, null, messageFactory, null );

		/// <summary>Logs a warning message with formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogWarning( this ILogger logger, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Warning, default, null, messageFactory, args );

		/// <summary>Logs a warning message with an event id (lazy).</summary>
		[StackTraceHidden]
		public static void LogWarning( this ILogger logger, EventId eventId, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Warning, eventId, null, messageFactory, null );

		/// <summary>Logs a warning message with an event id and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogWarning( this ILogger logger, EventId eventId, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Warning, eventId, null, messageFactory, args );

		/// <summary>Logs a warning message with an exception (lazy).</summary>
		[StackTraceHidden]
		public static void LogWarning( this ILogger logger, Exception exception, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Warning, default, exception, messageFactory, null );

		/// <summary>Logs a warning message with an exception and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogWarning( this ILogger logger, Exception exception, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Warning, default, exception, messageFactory, args );

		/// <summary>Logs a warning message with an event id and an exception (lazy).</summary>
		[StackTraceHidden]
		public static void LogWarning( this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Warning, eventId, exception, messageFactory, null );

		/// <summary>Logs a warning message with an event id, an exception, and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogWarning( this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Warning, eventId, exception, messageFactory, args );

		// ─── Error ───────────────────────────────────────────────────────────────

		/// <summary>Logs an error message (lazy).</summary>
		[StackTraceHidden]
		public static void LogError( this ILogger logger, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Error, default, null, messageFactory, null );

		/// <summary>Logs an error message with formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogError( this ILogger logger, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Error, default, null, messageFactory, args );

		/// <summary>Logs an error message with an event id (lazy).</summary>
		[StackTraceHidden]
		public static void LogError( this ILogger logger, EventId eventId, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Error, eventId, null, messageFactory, null );

		/// <summary>Logs an error message with an event id and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogError( this ILogger logger, EventId eventId, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Error, eventId, null, messageFactory, args );

		/// <summary>Logs an error message with an exception (lazy).</summary>
		[StackTraceHidden]
		public static void LogError( this ILogger logger, Exception exception, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Error, default, exception, messageFactory, null );

		/// <summary>Logs an error message with an exception and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogError( this ILogger logger, Exception exception, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Error, default, exception, messageFactory, args );

		/// <summary>Logs an error message with an event id and an exception (lazy).</summary>
		[StackTraceHidden]
		public static void LogError( this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Error, eventId, exception, messageFactory, null );

		/// <summary>Logs an error message with an event id, an exception, and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogError( this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Error, eventId, exception, messageFactory, args );

		// ─── Critical ────────────────────────────────────────────────────────────

		/// <summary>Logs a critical message (lazy).</summary>
		[StackTraceHidden]
		public static void LogCritical( this ILogger logger, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Critical, default, null, messageFactory, null );

		/// <summary>Logs a critical message with formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogCritical( this ILogger logger, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Critical, default, null, messageFactory, args );

		/// <summary>Logs a critical message with an event id (lazy).</summary>
		[StackTraceHidden]
		public static void LogCritical( this ILogger logger, EventId eventId, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Critical, eventId, null, messageFactory, null );

		/// <summary>Logs a critical message with an event id and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogCritical( this ILogger logger, EventId eventId, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Critical, eventId, null, messageFactory, args );

		/// <summary>Logs a critical message with an exception (lazy).</summary>
		[StackTraceHidden]
		public static void LogCritical( this ILogger logger, Exception exception, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Critical, default, exception, messageFactory, null );

		/// <summary>Logs a critical message with an exception and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogCritical( this ILogger logger, Exception exception, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Critical, default, exception, messageFactory, args );

		/// <summary>Logs a critical message with an event id and an exception (lazy).</summary>
		[StackTraceHidden]
		public static void LogCritical( this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory )
			=> LogLazy( logger, LogLevel.Critical, eventId, exception, messageFactory, null );

		/// <summary>Logs a critical message with an event id, an exception, and formatting arguments (lazy).</summary>
		[StackTraceHidden]
		public static void LogCritical( this ILogger logger, EventId eventId, Exception exception, Func<string> messageFactory, params object?[] args )
			=> LogLazy( logger, LogLevel.Critical, eventId, exception, messageFactory, args );
	}
}
