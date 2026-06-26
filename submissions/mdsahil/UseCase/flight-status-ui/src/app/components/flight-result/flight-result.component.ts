import { Component, Input } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FlightStatusResult } from '../../models/flight-status.model';

@Component({
  selector: 'app-flight-result',
  imports: [CommonModule, DatePipe],
  templateUrl: './flight-result.component.html',
  styleUrls: ['./flight-result.component.css']
})
export class FlightResultComponent {
  @Input({ required: true }) data!: FlightStatusResult;

  getStatusClass(status: string): string {
    switch (status) {
      case 'OnTime': return 'status-green';
      case 'Delayed': return 'status-amber';
      case 'Cancelled':
      case 'Diverted': return 'status-red';
      default: return 'status-grey';
    }
  }

  getDisplayStatus(status: string): string {
    switch (status) {
      case 'OnTime': return 'ON TIME';
      case 'Delayed': return 'DELAYED';
      case 'Cancelled': return 'CANCELLED';
      case 'Diverted': return 'DIVERTED';
      default: return 'UNKNOWN';
    }
  }
}
