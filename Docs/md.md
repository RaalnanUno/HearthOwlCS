# EVAuto – Backlog Items: Production Credentials + MuleSoft Local Mock

## PBI 1 — Obtain & Secure Production API Credentials for EVAuto

**Type:** Product Backlog Item / Infrastructure

### Description
Acquire, validate, and securely store all required production credentials needed for EVAuto to operate against production services.

This work enables production deployments and runtime connectivity but does **not** include application code changes.

### Scope / Requirements

#### Microsoft Graph
- Tenant ID
- Client ID (App Registration)
- Client Secret
- Confirm required permissions (App-only vs delegated)

#### MuleSoft
- API Key
- Production API Base URL (API URL)

### Acceptance Criteria
- [ ] Production **Tenant ID** obtained and documented
- [ ] Production **Microsoft Graph Client ID** confirmed
- [ ] Production **Microsoft Graph Client Secret** created
- [ ] Required Graph permissions reviewed and approved
- [ ] Production **MuleSoft API Key** obtained
- [ ] Production **MuleSoft API URL** confirmed
- [ ] All secrets stored in approved secret store (e.g., Azure Key Vault / GitHub Secrets / secure config store)
- [ ] No secrets committed to source control
- [ ] Deployment pipeline updated to reference secure secrets
- [ ] Validation performed (non-destructive connectivity check)

### Out of Scope
- Implementing application logic
- Modifying API contracts
- Schema or database changes

### Dependencies
- Security / IAM approval
- Access to Azure AD / Entra admin
- MuleSoft admin access
- Production environment readiness

### Risks / Notes
- Client secrets may have expiration policies — confirm rotation strategy
- Ensure production app registration is not shared with non-prod
- Confirm least-privilege permissions before approval

### Definition of Done
- Production credentials acquired and verified
- Secrets securely stored and referenced by pipeline
- Item reviewed and accepted by Dev / Ops


---

## PBI 2 — Create Local Mock Endpoint for MuleSoft API

**Type:** Product Backlog Item / Development Enablement

### Description
Create a local mock implementation of the MuleSoft API to support EVAuto development and testing without requiring access to the production MuleSoft environment.

The mock endpoint will simulate expected request/response behavior and enable local runs, CI validation, and early integration testing.

### Scope / Requirements
- Implement a local mock API endpoint that mirrors MuleSoft contracts
- Support core request/response shapes used by EVAuto
- Allow easy switching between:
  - Local mock endpoint
  - Non-prod / prod MuleSoft API
- Mock should be deterministic and developer-friendly

### Acceptance Criteria
- [ ] Local mock endpoint is available when running EVAuto locally
- [ ] Mock endpoint matches MuleSoft API routes used by the app
- [ ] Request/response schemas align with documented MuleSoft contracts
- [ ] Sample/static responses provided for common scenarios
- [ ] Environment-based switching implemented (local vs real API)
- [ ] No production credentials required to run locally
- [ ] Documentation added for enabling/disabling mock mode

### Out of Scope
- Full MuleSoft behavior parity
- Performance/load testing
- Production MuleSoft configuration changes

### Dependencies
- MuleSoft API contract / documentation
- Agreement on response samples and edge cases
- Local environment configuration standards

### Risks / Notes
- Mock responses may drift from real API — periodic validation required
- Ensure mock data is clearly marked as non-production
- Keep mock lightweight to avoid becoming a second system

### Definition of Done
- Local mock endpoint operational
- EVAuto can run end-to-end locally using mock data
- Switching between mock and real API is configuration-driven
- Item reviewed and accepted by Dev