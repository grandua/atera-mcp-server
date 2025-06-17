# Enhanced test script
Write-Host "Testing solution file detection in Docker..."
Write-Host "--- Building test container ---"
docker build --progress plain -t atera-path-test -f .\Scripts\TestScripts\Dockerfile .

# Verify results
Write-Host "\n--- Test completed ---"
if ($LASTEXITCODE -eq 0) {
    Write-Host "SUCCESS: Solution file was found in container" -ForegroundColor Green
} else {
    Write-Host "ERROR: Solution file was not found" -ForegroundColor Red
}
