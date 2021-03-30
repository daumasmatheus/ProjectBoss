import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NewProject } from '../models/new-project.model';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { BaseService } from 'src/app/services/base.service';
import { catchError } from 'rxjs/operators';
import { ProjectDataModel } from '../models/project.model';

@Injectable()
export class ProjectServices extends BaseService {
    constructor(private httpClient: HttpClient) { super(); }
    
    saveProject(project: NewProject): Observable<any> {
        return this.httpClient
                    .post(environment.apiBaseUrl + 'api/Project/NewProject', project, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    getProjectDataById(projectId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Project/GetProjectDataById?projectId=${projectId}`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    editProjectData(projectData: ProjectDataModel): Observable<any> {
        return this.httpClient
                    .patch(environment.apiBaseUrl + 'api/Project/EditProjectData', projectData, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    exportProjectAsPdf(projectId: string): Observable<any> {
        let headers = new HttpHeaders().set('Accept', 'application/pdf')
                                       .set('Authorization', `Bearer ${this.localStorage.getUserToken()}`);

        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Project/ExportProjectAsPdf?projectId=${projectId}`, { responseType: 'blob', headers: headers })
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    exportProjectAsXlsx(projectId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Project/ExportProjectAsXlsx?projectId=${projectId}`, { responseType: 'blob', headers:  this.GetAuthHeader()})
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }    

    getAllProjectsDataForDropdown(){
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Project/GetAllProjectsForDropdown`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }
}