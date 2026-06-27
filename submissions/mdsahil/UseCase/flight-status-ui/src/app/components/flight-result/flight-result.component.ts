import { Component, Input } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FlightStatusResult, FlightStatusEnum } from '../../models/flight-status.model';

@Component({
  selector: 'app-flight-result',
  imports: [CommonModule, DatePipe],
  templateUrl: './flight-result.component.html',
  styleUrls: ['./flight-result.component.css']
})
export class FlightResultComponent {
  @Input({ required: true }) data!: FlightStatusResult;
  FlightStatusEnum = FlightStatusEnum;

  getStatusClass(status: FlightStatusEnum): string {
    switch (status) {
      case FlightStatusEnum.OnTime: return 'status-green';
      case FlightStatusEnum.Delayed: return 'status-amber';
      case FlightStatusEnum.Cancelled:
      case FlightStatusEnum.Diverted: return 'status-red';
      default: return 'status-grey';
    }
  }

  getDisplayStatus(status: FlightStatusEnum): string {
    switch (status) {
      case FlightStatusEnum.OnTime: return 'ON TIME';
      case FlightStatusEnum.Delayed: return 'DELAYED';
      case FlightStatusEnum.Cancelled: return 'CANCELLED';
      case FlightStatusEnum.Diverted: return 'DIVERTED';
      default: return 'UNKNOWN';
    }
  }
}
