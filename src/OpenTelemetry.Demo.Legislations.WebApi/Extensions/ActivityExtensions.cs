using System;
using System.Collections.Generic;
using System.Diagnostics;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Context.Propagation;

namespace OpenTelemetry.Demo.Legislations.WebApi.Extensions
{
    public static class ActivityExtensions
    {
        public static void AddActivityToHeader<T>(this Activity activity, TextMapPropagator propagator,
            Dictionary<string, object> tags, MessageProperties props, ILogger<T> logger) where T : class
        {
            propagator.Inject(new PropagationContext(activity.Context, Baggage.Current), props,
                (messageProps, key, value) =>
                {
                    try
                    {
                        messageProps.Headers ??= new Dictionary<string, object>();
                        messageProps.Headers[key] = value;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to inject trace context.");
                    }
                });

            foreach (var kvp in tags)
            {
                activity?.SetTag(kvp.Key, kvp.Value);
            }
        }
    }
}