import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { catchError } from "rxjs/operators";

import { BaseService } from "src/app/services/base.service";
import { environment } from "src/environments/environment";
import { NewTaskModel } from "../models/new-task.model";
import { TaskModel } from "../models/task.model";

@Injectable()
export class TaskServices extends BaseService {
    constructor(private httpClient: HttpClient) { super(); }

    newTask(newTask: NewTaskModel): Observable<any> {
        return this.httpClient
                    .post(environment.apiBaseUrl + 'api/Task/NewTask', newTask, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }    
    
    getTasksByUserId(userId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Task/GetTasksByUserId?userId=${userId}`, this.GetJsonAuthHeader())
                    .pipe(                        
                        catchError(this.ServiceError)
                    );
    } 
    
    setTaskComplete(taskId: string): Observable<any> {
        return this.httpClient
                    .post(environment.apiBaseUrl + 'api/Task/SetTaskComplete', taskId, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    toggleTaskStatus(parameters: any): Observable<any> {
        return this.httpClient
                    .post(environment.apiBaseUrl + 'api/Task/ToggleTaskStatus', parameters, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    )
    }

    editTask(task: TaskModel): Observable<any> {
        return this.httpClient
                    .patch(environment.apiBaseUrl + 'api/Task/EditTask', task, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    removeTask(taskId: string): Observable<any> {
        return this.httpClient
                    .post(environment.apiBaseUrl + 'api/Task/RemoveTask', taskId, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    downloadTasksXlsl(userId: string, restrictData: boolean): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Task/ExportTasksAsXlsx?userId=${userId}&restrictData=${restrictData}`, { responseType: 'blob', headers: this.GetAuthHeader() })
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    downloadTasksPdf(userId: string, restrictData: boolean): Observable<any> {
        let headers = new HttpHeaders().set('Accept', 'application/pdf')
                                       .set('Authorization', `Bearer ${this.localStorage.getUserToken()}`);

        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Task/ExportTasksAsPdf?userId=${userId}&restrictData=${restrictData}`, { responseType: 'blob', headers: headers })
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    getAllActiveTasks(): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Task/GetAllActiveTasks`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    )
    }
    
    getAllTasksByAuthorId(authorId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Task/GetTasksByAuthor?authorId=${authorId}`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    )
    }

    getAllTasksByAttendantId(attendantId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Task/GetTasksByAttendant?attendantId=${attendantId}`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    )
    }

    getTasksByProjectId(projectId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Task/GetTasksByProjectId?projectId=${projectId}`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }    
}