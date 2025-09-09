import React, { useEffect } from "react";
import { Tabs, Tab, Typography, Box } from "@mui/material";
import GeneralSettingsTab from "./tabs/general";
import IrzPlusSettingsTab from "./tabs/irzplus";
import { useUserDetails } from "../../hooks/useUserDetails";
import Loading from "../../components/loading/loading";
import NotificationSettingsTab from "./tabs/notifications";

const UserProfilePage: React.FC = () => {
  const [value, setValue] = React.useState(0);

  const { userDetails, loadingUser, fetchUserDetails } = useUserDetails();

  const handleChange = (_event: React.SyntheticEvent, newValue: number) => {
    setValue(newValue);
  };

  useEffect(() => {
    fetchUserDetails();
  }, [fetchUserDetails]);

  const TabPanel = (props: {
    children: React.ReactNode;
    index: number;
    value: number;
  }) => {
    const { children, index, value, ...other } = props;
    return (
      <div
        role="tabpanel"
        hidden={value !== index}
        id={`tab-panel-${index}`}
        aria-labelledby={`tab-${index}`}
        {...other}
      >
        {value === index && <Box p={3}>{children}</Box>}
      </div>
    );
  };

  return (
    <>
      {loadingUser ? (
        <Loading />
      ) : (
        <Box m={2}>
          <Typography variant="h4" gutterBottom>
            Profil
          </Typography>
          <Box
            position="static"
            sx={{ borderBottom: 1, borderColor: "divider" }}
          >
            <Tabs value={value} onChange={handleChange} variant="fullWidth">
              <Tab label="Ogólne" />
              <Tab label="Powiadomienia" />
              <Tab label="IRZplus" />
            </Tabs>
          </Box>
          <TabPanel value={value} index={0}>
            <GeneralSettingsTab />
          </TabPanel>
          <TabPanel value={value} index={1}>
            <NotificationSettingsTab />
          </TabPanel>
          <TabPanel value={value} index={2}>
            <IrzPlusSettingsTab
              userDetails={userDetails}
              onReload={() => fetchUserDetails()}
            />
          </TabPanel>
        </Box>
      )}
    </>
  );
};

export default UserProfilePage;
