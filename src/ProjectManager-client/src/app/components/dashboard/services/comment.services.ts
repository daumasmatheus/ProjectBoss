import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommentModel } from '../models/comment.model';
import { environment } from 'src/environments/environment';
import { BaseService } from 'src/app/services/base.service';
import { catchError } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({providedIn: 'root'})
export class CommentServices extends BaseService {
    constructor(private httpClient: HttpClient) { super(); }

    submitComment(comment: CommentModel): Observable<any>{
        return this.httpClient
                    .post(environment.apiBaseUrl + 'api/Comment/NewComment', comment, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }    

    getComments(taskId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Comment/GetTaskComments?taskId=${taskId}`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }
}