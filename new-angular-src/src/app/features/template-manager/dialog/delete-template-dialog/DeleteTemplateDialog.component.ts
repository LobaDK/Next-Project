import { Component, inject, Inject } from '@angular/core';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TemplateService } from '../../services/template.service';

type DeleteStage = 'confirm' | 'final';

@Component({
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, TranslateModule],
  styleUrls: [ '../../../../styles/dialogs.scss'],
  templateUrl: './DeleteTemplateDialog.component.html'
})
export class DeleteTemplateDialog {

  stage: DeleteStage = 'confirm';
  private dialogRef = inject(MatDialogRef<DeleteTemplateDialog>);
  private templateService = inject(TemplateService);
  private translate = inject(TranslateService);
  readonly templateId = inject<string>(MAT_DIALOG_DATA);

cancel(): void {
  if (this.stage === 'final') {
    this.stage = 'confirm';
    return;
  }
  this.dialogRef.close(false);
}


confirm(): void {
  if (this.stage === 'confirm') {
    this.stage = 'final';
    return;
  }

  this.templateService.deleteTemplate(this.templateId).subscribe({
    complete: () => this.dialogRef.close(true),
    error: err => console.error('Delete failed', err),
  });
}
}
