#############################################################################################
### Attempts to find the location of MSBuild
#############################################################################################

function Get-Script-Directory
{    
  #Get the script's path, and then use Spit-Path to get the directory.
  #https://stackoverflow.com/questions/801967/how-can-i-find-the-source-path-of-an-executing-script
  return Split-Path $script:MyInvocation.MyCommand.Path
}

function Write-Assembly-Info($filePath, $majorNumber, $minorNumber, $buildNumber, $commitId){
	$content = [System.IO.File]::ReadAllText($filePath)
	$version = '"{0}.{1}.{2}"' -f $majorNumber, $minorNumber, $buildNumber 
	$content = $content.Replace('"9999.9999.9999.9999"', $version)
	$content = $content.Replace('<COMMITID>', $commitId)
	[System.IO.File]::WriteAllText($filePath, $content)
}

$file = (Join-Path (Get-Script-Directory) "..\..\Code\CapuchinSync.Hash\Properties\AssemblyInfo.cs")
$major = 1
$minor = 2
$build = 3
$commit = "abcd-efgh-ijklmnop-qrs-tuv-wxyz"

(Write-Assembly-Info $file $major $minor $build $commit)