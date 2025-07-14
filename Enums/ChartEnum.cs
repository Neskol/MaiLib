using System.ComponentModel.Design;

namespace MaiLib;

public class ChartEnum
{
    public enum ChartType
    {
        Standard,
        DX
    }

    public enum ChartVersion
    {
        Debug,
        Simai,
        SimaiFes,
        Ma2_103,
        Ma2_104,
        Ma2_105
    }
}