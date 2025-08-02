import {
  Box,
  Typography,
  Button,
  List,
  ListItem,
  ListItemText,
  IconButton,
} from "@mui/material";
import { MdAddAlert, MdDelete } from "react-icons/md";
import { useContext, useState } from "react";
import { toast } from "react-toastify";
import dayjs from "dayjs";
import { EmployeeContext } from "..";
import { EmployeesService } from "../../../../services/employees-service";

const EmployeeRemindersTab: React.FC = () => {
  const { employee, refetch } = useContext(EmployeeContext);
  const [modalOpen, setModalOpen] = useState(false);

  const handleDelete = async (reminderId: string) => {
    if (employee) {
      try {
        const res = await EmployeesService.deleteEmployeeReminder(
          employee?.id,
          reminderId
        );
        if (res.success) {
          toast.success("Przypomnienie usunięte");
          refetch();
        } else {
          toast.error("Nie udało się usunąć przypomnienia");
        }
      } catch {
        toast.error("Błąd podczas usuwania przypomnienia");
      }
    }
  };

  return (
    <Box>
      <Box display="flex" justifyContent="flex-end" mb={2}>
        <Button
          variant="contained"
          onClick={() => setModalOpen(true)}
          startIcon={<MdAddAlert />}
        >
          Dodaj przypomnienie
        </Button>
      </Box>

      {employee?.reminders?.length ? (
        <List>
          {employee.reminders.map((reminder) => (
            <ListItem key={reminder.id} divider>
              <ListItemText
                primary={reminder.title}
                secondary={
                  <>
                    <Typography variant="body2" color="textSecondary">
                      Termin: {dayjs(reminder.dueDate).format("DD.MM.YYYY")}
                    </Typography>
                  </>
                }
              />
              <ListItem>
                <IconButton
                  edge="end"
                  onClick={() => handleDelete(reminder.id)}
                  title="Usuń"
                >
                  <MdDelete />
                </IconButton>
              </ListItem>
            </ListItem>
          ))}
        </List>
      ) : (
        <Typography>Brak przypomnień dla pracownika.</Typography>
      )}

      {/* <AddReminderModal
        open={modalOpen}
        onClose={() => setModalOpen(false)}
        onSaved={refetch}
        employeeId={employee.id}
      /> */}
    </Box>
  );
};

export default EmployeeRemindersTab;
