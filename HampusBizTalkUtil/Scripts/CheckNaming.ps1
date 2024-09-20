
param (
    [string]$Miljo,
    [string]$IPNamn
)

Function CheckPortNaming()
{
    #Vanlig powershell
    PARAM (
        $Miljo,
        $IPNamn
        )
    process {
        [void] [System.reflection.Assembly]::LoadWithPartialName("Microsoft.BizTalk.ExplorerOM")
        
        $Catalog = New-Object Microsoft.BizTalk.ExplorerOM.BtsCatalogExplorer
        $Catalog.ConnectionString = "SERVER=$($Miljo);DATABASE=BizTalkMgmtDb;Integrated Security=SSPI"

	$ErrorActionPreference="silentlycontinue"
	trap { "Exception encountered:`r`n"; $_; "`r`nDiscarding Changes.`r`n";$Catalog.DiscardChanges(); }
	
        
         foreach($app in $Catalog.Applications)
        {            
                if($app.Name -eq  $IPNamn)
                {
                   $appName = $app.name
                   Write-Output "------------------------------------------------------------------------------------------------------------------------------"
                   Write-Output "Checking names for $appName"
                   CheckAppName $appName
                   foreach($ReceivePort in $app.ReceivePorts)
                   {
                     $ReceivePortName = $ReceivePort.Name              
                     CheckRecivePortName $ReceivePortName
                     CheckDescription $ReceivePort
                     foreach($ReceiveLocation in $ReceivePort.ReceiveLocations){
                        CheckDescription $ReceiveLocation
                     }

                   }
                   foreach($SendPort in $app.SendPorts)
                   {
                     $SendPortName = $SendPort.Name
                     CheckSendPortName $SendPortName $SendPort
                     CheckDescription $SendPort
                   }

                   Write-Output "Checking names for $appName is done, see above for information"
                   Write-Output "------------------------------------------------------------------------------------------------------------------------------"
                }
                
             }                             
         }        
        
    } #end of function
       

 function CheckRecivePortName {
    param (
        [string]$inputString
    )
    # Define the regular expression pattern for the desired format
    $pattern = "^IP(?<digits>\d{4})Receive(?<rest>\w+)$"

    # Use the -match operator to check if the input string matches the pattern
    if ($inputString -match $pattern) {
        Write-Output "The recivePort '$inputString' follows the specified format."
    } else {
        Write-Output "The recivePort '$inputString' does not follow the specified format."
        if ($inputString -notmatch "^IP") {
            Write-Output "The first two characters should be 'IP'."
        } elseif ($inputString -notmatch "\d{4}") {
            Write-Output "After IP The next four characters should be digits."
        } elseif ($inputString -notmatch "Receive") {
            Write-Output "The portname should contain 'Receive' after the full IP number"
        } elseif ($inputString -notmatch "$pattern\z") {
            Write-Output "The portname should not end after 'Receive', expects purpose"
        }
        Write-Output "Expected format is: <IP number>Receive<Purpose>"
    }
    Write-Output "---------------------------------------------------------------"
}
function CheckSendPortName {
    param (
        [string]$inputString,
        $sendPort
    )
    
    #Get portname from sendport
    $portType = $SendPort.PrimaryTransport.TransportType.Name
    
    #Create pattern with the porttype
    $pattern = "^IP(\d{1,4})Send(\w+)To(\w+)$portType$"  
    
    # Use the -match operator to check if the input string matches the pattern
    if ($inputString -match $pattern) {
        Write-Output "The sendport '$inputString' follows the specified format."
    } else {
        Write-Output "The sendport '$inputString' does not follow the specified format."
        if ($inputString -notmatch "^IP") {
            Write-Output "The first two characters should be 'IP'."
        } elseif ($inputString -notmatch "\d{1,4}") {
            Write-Output "After IP The next four characters should be digits."
        } elseif ($inputString -notmatch "Send") {
            Write-Output "The portname should contain 'Send'"
        } elseif ($inputString -notmatch "To") {
            Write-Output "The portname should contain 'To'"
        } elseif ($inputString -notmatch "$portType$") {
            Write-Output "The string should end with 'SFTP'."
        } else {
            Write-Output "The portname should contain at least two strings describing the port"
        }
        Write-Output "Expected format is: <IP number>Send<Purpose>To<System><AdapterType>"
    }
    Write-Output "---------------------------------------------------------------"
}

function CheckDescription {
    param (
        $port
    )
    $pattern = "^IK\d{4}$"
    $description = $port.Description
    $portName = $port.Name
    if ($description -match $pattern){
        Write-Output "The decription '$description' for '$portName' follows the specified format."
    }else{
        Write-Output "The decription '$description' for '$portName' does not follow the specified format."
    }
    Write-Output "---------------------------------------------------------------"
}

function CheckAppName{
    param (
        $appName
    )
    $pattern = "^IP\d{4}\s(?:\w+\s)*\w+$"
    if ($appName -match $pattern){
        Write-Output "The app name '$appName' follows the specified format."
    }else{
        Write-Output "The app name '$appName' does not follow the specified format."
    }
    Write-Output "---------------------------------------------------------------"
}

#=== Variabler ===#
#$Miljo="Localhost"
#$iPNamn = "IP0241 Paratus Kundreskontra"


CheckPortNaming -Miljo $Miljo -IPNamn $iPNamn
