FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /src
# Copy the main source project files
COPY . .
RUN dotnet restore "src/BookLibrary" --no-cache
COPY . .
WORKDIR "/src/."
RUN dotnet build "src/BookLibrary" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "src/BookLibrary" -c Release -o /app/publish
FROM base AS final

RUN apt-get upgrade
RUN apt-get update -y
RUN apt-get install -y tzdata

WORKDIR /app
COPY --from=publish /app/publish .
ENV TZ Europe/Berlin
ENTRYPOINT ["dotnet", "API.dll"]