### Benchmarks

BenchmarkDotNet v0.15.2, Linux CachyOS
AMD Ryzen 9 9950X3D 5.76GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.414
  [Host]     : .NET 8.0.20 (8.0.2025.41914), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.20 (8.0.2025.41914), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


#### Before

| Method                  | Mean     | Error     | StdDev    | Rank | Gen0     | Gen1     | Gen2     | Allocated |
|------------------------ |---------:|----------:|----------:|-----:|---------:|---------:|---------:|----------:|
| GetCountriesWithFilters | 3.705 ms | 0.0704 ms | 0.0723 ms |    2 | 320.3125 | 312.5000 | 242.1875 |    5.1 MB |
| GetCountries            | 3.496 ms | 0.0675 ms | 0.0663 ms |    1 | 328.1250 | 324.2188 | 250.0000 |   5.09 MB |

// * Hints *
Outliers
  DateVerificationRegexBenchmarks.GetCountriesWithFilters: Default -> 1 outlier  was  removed (4.03 ms)


#### After

| Method                  | Mean      | Error    | StdDev   | Rank | Gen0    | Gen1    | Allocated |
|------------------------ |----------:|---------:|---------:|-----:|--------:|--------:|----------:|
| GetCountries            | 193.43 μs | 1.398 μs | 1.239 μs |    2 | 17.5781 | 10.2539 | 866.03 KB |
| GetCountriesWithFilters |  42.78 μs | 0.554 μs | 0.518 μs |    1 |  3.7842 |  1.1597 | 185.52 KB |

// * Hints *
Outliers
  DateVerificationRegexBenchmarks.GetCountries: Default -> 1 outlier  was  removed (197.53 μs)


### Conclusion

### Before → After
- **GetCountries**: 3.496 ms → 193.43 μs (**18x faster**, **5.8x less memory**)
- **GetCountriesWithFilters**: 3.705 ms → 42.78 μs (**87x faster**, **27.5 less memory**)

