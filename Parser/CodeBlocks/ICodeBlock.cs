using static MaiLib.ChartEnum;
namespace MaiLib;

public interface ICodeBlock
{
    public string Compose(ChartVersion chartVersion);

    public class ComponentMissingException : Exception
    {
        public ComponentMissingException(string codeBlock, string missedComponents) : base(String.Format("CODE BLOCK {0} MISSED FOLLOWING COMPONENTS: {1}",codeBlock,missedComponents)){}
    }
}