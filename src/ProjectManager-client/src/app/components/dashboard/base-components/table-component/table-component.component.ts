import { SelectionModel } from '@angular/cdk/collections';
import { AfterViewInit, Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { threadId } from 'worker_threads';

@Component({
  selector: 'app-table-component',
  templateUrl: './table-component.component.html',
  styleUrls: ['./table-component.component.css']
})
export class TableComponentComponent implements OnInit, AfterViewInit, OnChanges {
  @Input() data: any;
  @Input() columns: any[];
  @Input() columnNames: any[];
  @Input() isSelectable: boolean;

  displayedColumns: string[];
  displayedColumnNames: string[];

  dataSource: MatTableDataSource<any>;
  selection = new SelectionModel<any>(true, []);

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTable) table: MatTable<any>;

  constructor() { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.data.previousValue) {
      this.data = changes.data.currentValue;
      this.renderTable();
    }
  }

  ngAfterViewInit(): void {    
    this.paginatorConfig(this.paginator);

    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }  

  ngOnInit(): void {     
    this.renderTable();   
  }  

  renderTable(){
    this.displayedColumns = this.columns; 
    this.displayedColumnNames = this.columnNames;
      
    if (this.isSelectable && !this.displayedColumns.includes('select')) {
      this.displayedColumns.unshift('select');
    }
    this.dataSource = new MatTableDataSource(this.data); 
  }

  paginatorConfig(paginator: MatPaginator){
    paginator._intl.nextPageLabel = 'Próximo';
    paginator._intl.previousPageLabel = 'Anterior';
    paginator._intl.itemsPerPageLabel = 'Itens por página';
    paginator._intl.getRangeLabel = (page: number, pageSize: number, length: number) => {
      const start = page * pageSize + 1;
      const end = (page + 1) * pageSize;
      return `${start} - ${end} de ${length}`;
    }
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;

    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;

    return numSelected === numRows;
  }

  masterToggle() {
    this.isAllSelected() ? this.selection.clear() : this.dataSource.data.forEach(row => this.selection.select(row));
  }

  checkboxLabel(row?: any) {
    if (!row) {
      return `${this.isAllSelected() ? 'Selecionar' : 'Remover' } tudo`;
    }
    return `${this.selection.isSelected(row) ? 'Remover' : 'Selecionar'}`;
  }
}
