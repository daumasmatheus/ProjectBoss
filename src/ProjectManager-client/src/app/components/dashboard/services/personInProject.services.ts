import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from 'src/app/services/base.service';
import { environment } from 'src/environments/environment';
import { catchError } from 'rxjs/operators';

@Injectable()
export class PersonInProjectServices extends BaseService{
    constructor(private httpClient: HttpClient) { super(); }
    
    getAssignedProjects(personId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/PersonInProject/GetAssignedProjects?personId=${personId}`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    getAllProjects(): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/PersonInProject/GetAllProjects`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    getProjectAttendants(projectId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/PersonInProject/GetProjectAttendants?projectId=${projectId}`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    removeProjectAttendants(parameters: any): Observable<any> {
        return this.httpClient
                    .post(environment.apiBaseUrl + `api/PersonInProject/RemoveProjectAttendants`, parameters, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    includeNewAttendant(parameters: any): Observable<any>{
        return this.httpClient
                    .post(environment.apiBaseUrl + 'api/PersonInProject/AddProjectAttendant', parameters, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }
}