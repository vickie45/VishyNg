To achieve the functionality described, you can create an Angular component that includes a form for the user to input the report criteria. Upon submission, the form data will be sent to an API to fetch the report data. The data will then be displayed in a table, and a download button will allow the user to download the data as an Excel file.

Here's a step-by-step guide to implementing this:

1. **Set up the Angular project**: If you haven't already, create a new Angular project using the Angular CLI.

    ```bash
    ng new report-app
    cd report-app
    ```

2. **Generate the report component**: Generate a new component for handling the report.

    ```bash
    ng generate component report
    ```

3. **Install necessary packages**: Install `xlsx` for Excel file generation and `axios` or Angular's `HttpClient` for making HTTP requests.

    ```bash
    npm install xlsx @angular/common@latest @angular/compiler@latest @angular/core@latest @angular/forms@latest @angular/platform-browser@latest @angular/platform-browser-dynamic@latest @angular/router@latest rxjs@latest zone.js@latest
    ```

4. **Implement the report component**:

    Update the `report.component.ts`:

    ```typescript
    import { Component } from '@angular/core';
    import { HttpClient } from '@angular/common/http';
    import * as XLSX from 'xlsx';

    @Component({
      selector: 'app-report',
      templateUrl: './report.component.html',
      styleUrls: ['./report.component.css']
    })
    export class ReportComponent {
      reportForm: any = {
        dateFrom: '',
        dateTo: '',
        appliedTo: '',
        reportType: '',
        representType: ''
      };
      reportData: any[] = [];
      displayedColumns: string[] = []; // Add your columns here

      constructor(private http: HttpClient) {}

      onSubmit() {
        this.http.post('YOUR_API_ENDPOINT', this.reportForm)
          .subscribe((data: any) => {
            this.reportData = data;
            this.displayedColumns = Object.keys(this.reportData[0]);
          }, error => {
            console.error('There was an error!', error);
          });
      }

      downloadExcel() {
        const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.reportData);
        const wb: XLSX.WorkBook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');
        XLSX.writeFile(wb, 'Report.xlsx');
      }
    }
    ```

    Update the `report.component.html`:

    ```html
    <form (ngSubmit)="onSubmit()">
      <input type="date" [(ngModel)]="reportForm.dateFrom" name="dateFrom" required>
      <input type="date" [(ngModel)]="reportForm.dateTo" name="dateTo" required>
      <input type="text" [(ngModel)]="reportForm.appliedTo" name="appliedTo" required>
      <select [(ngModel)]="reportForm.reportType" name="reportType" required>
        <option value="Type1">Type 1</option>
        <option value="Type2">Type 2</option>
      </select>
      <select [(ngModel)]="reportForm.representType" name="representType" required>
        <option value="Represent1">Represent 1</option>
        <option value="Represent2">Represent 2</option>
      </select>
      <button type="submit">Get Report</button>
    </form>

    <table *ngIf="reportData.length > 0">
      <thead>
        <tr>
          <th *ngFor="let column of displayedColumns">{{column}}</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let row of reportData">
          <td *ngFor="let column of displayedColumns">{{row[column]}}</td>
        </tr>
      </tbody>
    </table>

    <button *ngIf="reportData.length > 0" (click)="downloadExcel()">Download Excel</button>
    ```

    Update the `report.component.css` for any styling you might need.

5. **Import FormsModule and HttpClientModule**: Ensure that `FormsModule` and `HttpClientModule` are imported in your `app.module.ts`.

    ```typescript
    import { BrowserModule } from '@angular/platform-browser';
    import { NgModule } from '@angular/core';
    import { FormsModule } from '@angular/forms';
    import { HttpClientModule } from '@angular/common/http';

    import { AppComponent } from './app.component';
    import { ReportComponent } from './report/report.component';

    @NgModule({
      declarations: [
        AppComponent,
        ReportComponent
      ],
      imports: [
        BrowserModule,
        FormsModule,
        HttpClientModule
      ],
      providers: [],
      bootstrap: [AppComponent]
    })
    export class AppModule { }
    ```

6. **Run your application**: Finally, run your Angular application to see it in action.

    ```bash
    ng serve
    ```

This setup provides a basic structure for generating reports based on user input, displaying the data in a table, and offering an option to download the data as an Excel file. Adjust the API endpoint and data handling as per your specific requirements.