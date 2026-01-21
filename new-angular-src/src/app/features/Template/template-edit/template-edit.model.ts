export interface RatingOption {
  value: number;              // 1..N
  label: string;              // REQUIRED (your new rule)
}


export interface RatingQuestion {
  id: string;
  type: "rating";
  prompt: string;

  scale: RatingOption[];
}


export interface MatrixItem {
  value: string;              // stable stored value
  label: string;              // displayed text
}

export interface SingleChoiceMatrixQuestion {
  id: string;
  type: "matrix_single";
  prompt: string;

  rows: MatrixItem[];
  columns: MatrixItem[];
}


export interface RadioOption {
  value: string;              // stored answer value (stable)
  label: string;              // displayed text
}


export interface RadioGroupQuestion {
  id: string;                 // internal stable id (uuid or q1/q2)
  type: "radiogroup";
  prompt: string;

  options: RadioOption[];

  allowOtherComment: boolean; // checkbox: "Other / describe"
  otherLabel?: string | null; // e.g. "Other" or "Other (describe)"
}


export type TemplateQuestion =
  | RadioGroupQuestion
  | SingleChoiceMatrixQuestion
  | RatingQuestion;

export interface QuestionnaireForm {
  title: string;
  description?: string;
  schemaJson: string;
}
