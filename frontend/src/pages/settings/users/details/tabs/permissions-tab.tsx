import {
  Box,
  Button,
  Grid,
  Typography,
  Checkbox,
  FormControlLabel,
  FormGroup,
  CircularProgress,
  Paper,
} from "@mui/material";
import { Controller, useForm } from "react-hook-form";
import { useEffect, useState, useMemo } from "react";
import { MdSave } from "react-icons/md";
import { toast } from "react-toastify";
import { UsersService } from "../../../../../services/users-service";
import { handleApiResponse } from "../../../../../utils/axios/handle-api-response";
import type {
  PermissionModel,
  UserDetailsModel,
} from "../../../../../models/users/users";

interface PermissionsTabProps {
  user: UserDetailsModel;
  refetch: () => void;
}

type PermissionsFormState = Record<string, boolean>;

const PermissionsTab: React.FC<PermissionsTabProps> = ({ user, refetch }) => {
  const [allPermissions, setAllPermissions] = useState<PermissionModel[]>([]);
  const [loading, setLoading] = useState(true);

  const {
    handleSubmit,
    control,
    reset,
    formState: { isDirty },
  } = useForm<PermissionsFormState>({ defaultValues: {} });

  useEffect(() => {
    const fetchAllPermissions = async () => {
      setLoading(true);
      try {
        await handleApiResponse(
          () => UsersService.getAllPermissions(),
          (data) => {
            setAllPermissions(data.responseData?.items ?? []);
          },
          undefined,
          "Nie udało się pobrać listy uprawnień"
        );
      } finally {
        setLoading(false);
      }
    };
    fetchAllPermissions();
  }, []);

  useEffect(() => {
    if (allPermissions.length > 0 && user?.permissions) {
      const initialFormState: PermissionsFormState = {};
      allPermissions.forEach((p) => {
        initialFormState[p.name] = user.permissions.includes(p.name);
      });
      reset(initialFormState);
    }
  }, [allPermissions, user, reset]);

  const groupedPermissions = useMemo(() => {
    return allPermissions.reduce((acc, permission) => {
      const group = permission.group;
      if (!acc[group]) {
        acc[group] = [];
      }
      acc[group].push(permission);
      return acc;
    }, {} as Record<string, PermissionModel[]>);
  }, [allPermissions]);

  const onSubmit = async (data: PermissionsFormState) => {
    if (!user) return;

    const selectedPermissions = Object.entries(data)
      .filter(([, isSelected]) => isSelected)
      .map(([permissionName]) => permissionName);

    try {
      await handleApiResponse(
        () => UsersService.updateUserPermissions(user.id, selectedPermissions),
        () => {
          toast.success("Uprawnienia użytkownika zostały zaktualizowane");
          refetch();
        },
        undefined,
        "Nie udało się zapisać uprawnień"
      );
    } catch {
      toast.error("Wystąpił błąd podczas zapisu");
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" mt={5}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)} mt={2}>
      <Grid container spacing={3}>
        {Object.entries(groupedPermissions).map(([groupName, permissions]) => (
          <Grid size={{ xs: 12, sm: 6, md: 4 }} key={groupName}>
            <Paper variant="outlined" sx={{ p: 2 }}>
              <Typography variant="h6" sx={{ mb: 1 }}>
                {groupName}
              </Typography>
              <FormGroup>
                {permissions.map((permission) => (
                  <FormControlLabel
                    key={permission.name}
                    control={
                      <Controller
                        name={permission.name}
                        control={control}
                        render={({ field }) => (
                          <Checkbox
                            checked={field.value || false}
                            onChange={field.onChange}
                          />
                        )}
                      />
                    }
                    label={permission.description}
                  />
                ))}
              </FormGroup>
            </Paper>
          </Grid>
        ))}
      </Grid>
      <Box display="flex" justifyContent="flex-end" mt={3}>
        <Button
          type="submit"
          variant="contained"
          color="primary"
          startIcon={<MdSave />}
          disabled={!isDirty}
        >
          Zapisz uprawnienia
        </Button>
      </Box>
    </Box>
  );
};

export default PermissionsTab;
