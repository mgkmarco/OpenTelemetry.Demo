using System;
using System.Collections.Generic;
using System.Diagnostics;
using EasyNetQ;
using OpenTelemetry.Context.Propagation;

namespace OpenTelemetry.Demo.Legislations.WebApi.Extensions
{
    public static class ActivityExtensions
    {
        public static void AddActivityToHeader(this Activity activity, TextMapPropagator propagator, Dictionary<string, object> tags, MessageProperties props)
        {
            propagator.Inject(new PropagationContext(activity.Context, Baggage.Current), props, InjectContextIntoHeader);

            foreach (var kvp in tags)
            {
                activity?.SetTag(kvp.Key, kvp.Value);
            }
        }

        private static void InjectContextIntoHeader(MessageProperties props, string key, string value)
        {
            try
            {
                props.Headers ??= new Dictionary<string, object>();
                props.Headers[key] = value;
            }
            catch (Exception ex)
            {
                ///_logger.LogError(ex, "Failed to inject trace context.");
            }
        }
    }
}