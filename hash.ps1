$salt = "SistemaPuntoVenta2025"
$password = "123456"
$passwordWithSalt = $password + $salt
$bytes = [System.Text.Encoding]::UTF8.GetBytes($passwordWithSalt)
$sha256 = [System.Security.Cryptography.SHA256]::Create()
$hash = $sha256.ComputeHash($bytes)
$hashString = [System.Convert]::ToBase64String($hash)
Write-Host $hashString
