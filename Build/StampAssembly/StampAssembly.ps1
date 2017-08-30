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
	$filePath = [System.IO.Path]::GetFullPath($filePath)
	$directory = [System.IO.Path]::GetDirectoryName($filePath)
	$fileName = [System.IO.Path]::GetFileName($filePath)
	$originalWorkingDirectory = Get-Location
	Set-Location $directory

	# Checkout the latest version of the file so that we know what we're updating.
	# Note that this does mean that you'll want to commit any changes made to this file before building.
	git checkout --force $fileName

	Set-Location $originalWorkingDirectory

	$content = [System.IO.File]::ReadAllText($filePath)
	$version = '"{0}.{1}.{2}"' -f $majorNumber, $minorNumber, $buildNumber 
	$content = $content.Replace('"9999.9999.9999.9999"', $version)
	$content = $content.Replace('<COMMITID>', $commitId)
	[System.IO.File]::WriteAllText($filePath, $content)
}

$major = 1
$minor = 2
$build = 3
$commit = "abcd-efgh-ijklmnop-qrs-tuv-wxyz"

Get-ChildItem (Join-Path (Get-Script-Directory) "..\..") -Recurse -Filter AssemblyInfo.cs | 
Foreach-Object {
	(Write-Assembly-Info $_.FullName $major $minor $build $commit)
}