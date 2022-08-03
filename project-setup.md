# Setup of new dotnet project (in `./dotnet`)
- `dotnet new webapi --name WeatherForecastAPI --output ./dotnet`
- `dotnet new gitignore --output ./dotnet`
- Open in VS Codde and add run configuration; specify `uriFormat` to open swagger overview.

# Initialize GitVersion with default setup
- References
  - [GitVersion](https://gitversion.net/)
  - [GitVersion Mainline Development](https://gitversion.net/docs/reference/modes/mainline)
- Start docker container containing GitVersion CLI:
  ```
  docker run -it --rm -v "$(pwd):/repo" --entrypoint bash gittools/gitversion:5.10.3
  ```
- Create new GitVersion.yml (only accepting defaults):
  ```
  $ cd /repo/

  $ /tools/dotnet-gitversion init
  INFO [08/03/22 10:06:42:49] Working directory: /repo
  GitVersion init will guide you through setting GitVersion up to work for you
  
  Which would you like to change?
  
  0) Save changes and exit
  1) Exit without saving
  
  2) Run getting started wizard
  
  3) Set next version number
  4) Branch specific configuration
  5) Branch Increment mode (per commit/after tag) (Current: )
  6) Assembly versioning scheme (Current: )
  7) Setup build scripts
  
  > 0
  INFO [08/03/22 10:06:48:18] Saving config file
  INFO [08/03/22 10:06:48:25] Done writing

  $ exit
  ```
- Show effective configuration:
  ```
  $ docker run -it --rm -v "$(pwd):/repo" gittools/gitversion:5.10.3 /repo /showconfig
  ```
- Show current versioning details:
  ```
  $ docker run -it --rm -v "$(pwd):/repo" gittools/gitversion:5.10.3 /repo
  {
    "SemVer": "0.1.0",
    "FullSemVer": "0.1.0+2",
    "InformationalVersion": "0.1.0+2.Branch.main.Sha.0d450ec9bbd9f05157ac5f399e5c30ee595c065c",
    "BranchName": "main",
    ...
  }
  ```
- _Note_ The empty configuration file could have been omitted as only default values apply. It is added here to provide transparency about the tool feature and create the file in the repository to set parameters if necessary. For details, see [GitVersion - Configuration](https://gitversion.net/docs/reference/configuration)
- Example: Set tag to specify version 0.2.0 and print version details:
  ```
  $ git tag v0.2.0

  $ docker run -it --rm -v "$(pwd):/repo"   gittools/gitversion:5.10.3 /repo
  {
    "SemVer": "0.2.0",
    "FullSemVer": "0.2.0",
    "InformationalVersion": "0.2.0+Branch.main.Sha.1ffbc3efbaff6e74316eb46af97cb50a7f9289fc",
    "BranchName": "main",
    ...
  }
   ```
   For more details about the versioning with GitVersion, see [GitVersion Mainline Development](https://gitversion.net/docs/reference/modes/mainline).


# Setup infrastructure to build container
- References
  - [.NET Docker Sample](https://github.com/dotnet/dotnet-docker/blob/main/samples/dotnetapp/README.md)
- Build container:
  ```
  $ sudo docker build --build-arg API_VERSION="<build-command>" -t demo-api_dotnet:latest .
  ```
- Run container:
  ```
  $ docker run -it --rm -p 8080:80 demo-api_dotnet:latest
  ```
- Test call:
  ```
  $ curl http://localhost:8080/VersionInfo
  {"version":"<build-command>"}
  ```

# Setup GitHub project
- Create new project
  ```
  $ gh repo create dmce_demo-api_dotnet --public
  ```
- Add remote
  ```
  $ git remote add origin git@github.com:<YOUR NAME HERE>/dmce_demo-api_dotnet.git
  ```
- Set secrets to push to container registry.
  ```
  $ gh secret set AZ_ACR_NAME --body "[YOUR ACR NAME HERE]"
  $ gh secret set AZ_SP_CLIENT_ID --body "[YOUR SP ID HERE]"
  $ gh secret set AZ_SP_CLIENT_SECRET --body "[YOUR SP PASSWORD HERE]"
  ```