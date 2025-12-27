import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AccountService } from '../account.service'; // Ensure you have this service created
import { ToastrService } from 'ngx-toastr';
import { TextInputComponent } from '../../shared/components/text-input/text-input/text-input.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TextInputComponent],
  templateUrl: './register.html',
  styleUrls: ['./register.scss']
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private accountService = inject(AccountService);
  private toastr = inject(ToastrService);

  // Password Regex: At least 1 Uppercase, 1 Lowercase, 1 Number, 1 Non-alphanumeric, 6 chars min
  complexPassword = "(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$";

  registerForm: FormGroup = this.fb.group({
    displayName: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.pattern(this.complexPassword)]]
  });

  onSubmit() {
    if (this.registerForm.valid) {
      // Call your AccountService (assuming it exists)
      this.accountService.register(this.registerForm.value).subscribe({
        next: () => {
          this.toastr.success('Registration successful');
          this.router.navigateByUrl('/shop');
        },
        error: (err) => {
          // If you don't have a backend yet, just mock success:
          console.log(err);
          this.toastr.error('Registration failed (Backend needed)', 'Error');
        }
      });
    } else {
      this.registerForm.markAllAsTouched();
    }
  }
}
