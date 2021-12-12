using PsUtil.Cpu;
using System.Text.Json;

Console.WriteLine("Hello, World!");

var cpuInfoLinux = new CpuLinux();
// var info = cpuInfoLinux.Info();
// Console.WriteLine(info.Count());
// info.ForEach(item =>
// {
//     Console.WriteLine(JsonSerializer.Serialize(item));
// });

var counts =  cpuInfoLinux.Counts(false);
Console.WriteLine(counts);