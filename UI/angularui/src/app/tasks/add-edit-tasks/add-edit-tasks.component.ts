declare var require:any;

import { Component, OnInit, Input } from '@angular/core';
import { CronService } from 'src/app/cron.service';

@Component({
  selector: 'app-add-edit-tasks',
  templateUrl: './add-edit-tasks.component.html',
  styleUrls: ['./add-edit-tasks.component.css']
})
export class AddEditTasksComponent implements OnInit {

  constructor(private service:CronService) { }

  @Input() cron:any;
  Id:string | undefined;
  Name:string | undefined;
  Description:string | undefined;
  Url:string | undefined;
  CronExpression:string | undefined;
  IsActive:boolean=false;

  ngOnInit(): void {
      this.Id=this.cron.Id,
      this.Name=this.cron.Name,
      this.Description=this.cron.Description,
      this.Url=this.cron.Url,
      this.CronExpression=this.cron.CronExpression,
      this.IsActive=this.cron.IsActive
  }

  addCronTask(){    
    if (this.ValidateFields()){
      var val={
        Id: this.Id,
        Name: this.Name,
        Url: this.Url,
        Description: this.Description,
        CronExpression: this.CronExpression,
        IsActive: this.IsActive ? 1 : 0
      };
      this.service.addCron(val).subscribe(res=> {
        alert(res.toString());
      });
    }
    else{
      
    }
  }

  UpdateCronTask(){
    var val={
      Id: this.Id,
      Name: this.Name,
      Description: this.Description,
      Url: this.Url,
      CronExpression: this.CronExpression,
      IsActive: this.IsActive ? 1 : 0
    };
    this.service.updateCron(val).subscribe(res=> {
      alert(res.toString());
    });
  }

  ValidateFields(): boolean {
    var cronValidator = require('cron-expression-validator');

    if(cronValidator.isValidCronExpression(this.CronExpression)){
      alert('CronExpression is not valid.');
      return false;
    }
    
    return true;
  }

}
