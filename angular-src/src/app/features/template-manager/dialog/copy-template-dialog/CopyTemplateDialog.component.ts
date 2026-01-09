import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TemplateService } from '../../services/template.service';
import { Template, TemplateStatus } from '../../../../shared/models/template.model';

@Component({
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, TranslateModule],
  styleUrls: ['../../../../styles/dialogs.scss'],
  templateUrl: './CopyTemplateDialog.component.html'
})
export class CopyTemplateDialog {

  isLoading = false;
  private dialogRef = inject(MatDialogRef<CopyTemplateDialog>);
  private templateService = inject(TemplateService);
  private translate = inject(TranslateService);
  readonly templateId = inject<string>(MAT_DIALOG_DATA);

  cancel(): void {
    this.dialogRef.close(null);
  }

  confirm(): void {
    if (this.isLoading) return;

    this.isLoading = true;
    
    this.templateService.getTemplateDetails(this.templateId).subscribe({
      next: (template) => {
        const copiedTemplate = this.deepCopyAsNewTemplate(template);
        this.dialogRef.close(copiedTemplate);
      },
      error: (err) => {
        console.error('Error loading template to copy:', err);
        this.isLoading = false;
        // Could show error message or just close
        this.dialogRef.close(null);
      }
    });
  }

  /**
   * Create a deep-cloned editable draft from an existing template:
   * - Clears server metadata, sets Draft status, unlocks.
   * - Assigns temporary ids to template/questions/options.
   */
private deepCopyAsNewTemplate(template: Template): Template {
  // Deep clone to avoid mutating the original
  const clone: Template = JSON.parse(JSON.stringify(template));

  // If it has an ID, replace with a unique negative ID
  // If it has no ID, leave it undefined
  clone.id = clone.id ? `temp-${Date.now()}` : undefined;

  // Reset meta fields
  clone.createdAt = undefined;
  clone.lastUpdated = undefined;
  clone.templateStatus = TemplateStatus.Draft;
  clone.isLocked = false;
  clone.title = `${clone.title} (kopi)`
  clone.id = "";

  // Assign fresh negative IDs to questions & options
  clone.questions = clone.questions.map((q, qIndex) => ({
    ...q,
    id: -1 * (qIndex + 1), // new negative ID
    sortOrder: qIndex,
    options: q.options.map((o, oIndex) => ({
      ...o,
      id: -1 * (oIndex + 1),// new negative ID
      sortOrder: oIndex
    }))
  }));

  return clone;
}
}