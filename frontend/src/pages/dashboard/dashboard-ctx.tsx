import { createContext, useContext } from "react";

export interface DashboardContextModel {}
export const DasboardCtx = createContext({} as DashboardContextModel);
export const useDashboardCtx = () => useContext(DasboardCtx);
export const DashboardCtxProvider = ({ children }: any) => {
  return <DasboardCtx.Provider value={{}}>{children}</DasboardCtx.Provider>;
};
