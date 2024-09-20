param(
    [string]$connectionString,
    [bool]$Return
)

Import-Module SqlServer

class ApplicationProperties {
    [string]$Name
    [string[]]$AssemblyNames
    [ApplicationProperties[]]$References
}

$Applications = @()

$connection = New-Object System.Data.SqlClient.SqlConnection
$connection.ConnectionString = $connectionString
$connection.Open()
$command = $connection.CreateCommand()

# GET ALL APPLICATIONS
$command.CommandText = "SELECT [nvcName] as ApplicationName FROM [BizTalkMgmtDb].[dbo].[bts_application]"

$SqlDataReader = $command.ExecuteReader()
while ($SqlDataReader.Read()) {
    $application = New-Object ApplicationProperties
    $application.Name = $SqlDataReader.Item("ApplicationName")
    $application.AssemblyNames = @()
    $application.References = @()

    $Applications += $application
}

$SqlDataReader.Close()



$command.CommandText =
"SELECT a.nvcName as AssemblyName, app.nvcName as ApplicationName
FROM [BizTalkMgmtDb].[dbo].[bts_assembly] a
JOIN [BizTalkMgmtDb].[dbo].[bts_application] app ON a.nApplicationID = app.nID"

$SqlDataReader = $command.ExecuteReader()

while ($SqlDataReader.Read()) {
    $applicationName = $SqlDataReader.Item("ApplicationName")

    $existingApplication = $Applications | Where-Object { $_.Name -eq $applicationName } | Select-Object -First 1

    if ($existingApplication)
    {
        $existingApplication.AssemblyNames += $SqlDataReader.Item("AssemblyName")
    }
    else
    {
        $application = New-Object ApplicationProperties
        $application.Name = $SqlDataReader.Item("ApplicationName")
        $application.AssemblyNames = @($SqlDataReader.Item("AssemblyName"))
        $application.References = @()

        $Applications += $application
    }
}

$SqlDataReader.Close()


$command.CommandText = 
"select app.nvcName as ApplicationName, refapp.nvcName AS ReferenceApplicationName
from bts_application_reference as appref
inner join bts_application AS app ON app.nID = appref.nApplicationID
inner join bts_application AS refapp ON refapp.nID = appref.nReferencedApplicationID"

$SqlDataReader = $command.ExecuteReader()

while ($SqlDataReader.Read()) {
    $appName = $SqlDataReader.Item("ReferenceApplicationName")
    $reference = $SqlDataReader.Item("ApplicationName")

    $dependencyApplication = $Applications | Where-Object { $_.Name -eq $appName } | Select-Object -First 1
    $referenceApplication = $Applications | Where-Object { $_.Name -eq $reference } | Select-Object -First 1

    if ($referenceApplication)
    {
        $dependencyApplication.References += $referenceApplication
    }
    else
    {
        $application = New-Object ApplicationProperties
        $application.Name = $reference
        $application.AssemblyNames = @()
        $application.References = @()

        $dependencyApplication.References += $application
    }
}

$SqlDataReader.Close()
$connection.Close()


# Find DLLS and get their references
$basePath = "C:\Windows\Microsoft.NET\assembly\GAC_MSIL"

foreach ($application in $Applications)
{
    foreach ($assemblyName in $application.AssemblyNames)
    {
        $path = $basePath + "\" + $assemblyName
    
        $path += "\" + (Get-ChildItem -Path $path)[0]
        $path += "\" + (Get-ChildItem -Path $path -File)[0]

        # Load the assembly
        $assembly = [System.Reflection.Assembly]::LoadFile($path)
        # Get the referenced assemblies
        $referencedAssemblies = $assembly.GetReferencedAssemblies()
        # Print the referenced assemblies
        foreach ($referencedAssembly in $referencedAssemblies) {
            $ref = $referencedAssembly.Name

            $referenceApplication = $null
            foreach ($app in $Applications)
            {
                foreach ($asm in $app.AssemblyNames)
                {
                    if ($asm -eq $ref)
                    {
                        $referenceApplication = $app
                        break
                    }
                }
            }

            if ($referenceApplication)
            {
                $referenceApplication.References += $application
            }

        }
    }
}

if ($Return)
{
    return $Applications
}

foreach ($application in $Applications)
{
    if ($application.References.Count -gt 0)
    {
        Write-Host $application.Name
    }
}