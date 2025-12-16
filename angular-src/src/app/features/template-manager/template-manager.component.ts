import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { TemplateService } from './services/template.service';
import { TemplateEditorComponent } from './template-editor/template-editor.component';
import { PaginationComponent, PageChangeEvent } from '../../shared/components/pagination/pagination.component';
import { LoadingComponent } from '../../shared/loading/loading.component';
import { Template, TemplateBase, TemplateStatus } from '../../shared/models/template.model';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { DebouncedInputDirective } from '../../shared/directives/debounced-input.directive';
import { DeleteTemplateDialog } from './delete-template-modal/DeleteTemplateDialog.component';
import { CopyTemplateDialog } from './copy-template-modal/CopyTemplateDialog.component';
import { ErrorDialog } from './error-modal/ErrorDialog.component';
import { MatDialog } from '@angular/material/dialog';



/**
 * Template manager component.
 *
 * Provides an interface for questionnaire templates.
 *
 * Handles:
 * - Listing and searching templates (cursor pagination).
 * - Creating, editing, finalizing, copying, and deleting.
 * - Two-step delete confirmation via modal.
 *
 * Notes:
 * - Caches cursors by page to reduce refetching.
 * - Debounces search input (300 ms) to minimize API calls.
 * - Uses translation keys for all labels and default values.
 */
@Component({
    selector: 'app-template-manager',
    standalone: true,
    imports: [
        TemplateEditorComponent,
        FormsModule,
        CommonModule,
        PaginationComponent,
        LoadingComponent,
        TranslateModule,
        DebouncedInputDirective
    ],
    templateUrl: './template-manager.component.html',
    styleUrls: ['./template-manager.component.css']
})
export class TemplateManagerComponent {
  private templateService = inject(TemplateService);
  private translate = inject(TranslateService);
  private dialog = inject(MatDialog);

  templateBases: TemplateBase[] = [];
  cachedCursors: { [pageNumber: number]: string | null } = {};
  selectedTemplate: Template | null = null;

  templateStatus = TemplateStatus
  

  // Search & Pagination parameters.
  searchTerm = '';
  searchType: 'name' | 'id' = 'name';
  currentPage = 1;
  pageSize = 5;
  totalPages = 1;
  pageSizeOptions: number[] = [5, 10, 15, 20];

  isLoading = false;
  private searchSubject = new Subject<string>();

  lockedTitle = 'Skabelonen er udgivet';
  lockedText =
    'Denna skabelon er i skrivebeskyttet tilstand.<br />' +
    'Vælg <strong>Kopiér</strong> i listen, hvis du vil lave ændringer på en ' +
    'redigerbar version.';

 /**
 * Wire up debounced search and load the first page.
 * Debounce prevents excessive calls while typing; distinctUntilChanged
 * avoids re-querying the same term.
 */
  ngOnInit(): void {
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.searchTerm = term;
        this.resetData();
        this.fetchTemplateBases();
      });
    this.fetchTemplateBases();
  }

  /** Push new search terms into the debounced pipeline. */
  onSearchChange(term: string): void {
    this.searchSubject.next(term);
  }
  /**
   * Change items-per-page and fetch the first page with the new size.
   */
  onPageSizeChange(newSize: string): void {
    this.pageSize = parseInt(newSize, 10);
    this.resetData();
    this.fetchTemplateBases();
  }

  /** Toggle server-side search type ('name'|'id'), */
  onSearchTypeChange(type: string): void {
    if (type === 'name' || type === 'id') {
      this.searchType = type;
      this.resetData();
      this.fetchTemplateBases();
    }
  }

 /**
 * Reset pagination state and (optionally) search parameters.
 * Also clears the currently selected template/editor view.
 */
  resetData(resetSearch: boolean = false): void {
    this.currentPage = 1;
    this.totalPages = 1;
    this.cachedCursors = {};
    this.selectedTemplate = null;
    if (resetSearch) {
      this.searchTerm = '';
      this.searchType = 'name';
    }
  }

  /** Fetch a page of template bases. */
  private fetchTemplateBases(): void {
    this.isLoading = true;
    const nextCursor = this.cachedCursors[this.currentPage] ?? undefined;
    this.templateService
      .getTemplateBases(this.pageSize, nextCursor, this.searchTerm, this.searchType)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (response) => {
          this.templateBases = response.templateBases;
          if (response.templateBases.length === 0) {
            this.totalPages = this.currentPage;
            this.cachedCursors[this.currentPage + 1] = null;
          } else if (response.queryCursor) {
            this.cachedCursors[this.currentPage + 1] = response.queryCursor;
            this.totalPages = Math.ceil(response.totalCount / this.pageSize);
          } else {
            this.totalPages = this.currentPage;
          }
        },
        error: (err) => {
          console.error('Error fetching templates:', err);
          this.openErrorDialog('TEMPLATE.LIST.FAIL_LOAD_LIST');
        },
      });
  }

/**
 * Handles next/prev button clicks from PaginationComponent.
 * If moving forward without a known cursor, performs a fetch to acquire it.
 */
  handlePageChange(event: PageChangeEvent): void {
    const newPage = event.page;
    if (event.direction === 'forward' && newPage > this.currentPage && !this.cachedCursors[newPage]) {
      console.warn('Page not available yet. Fetching...');
      this.currentPage = newPage;
      this.fetchTemplateBases();
      return;
    }
    this.currentPage = newPage;
    this.fetchTemplateBases();
  }
  /**
   * takes the id of an template makes it the selected template for editor panel
   * Sets `isLoading` to cover the transition to the editor.
   * @param id templateId
   */
selectTemplate(id: string): void {
  this.selectedTemplate = null;
  this.isLoading = true;
  this.templateService.getTemplateDetails(id)
    .pipe(finalize(() => (this.isLoading = false)))
    .subscribe({
      next: tmpl => {
        this.selectedTemplate = tmpl;
      },
        error: err => {
        console.error('Error fetching template:', err);
        this.openErrorDialog('TEMPLATE.LIST.FAIL_LOAD_DETAIL');
      }
    });
}

/**
 * Promote a draft template to Finalized on the server,
 * then clear the editor and refresh the list.
 */
onFinalizeTemplate(tmpl: Template): void {
  if (tmpl.id){
  this.templateService.upgradeTemplate(tmpl.id).subscribe({
    next: () => {
      this.selectedTemplate = null;   // close editor
      this.fetchTemplateBases();      // refresh list
    },
    error: err => console.error('Upgrade failed', err)
  });
  }
}

/**
 * Create a local draft with translated defaults
 * and open it in the editor (not persisted until Save).
 */
  addTemplate(): void {
    this.selectedTemplate = {
      id: '',
      templateStatus: TemplateStatus.Draft,
      title: this.translate.instant('TEMPLATE.NEW.TITLE'), //  New Template
      description: this.translate.instant('TEMPLATE.NEW.DESC'), //Description for the new template
      questions: [
        {
          id: -1,
          prompt: this.translate.instant('TEMPLATE.NEW.STANDARD_NEW_QUESTION'), //Default Question
          allowCustom: true,
          options: [],
          sortOrder: 0
        },
      ],
    };
  }
  // addTemplate(): void {
  //   this.selectedTemplate = {
  //     id: '',
  //     title: ' New Template', //  
  //     description: 'Description for the new template', //
  //     questions: [
  //       {
  //         id: -1,
  //         prompt: 'Default Question', //Default Question
  //         allowCustom: true,
  //         options: [],
  //       },
  //     ],
  //   };
  // }

  isSelectedPageSize(size: number): boolean {
    return size === this.pageSize;
  }

/**
 * updates the current editor state.
 * - If no id: creates a new template then resets list & closes editor.
 * - If id exists: updates the template and refreshes the list.
 */
  onSaveTemplate(updatedTemplate: Template): void {
    if (!updatedTemplate.id) {
      updatedTemplate.id = `temp-${Date.now()}`;
      this.templateService.addTemplate(updatedTemplate).subscribe({
        next: (createdTemplate: Template) => {
          console.log('Template added successfully:', createdTemplate);
          this.selectedTemplate = null;
          this.resetData();
          this.fetchTemplateBases();
        },
        error: (err) => {
          console.error('Error adding template:', err);
          updatedTemplate.id = undefined;
          this.handleSaveError(err);
        },
      });
    } else {
      this.templateService.updateTemplate(updatedTemplate.id, updatedTemplate).subscribe({
        complete: () => {
          console.log('Template updated successfully:', updatedTemplate);
          this.selectedTemplate = null;
          this.fetchTemplateBases();
        },
        error: (err) => {
          console.error('Error updating template:', err);
          this.handleSaveError(err);
        },
      });
    }
  }

  private handleSaveError(err: any): void {
    let errorKey: string;
    if (err.status === 409) {
      // Conflict - duplicate title - always show user-friendly message
      errorKey = 'TEMPLATE.MISC.ERROR_DUPLICATE_TITLE';
    } else if (err.status === 400) {
      // Bad request - validation error
      errorKey = 'TEMPLATE.MISC.ERROR_VALIDATION';
    } else {
      // General error
      errorKey = 'TEMPLATE.MISC.ERROR_SAVE';
    }
    
    this.openErrorDialog(errorKey);
  }

  onCancelEdit(): void {
    this.selectedTemplate = null;
  }





















openDeleteDialog(templateId: string): void {
  this.dialog
    .open(DeleteTemplateDialog, {
      panelClass: 'app-modal',
      maxWidth: '28rem',   // matches max-w-md
      width: '100%',
      disableClose: true,
      data: templateId,
    })
    .afterClosed()
    .subscribe(deleted => {
      if (deleted) {
        this.resetData();
        this.fetchTemplateBases();
      }
    });
}

openCopyDialog(templateId: string): void {
  this.dialog
    .open(CopyTemplateDialog, {
      panelClass: 'app-modal',
      maxWidth: '28rem',   // matches max-w-md
      width: '100%',
      disableClose: true,
      data: templateId,
    })
    .afterClosed()
    .subscribe(copiedTemplate => {
      if (copiedTemplate) {
        this.selectedTemplate = copiedTemplate;   // open the new local draft immediately
      }
    });
}

openErrorDialog(errorMessageKey: string): void {
  this.dialog.open(ErrorDialog, {
    panelClass: 'app-modal',
    maxWidth: '28rem',   // matches max-w-md
    width: '100%',
    disableClose: false,
    data: errorMessageKey,
  });
}


}
