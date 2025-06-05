# this was made by ChatGPT, I do not take any credit for this script

param (
    [string]$filePath
)

$webhookUrl = "https://discord.com/api/webhooks/1337677067361189908/gEl4zQANQXAeo7ErltOapP9tvL9B91nRX8bWW-1nOJAmZAITGfwx1Iml55l-RGkXJFBz"

# Debugging: Print file path
Write-Host "File Path: $filePath"

# Ensure the file exists
if (-Not (Test-Path $filePath)) {
    Write-Host "ERROR: File not found - $filePath"
    exit 1
}

# Prepare headers
$boundary = [System.Guid]::NewGuid().ToString()
$LF = "`r`n"

$headers = @{
    "Content-Type" = "multipart/form-data; boundary=$boundary"
}

# Create JSON payload for the message
$jsonPayload = @{ "content" = "**New 0.9.5 Beta Build!**" } | ConvertTo-Json -Compress

# Create multipart body
$body = "--$boundary$LF"
$body += "Content-Disposition: form-data; name=`"payload_json`"$LF$LF"
$body += "$jsonPayload$LF"
$body += "--$boundary$LF"
$body += "Content-Disposition: form-data; name=`"file`"; filename=`"$(Split-Path $filePath -Leaf)`"$LF"
$body += "Content-Type: application/octet-stream$LF$LF"

# Read file bytes
$bodyBytes = [System.IO.File]::ReadAllBytes($filePath)
$bodyEnd = "$LF--$boundary--$LF"

# Create MemoryStream for request body
$bodyStream = [System.IO.MemoryStream]::new()
$writer = [System.IO.StreamWriter]::new($bodyStream)
$writer.Write($body)
$writer.Flush()
$bodyStream.Write($bodyBytes, 0, $bodyBytes.Length)
$writer.Write($bodyEnd)
$writer.Flush()
$bodyStream.Position = 0

# Send request
try {
    $response = Invoke-WebRequest -Uri $webhookUrl -Method Post -Headers $headers -Body $bodyStream -TimeoutSec 60
    Write-Host "✅ Success! File uploaded: $filePath"
} catch {
    Write-Host "❌ Upload failed: $_"
    exit 1
}
