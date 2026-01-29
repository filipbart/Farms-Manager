---
# Dialog Close Prevention Pattern

AppDialog wrapper prevents accidental modal closing:

```tsx
const AppDialog: React.FC<AppDialogProps> = ({ children, onClose, ...props }) => {
  const handleClose = (_event: object, reason: "backdropClick" | "escapeKeyDown") => {
    if (reason !== "backdropClick") {
      onClose(); // Only allow escape key closing
    }
  };

  return <Dialog onClose={handleClose} {...props}>{children}</Dialog>;
};
```

- **Backdrop Click Blocked** - Prevents closing by clicking outside modal
- **Escape Key Allowed** - Escape is considered intentional user action
- **Form Protection** - Prevents data loss from accidental clicks
- **Used Everywhere** - AppDialog used for forms, file previews, and other dialogs
- **Consistent UX** - Users must explicitly close modals through buttons or escape

```tsx
// Usage in modals
<AppDialog open={open} onClose={close} fullWidth maxWidth="sm">
  <form>...</form>
</AppDialog>
```
