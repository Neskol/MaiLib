using System.ComponentModel.Design;

namespace MaiLib;

public class ChartEnum
{
    public enum ChartType{
        Standard,
        StandardUtage,
        DX,
        DXFestival,
        DXUtage
    }

    public enum ChartVersion{
        Simai,
        SimaiFes,
        Ma2_103,
        Ma2_104
    }
}
