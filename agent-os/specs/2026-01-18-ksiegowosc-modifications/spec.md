# Specification: Księgowość Modifications

## Goal
Poprawa funkcjonalności i dodanie nowych możliwości w module księgowości, w tym naprawa błędów UI, usprawnienie walidacji, automatyzacja procesów oraz synchronizacja danych między modułami.

## User Stories
- As a księgowy, I want to see all invoice details correctly so that I can review and process invoices without missing information
- As a user with approval permissions, I want to efficiently process multiple invoices in sequence so that I can complete my work faster
- As an administrator, I want to track all status changes for audit purposes so that I maintain data integrity and compliance

## Specific Requirements

**Bug Fixes - Data Display Issues**
- Fix "Ubojnia" field visibility in Sales module invoice details modal
- Fix "Typ wydatku" field visibility in Production Expenses module invoice details modal  
- Remove grey background styling from module-specific forms
- Implement automatic view refresh after "Przekaż do biura" action completes successfully

**Validation & Business Logic**
- Add employee assignment validation for all invoice types before allowing acceptance
- Implement automatic contractor creation in KSeFSynchronizationJob when NIP not found
- Add expense type assignment modal for new contractors in Production Expenses module
- Enable payment status override for already accepted invoices with status changes

**New Features**
- Add "Przeglądaj faktury" mode for sequential processing of filtered invoice lists
- Implement bidirectional payment status synchronization between modules (Feeds/Sales ↔ Accounting)
- Add S3 file upload functionality for invoice attachments
- Implement payment date handling with default logic based on payment method
- Create audit logging system for invoice status changes (backend only)

**Technical Requirements**
- All data display fixes must work across all modules (Sales, Production Expenses, Gas, Feeds)
- Sequential invoice processing should include interruption capability
- Payment date defaults: Cash → Invoice Date, Transfer → Current Date
- Audit logs to track: Accept, Hold, Reject, Transfer to Office actions
- All features should respect existing user permissions and roles

## Visual Design
No visual assets provided. Development should follow existing UI patterns in the application.

## Existing Code to Leverage

**Invoice Details Modal**
- `frontend/src/components/modals/accounting/invoice-details-modal.tsx` - Base modal structure to extend
- Contains existing invoice detail display patterns and form layouts

**File Upload Components**  
- `frontend/src/components/modals/accounting/upload-invoice-modal.tsx` - S3 upload patterns
- Similar upload components in feeds, gas, sales, and expenses modules for reference

**Payment Status Logic**
- `backend/src/FarmsManager.Domain/Aggregates/AccountingAggregate/Enums/KSeFPaymentStatus.cs` - Existing payment status enums
- `backend/src/FarmsManager.Domain/Aggregates/FeedAggregate/Enums/FeedPaymentStatus.cs` - Cross-module payment patterns

**Synchronization Job**
- `backend/src/FarmsManager.Infrastructure/BackgroundJobs/KSeFSynchronizationJob.cs` - Extend for contractor creation logic
- Contains existing KSeF integration and contractor handling patterns

**API Endpoints**
- `frontend/src/common/ApiUrl.ts` lines 370-408 - Existing accounting API structure
- Extend with new endpoints for attachments, audit logs, and status synchronization

## Out of Scope
- Audit log viewer UI (backend logging only)
- External integrations beyond KSeF
- Changes to modules other than Accounting
- New user roles or permission systems
- Database schema changes beyond what's necessary for new features
