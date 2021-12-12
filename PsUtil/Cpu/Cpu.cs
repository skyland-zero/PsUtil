namespace PsUtil.Cpu
{
    internal class Cpu
    {
    }

    public class InfoStat
    {
        /// <summary>
        /// 
        /// </summary>
        public int Cpu { get; set; }

        /// <summary>
        /// CPU制造商
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// CPU制造商
        /// </summary>
        public string Family { get; set; }

        /// <summary>
        /// CPU制造商
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        ///CPU属于制作更新版本
        /// </summary>
        public int Stepping { get; set; }

        /// <summary>
        /// 单个CPU的标号
        /// </summary>
        public string PhysicalId { get; set; }

        /// <summary>
        /// 当前物理核在其所处CPU中的编号，这个编号不一定连续
        /// </summary>
        public string? CoreId { get; set; }

        /// <summary>
        /// Cores
        /// </summary>
        public int Cores { get; set; }

        /// <summary>
        /// CPU属于的名字及其编号、标称主频
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// CPU的实际使用主频
        /// </summary>
        public float Mhz { get; set; }

        /// <summary>
        /// CPU二级缓存大小
        /// </summary>
        public int CacheSize { get; set; }

        /// <summary>
        /// 当前CPU支持的功能
        /// </summary>
        public List<string> Flags { get; set; }

        /// <summary>
        /// 微码
        /// </summary>
        public string Microcode { get; set; }
    }
}
