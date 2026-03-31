export type MovieId = string;

export interface Movie {
  id: MovieId;
  title: string;
  imageUrl: string | null;
}

export interface MovieDetail extends Movie {
  description: string | null;
  duration: string | null;
  director: string | null;
  genre: string | null;
  releaseDate: string | null;
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
    Id?: string;
    Title?: string;
    ImageUrl?: string | null;
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
  ReleaseDate?: string | null;
  DurationInMinutes?: number;
  ReleaseYear?: number;
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

