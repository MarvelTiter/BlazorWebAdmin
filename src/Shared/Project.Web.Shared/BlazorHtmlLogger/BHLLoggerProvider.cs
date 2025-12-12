using LoggerProviderExtensions.DbLogger;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Project.Web.Shared.BlazorHtmlLogger;

internal class BHLLoggerProcessor
{
    private static Lazy<BHLLoggerProcessor>? instance;
    public static Lazy<BHLLoggerProcessor> Instance
    {
        get
        {
            instance ??= new(() => new());
            return instance;
        }
    }
}

internal class BHLLogger(string category, BHLLoggerProcessor logger, IExternalScopeProvider? scopeProvider) : ILogger
{
    [ThreadStatic]
    private static StringWriter? t_stringWriter;
    public IExternalScopeProvider? ScopeProvider { get; set; } = scopeProvider;
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return ScopeProvider?.Push(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }
        var logEntry = new LogEntry<TState>(logLevel, category, eventId, state, exception, formatter);
        t_stringWriter ??= new();
        FormatContent(logEntry, ScopeProvider, t_stringWriter);
        var sb = t_stringWriter.GetStringBuilder();
        if (sb.Length == 0)
        {
            return;
        }
        string message = sb.ToString();
        sb.Clear();
    }
    private static void FormatContent<TState>(in LogEntry<TState> entry, IExternalScopeProvider? scope, StringWriter textWriter)
    {
        var message = entry.Formatter(entry.State, entry.Exception);
        var logLevelString = entry.LogLevel.ToString();
        FormatContent(scope, textWriter, logLevelString, entry.Category, entry.EventId.Id, message, string.Empty, entry.Exception?.ToString(), false, false);
    }
    private static void FormatContent(IExternalScopeProvider? scope, StringWriter textWriter
        , string? logLevelString
        , string category
        , int eventId
        , string message
        , string timestamp
        , string? exception
        , bool includeScoped
        , bool singleLine)
    {
        textWriter.Write('[');
        textWriter.Write(timestamp);
        textWriter.Write(']');

        if (logLevelString != null)
        {
            textWriter.Write('[');
            textWriter.Write(logLevelString);
            textWriter.Write(']');
        }

        textWriter.Write(':');
        textWriter.Write(category);
        textWriter.Write('[');

#if NET
        Span<char> span = stackalloc char[10];
        if (eventId.TryFormat(span, out int charsWritten))
            textWriter.Write(span.Slice(0, charsWritten));
        else
#endif
            textWriter.Write(eventId.ToString());

        textWriter.Write(']');
        if (!singleLine)
            textWriter.Write(Environment.NewLine);

        // scope information
        WriteScopeInformation(textWriter, scope, includeScoped, singleLine);
        WriteMessage(textWriter, message, false);

        // Example:
        // System.InvalidOperationException
        //    at Namespace.Class.Function() in File:line X
        if (exception != null)
        {
            // exception message
            WriteMessage(textWriter, exception, singleLine);
        }
        //if (singleLine)
        textWriter.Write(Environment.NewLine);
    }
    private static readonly string messagePadding = new string(' ', 4);
    private static readonly string newLineWithMessagePadding = Environment.NewLine + messagePadding;
    private static void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider? scopeProvider, bool includeScopeds, bool singleLine)
    {
        if (includeScopeds && scopeProvider != null)
        {
            bool paddingNeeded = !singleLine;
            scopeProvider.ForEachScope((scope, state) =>
            {
                if (paddingNeeded)
                {
                    paddingNeeded = false;
                    state.Write(messagePadding);
                    state.Write("=> ");
                }
                else
                {
                    state.Write(" => ");
                }
                state.Write(scope);
            }, textWriter);

            if (!paddingNeeded && !singleLine)
            {
                textWriter.Write(Environment.NewLine);
            }
        }
    }

    private static void WriteMessage(TextWriter textWriter, string message, bool singleLine)
    {
        if (!string.IsNullOrEmpty(message))
        {
            if (singleLine)
            {
                textWriter.Write(' ');
                WriteReplacing(textWriter, Environment.NewLine, " ", message);
            }
            else
            {
                textWriter.Write(messagePadding);
                WriteReplacing(textWriter, Environment.NewLine, newLineWithMessagePadding, message);
                textWriter.Write(Environment.NewLine);
            }
        }

        static void WriteReplacing(TextWriter writer, string oldValue, string newValue, string message)
        {
            string newMessage = message.Replace(oldValue, newValue);
            writer.Write(newMessage);
        }
    }
}

internal class BHLLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly ConcurrentDictionary<string, BHLLogger> loggers = new();
    private readonly Lazy<BHLLoggerProcessor> processor;
    private IExternalScopeProvider? scopeProvider;

    public BHLLoggerProvider()
    {
        processor = BHLLoggerProcessor.Instance;
    }
    public ILogger CreateLogger(string categoryName)
    {
        return loggers.GetOrAdd(categoryName, c =>
        {
            return new BHLLogger(c, processor.Value, scopeProvider);
        });
    }

    public void Dispose()
    {
        loggers.Clear();
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        this.scopeProvider = scopeProvider;
        foreach (var item in loggers)
        {
            item.Value.ScopeProvider = scopeProvider;
        }
    }
}
