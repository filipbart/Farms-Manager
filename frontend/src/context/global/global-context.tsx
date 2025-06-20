import { createContext, useReducer } from "react";
import {
  GlobalContextReducer,
  GlobalContextReducerActionType,
  GlobalContextReducerInitialState,
  type GlobalContextReducerStateModel,
} from "./global-context-reducer";

export interface GlobalContextMode {
  state: GlobalContextReducerStateModel;
  setPageLoaded: () => void;
}

export const GlobalContext = createContext({} as GlobalContextMode);

export const GlobalContextProvider = ({ children }: any) => {
  const [state, dispatch] = useReducer(
    GlobalContextReducer,
    GlobalContextReducerInitialState
  );

  const setPageLaoded = () => {
    dispatch({
      type: GlobalContextReducerActionType.SetPageLoaded,
      payload: true,
    });
  };

  return (
    <GlobalContext.Provider value={{ state, setPageLoaded: setPageLaoded }}>
      {children}
    </GlobalContext.Provider>
  );
};
