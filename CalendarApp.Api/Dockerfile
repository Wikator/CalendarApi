# Use the ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

# Use the ASP.NET Core SDK as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["CalendarApp.Api.csproj", "./"]
RUN dotnet restore "CalendarApp.Api.csproj"
COPY . .
WORKDIR "/src/"

# Build the application
RUN dotnet build "CalendarApp.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "CalendarApp.Api.csproj" -c Release -o /app/publish

# Final stage: create the runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CalendarApp.Api.dll"]
