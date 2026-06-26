import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-flight-search',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './flight-search.component.html',
  styleUrls: ['./flight-search.component.css']
})
export class FlightSearchComponent {
  searchForm!: FormGroup;
  @Output() search = new EventEmitter<{ flightNumber: string, date: string }>();

  constructor(private fb: FormBuilder) {
    this.searchForm = this.fb.group({
      flightNumber: ['', [Validators.required, Validators.pattern(/^[A-Za-z0-9]+$/)]],
      date: ['', Validators.required]
    });

  }

  onSubmit() {
    if (this.searchForm.valid) {
      const { flightNumber, date } = this.searchForm.value;
      if (flightNumber && date) {
        this.search.emit({ flightNumber, date });
      }
    }
  }
}
