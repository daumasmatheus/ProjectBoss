import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { catchError } from 'rxjs/operators';
import { BaseService } from 'src/app/services/base.service';

@Injectable({providedIn: 'root'})
export class StatisticsServices extends BaseService {
    constructor(private httpClient: HttpClient) { super(); }

    getOpenAndOnGoingTasksByPersonInProject(projectId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Statistics/GetOpenAndOnGoingTasksByPersonInProject?projectId=${projectId}`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    getTasksStatusByProject(projectId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Statistics/GetTasksStatusByProject?projectId=${projectId}`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    getNewAndClosedTasksByDateByProject(projectId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Statistics/GetNewAndClosedTasksByDateByProject?projectId=${projectId}`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    getPersonOverviewStatistics(personId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Statistics/GetPersonOverviewStatistics?personId=${personId}`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }    

    getCreatedUsers(): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Statistics/GetCreatedUsers`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }    

    getTotalCreatedTasksByDate(): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Statistics/GetTotalCreatedTasksByDate`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }   

    getTotalConcludedTasksByDate(): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Statistics/GetTotalConcludedTasksByDate`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }   

    getTotalCreatedProjectsByDate(): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Statistics/GetTotalCreatedProjectsByDate`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }   

    getTotalConcludedProjectsByDate(): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Statistics/GetTotalConcludedProjectsByDate`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }   
}