import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';

import { IDropdownSettings, MultiSelectComponent } from 'ng-multiselect-dropdown';
import { PagingParameters } from 'src/app/models/book-parameter.model';
import { Filters } from '../../../models/filters.model';
import { BookService, FilterParameter } from '../../../services/book.service';

@Component({
  selector: 'app-book-header',
  templateUrl: './book-header.component.html',
  styleUrls: ['./book-header.component.css']
})
export class BookHeaderComponent implements OnInit {
  @ViewChild("bookHeaderForm") public bookHeaderForm!: NgForm;
  @ViewChild("dropdown") public dropdown!: MultiSelectComponent;
  @Output() public filtersChanged = new EventEmitter<Filters>();
  public dropdownList: any[] = [];
  public dropdownSettings: IDropdownSettings = {};
  public textValueMap = new Map([
    ['Title', 'title'],
    ['Author', 'author'],
    ['Keyword', 'descriptionKeyword'],
    ['Publisher', 'publisher'],
    ['Genre', 'genre'],
  ]);

  constructor(private bookService: BookService) { }

  ngOnInit(): void {
    this.dropdownList = [
      { item_id: 1, item_text: 'Title' },
      { item_id: 2, item_text: 'Author' },
      { item_id: 3, item_text: 'Keyword' },
      { item_id: 4, item_text: 'Publisher' },
      { item_id: 5, item_text: 'Genre' }
    ];
    this.dropdownSettings = {
      idField: 'item_id',
      textField: 'item_text',
    };
  }

  onChange() {            
    let filter = this.bookHeaderForm.value['filter'];
    let filterParameters = this.getFilterParameters();
    if (!filter) {
      this.filtersChanged.emit({filter, filterParameters});
      return;
    }
    if (filter.length < 3) {
      return;
    }
    this.filtersChanged.emit({filter, filterParameters});
  }

  onSubmit() {
    let filter = this.bookHeaderForm.value['filter'];
    let filterParameters = this.getFilterParameters();
    this.filtersChanged.emit({filter, filterParameters});
  }

  private getFilterParameters() {
    let filterParameters: FilterParameter[] = this.dropdown.selectedItems
      .map(item => {
        let parameter = this.textValueMap.get(item.text.toString());
        if (!parameter) {
          parameter = 'title';
        }
        return <FilterParameter>parameter;
      });
    if (filterParameters.length === 0) {
      filterParameters.push('title');
    }
    return filterParameters;
  }
}