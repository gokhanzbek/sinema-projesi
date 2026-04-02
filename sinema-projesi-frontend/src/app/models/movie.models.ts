export type MovieId = string;

export interface Movie {
  id: MovieId;
  title: string;
  imageUrl: string | null;
  /** API’den gelir; IMDb alanı backend’de hazır. */
  imdbRating?: number | null;
}

/** Ana sayfa /api/Movies/filter isteği */
export interface HomeMovieFilters {
  eventDate: string;
  sort: string;
  minPrice: number | null;
  maxPrice: number | null;
  genres: string[];
  timeSlots: string[];
}

export interface MovieDetail extends Movie {
  description: string | null;
  duration: string | null;
  director: string | null;
  /** Birleştirilmiş kategori metni (görünüm için). */
  genre: string | null;
  categories: string[] | null;
  categoryIds?: number[];
  releaseDate: string | null;
  /** API süre (dakika) — admin formu için */
  durationMinutes?: number;
  releaseYear?: number;
  /** IMDb tt… kimliği — admin düzenleme */
  imdbId?: string | null;
}

export interface ShowtimeSlot {
  showtimeId: number;
  time: string;
  price: number;
}

export interface HallShowtimes {
  hallName: string;
  times: ShowtimeSlot[];
}

export interface DateShowtimes {
  dateTitle: string;
  halls: HallShowtimes[];
}

export interface GetAllMoviesResponse {
  totalCount: number;
  movies: Array<{
    id?: string;
    title?: string;
    imageUrl?: string | null;
    categories?: string[] | null;
    Id?: string;
    Title?: string;
    ImageUrl?: string | null;
    Categories?: string[] | null;
  }>;
}

export interface GetMovieByIdResponse {
  id?: number | string;
  title?: string;
  imageUrl?: string | null;
  description?: string | null;
  duration?: string | null;
  director?: string | null;
  genre?: string | null;
  categories?: string[] | null;
  categoryIds?: number[];
  releaseDate?: string | null;
  durationInMinutes?: number;
  releaseYear?: number;
  Id?: number | string;
  Title?: string;
  ImageUrl?: string | null;
  Description?: string | null;
  Duration?: string | null;
  Director?: string | null;
  Genre?: string | null;
  Categories?: string[] | null;
  CategoryIds?: number[];
  ReleaseDate?: string | null;
  DurationInMinutes?: number;
  ReleaseYear?: number;
  imdbRating?: number | null;
  ImdbRating?: number | null;
  imdbId?: string | null;
  ImdbId?: string | null;
}

export interface GetShowtimesByMovieResponse {
  showtimes?: Array<{
    dateTitle?: string;
    DateTitle?: string;
    halls?: Array<{
      hallName?: string;
      HallName?: string;
      times?: Array<{
        showtimeId?: number;
        ShowtimeId?: number;
        time?: string;
        Time?: string;
        price?: number;
        Price?: number;
      }>;
      Times?: Array<{
        showtimeId?: number;
        ShowtimeId?: number;
        time?: string;
        Time?: string;
        price?: number;
        Price?: number;
      }>;
    }>;
    Halls?: Array<{
      hallName?: string;
      HallName?: string;
      times?: Array<{
        showtimeId?: number;
        ShowtimeId?: number;
        time?: string;
        Time?: string;
        price?: number;
        Price?: number;
      }>;
      Times?: Array<{
        showtimeId?: number;
        ShowtimeId?: number;
        time?: string;
        Time?: string;
        price?: number;
        Price?: number;
      }>;
    }>;
  }>;
}

