import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

interface TableData {
  id: number;
  name: string;
  email: string;
  role: string;
  status: 'Active' | 'Pending' | 'Inactive';
  lastLogin: string;
  actions?: string[];
}

@Component({
  selector: 'app-table-example',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './table-example.component.html',
  styleUrls: ['./table-example.component.css']
})
export class TableExampleComponent {
  title = 'Table Examples';

  // Sample table data
  tableData: TableData[] = [
    {
      id: 1,
      name: 'John Doe',
      email: 'john.doe@example.com',
      role: 'Manager',
      status: 'Active',
      lastLogin: '2024-01-15 10:30',
      actions: ['Edit', 'Delete']
    },
    {
      id: 2,
      name: 'Jane Smith',
      email: 'jane.smith@example.com',
      role: 'Developer',
      status: 'Active',
      lastLogin: '2024-01-15 09:15',
      actions: ['Edit', 'Delete']
    },
    {
      id: 3,
      name: 'Bob Johnson',
      email: 'bob.johnson@example.com',
      role: 'Designer',
      status: 'Pending',
      lastLogin: '2024-01-14 16:45',
      actions: ['Edit', 'Delete']
    },
    {
      id: 4,
      name: 'Alice Brown',
      email: 'alice.brown@example.com',
      role: 'Analyst',
      status: 'Inactive',
      lastLogin: '2024-01-10 14:20',
      actions: ['Edit', 'Delete']
    },
    {
      id: 5,
      name: 'Charlie Wilson',
      email: 'charlie.wilson@example.com',
      role: 'Developer',
      status: 'Active',
      lastLogin: '2024-01-15 11:00',
      actions: ['Edit', 'Delete']
    }
  ];

  // Simple table data for minimal example
  simpleData = [
    { product: 'Laptop', price: '$999', stock: '15' },
    { product: 'Mouse', price: '$25', stock: '150' },
    { product: 'Keyboard', price: '$75', stock: '80' },
    { product: 'Monitor', price: '$299', stock: '25' }
  ];

  onEdit(item: TableData): void {
    console.log('Edit:', item);
  }

  onDelete(item: TableData): void {
    console.log('Delete:', item);
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case 'Active': return 'badge-active';
      case 'Pending': return 'badge-pending';
      case 'Inactive': return 'badge-inactive';
      default: return 'badge-inactive';
    }
  }
}