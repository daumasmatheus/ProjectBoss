import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { BaseService } from 'src/app/services/base.service';
import { catchError } from 'rxjs/operators';
import { PersonModel } from '../models/person.1.model';

@Injectable()
export class PersonServices extends BaseService {
    constructor(private httpClient: HttpClient) { super(); }

    getPersonData(personId: string): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + `api/Person/GetPersonData?personId=${personId}`, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    saveChanges(person: PersonModel): Observable<any> {
        return this.httpClient
                    .patch(environment.apiBaseUrl + 'api/Person/EditPerson', person, this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }

    getAll(): Observable<any> {
        return this.httpClient
                    .get(environment.apiBaseUrl + 'api/Person/GetAll', this.GetJsonAuthHeader())
                    .pipe(
                        catchError(this.ServiceError)
                    );
    }
    
}