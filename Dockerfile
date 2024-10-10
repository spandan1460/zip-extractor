FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim AS build-env
WORKDIR /App
RUN curl -L https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | sh

# Restore as distinct layers
# Copy everything
RUN rm -rf /App/build
WORKDIR /App/build
COPY . ./

RUN dotnet restore --use-current-runtime --no-cache

# Build and publish a release

RUN aspnetcore_version=7.0.10 \
    && curl -fSL --output aspnetcore.tar.gz https://dotnetcli.azureedge.net/dotnet/aspnetcore/Runtime/$aspnetcore_version/aspnetcore-runtime-$aspnetcore_version-linux-arm.tar.gz \
    && aspnetcore_sha512='024354ba1a278d38ac273eac470dbc4562e00351fc675913599cbfcbf853243f8bc1e0a0de7df1a53da087ff836760ab1964009e9c7447fc28a8cf9493eb7e24' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && tar -oxzf aspnetcore.tar.gz ./shared/Microsoft.AspNetCore.App \
    && rm aspnetcore.tar.gz

# -v:m show only console out and errors not warnings - see https://github.com/dotnet/sdk/issues/7986
# https://gordonbeeming.com/blog/msb4019-microsoft-data-tools-schema-sqltasks-targets-was-not-found - ran into this error when attempting to publish
RUN dotnet publish -c Release -v:m --use-current-runtime
RUN apt-get upgrade

# Build runtime image
WORKDIR /App
FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim
COPY --from=build-env /App/build/src/bin/Release/net7.0/linux-* /App/
COPY entrypoint.sh .
ENV ASPNETCORE_ENVIRONMENT "development"
RUN chmod u+x entrypoint.sh

# Create a simple background monitor that verifies that the service is running
ENTRYPOINT ["./entrypoint.sh"]
