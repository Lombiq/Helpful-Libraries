using Lombiq.HelpfulLibraries.Common.Utilities;
using Shouldly;
using Xunit;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.Utilities;

public class HtmlHelperTests
{
    [Theory]
    [InlineData("<div>Hello World</div>", "Hello World")]
    [InlineData("\n\n     <div> \n    Hello  World\n\n Good Bye! </div>  ", "Hello  World\n Good Bye!")]
    public void SingleElementTextContentShouldHaveCorrectSpacing(string input, string expectedResult) =>
        HtmlHelper.ConvertToPlainText(input).ShouldBe(expectedResult);

    [Theory]
    [InlineData("<div>Hello World</div>\n<div>Good <div>Bye!</div></div>", "Hello World\nGood Bye!")]
    [InlineData("<div>A</div><div>B</div><div>C</div><div>D</div>", "ABCD")]
    public void MultipleElementsShouldDisplayCorrectly(string input, string expectedResult) =>
        HtmlHelper.ConvertToPlainText(input).ShouldBe(expectedResult);
}
