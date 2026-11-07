using System.Reflection;
using AI.Client.API.AccountUsers;

namespace AI.Client.UnitTests;

public class AccessAreasTests
{
    [Fact]
    public void EveryAreaConstant_All_ContainsIt()
    {
        var constants = typeof(AccessAreas).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral).Select(f => (string)f.GetRawConstantValue()!);
        Assert.Equal(constants.OrderBy(x => x), AccessAreas.All.OrderBy(x => x));
    }
}
