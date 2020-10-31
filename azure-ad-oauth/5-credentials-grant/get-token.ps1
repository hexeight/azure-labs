# Get OAuth 2.0 token using Client secret
$Parameters = @{
  grant_type = 'client_credentials'
  scope = 'https://graph.microsoft.com/.default'
  client_id = '<Enter Client ID>'
  client_secret = '<Enter Client Secret>'
}

$tenantId = "<Enter Tenant ID>"
$authorizeUri = "https://login.microsoftonline.com/$tenantId/oauth2/v2.0/token"
$tokenResponse = Invoke-RestMethod -Method Post -Uri $authorizeUri -Body $Parameters
$tokenResponse | ConvertTo-JSON