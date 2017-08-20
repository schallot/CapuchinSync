#############################################################################################
### Attempts to find the location of MSBuild
#############################################################################################

function Get-Script-Directory
{    
  #Get the script's path, and then use Spit-Path to get the directory.
  #https://stackoverflow.com/questions/801967/how-can-i-find-the-source-path-of-an-executing-script
  return Split-Path $script:MyInvocation.MyCommand.Path
}

$Source = @" 
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
namespace MsBuildLocator
{
	// Grab location of msbuild based on https://stackoverflow.com/questions/43078438/building-msbuild-15-project-programmatically
    public class MsBuildInstallation
    {
        public double Version;
        public string Path;
        public override string ToString()
        {
            return string.Format("Version: {0}, Path: {1}", Version, Path);
        }

        public static MsBuildInstallation[] Get()
        {
            var defaultResult = new MsBuildInstallation[] { };
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\MSBuild\\ToolsVersions");
            if (key == null) return defaultResult;
            List<MsBuildInstallation> list = new List<MsBuildInstallation>();
            foreach (var v in key.GetSubKeyNames())
            {
                double version;
                double.TryParse(v, out version);
                RegistryKey productKey = key.OpenSubKey(v);
                string path = "";
                if (productKey != null)
                {
                    var pathValue = productKey.GetValue("MSBuildToolsPath");
                    if (pathValue != null)
                    {
                        path = pathValue.ToString();
						path = System.IO.Path.Combine(path, "msbuild.exe");
                    }
                }
                list.Add(new MsBuildInstallation {Path = path, Version = version});
            }
            return list.ToArray();
        }
        
        public static MsBuildInstallation GetNewestInstalledVersion()
        {
            var locations = Get();
            if (locations == null) return null;
            return locations.OrderByDescending(x => x.Version).FirstOrDefault();
        }
    }   
}
"@ 

Add-Type -TypeDefinition $Source -Language CSharp  

./Nuget.exe restore ..\Code\CapuchinSync.sln

$msbuild = [MsBuildLocator.MsBuildInstallation]::GetNewestInstalledVersion().Path
$solution = (Join-Path (Get-Script-Directory) "\..\Code\CapuchinSync.sln")

& "$msbuild" "$solution"