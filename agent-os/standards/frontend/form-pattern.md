---
# Form Component Pattern

All forms use react-hook-form with consistent API handling:

```tsx
const Modal: React.FC<ModalProps> = ({ open, onClose, onSave }) => {
  const [loading, setLoading] = useState(false);
  const { register, handleSubmit, formState: { errors }, reset } = useForm<FormData>();

  const handleSave = async (data: FormData) => {
    if (loading) return;
    setLoading(true);
    
    await handleApiResponse(
      () => Service.createItem(data),
      () => {
        toast.success("Success message");
        onSave(); // Usually called after success
        onClose();
      },
      undefined,
      "Error message"
    );
    
    setLoading(false);
  };

  return (
    <form onSubmit={handleSubmit(handleSave)}>
      <TextField {...register("field", { required: "Field required" })} />
      <LoadingButton loading={loading} type="submit">Save</LoadingButton>
    </form>
  );
};
```

- **react-hook-form** - Standard form library with validation
- **handleApiResponse** - Standard wrapper for API calls with toast notifications
- **Loading State** - Prevent double submissions during API calls
- **onSave Callback** - Usually called after success to refresh parent data
- **Form Reset** - Reset form on close to clear validation errors
