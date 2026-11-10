namespace Microsoft.Extensions.Logging.Tests;

public class LazyLoggerExtensionsTests
{
    private const string Template = "Hello {Name}";

    private static readonly EventId TestEventId = new( 42, "TestEvent" );
    private static readonly Exception TestException = new InvalidOperationException( "boom" );
    private static readonly object?[] TestArgs = ["World"];

    private static readonly LogLevel[] AllLevels =
    [
        LogLevel.Trace,
        LogLevel.Debug,
        LogLevel.Information,
        LogLevel.Warning,
        LogLevel.Error,
        LogLevel.Critical,
    ];

    private delegate void LogCall(
        ILogger logger, EventId eventId, Exception exception, Func<string> factory, object?[] args );

    private sealed record Overload( LogCall Call, bool HasEventId, bool HasException, bool HasArgs );

    private sealed record LogEntry(
        LogLevel Level,
        EventId EventId,
        Exception? Exception,
        string Message,
        IReadOnlyList<KeyValuePair<string, object?>>? State );

    // ─── Theory data: every public overload of every level ───────────────────

    public static TheoryData<LogLevel, int> AllOverloads => BuildData( _ => true );

    public static TheoryData<LogLevel, int> OverloadsWithArgs => BuildData( o => o.HasArgs );

    public static TheoryData<LogLevel, int> OverloadsWithException => BuildData( o => o.HasException );

    private static TheoryData<LogLevel, int> BuildData( Func<Overload, bool> filter )
    {
        var data = new TheoryData<LogLevel, int>();

        foreach ( var level in AllLevels )
        {
            var overloads = OverloadsFor( level );

            for ( var i = 0; i < overloads.Length; i++ )
            {
                if ( filter( overloads[i] ) )
                {
                    data.Add( level, i );
                }
            }
        }

        return data;
    }

    private static Overload[] OverloadsFor( LogLevel level ) => level switch
    {
        LogLevel.Trace =>
        [
            new Overload( ( l, _, _, f, _ ) => l.LogTrace( f ), false, false, false ),
            new Overload( ( l, _, _, f, a ) => l.LogTrace( f, a ), false, false, true ),
            new Overload( ( l, id, _, f, _ ) => l.LogTrace( id, f ), true, false, false ),
            new Overload( ( l, id, _, f, a ) => l.LogTrace( id, f, a ), true, false, true ),
            new Overload( ( l, _, ex, f, _ ) => l.LogTrace( ex, f ), false, true, false ),
            new Overload( ( l, _, ex, f, a ) => l.LogTrace( ex, f, a ), false, true, true ),
            new Overload( ( l, id, ex, f, _ ) => l.LogTrace( id, ex, f ), true, true, false ),
            new Overload( ( l, id, ex, f, a ) => l.LogTrace( id, ex, f, a ), true, true, true ),
        ],
        LogLevel.Debug =>
        [
            new Overload( ( l, _, _, f, _ ) => l.LogDebug( f ), false, false, false ),
            new Overload( ( l, _, _, f, a ) => l.LogDebug( f, a ), false, false, true ),
            new Overload( ( l, id, _, f, _ ) => l.LogDebug( id, f ), true, false, false ),
            new Overload( ( l, id, _, f, a ) => l.LogDebug( id, f, a ), true, false, true ),
            new Overload( ( l, _, ex, f, _ ) => l.LogDebug( ex, f ), false, true, false ),
            new Overload( ( l, _, ex, f, a ) => l.LogDebug( ex, f, a ), false, true, true ),
            new Overload( ( l, id, ex, f, _ ) => l.LogDebug( id, ex, f ), true, true, false ),
            new Overload( ( l, id, ex, f, a ) => l.LogDebug( id, ex, f, a ), true, true, true ),
        ],
        LogLevel.Information =>
        [
            new Overload( ( l, _, _, f, _ ) => l.LogInformation( f ), false, false, false ),
            new Overload( ( l, _, _, f, a ) => l.LogInformation( f, a ), false, false, true ),
            new Overload( ( l, id, _, f, _ ) => l.LogInformation( id, f ), true, false, false ),
            new Overload( ( l, id, _, f, a ) => l.LogInformation( id, f, a ), true, false, true ),
            new Overload( ( l, _, ex, f, _ ) => l.LogInformation( ex, f ), false, true, false ),
            new Overload( ( l, _, ex, f, a ) => l.LogInformation( ex, f, a ), false, true, true ),
            new Overload( ( l, id, ex, f, _ ) => l.LogInformation( id, ex, f ), true, true, false ),
            new Overload( ( l, id, ex, f, a ) => l.LogInformation( id, ex, f, a ), true, true, true ),
        ],
        LogLevel.Warning =>
        [
            new Overload( ( l, _, _, f, _ ) => l.LogWarning( f ), false, false, false ),
            new Overload( ( l, _, _, f, a ) => l.LogWarning( f, a ), false, false, true ),
            new Overload( ( l, id, _, f, _ ) => l.LogWarning( id, f ), true, false, false ),
            new Overload( ( l, id, _, f, a ) => l.LogWarning( id, f, a ), true, false, true ),
            new Overload( ( l, _, ex, f, _ ) => l.LogWarning( ex, f ), false, true, false ),
            new Overload( ( l, _, ex, f, a ) => l.LogWarning( ex, f, a ), false, true, true ),
            new Overload( ( l, id, ex, f, _ ) => l.LogWarning( id, ex, f ), true, true, false ),
            new Overload( ( l, id, ex, f, a ) => l.LogWarning( id, ex, f, a ), true, true, true ),
        ],
        LogLevel.Error =>
        [
            new Overload( ( l, _, _, f, _ ) => l.LogError( f ), false, false, false ),
            new Overload( ( l, _, _, f, a ) => l.LogError( f, a ), false, false, true ),
            new Overload( ( l, id, _, f, _ ) => l.LogError( id, f ), true, false, false ),
            new Overload( ( l, id, _, f, a ) => l.LogError( id, f, a ), true, false, true ),
            new Overload( ( l, _, ex, f, _ ) => l.LogError( ex, f ), false, true, false ),
            new Overload( ( l, _, ex, f, a ) => l.LogError( ex, f, a ), false, true, true ),
            new Overload( ( l, id, ex, f, _ ) => l.LogError( id, ex, f ), true, true, false ),
            new Overload( ( l, id, ex, f, a ) => l.LogError( id, ex, f, a ), true, true, true ),
        ],
        LogLevel.Critical =>
        [
            new Overload( ( l, _, _, f, _ ) => l.LogCritical( f ), false, false, false ),
            new Overload( ( l, _, _, f, a ) => l.LogCritical( f, a ), false, false, true ),
            new Overload( ( l, id, _, f, _ ) => l.LogCritical( id, f ), true, false, false ),
            new Overload( ( l, id, _, f, a ) => l.LogCritical( id, f, a ), true, false, true ),
            new Overload( ( l, _, ex, f, _ ) => l.LogCritical( ex, f ), false, true, false ),
            new Overload( ( l, _, ex, f, a ) => l.LogCritical( ex, f, a ), false, true, true ),
            new Overload( ( l, id, ex, f, _ ) => l.LogCritical( id, ex, f ), true, true, false ),
            new Overload( ( l, id, ex, f, a ) => l.LogCritical( id, ex, f, a ), true, true, true ),
        ],
        _ => throw new ArgumentOutOfRangeException( nameof( level ) ),
    };

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static ILogger CreateDisabledLogger()
    {
        var logger = A.Fake<ILogger>();
        A.CallTo( () => logger.IsEnabled( A<LogLevel>._ ) ).Returns( false );
        return logger;
    }

    private static ILogger CreateLogger( LogLevel enabledLevel, List<LogEntry> entries )
    {
        var logger = A.Fake<ILogger>();
        A.CallTo( () => logger.IsEnabled( A<LogLevel>._ ) )
            .ReturnsLazily( ( LogLevel level ) => level == enabledLevel );
        A.CallTo( logger )
            .Where( call => call.Method.Name == nameof( ILogger.Log ) )
            .Invokes( call => entries.Add( ToEntry( call ) ) );
        return logger;
    }

    private static LogEntry ToEntry( IFakeObjectCall call )
    {
        var formatter = call.Arguments[4] as Delegate
            ?? throw new InvalidOperationException( "The formatter was not passed to ILogger.Log." );
        var message = formatter.DynamicInvoke( call.Arguments[2], call.Arguments[3] ) as string
            ?? throw new InvalidOperationException( "The formatter returned no message." );

        return new LogEntry(
            call.GetArgument<LogLevel>( 0 ),
            call.GetArgument<EventId>( 1 ),
            call.GetArgument<Exception?>( 3 ),
            message,
            call.Arguments[2] as IReadOnlyList<KeyValuePair<string, object?>> );
    }

    private static Overload GetOverload( LogLevel level, int index ) => OverloadsFor( level )[index];

    private static void Invoke( Overload overload, ILogger logger, Func<string> factory )
        => overload.Call( logger, TestEventId, TestException, factory, TestArgs );

    private static LogEntry LogOnce( LogLevel level, int index )
    {
        var entries = new List<LogEntry>();
        Invoke( GetOverload( level, index ), CreateLogger( level, entries ), () => Template );
        return entries.Single();
    }

    // ─── Lazy evaluation ─────────────────────────────────────────────────────

    [Theory]
    [MemberData( nameof( AllOverloads ) )]
    public void Log_WhenLevelIsDisabled_DoesNotEvaluateMessageFactory( LogLevel level, int index )
    {
        // Arrange
        var logger = CreateDisabledLogger();
        var invocations = 0;

        // Act
        Invoke( GetOverload( level, index ), logger, () =>
        {
            invocations++;
            return Template;
        } );

        // Assert
        invocations.ShouldBe( 0 );
    }

    [Theory]
    [MemberData( nameof( AllOverloads ) )]
    public void Log_WhenLevelIsDisabled_WritesNothing( LogLevel level, int index )
    {
        // Arrange
        var logger = CreateDisabledLogger();

        // Act
        Invoke( GetOverload( level, index ), logger, () => Template );

        // Assert
        A.CallTo( logger )
            .Where( call => call.Method.Name == nameof( ILogger.Log ) )
            .MustNotHaveHappened();
    }

    [Theory]
    [MemberData( nameof( AllOverloads ) )]
    public void Log_WhenLevelIsEnabled_EvaluatesMessageFactoryOnce( LogLevel level, int index )
    {
        // Arrange
        var logger = CreateLogger( level, [] );
        var invocations = 0;

        // Act
        Invoke( GetOverload( level, index ), logger, () =>
        {
            invocations++;
            return Template;
        } );

        // Assert
        invocations.ShouldBe( 1 );
    }

    // ─── Written entry ───────────────────────────────────────────────────────

    [Theory]
    [MemberData( nameof( AllOverloads ) )]
    public void Log_WhenLevelIsEnabled_WritesEntryWithRequestedLevel( LogLevel level, int index )
    {
        // Act
        var entry = LogOnce( level, index );

        // Assert
        entry.Level.ShouldBe( level );
    }

    [Theory]
    [MemberData( nameof( AllOverloads ) )]
    public void Log_WhenLevelIsEnabled_WritesGivenEventIdOrDefault( LogLevel level, int index )
    {
        // Arrange
        var expected = GetOverload( level, index ).HasEventId ? TestEventId : default;

        // Act
        var entry = LogOnce( level, index );

        // Assert
        entry.EventId.ShouldBe( expected );
    }

    [Theory]
    [MemberData( nameof( AllOverloads ) )]
    public void Log_WhenLevelIsEnabled_WritesGivenExceptionOrNone( LogLevel level, int index )
    {
        // Arrange
        var expected = GetOverload( level, index ).HasException ? TestException : null;

        // Act
        var entry = LogOnce( level, index );

        // Assert
        entry.Exception.ShouldBeSameAs( expected );
    }

    [Theory]
    [MemberData( nameof( AllOverloads ) )]
    public void Log_WhenLevelIsEnabled_WritesMessageFormattedWithArgumentsOrTemplate( LogLevel level, int index )
    {
        // Arrange
        var expected = GetOverload( level, index ).HasArgs ? "Hello World" : Template;

        // Act
        var entry = LogOnce( level, index );

        // Assert
        entry.Message.ShouldBe( expected );
    }

    [Theory]
    [MemberData( nameof( OverloadsWithArgs ) )]
    public void Log_WithArguments_CapturesNamedPlaceholderAsStructuredProperty( LogLevel level, int index )
    {
        // Act
        var entry = LogOnce( level, index );

        // Assert
        entry.State.ShouldNotBeNull().ShouldContain( new KeyValuePair<string, object?>( "Name", "World" ) );
    }

    [Theory]
    [MemberData( nameof( OverloadsWithArgs ) )]
    public void Log_WithArguments_KeepsOriginalTemplateInStructuredState( LogLevel level, int index )
    {
        // Act
        var entry = LogOnce( level, index );

        // Assert
        entry.State.ShouldNotBeNull()
            .ShouldContain( new KeyValuePair<string, object?>( "{OriginalFormat}", Template ) );
    }

    // ─── Argument validation ─────────────────────────────────────────────────

    [Theory]
    [MemberData( nameof( AllOverloads ) )]
    public void Log_WithNullLogger_ThrowsArgumentNullExceptionForLogger( LogLevel level, int index )
    {
        // Arrange
        var overload = GetOverload( level, index );
        ILogger logger = null!; // intentionally invalid argument to test the guard clause

        // Act
        var exception = Should.Throw<ArgumentNullException>( () => Invoke( overload, logger, () => Template ) );

        // Assert
        exception.ParamName.ShouldBe( "logger" );
    }

    [Theory]
    [MemberData( nameof( AllOverloads ) )]
    public void Log_WithNullMessageFactory_ThrowsArgumentNullExceptionForMessageFactory( LogLevel level, int index )
    {
        // Arrange
        var overload = GetOverload( level, index );
        var logger = CreateLogger( level, [] );
        Func<string> factory = null!; // intentionally invalid argument to test the guard clause

        // Act
        var exception = Should.Throw<ArgumentNullException>( () => Invoke( overload, logger, factory ) );

        // Assert
        exception.ParamName.ShouldBe( "messageFactory" );
    }

    [Theory]
    [MemberData( nameof( OverloadsWithException ) )]
    public void Log_WithNullException_ThrowsArgumentNullExceptionForException( LogLevel level, int index )
    {
        // Arrange
        var overload = GetOverload( level, index );
        var logger = CreateLogger( level, [] );
        Exception exception = null!; // intentionally invalid argument to test the guard clause

        // Act
        var thrown = Should.Throw<ArgumentNullException>(
            () => overload.Call( logger, TestEventId, exception, () => Template, TestArgs ) );

        // Assert
        thrown.ParamName.ShouldBe( "exception" );
    }

    [Theory]
    [MemberData( nameof( OverloadsWithException ) )]
    public void Log_WithNullExceptionAndDisabledLevel_StillThrowsArgumentNullException( LogLevel level, int index )
    {
        // Arrange
        var overload = GetOverload( level, index );
        var logger = CreateDisabledLogger();
        Exception exception = null!; // intentionally invalid argument to test the guard clause

        // Act / Assert
        Should.Throw<ArgumentNullException>(
            () => overload.Call( logger, TestEventId, exception, () => Template, TestArgs ) );
    }

    [Theory]
    [MemberData( nameof( AllOverloads ) )]
    public void Log_WhenMessageFactoryReturnsNull_ThrowsInvalidOperationException( LogLevel level, int index )
    {
        // Arrange
        var overload = GetOverload( level, index );
        var logger = CreateLogger( level, [] );
        Func<string> factory = () => null!; // intentionally returns null to test the guard clause

        // Act / Assert
        Should.Throw<InvalidOperationException>( () => Invoke( overload, logger, factory ) );
    }

    // ─── Regressions ─────────────────────────────────────────────────────────

    [Fact]
    public void LogDebug_WithNamedPlaceholder_WritesFormattedMessageInsteadOfThrowingFormatException()
    {
        // Arrange
        var entries = new List<LogEntry>();
        var logger = CreateLogger( LogLevel.Debug, entries );

        // Act
        logger.LogDebug( () => "This is a test for {DebugLevel}", "DebugLevel" );

        // Assert
        entries.Single().Message.ShouldBe( "This is a test for DebugLevel" );
    }

    [Fact]
    public void LogInformation_WithBracesAndNoArguments_WritesMessageUnchanged()
    {
        // Arrange
        var entries = new List<LogEntry>();
        var logger = CreateLogger( LogLevel.Information, entries );

        // Act
        logger.LogInformation( () => "Payload {\"id\": 1} {unclosed" );

        // Assert
        entries.Single().Message.ShouldBe( "Payload {\"id\": 1} {unclosed" );
    }
}
