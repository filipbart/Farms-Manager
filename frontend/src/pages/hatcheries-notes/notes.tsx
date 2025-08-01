import { Box, Button, Typography, CircularProgress } from "@mui/material";
import { useEffect, useState, useCallback } from "react";
import { toast } from "react-toastify";
import NoteCard from "../../components/common/note-card";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { HatcheriesService } from "../../services/hatcheries-service";
import type { HatcheryNote } from "../../models/hatcheries/hatcheries-notes";

const HatcheriesNotesPanel: React.FC = () => {
  const [notes, setNotes] = useState<HatcheryNote[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchNotes = useCallback(async () => {
    setLoading(true);
    await handleApiResponse(
      () => HatcheriesService.getNotes(),
      (data) => {
        setNotes(data.responseData?.items ?? []);
      },
      () => {
        toast.error("Błąd podczas pobierania notatek");
      }
    );
    setLoading(false);
  }, []);

  useEffect(() => {
    fetchNotes();
  }, [fetchNotes]);

  const handleAddNote = async () => {
    await handleApiResponse(
      () => HatcheriesService.addNote({ title: "Nowa notatka", content: "" }),
      () => {
        toast.success("Dodano nową notatkę");
        fetchNotes();
      },
      () => {
        toast.error("Nie udało się dodać notatki");
      }
    );
  };

  const handleUpdateNote = async (
    id: string,
    title: string,
    content: string
  ) => {
    await handleApiResponse(
      () => HatcheriesService.updateNote(id, { title, content }),
      () => {
        toast.success("Notatka została zaktualizowana");
        fetchNotes();
      },
      () => {
        toast.error("Nie udało się zaktualizować notatki");
      }
    );
  };

  const handleDeleteNote = async (id: string) => {
    await handleApiResponse(
      () => HatcheriesService.deleteNote(id),
      () => {
        toast.success("Notatka została usunięta");
        fetchNotes();
      },
      () => {
        toast.error("Nie udało się usunąć notatki");
      }
    );
  };

  return (
    <Box p={2}>
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        mb={2}
      >
        <Typography variant="h6">Notatki</Typography>
        <Button variant="contained" onClick={handleAddNote} disabled={loading}>
          Dodaj notatkę
        </Button>
      </Box>

      {loading ? (
        <Box display="flex" justifyContent="center" mt={4}>
          <CircularProgress />
        </Box>
      ) : (
        <Box
          sx={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fill, minmax(250px, 1fr))",
            gap: 2,
          }}
        >
          {notes.length > 0 ? (
            notes.map((note) => (
              <NoteCard
                key={note.id}
                note={note}
                onUpdate={handleUpdateNote}
                onDelete={handleDeleteNote}
              />
            ))
          ) : (
            <Typography>Brak notatek do wyświetlenia.</Typography>
          )}
        </Box>
      )}
    </Box>
  );
};

export default HatcheriesNotesPanel;
