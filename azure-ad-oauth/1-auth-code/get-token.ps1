# Get OAuth 2.0 token using Client secret
$Parameters = @{
  grant_type = 'authorization_code'
  scope = '<Enter scope>'
  client_id = "<Enter Client ID>"
  client_secret = "<Enter Client Secret>"
  redirect_uri = 'http://localhost:8080/1-auth-code/redirect.html'
  code = '<Enter code>'
  state = '<Enter state>'
}

$tokenResponse = Invoke-RestMethod -Method Post -Uri https://login.microsoftonline.com/981c08e3-3aac-4b99-9ead-f05044d30a50/oauth2/v2.0/token -Body $Parameters
$tokenResponse | ConvertTo-JSON