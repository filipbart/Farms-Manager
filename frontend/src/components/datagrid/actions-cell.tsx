import React from "react";
import { Button, Stack } from "@mui/material";

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
  const handleEdit = () => {
    if (onEdit) {
      onEdit(params.row);
    }
  };

  const handleDelete = () => {
    if (onDelete) {
      onDelete(params.row.id);
    }
  };

  return (
    <Stack direction="row" spacing={1}>
      {onEdit && (
        <Button
          variant="outlined"
          size="small"
          onClick={handleEdit}
          disabled={disabledEdit}
        >
          Edytuj
        </Button>
      )}
      {onDelete && (
        <Button
          variant="outlined"
          size="small"
          color="error"
          onClick={handleDelete}
          disabled={disabledDelete}
        >
          Usuń
        </Button>
      )}
    </Stack>
  );
};

export default ActionsCell;
