# FEAT k6 Performance Tests (API)

Scope:
- POST /api/Search (initial + filtered + pagination)
- GET /api/Courses/{instanceId} (details)
- GET /api/AutocompleteLocations?query=... (location autocomplete)

## Run baseline (~0.02 rps)
BASE_URL="https://s265t02-app-api.azurewebsites.net" SCENARIO="baseline" k6 run feat/performance/k6/scripts/searchApi.test.js

## Run peak (~0.08 rps)
BASE_URL="https://s265t02-app-api.azurewebsites.net" SCENARIO="peak" k6 run feat/performance/k6/scripts/searchApi.test.js

## Run spike (~0.25 rps)
BASE_URL="https://s265t02-app-api.azurewebsites.net" SCENARIO="spike" k6 run feat/performance/k6/scripts/searchApi.test.js

## Run stress (6–12 rps burst)
BASE_URL="https://s265t02-app-api.azurewebsites.net" SCENARIO="stress" k6 run feat/performance/k6/scripts/searchApi.test.js

