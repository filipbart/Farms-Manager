import React, { useState } from "react";
import { Avatar, Button, Box, CircularProgress } from "@mui/material";
import { toast } from "react-toastify";
import { useAuth } from "../../auth/useAuth";
import { UserService } from "../../services/user-service";

const AvatarUploader: React.FC = () => {
  const { userData, fetchUserData } = useAuth();
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [preview, setPreview] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files?.[0]) {
      const file = e.target.files[0];

      if (!file.type.startsWith("image/")) {
        toast.error("Proszę wybrać plik graficzny.");
        return;
      }
      setSelectedFile(file);
      setPreview(URL.createObjectURL(file));

      e.target.value = "";
    }
  };

  const handleSave = async () => {
    if (!selectedFile) return;
    setLoading(true);

    try {
      const response = await UserService.updateUserAvatar(selectedFile);
      if (response.success && response.responseData) {
        await fetchUserData();
        toast.success("Awatar został zaktualizowany!");
        setSelectedFile(null);
        setPreview(null);
      }
    } catch {
      toast.error("Nie udało się zaktualizować awatara.");
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = () => {
    setSelectedFile(null);
    if (preview) {
      URL.revokeObjectURL(preview);
    }
    setPreview(null);
  };

  return (
    <Box display="flex" flexDirection="column" alignItems="center" gap={2}>
      <Avatar
        src={preview || userData?.avatarPath}
        alt={userData?.name}
        sx={{ width: 150, height: 150, fontSize: "4rem" }}
      >
        {!preview && !userData?.avatarPath}
      </Avatar>

      <Button variant="outlined" component="label" disabled={loading}>
        Zmień awatar
        <input
          type="file"
          hidden
          accept="image/*"
          onChange={handleFileChange}
        />
      </Button>

      {selectedFile && (
        <Box display="flex" gap={1}>
          <Button onClick={handleSave} disabled={loading} variant="contained">
            {loading ? <CircularProgress size={24} /> : "Zapisz"}
          </Button>
          <Button onClick={handleCancel} color="inherit" disabled={loading}>
            Anuluj
          </Button>
        </Box>
      )}
    </Box>
  );
};

export default AvatarUploader;
