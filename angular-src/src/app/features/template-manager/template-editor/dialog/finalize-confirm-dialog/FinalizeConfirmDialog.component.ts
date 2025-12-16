import { Component, inject } from '@angular/core';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';
import { Template } from '../../../../../shared/models/template.model';

@Component({
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, TranslateModule],
  styleUrls: ['../../../../../styles/dialogs.scss'],
  templateUrl: './FinalizeConfirmDialog.component.html'
})
export class FinalizeConfirmDialog {

  private dialogRef = inject(MatDialogRef<FinalizeConfirmDialog>);
  readonly template = inject<Template>(MAT_DIALOG_DATA);

  cancel(): void {
    this.dialogRef.close(false);
  }

  confirm(): void {
    this.dialogRef.close(true);
  }
}