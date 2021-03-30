import { CurrencyPipe } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ChartData, ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { Color, Label, SingleDataSet } from 'ng2-charts';
import { forkJoin } from 'rxjs';
import { LocalStorageUtils } from 'src/app/helpers/localstorage';
import { StatisticsServices } from '../services/statistics.services';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.css']
})
export class StatisticsComponent implements OnInit {
  localStorageUtils = new LocalStorageUtils();

  @Input() projectId: string;
  @Input() isProjectDashboard: boolean;

  loadingData: boolean = false;
  noChartData: boolean = false;

  getOpenAndOnGoingTasksByPersonInProjectData: any[];
  getTasksStatusByProjectData: any[];
  getNewAndClosedTasksByDateByProjectData: any[];

  createdUsersLabels: Label[];
  createdUsers: ChartDataSets[];

  totalTasks: SingleDataSet = [];
  totalTasksLabels: Label[] = [];

  createdTasksLabels: Label[];
  createdTasks: ChartDataSets[];

  concludedTasksLabels: Label[];
  concludedTasks: ChartDataSets[];

  totalProjects: SingleDataSet = [];
  totalProjectsLabels: Label[] = [];

  createdProjectsLabels: Label[];
  createdProjects: ChartDataSets[];

  concludedProjectsLabels: Label[];
  concludedProjects: ChartDataSets[];

  pieColors: any[] = [
    {
      backgroundColor: [
        'rgba(3, 117, 209)',
        'rgba(3, 209, 81)'
      ]
    }
  ]

  userBarColor: Color[] = [
    { backgroundColor: 'rgba(3, 117, 209)' }
  ]

  tasksBarColors: Color[] = [
    { backgroundColor: 'rgba(86, 3, 209)' }
  ]

  projectBarColors: Color[] = [
    { backgroundColor: 'rgba(209, 60, 3 )' }
  ]

  view: any[] = [600, 250];  

  isProjectManagerOrUser: boolean = false;  

  constructor(private statisticsServices: StatisticsServices) {
    this.isProjectManagerOrUser = this.localStorageUtils.checkUserClaim("CommonUser") || this.localStorageUtils.checkUserClaim("ProjectManager");
  }

  ngOnInit(): void {
    if (this.isProjectDashboard) {
      this.getChartsDataForProjectDashboard();
    } else {
      this.getDataForAdminDashboard();
    }  
  }

  onResize(event) {
    this.view = [event.target.innerWidth / 1.35, 400];
  }

  getChartsDataForProjectDashboard(){
    this.loadingData = true;

    forkJoin([
      this.statisticsServices.getOpenAndOnGoingTasksByPersonInProject(this.projectId),
      this.statisticsServices.getTasksStatusByProject(this.projectId),
      this.statisticsServices.getNewAndClosedTasksByDateByProject(this.projectId)
    ]).subscribe(
      (results: any) => {    
        console.log(results);
        
        this.getOpenAndOnGoingTasksByPersonInProjectData = results[0];
        this.getTasksStatusByProjectData = results[1];
        this.getNewAndClosedTasksByDateByProjectData = results[2];

        this.loadingData = false;
      }, error => {
        console.error(error);
        this.loadingData = false;
      }
    );
  }  

  getDataForAdminDashboard() {
    this.loadingData = true;

    forkJoin([
      this.statisticsServices.getCreatedUsers(),
      this.statisticsServices.getTotalCreatedTasksByDate(),
      this.statisticsServices.getTotalConcludedTasksByDate(),
      this.statisticsServices.getTotalCreatedProjectsByDate(),
      this.statisticsServices.getTotalConcludedProjectsByDate()
    ]).subscribe(
      (result: any[]) => {
        if (!result[1] || !result[2] || !result[3] || !result[4]) {
          this.noChartData = true;
          this.loadingData = false;
          return;
        }

        this.setCreatedUsersChartData(result[0]);

        this.setTotalTasksChartData(result[1], result[2]);
        this.setCreatedTasksChartData(result[1]);
        this.setConcludedTasksChartData(result[2]);

        this.setTotalProjectsChartData(result[3], result[4]);
        this.setCreatedProjectsChartData(result[3]);
        this.setConcludedProjectsChartData(result[4]);
        
        this.loadingData = false;
      }, error => {
        console.error(error);
        this.loadingData = false;
      }
    )
  }

  setCreatedUsersChartData(data: any) {
    this.createdUsersLabels = data.series.map(el => el.name);
    this.createdUsers = [
      { data: data.series.map(el => el.value), label: data.name }
    ];
  }

  setTotalTasksChartData(created: any, concluded: any) {
    if (created.series && created.series.length > 0) {
      this.totalTasksLabels.push('Criadas');
      this.totalTasks.push(created.series.map(el => el.value).reduce((acc, curVal) => acc + curVal));
    }
    if (concluded.series && concluded.series.length > 0) {
      this.totalTasksLabels.push('Concluídas');
      this.totalTasks.push(concluded.series.map(el => el.value).reduce((acc, curVal) => acc + curVal));
    }
  }

  setCreatedTasksChartData(data: any){
    this.createdTasksLabels = data.series.map(el => el.name);
    this.createdTasks = [
      { data: data.series.map(el => el.value), label: data.name }
    ];
  }

  setConcludedTasksChartData(data: any){
    this.concludedTasksLabels = data.series.map(el => el.name);
    this.concludedTasks = [
      { data: data.series.map(el => el.value), label: data.name }
    ];
  }

  setTotalProjectsChartData(created: any, concluded: any) {
    if (created.series && created.series.length > 0) {
      this.totalProjectsLabels.push('Criados');
      this.totalProjects.push(created.series.map(el => el.value).reduce((acc, curVal) => acc + curVal));
    }
    if (concluded.series && concluded.series.length > 0) {
      this.totalProjectsLabels.push('Concluídos');
      this.totalProjects.push(concluded.series.map(el => el.value).reduce((acc, curVal) => acc + curVal));
    }
  }

  setCreatedProjectsChartData(data: any){
    this.createdProjectsLabels = data.series.map(el => el.name);
    this.createdProjects = [
      { data: data.series.map(el => el.value), label: data.name }
    ];
  }

  setConcludedProjectsChartData(data: any){
    this.concludedProjectsLabels = data.series.map(el => el.name);
    this.concludedProjects = [
      { data: data.series.map(el => el.value), label: data.name }
    ];
  }
}