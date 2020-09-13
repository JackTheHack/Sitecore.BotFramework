$params = @{
  DnsName = "DO_NOT_TRUST_RootCertificate"
  KeyLength = 2048
  KeyAlgorithm = 'RSA'
  HashAlgorithm = 'SHA256'
  KeyExportPolicy = 'Exportable'
  NotAfter = (Get-Date).AddYears(10)
  CertStoreLocation = 'Cert:\LocalMachine\My'
  KeyUsage = 'CertSign','CRLSign' #fixes invalid cert error
}

$rootCA = New-SelfSignedCertificate @params

$params = @{
  DnsName = "bot93sc"
  Signer = $rootCA
  KeyLength = 2048
  KeyAlgorithm = 'RSA'
  HashAlgorithm = 'SHA256'
  KeyExportPolicy = 'Exportable'
  NotAfter = (Get-date).AddYears(5)
  CertStoreLocation = 'Cert:\LocalMachine\My'
}
$vpnCert = New-SelfSignedCertificate @params

# Extra step needed since self-signed cannot be directly shipped to trusted root CA store
# if you want to silence the cert warnings on other systems you'll need to import the rootCA.crt on them too
Export-Certificate -Cert $rootCA -FilePath "rootCA.crt"
Import-Certificate -CertStoreLocation 'Cert:\LocalMachine\Root' -FilePath "rootCA.crt"

Export-PfxCertificate -Cert $vpnCert -FilePath 'localhost.pfx' -Password (ConvertTo-SecureString -AsPlainText 'securepw' -Force)