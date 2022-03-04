#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

RUN apt-get update
RUN apt-get install -y ffmpeg

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FFmpegWrapper.Example/FFmpegWrapper.Example.csproj", "FFmpegWrapper.Example/"]
COPY ["FFmpegWrapper/FFmpegWrapper.csproj", "FFmpegWrapper/"]
RUN dotnet restore "FFmpegWrapper.Example/FFmpegWrapper.Example.csproj"
COPY . .
WORKDIR "/src/FFmpegWrapper.Example"
RUN dotnet build "FFmpegWrapper.Example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FFmpegWrapper.Example.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FFmpegWrapper.Example.dll"]