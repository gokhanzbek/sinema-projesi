import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { catchError, finalize, of } from 'rxjs';
import { swalDark } from '../core/swal-dark-theme';
import { UserProfile, UserProfileService } from '../services/user-profile.service';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.scss'
})
export class UserProfileComponent implements OnInit {
  private readonly profileService = inject(UserProfileService);
  private readonly fb = inject(FormBuilder);

  profile: UserProfile | null = null;
  loadError: string | null = null;
  loadingProfile = true;

  submittingPassword = false;
  passwordFormError: string | null = null;

  readonly passwordForm = this.fb.nonNullable.group({
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(3)]],
    confirmNewPassword: ['', Validators.required]
  });

  ngOnInit(): void {
    this.profileService
      .getProfile()
      .pipe(
        catchError(() => {
          this.loadError =
            'Profil yüklenemedi. Oturum süreniz dolmuş olabilir; tekrar giriş yapın.';
          return of(null);
        }),
        finalize(() => {
          this.loadingProfile = false;
        })
      )
      .subscribe((p) => {
        if (p) this.profile = p;
      });
  }

  submitPassword(): void {
    this.passwordFormError = null;
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }
    const v = this.passwordForm.getRawValue();
    if (v.newPassword !== v.confirmNewPassword) {
      this.passwordFormError = 'Yeni şifre ile tekrarı eşleşmiyor.';
      return;
    }

    this.submittingPassword = true;
    this.profileService
      .changePassword({
        currentPassword: v.currentPassword,
        newPassword: v.newPassword,
        confirmNewPassword: v.confirmNewPassword
      })
      .pipe(
        finalize(() => {
          this.submittingPassword = false;
        })
      )
      .subscribe({
        next: (r) => {
          if (r.succeeded) {
            this.passwordForm.reset();
            void swalDark.fire({
              icon: 'success',
              title: 'Şifre güncellendi',
              text: r.message || 'Yeni şifreniz kaydedildi.',
              confirmButtonText: 'Tamam',
              showCancelButton: false
            });
          } else {
            this.passwordFormError = r.message || 'İşlem tamamlanamadı.';
          }
        },
        error: (e: Error) => {
          this.passwordFormError = e.message;
        }
      });
  }
}
