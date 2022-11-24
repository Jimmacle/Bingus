using BenchmarkDotNet.Running;
using Benchmarks;

/*
 * no loop
 * |            Method |     Mean |    Error |   StdDev | Allocated |
 * |------------------ |---------:|---------:|---------:|----------:|
 * | MMulGenericDouble | 25.93 ns | 0.270 ns | 0.225 ns |         - |
 */

BenchmarkRunner.Run(typeof(Misc));