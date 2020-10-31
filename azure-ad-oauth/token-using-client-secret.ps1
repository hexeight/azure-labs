# Get OAuth 2.0 token using Client secret
$Parameters = @{
  grant_type = 'client_credentials'
  scope = 'https://graph.microsoft.com/.default'
  client_id = '79254b4e-c4fc-4998-b1b2-b01d11249365'
  client_secret = 'A-~B-3-ATq_rtxoz1iUhO8QC2VE_RW58Oa'
}

$tokenResponse = Invoke-RestMethod -Method Post -Uri https://login.microsoftonline.com/981c08e3-3aac-4b99-9ead-f05044d30a50/oauth2/v2.0/token -Body $Parameters
$tokenResponse | ConvertTo-JSON