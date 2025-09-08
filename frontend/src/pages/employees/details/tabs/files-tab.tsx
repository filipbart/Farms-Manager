import {
  Box,
  Typography,
  Button,
  List,
  ListItem,
  ListItemText,
  IconButton,
} from "@mui/material";
import { MdDelete, MdDownload, MdUploadFile } from "react-icons/md";
import { useContext, useRef, useState } from "react";
import { toast } from "react-toastify";
import ApiUrl from "../../../../common/ApiUrl";
import { EmployeesService } from "../../../../services/employees-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { downloadFile } from "../../../../utils/download-file";
import { EmployeeContext } from "../../../../context/employee-context";

const EmployeeFilesTab: React.FC = () => {
  const { employee, refetch } = useContext(EmployeeContext);
  const [loadingFilePath, setLoadingFilePath] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);
  const inputRef = useRef<HTMLInputElement | null>(null);

  const handleDownload = async (path: string) => {
    const lastDotIndex = path.lastIndexOf(".");
    const fileExtension =
      lastDotIndex !== -1 && lastDotIndex < path.length - 1
        ? path.substring(lastDotIndex + 1)
        : "pdf";
    await downloadFile({
      url: ApiUrl.GetFile,
      params: { filePath: path },
      defaultFilename: "Plik",
      setLoading: (v) => setLoadingFilePath(v ? path : null),
      errorMessage: "Błąd podczas pobierania pliku",
      fileExtension: fileExtension,
    });
  };

  const handleDelete = async (fileId: string) => {
    if (!employee) return;
    try {
      const res = await EmployeesService.deleteEmployeeFile(
        employee.id,
        fileId
      );
      if (res.success) {
        toast.success("Plik został usunięty");
        refetch();
      } else {
        toast.error("Nie udało się usunąć pliku");
      }
    } catch {
      toast.error("Błąd podczas usuwania pliku");
    }
  };

  const handleUploadClick = () => {
    inputRef.current?.click();
  };

  const handleUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
    if (!employee) return;
    const files = event.target.files;
    if (!files || files.length === 0) return;

    const fileList = Array.from(files);
    setUploading(true);

    await handleApiResponse(
      () => EmployeesService.uploadEmployeeFiles(employee.id, fileList),
      async () => {
        toast.success("Pliki zostały wgrane");
        refetch();
      },
      undefined,
      "Błąd podczas wgrywania plików"
    );

    setUploading(false);
    event.target.value = "";
  };

  return (
    <Box>
      <Box display="flex" justifyContent="flex-end" mb={2}>
        <input
          ref={inputRef}
          type="file"
          hidden
          multiple
          onChange={handleUpload}
        />
        <Button
          variant="contained"
          onClick={handleUploadClick}
          startIcon={<MdUploadFile />}
          disabled={uploading}
        >
          Dodaj plik
        </Button>
      </Box>

      {employee?.files.length === 0 ? (
        <Typography>Brak plików przypisanych do pracownika.</Typography>
      ) : (
        <List>
          {employee?.files.map((file) => (
            <ListItem
              key={file.id}
              divider
              secondaryAction={
                <>
                  <IconButton
                    edge="end"
                    color="primary"
                    onClick={() => handleDownload(file.filePath)}
                    disabled={loadingFilePath === file.filePath}
                    title="Pobierz"
                  >
                    <MdDownload />
                  </IconButton>
                  <IconButton
                    edge="end"
                    color="error"
                    onClick={() => handleDelete(file.id)}
                    title="Usuń"
                  >
                    <MdDelete />
                  </IconButton>
                </>
              }
            >
              <ListItemText primary={file.fileName} />
            </ListItem>
          ))}
        </List>
      )}
    </Box>
  );
};

export default EmployeeFilesTab;
