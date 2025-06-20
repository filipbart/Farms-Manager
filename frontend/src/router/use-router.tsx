import type { RouteName } from "./route-names";
import { createContext, useContext } from "react";

interface RouterContextModel {
  getRoute: (routeName: RouteName, args?: any) => string;
}

export const RouterContext = createContext({} as RouterContextModel);

export const RouterProvider = ({ children }: any) => {
  const getRoute = (routeName: RouteName, args?: any) => {
    let route = `/${routeName}`;
    if (args) {
      Object.keys(args).forEach((key) => {
        route += `/${args[key]}`;
      });
    }
    return route;
  };

  return (
    <RouterContext.Provider value={{ getRoute }}>
      {children}
    </RouterContext.Provider>
  );
};

export const useRouter = () => useContext(RouterContext);
