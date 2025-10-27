param(
  [Parameter(Mandatory=$true)][string]$Probencode
)

Write-Host "Exporting PDF for probencode $Probencode..."
# Minimal runner via UI services is not available headless; run app and trigger manually.
Write-Host "Start the app and use Dashboard → PDF Export."
