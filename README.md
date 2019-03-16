## Execute Action

[![NuGet](https://img.shields.io/nuget/v/wk.AlfrescoExecuteAction.svg)](https://www.nuget.org/packages/wk.AlfrescoExecuteAction)

## Installation

```
dotnet tool install -g wk.AlfrescoExecuteAction
```

## Usage

```bash
wk-alfresco-execute-acion \
    --url http://localhost:8082 \
    --user admin \
    --password admin \
    --target-path x/y/z/README.md \
    http/AddFeature.json

wk-alfresco-execute-acion \
    -h http://localhost:8082 \
    -u admin \
    -p admin \
    -t x/y/z/README.md \
    http/AddFeature.json
```