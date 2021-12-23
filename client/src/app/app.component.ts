import { Component, OnInit } from '@angular/core';
import {HttpClient} from '@angular/common/http'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'client';
  appUsers:any
  constructor(public http:HttpClient) {    
    
  }
  ngOnInit(): void {
   this.http.get("https://localhost:5001/api/Users").subscribe(Response=>{ 
     this.appUsers=Response;
   })
  } 
}
