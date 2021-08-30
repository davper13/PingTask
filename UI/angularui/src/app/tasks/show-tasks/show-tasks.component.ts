import { Component, OnInit } from '@angular/core';
import { CronService } from 'src/app/cron.service';

@Component({
  selector: 'app-show-tasks',
  templateUrl: './show-tasks.component.html',
  styleUrls: ['./show-tasks.component.css']
})
export class ShowTasksComponent implements OnInit {

  constructor(private service:CronService) { }

  tasksList:any=[];

  ModalTitle:string | undefined;
  ActivateAddEditCronComp:boolean=false;
  cron:any;


  ngOnInit(): void {
    this.refreshTasksList();
  }

  refreshTasksList(){
    this.service.getCronList().subscribe(data =>{
      this.tasksList=data;
    });
  }

  addClick(){
    this.cron={
      Id:0,
      Name:"",
      Description:"",
      Url:"",
      CronExpression:"",
      IsActive:false
    }
    this.ModalTitle="Add Cron Expression";
    this.ActivateAddEditCronComp=true;
  }

  editClick(item:any){
    this.cron=item;
    this.ModalTitle="Edit Cron Expression";
    this.ActivateAddEditCronComp=true;
  }

  deleteClick(item:any){
    if(confirm('Are you sure??')){
      this.service.deleteCron(item.Id).subscribe(data=>{
        alert(data.toString());
        this.refreshTasksList();
      })
    }
  }

  closeClick(){
    this.ActivateAddEditCronComp=false;
    this.refreshTasksList();
  }
}
