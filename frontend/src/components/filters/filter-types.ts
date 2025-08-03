export type FilterType =
  | "multiSelect"
  | "date"
  | "number"
  | "text"
  | "select"
  | "multiSelectSearch";

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

export interface MultiSelectSearchFilter<K extends string>
  extends BaseFilterConfig<K> {
  type: "multiSelectSearch";
  options: { value: string; label: string }[];
}

export interface SelectFilter<K extends string> extends BaseFilterConfig<K> {
  type: "select";
  options: { value: string; label: string }[];
}

export interface DateFilter<K extends string> extends BaseFilterConfig<K> {
  type: "date";
}

export interface NumberFilter<K extends string> extends BaseFilterConfig<K> {
  type: "number";
}

export interface TextFilter<K extends string> extends BaseFilterConfig<K> {
  type: "text";
}

export type FilterConfig<K extends string = string> =
  | MultiSelectFilter<K>
  | SelectFilter<K>
  | DateFilter<K>
  | NumberFilter<K>
  | TextFilter<K>
  | MultiSelectSearchFilter<K>;
