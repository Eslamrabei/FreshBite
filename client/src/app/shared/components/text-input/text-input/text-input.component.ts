import { Component, Input, Self, Optional } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-text-input',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.scss']
})
export class TextInputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() type = 'text';

  constructor(@Self() @Optional() public controlDir: NgControl) {
    // This binds the "bridge" to this component
    if (this.controlDir) {
      this.controlDir.valueAccessor = this;
    }
  }

  // Helper to cast the generic AbstractControl to a FormControl so we can use it in the HTML
  get control(): FormControl {
    return this.controlDir.control as FormControl;
  }

  // --- ControlValueAccessor Interface Methods ---
  writeValue(obj: any): void { }
  registerOnChange(fn: any): void { }
  registerOnTouched(fn: any): void { }
}
