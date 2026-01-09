import { Directive, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { fromEvent, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, takeUntil } from 'rxjs/operators';

/**
 * Directive to debounce input events.
 * 
 * Usage:
 * ```html
 * <input 
 *   type="text" 
 *   appDebouncedInput 
 *   [debounceTime]="500"
 *   (debouncedInput)="onSearch($event)" 
 * />
 * ```
 * 
 * This will emit the input value after 500ms of no typing.
 */
@Directive({
  selector: '[appDebouncedInput]',
  standalone: true
})
export class DebouncedInputDirective implements OnInit, OnDestroy {
  @Input() debounceTime = 400;
  @Output() debouncedInput = new EventEmitter<string>();

  private destroy$ = new Subject<void>();

  constructor(private elementRef: ElementRef<HTMLInputElement>) {}

  ngOnInit(): void {
    fromEvent(this.elementRef.nativeElement, 'input')
      .pipe(
        map((event: Event) => (event.target as HTMLInputElement).value),
        debounceTime(this.debounceTime),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe((value) => {
        this.debouncedInput.emit(value);
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
