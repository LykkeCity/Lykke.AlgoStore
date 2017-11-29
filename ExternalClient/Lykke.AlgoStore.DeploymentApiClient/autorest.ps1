# This script uses Autorest to generate service's client library

# == Prerequisites ==

# Nodejs version >= 6.11.2 - https://nodejs.org/en/download/
# NPM version >= 3.10.10 - https://www.npmjs.com/get-npm
# Autorest version >= 1.2.2 - https://www.npmjs.com/package/autorest

# Run this file if you use PowerShell directly
#autorest -Input http://193.19.175.201:38880/v2/api-docs -CodeGenerator CSharp -OutputDirectory ./JavaApiClient -Namespace Lykke.AlgoStore.Client.JavaApiClient
autorest -Input C:/_temp/api-docs.json -CodeGenerator CSharp -OutputDirectory ./AutorestClient -Namespace Lykke.AlgoStore.DeploymentApiClient
