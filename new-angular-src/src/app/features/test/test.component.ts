import { Component } from '@angular/core';

@Component({
  selector: 'app-test',
  standalone: true,
  imports: [],
  templateUrl: './test.component.html',
  styleUrls: ['./test.component.css']
})
export class TestComponent {
  title = 'CSS Test Component';
  
  cards = [
    {
      title: 'Regular Card',
      content: 'This is an example of the regular app-card class with full padding and styling.',
      type: 'primary'
    },
    {
      title: 'Small Card',
      content: 'This demonstrates the app-card-sm class with medium padding.',
      type: 'secondary'
    },
    {
      title: 'Compact Card',
      content: 'Compact card example with minimal padding.',
      type: 'info'
    }
  ];

  buttonExamples = [
    { label: 'Primary Button', class: 'bg-primary_orange_1 hover:bg-primary_orange_light' },
    { label: 'Secondary Button', class: 'bg-primary_dark_blue hover:bg-blue-700' },
    { label: 'Success Button', class: 'bg-green-500 hover:bg-green-600' },
    { label: 'Danger Button', class: 'bg-red-500 hover:bg-red-600' }
  ];
}