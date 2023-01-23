import { Component,Injectable, Input } from '@angular/core';
import { result } from '../app.component';


@Injectable() 
@Component({
  selector: 'img-button',
  templateUrl: './img-button.html',
  styleUrls: ['./img-button.less']
})

export class ImgButtonComponent{
  //helper:
  private clickTimeout:any = null;//used to prevent the "add to favorite" from opening the link.
  
  @Input() item!: result;
  @Input() favorite: boolean=false;
  @Input() addToFav!: Function;//calls the parent function

   goToLink(url:string){
     if (this.clickTimeout) {
       this.clickTimeout=false;
       return;
     }
     window.open(url, "_blank");
   }
}