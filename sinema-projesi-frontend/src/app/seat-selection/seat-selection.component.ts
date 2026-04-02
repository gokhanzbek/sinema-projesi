import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError, finalize, take } from 'rxjs/operators';
import { TicketService } from '../services/ticket.service';

export type SeatStatus = 'available' | 'occupied';

export interface SeatSlot {
  id: string;
  row: string;
  col: number;
  status: SeatStatus;
}

@Component({
  selector: 'app-seat-selection',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './seat-selection.component.html',
  styleUrl: './seat-selection.component.scss'
})
export class SeatSelectionComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly ticketService = inject(TicketService);

  sessionId = 0;
  price = 0;
  isLoading = true;
  loadError: string | null = null;
  purchaseMessage: string | null = null;
  purchasing = false;

  allSeats: SeatSlot[] = [];
  seatRows: SeatSlot[][] = [];
  colLabels: number[] = [];

  readonly selected = new Set<string>();

  ngOnInit(): void {
    this.route.paramMap.pipe(take(1)).subscribe((params) => {
      const rawId = params.get('sessionId') || params.get('id');
      const id = rawId ? parseInt(rawId, 10) : NaN;

      if (Number.isNaN(id) || id <= 0) {
        this.isLoading = false;
        this.loadError = 'Geçersiz seans bağlantısı.';
        return;
      }

      this.sessionId = id;
      this.fetchApiData(this.sessionId);
    });
  }

  private fetchApiData(id: number): void {
    this.isLoading = true;
    this.loadError = null;

    forkJoin({
      showtime: this.ticketService.getShowtimeById(id).pipe(
        catchError((err) => {
          console.error('Seans detayı çekilirken hata:', err);
          return of(null);
        })
      ),
      seats: this.ticketService.getOccupiedSeats(id).pipe(
        catchError((err) => {
          console.error('Koltuklar çekilirken hata:', err);
          return of(null);
        })
      )
    })
      .pipe(
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe((result) => {
        if (!result.showtime || !result.showtime.id) {
          this.loadError = 'Seans bilgisi alınamadı.';
          return;
        }

        const rowCount = result.showtime.hallRowCount;
        const colCount = result.showtime.hallColumnCount;
        if (rowCount < 1 || colCount < 1) {
          this.loadError = 'Salon koltuk düzeni tanımlı değil.';
          return;
        }

        this.price = Number(result.showtime.price);

        this.allSeats = SeatSelectionComponent.generateAllSeats(rowCount, colCount);
        this.colLabels = Array.from({ length: colCount }, (_, i) => i + 1);
        this.seatRows = SeatSelectionComponent.groupSeatsByRow(this.allSeats);

        if (result.seats) {
          const seatsData: unknown = result.seats;
          const seatsArray = Array.isArray(seatsData)
            ? seatsData
            : ((seatsData as { occupiedSeats?: string[] }).occupiedSeats ?? []);
          const occupiedCanon = new Set(
            (seatsArray as string[]).map((s) => this.canonSeat(s))
          );

          for (const seat of this.allSeats) {
            seat.status = occupiedCanon.has(this.canonSeat(seat.id))
              ? 'occupied'
              : 'available';
          }
        }
      });
  }

  private static generateAllSeats(rowCount: number, columnCount: number): SeatSlot[] {
    const rows = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'.slice(0, rowCount).split('');
    const seats: SeatSlot[] = [];
    for (const row of rows) {
      for (let col = 1; col <= columnCount; col++) {
        seats.push({
          id: `${row}${col}`,
          row,
          col,
          status: 'available'
        });
      }
    }
    return seats;
  }

  private static groupSeatsByRow(seats: SeatSlot[]): SeatSlot[][] {
    const order = [...new Set(seats.map((s) => s.row))];
    return order.map((r) => seats.filter((s) => s.row === r));
  }

  private canonSeat(raw: string): string {
    return raw.trim().toUpperCase().replace(/-/g, '').replace(/\s+/g, '');
  }

  isSelected(seat: SeatSlot): boolean {
    return this.selected.has(seat.id);
  }

  toggleSeat(seat: SeatSlot): void {
    if (seat.status === 'occupied') return;
    if (this.selected.has(seat.id)) {
      this.selected.delete(seat.id);
    } else {
      this.selected.add(seat.id);
    }
  }

  get selectedList(): string[] {
    return [...this.selected].sort((a, b) => a.localeCompare(b, 'tr', { numeric: true }));
  }

  get totalAmount(): number {
    return this.selected.size * this.price;
  }

  formatPrice(n: number): string {
    return `${Math.round(n)} TL`;
  }

  trackCol = (_: number, c: number) => c;
  trackRow = (_: number, r: SeatSlot[]) => r[0]?.row ?? '';
  trackSeat = (_: number, s: SeatSlot) => s.id;

  checkout(): void {
    if (!this.selected.size || this.purchasing) return;
    this.purchasing = true;
    this.purchaseMessage = null;
    this.ticketService
      .createTicket({
        showtimeId: this.sessionId,
        seatNumbers: this.selectedList,
        price: this.price
      })
      .subscribe({
        next: (r) => {
          this.purchasing = false;
          if (r.succeeded) {
            this.router.navigate(['/checkout-success']);
            return;
          }
          this.purchaseMessage = r.message ?? 'İşlem tamamlanamadı.';
        },
        error: () => {
          this.purchasing = false;
          this.purchaseMessage = 'Sunucuya ulaşılamadı veya oturum süresi doldu.';
        }
      });
  }
}
