import { Component } from '@angular/core';
import { CommonModule } from '@angular/common'; // Important for ngIf, etc.

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.scss']
})
export class ContactComponent {

  onSubmit(event: Event) {
    event.preventDefault();
    console.log('Form Submitted');
    alert('Message sent successfully! (This is a demo)');
  }
}
