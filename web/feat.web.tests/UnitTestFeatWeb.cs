using System.Reflection;

namespace feat.web.tests;

public class UnitTestFeatWeb
{
    private Assembly LoadFeatWebAssembly()
    {
        try
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => string.Equals(a.GetName().Name, "feat.web", StringComparison.OrdinalIgnoreCase));

            return asm != null ? asm : Assembly.Load("feat.web");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to load assembly 'feat.web'", ex);
        }
    }

    [Fact]
    public void AssemblyLoads_ProgramTypeExists()
    {
        var asm = LoadFeatWebAssembly();
        
        Assert.NotNull(asm);
        
        var program = asm.GetTypes()
            .FirstOrDefault(t => string.Equals(t.Name, "Program", StringComparison.OrdinalIgnoreCase));
        
        Assert.NotNull(program);
    }
    
}