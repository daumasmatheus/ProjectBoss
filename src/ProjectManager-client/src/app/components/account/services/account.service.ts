import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { User } from '../models/user.model';
import { environment } from 'src/environments/environment';
import { BaseService } from 'src/app/services/base.service';
import { ExternalUser } from '../models/externalUser.model';
import { ChangePasswordModel } from '../../dashboard/models/changePassword.model';

@Injectable()
export class AccountService extends BaseService{
    constructor(private httpClient: HttpClient) { super(); }
    
    registerUser(user: User): Observable<any> {
        return this.httpClient
                    .post(environment.apiBaseUrl + 'api/auth/register', user, this.GetJsonHeader())
                    .pipe(
                        catchError(this.ServiceError));
    }

    login(user: User): Observable<any> {
        return this.httpClient
                        .post(environment.apiBaseUrl + 'api/auth/login', user, this.GetJsonHeader())
                        .pipe(
                            catchError(this.ServiceError));
    }

    externalAuth(user: ExternalUser): Observable<any> {
        return this.httpClient
                        .post(environment.apiBaseUrl + 'api/auth/external-auth', user, this.GetJsonHeader())
                        .pipe(
                            catchError(this.ServiceError));
    }

    changePassword(data: ChangePasswordModel): Observable<any> {
        return this.httpClient
                    .post(environment.apiBaseUrl + 'api/auth/ChangePassword', data)
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }
}