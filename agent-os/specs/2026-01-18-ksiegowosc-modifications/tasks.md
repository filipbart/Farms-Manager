# Task Breakdown: Księgowość Modifications

## Overview
Total Tasks: 48

## Task List

### Backend Infrastructure

#### Task Group 1: Database Schema & Models
**Dependencies:** None

- [ ] 1.0 Complete database layer for new features
  - [ ] 1.1 Write 2-8 focused tests for audit logging functionality
    - Test audit log creation for invoice status changes
    - Test audit log retrieval for admin users
    - Test audit log data integrity and required fields
  - [ ] 1.2 Create AuditLogEntity model
    - Fields: Id, InvoiceId, Action, PreviousStatus, NewStatus, UserId, Timestamp, UserName
    - Validations: Required fields, valid status transitions
    - Reuse pattern from: existing entity patterns in Domain layer
  - [ ] 1.3 Create migration for AuditLog table
    - Add indexes for: InvoiceId, UserId, Timestamp
    - Foreign keys: InvoiceId → KSeFInvoiceEntity, UserId → User
  - [ ] 1.4 Add PaymentDate field to KSeFInvoiceEntity
    - Migration to add nullable PaymentDate datetime field
    - Update entity model to include new field
  - [ ] 1.5 Create InvoiceAttachmentEntity model
    - Fields: Id, InvoiceId, FileName, FilePath, FileSize, UploadedAt, UploadedBy
    - Validations: Required fields, file size limits
  - [ ] 1.6 Create migration for InvoiceAttachment table
    - Add indexes for: InvoiceId, UploadedAt
    - Foreign keys: InvoiceId → KSeFInvoiceEntity, UploadedBy → User
  - [ ] 1.7 Ensure database layer tests pass
    - Run ONLY the 2-8 tests written in 1.1
    - Verify migrations run successfully
    - Do NOT run the entire test suite at this stage

**Acceptance Criteria:**
- The 2-8 tests written in 1.1 pass
- Models pass validation tests
- Migrations run successfully
- Associations work correctly

#### Task Group 2: Backend Services & Logic
**Dependencies:** Task Group 1

- [ ] 2.0 Complete backend services layer
  - [ ] 2.1 Write 2-8 focused tests for audit logging service
    - Test audit log creation for different status changes
    - Test audit log retrieval and filtering
    - Test permission checks for audit log access
  - [ ] 2.2 Create AuditLoggingService
    - Methods: LogStatusChange, GetAuditLogs, GetInvoiceAuditHistory
    - Follow pattern from: existing service patterns in Application layer
    - Implement proper error handling and validation
  - [ ] 2.3 Extend KSeFSynchronizationJob for contractor creation
    - Add automatic contractor creation when NIP not found
    - Add expense type assignment modal logic
    - Integrate with existing synchronization workflow
  - [ ] 2.4 Create PaymentStatusSynchronizationService
    - Methods: SyncPaymentStatusToAccounting, SyncPaymentStatusFromAccounting
    - Handle bidirectional sync between Feeds/Sales and Accounting
    - Implement conflict resolution logic
  - [ ] 2.5 Create FileUploadService for S3 attachments
    - Methods: UploadAttachment, DeleteAttachment, GetAttachmentUrl
    - Reuse pattern from: existing S3 upload implementations
    - Implement file validation and security checks
  - [ ] 2.6 Update invoice validation logic
    - Add employee assignment validation before acceptance
    - Update SaveAccountingInvoiceCommand validation
    - Integrate with existing validation patterns
  - [ ] 2.7 Ensure backend service tests pass
    - Run ONLY the 2-8 tests written in 2.1
    - Verify critical service operations work
    - Do NOT run the entire test suite at this stage

**Acceptance Criteria:**
- The 2-8 tests written in 2.1 pass
- All service operations work correctly
- Proper validation enforced
- Consistent error handling

#### Task Group 3: API Endpoints
**Dependencies:** Task Group 2

- [ ] 3.0 Complete API layer
  - [ ] 3.1 Write 2-8 focused tests for new API endpoints
    - Test attachment upload/download endpoints
    - Test audit log retrieval endpoints
    - Test payment status synchronization endpoints
  - [ ] 3.2 Add attachment endpoints to AccountingController
    - POST /accounting/invoices/{id}/attachments - Upload attachment
    - GET /accounting/invoices/{id}/attachments/{attachmentId} - Download attachment
    - DELETE /accounting/invoices/{id}/attachments/{attachmentId} - Delete attachment
  - [ ] 3.3 Add audit log endpoints to AccountingController
    - GET /accounting/invoices/{id}/audit-logs - Get invoice audit history
    - Implement admin-only access restriction
  - [ ] 3.4 Add payment status sync endpoints
    - POST /accounting/invoices/{id}/sync-payment-status - Sync payment status
    - Handle both directions of synchronization
  - [ ] 3.5 Update existing invoice endpoints
    - Add PaymentDate to invoice update responses
    - Add attachment metadata to invoice details
    - Add audit log summary to invoice details
  - [ ] 3.6 Update ApiUrl.ts with new endpoints
    - Add AccountingUploadAttachment, AccountingDownloadAttachment
    - Add AccountingGetAuditLogs, AccountingSyncPaymentStatus
  - [ ] 3.7 Ensure API layer tests pass
    - Run ONLY the 2-8 tests written in 3.1
    - Verify critical CRUD operations work
    - Do NOT run the entire test suite at this stage

**Acceptance Criteria:**
- The 2-8 tests written in 3.1 pass
- All CRUD operations work
- Proper authorization enforced
- Consistent response format

### Frontend Components

#### Task Group 4: UI Bug Fixes
**Dependencies:** Task Group 3

- [ ] 4.0 Complete UI bug fixes
  - [ ] 4.1 Write 2-8 focused tests for invoice details modal
    - Test "Ubojnia" field display for Sales module
    - Test "Typ wydatku" field display for Production Expenses module
    - Test modal data loading and display
  - [ ] 4.2 Fix "Ubojnia" field visibility in invoice details modal
    - Update data loading logic to fetch from correct entity
    - Ensure proper display in Sales module context
    - Reference: existing invoice-details-modal.tsx structure
  - [ ] 4.3 Fix "Typ wydatku" field visibility in invoice details modal
    - Update data loading logic to fetch from ProductionExpenses entity
    - Ensure proper display in Production Expenses module context
  - [ ] 4.4 Remove grey background styling from module forms
    - Update CSS classes in invoice details modal
    - Follow existing design system patterns
    - Ensure consistency across all module forms
  - [ ] 4.5 Implement automatic view refresh after "Przekaż do biura"
    - Add success callback to transfer-to-office action
    - Refresh invoice list and modal state
    - Reference existing refresh patterns in accounting page
  - [ ] 4.6 Ensure UI bug fix tests pass
    - Run ONLY the 2-8 tests written in 4.1
    - Verify modal displays data correctly
    - Do NOT run the entire test suite at this stage

**Acceptance Criteria:**
- The 2-8 tests written in 4.1 pass
- Modal displays all fields correctly
- Consistent styling across forms
- Proper view refresh functionality

#### Task Group 5: Validation & Business Logic UI
**Dependencies:** Task Group 4

- [ ] 5.0 Complete validation and business logic UI
  - [ ] 5.1 Write 2-8 focused tests for validation logic
    - Test employee assignment validation before acceptance
    - Test payment status override functionality
    - Test expense type assignment modal
  - [ ] 5.2 Add employee assignment validation to invoice details modal
    - Disable "Zaakceptuj" button when employee not assigned
    - Show validation message to user
    - Reference existing validation patterns in forms
  - [ ] 5.3 Add payment status override functionality
    - Add button to override payment status for accepted invoices
    - Show only when invoice accepted and status changed
    - Implement confirmation modal for override action
  - [ ] 5.4 Add expense type assignment modal for new contractors
    - Create modal component for expense type selection
    - Integrate with KSeFSynchronizationJob workflow
    - Reference existing modal patterns in accounting module
  - [ ] 5.5 Add payment date handling with default logic
    - Add PaymentDate field to invoice details modal
    - Implement default logic: Cash → Invoice Date, Transfer → Current Date
    - Allow manual date editing
  - [ ] 5.6 Ensure validation UI tests pass
    - Run ONLY the 2-8 tests written in 5.1
    - Verify validation works correctly
    - Do NOT run the entire test suite at this stage

**Acceptance Criteria:**
- The 2-8 tests written in 5.1 pass
- Validation prevents invalid actions
- Override functionality works correctly
- Payment date logic implemented

#### Task Group 6: New Features UI
**Dependencies:** Task Group 5

- [ ] 6.0 Complete new features UI
  - [ ] 6.1 Write 2-8 focused tests for new features
    - Test "Przeglądaj faktury" mode functionality
    - Test file upload and attachment display
    - Test sequential invoice processing
  - [ ] 6.2 Add "Przeglądaj faktury" mode to accounting page
    - Add button to start sequential processing mode
    - Implement iteration over filtered invoice list
    - Add interruption capability for user
    - Reference existing list processing patterns
  - [ ] 6.3 Add file upload functionality to invoice details modal
    - Add attachment upload section
    - Display existing attachments with download/delete options
    - Reference: upload-invoice-modal.tsx patterns
  - [ ] 6.4 Implement sequential invoice processing UI
    - Add navigation controls for next/previous invoice
    - Show progress indicator for batch processing
    - Auto-advance after main action completion
  - [ ] 6.5 Add attachment management UI
    - Display attachment list with file info
    - Add download and delete actions
    - Implement file size and type validation
  - [ ] 6.6 Ensure new features UI tests pass
    - Run ONLY the 2-8 tests written in 6.1
    - Verify new features work correctly
    - Do NOT run the entire test suite at this stage

**Acceptance Criteria:**
- The 2-8 tests written in 6.1 pass
- Sequential processing works correctly
- File upload functionality operational
- UI matches existing design patterns

### Testing & Integration

#### Task Group 7: Test Review & Gap Analysis
**Dependencies:** Task Groups 1-6

- [ ] 7.0 Review existing tests and fill critical gaps only
  - [ ] 7.1 Review tests from Task Groups 1-6
    - Review the 2-8 tests written by database layer (Task 1.1)
    - Review the 2-8 tests written by backend services (Task 2.1)
    - Review the 2-8 tests written by API layer (Task 3.1)
    - Review the 2-8 tests written by UI bug fixes (Task 4.1)
    - Review the 2-8 tests written by validation UI (Task 5.1)
    - Review the 2-8 tests written by new features UI (Task 6.1)
    - Total existing tests: approximately 12-48 tests
  - [ ] 7.2 Analyze test coverage gaps for accounting modifications
    - Identify critical workflows lacking test coverage
    - Focus on end-to-end invoice processing workflows
    - Prioritize integration points between modules
  - [ ] 7.3 Write up to 10 additional strategic tests maximum
    - Add tests for complete invoice processing workflow
    - Add tests for payment status synchronization scenarios
    - Add tests for audit logging integrity
    - Add tests for file upload and attachment management
    - Add tests for sequential invoice processing edge cases
  - [ ] 7.4 Run feature-specific tests only
    - Run ONLY tests related to accounting modifications
    - Expected total: approximately 22-58 tests maximum
    - Do NOT run the entire application test suite
    - Verify critical workflows pass

**Acceptance Criteria:**
- All feature-specific tests pass (approximately 22-58 tests total)
- Critical accounting workflows are covered
- No more than 10 additional tests added when filling in gaps
- Testing focused exclusively on accounting modifications

## Execution Order

Recommended implementation sequence:
1. Database Layer (Task Group 1)
2. Backend Services (Task Group 2)
3. API Layer (Task Group 3)
4. UI Bug Fixes (Task Group 4)
5. Validation & Business Logic UI (Task Group 5)
6. New Features UI (Task Group 6)
7. Test Review & Gap Analysis (Task Group 7)

## Notes
- This implementation focuses on modifying existing accounting module functionality
- Heavy reuse of existing patterns from feeds, sales, and expense modules
- Sequential processing allows for staged deployment if needed
- All changes maintain backward compatibility with existing data
