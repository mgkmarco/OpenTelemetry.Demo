using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Context.Propagation;

namespace OpenTelemetry.Demo.Consumer.Extensions
{
    public static class TextMapPropagatorExtensions
    {
        public static ActivityContext ExtractActivityContextFromParentContext<T>(this TextMapPropagator propagator,
            MessageProperties props, ILogger<T> logger, PropagationContext propagationContext = default) where T : class
        {
            var parentContext = propagator.Extract(propagationContext, props, (messageProps, key) =>
            {
                try
                {
                    if (messageProps.Headers.TryGetValue(key, out var value))
                    {
                        var bytes = value as byte[];
                        return new[] {Encoding.UTF8.GetString(bytes ?? Array.Empty<byte>())};
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Failed to extract trace context: {ex}");
                }
            
                return Enumerable.Empty<string>();
            });

            Baggage.Current = parentContext.Baggage;

            return parentContext.ActivityContext;
        }
    }
}