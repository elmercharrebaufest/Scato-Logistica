# Use a Windows base image with .NET Framework 4.5.2
#FROM mcr.microsoft.com/dotnet/framework/aspnet:4.5.2-windowsservercore-1803
FROM mcr.microsoft.com/dotnet/framework/sdk

# Set the working directory
WORKDIR C:\app

# Enable debugging environment variables
ENV COR_ENABLE_PROFILING 1
ENV COR_PROFILER {32DA19BD-35B1-4B4D-AE9A-00D8D8BFCA84}
ENV MicrosoftInstrumentationEngine_FileLogPath C:\app\logs

# Copy your application files to the container
COPY . .

# Expose the port your application listens on (if applicable)
EXPOSE 80

# Define the command to start your application
CMD ["YourApp.exe"]
