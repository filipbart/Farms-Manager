# Specification: Accounting Enhancements

## Goal
Enhance the accounting module by integrating module-aware AI data extraction, improving the user interface for invoice verification, and adding robustness through duplicate validation and additional attachments.

## User Stories
- As an accounting user, I want to select a target module (e.g., Feeds, Gas) before uploading an invoice so that the system automatically extracts data specific to that module's needs.
- As an accounting user, I want to see the original uploaded file alongside the data entry form for non-KSeF invoices so that I can easily verify and correct any mistakes.
- As a system administrator, I want the system to prevent duplicate invoices from being saved so that our financial records remain accurate and clean.

## Specific Requirements

**Module Selection in Upload**
- Add a "Module" selection dropdown to `upload-invoice-modal.tsx`.
- Include options from `ModuleType` (Feeds, Gas, ProductionExpenses, Sales, etc.).
- Update `AccountingService.uploadInvoices` to accept the selected `ModuleType`.
- Ensure the backend receives this type to steer the AI extraction process.

**AI Data Extraction Enhancement**
- Update backend AI extraction logic to prioritize module-specific fields when a module is selected.
- For "Feeds", extract quantities, unit prices, and specific vendor details.
- For "Gas", extract liters/quantities and contractor information.
- For "Sales", extract buyer details and line items.
- Fall back to standard `KSeFInvoiceEntity` fields for "None" or other modules.

**Independent Scrolling in Modal**
- Modify the layout of `save-accounting-invoice-modal.tsx` to use a two-column split (Preview | Form).
- Apply `overflow-y-auto` and `h-full` (or equivalent fixed height) to both columns.
- Ensure scrolling the form on the right does not affect the preview on the left, and vice versa.

**File Preview for Non-KSeF Invoices**
- In `save-accounting-invoice-modal.tsx` and `invoice-details-modal.tsx`, check the invoice source.
- If the invoice is NOT from KSeF, use the `@/frontend/src/components/common/file-preview.tsx` component to display the actual uploaded file.
- This preview should completely replace the generated visualization for manual uploads.

**Automatic Farm Assignment**
- In `save-accounting-invoice-modal.tsx`, when a business entity is identified, check if it has only one associated farm.
- If exactly one farm is found, automatically select it in the form.
- The field must remain editable so the user can change it if necessary.

**Additional Attachments**
- Add an "Attachments" section to `save-accounting-invoice-modal.tsx` for optional file uploads.
- Backend must store these files in a dedicated subfolder (e.g., `accounting/attachments/{invoiceId}/`).
- Update `KSeFInvoiceEntity` or a related entity to track these additional attachments.
- Enable viewing and downloading these attachments from the invoice details view.

**Duplicate Validation**
- Implement duplicate check logic in `SaveAccountingInvoiceCommandHandler`.
- Validation must be triggered upon clicking the "Save" button.
- Check for existing invoices with the same `SellerNip` (or `BuyerNip` for sales) and `InvoiceNumber`.
- Return a clear error message to the frontend if a duplicate is found.

**Accounting & Sales Filters**
- Add a new filter for `ModuleType` in `frontend/src/pages/accounting/index.tsx`.
- Add a similar `ModuleType` filter to `frontend/src/pages/sales/index.tsx`.
- Update the download PDF action in `accounting/index.tsx` to serve the original uploaded file instead of a generated visualization for manual invoices.

## Existing Code to Leverage

**FilePreview Component**
- Re-use `@/frontend/src/components/common/file-preview.tsx` for displaying uploaded PDF/images.
- Ensure `maxHeight` and other props are correctly adjusted for the new independent scrolling layout.

**SaveAccountingInvoiceCommand**
- Extend the `SaveAccountingInvoiceDto` in `@/backend/src/FarmsManager.Application/Commands/Accounting/SaveAccountingInvoiceCommand.cs` to handle attachments and validation.
- Re-use the `CheckForDuplicatesAsync` pattern already present in the handler.

**AccountingService**
- Update `@/frontend/src/services/accounting-service.ts` to support the new `moduleType` parameter in upload methods and the new filter in list methods.

**ModuleType Enum**
- Reference `@/backend/src/FarmsManager.Domain/Aggregates/AccountingAggregate/Enums/ModuleType.cs` for all module-related logic and filters.

## Out of Scope
- Changing the visualization for KSeF invoices (they will continue to use the generated view).
- Real-time duplicate validation as the user types (validation occurs on save).
- Universal file size limits (will follow global system defaults if any).
- Multi-select for the Module filter in Sales (simple dropdown/single-select is sufficient).
