export enum QuestionType {
  Rating = "rating",
  MatrixSingle = "matrix",
  RadioGroup = "radiogroup",
}

export interface EditorBaseQuestion {
  id: string;                 // stable question id
  type: QuestionType;
  prompt: string;

  // purely editor:
  expanded?: boolean;
}

export interface RatingOption {
  value: number;              // 1..N
  description?: string;
  label: string;              // REQUIRED (your new rule)
}


export interface RatingQuestion extends EditorBaseQuestion{
    type: QuestionType.Rating;
    scale: RatingOption[];
}


export interface MatrixItem {
  value: string;              // stable stored value
  label: string;              // displayed text
}

export interface SingleChoiceMatrixQuestion extends EditorBaseQuestion {
  type: QuestionType.MatrixSingle;
  rows: string[];                 // row labels only
  columns: string[];              // column labels only
}


export interface RadioOption {
  value: string;              // displayed text
  label: string;              // displayed text
}


export interface RadioGroupQuestion extends EditorBaseQuestion {
  type: QuestionType.RadioGroup;
  options: string[];              // just labels
  allowOtherComment: boolean;
  otherLabel?: string | null;
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
