az login

az group create --name com.loitzl.userinviter --location westeurope
az appservice plan create --name "com.loitzl.userinviter" --resource-group "com.loitzl.userinviter" --sku F1
dotnet publish -o publish
rm .\publish.zip
Compress-Archive -Path publish/* -DestinationPath publish.zip
az webapp create --name "com-loitzl-userinviter" --resource-group "com.loitzl.userinviter" --plan "com.loitzl.userinviter"
az webapp deployment source config-zip --src .\publish.zip -n "com-loitzl-userinviter" -g "com.loitzl.userinviter"