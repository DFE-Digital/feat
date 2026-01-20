using feat.common;
using feat.common.Extensions;

namespace feat.web.tests;

public class StringExtensionsTests
{
    [Fact]
    public void TruncateString_WithCutoff150_ReturnsTruncatedAndRemainder()
    {
        var longText = string.Join(" ", Enumerable.Range(0, 100).Select(i => $"Word{i}"));

        var result = longText.TruncateString(out var remainder, 150);

        Assert.True(result.Length <= 150);
        Assert.NotEmpty(remainder);
    }

    [Theory]
    [InlineData("Some text of value", "Some text of value")]
    [InlineData("", SharedStrings.NotProvided)]
    [InlineData(null, SharedStrings.NotProvided)]
    [InlineData("  ", "  ")]
    public void ValueOrNotProvided_ReturnsExpected(string? input, string expected)
    {
        Assert.Equal(expected, input.ValueOrNotProvided());
    }
}