using System;
namespace GrantAccessCLI
{
	public class ContextModel
	{
		public Uri server { get; set; }
		public string token { get; set; }
		public bool insecure { get; set; }

	}
}

