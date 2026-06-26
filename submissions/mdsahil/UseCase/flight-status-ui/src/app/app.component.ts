import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlightSearchComponent } from './components/flight-search/flight-search.component';
import { FlightResultComponent } from './components/flight-result/flight-result.component';
import { FlightStatusService } from './services/flight-status.service';

@Component({
  selector: 'app-root',
  imports: [CommonModule, FlightSearchComponent, FlightResultComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  private flightService = inject(FlightStatusService);
  state = this.flightService.state;

  onSearch(event: { flightNumber: string, date: string }) {
    this.flightService.searchFlight(event.flightNumber, event.date);
  }
}
