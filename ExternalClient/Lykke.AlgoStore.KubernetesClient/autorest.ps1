# This script uses Autorest to generate service's client library

# == Prerequisites ==

# Nodejs version >= 6.11.2 - https://nodejs.org/en/download/
# NPM version >= 3.10.10 - https://www.npmjs.com/get-npm
# Autorest version >= 1.2.2 - https://www.npmjs.com/package/autorest

# Run this file if you use PowerShell directly 
# REMARKS:
# Input url from below MUST be the same as the one (DeploymentApiServiceUrl) in settings
# We should uncomment line bellow as soon as Nikolay is finished with changes in regard to swagger definitions

# If you need to update client please use provided swagger file.
# First update swagger file and regenerate client via this script

#autorest -Input http://127.0.0.1:8001/swagger.json -AddCredentials true -CodeGenerator CSharp -OutputDirectory ./AutorestClient -Namespace Lykke.AlgoStore.KubernetesClient
autorest -Input swagger.json -AddCredentials true -CodeGenerator CSharp -OutputDirectory ./AutorestClient -Namespace Lykke.AlgoStore.KubernetesClient
