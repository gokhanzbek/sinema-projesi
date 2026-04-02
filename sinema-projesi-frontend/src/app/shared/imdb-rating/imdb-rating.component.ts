import { CommonModule } from '@angular/common';
import { Component, HostBinding, Input } from '@angular/core';

@Component({
  selector: 'app-imdb-rating',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      class="imdb-rating"
      *ngIf="show"
      role="img"
      [attr.aria-label]="'IMDb puanı ' + formatted"
    >
      <span class="imdb-rating__star" aria-hidden="true">★</span>
      <span class="imdb-rating__value">{{ formatted }}</span>
    </div>
  `,
  styles: `
    :host {
      display: block;
    }

    .imdb-rating {
      display: inline-flex;
      align-items: center;
      gap: 5px;
      margin-top: 4px;
    }

    .imdb-rating__star {
      color: #f5c518;
      font-size: 1.05em;
      line-height: 1;
      text-shadow: 0 1px 2px rgba(0, 0, 0, 0.45);
    }

    .imdb-rating__value {
      font-weight: 800;
      font-variant-numeric: tabular-nums;
      color: inherit;
      line-height: 1;
      letter-spacing: -0.02em;
    }

    :host(.imdb-rating-host--large) .imdb-rating {
      margin-top: 0;
      margin-bottom: 14px;
      gap: 7px;
    }

    :host(.imdb-rating-host--large) .imdb-rating__star {
      font-size: 1.35rem;
    }

    :host(.imdb-rating-host--large) .imdb-rating__value {
      font-size: 1.15rem;
    }
  `
})
export class ImdbRatingComponent {
  @Input() value: number | null | undefined = null;

  /** Film detay gibi daha büyük gösterim */
  @Input() large = false;

  @HostBinding('class.imdb-rating-host--large')
  get largeHost(): boolean {
    return this.large;
  }

  get show(): boolean {
    return this.value != null && !Number.isNaN(Number(this.value));
  }

  get formatted(): string {
    const v = this.value;
    if (v == null || Number.isNaN(Number(v))) return '';
    return Number(v).toLocaleString('tr-TR', {
      minimumFractionDigits: 1,
      maximumFractionDigits: 1
    });
  }
}
