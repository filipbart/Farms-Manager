import { Box, Button, Typography, CircularProgress } from "@mui/material";
import { useEffect, useState, useCallback } from "react";
import { toast } from "react-toastify";
import { DndContext, closestCenter, type DragEndEvent } from "@dnd-kit/core";
import {
  arrayMove,
  SortableContext,
  rectSortingStrategy,
  useSortable,
} from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import NoteCard from "../../components/common/note-card";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { HatcheriesService } from "../../services/hatcheries-service";
import type { HatcheryNote } from "../../models/hatcheries/hatcheries-notes";

const SortableNote: React.FC<{
  note: HatcheryNote;
  onUpdate: (id: string, title: string, content: string) => Promise<void>;
  onDelete: (id: string) => Promise<void>;
}> = ({ note, onUpdate, onDelete }) => {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: note.id });

  const style: React.CSSProperties = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
    zIndex: isDragging ? 10 : "auto",
    position: "relative",
  };

  return (
    <Box ref={setNodeRef} style={style}>
      <NoteCard
        note={note}
        onUpdate={onUpdate}
        onDelete={onDelete}
        dndAttributes={attributes}
        dndListeners={listeners}
      />
    </Box>
  );
};

const HatcheriesNotesPanel: React.FC = () => {
  const [notes, setNotes] = useState<HatcheryNote[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchNotes = useCallback(async () => {
    setLoading(true);
    await handleApiResponse(
      () => HatcheriesService.getNotes(),
      (data) => {
        // Zakładamy, że notatki mają pole 'order' i są sortowane po nim
        const sortedNotes = (data.responseData?.items ?? []).sort(
          (a, b) => (a.order ?? 0) - (b.order ?? 0)
        );
        setNotes(sortedNotes);
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

  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event;

    if (over && active.id !== over.id) {
      const oldIndex = notes.findIndex((n) => n.id === active.id);
      const newIndex = notes.findIndex((n) => n.id === over.id);

      const newNotesOrder = arrayMove(notes, oldIndex, newIndex);
      setNotes(newNotesOrder);

      const orderedIds = newNotesOrder.map((note) => note.id);

      await handleApiResponse(() =>
        HatcheriesService.updateNotesOrder(orderedIds)
      );
    }
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
        <DndContext
          collisionDetection={closestCenter}
          onDragEnd={handleDragEnd}
        >
          <SortableContext
            items={notes.map((n) => n.id)}
            strategy={rectSortingStrategy}
          >
            <Box
              sx={{
                display: "grid",
                gridTemplateColumns: "repeat(auto-fill, minmax(250px, 1fr))",
                gap: 2,
              }}
            >
              {notes.length > 0 ? (
                notes.map((note) => (
                  <SortableNote
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
          </SortableContext>
        </DndContext>
      )}
    </Box>
  );
};

export default HatcheriesNotesPanel;
