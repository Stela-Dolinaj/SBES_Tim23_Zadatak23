using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manager
{
	public class Formatter
	{
		/// <summary>
		/// Returns username based on the Logon Name. 
		/// </summary>
		/// <param name="logName"> Logon name can be formatted either as a UPN (<username>@<domain_name>) or a SPN (<domain_name>\<username>) or for certificates CN=username </param>
		/// <returns> username </returns>
		public static string ParseName(string logName)
		{
			string[] parts = new string[] { };

			if (logName.Contains("@"))
			{
				///UPN format
				parts = logName.Split('@');
				return parts[0];
			}
			else if (logName.Contains("\\"))
			{
				/// SPN format
				parts = logName.Split('\\');
				return parts[1];
			}
            else if (logName.Contains("CN"))
            {
                // sertifikati, name je formiran kao CN=imeKorisnika;
                int startIndex = logName.IndexOf("=") + 1;
                int endIndex = logName.IndexOf(";");
                string s = logName.Substring(startIndex, endIndex - startIndex);
                return s;
            }
            else
			{
				return logName;
			}
		}
	}
}
