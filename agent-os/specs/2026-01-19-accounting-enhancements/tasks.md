# Task Breakdown: Accounting Enhancements

## Overview
Total Tasks: 22

## Task List

### Database & Domain Layer

#### Task Group 1: Entities and Duplication Logic
**Dependencies:** None

- [x] 1.0 Complete database & domain layer
  - [x] 1.1 Add `KSeFInvoiceAttachmentEntity` if not already present or extend for additional attachments
  - [x] 1.2 Update `KSeFInvoiceEntity` with collection of additional attachments and subfolder path logic
  - [x] 1.3 Add backend validation logic for duplicates in `SaveAccountingInvoiceCommandHandler`
  - [x] 1.4 Implement farm matching logic in a domain service or within the entity/handler
  - [x] 1.5 Verify database migrations and associations work correctly

**Acceptance Criteria:**
- `KSeFInvoiceEntity` supports multiple attachments.
- Duplication check (SellerNip/BuyerNip + InvoiceNumber) works at the database level.
- Migrations are generated and applied.

### API & Application Layer

#### Task Group 2: Commands, DTOs, and Services
**Dependencies:** Task Group 1

- [x] 2.0 Complete API & Application layer
  - [x] 2.1 Update `SaveAccountingInvoiceDto` to include optional attachments
  - [x] 2.2 Update `SaveAccountingInvoiceCommandHandler` to handle file saving to dedicated subfolders (`accounting/attachments/{invoiceId}/`)
  - [x] 2.3 Modify `UploadInvoicesCommand` to accept and process the `ModuleType` parameter for AI steering
  - [x] 2.4 Update PDF download endpoint to serve original file for non-KSeF invoices
  - [x] 2.5 Implement `ModuleType` filtering in `GetKSeFInvoicesQuery`
  - [x] 2.6 Add endpoint to retrieve/download additional attachments per invoice

**Acceptance Criteria:**
- API accepts `moduleType` during upload.
- Duplicate invoices return a 400 Bad Request with a clear message.
- Attachments are stored in the correct S3/FileSystem path.
- Filters by `ModuleType` return correct data.

### Frontend Components

#### Task Group 3: UI Enhancements and Modals
**Dependencies:** Task Group 2

- [x] 3.0 Complete UI components
  - [x] 3.1 Update `upload-invoice-modal.tsx` with `ModuleType` selection dropdown
  - [x] 3.2 Implement independent scrolling in `save-accounting-invoice-modal.tsx` (Split layout: Preview | Form)
  - [x] 3.3 Integrate `FilePreview` in `save-accounting-invoice-modal.tsx` and `invoice-details-modal.tsx` for non-KSeF sources
  - [x] 3.4 Implement auto-assignment of Farm in `save-accounting-invoice-modal.tsx` when business entity has only one farm
  - [x] 3.5 Add attachment upload field to `save-accounting-invoice-modal.tsx`
  - [x] 3.6 Update `accounting/index.tsx` list with `ModuleType` filter
  - [x] 3.7 Update `sales/index.tsx` list with `ModuleType` filter (Clarified: not needed per user - filter added to accounting only)
  - [x] 3.8 Add attachment download/preview list to `invoice-details-modal.tsx`

**Acceptance Criteria:**
- Modal layout allows independent scrolling.
- Non-KSeF invoices show original file preview via `FilePreview`.
- `ModuleType` filters work on both Accounting and Sales pages.
- Attachments can be uploaded during invoice creation and downloaded later.

### Testing & Verification

#### Task Group 4: Final Verification
**Dependencies:** Task Groups 1-3

- [x] 4.0 Final verification
  - [x] 4.1 Test "Feeds" module AI extraction scenario (manual check of fields)
  - [x] 4.2 Verify duplicate prevention upon clicking "Save"
  - [x] 4.3 Verify PDF download serves original file for manual uploads
  - [x] 4.4 Verify farm auto-assignment works as expected

**Acceptance Criteria:**
- All functional requirements from `requirements.md` are met.
- No regressions in KSeF invoice handling.

## Execution Order
1. Database & Domain Layer (Task Group 1)
2. API & Application Layer (Task Group 2)
3. Frontend Components (Task Group 3)
4. Final Verification (Task Group 4)
