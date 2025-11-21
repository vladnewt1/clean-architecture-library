# Test AutoMapper Endpoints
$baseUrl = "http://localhost:5082/api/automapper"

Write-Host "=== Testing AutoMapper Demo Controller ===" -ForegroundColor Cyan

# Test 1: Get Info
Write-Host "`n1. GET /info - AutoMapper Pattern Info" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/info" -Method GET
    Write-Host "Status: SUCCESS" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "Status: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Get All Books (Entity -> DTO)
Write-Host "`n2. GET /books - Get All Books as DTOs" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/books" -Method GET
    Write-Host "Status: SUCCESS - Retrieved $($response.Count) books" -ForegroundColor Green
    if ($response.Count -gt 0) {
        Write-Host "Sample Book DTO:" -ForegroundColor Cyan
        $response[0] | ConvertTo-Json -Depth 2
    }
} catch {
    Write-Host "Status: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Create Book (DTO -> Entity)
Write-Host "`n3. POST /books - Create Book from DTO" -ForegroundColor Yellow
$newBook = @{
    title = "AutoMapper in Action"
    author = "Jimmy Bogard"
    isbn = "978-1-61729-456-7"
    publishedYear = 2024
    publisher = "Manning Publications"
    category = "Technical"
    description = "Practical guide to AutoMapper"
    pageCount = 350
    language = "English"
    totalCopies = 5
    price = 49.99
    coverImageUrl = "https://example.com/automapper.jpg"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/books" -Method POST -Body $newBook -ContentType "application/json"
    Write-Host "Status: SUCCESS - Created book with ID: $($response.id)" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 2
    $global:createdBookId = $response.id
} catch {
    Write-Host "Status: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Get Single Book
Write-Host "`n4. GET /books/{id} - Get Single Book as DTO" -ForegroundColor Yellow
if ($global:createdBookId) {
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/books/$($global:createdBookId)" -Method GET
        Write-Host "Status: SUCCESS" -ForegroundColor Green
        $response | ConvertTo-Json -Depth 2
    } catch {
        Write-Host "Status: FAILED - $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Test 5: Get All Members (Entity -> DTO with nested Address)
Write-Host "`n5. GET /members - Get All Members as DTOs" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/members" -Method GET
    Write-Host "Status: SUCCESS - Retrieved $($response.Count) members" -ForegroundColor Green
    if ($response.Count -gt 0) {
        Write-Host "Sample Member DTO with Address:" -ForegroundColor Cyan
        $response[0] | ConvertTo-Json -Depth 3
    }
} catch {
    Write-Host "Status: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

# Test 6: Create Member (DTO -> Entity with nested Address)
Write-Host "`n6. POST /members - Create Member from DTO" -ForegroundColor Yellow
$newMember = @{
    firstName = "Test"
    lastName = "AutoMapper"
    email = "test.automapper@example.com"
    phoneNumber = "+380501234567"
    dateOfBirth = "1990-01-01"
    address = @{
        street = "Test Street 123"
        city = "Kyiv"
        state = "Kyiv Oblast"
        postalCode = "01001"
        country = "Ukraine"
    }
    membershipType = "Regular"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/members" -Method POST -Body $newMember -ContentType "application/json"
    Write-Host "Status: SUCCESS - Created member with ID: $($response.id)" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 3
    $global:createdMemberId = $response.id
} catch {
    Write-Host "Status: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

# Test 7: Get All Loans (Entity -> DTO with navigation properties)
Write-Host "`n7. GET /loans - Get All Loans as DTOs" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/loans" -Method GET
    Write-Host "Status: SUCCESS - Retrieved $($response.Count) loans" -ForegroundColor Green
    if ($response.Count -gt 0) {
        Write-Host "Sample Loan DTO with navigation properties:" -ForegroundColor Cyan
        $response[0] | ConvertTo-Json -Depth 2
    }
} catch {
    Write-Host "Status: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

# Test 8: Get Member Statistics (calculated properties)
Write-Host "`n8. GET /members/statistics - Get Member Statistics DTOs" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/members/statistics" -Method GET
    Write-Host "Status: SUCCESS - Retrieved statistics for $($response.Count) members" -ForegroundColor Green
    if ($response.Count -gt 0) {
        Write-Host "Sample Member Statistics DTO:" -ForegroundColor Cyan
        $response[0] | ConvertTo-Json -Depth 2
    }
} catch {
    Write-Host "Status: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

# Test 9: Complex Demo (transaction with multiple DTOs)
Write-Host "`n9. POST /complex-demo - Complex Transaction Demo" -ForegroundColor Yellow
if ($global:createdBookId -and $global:createdMemberId) {
    $complexDemo = @{
        bookId = $global:createdBookId
        memberId = $global:createdMemberId
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/complex-demo" -Method POST -Body $complexDemo -ContentType "application/json"
        Write-Host "Status: SUCCESS" -ForegroundColor Green
        $response | ConvertTo-Json -Depth 3
    } catch {
        Write-Host "Status: FAILED - $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n=== AutoMapper Testing Complete ===" -ForegroundColor Cyan
