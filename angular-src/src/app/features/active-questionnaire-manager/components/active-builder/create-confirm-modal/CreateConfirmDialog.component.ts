import { Component, inject } from '@angular/core';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';

interface CreateConfirmDialogData {
  title: string;
  text: string;
  confirmText: string;
  cancelText: string;
}

@Component({
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, TranslateModule],
  styleUrls: ['../../../../../styles/dialogs.scss'],
  templateUrl: './CreateConfirmDialog.component.html'
})
export class CreateConfirmDialog {

  private dialogRef = inject(MatDialogRef<CreateConfirmDialog>);
  readonly dialogData = inject<CreateConfirmDialogData>(MAT_DIALOG_DATA);

  cancel(): void {
    this.dialogRef.close(false);
  }

  confirm(): void {
    this.dialogRef.close(true);
  }
}