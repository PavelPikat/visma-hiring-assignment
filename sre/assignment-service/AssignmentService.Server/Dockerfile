FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AssignmentService.Server/AssignmentService.Server.csproj", "AssignmentService.Server/"]
RUN dotnet restore "AssignmentService.Server/AssignmentService.Server.csproj"
COPY . .
WORKDIR "/src/AssignmentService.Server"
RUN dotnet build "AssignmentService.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AssignmentService.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AssignmentService.Server.dll"]
