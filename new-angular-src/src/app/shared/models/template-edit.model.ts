export enum TemplateStatus {
  Draft = 'Draft',
  Finalized = 'Finalized',
}

export enum QuestionType {
  Rating = "rating",
  MatrixSingle = "matrix",
  RadioGroup = "radiogroup",
}

export interface EditorBaseQuestion {
  id: string;
  type: QuestionType;
  prompt: string;

  // purely editor:
  expanded?: boolean;
}
export interface ChoiceOption {
  value: number;
  label: string;
}


export interface RadioGroupQuestion extends EditorBaseQuestion {
  type: QuestionType.RadioGroup;
  options: ChoiceOption[];              // just labels
  allowOtherComment: boolean;
  otherLabel?: string | null;
}

export type TemplateQuestion =
  | RadioGroupQuestion

  export interface QuestionnaireTemplateEditor {
    title: string;
    description?: string;
    status: TemplateStatus;
    questions: TemplateQuestion[];
  }


export interface QuestionnaireForm {
  title: string;
  description?: string;
  schemaJson: string;
}









/* | SingleChoiceMatrixQuestion | RatingQuestion */;

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