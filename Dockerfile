# Use the ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

# Use the ASP.NET Core SDK as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["./CalendarApp.Api/CalendarApp.Api.csproj", "CalendarApp.Api/"]
COPY ["./CalendarApp.Tests/CalendarApp.Tests.csproj", "CalendarApp.Tests/"]
COPY ["./CalendarApp.DataAccess/CalendarApp.DataAccess.csproj", "CalendarApp.DataAccess/"]
COPY ["./CalendarApp.Models/CalendarApp.Models.csproj", "CalendarApp.Models/"]

RUN dotnet restore "CalendarApp.Api/CalendarApp.Api.csproj"
RUN dotnet restore "CalendarApp.Tests/CalendarApp.Tests.csproj"
COPY . .
WORKDIR "/src/"

# Build the application
RUN dotnet build "CalendarApp.Api/CalendarApp.Api.csproj" -c Release -o /app/build

# Test the application
RUN dotnet test "CalendarApp.Tests/CalendarApp.Tests.csproj" --verbosity normal

# Publish the application
FROM build AS publish
RUN dotnet publish "CalendarApp.Api/CalendarApp.Api.csproj" -c Release -o /app/publish

# Final stage: create the runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ARG ENVIRONMENT
ENV ASPNETCORE_ENVIRONMENT=$ENVIRONMENT
ENTRYPOINT ["dotnet", "CalendarApp.Api.dll"]
