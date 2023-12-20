using static MaiLib.ChartEnum;
namespace MaiLib;

public interface ICodeBlock
{
    public string Compose(ChartVersion chartVersion);

    public class ComponentMissingException : Exception
    {
        public ComponentMissingException(string codeBlock, string missedComponents) : base(String.Format("CODE BLOCK {0} MISSED FOLLOWING COMPONENTS: {1}",codeBlock,missedComponents)){}
    }

    public class ExcessiveComponentsException : Exception
    {
        public ExcessiveComponentsException(string codeBlock, string unexpectedComponents) : base(String.Format("CODE BLOCK {0} WAS PROVIDED WITH FOLLOWING COMPONENTS MORE THAN EXPECTED: {1}",codeBlock,unexpectedComponents)){}
    }

    public class UnexpectedStringSuppliedException : Exception
    {
        public UnexpectedStringSuppliedException(string codeBlock, string expectedString, string actualString) : base(
            String.Format("CODE BLOCK {0} WAS SUPPLIED WITH UNEXPECTED STRING. EXPECTED: {1}; ACTUAL: {2}", codeBlock,
                expectedString, actualString))
        {
        }
    }
}