import { Component, inject } from '@angular/core';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, TranslateModule],
  styleUrls: ['../../../../styles/dialogs.scss'],
  templateUrl: './ErrorDialog.component.html'
})
export class ErrorDialog {

  private dialogRef = inject(MatDialogRef<ErrorDialog>);
  readonly errorMessage = inject<string>(MAT_DIALOG_DATA);

  close(): void {
    this.dialogRef.close();
  }
}