#nullable enable
using System;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microsoft.Extensions.Logging.Tests
{
    public class LoggerExtensionsLazyTests
    {
        // ─── Setup ───────────────────────────────────────────────────────────────

        private static Mock<ILogger> CreateLogger( LogLevel enabledLevel )
        {
            var mock = new Mock<ILogger>( MockBehavior.Strict );
            mock.Setup( l => l.IsEnabled( It.IsAny<LogLevel>() ) )
                .Returns<LogLevel>( level => level == enabledLevel );
            mock.Setup( l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>() ) );
            return mock;
        }

        private static Mock<ILogger> CreateDisabledLogger()
        {
            var mock = new Mock<ILogger>( MockBehavior.Strict );
            mock.Setup( l => l.IsEnabled( It.IsAny<LogLevel>() ) ).Returns( false );
            // Log() must NEVER be called → no setup → Strict will throw
            return mock;
        }

        // ─── Null-Guard Tests ─────────────────────────────────────────────────────

        [Fact]
        public void LogDebug_NullLogger_ThrowsArgumentNullException()
        {
            ILogger? logger = null;
            Assert.Throws<ArgumentNullException>( () => logger!.LogDebug( () => "test" ) );
        }

        [Fact]
        public void LogDebug_NullFactory_ThrowsArgumentNullException()
        {
            var logger = CreateLogger( LogLevel.Debug ).Object;
            Assert.Throws<ArgumentNullException>( () => logger.LogDebug( (Func<string>)null! ) );
        }

        [Fact]
        public void LogError_NullException_ThrowsArgumentNullException()
        {
            var logger = CreateLogger( LogLevel.Error ).Object;
            Assert.Throws<ArgumentNullException>( () => logger.LogError( (Exception)null!, () => "msg" ) );
        }

        [Fact]
        public void LogCritical_NullException_ThrowsArgumentNullException()
        {
            var logger = CreateLogger( LogLevel.Critical ).Object;
            Assert.Throws<ArgumentNullException>( () => logger.LogCritical( (Exception)null!, () => "msg" ) );
        }

        // ─── Factory returns null ─────────────────────────────────────────────────

        [Fact]
        public void LogDebug_FactoryReturnsNull_ThrowsInvalidOperationException()
        {
            var logger = CreateLogger( LogLevel.Debug );
            // Re-allow Log() call that would have been triggered
            Assert.Throws<InvalidOperationException>( () => logger.Object.LogDebug( () => null! ) );
        }

        // ─── Lazy Evaluation: delegate must NOT be called when level is OFF ───────

        [Theory]
        [InlineData( LogLevel.Trace )]
        [InlineData( LogLevel.Debug )]
        [InlineData( LogLevel.Information )]
        [InlineData( LogLevel.Warning )]
        [InlineData( LogLevel.Error )]
        [InlineData( LogLevel.Critical )]
        public void LazyDelegate_IsNotInvoked_WhenLevelDisabled( LogLevel level )
        {
            var logger = CreateDisabledLogger();
            var invoked = 0;
            Func<string> factory = () => { invoked++; return "msg"; };

            LogViaLevel( logger.Object, level, factory, null );

            Assert.Equal( 0, invoked );
        }

        // ─── Lazy Evaluation: delegate MUST be called when level is ON ────────────

        [Theory]
        [InlineData( LogLevel.Trace )]
        [InlineData( LogLevel.Debug )]
        [InlineData( LogLevel.Information )]
        [InlineData( LogLevel.Warning )]
        [InlineData( LogLevel.Error )]
        [InlineData( LogLevel.Critical )]
        public void LazyDelegate_IsInvoked_WhenLevelEnabled( LogLevel level )
        {
            var logger = CreateLogger( level );
            var invoked = 0;
            Func<string> factory = () => { invoked++; return "hello {Name}"; };

            LogViaLevel( logger.Object, level, factory, new object?[] { "World" } );

            Assert.Equal( 1, invoked );
        }

        // ─── Exception overloads ─────────────────────────────────────────────────

        [Fact]
        public void Error_WithException_DoesNotInvokeDelegate_WhenDisabled()
        {
            var logger = CreateDisabledLogger();
            var invoked = 0;
            logger.Object.LogError( new InvalidOperationException( "x" ), () => { invoked++; return "boom"; } );
            Assert.Equal( 0, invoked );
        }

        [Fact]
        public void Error_WithException_InvokesDelegate_WhenEnabled()
        {
            var logger = CreateLogger( LogLevel.Error );
            var invoked = 0;
            logger.Object.LogError( new InvalidOperationException( "x" ), () => { invoked++; return "boom {Code}"; }, 500 );
            Assert.Equal( 1, invoked );
        }

        [Fact]
        public void Critical_WithException_DoesNotInvokeDelegate_WhenDisabled()
        {
            var logger = CreateDisabledLogger();
            var invoked = 0;
            logger.Object.LogCritical( new InvalidOperationException( "x" ), () => { invoked++; return "boom"; } );
            Assert.Equal( 0, invoked );
        }

        [Fact]
        public void Critical_WithException_InvokesDelegate_WhenEnabled()
        {
            var logger = CreateLogger( LogLevel.Critical );
            var invoked = 0;
            logger.Object.LogCritical( new InvalidOperationException( "x" ), () => { invoked++; return "boom {Code}"; }, 42 );
            Assert.Equal( 1, invoked );
        }

        // ─── Warning exception overload (now present) ─────────────────────────────

        [Fact]
        public void Warning_WithException_DoesNotInvokeDelegate_WhenDisabled()
        {
            var logger = CreateDisabledLogger();
            var invoked = 0;
            logger.Object.LogWarning( new Exception( "w" ), () => { invoked++; return "warn"; } );
            Assert.Equal( 0, invoked );
        }

        [Fact]
        public void Warning_WithException_InvokesDelegate_WhenEnabled()
        {
            var logger = CreateLogger( LogLevel.Warning );
            var invoked = 0;
            logger.Object.LogWarning( new Exception( "w" ), () => { invoked++; return "warn {X}"; }, "val" );
            Assert.Equal( 1, invoked );
        }

        // ─── Helper ──────────────────────────────────────────────────────────────

        private static void LogViaLevel( ILogger logger, LogLevel level, Func<string> factory, object?[]? args )
        {
            args ??= Array.Empty<object?>();
            switch ( level )
            {
                case LogLevel.Trace:       logger.LogTrace( factory, args );       return;
                case LogLevel.Debug:       logger.LogDebug( factory, args );       return;
                case LogLevel.Information: logger.LogInformation( factory, args ); return;
                case LogLevel.Warning:     logger.LogWarning( factory, args );     return;
                case LogLevel.Error:       logger.LogError( factory, args );       return;
                case LogLevel.Critical:    logger.LogCritical( factory, args );    return;
                default: throw new ArgumentOutOfRangeException( nameof( level ) );
            }
        }
    }
}
