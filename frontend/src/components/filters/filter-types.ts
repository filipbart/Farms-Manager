export type FilterType = "multiSelect" | "date";

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

export type FilterConfig<K extends string = string> =
  | MultiSelectFilter<K>
  | DateFilter<K>;
