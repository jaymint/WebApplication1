   # Use the official .NET SDK image to build the application
   FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
   WORKDIR /app

   # Copy the project files
   COPY . ./

   # Restore dependencies and build the application
   RUN dotnet restore
   RUN dotnet publish -c Release -o out

   # Use the official .NET runtime image to run the application
   FROM mcr.microsoft.com/dotnet/aspnet:8.0
   WORKDIR /app
   COPY --from=build-env /app/out .

   # Expose the port your application runs on
   EXPOSE 8080

   # Set the entry point for the container
   ENTRYPOINT ["dotnet", "WebApplication1.dll"]
   