function Initialize-SharedAssemblyInfo {
  Param(
    [Parameter(Mandatory=$False)]
    [Switch]$RemoveComments = $False
    )
  process {
    $crlf = [System.Environment]::NewLine;

    $current = $PSScriptRoot;
    while ((-Not (Test-Path "$current\Sources")) -Or ($current.Length -lt 4)) {
      $current = (Get-Item $current).Parent.FullName;
    }

    $solutions = "$current";

    if (-Not (Test-Path $solutions)) {
      throw "Unable to find solutions directory.";
    }

    Write-Host -ForegroundColor Green "Found Solution at $solutions";

    $current = $PSScriptRoot;
    while ((-Not (Test-Path "$current\AssemblyInfo")) -Or ($current.Length -lt 4)) {
      $current = (Get-Item $current).Parent.FullName;
    }

    $sharedAssemblyInfoDir = "$current\AssemblyInfo";

    if (-Not (Test-Path $sharedAssemblyInfoDir)) {
      throw "Unable to find solution's AssemblyInfo directory.";
    }

    Write-Host -ForegroundColor Green "Found AssemblyInfo Directory at $sharedAssemblyInfoDir";

    $sharedAssemblyInfo = Join-Path $sharedAssemblyInfoDir "SharedAssemblyInfo.cs";

    if (-Not (Test-Path $sharedAssemblyInfo)) {
      Write-Host -ForegroundColor Yellow "SharedAssemblyInfo.cs not found, supply some details and we can set it up for you...";
      Set-SharedAssemblyInfo -SharedAssemblyInfoFile $sharedAssemblyInfo;
    }

    $assemblyInfoFragment = "<Compile Include=`"Properties\AssemblyInfo.cs`" />";
    $sharedAssemblyInfoFragment = "`t`t<Compile Include=`"{0}`">" + $crlf + "`t`t`t<Link>Properties\SharedAssemblyInfo.cs</Link>" + $crlf + "`t`t</Compile>";

    $sharedAttributes = Get-Content $sharedAssemblyInfo `
                | Where-Object { -not [String]::IsNullOrWhiteSpace($_) } `
                | Where-Object { -not $_.StartsWith("//") } `
                | Where-Object { -not $_.StartsWith("using") } `
                | ForEach-Object { $_.Split((' ', '('))[1] };

    if (Test-Path $sharedAssemblyInfo) {
      Write-Host -ForegroundColor Green "Found SharedAssemblyInfo.cs in $sharedAssemblyInfoDir";
    } else {
      throw "Could not find SharedAssemblyInfo.cs... exiting.";
    }

    $projects = (Get-ChildItem -Recurse $solutions *.csproj);

    foreach ($project in $projects) {
      $projectXml = [xml](Get-Content $project.VersionInfo.FileName);
      $assemblyInfo = ($projectXml.Project.ItemGroup `
                        | Where-Object { `
                          ($_.GetType().Name -eq "XmlElement") `
                          -And (($_.ChildNodes | Select-Object -Last 1).Name -eq "Compile")}).Compile.Include `
                        | Where-Object { $_.EndsWith("AssemblyInfo.cs") };

      if ($assemblyInfo | Where-Object { $_.EndsWith("SharedAssemblyInfo.cs")}) {
        Write-Host -ForegroundColor Green "  $project already contains SharedAssemblyInfo.cs";
      } else { 
        Write-Host -ForegroundColor Yellow -NoNewLine "  $project dosn't contain SharedAssemblyInfo.cs";

        Push-Location ($project.DirectoryName);
        $relativeSharedAssemblyPath = Resolve-Path -Relative $sharedAssemblyInfo; 
        Pop-Location;
        $projectFile = (Get-Content $project.VersionInfo.FileName);
        $projectFile = $projectFile.Replace($assemblyInfoFragment, $assemblyInfoFragment + $crlf + ($sharedAssemblyInfoFragment -f $relativeSharedAssemblyPath));
        Set-Content -Path $project.VersionInfo.FileName $projectFile;

        Write-Host -ForegroundColor Green "... added.";
      }

      $assemblyInfoPath = (Join-Path $project.DirectoryName "properties/AssemblyInfo.cs");
      $assemblyInfo = Get-Content $assemblyInfoPath;
      $newAssemblyInfo = New-Object System.Text.StringBuilder ;

      Write-Host -ForegroundColor White "    Inspecting AssemblyInfo.cs in $project"
      $previousLine = "";

      foreach ($line in $assemblyInfo) {
        $found = $False;

        foreach ($attribute in $sharedAttributes) {
          if ($line.StartsWith("[assembly: $attribute")) {
            Write-Host -ForegroundColor Yellow "      Removing shared attribute: $attribute"
            $found = $True;
          } 
        }

        if (-not $found) {
          if ($RemoveComments -and $line.StartsWith("//")) {
            continue;
          }

          if ([String]::IsNullOrWhiteSpace($line) -and [String]::IsNullOrWhiteSpace($previousLine)) {
            continue; 
          }

          $newAssemblyInfo.AppendLine($line) | Out-Null;
        }

        $previousLine = $line;
      }

      Set-Content -Path $assemblyInfoPath -Value $newAssemblyInfo.ToString().Trim() -Encoding UTF8;
    }
  }
}

function Set-SharedAssemblyInfo {
  Param(
    [parameter(Mandatory=$true)]
    $SharedAssemblyInfoFile,    
    [parameter(Mandatory=$true)]
    $Company = "Company",
    [parameter(Mandatory=$true)]
    $Product = "Product",
    [parameter(Mandatory=$true)]
    $Year = (Get-Date).Year,
    [parameter(Mandatory=$true)]
    $Trademark = ""
        )

  process {
    $template = "using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

/// ««««««««««««««««««««««««««««««««««««««««««««««««««««««««««««««««««««««
/// 
///                           UNIFIED VERSIONING
/// 
/// ----------------------------------------------------------------------
///                           ASSEMBLY METADATA
/// ----------------------------------------------------------------------
[assembly: AssemblyProduct(`"$Product`")]
[assembly: AssemblyCompany(`"$Company`")]
[assembly: AssemblyCopyright(`"Copyright © $Company $Year, All Rights Reserved`")]
[assembly: AssemblyTrademark(`"$Trademark`")]
[assembly: AssemblyConfiguration(`"`")]
/// ----------------------------------------------------------------------
///                            VERSIONING NOTES
/// ----------------------------------------------------------------------
/// This Assembly Versioning uses two starndards depending on a Flag called UseSemanticVersion
/// This flag is set at the Versioning XML file in the AssemblyInfo directory of every solution
/// On that same file, the verion is set for the entire product
/// 
/// For local environments, the Flag is set to False by default, and hence uses the Microsost versioning standards
/// 
/// For build environments, i.e.: CI/CD servers, the Flag is set to True, and hece Semantic Versioning is used in accordance with http://semver.org/ (Semantic Versioning Specification)
/// ----------------------------------------------------------------------
/// Further reference: http://stackoverflow.com/questions/64602/what-are-differences-between-assemblyversion-assemblyfileversion-and-assemblyin
/// Further reference: http://www.danielfortunov.com/software/%24daniel_fortunovs_adventures_in_software_development/2009/03/03/assembly_versioning_in_net
/// ----------------------------------------------------------------------
///                           ASSEMBLY VERSIONS
/// ----------------------------------------------------------------------
/// Description:    Used by other assemblies for reference
/// SemVer Format:  major.minor
/// MS Format:      major.minor
/// Notes:          If this number changes, other assemblies have to update their references to your assembly
/// SemVer Example: `"2.1`"
/// MS Example:     `"2.1`"
[assembly: AssemblyVersion(`"1.0`")]
/// Description:    Used for deployments and generated by the build
/// SemVer Format:  major.minor.patch.build
/// MS Format:      major.minor.build_date.revision
/// Semver Example: `"2.1.15.347`"
/// MS Example:     `"2.1.yyMMdd.023`"
[assembly: AssemblyFileVersion(`"1.0.0`")]
/// Description:    The Product version of the assembly.
///                 This is the version you would use when talking to customers or for display on your website
/// SemVer Format:  major.minor [pre-release identifiers]
/// MS Format:      major.minor.patch [pre-release identifiers]
/// Notes:          This version can be a string, like '1.0 Release Candidate'
///                 [pre-release identifiers] = Development Stage
///                     Alpha           aka: CTP (Community Technology Preview) » Very much like `"Show and Tell`". Features are present to varying degrees and customer can get an idea of where the release is going
///                     Beta            Features are mostly implemented but still have rough edges. Quality is fair at this point. The higher number beta, the higher the quality
///                     RC              aka: Release Candidate » Product believes it's ready to ship. One last chance for customers to provide feedback and find major blocking issues
///                     RTM             aka: Release to Manufactoring » Product is complete and ready to be shipped to customers
///                     RTW             aka: Release to Web » Product is complete and ready to be published to customers
/// SemVer Example: `"2.1 RC1`"
/// MS Example:     `"2.1.15 RC1`"
[assembly: AssemblyInformationalVersion(`"1.0.0`")]
/// »»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»"

  Set-Content -Path $SharedAssemblyInfoFile -Value $template -Encoding UTF8;
  }
}

Initialize-SharedAssemblyInfo -RemoveComments