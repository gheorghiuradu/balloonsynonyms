FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["KinderPlay/KinderPlay.csproj", "KinderPlay/"]
RUN dotnet restore "KinderPlay/KinderPlay.csproj"
COPY . .
WORKDIR "/src/KinderPlay"
RUN dotnet build "KinderPlay.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "KinderPlay.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "KinderPlay.dll"]