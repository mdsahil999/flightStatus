import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { FlightStatusResult } from '../models/flight-status.model';

export interface AppState {
  data: FlightStatusResult | null;
  loading: boolean;
  error: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class FlightStatusService {
  // Use a relative or configurable URL in production. 
  // For this exercise, assuming API is hosted on localhost:5001 or we will proxy it.
  private apiUrl = 'http://localhost:5253/flights/status';

  state = signal<AppState>({ data: null, loading: false, error: null });

  constructor(private http: HttpClient) { }

  searchFlight(flightNumber: string, date: string) {
    this.state.set({ data: null, loading: true, error: null });

    this.http.get<FlightStatusResult>(`${this.apiUrl}?flightNumber=${flightNumber}&date=${date}`).subscribe({
      next: (res) => this.state.set({ data: res, loading: false, error: null }),
      error: (err: HttpErrorResponse) => {
        const msg = err.status === 400 ? 'Flight number and date are required.' : 'Unable to retrieve flight details. The system encountered a timeout.';
        this.state.set({ data: null, loading: false, error: msg });
      }
    });
  }
}
