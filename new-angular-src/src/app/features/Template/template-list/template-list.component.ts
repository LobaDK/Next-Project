import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { CopyTemplateDialog } from './copy-template-dialog/CopyTemplateDialog.component';
import { MatDialog } from '@angular/material/dialog';
import { DeleteTemplateDialog } from './delete-template-dialog/DeleteTemplateDialog.component';

type TemplateStatus = 'Draft' | 'Finalized';

interface TemplateListItem {
  id: string;
  title: string;
  status: TemplateStatus;
}

@Component({
  selector: 'app-template-list',
  imports: [CommonModule],
  templateUrl: './template-list.component.html',
  styleUrl: './template-list.component.css',
})
export class TemplateListComponent {
  private dialog = inject(MatDialog);
  templates: TemplateListItem[] = [
    { id: '1', title: 'Math Quiz Template', status: 'Draft' },
    { id: '2', title: 'Final Exam Survey', status: 'Finalized' },
    { id: '3', title: 'Feedback Form', status: 'Draft' },
  ];

  editTemplate(templateId: string) {
    console.log('Edit template', templateId);
  }
  copyTemplate(templateId: string) {
    const ref = this.dialog.open(CopyTemplateDialog, {
      data: templateId,
      width: '500px',
      disableClose: true,
    });

    ref.afterClosed().subscribe((result) => {
      if (!result) return;
      console.log('Copied template returned from dialog:', result);

      // later: call API to create template
      // this.templateService.createTemplate(result).subscribe(...)
    });
  }

    deleteTemplate(templateId: string) {
      const ref = this.dialog.open(DeleteTemplateDialog, {
        data: templateId,
        width: '500px',
        disableClose: true,
      });

      ref.afterClosed().subscribe((confirmed) => {
        if (!confirmed) return;

        console.log('User confirmed delete for template:', templateId);

        // later: call API to delete
        // this.templateService.deleteTemplate(templateId).subscribe(...)
      });
    }


  getBadgeClass(status: TemplateStatus) {
    return status === 'Finalized' ? 'badge-active' : 'badge-pending';
  }
}
