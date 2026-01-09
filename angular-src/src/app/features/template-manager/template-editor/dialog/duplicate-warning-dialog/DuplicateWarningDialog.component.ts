import { Component, inject } from '@angular/core';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  standalone: true,
  selector: 'app-duplicate-warning-dialog',
  imports: [MatDialogModule, MatButtonModule, TranslateModule],
  styleUrls: ['../../../../../styles/dialogs.scss'],
  templateUrl: './DuplicateWarningDialog.component.html'
})
export class DuplicateWarningDialog {

  private dialogRef = inject(MatDialogRef<DuplicateWarningDialog>);

  close(): void {
    this.dialogRef.close();
  }
}