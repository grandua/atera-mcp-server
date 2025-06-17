FROM mcr.microsoft.com/dotnet/sdk:9.0

WORKDIR /app
COPY . .

# Test script to verify solution file detection
RUN echo "--- Debug Info ---" && \
    pwd && \
    ls -la && \
    if [ -f "AteraMcp.sln" ]; then \
        echo "SUCCESS: Solution file found at $(pwd)/AteraMcp.sln"; \
    else \
        echo "ERROR: Solution file not found"; \
        exit 1; \
    fi
