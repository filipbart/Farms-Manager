import React, { useEffect, useState, useCallback } from "react";
import {
  Box,
  Typography,
  Button,
  Paper,
  IconButton,
  Chip,
  Switch,
  Tooltip,
  CircularProgress,
} from "@mui/material";
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  DragIndicator as DragIcon,
} from "@mui/icons-material";
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
} from "@dnd-kit/core";
import type { DragEndEvent } from "@dnd-kit/core";
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  useSortable,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { toast } from "react-toastify";
import { SettingsService } from "../../services/settings-service";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import type { InvoiceFarmAssignmentRule } from "../../models/settings/invoice-farm-assignment-rule";
import InvoiceFarmAssignmentRuleModal from "../../components/modals/settings/invoice-farm-assignment-rule-modal";
import ConfirmDialog from "../../components/common/confirm-dialog";

interface SortableRuleItemProps {
  rule: InvoiceFarmAssignmentRule;
  onEdit: (rule: InvoiceFarmAssignmentRule) => void;
  onDelete: (rule: InvoiceFarmAssignmentRule) => void;
  onToggleActive: (rule: InvoiceFarmAssignmentRule) => void;
}

const SortableRuleItem: React.FC<SortableRuleItemProps> = ({
  rule,
  onEdit,
  onDelete,
  onToggleActive,
}) => {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: rule.id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  };

  return (
    <Paper
      ref={setNodeRef}
      style={style}
      sx={{
        p: 2,
        mb: 1,
        display: "flex",
        alignItems: "center",
        gap: 2,
        bgcolor: rule.isActive
          ? "background.paper"
          : "action.disabledBackground",
      }}
      elevation={isDragging ? 4 : 1}
    >
      <IconButton
        {...attributes}
        {...listeners}
        size="small"
        sx={{ cursor: "grab" }}
      >
        <DragIcon />
      </IconButton>

      <Box sx={{ flex: 1 }}>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1, mb: 0.5 }}>
          <Typography variant="subtitle1" fontWeight={600}>
            {rule.name}
          </Typography>
          <Chip
            label={`Priorytet: ${rule.priority}`}
            size="small"
            variant="outlined"
          />
          {!rule.isActive && (
            <Chip label="Nieaktywna" size="small" color="warning" />
          )}
        </Box>

        {rule.description && (
          <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
            {rule.description}
          </Typography>
        )}

        <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1, mb: 1 }}>
          <Typography variant="caption" color="text.secondary">
            Lokalizacja docelowa:
          </Typography>
          <Chip label={rule.targetFarmName} size="small" color="primary" />
        </Box>

        <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5 }}>
          {rule.includeKeywords.length > 0 && (
            <>
              <Typography
                variant="caption"
                color="text.secondary"
                sx={{ mr: 0.5 }}
              >
                Słowa kluczowe:
              </Typography>
              {rule.includeKeywords.map((keyword, idx) => (
                <Chip
                  key={idx}
                  label={keyword}
                  size="small"
                  color="success"
                  variant="outlined"
                />
              ))}
            </>
          )}
          {rule.excludeKeywords.length > 0 && (
            <>
              <Typography
                variant="caption"
                color="text.secondary"
                sx={{ mx: 0.5 }}
              >
                Wyklucz:
              </Typography>
              {rule.excludeKeywords.map((keyword, idx) => (
                <Chip
                  key={idx}
                  label={keyword}
                  size="small"
                  color="error"
                  variant="outlined"
                />
              ))}
            </>
          )}
        </Box>

        {(rule.taxBusinessEntityName || rule.invoiceDirectionName) && (
          <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5, mt: 1 }}>
            {rule.taxBusinessEntityName && (
              <Chip
                label={`Działalność: ${rule.taxBusinessEntityName}`}
                size="small"
                variant="outlined"
              />
            )}
            {rule.invoiceDirectionName && (
              <Chip
                label={`Kierunek: ${rule.invoiceDirectionName}`}
                size="small"
                variant="outlined"
              />
            )}
          </Box>
        )}
      </Box>

      <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
        <Tooltip title={rule.isActive ? "Dezaktywuj" : "Aktywuj"}>
          <Switch
            checked={rule.isActive}
            onChange={() => onToggleActive(rule)}
            size="small"
          />
        </Tooltip>
        <Tooltip title="Edytuj">
          <IconButton size="small" onClick={() => onEdit(rule)}>
            <EditIcon />
          </IconButton>
        </Tooltip>
        <Tooltip title="Usuń">
          <IconButton size="small" color="error" onClick={() => onDelete(rule)}>
            <DeleteIcon />
          </IconButton>
        </Tooltip>
      </Box>
    </Paper>
  );
};

const InvoiceFarmAssignmentRulesPage: React.FC = () => {
  const [rules, setRules] = useState<InvoiceFarmAssignmentRule[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRule, setEditingRule] =
    useState<InvoiceFarmAssignmentRule | null>(null);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [ruleToDelete, setRuleToDelete] =
    useState<InvoiceFarmAssignmentRule | null>(null);

  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  const fetchRules = useCallback(async () => {
    setLoading(true);
    try {
      await handleApiResponse(
        () => SettingsService.getInvoiceFarmAssignmentRules(),
        (data) => {
          if (data.responseData) {
            setRules(data.responseData);
          }
        },
        undefined,
        "Błąd podczas pobierania reguł"
      );
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchRules();
  }, [fetchRules]);

  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event;

    if (over && active.id !== over.id) {
      const oldIndex = rules.findIndex((r) => r.id === active.id);
      const newIndex = rules.findIndex((r) => r.id === over.id);

      const newRules = arrayMove(rules, oldIndex, newIndex);
      setRules(newRules);

      try {
        await handleApiResponse(
          () =>
            SettingsService.reorderInvoiceFarmAssignmentRules(
              newRules.map((r) => r.id)
            ),
          () => {
            toast.success("Kolejność reguł została zmieniona");
          },
          () => {
            fetchRules();
          },
          "Błąd podczas zmiany kolejności"
        );
      } catch {
        fetchRules();
      }
    }
  };

  const handleAddRule = () => {
    setEditingRule(null);
    setModalOpen(true);
  };

  const handleEditRule = (rule: InvoiceFarmAssignmentRule) => {
    setEditingRule(rule);
    setModalOpen(true);
  };

  const handleDeleteRule = (rule: InvoiceFarmAssignmentRule) => {
    setRuleToDelete(rule);
    setDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (!ruleToDelete) return;

    try {
      await handleApiResponse(
        () => SettingsService.deleteInvoiceFarmAssignmentRule(ruleToDelete.id),
        () => {
          toast.success("Reguła została usunięta");
          fetchRules();
        },
        undefined,
        "Błąd podczas usuwania reguły"
      );
    } finally {
      setDeleteDialogOpen(false);
      setRuleToDelete(null);
    }
  };

  const handleToggleActive = async (rule: InvoiceFarmAssignmentRule) => {
    try {
      await handleApiResponse(
        () =>
          SettingsService.updateInvoiceFarmAssignmentRule(rule.id, {
            isActive: !rule.isActive,
          }),
        () => {
          toast.success(
            rule.isActive
              ? "Reguła została dezaktywowana"
              : "Reguła została aktywowana"
          );
          fetchRules();
        },
        undefined,
        "Błąd podczas zmiany statusu reguły"
      );
    } catch {
      // Error handled by handleApiResponse
    }
  };

  const handleModalClose = () => {
    setModalOpen(false);
    setEditingRule(null);
  };

  const handleModalSave = () => {
    fetchRules();
    handleModalClose();
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" py={4}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mb: 3,
        }}
      >
        <Box>
          <Typography variant="h5" fontWeight={600}>
            Reguły przypisywania faktur do lokalizacji
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Zarządzaj regułami automatycznego przypisywania faktur do
            lokalizacji (ferm). Przeciągnij regułę, aby zmienić jej priorytet.
            Przy dopasowaniu fermy automatycznie przypisywany jest aktywny cykl.
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleAddRule}
        >
          Dodaj regułę
        </Button>
      </Box>

      {rules.length === 0 ? (
        <Paper sx={{ p: 4, textAlign: "center" }}>
          <Typography color="text.secondary">
            Brak zdefiniowanych reguł. Kliknij "Dodaj regułę", aby utworzyć
            pierwszą.
          </Typography>
        </Paper>
      ) : (
        <DndContext
          sensors={sensors}
          collisionDetection={closestCenter}
          onDragEnd={handleDragEnd}
        >
          <SortableContext
            items={rules.map((r) => r.id)}
            strategy={verticalListSortingStrategy}
          >
            {rules.map((rule) => (
              <SortableRuleItem
                key={rule.id}
                rule={rule}
                onEdit={handleEditRule}
                onDelete={handleDeleteRule}
                onToggleActive={handleToggleActive}
              />
            ))}
          </SortableContext>
        </DndContext>
      )}

      <InvoiceFarmAssignmentRuleModal
        open={modalOpen}
        onClose={handleModalClose}
        onSave={handleModalSave}
        rule={editingRule}
      />

      <ConfirmDialog
        open={deleteDialogOpen}
        onClose={() => setDeleteDialogOpen(false)}
        onConfirm={confirmDelete}
        title="Usuń regułę"
        content={`Czy na pewno chcesz usunąć regułę "${ruleToDelete?.name}"?`}
      />
    </Box>
  );
};

export default InvoiceFarmAssignmentRulesPage;
