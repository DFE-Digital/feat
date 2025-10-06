using Azure.Search.Documents.Models;

namespace feat.api.Models;

public class Facet
{
    public Facet(string name, IList<FacetResult> results)
    {
        Name = name;
        
        foreach (var facetResult in results)
        {
            var values = facetResult.AsValueFacetResult<string>();
            
            if (values == null)
            {
                continue;
            }
            Values.Add(values.Value, values.Count);
        }
    }
    
    public string Name { get; set; }

    public Dictionary<string, long> Values { get; set; } = new();
}