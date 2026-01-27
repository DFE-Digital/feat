using System.ComponentModel;

namespace feat.ingestion.Models.DU.Enums;

public enum Availability
{
	[Description("Not available")] 
	NotAvailable = 0,
	Optional = 1,
	Compulsory = 2,
}