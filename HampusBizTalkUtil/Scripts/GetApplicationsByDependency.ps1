param(
    [string]$connectionString,
    [string]$dependency,
    [string]$scriptFolder
)

$Applications = & ($scriptFolder + "\GetDependencies.ps1") $connectionString $true

$references = @()

foreach ($application in $Applications)
{
    if ($application.Name -eq $dependency)
    {
        foreach ($ref in $application.References)
        {
            if (-not $references.Contains($ref.Name))
            {
                $references += $ref.name
                Write-Host $ref.Name
            }
        }
    }
}