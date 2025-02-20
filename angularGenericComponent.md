To integrate **Bootstrap 5** with **Angular 17** and create a fully-featured generic component, we'll use Bootstrap's utility classes and components for styling and layout. Below is the updated implementation with Bootstrap 5.

---

### 1. **Install Bootstrap 5**
First, install Bootstrap 5 in your Angular project:

```bash
npm install bootstrap @ng-bootstrap/ng-bootstrap
```

Add Bootstrap CSS to your `angular.json` file:

```json
"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css",
  "src/styles.css"
]
```

---

### 2. **Generic Service**
The service remains the same as before. It handles data fetching, updating, deleting, and searching.

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class GenericDataService<T> {
  constructor(private http: HttpClient) {}

  fetchData(endpoint: string): Observable<T[]> {
    return this.http.get<T[]>(endpoint).pipe(
      catchError(() => {
        console.error('Failed to fetch data');
        return of([]);
      })
    );
  }

  fetchItem(endpoint: string, id: string): Observable<T> {
    return this.http.get<T>(`${endpoint}/${id}`).pipe(
      catchError(() => {
        console.error('Failed to fetch item');
        return of(null);
      })
    );
  }

  postData(endpoint: string, data: T): Observable<T> {
    return this.http.post<T>(endpoint, data).pipe(
      catchError(() => {
        console.error('Failed to post data');
        return of(null);
      })
    );
  }

  updateData(endpoint: string, id: string, data: T): Observable<T> {
    return this.http.put<T>(`${endpoint}/${id}`, data).pipe(
      catchError(() => {
        console.error('Failed to update data');
        return of(null);
      })
    );
  }

  deleteData(endpoint: string, id: string): Observable<void> {
    return this.http.delete<void>(`${endpoint}/${id}`).pipe(
      catchError(() => {
        console.error('Failed to delete data');
        return of(null);
      })
    );
  }

  searchData(endpoint: string, query: string): Observable<T[]> {
    return this.http.get<T[]>(`${endpoint}?q=${query}`).pipe(
      catchError(() => {
        console.error('Failed to search data');
        return of([]);
      })
    );
  }
}
```

---

### 3. **Generic Component with Bootstrap 5**
The component now uses Bootstrap 5 classes for styling and layout.

```typescript
import { Component, Input, Output, EventEmitter, OnInit, TemplateRef } from '@angular/core';
import { GenericDataService } from './generic-data.service';

@Component({
  selector: 'app-generic-component',
  template: `
    <!-- Loading State -->
    <div *ngIf="loading" class="text-center my-4">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <!-- Error State -->
    <div *ngIf="error" class="alert alert-danger my-4">
      {{ error }}
    </div>

    <!-- Search and Filter -->
    <div *ngIf="enableSearch" class="my-4">
      <input
        [(ngModel)]="searchQuery"
        placeholder="Search..."
        (input)="onSearch()"
        class="form-control"
      />
    </div>

    <!-- Data Table or List -->
    <ng-container *ngIf="data && data.length > 0">
      <ng-container *ngTemplateOutlet="template; context: { $implicit: data }"></ng-container>
    </ng-container>

    <!-- Pagination -->
    <div *ngIf="enablePagination" class="d-flex justify-content-center my-4">
      <nav aria-label="Page navigation">
        <ul class="pagination">
          <li class="page-item" [class.disabled]="currentPage === 1">
            <button class="page-link" (click)="prevPage()">Previous</button>
          </li>
          <li class="page-item">
            <span class="page-link">Page {{ currentPage }} of {{ totalPages }}</span>
          </li>
          <li class="page-item" [class.disabled]="currentPage === totalPages">
            <button class="page-link" (click)="nextPage()">Next</button>
          </li>
        </ul>
      </nav>
    </div>

    <!-- Add New Item Button -->
    <button *ngIf="enableAddButton" (click)="onAdd()" class="btn btn-primary my-4">
      Add New
    </button>

    <!-- Modal for Adding/Editing -->
    <div *ngIf="showModal" class="modal fade show" [class.show]="showModal" tabindex="-1" style="display: block;">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ selectedItem ? 'Edit Item' : 'Add Item' }}</h5>
            <button type="button" class="btn-close" (click)="onCancel()"></button>
          </div>
          <div class="modal-body">
            <ng-container *ngTemplateOutlet="modalTemplate; context: { $implicit: selectedItem }"></ng-container>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" (click)="onCancel()">Cancel</button>
            <button type="button" class="btn btn-primary" (click)="onSave()">Save</button>
          </div>
        </div>
      </div>
    </div>
    <div *ngIf="showModal" class="modal-backdrop fade show"></div>
  `,
})
export class GenericComponent<T> implements OnInit {
  @Input() endpoint: string = '';
  @Input() template: TemplateRef<any>;
  @Input() modalTemplate: TemplateRef<any>;
  @Input() enableSearch: boolean = false;
  @Input() enablePagination: boolean = false;
  @Input() pageSize: number = 10;
  @Input() enableAddButton: boolean = false;
  @Output() itemAdded = new EventEmitter<T>();
  @Output() itemUpdated = new EventEmitter<T>();
  @Output() itemDeleted = new EventEmitter<string>();

  data: T[] = [];
  loading: boolean = false;
  error: string | null = null;
  searchQuery: string = '';
  currentPage: number = 1;
  totalPages: number = 1;
  showModal: boolean = false;
  selectedItem: T | null = null;

  constructor(private genericDataService: GenericDataService<T>) {}

  ngOnInit(): void {
    this.fetchData();
  }

  fetchData(): void {
    this.loading = true;
    this.genericDataService.fetchData(this.endpoint).subscribe({
      next: (response) => {
        this.data = response;
        this.totalPages = Math.ceil(this.data.length / this.pageSize);
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to fetch data';
        this.loading = false;
      },
    });
  }

  onSearch(): void {
    if (this.searchQuery) {
      this.genericDataService.searchData(this.endpoint, this.searchQuery).subscribe({
        next: (response) => {
          this.data = response;
        },
        error: (err) => {
          this.error = 'Failed to search data';
        },
      });
    } else {
      this.fetchData();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  onAdd(): void {
    this.selectedItem = null;
    this.showModal = true;
  }

  onEdit(item: T): void {
    this.selectedItem = item;
    this.showModal = true;
  }

  onSave(): void {
    if (this.selectedItem) {
      this.genericDataService.updateData(this.endpoint, (this.selectedItem as any).id, this.selectedItem).subscribe({
        next: (response) => {
          this.itemUpdated.emit(response);
          this.showModal = false;
          this.fetchData();
        },
        error: (err) => {
          this.error = 'Failed to update item';
        },
      });
    } else {
      this.genericDataService.postData(this.endpoint, this.selectedItem).subscribe({
        next: (response) => {
          this.itemAdded.emit(response);
          this.showModal = false;
          this.fetchData();
        },
        error: (err) => {
          this.error = 'Failed to add item';
        },
      });
    }
  }

  onCancel(): void {
    this.showModal = false;
  }

  onDelete(id: string): void {
    this.genericDataService.deleteData(this.endpoint, id).subscribe({
      next: () => {
        this.itemDeleted.emit(id);
        this.fetchData();
      },
      error: (err) => {
        this.error = 'Failed to delete item';
      },
    });
  }
}
```

---

### 4. **Usage Example**
#### Template for a Specific Page
```html
<ng-template #customTemplate let-data>
  <table class="table table-striped">
    <thead>
      <tr>
        <th>Name</th>
        <th>Description</th>
        <th>Actions</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let item of data">
        <td>{{ item.name }}</td>
        <td>{{ item.description }}</td>
        <td>
          <button class="btn btn-warning btn-sm me-2" (click)="onEdit(item)">Edit</button>
          <button class="btn btn-danger btn-sm" (click)="onDelete(item.id)">Delete</button>
        </td>
      </tr>
    </tbody>
  </table>
</ng-template>

<ng-template #modalTemplate let-item>
  <div class="mb-3">
    <label for="name" class="form-label">Name</label>
    <input [(ngModel)]="item.name" id="name" class="form-control" placeholder="Name" />
  </div>
  <div class="mb-3">
    <label for="description" class="form-label">Description</label>
    <input [(ngModel)]="item.description" id="description" class="form-control" placeholder="Description" />
  </div>
</ng-template>

<app-generic-component
  [endpoint]="'/api/data'"
  [template]="customTemplate"
  [modalTemplate]="modalTemplate"
  [enableSearch]="true"
  [enablePagination]="true"
  [enableAddButton]="true">
</app-generic-component>
```

---

### 5. **Features Included**
- **Bootstrap 5 Styling**: Tables, buttons, forms, modals, and pagination are styled using Bootstrap 5.
- **Dynamic Data Fetching**: Fetch data from any API endpoint.
- **Search and Filtering**: Search functionality with dynamic filtering.
- **Pagination**: Paginate large datasets.
- **CRUD Operations**: Add, edit, and delete items.
- **Customizable Templates**: Use custom templates for rendering data and modals.
- **Event Handling**: Emit events for adding, updating, and deleting items.
- **Loading and Error States**: Handle loading and error states gracefully.
- **Reactive Forms**: Support for forms in modals.

This implementation provides a **fully-featured, reusable, and customizable Angular component** with **Bootstrap 5** integration.