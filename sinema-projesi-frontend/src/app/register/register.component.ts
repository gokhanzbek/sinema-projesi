import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  submitting = false;
  errorMessage: string | null = null;

  readonly form = this.fb.nonNullable.group({
    userName: ['', Validators.required],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', Validators.required]
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    if (v.password !== v.confirmPassword) {
      this.errorMessage = 'Şifreler eşleşmiyor.';
      return;
    }
    this.submitting = true;
    this.errorMessage = null;
    this.auth
      .register({
        userName: v.userName.trim(),
        firstName: v.firstName.trim(),
        lastName: v.lastName.trim(),
        email: v.email.trim(),
        password: v.password,
        confirmPassword: v.confirmPassword
      })
      .subscribe((res) => {
        this.submitting = false;
        if (res.succeeded) {
          void this.router.navigate(['/login']);
          return;
        }
        this.errorMessage = res.message || 'Kayıt başarısız.';
      });
  }
}
