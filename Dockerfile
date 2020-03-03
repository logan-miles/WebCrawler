FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY ./*.sln  ./
COPY ./src/ ./src
COPY ./tests/ ./tests

RUN dotnet restore WebCrawler.sln

LABEL test=true
# install the report generator tool
RUN dotnet tool install dotnet-reportgenerator-globaltool --version 4.5.0 --tool-path /tools

# # run the test and collect code coverage (requires coverlet.msbuild to be added to test project)
# # for exclude, use %2c for ,
RUN dotnet test --results-directory /testresults --logger "trx;LogFileName=test_results.xml" /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=/testresults/coverage/ /p:Exclude="[xunit.*]*%2c[StackExchange.*]*" ./tests/WebCrawler.Tests/WebCrawler.Tests.csproj

# # generate html reports using report generator tool
RUN /tools/reportgenerator "-reports:/testresults/coverage/coverage.cobertura.xml" "-targetdir:/testresults/coverage/reports" "-reporttypes:HTMLInline;HTMLChart"
RUN ls -la /testresults/coverage/reports

RUN dotnet publish src/WebCrawler.ConsoleApp/WebCrawler.ConsoleApp.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "WebCrawler.ConsoleApp.dll"]
