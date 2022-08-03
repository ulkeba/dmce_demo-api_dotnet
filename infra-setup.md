# Create Azure Container Registry
- `az acr create --resource-group dmce-d01 --name dmced01acr`
- `az acr create --resource-group dmce-d01 --name dmced01acr --sku standard`

# Create Service Principal for GitHub Actions
```
ACR_REGISTRY_ID=$(az acr show --name dmced01acr --query "id" --output tsv)
SP_ID=dmce-gh-sp
az ad sp create-for-rbac --name dmce-gh-sp --scopes $ACR_REGISTRY_ID --role acrpush --query "password" --output tsv
```
