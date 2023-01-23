import { Component,Injectable, Input } from '@angular/core';

@Injectable() 
@Component({
  selector: 'app-search',
  templateUrl: './search-component.html',
  styleUrls: ['./search-component.less']
})

export class SearchComponent{
    @Input() doSearch! : Function;
    search: string="";
    loading:boolean=false;

    prepSearch(newSearch:string){
        if (newSearch == "" || newSearch == this.search)
            return;
        this.doSearch(newSearch, this.search);
        this.search = newSearch;
    }
}