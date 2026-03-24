import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';

interface QuestionnaireConfirmDialogData {
    titleKey: string;
    textKey: string;
    confirmKey: string;
    cancelKey: string;
}

@Component({
    standalone: true,
    imports: [MatDialogModule, MatButtonModule, TranslateModule],
    styleUrls: ['../../../styles/dialogs.scss'],
    templateUrl: './QuestionnaireConfirmDialog.component.html'
})
export class QuestionnaireConfirmDialog {
    readonly data = inject<QuestionnaireConfirmDialogData>(MAT_DIALOG_DATA);
    private dialogRef = inject(MatDialogRef<QuestionnaireConfirmDialog>);

    cancel(): void {
        this.dialogRef.close(false);
    }

    confirm(): void {
        this.dialogRef.close(true);
    }
}