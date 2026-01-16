export interface QuestionnaireTemplate {
  id: number;
  name: string;
  description: string;
  questions: Question[];
}

export interface Question {
  id: number;
  text: string;
  type: 'text' | 'rating';
  maxRating?: number;
}

export interface User {
  id: number;
  name: string;
  email: string;
}

export interface Answer {
  id: number;
  userId: number;
  userName: string;
  templateId: number;
  templateName: string;
  questionId: number;
  questionText: string;
  value: string | number;
  submittedAt: Date;
}

export interface PaginationData {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalItems: number;
}

export interface TableRowData {
  id: string;
  userName: string;
  templateName: string;
  submittedAt: Date;
  [key: string]: any; // For dynamic question columns
}

export interface ChartDataset {
  label: string;
  data: (number | null)[];
  borderColor: string;
  backgroundColor: string;
  tension: number;
  fill: boolean;
  spanGaps: boolean;
}

export interface ChartData {
  labels: string[];
  datasets: ChartDataset[];
}