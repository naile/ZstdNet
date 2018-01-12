using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ZstdNet.Benchmarks
{
    [MemoryDiagnoser]
    public class CompressionBenchmarks
    {
        byte[] Test = new byte[1024];
        byte[] Dest = new byte[1024 * 2];

        public CompressionBenchmarks()
        {
            var r = new Random(0);
            r.NextBytes(Test);
        }

        Compressor ZNetCompressor = new Compressor(new CompressionOptions(1));

        [Benchmark]
        public void Compress1KBRandom()
        {
            ZNetCompressor.Wrap(Test, Dest, 0);
        }
    }
}
