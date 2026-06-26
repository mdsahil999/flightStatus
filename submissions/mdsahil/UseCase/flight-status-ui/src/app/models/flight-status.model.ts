export type UnifiedStatus = 'OnTime' | 'Delayed' | 'Cancelled' | 'Diverted' | 'Unknown';

export interface FlightStatusResult {
  flightNumber: string;
  status: UnifiedStatus;
  scheduledDeparture: string;
  actualDeparture?: string;
  scheduledArrival: string;
  actualArrival?: string;
  terminal?: string;
  gate?: string;
  delayReason?: string;
  lastUpdatedUtc: string;
}
