import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CronService {
  readonly APIUrl="http://localhost:31401/api"

  constructor(private http:HttpClient) {}

  getCronList():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/cron');
  }

  addCron(val:any){
    return this.http.post(this.APIUrl+'/cron',val);
  }

  updateCron(val:any){
    return this.http.put(this.APIUrl+'/cron',val);
  }

  deleteCron(val:any){
    return this.http.delete(this.APIUrl+'/cron/' + val);
  }
}
