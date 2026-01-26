---
# Modal Component Pattern

All modals follow consistent structure using AppDialog wrapper:

```tsx
interface ModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const Modal: React.FC<ModalProps> = ({ open, onClose, onSave }) => {
  const [loading, setLoading] = useState(false);
  const { register, handleSubmit, formState: { errors }, reset } = useForm<FormData>();

  const close = () => {
    reset();
    onClose();
  };

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="sm">
      <DialogTitle>Title</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>...</DialogContent>
        <DialogActions>...</DialogActions>
      </form>
    </AppDialog>
  );
};
```

- **AppDialog Wrapper** - Prevents backdrop click closing (only escape/confirm allowed)
- **Form Reset** - Always reset form on close to prevent stale data
- **Loading State** - Use useState + LoadingButton for async operations
- **Should Use AppDialog** - All modals should use AppDialog, though exceptions exist in codebase
