# replace with paths to your own files
$Command = ".\output\File copy.exe"
$Parms = "C:\source","C:\destination"
$Prms = $Parms.Split(",")
& "$Command" $Prms