FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-TechDevs.Core.UserManagement.API/obj/Docker/publish} .
ENTRYPOINT ["dotnet", "TechDevs.Core.UserManagement.API.dll"]
