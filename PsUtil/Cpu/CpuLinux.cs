namespace PsUtil.Cpu
{
    internal class CpuLinux
    {
        public List<InfoStat> Info()
        {
            var fileName = Path.Combine("/proc/cpuinfo");

            var ret = new List<InfoStat>();
            string processorName = "";

            var c = new InfoStat { Cpu = -1, Cores = 1 };
            if (File.Exists(fileName))
            {
                var lines = File.ReadLines(fileName);
                foreach (var line in lines)
                {
                    var fields = line?.Split(":");
                    if (fields == null || fields?.Length < 2)
                    {
                        continue;
                    }
                    var key = fields[0].Trim();
                    var value = fields[1].Trim();

                    switch (key)
                    {
                        case "Processor":
                            processorName = value;
                            break;
                        case "processor":
                            if (c.Cpu >= 0)
                            {
                                if (!FinishCpuInfo(c))
                                {
                                    return ret;
                                }
                                ret.Add(c);
                            }
                            c = new InfoStat { Cores = 1, ModelName = processorName };
                            c.Cpu = int.Parse(value);
                            break;
                        case "vendorId":
                        case "vendor_id":
                            c.VendorId = value;
                            break;
                        case "CPU implementer":
                            if (uint.TryParse(value, out var v))
                            {
                                switch (v)
                                {
                                    case 0x41:
                                        c.VendorId = "ARM";
                                        break;
                                    case 0x42:
                                        c.VendorId = "Broadcom";
                                        break;
                                    case 0x43:
                                        c.VendorId = "Cavium";
                                        break;
                                    case 0x44:
                                        c.VendorId = "DEC";
                                        break;
                                    case 0x46:
                                        c.VendorId = "Fujitsu";
                                        break;
                                    case 0x48:
                                        c.VendorId = "HiSilicon";
                                        break;
                                    case 0x49:
                                        c.VendorId = "Infineon";
                                        break;
                                    case 0x4d:
                                        c.VendorId = "Motorola/Freescale";
                                        break;
                                    case 0x4e:
                                        c.VendorId = "NVIDIA";
                                        break;
                                    case 0x50:
                                        c.VendorId = "APM";
                                        break;
                                    case 0x51:
                                        c.VendorId = "Qualcomm";
                                        break;
                                    case 0x56:
                                        c.VendorId = "Marvell";
                                        break;
                                    case 0x61:
                                        c.VendorId = "Apple";
                                        break;
                                    case 0x69:
                                        c.VendorId = "Intel";
                                        break;
                                    case 0xc0:
                                        c.VendorId = "Ampere";
                                        break;
                                }
                            }
                            break;
                        case "cpu family":
                            c.Family = value;
                            break;
                        case "model":
                        case "CPU part":
                            c.Model = value;
                            break;
                        case "model name":
                        case "cpu":
                            c.ModelName = value;
                            if (value.Contains("POWER8") || value.Contains("POWER7"))
                            {
                                c.Model = value.Split(" ")[0];
                                c.Family = "POWER";
                                c.VendorId = "IBM";
                            }
                            break;
                        case "stepping":
                        case "revision":
                        case "CPU revision":
                            var val = value;
                            if (key == "revision")
                            {
                                val = val.Split(".")[0];
                            }
                            if (int.TryParse(val, out var t))
                            {
                                c.Stepping = t;
                            }
                            break;
                        case "cpu MHz":
                        case "clock":
                            if (float.TryParse(value.Replace("MHz", ""), out var tClick))
                            {
                                c.Mhz = tClick;
                            }
                            break;
                        case "cache size":
                            if (int.TryParse(value.Replace(" KB", ""), out var tCache))
                            {
                                c.CacheSize = tCache;
                            }
                            break;
                        case "physical id":
                            c.PhysicalId = value;
                            break;
                        case "core id":
                            c.CoreId = value;
                            break;
                        case "flags":
                        case "Features":
                            c.Flags = value.Split(new char[] { ',', ' ' }).ToList();
                            break;
                        case "microcode":
                            c.Microcode = value;
                            break;
                    }

                }
                if (c.Cpu >= 0)
                {
                    if (!FinishCpuInfo(c))
                    {
                        return ret;
                    }
                    ret.Add(c);
                }
            }
            return ret;
        }

        private bool FinishCpuInfo(InfoStat c)
        {
            IEnumerable<string> lines;
            float value;
            string tempFile;
            if (c.CoreId == null || c.CoreId.Length == 0)
            {
                tempFile = SysCpuPath(c.Cpu, "topology/core_id");
                if (File.Exists(tempFile))
                {
                    lines = File.ReadLines(tempFile);
                    c.CoreId = lines.FirstOrDefault();
                }
            }

            tempFile = SysCpuPath(c.Cpu, "cpufreq/cpuinfo_max_freq");
            if (!File.Exists(tempFile))
            {
                return true;
            }
            lines = File.ReadLines(tempFile);
            if (lines.Count() == 0)
            {
                return true;
            }
            value = float.Parse(lines.First());
            c.Mhz = value / 1000f;
            if (c.Mhz > 9999)
            {
                c.Mhz = c.Mhz;
            }

            return true;
        }

        private string SysCpuPath(int cpu, string relPath)
        {
            return Path.Combine("/sys", $"devices/system/cpu/cpu{cpu}", relPath);
        }

        public int Counts(bool logical)
        {
            if (logical)
            {
                var ret = 0;
                var procCpuInfo = Path.Combine("/proc", "cpuinfo");
                if (File.Exists(procCpuInfo))
                {
                    var lines = File.ReadLines(procCpuInfo);
                    foreach (var line in lines)
                    {
                        var tLine = line.ToLower();
                        if (tLine.StartsWith("processor"))
                        {
                            var t = tLine.Split(":");
                            if (t.Count() >= 2 && int.TryParse(t[1], out var outValue))
                            {
                                ret++;
                            }
                        }
                    }
                }
                if (ret == 0)
                {
                    var procStat = Path.Combine("/proc", "stat");
                    if (File.Exists(procStat))
                    {
                        var lines = File.ReadLines(procStat);
                        foreach (var line in lines)
                        {
                            if (line?.Length >= 4 && line.StartsWith("cpu") && '0' <= line[3] && line[3] <= 9)
                            {
                                ret++;
                            }
                        }
                    }
                }
                return ret;
            }
            //physical cores
            var cpuBaseDir = Path.Combine("/sys", "devices/system/cpu");
            if (Directory.Exists(cpuBaseDir))
            {
                var dict = new Dictionary<string, bool>();
                var dirs = Directory.GetDirectories(cpuBaseDir, "cpu*");
                foreach (var endDir in new[] { "topology/core_cpus_list", "topology/thread_siblings_list" })
                {
                    foreach (var d in dirs)
                    {
                        if (File.Exists(Path.Combine(d, endDir)))
                        {
                            var lines = File.ReadLines(Path.Combine(d, endDir));
                            if (lines.Count() != 1)
                            {
                                continue;
                            }
                            dict[lines.First()] = true;
                        }
                    }
                    var ret = dict.Count;
                    if (ret != 0)
                    {
                        return ret;
                    }
                }
            }
            // https://github.com/giampaolo/psutil/blob/122174a10b75c9beebe15f6c07dcf3afbe3b120d/psutil/_pslinux.py#L631-L652
            var filename = "/proc/cpuinfo";
            if (File.Exists(filename))
            {
                var dict = new Dictionary<int, int>();
                var currentInfo = new Dictionary<string, int>();
                var lines = File.ReadLines(filename);
                foreach (var item in lines)
                {
                    var line = item.Trim().ToLower();
                    if (line == "")
                    {
                        if (currentInfo.ContainsKey("physical id") && currentInfo.ContainsKey("cpu cores"))
                        {
                            dict[currentInfo["physical id"]] = currentInfo["cpu cores"];
                        }
                        currentInfo = new Dictionary<string, int>();
                        continue;
                    }

                    var fields = line.Split(":");
                    if (fields.Length < 2)
                    {
                        continue;
                    }
                    fields[0] = fields[0].Trim();
                    if (fields[0] == "physical id" || fields[0] == "cpu cores")
                    {
                        if (int.TryParse(fields[1].Trim(), out var outValue))
                        {
                            currentInfo[fields[0]] = outValue;
                        }
                    }
                }
                var ret = 0;
                foreach (var v in dict.Values)
                {
                    ret += v;
                }
                return ret;
            }
            return 0;
        }
    }
}
