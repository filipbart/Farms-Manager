import { Box, Paper, Typography, TextField, IconButton } from "@mui/material";
import { useState } from "react";
import {
  MdEdit,
  MdDelete,
  MdSave,
  MdCancel,
  MdDragIndicator,
} from "react-icons/md";

interface NoteModel {
  id: string;
  title: string;
  content: string;
}

interface NoteCardProps {
  note: NoteModel;
  onUpdate: (id: string, title: string, content: string) => void;
  onDelete: (id: string) => void;
  dndAttributes?: any;
  dndListeners?: any;
}

const NoteCard: React.FC<NoteCardProps> = ({
  note,
  onUpdate,
  onDelete,
  dndAttributes,
  dndListeners,
}) => {
  const [isEditing, setIsEditing] = useState(false);
  const [editedTitle, setEditedTitle] = useState(note.title);
  const [editedContent, setEditedContent] = useState(note.content);

  const handleSave = () => {
    onUpdate(note.id, editedTitle, editedContent);
    setIsEditing(false);
  };

  const handleCancel = () => {
    setEditedTitle(note.title);
    setEditedContent(note.content);
    setIsEditing(false);
  };

  return (
    <Paper
      elevation={2}
      sx={{
        p: 2,
        display: "flex",
        flexDirection: "column",
        height: "100%",
        minHeight: 150,
        overflow: "hidden",
      }}
    >
      {isEditing ? (
        <>
          <TextField
            variant="standard"
            placeholder="Tytuł"
            value={editedTitle}
            onChange={(e) => setEditedTitle(e.target.value)}
            sx={{ mb: 1 }}
          />
          <TextField
            variant="standard"
            placeholder="Treść notatki..."
            value={editedContent}
            onChange={(e) => setEditedContent(e.target.value)}
            multiline
            rows={4}
            sx={{ flexGrow: 1 }}
          />
        </>
      ) : (
        <Box sx={{ flexGrow: 1, display: "flex", flexDirection: "column" }}>
          <Box
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              cursor: "grab",
              mb: 1,
            }}
            {...dndAttributes}
            {...dndListeners}
          >
            <Typography
              variant="h6"
              sx={{
                wordBreak: "break-word",
                flexGrow: 1,
              }}
            >
              {note.title}
            </Typography>
            <MdDragIndicator />
          </Box>
          <Typography
            variant="body2"
            sx={{
              flexGrow: 1,
              whiteSpace: "pre-wrap",
              wordBreak: "break-word",
            }}
          >
            {note.content}
          </Typography>
        </Box>
      )}

      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        mt={1}
      >
        <Box></Box>
        <Box>
          {isEditing ? (
            <>
              <IconButton size="small" onClick={handleSave} title="Zapisz">
                <MdSave />
              </IconButton>
              <IconButton size="small" onClick={handleCancel} title="Anuluj">
                <MdCancel />
              </IconButton>
            </>
          ) : (
            <>
              <IconButton
                size="small"
                onClick={() => setIsEditing(true)}
                title="Edytuj"
              >
                <MdEdit />
              </IconButton>
              <IconButton
                size="small"
                onClick={() => onDelete(note.id)}
                title="Usuń"
                color="error"
              >
                <MdDelete />
              </IconButton>
            </>
          )}
        </Box>
      </Box>
    </Paper>
  );
};

export default NoteCard;
