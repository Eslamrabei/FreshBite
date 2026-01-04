import { Component, input, model, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { Pagination, Product } from '../../models/pagination';

@Component({
  selector: 'app-pager',
  imports: [PaginationModule, FormsModule],
  templateUrl: './pager.html',
  styleUrl: './pager.scss',
})
export class Pager {
  totalCount = input<number>(0);
  pageSize = input<number>(10);
  pageChanged = output<number>();
  pageIndex = model<number>(1);



  onPagerChange(event: any) {
    this.pageIndex.set(event.page);
    this.pageChanged.emit(event.page);
  }

}
