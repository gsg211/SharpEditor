#!/bin/bash

API_URL="http://localhost:5035"

echo "========================================"
echo "    TEST BACKEND - Document Editor API   "
echo "========================================"


# [1] REGISTER ion
echo -e "\n[1] REGISTER user ion..."
curl -X POST "$API_URL/auth/register" -H "Content-Type: application/json" -d '{"email":"ion@test.com","password":"Password123!","username":"Ion"}'
echo ""

# [2] REGISTER maria
echo -e "\n[2] REGISTER user maria..."
curl -X POST "$API_URL/auth/register" -H "Content-Type: application/json" -d '{"email":"maria@test.com","password":"Password123!","username":"Maria"}'
echo ""
# ---------------------------------------------------------------------
# [3] LOGIN ion
# ---------------------------------------------------------------------
echo -e "\n[3] LOGIN ion..."
RESPONSE_ION=$(curl -s -X POST "$API_URL/auth/login" -H "Content-Type: application/json" -d '{"email":"ion@test.com","password":"Password123!"}')
TOKEN_ION=$(echo $RESPONSE_ION | jq -r '.token // .accessToken')
echo "Token Ion salvat."

# ---------------------------------------------------------------------
# [4] LOGIN maria & Extragere UserId Maria
# ---------------------------------------------------------------------
echo -e "\n[4] LOGIN maria..."
RESPONSE_MARIA=$(curl -s -X POST "$API_URL/auth/login" -H "Content-Type: application/json" -d '{"email":"maria@test.com","password":"Password123!"}')
TOKEN_MARIA=$(echo $RESPONSE_MARIA | jq -r '.token // .accessToken')

# Încercăm să extragem ID-ul Mariei din răspunsul de Login (dacă e inclus în obiectul de user)
USER_ID_MARIA=$(echo $RESPONSE_MARIA | jq -r '.user.id // .userId // empty')

if [ -z "$USER_ID_MARIA" ]; then
  # Dacă nu e în login, de regulă Maria fiind al doilea user înregistrat într-o bază curată va avea ID-ul 2
  USER_ID_MARIA="2"
fi
echo "Token Maria salvat. (UserId detectat pentru Maria: $USER_ID_MARIA)"

# ---------------------------------------------------------------------
# [5] CREARE document (ion) -> POST /shared-docs
# ---------------------------------------------------------------------
echo -e "\n[5] CREARE document (ion)..."
DOC_RESPONSE=$(curl -s -X POST "$API_URL/shared-docs" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN_ION" \
  -d '{"title":"Raport Anual IP","content":"Acesta este continutul initial al documentului."}')

# Extragem ID-ul din JSON-ul întors (SharedDocDetailDto are proprietatea 'shareId' sau 'id')
SHARE_ID=$(echo $DOC_RESPONSE | jq -r '.shareId // .id // empty')

if [ -z "$SHARE_ID" ] || [ "$SHARE_ID" = "null" ]; then
  echo "EROARE: Nu s-a putut extrage shareId din răspuns!"
  echo "Răspunsul primit a fost: $DOC_RESPONSE"
  echo "Forțăm shareId generic '1' pentru continuarea testului."
  SHARE_ID="1"
else
  echo "shareId extras cu succes: $SHARE_ID"
fi

# ---------------------------------------------------------------------
# [6] GET lista documente ion -> GET /shared-docs
# ---------------------------------------------------------------------
echo -e "\n[6] GET lista documente ion..."
curl -X GET "$API_URL/shared-docs" -H "Authorization: Bearer $TOKEN_ION"
echo ""

# ---------------------------------------------------------------------
# [7] GET document specific -> GET /shared-docs/{shareId}
# ---------------------------------------------------------------------
echo -e "\n[7] GET document specific..."
curl -X GET "$API_URL/shared-docs/$SHARE_ID" -H "Authorization: Bearer $TOKEN_ION"
echo ""

# ---------------------------------------------------------------------
# [8] UPDATE document (version 1) -> PUT /shared-docs/{shareId}
# ---------------------------------------------------------------------
echo -e "\n[8] UPDATE document (version 1)..."
curl -X PUT "$API_URL/shared-docs/$SHARE_ID" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN_ION" \
  -d '{"title":"Raport Anual IP Modificat","content":"Continut modificat prima data.","version":1}'
echo ""

# ---------------------------------------------------------------------
# [9] UPDATE document din nou (version 2)
# ---------------------------------------------------------------------
echo -e "\n[9] UPDATE document din nou (version 2)..."
curl -X PUT "$API_URL/shared-docs/$SHARE_ID" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN_ION" \
  -d '{"title":"Raport Anual IP Final","content":"Continut finalizat.","version":2}'
echo ""

# ---------------------------------------------------------------------
# [10] UPDATE document cu versiune gresita (409 asteptat)
# ---------------------------------------------------------------------
echo -e "\n[10] UPDATE document cu versiune gresita (409 asteptat)..."
curl -i -X PUT "$API_URL/shared-docs/$SHARE_ID" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN_ION" \
  -d '{"title":"Atac Concurent","content":"Ar trebui sa dea eroare.","version":1}'
echo ""

# ---------------------------------------------------------------------
# [11] SHARE document cu maria (ReadOnly)
# ---------------------------------------------------------------------
echo -e "\n[11] SHARE document cu maria (ReadOnly)..."
curl -X POST "$API_URL/shared-docs/$SHARE_ID/shares" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN_ION" \
     -d "{\"userEmail\":\"maria@test.com\",\"userId\":$USER_ID_MARIA,\"permission\":\"ReadOnly\"}"
echo ""

# ---------------------------------------------------------------------
# [12] GET shares document -> GET /shared-docs/{shareId}/shares
# ---------------------------------------------------------------------
echo -e "\n[12] GET shares document..."
curl -X GET "$API_URL/shared-docs/$SHARE_ID/shares" -H "Authorization: Bearer $TOKEN_ION"
echo ""

# ---------------------------------------------------------------------
# [13] Maria incearca sa editeze fara drepturi (403 asteptat)
# ---------------------------------------------------------------------
echo -e "\n[13] Maria incearca sa editeze (403 asteptat)..."
curl -i -X PUT "$API_URL/shared-docs/$SHARE_ID" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN_MARIA" \
  -d '{"title":"Maria Hack","content":"Incerc sa modific ilegal.","version":3}'
echo ""

# ---------------------------------------------------------------------
# [14] UPDATE permisiune maria la ReadWrite -> PUT /shared-docs/{shareId}/shares/{userId}
# ---------------------------------------------------------------------
echo -e "\n[14] UPDATE permisiune maria la ReadWrite..."
curl -X PUT "$API_URL/shared-docs/$SHARE_ID/shares/$USER_ID_MARIA" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN_ION" \
  -d '{"permission":"ReadWrite"}'
echo ""

# ---------------------------------------------------------------------
# [15] Maria editeaza acum (200 asteptat)
# ---------------------------------------------------------------------
echo -e "\n[15] Maria editeaza acum (200 asteptat)..."
curl -X PUT "$API_URL/shared-docs/$SHARE_ID" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN_MARIA" \
  -d '{"title":"Raport Editat de Maria","content":"Maria are drepturi acum.","version":3}'
echo ""

# ---------------------------------------------------------------------
# [16] REVOKE acces maria -> DELETE /shared-docs/{shareId}/shares/{userId}
# ---------------------------------------------------------------------
echo -e "\n[16] REVOKE acces maria..."
curl -i -X DELETE "$API_URL/shared-docs/$SHARE_ID/shares/$USER_ID_MARIA" \
  -H "Authorization: Bearer $TOKEN_ION"
echo ""

# ---------------------------------------------------------------------
# [17] Maria incearca sa acceseze dupa revocare (403 sau 404 asteptat)
# ---------------------------------------------------------------------
echo -e "\n[17] Maria incearca sa acceseze dupa revocare (403/404 asteptat)..."
curl -i -X GET "$API_URL/shared-docs/$SHARE_ID" -H "Authorization: Bearer $TOKEN_MARIA"
echo ""

# ---------------------------------------------------------------------
# [18] STERGERE document (ion) -> DELETE /shared-docs/{shareId}
# ---------------------------------------------------------------------
echo -e "\n[18] STERGERE document (ion)..."
curl -i -X DELETE "$API_URL/shared-docs/$SHARE_ID" -H "Authorization: Bearer $TOKEN_ION"
echo ""

# ---------------------------------------------------------------------
# [19] GET document dupa stergere (404 asteptat)
# ---------------------------------------------------------------------
echo -e "\n[19] GET document dupa stergere (404 asteptat)..."
curl -i -X GET "$API_URL/shared-docs/$SHARE_ID" -H "Authorization: Bearer $TOKEN_ION"
echo ""

echo "========================================"
echo "            TESTE COMPLETE              "
echo "========================================"
