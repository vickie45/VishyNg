import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  ngOnInit() {
    // Function to clean up leftover modal-backdrop elements
    const cleanupModalBackdrops = () => {
      const backdrops = document.querySelectorAll('.modal-backdrop');
      backdrops.forEach((backdrop) => {
        backdrop.parentNode?.removeChild(backdrop);
      });

      // Ensure the body class and styles are reset
      document.body.classList.remove('modal-open');
      document.body.style.paddingRight = '0';
    };

    // Attach to the Bootstrap modal close event
    document.addEventListener('hidden.bs.modal', () => {
      cleanupModalBackdrops();
    });

    // Fallback: Periodically check and clean up backdrops
    setInterval(() => {
      cleanupModalBackdrops();
    }, 500);
  }
}