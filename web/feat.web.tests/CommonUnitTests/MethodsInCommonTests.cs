using System.Reflection;
using feat.common;
using feat.common.Extensions;

namespace feat.web.tests.CommonUnitTests;

public class MethodsInCommonTests
{
    // Tests for feat.common methods used in web client
    private Assembly LoadFeatCommonAssembly()
    {
        try
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a =>
                    string.Equals(a.GetName().Name, "feat.common", StringComparison.OrdinalIgnoreCase));

            return asm != null ? asm : Assembly.Load("feat.common");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Damn it, failed to load assembly 'feat.common'", ex);
        }
    }

    [Fact]
    public void Load_FeatCommon_Assembly_ShouldReturn_FeatCommonAssembly()
    {
        var asm = LoadFeatCommonAssembly();
        Assert.NotNull(asm);
        Assert.Equal("feat.common", asm.GetName().Name, ignoreCase: true);
    }

    [Fact]
    public void Load_FeatCommon_Assembly_ShouldHaveExportedTypes()
    {
        var asm = LoadFeatCommonAssembly();
        var types = asm.GetExportedTypes();
        
        Assert.NotNull(types);
        Assert.NotEmpty(types);
        
        var stringExtensionType = types.FirstOrDefault(x => x.Name.Equals("StringExtensions", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(stringExtensionType);
    }

    [Fact]
    public void TruncateString_WithCutoff150_ReturnsTruncatedAndRemainder1()
    {
        var longText = string.Join(" ", Enumerable.Range(0, 100).Select(i => $"Word{i}"));
        var result = longText.TruncateString(out var remainder, 150);

        Assert.NotNull(result);
        Assert.NotNull(remainder);
        Assert.True(result.Length <= 150, "Result should be no longer than cutoff");
        Assert.NotEmpty(remainder);
    }
    
    [Fact]
    public void TruncateString_WithCutoff250_ReturnsTruncatedAndRemainder2()
    {
        const string originalText = $"This qualification is tailored for individuals currently employed in the construction industry who specialize in trowel occupations, such as bricklaying and masonry. It is ideal for professionals aiming to validate and enhance their skills in constructing complex masonry structures, including architectural and decorative elements. Candidates should have prior experience in trowel occupations and a solid understanding of construction site processes. The course is particularly beneficial for those seeking to formalize their expertise and gain recognition through a nationally accredited qualification. By completing this diploma, learners demonstrate their competence in various trowel-related tasks, including erecting masonry structures, setting out to form masonry structures, and developing good occupational working relationships. This qualification not only validates existing skills but also enhances career prospects by equipping trowel operatives with the necessary expertise to perform complex tasks effectively within the construction sector.";
        var result = originalText.TruncateString(out string remainder, 250);

        Assert.NotNull(result);
        Assert.NotNull(remainder);
        Assert.NotEmpty(remainder);
        Assert.True(result.Length <= 250, "Result should be no longer than the cutoff length");
        Assert.True(result.Length + remainder.Length <= originalText.Length, "Result and remainder should be no longer than original text");
    }
    
    [Theory]
    [InlineData("Some text of value", "Some text of value")]
    [InlineData("", SharedStrings.NotProvided)]
    [InlineData(null, SharedStrings.NotProvided)]
    [InlineData("  ", "  ")]
    public void StringExtentions_ValueOrNotProvided_ReturnsString(string? textValue, string expectedResult)
    {
        var result = textValue.ValueOrNotProvided();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(expectedResult, result);
    }
}