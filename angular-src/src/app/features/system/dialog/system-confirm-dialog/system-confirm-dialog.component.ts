import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';

export interface SystemConfirmDialogData {
  titleKey: string;
  textKey: string;
  confirmKey: string;
}

@Component({
  selector: 'app-system-confirm-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, TranslateModule],
  styleUrls: ['../../../../styles/dialogs.scss'],
  templateUrl: './system-confirm-dialog.component.html'
})
export class SystemConfirmDialogComponent {
  private dialogRef = inject(MatDialogRef<SystemConfirmDialogComponent, boolean>);
  readonly data = inject<SystemConfirmDialogData>(MAT_DIALOG_DATA);

  cancel(): void {
    this.dialogRef.close(false);
  }

  confirm(): void {
    this.dialogRef.close(true);
  }
}
