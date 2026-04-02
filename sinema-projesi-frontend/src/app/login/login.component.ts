import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly form = this.fb.nonNullable.group({
    usernameOrEmail: ['', Validators.required],
    password: ['', Validators.required]
  });

  errorMessage: string | null = null;
  submitting = false;

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitting = true;
    this.errorMessage = null;
    const { usernameOrEmail, password } = this.form.getRawValue();
    this.auth.login(usernameOrEmail.trim(), password).subscribe((r) => {
      this.submitting = false;
      if (!r.ok) {
        this.errorMessage = r.message ?? 'Giriş başarısız.';
        return;
      }
      const raw = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
      this.router.navigateByUrl(this.sanitizeReturnUrl(raw));
    });
  }

  private sanitizeReturnUrl(url: string): string {
    let t = url.trim();
    try {
      t = decodeURIComponent(t);
    } catch {
      /* keep trimmed */
    }
    t = t.trim();
    if (!t.startsWith('/') || t.includes('//')) {
      return '/';
    }
    return t;
  }
}
