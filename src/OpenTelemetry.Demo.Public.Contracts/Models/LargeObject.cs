using System;

namespace OpenTelemetry.Demo.Public.Contracts.Models
{
    public class LargeObject
    {
        public readonly Guid Id;
        public readonly double[] Doubles;

        public LargeObject()
        {
            Id = Guid.NewGuid();
            Doubles = new double[1024];
            Random r = new Random();
                
            for (int i = 0; i < Doubles.Length; i++)
            {
                Doubles[i] = r.NextDouble();
            }
        }
    }
}