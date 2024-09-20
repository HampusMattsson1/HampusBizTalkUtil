#get-service BTS* | foreach-object -process {restart-service $_.Name}

$services = get-service BTS*
$total = $services.Count
Write-Host "0/$total"
$i = 0
$services | foreach-object -process {
    $i++
    Write-Host "$i/$total"
    restart-service $_.Name
}