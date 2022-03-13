using System;
namespace GrantAccessCLI
{
	public class ManifestModel
	{
		public string apiVersion { get; set; }
		public string kind { get; set; }
		public dynamic definition { get; set; }
	}
}

