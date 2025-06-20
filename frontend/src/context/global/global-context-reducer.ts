import type Action from "../../common/interfaces/action";

export interface GlobalContextReducerStateModel {
  pageLoaded: boolean;
}

export enum GlobalContextReducerActionType {
  SetPageLoaded,
}

export const GlobalContextReducerInitialState: GlobalContextReducerStateModel =
  {
    pageLoaded: false,
  };

export const GlobalContextReducer: (
  state: GlobalContextReducerStateModel,
  action: Action<GlobalContextReducerActionType>
) => GlobalContextReducerStateModel = (state, action) => {
  switch (action.type) {
    case GlobalContextReducerActionType.SetPageLoaded:
      return {
        ...state,
        pageLoaded: action.payload as boolean,
      };
    default:
      return state;
  }
};
