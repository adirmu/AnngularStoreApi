import { HttpClient, HttpHeaders } from '@angular/common/http'; 
import { Component,Injectable } from '@angular/core';


export interface result{
  name:string,
  clone_url:string,
  owner:{avatar_url:string}
}

@Injectable() 
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})

export class AppComponent {
  private headers =()=> new HttpHeaders({
    'Access-Control-Allow-Origin': '*',
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${this.auth_token}`
  })
  private static search_api:string = 'https://localhost:44307/api/UserAPI/search?search=';
  private static login:string = 'https://localhost:44307/api/UserAPI/login?username='; // &pass=
  private static addToFav_api:string = 'https://localhost:44307/api/UserAPI/AddToFav?name=';//&url=
  auth_token:string ='';

  results:result[]=[];
  favorites:result[]=[];

  //helper:
  private debounce:any;//used to prevent calling the api to quickly.
  
  constructor(private http: HttpClient) { } 

  loginFunction=(username:string,password:string)=>{
    this.http.get([AppComponent.login,username,'&pass=',password].join(""), {headers: this.headers() }).subscribe(this.loginORregister);
  }

  registerFunction=(username:string,password:string)=>{
    this.http.get([AppComponent.login,username,'&pass=',password,'&register=true'].join(""), {headers: this.headers() }).subscribe(this.loginORregister);
  }

  loginORregister=(response:any)=>{
    this.auth_token = response;
    let parsed = this.parseJwt(response);
    this.updateFromToken(parsed.favorites);
  }

  updateFromToken =(favorites:any[])=>{
    this.favorites = favorites.map((f:any)=>{
      return {
        name:f.Name,
        clone_url:f.URL,
        owner:{avatar_url:f.Avatar}
      }
    });
  }

  parseJwt=(token:string)=> {
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(window.atob(base64).split('').map((c)=>  '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join(''));

    return JSON.parse(jsonPayload);
  }

  doSearch=(newSearch:string, oldSearch:string)=>{
    clearTimeout(this.debounce);//cancels the last timer, since we pressed another button
    this.debounce = setTimeout(()=>{//starts a timer, to wait between key presses. (prevents useless api calls.)
      this.results = [];
      
      this.http.get(AppComponent.search_api + newSearch, {headers: this.headers() }).subscribe((response:any)=>{
        JSON.parse(response).items.map((item:result)=>{
          this.results.push({
            name:item.name,
            clone_url:item.clone_url,
            owner:{
              avatar_url:item.owner.avatar_url
            }
          });
        })
      });
    }, 1000);
  }

  get favorite(){
    return this.addToFav.bind(this);
  }

  addToFav(item:result){
    if (this.favorites.includes(item))
      return;
    let userdata = this;
    this.http.get([AppComponent.addToFav_api,item.name,"&url=",item.clone_url,"&avatar=",item.owner.avatar_url].join(""), {headers: this.headers() }).subscribe((response:any)=>{
      if(!response){
        this.auth_token = '';
        return;
      }
      let parsed = JSON.parse(response);
      this.updateFromToken(parsed.favorites);
      userdata.auth_token = parsed.jwt;
    });
  }
}
