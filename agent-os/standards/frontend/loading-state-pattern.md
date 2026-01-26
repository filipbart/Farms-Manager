---
# Loading State Pattern

Consistent loading state management for async operations:

```tsx
const Modal: React.FC<ModalProps> = () => {
  const [loading, setLoading] = useState(false);

  const handleSave = async (data: FormData) => {
    if (loading) return; // Always first line to prevent double submissions
    setLoading(true);
    
    try {
      await handleApiResponse(() => Service.createItem(data));
    } finally {
      setLoading(false); // Always reset loading state
    }
  };

  return (
    <LoadingButton 
      loading={loading} 
      disabled={loading} 
      type="submit"
    >
      Save
    </LoadingButton>
  );
};
```

- **Guard Pattern** - `if (loading) return` is always first line in async functions
- **LoadingButton** - Custom component that shows spinner during loading
- **Disabled State** - Disable buttons and form elements during loading
- **Finally Block** - Always reset loading state in finally to prevent stuck loading
- **Height Default** - LoadingButton height="0" matches normal Button height
