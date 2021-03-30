import { Component, OnInit } from '@angular/core';
import { LocalStorageUtils } from 'src/app/helpers/localstorage';
import { PersonOverviewModel } from '../models/personOverview.model';
import { TaskModel } from '../models/task.model';
import { StatisticsServices } from '../services/statistics.services';
import { TaskServices } from '../services/task.services';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.css']
})
export class OverviewComponent implements OnInit {
  localStorageUtils = new LocalStorageUtils();

  isProjectManagerOrUser: boolean = false;  

  statisticsData: PersonOverviewModel;

  constructor(private statisticsServices: StatisticsServices) { 
    this.getPersonStatisticsData();

    this.isProjectManagerOrUser = this.localStorageUtils.checkUserClaim("CommonUser") || this.localStorageUtils.checkUserClaim("ProjectManager");
  }

  ngOnInit(): void {
  }

  getPersonStatisticsData(){
    if (this.isProjectManagerOrUser) {
      let personId = this.localStorageUtils.getUser().personId;

      this.statisticsServices.getPersonOverviewStatistics(personId).subscribe(
        (resp: PersonOverviewModel) => {
          this.statisticsData = resp;
        }, error => {
          console.error(error);
        }
      )
    }
  }
}