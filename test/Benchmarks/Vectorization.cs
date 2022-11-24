using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

public class Vectorization
{
    private static Random _random = new(0);
    private static int[] _numbers = Enumerable.Range(0, 80000).Select(_ => _random.Next(-1000, 1000)).ToArray();
    private static int _expected = _numbers.Sum();
    private static int[] _temp = new int[8];
    
    [Benchmark]
    public int LoopAddNonVector()
    {
        var result = 0;
        foreach (var number in _numbers)
            result += number;

        if (result != _expected)
            throw new Exception();
        return result;
    }

    [Benchmark]
    public int LoopAddNonVectorBy8()
    {
        _temp[0] =
            _temp[1] =
                _temp[2] =
                    _temp[3] =
                        _temp[4] =
                            _temp[5] =
                                _temp[6] =
                                    _temp[7] = 0;

        for (var i = 0; i < _numbers.Length; i += 8)
        {
            _temp[0] += _numbers[i + 0];
            _temp[1] += _numbers[i + 1];
            _temp[2] += _numbers[i + 2];
            _temp[3] += _numbers[i + 3];
            _temp[4] += _numbers[i + 4];
            _temp[5] += _numbers[i + 5];
            _temp[6] += _numbers[i + 6];
            _temp[7] += _numbers[i + 7];
        }

        var sum = _temp[0] + _temp[1] + _temp[2] + _temp[3] + _temp[4] + _temp[5] + _temp[6] + _temp[7];
        
        if (sum != _expected)
            throw new Exception();
        return sum;
    }

    [Benchmark]
    public unsafe int LoopAddVector()
    {
        var vectorResult = Vector256.Create(0);
        fixed (int* numbers = &_numbers[0])
        {
            for (var i = 0; i < _numbers.Length; i += Vector256<int>.Count)
            {
                vectorResult += Vector256.Load(numbers + i);
            }
        }

        var result = Vector256.Sum(vectorResult);
        if (result != _expected)
            throw new Exception();
        return result;
    }
}
