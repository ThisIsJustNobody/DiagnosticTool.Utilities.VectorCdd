using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.XmlParser;

namespace DiagnosticTool.Utilities.VectorCdd.TestHelpers;

/// <summary>
/// 测试数据加载器，集中管理 .cdd 文件的加载和缓存
/// </summary>
public static class CddTestData
{
    private static readonly Lazy<CanDela> _flr3 = new(() => LoadCdd("FLR3.cdd"));
    private static readonly Lazy<CanDela> _sweet500 = new(() => LoadCdd("RNA_SWEET500_new.cdd"));
    private static readonly Lazy<CanDela> _hkmc = new(() => LoadCdd("HKMC_FC_4_8.91.cdd"));
    private static readonly Lazy<CanDela> _vfRadar = new(() => LoadCdd("VF_Radar_V2.3.cdd"));
    private static readonly Lazy<CanDela> _zeekr = new(() => LoadCdd("Zeekr_PECU.cdd"));
    private static readonly Lazy<CanDela> _pr58307 = new(() => LoadCdd("PR58307_SAIC_MRGen21_SigmaA11.cdd"));

    public static CanDela Flr3 => _flr3.Value;
    public static CanDela Sweet500 => _sweet500.Value;
    public static CanDela Hkmc => _hkmc.Value;
    public static CanDela VfRadar => _vfRadar.Value;
    public static CanDela Zeekr => _zeekr.Value;
    public static CanDela Pr58307 => _pr58307.Value;

    /// <summary>
    /// 获取所有已加载的 .cdd 文件
    /// </summary>
    public static IEnumerable<(string Name, CanDela Candela)> All
    {
        get
        {
            yield return ("FLR3", Flr3);
            yield return ("RNA_SWEET500", Sweet500);
            yield return ("HKMC", Hkmc);
            yield return ("VF_Radar", VfRadar);
            yield return ("Zeekr", Zeekr);
            yield return ("PR58307", Pr58307);
        }
    }

    /// <summary>
    /// 获取 Resource 目录路径
    /// </summary>
    public static string ResourceDir => Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "ref"));

    private static CanDela LoadCdd(string filename)
    {
        var filepath = Path.Combine(ResourceDir, filename);
        if (!File.Exists(filepath))
            throw new FileNotFoundException($"找不到 .cdd 测试文件: {filepath}");

        var xml = File.ReadAllText(filepath);
        var candela = xml.DeserializeXmlToObject<CanDela>();
        candela.SetCanDelaReference();
        return candela;
    }
}
