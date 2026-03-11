#build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 as build
WORKDIR /Alerter
#Copy all
COPY . ./
#restore as distinct layers 
RUN dotnet restore
#build and publish 
RUN dotnet publish -c Release -o /TeleAlerter

#build runtime image 
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /Alerter
COPY --from=build /TeleAlerter .
ENTRYPOINT [ "dotnet", "Alerter.dll" ]



