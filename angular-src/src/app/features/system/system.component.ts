import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { LoadingComponent } from '../../shared/loading/loading.component';
import { SystemConfirmDialogComponent, SystemConfirmDialogData } from './dialog/system-confirm-dialog/system-confirm-dialog.component';
import { ApplicationLog, EventIdDto, SystemService } from './services/system.service';

type LogLevel = 'Trace' | 'Debug' | 'Information' | 'Warning' | 'Error' | 'Critical' | 'None';

interface AggregatedLogFile {
  year: number;
  months: AggregatedLogMonth[];
  _yearExpanded?: boolean;
}

interface AggregatedLogMonth {
  month: number;
  files: string[];
  _monthExpanded?: boolean;
}

interface SettingsField {
  section: string;
  key: string;
  type: string;
  required: boolean;
  isSecret: boolean;
  inputKind: 'string' | 'integer' | 'boolean' | 'json';
  hidden?: boolean;
  description?: string;
  value: unknown;
  originalValue: unknown;
}

@Component({
  selector: 'app-system',
  standalone: true,
  imports: [CommonModule, FormsModule, MatTabsModule, TranslateModule, LoadingComponent],
  templateUrl: './system.component.html',
  styleUrl: './system.component.css'
})
export class SystemComponent {
  private systemService = inject(SystemService);
  private dialog = inject(MatDialog);
  private translate = inject(TranslateService);

  readonly logLevels: LogLevel[] = ['Trace', 'Debug', 'Information', 'Warning', 'Error', 'Critical', 'None'];

  isLoading = false;
  message = '';
  isErrorMessage = false;

  selectedLogLevel: LogLevel = 'Information';
  categories: string[] = [];
  events: EventIdDto[] = [];
  selectedCategories = new Set<string>();
  selectedEvents = new Set<number>();
  logs: ApplicationLog[] = [];

  logFiles: string[] = [];
  aggregatedLogFiles: AggregatedLogFile[] = [];

  settingsJson = '';
  settingsSchemaJson = '';
  defaultSettingsJson = '';
  settingsSchema: unknown = null;
  currentSettings: Record<string, unknown> = {};
  currentSettingsIndex = new Map<string, unknown>();

  updateSettingsJson = '';
  patchSettingsJson = '';

  settingsTabs: string[] = [];
  selectedSettingsTab = 0;
  settingsFields: SettingsField[] = [];
  useAdvancedMode = false;
  selectedImportFile: File | null = null;

  ngOnInit(): void {
    this.loadLogMetadata();
    this.loadLogFiles();
    this.loadSettings();
    this.loadSettingsSchema();
    this.loadDefaultSettings();
  }

  ping(): void {
    this.startBusy();
    this.systemService.ping().pipe(finalize(() => this.stopBusy())).subscribe({
      next: () => this.setMessage('SYSTEM.MESSAGES.PING_OK'),
      error: () => this.setMessage('SYSTEM.MESSAGES.PING_FAILED', true)
    });
  }

  loadLogMetadata(): void {
    this.startBusy();
    this.systemService.getDatabaseLogCategories().pipe(finalize(() => this.stopBusy())).subscribe({
      next: categories => {
        this.categories = categories;
      },
      error: () => this.setMessage('SYSTEM.MESSAGES.LOG_METADATA_FAILED', true)
    });

    this.systemService.getDatabaseLogEvents().subscribe({
      next: events => {
        this.events = events;
      },
      error: () => this.setMessage('SYSTEM.MESSAGES.LOG_METADATA_FAILED', true)
    });
  }

  toggleCategory(category: string, checked: boolean): void {
    if (checked) {
      this.selectedCategories.add(category);
      return;
    }
    this.selectedCategories.delete(category);
  }

  toggleEvent(eventId: number, checked: boolean): void {
    if (checked) {
      this.selectedEvents.add(eventId);
      return;
    }
    this.selectedEvents.delete(eventId);
  }

  loadLogs(): void {
    this.startBusy();
    this.systemService.getDatabaseLogs({
      logLevel: this.selectedLogLevel,
      categories: [...this.selectedCategories],
      events: [...this.selectedEvents]
    }).pipe(finalize(() => this.stopBusy())).subscribe({
      next: logs => {
        this.logs = logs;
        this.setMessage('SYSTEM.MESSAGES.LOGS_LOADED');
      },
      error: () => this.setMessage('SYSTEM.MESSAGES.LOGS_FAILED', true)
    });
  }

  clearLogFilters(): void {
    this.selectedLogLevel = 'Information';
    this.selectedCategories.clear();
    this.selectedEvents.clear();
  }

  loadLogFiles(): void {
    this.startBusy();
    this.systemService.getLogFileNames().pipe(finalize(() => this.stopBusy())).subscribe({
      next: files => {
        this.logFiles = files;
        this.aggregatedLogFiles = this.aggregizeLogFiles(files);
      },
      error: () => this.setMessage('SYSTEM.MESSAGES.LOG_FILES_FAILED', true)
    });
  }

  private aggregizeLogFiles(files: string[]): AggregatedLogFile[] {
    const dateMap = new Map<string, Map<string, string[]>>();

    files.forEach(file => {
      const dateMatch = file.match(/(\d{4})(\d{2})(\d{2})/);
      if (dateMatch) {
        const year = dateMatch[1];
        const month = dateMatch[2];

        if (!dateMap.has(year)) {
          dateMap.set(year, new Map());
        }
        const yearMap = dateMap.get(year)!;
        if (!yearMap.has(month)) {
          yearMap.set(month, []);
        }
        yearMap.get(month)!.push(file);
      } else {
        // Files without date pattern
        if (!dateMap.has('other')) {
          dateMap.set('other', new Map());
        }
        const otherMap = dateMap.get('other')!;
        if (!otherMap.has('other')) {
          otherMap.set('other', []);
        }
        otherMap.get('other')!.push(file);
      }
    });

    return Array.from(dateMap.entries())
      .sort((a, b) => {
        if (a[0] === 'other') return 1;
        if (b[0] === 'other') return -1;
        return b[0].localeCompare(a[0]);
      })
      .map(([year, monthMap]) => ({
        year: isNaN(Number(year)) ? 0 : Number(year),
        months: Array.from(monthMap.entries())
          .sort((a, b) => b[0].localeCompare(a[0]))
          .map(([month, fileList]) => ({
            month: isNaN(Number(month)) ? 0 : Number(month),
            files: fileList
          }))
      }));
  }

  downloadLogFile(fileName: string): void {
    this.startBusy();
    this.systemService.downloadLogFile(fileName).pipe(finalize(() => this.stopBusy())).subscribe({
      next: blob => {
        this.downloadBlob(blob, fileName);
        this.setMessage('SYSTEM.MESSAGES.LOG_DOWNLOAD_OK');
      },
      error: () => this.setMessage('SYSTEM.MESSAGES.LOG_DOWNLOAD_FAILED', true)
    });
  }

  loadSettings(): void {
    this.startBusy();
    this.systemService.getSettings().pipe(finalize(() => this.stopBusy())).subscribe({
      next: settings => {
        this.settingsJson = JSON.stringify(settings, null, 2);
        this.currentSettings = settings as Record<string, unknown>;
        this.currentSettingsIndex = this.createSettingsIndex(this.currentSettings);
        this.buildSettingsFields();
      },
      error: () => this.setMessage('SYSTEM.MESSAGES.SETTINGS_FAILED', true)
    });
  }

  loadSettingsSchema(): void {
    this.startBusy();
    this.systemService.getSettingsSchema().pipe(finalize(() => this.stopBusy())).subscribe({
      next: schema => {
        this.settingsSchemaJson = JSON.stringify(schema, null, 2);
        this.settingsSchema = schema;
        this.buildSettingsFields();
      },
      error: () => this.setMessage('SYSTEM.MESSAGES.SCHEMA_FAILED', true)
    });
  }

  private createSettingsIndex(obj: Record<string, unknown>, prefix = ''): Map<string, unknown> {
    const index = new Map<string, unknown>();

    const walk = (value: unknown, currentPath: string): void => {
      if (currentPath) {
        index.set(currentPath, value);
      }

      if (value && typeof value === 'object' && !Array.isArray(value)) {
        Object.entries(value as Record<string, unknown>).forEach(([key, child]) => {
          const childPath = currentPath ? `${currentPath}.${key}` : key;
          walk(child, childPath);
        });
      }
    };

    walk(obj, prefix);
    return index;
  }

  private normalizeSettingsPath(path: string): string {
    return path.replace(/[._\-\s]/g, '').toLowerCase();
  }

  private getSettingValueByPath(path: string): unknown {
    if (this.currentSettingsIndex.has(path)) {
      return this.currentSettingsIndex.get(path);
    }

    const normalizedTarget = this.normalizeSettingsPath(path);
    const matchedEntry = Array.from(this.currentSettingsIndex.entries()).find(([existingPath]) =>
      this.normalizeSettingsPath(existingPath) === normalizedTarget
    );

    return matchedEntry?.[1];
  }

  private buildSettingsFields(): void {
    if (!this.settingsSchema) return;

    const schema = this.settingsSchema as Record<string, unknown>;
    const fields: SettingsField[] = [];
    const tabSet = new Set<string>();

    Object.entries(schema).forEach(([section, sectionDef]) => {
      if (!sectionDef || typeof sectionDef !== 'object') return;

      tabSet.add(section);
      const sectionObj = sectionDef as Record<string, unknown>;

      Object.entries(sectionObj).forEach(([fieldKey, fieldDef]) => {
        if (!fieldDef || typeof fieldDef !== 'object') return;

        const fieldObj = fieldDef as Record<string, unknown>;
        const keyPath = `${section}.${fieldKey}`;
        const existingValue = this.getSettingValueByPath(keyPath);
        const inputKind = this.getInputKind((fieldObj['type'] as string) || 'string', existingValue);
        const isSecret = Boolean(fieldObj['isSecret'] ?? fieldObj['IsSecret']);
        const normalizedValue = this.toFieldDisplayValue(existingValue, inputKind);

        fields.push({
          section,
          key: fieldKey,
          type: (fieldObj['type'] as string) || 'string',
          required: (fieldObj['required'] as boolean) || false,
          isSecret,
          inputKind,
          hidden: isSecret,
          description: (fieldObj['description'] as string) || '',
          value: normalizedValue,
          originalValue: normalizedValue
        });
      });
    });

    this.settingsTabs = Array.from(tabSet).sort();
    this.settingsFields = fields;
    this.selectedSettingsTab = 0;
  }

  private getInputKind(schemaType: string, existingValue: unknown): 'string' | 'integer' | 'boolean' | 'json' {
    const normalizedType = (schemaType || '').toLowerCase();

    if (Array.isArray(existingValue)) {
      return 'json';
    }

    if (existingValue !== null && typeof existingValue === 'object') {
      return 'json';
    }

    if (normalizedType.includes('dictionary') || normalizedType.includes('list') || normalizedType.includes('array') || normalizedType.includes('object')) {
      return 'json';
    }

    if (normalizedType === 'boolean' || typeof existingValue === 'boolean') {
      return 'boolean';
    }

    if (normalizedType.includes('int') || normalizedType.includes('number') || typeof existingValue === 'number') {
      return 'integer';
    }

    return 'string';
  }

  private toFieldDisplayValue(value: unknown, inputKind: 'string' | 'integer' | 'boolean' | 'json'): unknown {
    if (inputKind === 'json') {
      if (value === undefined || value === null || value === '') {
        return '';
      }

      if (typeof value === 'string') {
        return value;
      }

      try {
        return JSON.stringify(value, null, 2);
      } catch {
        return String(value);
      }
    }

    return value ?? '';
  }

  toggleSecretVisibility(field: SettingsField): void {
    field.hidden = !field.hidden;
  }

  toggleAdvancedMode(): void {
    this.useAdvancedMode = !this.useAdvancedMode;
  }

  hasSettingsChanges(): boolean {
    return this.settingsFields.some(f => f.value !== f.originalValue);
  }

  resetSettingsTab(section: string): void {
    this.settingsFields.filter(f => f.section === section).forEach(f => {
      f.value = f.originalValue;
    });
  }

  getYearFilesCount(yearGroup: AggregatedLogFile): number {
    return yearGroup.months.reduce((total, month) => total + month.files.length, 0);
  }

  getMonthName(monthNumber: number): string {
    if (monthNumber === 0) return '';
    const date = new Date(2000, monthNumber - 1, 1);
    const locale = this.translate.getCurrentLang() || 'en';
    const monthName = date.toLocaleString(locale, { month: 'long' });
    return monthName.charAt(0).toUpperCase() + monthName.slice(1);
  }

  getSettingsSectionLabel(tabName: string): string {
    const titleCase = tabName.charAt(0).toUpperCase() + tabName.slice(1).toLowerCase();
    const upperCase = tabName.toUpperCase();
    const lowerCase = tabName.toLowerCase();
    const candidates = [
      `SYSTEM.SETTINGS.SECTION_${tabName}`,
      `SYSTEM.SETTINGS.SECTION_${titleCase}`,
      `SYSTEM.SETTINGS.SECTION_${upperCase}`,
      `SYSTEM.SETTINGS.SECTION_${lowerCase}`
    ];

    for (const key of candidates) {
      const translated = this.translate.instant(key);
      if (translated !== key) {
        return translated;
      }
    }

    return tabName;
  }

  loadDefaultSettings(): void {
    this.startBusy();
    this.systemService.getDefaultSettings().pipe(finalize(() => this.stopBusy())).subscribe({
      next: defaults => {
        this.defaultSettingsJson = JSON.stringify(defaults, null, 2);
      },
      error: () => this.setMessage('SYSTEM.MESSAGES.DEFAULTS_FAILED', true)
    });
  }

  updateSettings(): void {
    const payload = this.useAdvancedMode
      ? this.parseJson(this.updateSettingsJson)
      : this.buildUpdatePayloadFromFields();

    if (payload == null) {
      this.setMessage('SYSTEM.MESSAGES.INVALID_JSON', true);
      return;
    }

    this.openConfirmDialog({
      titleKey: 'SYSTEM.CONFIRM.UPDATE_TITLE',
      textKey: 'SYSTEM.CONFIRM.UPDATE_TEXT',
      confirmKey: 'SYSTEM.CONFIRM.UPDATE_BUTTON'
    }, () => {
      this.startBusy();
      this.systemService.updateSettings(payload).pipe(finalize(() => this.stopBusy())).subscribe({
        next: () => {
          this.setMessage('SYSTEM.MESSAGES.UPDATE_OK');
          this.loadSettings();
        },
        error: () => this.setMessage('SYSTEM.MESSAGES.UPDATE_FAILED', true)
      });
    });
  }

  private buildUpdatePayloadFromFields(): Record<string, unknown> | null {
    const payload: Record<string, unknown> = {};

    for (const field of this.settingsFields) {
      let parsedValue: unknown = field.value;

      if (field.inputKind === 'integer') {
        if (typeof parsedValue === 'string') {
          const trimmed = parsedValue.trim();
          if (trimmed.length === 0) {
            parsedValue = 0;
          } else {
            const n = Number(trimmed);
            if (Number.isNaN(n)) {
              return null;
            }
            parsedValue = n;
          }
        }
      }

      if (field.inputKind === 'boolean') {
        parsedValue = Boolean(parsedValue);
      }

      if (field.inputKind === 'json') {
        if (typeof parsedValue === 'string') {
          const trimmed = parsedValue.trim();
          if (trimmed.length === 0) {
            parsedValue = {};
          } else {
            try {
              parsedValue = JSON.parse(trimmed);
            } catch {
              return null;
            }
          }
        }
      }

      this.setNestedValue(payload, `${field.section}.${field.key}`, parsedValue);
    }

    return payload;
  }

  private setNestedValue(target: Record<string, unknown>, path: string, value: unknown): void {
    const segments = path.split('.');
    let cursor: Record<string, unknown> = target;

    for (let i = 0; i < segments.length; i++) {
      const segment = segments[i];
      const isLeaf = i === segments.length - 1;

      if (isLeaf) {
        cursor[segment] = value;
        return;
      }

      const existing = cursor[segment];
      if (!existing || typeof existing !== 'object' || Array.isArray(existing)) {
        cursor[segment] = {};
      }

      cursor = cursor[segment] as Record<string, unknown>;
    }
  }

  patchSettings(): void {
    const payload = this.parseJson(this.patchSettingsJson);
    if (payload == null) {
      this.setMessage('SYSTEM.MESSAGES.INVALID_JSON', true);
      return;
    }

    this.openConfirmDialog({
      titleKey: 'SYSTEM.CONFIRM.PATCH_TITLE',
      textKey: 'SYSTEM.CONFIRM.PATCH_TEXT',
      confirmKey: 'SYSTEM.CONFIRM.PATCH_BUTTON'
    }, () => {
      this.startBusy();
      this.systemService.patchSettings(payload).pipe(finalize(() => this.stopBusy())).subscribe({
        next: () => {
          this.setMessage('SYSTEM.MESSAGES.PATCH_OK');
          this.loadSettings();
        },
        error: () => this.setMessage('SYSTEM.MESSAGES.PATCH_FAILED', true)
      });
    });
  }

  exportSettings(): void {
    this.startBusy();
    this.systemService.exportSettings().pipe(finalize(() => this.stopBusy())).subscribe({
      next: blob => {
        this.downloadBlob(blob, 'settings-export.json');
        this.setMessage('SYSTEM.MESSAGES.EXPORT_OK');
      },
      error: () => this.setMessage('SYSTEM.MESSAGES.EXPORT_FAILED', true)
    });
  }

  onImportFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedImportFile = input.files?.[0] ?? null;
  }

  importSettings(): void {
    if (!this.selectedImportFile) {
      this.setMessage('SYSTEM.MESSAGES.IMPORT_FILE_REQUIRED', true);
      return;
    }

    this.openConfirmDialog({
      titleKey: 'SYSTEM.CONFIRM.IMPORT_TITLE',
      textKey: 'SYSTEM.CONFIRM.IMPORT_TEXT',
      confirmKey: 'SYSTEM.CONFIRM.IMPORT_BUTTON'
    }, () => {
      this.startBusy();
      this.systemService.importSettings(this.selectedImportFile!).pipe(finalize(() => this.stopBusy())).subscribe({
        next: () => {
          this.setMessage('SYSTEM.MESSAGES.IMPORT_OK');
          this.selectedImportFile = null;
          this.loadSettings();
        },
        error: () => this.setMessage('SYSTEM.MESSAGES.IMPORT_FAILED', true)
      });
    });
  }

  stopServer(): void {
    this.openConfirmDialog({
      titleKey: 'SYSTEM.CONFIRM.STOP_TITLE',
      textKey: 'SYSTEM.CONFIRM.STOP_TEXT',
      confirmKey: 'SYSTEM.CONFIRM.STOP_BUTTON'
    }, () => {
      this.startBusy();
      this.systemService.stopServer().pipe(finalize(() => this.stopBusy())).subscribe({
        next: () => this.setMessage('SYSTEM.MESSAGES.STOP_OK'),
        error: () => this.setMessage('SYSTEM.MESSAGES.STOP_FAILED', true)
      });
    });
  }

  private openConfirmDialog(data: SystemConfirmDialogData, onConfirm: () => void): void {
    const dialogRef = this.dialog.open(SystemConfirmDialogComponent, {
      data,
      maxWidth: '560px',
      width: '90vw'
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        onConfirm();
      }
    });
  }

  private parseJson(value: string): unknown | null {
    try {
      return JSON.parse(value);
    } catch {
      return null;
    }
  }

  private downloadBlob(blob: Blob, fileName: string): void {
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = fileName;
    anchor.click();
    URL.revokeObjectURL(url);
  }

  private startBusy(): void {
    this.isLoading = true;
  }

  private stopBusy(): void {
    this.isLoading = false;
  }

  private setMessage(key: string, isError = false): void {
    this.message = key;
    this.isErrorMessage = isError;
  }
}
