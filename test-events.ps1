# Test Event Driven Architecture - ПР3
# Run this script after starting the API: dotnet run --project src/LibraryManagement.API/LibraryManagement.API.csproj

$baseUrl = "http://localhost:5000/api"

Write-Host "=== Testing Event Driven Architecture ===" -ForegroundColor Cyan

# Test 1: Register a Member (MemberRegisteredEvent)
Write-Host "`n1. Testing Member Registration (MemberRegisteredEvent)..." -ForegroundColor Yellow
$memberData = @{
    firstName = "Іван"
    lastName = "Петренко"
    email = "ivan.petrenko@test.com"
    phoneNumber = "+380501234567"
    address = @{
        street = "вул. Шевченка 10"
        city = "Київ"
        zipCode = "01001"
        country = "Україна"
    }
    membershipType = "Standard"
} | ConvertTo-Json

try {
    $member = Invoke-RestMethod -Method POST -Uri "$baseUrl/membermanagement/register" -ContentType "application/json" -Body $memberData
    Write-Host "✅ Member registered: $($member.firstName) $($member.lastName) (ID: $($member.id))" -ForegroundColor Green
    Write-Host "   Library Card: $($member.libraryCardNumber)" -ForegroundColor Green
    $memberId = $member.id
} catch {
    Write-Host "❌ Error: $_" -ForegroundColor Red
    exit
}

# Test 2: Add a Book
Write-Host "`n2. Adding a book..." -ForegroundColor Yellow
$bookData = @{
    title = "Кобзар"
    author = "Тарас Шевченко"
    isbn = "978-617-12-5432-1"
    publishedYear = 1840
    category = "Poetry"
    availableCopies = 5
    totalCopies = 5
} | ConvertTo-Json

try {
    $book = Invoke-RestMethod -Method POST -Uri "$baseUrl/books" -ContentType "application/json" -Body $bookData
    Write-Host "✅ Book added: $($book.title) by $($book.author) (ID: $($book.id))" -ForegroundColor Green
    $bookId = $book.id
} catch {
    Write-Host "❌ Error: $_" -ForegroundColor Red
    exit
}

# Test 3: Create a Loan (BookBorrowedEvent)
Write-Host "`n3. Testing Book Borrowing (BookBorrowedEvent)..." -ForegroundColor Yellow
$loanData = @{
    bookId = $bookId
    memberId = $memberId
} | ConvertTo-Json

try {
    $loan = Invoke-RestMethod -Method POST -Uri "$baseUrl/loans" -ContentType "application/json" -Body $loanData
    Write-Host "✅ Loan created: Loan ID $($loan.id)" -ForegroundColor Green
    Write-Host "   Book: $($loan.bookTitle)" -ForegroundColor Green
    Write-Host "   Member: $($loan.memberName)" -ForegroundColor Green
    Write-Host "   Due Date: $($loan.dueDate)" -ForegroundColor Green
    $loanId = $loan.id
} catch {
    Write-Host "❌ Error: $_" -ForegroundColor Red
    exit
}

# Test 4: Return the Book (BookReturnedEvent)
Write-Host "`n4. Testing Book Return (BookReturnedEvent)..." -ForegroundColor Yellow
try {
    $returnResult = Invoke-RestMethod -Method POST -Uri "$baseUrl/loans/$loanId/return" -ContentType "application/json" -Body "{}"
    Write-Host "✅ Book returned successfully!" -ForegroundColor Green
    if ($returnResult.lateFee -gt 0) {
        Write-Host "   Late Fee: $($returnResult.lateFee)" -ForegroundColor Yellow
    } else {
        Write-Host "   No late fee (returned on time)" -ForegroundColor Green
    }
} catch {
    Write-Host "❌ Error: $_" -ForegroundColor Red
}

Write-Host "`n=== Test Complete ===" -ForegroundColor Cyan
Write-Host "Check the server console output for event logs!" -ForegroundColor Magenta
Write-Host "Look for:" -ForegroundColor Magenta
Write-Host "  📧 SendWelcomeEmailAsync" -ForegroundColor Magenta
Write-Host "  🎉 SendMemberRegisteredNotificationAsync" -ForegroundColor Magenta
Write-Host "  📚 SendBookBorrowedNotificationAsync" -ForegroundColor Magenta
Write-Host "  ✅ SendBookReturnedNotificationAsync" -ForegroundColor Magenta
Write-Host "  📝 LogEventAsync (Audit logs)" -ForegroundColor Magenta
