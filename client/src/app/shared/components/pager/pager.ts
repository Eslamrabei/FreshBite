import { Component, input, output, signal } from '@angular/core';
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
  totalCount = input<number>();
  pageSize = input<number>();
  pageChanged = output<number>();
  pageIndex = 1;



  onPagerChange(event: any) {
    this.pageChanged.emit(event.page)
  }

}
