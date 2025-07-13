#!/bin/bash

echo "=== Testing User Management API Middleware ==="
echo

# Test 1: Request without token (should return 401)
echo "1. Testing request without authentication token:"
curl -s -w "\nStatus: %{http_code}\n" http://localhost:5000/users
echo

# Test 2: Request with invalid token (should return 401)
echo "2. Testing request with invalid token:"
curl -s -w "\nStatus: %{http_code}\n" -H "Authorization: Bearer invalid-token" http://localhost:5000/users
echo

# Test 3: Request with valid token (should return 200)
echo "3. Testing request with valid token:"
curl -s -w "\nStatus: %{http_code}\n" -H "Authorization: Bearer valid-token-123" http://localhost:5000/users
echo

# Test 4: Create a user with valid token
echo "4. Testing POST request with valid token:"
curl -s -w "\nStatus: %{http_code}\n" -X POST \
  -H "Authorization: Bearer valid-token-123" \
  -H "Content-Type: application/json" \
  -d '{"name":"John Doe","email":"john@example.com","age":30}' \
  http://localhost:5000/users
echo

# Test 5: Test error handling middleware
echo "5. Testing error handling middleware:"
curl -s -w "\nStatus: %{http_code}\n" -H "Authorization: Bearer valid-token-123" http://localhost:5000/test/error
echo

# Test 6: Test validation error
echo "6. Testing validation error:"
curl -s -w "\nStatus: %{http_code}\n" -X POST \
  -H "Authorization: Bearer valid-token-123" \
  -H "Content-Type: application/json" \
  -d '{"name":"","email":"invalid-email","age":-5}' \
  http://localhost:5000/users
echo