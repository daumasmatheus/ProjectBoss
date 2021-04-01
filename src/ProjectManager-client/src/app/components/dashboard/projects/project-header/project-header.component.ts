import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LocalStorageUtils } from 'src/app/helpers/localstorage';
import { PersonInProjectSimpleModel } from '../../models/personInProject.model';
import { ProjectDataModel, ProjectModel } from '../../models/project.model';

@Component({
  selector: 'app-project-header',
  templateUrl: './project-header.component.html',
  styleUrls: ['./project-header.component.css']
})
export class ProjectHeaderComponent implements OnInit {
  localStorageUtils = new LocalStorageUtils();

  @Output() projectIdEvt = new EventEmitter<any>();
  
  projects: PersonInProjectSimpleModel[]; 
  isCommonUser: boolean = false; 

  constructor(private activatedRoute: ActivatedRoute) {
    this.isCommonUser = this.localStorageUtils.checkUserClaim("User");
    this.getProjectsFromRouteResolver();
  }  

  ngOnInit(): void {
  }  

  sendProjectId(projectId: any){    
    this.projectIdEvt.emit(projectId);
  }

  getProjectsFromRouteResolver(){
    this.activatedRoute.data.subscribe(
      (data: {projects: PersonInProjectSimpleModel[]}) => {
        this.projects = data.projects;        
      }
    )
  }
}