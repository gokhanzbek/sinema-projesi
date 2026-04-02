import { AsyncPipe, NgIf } from '@angular/common';
import { Component, HostListener, inject } from '@angular/core';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { filter, map } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, AsyncPipe, NgIf],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  menuOpen = false;

  readonly navAuth$ = this.auth.isLoggedIn$.pipe(
    map((loggedIn) => ({
      loggedIn,
      initials: loggedIn ? this.auth.getUserInitials() : '',
      isAdmin: loggedIn && this.auth.isAdmin()
    }))
  );

  constructor() {
    this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(() => {
        this.menuOpen = false;
      });
  }

  @HostListener('document:click')
  onDocumentClick(): void {
    this.menuOpen = false;
  }

  toggleMenu(event: MouseEvent): void {
    event.stopPropagation();
    this.menuOpen = !this.menuOpen;
  }

  onUserAreaClick(event: MouseEvent): void {
    event.stopPropagation();
  }

  logout(): void {
    this.auth.logout();
    this.menuOpen = false;
    void this.router.navigateByUrl('/');
  }
}
