import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-login',
  templateUrl: './app-login.component.html',
  styleUrls: ['./app-login.component.less']
})
export class AppLoginComponent{
  @Input() loginFunction!:Function;
  @Input() registerFunction!:Function;
  username:string="";
  password:string="";
  register:boolean=false;
}
