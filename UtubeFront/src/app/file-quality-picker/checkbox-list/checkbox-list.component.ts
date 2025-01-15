//   templateUrl: './checkbox-list.component.html',
//   styleUrl: './checkbox-list.component.css'

// checkbox-list.component.ts
import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common'; // Import CommonModule

@Component({
  imports: [CommonModule], 
  selector: 'app-checkbox-list',
  standalone: true,
  template: `
    <h2>{{ listTitle }}</h2>
    <ul>
      <li *ngFor="let item of items; let i = index">
        <input type="checkbox" [id]="getCheckboxId(i)" [checked]="isSelected(item)" (change)="onCheckboxChange(item, $event)" />
        <label [for]="getCheckboxId(i)">{{ item }}</label>
      </li>
    </ul>
  `,
  styles: []
})
export class CheckboxListComponent implements OnInit, OnChanges{
  @Input() items: string[] = []; 
  @Input() listTitle: string = "Select Items";
  selectedItems: string[] = []; 
  @Output() selectedItemsChange = new EventEmitter<string[]>();

  ngOnInit(): void {
    // Initialize selectedItems if needed (e.g., from local storage)
    // Example: this.selectedItems = JSON.parse(localStorage.getItem('selectedItems') || '[]');
      console.log("ngOnInit");
  }

  ngOnChanges(changes: SimpleChanges): void {
      //if (changes['items'] && changes['items'].currentValue) {
        //this.selectedItems = this.selectedItems.filter(item => changes['items'].currentValue.includes(item));

        console.log("ngOnChanges");
      //}
  }

  getCheckboxId(index: number): string {
    return `chb-${this.listTitle.replaceAll(' ', '') }-${index}`; 
  }

  onCheckboxChange(item: string, event: Event) {
    const isChecked = (event.target as HTMLInputElement).checked;

    let newSelectedItems: string[];

    if (isChecked) {
      newSelectedItems = [...this.selectedItems, item];
    } else {
      newSelectedItems = this.selectedItems.filter(i => i !== item);
    }

    console.log("checkbox component newSelectedItems   " + newSelectedItems);

    this.selectedItems = newSelectedItems;
    this.selectedItemsChange.emit(newSelectedItems);

    console.log("checkbox component this.selectedItems   " + this.selectedItems);
    console.log("checkbox component this.items   " + this.items);
  }

  isSelected(item: string): boolean {
    return this.selectedItems.includes(item);
  }
}