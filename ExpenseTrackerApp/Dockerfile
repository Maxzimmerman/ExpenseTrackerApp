#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy the HTTPS certificate into the container
# Correct the destination path
COPY ExpenseTrackerApp/https_certificate/aspnetapp.pfx /app/ExpenseTrackerApp/https_certificate/aspnetapp.pfx

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ExpenseTrackerApp/ExpenseTrackerApp.csproj", "ExpenseTrackerApp/"]
RUN dotnet restore "./ExpenseTrackerApp/ExpenseTrackerApp.csproj"
COPY . .
WORKDIR "/src/ExpenseTrackerApp"
RUN dotnet build "./ExpenseTrackerApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ExpenseTrackerApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseTrackerApp.dll"]