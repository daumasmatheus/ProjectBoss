import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from 'src/app/services/base.service';
import { environment } from 'src/environments/environment';
import { catchError } from 'rxjs/operators';
import { UserDataModel } from '../models/userData.model';

@Injectable({providedIn: 'root'})
export class UsersServices extends BaseService {
    constructor(private httpClient: HttpClient) { super(); }
    
    getUsers(): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/User/GetUsers`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    getUserById(userId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/User/GetUserById?userId=${userId}`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    editUser(userData: UserDataModel): Observable<any> {
        return this.httpClient
                    .post(environment.apiBaseUrl + `api/User/EditUser`, userData, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    resetUserPassword(userId: string): Observable<any> {
        return this.httpClient
                    .post(environment.apiBaseUrl + `api/User/ResetUserPassword`, userId, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    downloadDataXlsl(): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/User/ExportUsersAsXlsx`, { responseType: 'blob', headers: this.GetAuthHeader() })
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    downloadDataPdf(): Observable<any> {
        let headers = new HttpHeaders().set('Accept', 'application/pdf')
                                       .set('Authorization', `Bearer ${this.localStorage.getUserToken()}`);

        return this.httpClient
                    .get(environment.apiBaseUrl + `api/User/ExportUsersAsPdf`, { responseType: 'blob', headers: headers })
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    getRoles(): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/User/GetRoles`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }
}