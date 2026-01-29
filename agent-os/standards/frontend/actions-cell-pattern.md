---
# DataGrid Actions Cell Pattern

Standardized action buttons for DataGrid rows:

```tsx
interface ActionsCellProps {
  params: any;
  onDelete?: (id: string) => void;
  onEdit?: (row: any) => void;
  disabledEdit?: boolean;
  disabledDelete?: boolean;
}

const ActionsCell: React.FC<ActionsCellProps> = ({
  params,
  onEdit,
  onDelete,
  disabledEdit = false,
  disabledDelete = false,
}) => {
  const [open, setOpen] = useState(false);

  const handleConfirmDelete = () => {
    if (onDelete) {
      onDelete(params.row.id);
    }
    handleClose();
  };

  return (
    <>
      <Stack direction="row" spacing={1}>
        {onEdit && (
          <Button variant="outlined" size="small" onClick={() => onEdit(params.row)}>
            Edytuj
          </Button>
        )}
        {onDelete && (
          <Button variant="outlined" size="small" color="error" onClick={() => setOpen(true)}>
            Usuń
          </Button>
        )}
      </Stack>

      <Dialog open={open} onClose={() => setOpen(false)}>
        <DialogTitle>Potwierdzenie usunięcia</DialogTitle>
        <DialogContent>Czy na pewno chcesz usunąć ten element?</DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Anuluj</Button>
          <Button onClick={handleConfirmDelete} color="error" autoFocus>Usuń</Button>
        </DialogActions>
      </Dialog>
    </>
  );
};
```

- **Standard Usage** - Always used in DataGrid for row actions (rare exceptions)
- **Edit/Delete Buttons** - Consistent styling and disabled states
- **Confirm Dialog** - Built-in confirmation for delete operations
- **MUI Dialog** - Can use standard Dialog (doesn't require AppDialog)
- **Optional Actions** - Edit and delete are optional based on permissions
