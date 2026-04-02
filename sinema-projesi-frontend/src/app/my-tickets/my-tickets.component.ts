import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, finalize, of } from 'rxjs';
import { swalDark } from '../core/swal-dark-theme';
import { GetMyTicketsResult, MyTicketDto, TicketService } from '../services/ticket.service';

@Component({
  selector: 'app-my-tickets',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './my-tickets.component.html',
  styleUrl: './my-tickets.component.scss'
})
export class MyTicketsComponent implements OnInit {
  private readonly ticketService = inject(TicketService);
  private readonly cdr = inject(ChangeDetectorRef);

  tickets: MyTicketDto[] = [];
  isLoading = true;
  errorMessage: string | null = null;
  /** İptal isteği giden bilet (buton disabled) */
  cancelingId: number | null = null;

  ngOnInit(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.ticketService
      .getMyTickets()
      .pipe(
        catchError(() =>
          of({
            succeeded: false,
            message: 'Sunucuya ulaşılamadı veya oturum süresi doldu.',
            tickets: [] as MyTicketDto[]
          } satisfies GetMyTicketsResult)
        ),
        finalize(() => {
          this.isLoading = false;
          this.cdr.markForCheck();
        })
      )
      .subscribe({
        next: (res: GetMyTicketsResult) => {
          try {
            if (!res.succeeded) {
              this.errorMessage = res.message ?? 'Biletler yüklenemedi.';
              this.tickets = [];
              return;
            }
            this.errorMessage = null;
            const list = Array.isArray(res.tickets) ? res.tickets : [];
            this.tickets = this.sortTickets(list);
          } catch {
            this.errorMessage = 'Bilet verisi işlenirken hata oluştu.';
            this.tickets = [];
          }
        },
        error: () => {
          this.errorMessage = 'Biletler alınamadı.';
          this.tickets = [];
          this.isLoading = false;
          this.cdr.markForCheck();
        }
      });
  }

  async onCancelClick(ticket: MyTicketDto): Promise<void> {
    if (ticket.status !== 1 || this.cancelingId != null) return;

    const confirm = await swalDark.fire({
      title: 'Emin misin?',
      text: 'Bu bileti iptal etmek istediğine emin misin? İptal edilen bilet geri alınamaz; koltuk yeniden satışa açılır.',
      icon: 'warning',
      showCancelButton: true,
      focusCancel: true,
      confirmButtonText: 'Evet, iptal et',
      cancelButtonText: 'Vazgeç'
    });

    if (!confirm.isConfirmed) return;

    this.cancelingId = ticket.id;
    this.cdr.markForCheck();

    this.ticketService
      .cancelTicket(ticket.id)
      .pipe(
        finalize(() => {
          this.cancelingId = null;
          this.cdr.markForCheck();
        })
      )
      .subscribe((out) => {
        if (out.succeeded) {
          this.applyTicketCancelled(ticket.id);
          void swalDark.fire({
            icon: 'success',
            title: 'Bilet iptal edildi',
            text: out.message ?? 'Biletin başarıyla iptal edildi.',
            confirmButtonText: 'Tamam',
            showCancelButton: false,
            customClass: {
              popup: 'swal-neon-popup',
              title: 'swal-neon-title',
              htmlContainer: 'swal-neon-text',
              confirmButton: 'swal-neon-btn swal-neon-btn--accent',
              actions: 'swal-neon-actions',
              icon: 'swal-neon-icon'
            }
          });
          return;
        }
        void swalDark.fire({
          icon: 'error',
          title: 'İptal edilemedi',
          text: out.message ?? 'Sunucu iptal isteğini reddetti.',
          confirmButtonText: 'Tamam',
          showCancelButton: false,
          customClass: {
            popup: 'swal-neon-popup',
            title: 'swal-neon-title',
            htmlContainer: 'swal-neon-text',
            confirmButton: 'swal-neon-btn swal-neon-btn--ghost',
            actions: 'swal-neon-actions',
            icon: 'swal-neon-icon'
          }
        });
      });
  }

  private applyTicketCancelled(ticketId: number): void {
    this.tickets = this.sortTickets(
      this.tickets.map((t) => (t.id === ticketId ? { ...t, status: 2 } : t))
    );
    this.cdr.markForCheck();
  }

  /** Aktif (1) → Geçmiş (3) → İptal (2) */
  private sortTickets(items: MyTicketDto[]): MyTicketDto[] {
    const rank: Record<number, number> = { 1: 0, 3: 1, 2: 2 };
    return [...items].sort((a, b) => {
      const sa = Number(a.status);
      const sb = Number(b.status);
      const ra = rank[sa] ?? 99;
      const rb = rank[sb] ?? 99;
      if (ra !== rb) return ra - rb;
      const ta = new Date(a.showtimeStart).getTime();
      const tb = new Date(b.showtimeStart).getTime();
      const na = Number.isNaN(ta) ? 0 : ta;
      const nb = Number.isNaN(tb) ? 0 : tb;
      return nb - na;
    });
  }

  getStatusText(status: number): string {
    switch (status) {
      case 1:
        return 'Aktif';
      case 2:
        return 'İptal Edildi';
      case 3:
        return 'Geçmiş';
      default:
        return 'Bilinmiyor';
    }
  }

  getStatusClass(status: number): string {
    switch (status) {
      case 1:
        return 'status-active';
      case 2:
        return 'status-canceled';
      case 3:
        return 'status-past';
      default:
        return 'status-unknown';
    }
  }

  formatShowtime(iso: string): string {
    if (!iso?.trim()) return '—';
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return iso;
    return new Intl.DateTimeFormat('tr-TR', {
      dateStyle: 'medium',
      timeStyle: 'short'
    }).format(d);
  }

  formatPrice(price: number): string {
    return `${Math.round(Number(price))} TL`;
  }

  barcodePattern(id: number): string {
    const w = 48;
    let s = '';
    let seed = id * 7919 + 13;
    for (let i = 0; i < w; i++) {
      seed = (seed * 1103515245 + 12345) & 0x7fffffff;
      const thick = seed % 5 === 0;
      s += thick ? '█' : seed % 2 === 0 ? '▌' : '▏';
    }
    return s;
  }

  trackTicket = (_: number, t: MyTicketDto) => t.id;
}
