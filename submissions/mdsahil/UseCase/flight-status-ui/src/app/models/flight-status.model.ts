export enum FlightStatusEnum {
  OnTime = 0,
  Delayed = 1,
  Cancelled = 2,
  Diverted = 3,
  Unknown = 4
}

export interface FlightStatusResult {
  flightNumber: string;
  status: FlightStatusEnum;
  scheduledDeparture: string;
  actualDeparture?: string;
  scheduledArrival: string;
  actualArrival?: string;
  terminal?: string;
  gate?: string;
  delayReason?: string;
  lastUpdatedUtc: string;
}
