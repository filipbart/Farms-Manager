export type FilterType = "multiSelect" | "date" | "number";

interface BaseFilterConfig<K extends string> {
  key: K;
  label: string;
  type: FilterType;
  disabled?: boolean;
}

export interface MultiSelectFilter<K extends string>
  extends BaseFilterConfig<K> {
  type: "multiSelect";
  options: { value: string; label: string }[];
}

export interface DateFilter<K extends string> extends BaseFilterConfig<K> {
  type: "date";
}

export interface NumberFilter<K extends string> extends BaseFilterConfig<K> {
  type: "number";
}

export type FilterConfig<K extends string = string> =
  | MultiSelectFilter<K>
  | DateFilter<K>
  | NumberFilter<K>;
