import { Component, inject } from '@angular/core';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, TranslateModule],
  styleUrls: ['../../../styles/dialogs.scss'],
  templateUrl: './SubmitConfirmDialog.component.html'
})
export class SubmitConfirmDialog {

  private dialogRef = inject(MatDialogRef<SubmitConfirmDialog>);

  cancel(): void {
    this.dialogRef.close(false);
  }

  confirm(): void {
    this.dialogRef.close(true);
  }
}